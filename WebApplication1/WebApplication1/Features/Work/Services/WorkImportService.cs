using System.Diagnostics;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkImportService : IWorkImportService
{
    private readonly AppDbContext _context;

    public WorkImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkImportBatchDto>> GetBatchPageAsync(WorkImportBatchQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkImportBatches.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.FileName.Contains(query.Keyword));

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (query.StartDate.HasValue)
            q = q.Where(x => x.CreatedAt >= query.StartDate.Value.ToDateTime(TimeOnly.MinValue));

        if (query.EndDate.HasValue)
            q = q.Where(x => x.CreatedAt <= query.EndDate.Value.ToDateTime(TimeOnly.MaxValue));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new WorkImportBatchDto
            {
                Id = x.Id,
                FileName = x.FileName,
                FileSize = x.FileSize,
                TotalRows = x.TotalRows,
                SuccessRows = x.SuccessRows,
                FailedRows = x.FailedRows,
                SkippedRows = x.SkippedRows,
                DuplicateRows = x.DuplicateRows,
                Status = x.Status,
                ImportStrategy = x.ImportStrategy,
                StartedAt = x.StartedAt,
                FinishedAt = x.FinishedAt,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<WorkImportBatchDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkImportPreviewResultDto> PreviewWorkLogAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportPreviewResultDto();
        var previewItems = new List<WorkImportPreviewDto>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        var projects = await _context.WorkProjects.ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            var rowNumber = (int)row.RowNumber();
            var workDate = row.Cell(1).GetString();
            var projectName = row.Cell(2).GetString();
            var content = row.Cell(3).GetString();
            var hoursStr = row.Cell(4).GetString();
            var remark = row.Cell(5).GetString();

            var preview = new WorkImportPreviewDto
            {
                RowNumber = rowNumber,
                WorkDate = workDate,
                ProjectName = projectName,
                OriginalContent = content,
                TotalHours = decimal.TryParse(hoursStr, out var h) ? h : null,
                Remark = remark,
                ValidationStatus = WorkImportValidationStatus.Valid
            };

            var validationErrors = new List<string>();

            if (!DateOnly.TryParse(workDate, out _))
            {
                validationErrors.Add("日期格式错误");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }

            if (string.IsNullOrWhiteSpace(projectName))
            {
                validationErrors.Add("项目名称不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }
            else if (!projects.Any(p => p.ProjectName == projectName))
            {
                validationErrors.Add("项目不存在");
                preview.ValidationStatus = WorkImportValidationStatus.Warning;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                validationErrors.Add("工作内容不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }

            if (!decimal.TryParse(hoursStr, out _) && !string.IsNullOrWhiteSpace(hoursStr))
            {
                validationErrors.Add("工时格式错误");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }

            preview.ErrorMessage = string.Join("; ", validationErrors);
            previewItems.Add(preview);
        }

        result.Items = previewItems;
        result.TotalRows = previewItems.Count;
        result.ValidRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Valid);
        result.WarningRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Warning);
        result.ErrorRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Error);

        return result;
    }

    public async Task<WorkImportConfirmResultDto> ExecuteWorkLogImportAsync(WorkImportConfirmDto input, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportConfirmResultDto();
        var stopwatch = Stopwatch.StartNew();

        var batch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            FileName = "工作日志导入",
            Status = WorkImportStatus.Processing,
            ImportStrategy = input.ImportStrategy,
            StartedAt = DateTime.UtcNow
        };

        _context.WorkImportBatches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var projects = await _context.WorkProjects.ToListAsync(cancellationToken);

            var workLogs = new List<WorkLog>();

            result.SuccessRows = 0;
            result.FailedRows = 0;

            foreach (var item in input.Items)
            {
                if (item.ValidationStatus == WorkImportValidationStatus.Error)
                {
                    result.FailedRows++;
                    continue;
                }

                var project = projects.FirstOrDefault(p => p.ProjectName == item.ProjectName);
                var workDate = DateOnly.TryParse(item.WorkDate, out var d) ? d : DateOnly.FromDateTime(DateTime.Now);

                workLogs.Add(new WorkLog
                {
                    Id = Guid.NewGuid(),
                    WorkDate = workDate,
                    WeekDay = "",
                    ProjectId = project?.Id ?? Guid.Empty,
                    Title = item.OriginalContent?.Length > 50 ? item.OriginalContent[..50] : (item.OriginalContent ?? "导入的工作日志"),
                    OriginalContent = item.OriginalContent,
                    TotalHours = item.TotalHours ?? 0,
                    Status = WorkLogStatus.Normal,
                    SourceType = WorkLogSourceType.ExcelImport,
                    ImportBatchId = batch.Id,
                    CreatedAt = DateTime.UtcNow
                });

                result.SuccessRows++;
            }

            if (workLogs.Count > 0)
            {
                await _context.WorkLogs.AddRangeAsync(workLogs, cancellationToken);
            }

            batch.Status = WorkImportStatus.Completed;
            batch.TotalRows = result.SuccessRows + result.FailedRows;
            batch.SuccessRows = result.SuccessRows;
            batch.FailedRows = result.FailedRows;
        }
        catch (Exception ex)
        {
            batch.Status = WorkImportStatus.Failed;
            batch.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            batch.FinishedAt = DateTime.UtcNow;
            batch.TotalRows = result.SuccessRows + result.FailedRows;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public byte[] GenerateWorkLogTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("工作日志");

        var headers = new[] { "日期", "项目名称", "工作内容", "工时(小时)", "备注" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        worksheet.Cell(2, 1).Value = "2024-05-01";
        worksheet.Cell(2, 2).Value = "生产线升级项目";
        worksheet.Cell(2, 3).Value = "完成设备调试和参数优化";
        worksheet.Cell(2, 4).Value = 4;
        worksheet.Cell(2, 5).Value = "";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<WorkImportPreviewResultDto> PreviewProjectAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportPreviewResultDto();
        var previewItems = new List<WorkImportPreviewDto>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        var existingProjects = await _context.WorkProjects.ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            var rowNumber = (int)row.RowNumber();
            var projectName = row.Cell(1).GetString();
            var projectCode = row.Cell(2).GetString();
            var projectType = row.Cell(3).GetString();
            var customerName = row.Cell(4).GetString();
            var description = row.Cell(5).GetString();

            var preview = new WorkImportPreviewDto
            {
                RowNumber = rowNumber,
                ProjectName = projectName,
                DeviceNames = projectCode,
                TaskTypeNames = projectType,
                OriginalContent = description,
                Remark = customerName,
                ValidationStatus = WorkImportValidationStatus.Valid
            };

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                validationErrors.Add("项目名称不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }
            else if (existingProjects.Any(p => p.ProjectName == projectName))
            {
                validationErrors.Add("项目已存在");
                preview.ValidationStatus = WorkImportValidationStatus.Warning;
                preview.DuplicateStatus = 1;
            }

            preview.ErrorMessage = string.Join("; ", validationErrors);
            previewItems.Add(preview);
        }

        result.Items = previewItems;
        result.TotalRows = previewItems.Count;
        result.ValidRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Valid);
        result.WarningRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Warning);
        result.ErrorRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Error);
        result.DuplicateRows = previewItems.Count(x => x.DuplicateStatus == 1);

        return result;
    }

    public async Task<WorkImportConfirmResultDto> ExecuteProjectImportAsync(WorkImportConfirmDto input, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportConfirmResultDto();

        var batch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            FileName = "项目导入",
            Status = WorkImportStatus.Processing,
            ImportStrategy = input.ImportStrategy,
            StartedAt = DateTime.UtcNow
        };

        _context.WorkImportBatches.Add(batch);
        await _context.SaveChangesAsync();

        try
        {
            var existingProjects = await _context.WorkProjects.ToListAsync();
            var newProjects = new List<WorkProject>();

            foreach (var item in input.Items)
            {
                if (item.ValidationStatus == WorkImportValidationStatus.Error)
                {
                    result.FailedRows++;
                    continue;
                }

                if (item.DuplicateStatus == 1 && input.ImportStrategy == WorkImportStrategy.SkipDuplicate)
                {
                    result.SkippedRows++;
                    continue;
                }

                var projectType = item.TaskTypeNames?.ToLower() switch
                {
                    "内部" or "internal" => WorkProjectType.Internal,
                    "外部" or "external" => WorkProjectType.External,
                    "研发" or "randd" => WorkProjectType.RAndD,
                    "支持" or "support" => WorkProjectType.Support,
                    _ => WorkProjectType.Other
                };

                newProjects.Add(new WorkProject
                {
                    Id = Guid.NewGuid(),
                    ProjectName = item.ProjectName,
                    ProjectCode = item.DeviceNames,
                    ProjectType = projectType,
                    CustomerName = item.Remark,
                    Description = item.OriginalContent,
                    Status = WorkProjectStatus.Active,
                    CreatedAt = DateTime.UtcNow
                });

                result.SuccessRows++;
            }

            if (newProjects.Count > 0)
            {
                await _context.WorkProjects.AddRangeAsync(newProjects);
            }

            batch.Status = WorkImportStatus.Completed;
            batch.TotalRows = result.SuccessRows + result.FailedRows + result.SkippedRows;
            batch.SuccessRows = result.SuccessRows;
            batch.FailedRows = result.FailedRows;
            batch.SkippedRows = result.SkippedRows;
        }
        catch (Exception ex)
        {
            batch.Status = WorkImportStatus.Failed;
            batch.ErrorMessage = ex.Message;
        }
        finally
        {
            batch.FinishedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return result;
    }

    public byte[] GenerateProjectTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("项目");

        var headers = new[] { "项目名称", "项目编号", "项目类型", "客户名称", "项目描述" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        worksheet.Cell(2, 1).Value = "生产线升级项目";
        worksheet.Cell(2, 2).Value = "PRJ-001";
        worksheet.Cell(2, 3).Value = "内部";
        worksheet.Cell(2, 4).Value = "内部";
        worksheet.Cell(2, 5).Value = "生产线自动化升级改造";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<WorkImportPreviewResultDto> PreviewDeviceAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportPreviewResultDto();
        var previewItems = new List<WorkImportPreviewDto>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        var existingDevices = await _context.WorkDevices.ToListAsync(cancellationToken);
        var projects = await _context.WorkProjects.ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            var rowNumber = (int)row.RowNumber();
            var deviceName = row.Cell(1).GetString();
            var deviceCode = row.Cell(2).GetString();
            var deviceType = row.Cell(3).GetString();
            var projectName = row.Cell(4).GetString();
            var description = row.Cell(5).GetString();

            var preview = new WorkImportPreviewDto
            {
                RowNumber = rowNumber,
                ProjectName = deviceName,
                DeviceNames = deviceCode,
                TaskTypeNames = deviceType,
                OriginalContent = description,
                Remark = projectName,
                ValidationStatus = WorkImportValidationStatus.Valid
            };

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(deviceName))
            {
                validationErrors.Add("设备名称不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }
            else if (existingDevices.Any(d => d.DeviceName == deviceName))
            {
                validationErrors.Add("设备已存在");
                preview.ValidationStatus = WorkImportValidationStatus.Warning;
                preview.DuplicateStatus = 1;
            }

            if (!string.IsNullOrWhiteSpace(projectName) && !projects.Any(p => p.ProjectName == projectName))
            {
                validationErrors.Add("项目不存在");
                preview.ValidationStatus = WorkImportValidationStatus.Warning;
            }

            preview.ErrorMessage = string.Join("; ", validationErrors);
            previewItems.Add(preview);
        }

        result.Items = previewItems;
        result.TotalRows = previewItems.Count;
        result.ValidRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Valid);
        result.WarningRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Warning);
        result.ErrorRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Error);
        result.DuplicateRows = previewItems.Count(x => x.DuplicateStatus == 1);

        return result;
    }

    public async Task<WorkImportConfirmResultDto> ExecuteDeviceImportAsync(WorkImportConfirmDto input, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportConfirmResultDto();

        var batch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            FileName = "设备导入",
            Status = WorkImportStatus.Processing,
            ImportStrategy = input.ImportStrategy,
            StartedAt = DateTime.UtcNow
        };

        _context.WorkImportBatches.Add(batch);
        await _context.SaveChangesAsync();

        try
        {
            var projects = await _context.WorkProjects.ToListAsync();
            var newDevices = new List<WorkDevice>();

            foreach (var item in input.Items)
            {
                if (item.ValidationStatus == WorkImportValidationStatus.Error)
                {
                    result.FailedRows++;
                    continue;
                }

                if (item.DuplicateStatus == 1 && input.ImportStrategy == WorkImportStrategy.SkipDuplicate)
                {
                    result.SkippedRows++;
                    continue;
                }

                var deviceType = item.TaskTypeNames?.ToLower() switch
                {
                    "生产线" or "productionline" => WorkDeviceType.ProductionLine,
                    "设备" or "equipment" => WorkDeviceType.Equipment,
                    "测试设备" or "testingdevice" => WorkDeviceType.TestingDevice,
                    _ => WorkDeviceType.Equipment
                };

                var project = projects.FirstOrDefault(p => p.ProjectName == item.Remark);

                newDevices.Add(new WorkDevice
                {
                    Id = Guid.NewGuid(),
                    DeviceName = item.ProjectName,
                    DeviceCode = item.DeviceNames,
                    DeviceType = deviceType,
                    ProjectId = project?.Id,
                    Description = item.OriginalContent,
                    Status = WorkDeviceStatus.Active,
                    CreatedAt = DateTime.UtcNow
                });

                result.SuccessRows++;
            }

            if (newDevices.Count > 0)
            {
                await _context.WorkDevices.AddRangeAsync(newDevices);
            }

            batch.Status = WorkImportStatus.Completed;
            batch.TotalRows = result.SuccessRows + result.FailedRows + result.SkippedRows;
            batch.SuccessRows = result.SuccessRows;
            batch.FailedRows = result.FailedRows;
            batch.SkippedRows = result.SkippedRows;
        }
        catch (Exception ex)
        {
            batch.Status = WorkImportStatus.Failed;
            batch.ErrorMessage = ex.Message;
        }
        finally
        {
            batch.FinishedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return result;
    }

    public byte[] GenerateDeviceTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("设备");

        var headers = new[] { "设备名称", "设备编号", "设备类型", "所属项目", "设备描述" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        worksheet.Cell(2, 1).Value = "A线体";
        worksheet.Cell(2, 2).Value = "DEVICE-A01";
        worksheet.Cell(2, 3).Value = "生产线";
        worksheet.Cell(2, 4).Value = "生产线升级项目";
        worksheet.Cell(2, 5).Value = "主生产线A";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<WorkImportPreviewResultDto> PreviewTaskTypeAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportPreviewResultDto();
        var previewItems = new List<WorkImportPreviewDto>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        var existingTypes = await _context.WorkTaskTypes.ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            var rowNumber = (int)row.RowNumber();
            var typeName = row.Cell(1).GetString();
            var typeCode = row.Cell(2).GetString();
            var description = row.Cell(3).GetString();

            var preview = new WorkImportPreviewDto
            {
                RowNumber = rowNumber,
                ProjectName = typeName,
                DeviceNames = typeCode,
                OriginalContent = description,
                ValidationStatus = WorkImportValidationStatus.Valid
            };

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(typeName))
            {
                validationErrors.Add("任务类型名称不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }
            else if (existingTypes.Any(t => t.TypeName == typeName))
            {
                validationErrors.Add("任务类型已存在");
                preview.ValidationStatus = WorkImportValidationStatus.Warning;
                preview.DuplicateStatus = 1;
            }

            preview.ErrorMessage = string.Join("; ", validationErrors);
            previewItems.Add(preview);
        }

        result.Items = previewItems;
        result.TotalRows = previewItems.Count;
        result.ValidRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Valid);
        result.WarningRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Warning);
        result.ErrorRows = previewItems.Count(x => x.ValidationStatus == WorkImportValidationStatus.Error);
        result.DuplicateRows = previewItems.Count(x => x.DuplicateStatus == 1);

        return result;
    }

    public async Task<WorkImportConfirmResultDto> ExecuteTaskTypeImportAsync(WorkImportConfirmDto input, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportConfirmResultDto();

        var batch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            FileName = "任务类型导入",
            Status = WorkImportStatus.Processing,
            ImportStrategy = input.ImportStrategy,
            StartedAt = DateTime.UtcNow
        };

        _context.WorkImportBatches.Add(batch);
        await _context.SaveChangesAsync();

        try
        {
            var newTypes = new List<WorkTaskType>();

            foreach (var item in input.Items)
            {
                if (item.ValidationStatus == WorkImportValidationStatus.Error)
                {
                    result.FailedRows++;
                    continue;
                }

                if (item.DuplicateStatus == 1 && input.ImportStrategy == WorkImportStrategy.SkipDuplicate)
                {
                    result.SkippedRows++;
                    continue;
                }

                newTypes.Add(new WorkTaskType
                {
                    Id = Guid.NewGuid(),
                    TypeName = item.ProjectName,
                    TypeCode = item.DeviceNames,
                    Description = item.OriginalContent,
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow
                });

                result.SuccessRows++;
            }

            if (newTypes.Count > 0)
            {
                await _context.WorkTaskTypes.AddRangeAsync(newTypes);
            }

            batch.Status = WorkImportStatus.Completed;
            batch.TotalRows = result.SuccessRows + result.FailedRows + result.SkippedRows;
            batch.SuccessRows = result.SuccessRows;
            batch.FailedRows = result.FailedRows;
            batch.SkippedRows = result.SkippedRows;
        }
        catch (Exception ex)
        {
            batch.Status = WorkImportStatus.Failed;
            batch.ErrorMessage = ex.Message;
        }
        finally
        {
            batch.FinishedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return result;
    }

    public byte[] GenerateTaskTypeTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("任务类型");

        var headers = new[] { "类型名称", "类型编号", "描述" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        worksheet.Cell(2, 1).Value = "设备调试";
        worksheet.Cell(2, 2).Value = "TT-001";
        worksheet.Cell(2, 3).Value = "设备调试和参数优化";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
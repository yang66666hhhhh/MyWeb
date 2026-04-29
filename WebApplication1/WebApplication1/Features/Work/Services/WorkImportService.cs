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
        {
            q = q.Where(x => x.FileName.Contains(query.Keyword));
        }

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

    public async Task<WorkImportPreviewResultDto> PreviewAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportPreviewResultDto();
        var previewItems = new List<WorkImportPreviewDto>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        var projects = await _context.WorkProjects.ToListAsync(cancellationToken);
        var taskTypes = await _context.WorkTaskTypes.ToListAsync(cancellationToken);
        var devices = await _context.WorkDevices.ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            var rowNumber = (int)row.RowNumber();
            var workDate = row.Cell(1).GetString();
            var projectName = row.Cell(2).GetString();
            var deviceName = row.Cell(3).GetString();
            var taskTypeName = row.Cell(4).GetString();
            var content = row.Cell(5).GetString();
            var hoursStr = row.Cell(6).GetString();
            var remark = row.Cell(7).GetString();

            var preview = new WorkImportPreviewDto
            {
                RowNumber = rowNumber,
                WorkDate = workDate,
                ProjectName = projectName,
                DeviceNames = deviceName,
                TaskTypeNames = taskTypeName,
                OriginalContent = content,
                TotalHours = decimal.TryParse(hoursStr, out var h) ? h : null,
                Remark = remark,
                ValidationStatus = WorkImportValidationStatus.Valid,
                DuplicateStatus = 0
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
            else
            {
                var project = projects.FirstOrDefault(p => p.ProjectName == projectName);
                if (project == null)
                {
                    validationErrors.Add("项目不存在");
                    preview.ValidationStatus = WorkImportValidationStatus.Warning;
                }
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                validationErrors.Add("工作内容不能为空");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }

            if (!decimal.TryParse(hoursStr, out _))
            {
                validationErrors.Add("工时格式错误");
                preview.ValidationStatus = WorkImportValidationStatus.Error;
            }

            var existingLog = await _context.WorkLogs
                .FirstOrDefaultAsync(x =>
                    x.WorkDate == DateOnly.Parse(workDate) &&
                    x.OriginalContent == content,
                    cancellationToken);

            if (existingLog != null)
            {
                preview.DuplicateStatus = 1;
                preview.DuplicateRows = 1;
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

    public async Task<WorkImportConfirmResultDto> ExecuteAsync(WorkImportConfirmDto input, CancellationToken cancellationToken = default)
    {
        var result = new WorkImportConfirmResultDto();
        var stopwatch = Stopwatch.StartNew();

        var batch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            Status = WorkImportStatus.Processing,
            ImportStrategy = input.ImportStrategy,
            StartedAt = DateTime.UtcNow
        };

        _context.WorkImportBatches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var projects = await _context.WorkProjects.ToListAsync(cancellationToken);
            var rows = await _context.WorkImportRows
                .Where(x => x.BatchId == batch.Id)
                .ToListAsync(cancellationToken);

            if (rows.Count == 0)
            {
                result.FailedRows = 0;
                result.SkippedRows = 0;
                result.SuccessRows = 0;
            }
            else
            {
                foreach (var importRow in rows)
                {
                    if (importRow.ValidationStatus == WorkImportValidationStatus.Error)
                    {
                        result.FailedRows++;
                        continue;
                    }

                    if (importRow.DuplicateStatus == 1)
                    {
                        if (input.ImportStrategy == WorkImportStrategy.SkipDuplicate)
                        {
                            result.SkippedRows++;
                            continue;
                        }

                        if (input.ImportStrategy == WorkImportStrategy.OverwriteDuplicate && importRow.ImportedWorkLogId.HasValue)
                        {
                            var existingLog = await _context.WorkLogs.FindAsync([importRow.ImportedWorkLogId], cancellationToken);
                            if (existingLog != null)
                            {
                                existingLog.WorkDate = importRow.ParsedDate ?? DateOnly.FromDateTime(DateTime.Now);
                                existingLog.Title = importRow.RawContent?.Length > 50
                                    ? importRow.RawContent[..50]
                                    : importRow.RawContent ?? "导入的工作日志";
                                existingLog.OriginalContent = importRow.RawContent;
                                existingLog.TotalHours = importRow.ParsedHours ?? 0;
                                existingLog.UpdatedAt = DateTime.UtcNow;
                                result.SuccessRows++;
                            }
                        }
                        else
                        {
                            result.SkippedRows++;
                            continue;
                        }
                    }
                    else
                    {
                        var project = projects.FirstOrDefault(p => p.ProjectName == importRow.RawProject);
                        var newLog = new WorkLog
                        {
                            Id = Guid.NewGuid(),
                            WorkDate = importRow.ParsedDate ?? DateOnly.FromDateTime(DateTime.Now),
                            WeekDay = importRow.RawWeekDay ?? "",
                            ProjectId = project?.Id ?? Guid.Empty,
                            Title = importRow.RawContent?.Length > 50
                                ? importRow.RawContent[..50]
                                : importRow.RawContent ?? "导入的工作日志",
                            OriginalContent = importRow.RawContent,
                            TotalHours = importRow.ParsedHours ?? 0,
                            Status = WorkLogStatus.Normal,
                            SourceType = WorkLogSourceType.ExcelImport,
                            ImportBatchId = batch.Id,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.WorkLogs.Add(newLog);
                        importRow.ImportedWorkLogId = newLog.Id;
                        result.SuccessRows++;
                    }
                }
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
            stopwatch.Stop();
            batch.FinishedAt = DateTime.UtcNow;
            batch.TotalRows = result.SuccessRows + result.FailedRows + result.SkippedRows;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public byte[] GenerateTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("工作日志");

        var headers = new[] { "日期", "项目名称", "设备名称", "任务类型", "工作内容", "工时(小时)", "备注" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        worksheet.Cell(2, 1).Value = "2024-05-01";
        worksheet.Cell(2, 2).Value = "生产线升级项目";
        worksheet.Cell(2, 3).Value = "A线体";
        worksheet.Cell(2, 4).Value = "设备调试";
        worksheet.Cell(2, 5).Value = "完成设备调试和参数优化";
        worksheet.Cell(2, 6).Value = 4;
        worksheet.Cell(2, 7).Value = "";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
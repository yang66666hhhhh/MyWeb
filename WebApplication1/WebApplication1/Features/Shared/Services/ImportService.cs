using System.Globalization;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Assets.Entities;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Shared.Dtos;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Shared.Services;

public interface IImportService
{
    Task<ImportPreviewDto> PreviewTasksAsync(Stream stream, string fileName, CancellationToken ct);
    Task<ImportResultDto> ImportTasksAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct);
    Task<ImportPreviewDto> PreviewWorkLogsAsync(Stream stream, string fileName, CancellationToken ct);
    Task<ImportResultDto> ImportWorkLogsAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct);
    Task<ImportPreviewDto> PreviewHabitsAsync(Stream stream, string fileName, CancellationToken ct);
    Task<ImportResultDto> ImportHabitsAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct);
    Task<ImportPreviewDto> PreviewIncomeAsync(Stream stream, string fileName, CancellationToken ct);
    Task<ImportResultDto> ImportIncomeAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct);
    Task<ImportPreviewDto> PreviewExpenseAsync(Stream stream, string fileName, CancellationToken ct);
    Task<ImportResultDto> ImportExpenseAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct);
}

public class ImportService(AppDbContext db) : IImportService
{
    public async Task<ImportPreviewDto> PreviewTasksAsync(Stream stream, string fileName, CancellationToken ct)
    {
        var rows = ReadFile(stream, fileName);
        var errors = new List<ImportErrorDto>();
        var validRows = 0;

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowErrors = ValidateTaskRow(row, i + 2);
            if (rowErrors.Count > 0)
                errors.AddRange(rowErrors);
            else
                validRows++;
        }

        return new ImportPreviewDto
        {
            TotalRows = rows.Count,
            ValidRows = validRows,
            ErrorRows = errors.Select(e => e.RowNumber).Distinct().Count(),
            PreviewRows = rows.Take(10).ToList(),
            Errors = errors
        };
    }

    public async Task<ImportResultDto> ImportTasksAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct)
    {
        var result = new ImportResultDto { TotalRows = rows.Count };

        foreach (var row in rows)
        {
            try
            {
                var title = GetString(row, "标题");
                if (string.IsNullOrWhiteSpace(title))
                {
                    result.SkippedRows++;
                    continue;
                }

                var item = new TaskItem
                {
                    UserId = userId,
                    PlanDate = GetDateOnly(row, "计划日期") ?? DateOnly.FromDateTime(DateTime.Today),
                    Title = title,
                    Description = GetString(row, "描述"),
                    Type = ParseEnum<TaskType>(GetString(row, "类型"), TaskType.Personal),
                    Source = ParseEnum<TaskSource>(GetString(row, "来源"), TaskSource.Growth),
                    Priority = ParseEnum<TaskPriority>(GetString(row, "优先级"), TaskPriority.Medium),
                    Status = ParseEnum<TaskItemStatus>(GetString(row, "状态"), TaskItemStatus.Pending),
                    StartTime = GetString(row, "开始时间"),
                    EndTime = GetString(row, "结束时间"),
                    EstimatedHours = GetDecimal(row, "预估工时"),
                    ActualHours = GetDecimal(row, "实际工时"),
                    Remark = GetString(row, "备注")
                };

                db.Tasks.Add(item);
                result.SuccessRows++;
            }
            catch (Exception ex)
            {
                result.FailedRows++;
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = rows.IndexOf(row) + 2,
                    Message = ex.Message
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<ImportPreviewDto> PreviewWorkLogsAsync(Stream stream, string fileName, CancellationToken ct)
    {
        var rows = ReadFile(stream, fileName);
        var errors = new List<ImportErrorDto>();
        var validRows = 0;

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowErrors = ValidateWorkLogRow(row, i + 2);
            if (rowErrors.Count > 0)
                errors.AddRange(rowErrors);
            else
                validRows++;
        }

        return new ImportPreviewDto
        {
            TotalRows = rows.Count,
            ValidRows = validRows,
            ErrorRows = errors.Select(e => e.RowNumber).Distinct().Count(),
            PreviewRows = rows.Take(10).ToList(),
            Errors = errors
        };
    }

    public async Task<ImportResultDto> ImportWorkLogsAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct)
    {
        var result = new ImportResultDto { TotalRows = rows.Count };

        foreach (var row in rows)
        {
            try
            {
                var title = GetString(row, "标题");
                if (string.IsNullOrWhiteSpace(title))
                {
                    result.SkippedRows++;
                    continue;
                }

                var item = new WorkLog
                {
                    UserId = userId,
                    WorkDate = GetDateOnly(row, "工作日期") ?? DateOnly.FromDateTime(DateTime.Today),
                    WeekDay = GetString(row, "星期") ?? string.Empty,
                    Title = title,
                    OriginalContent = GetString(row, "原始内容"),
                    Summary = GetString(row, "摘要"),
                    TotalHours = GetDecimal(row, "工时") ?? 0,
                    Status = ParseEnum<WorkLogStatus>(GetString(row, "状态"), WorkLogStatus.Normal),
                    Remark = GetString(row, "备注")
                };

                db.WorkLogs.Add(item);
                result.SuccessRows++;
            }
            catch (Exception ex)
            {
                result.FailedRows++;
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = rows.IndexOf(row) + 2,
                    Message = ex.Message
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<ImportPreviewDto> PreviewHabitsAsync(Stream stream, string fileName, CancellationToken ct)
    {
        var rows = ReadFile(stream, fileName);
        var errors = new List<ImportErrorDto>();
        var validRows = 0;

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowErrors = ValidateHabitRow(row, i + 2);
            if (rowErrors.Count > 0)
                errors.AddRange(rowErrors);
            else
                validRows++;
        }

        return new ImportPreviewDto
        {
            TotalRows = rows.Count,
            ValidRows = validRows,
            ErrorRows = errors.Select(e => e.RowNumber).Distinct().Count(),
            PreviewRows = rows.Take(10).ToList(),
            Errors = errors
        };
    }

    public async Task<ImportResultDto> ImportHabitsAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct)
    {
        var result = new ImportResultDto { TotalRows = rows.Count };

        foreach (var row in rows)
        {
            try
            {
                var name = GetString(row, "习惯名称");
                if (string.IsNullOrWhiteSpace(name))
                {
                    result.SkippedRows++;
                    continue;
                }

                var item = new Habit
                {
                    UserId = userId,
                    Name = name,
                    HabitType = GetString(row, "类型") ?? string.Empty,
                    Description = GetString(row, "描述"),
                    TargetFrequency = GetString(row, "目标频率") ?? "每天",
                    Status = ParseStatus(GetString(row, "状态"))
                };

                db.Habits.Add(item);
                result.SuccessRows++;
            }
            catch (Exception ex)
            {
                result.FailedRows++;
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = rows.IndexOf(row) + 2,
                    Message = ex.Message
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<ImportPreviewDto> PreviewIncomeAsync(Stream stream, string fileName, CancellationToken ct)
    {
        var rows = ReadFile(stream, fileName);
        var errors = new List<ImportErrorDto>();
        var validRows = 0;

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowErrors = ValidateIncomeRow(row, i + 2);
            if (rowErrors.Count > 0)
                errors.AddRange(rowErrors);
            else
                validRows++;
        }

        return new ImportPreviewDto
        {
            TotalRows = rows.Count,
            ValidRows = validRows,
            ErrorRows = errors.Select(e => e.RowNumber).Distinct().Count(),
            PreviewRows = rows.Take(10).ToList(),
            Errors = errors
        };
    }

    public async Task<ImportResultDto> ImportIncomeAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct)
    {
        var result = new ImportResultDto { TotalRows = rows.Count };

        foreach (var row in rows)
        {
            try
            {
                var title = GetString(row, "标题");
                if (string.IsNullOrWhiteSpace(title))
                {
                    result.SkippedRows++;
                    continue;
                }

                var item = new Income
                {
                    UserId = userId,
                    IncomeDate = GetString(row, "收入日期") ?? DateTime.Today.ToString("yyyy-MM-dd"),
                    Category = GetString(row, "分类") ?? string.Empty,
                    Title = title,
                    Amount = GetDecimal(row, "金额") ?? 0,
                    Description = GetString(row, "描述"),
                    Remark = GetString(row, "备注")
                };

                db.Incomes.Add(item);
                result.SuccessRows++;
            }
            catch (Exception ex)
            {
                result.FailedRows++;
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = rows.IndexOf(row) + 2,
                    Message = ex.Message
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<ImportPreviewDto> PreviewExpenseAsync(Stream stream, string fileName, CancellationToken ct)
    {
        var rows = ReadFile(stream, fileName);
        var errors = new List<ImportErrorDto>();
        var validRows = 0;

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowErrors = ValidateExpenseRow(row, i + 2);
            if (rowErrors.Count > 0)
                errors.AddRange(rowErrors);
            else
                validRows++;
        }

        return new ImportPreviewDto
        {
            TotalRows = rows.Count,
            ValidRows = validRows,
            ErrorRows = errors.Select(e => e.RowNumber).Distinct().Count(),
            PreviewRows = rows.Take(10).ToList(),
            Errors = errors
        };
    }

    public async Task<ImportResultDto> ImportExpenseAsync(List<Dictionary<string, object?>> rows, Guid userId, CancellationToken ct)
    {
        var result = new ImportResultDto { TotalRows = rows.Count };

        foreach (var row in rows)
        {
            try
            {
                var title = GetString(row, "标题");
                if (string.IsNullOrWhiteSpace(title))
                {
                    result.SkippedRows++;
                    continue;
                }

                var item = new Expense
                {
                    UserId = userId,
                    ExpenseDate = GetString(row, "支出日期") ?? DateTime.Today.ToString("yyyy-MM-dd"),
                    Category = GetString(row, "分类") ?? string.Empty,
                    Title = title,
                    Amount = GetDecimal(row, "金额") ?? 0,
                    Description = GetString(row, "描述"),
                    Remark = GetString(row, "备注")
                };

                db.Expenses.Add(item);
                result.SuccessRows++;
            }
            catch (Exception ex)
            {
                result.FailedRows++;
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = rows.IndexOf(row) + 2,
                    Message = ex.Message
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }

    private static List<Dictionary<string, object?>> ReadFile(Stream stream, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        return ext switch
        {
            ".xlsx" or ".xls" => ReadExcel(stream),
            ".csv" => ReadCsv(stream),
            ".json" => ReadJson(stream),
            _ => throw new NotSupportedException($"不支持的文件格式: {ext}")
        };
    }

    private static List<Dictionary<string, object?>> ReadExcel(Stream stream)
    {
        var rows = new List<Dictionary<string, object?>>();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var headerRow = worksheet.Row(1);
        var headers = new List<string>();

        for (int col = 1; col <= worksheet.ColumnsUsed().Count(); col++)
        {
            headers.Add(headerRow.Cell(col).GetString());
        }

        for (int row = 2; row <= worksheet.RowsUsed().Count(); row++)
        {
            var dict = new Dictionary<string, object?>();
            for (int col = 0; col < headers.Count; col++)
            {
                var cell = worksheet.Cell(row, col + 1);
                dict[headers[col]] = cell.Value.IsBlank ? null : cell.GetString();
            }
            rows.Add(dict);
        }

        return rows;
    }

    private static List<Dictionary<string, object?>> ReadCsv(Stream stream)
    {
        var rows = new List<Dictionary<string, object?>>();
        using var reader = new StreamReader(stream);
        string? line;
        List<string>? headers = null;
        var lineNumber = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            var fields = ParseCsvLine(line);
            if (lineNumber == 1)
            {
                headers = fields;
                continue;
            }

            if (headers is null) continue;

            var dict = new Dictionary<string, object?>();
            for (int i = 0; i < headers.Count; i++)
            {
                dict[headers[i]] = i < fields.Count ? fields[i] : null;
            }
            rows.Add(dict);
        }

        return rows;
    }

    private static List<Dictionary<string, object?>> ReadJson(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var items = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);
        if (items is null) return [];

        return items.Select(item =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var kvp in item)
            {
                dict[kvp.Key] = kvp.Value.ValueKind switch
                {
                    JsonValueKind.String => kvp.Value.GetString(),
                    JsonValueKind.Number => kvp.Value.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => kvp.Value.ToString()
                };
            }
            return dict;
        }).ToList();
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var current = new System.Text.StringBuilder();

        foreach (var c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        fields.Add(current.ToString());

        return fields;
    }

    private static string? GetString(Dictionary<string, object?> row, string key)
    {
        return row.TryGetValue(key, out var value) ? value?.ToString() : null;
    }

    private static decimal? GetDecimal(Dictionary<string, object?> row, string key)
    {
        var str = GetString(row, key);
        if (string.IsNullOrWhiteSpace(str)) return null;
        return decimal.TryParse(str, out var d) ? d : null;
    }

    private static DateOnly? GetDateOnly(Dictionary<string, object?> row, string key)
    {
        var str = GetString(row, key);
        if (string.IsNullOrWhiteSpace(str)) return null;
        return DateOnly.TryParse(str, out var d) ? d : null;
    }

    private static T ParseEnum<T>(string? value, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }

    private static int ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 1;
        return value switch
        {
            "启用" or "active" or "1" => 1,
            "禁用" or "inactive" or "0" => 0,
            _ => 1
        };
    }

    private static List<ImportErrorDto> ValidateTaskRow(Dictionary<string, object?> row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();
        var title = GetString(row, "标题");
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "标题", Message = "标题不能为空" });
        }
        return errors;
    }

    private static List<ImportErrorDto> ValidateWorkLogRow(Dictionary<string, object?> row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();
        var title = GetString(row, "标题");
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "标题", Message = "标题不能为空" });
        }
        return errors;
    }

    private static List<ImportErrorDto> ValidateHabitRow(Dictionary<string, object?> row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();
        var name = GetString(row, "习惯名称");
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "习惯名称", Message = "习惯名称不能为空" });
        }
        return errors;
    }

    private static List<ImportErrorDto> ValidateIncomeRow(Dictionary<string, object?> row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();
        var title = GetString(row, "标题");
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "标题", Message = "标题不能为空" });
        }
        var amount = GetDecimal(row, "金额");
        if (amount.HasValue && amount < 0)
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "金额", Message = "金额不能为负数" });
        }
        return errors;
    }

    private static List<ImportErrorDto> ValidateExpenseRow(Dictionary<string, object?> row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();
        var title = GetString(row, "标题");
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "标题", Message = "标题不能为空" });
        }
        var amount = GetDecimal(row, "金额");
        if (amount.HasValue && amount < 0)
        {
            errors.Add(new ImportErrorDto { RowNumber = rowNumber, Field = "金额", Message = "金额不能为负数" });
        }
        return errors;
    }
}

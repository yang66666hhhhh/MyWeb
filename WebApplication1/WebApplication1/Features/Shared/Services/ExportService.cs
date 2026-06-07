using System.Globalization;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Assets.Entities;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Shared.Dtos;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Shared.Services;

public interface IExportService
{
    Task<byte[]> ExportTasksAsync(ExportQueryDto query, Guid? userId, CancellationToken ct);
    Task<byte[]> ExportWorkLogsAsync(ExportQueryDto query, Guid? userId, CancellationToken ct);
    Task<byte[]> ExportHabitsAsync(ExportQueryDto query, Guid? userId, CancellationToken ct);
    Task<byte[]> ExportIncomeAsync(ExportQueryDto query, Guid? userId, CancellationToken ct);
    Task<byte[]> ExportExpenseAsync(ExportQueryDto query, Guid? userId, CancellationToken ct);
}

public class ExportService(AppDbContext db) : IExportService
{
    public async Task<byte[]> ExportTasksAsync(ExportQueryDto query, Guid? userId, CancellationToken ct)
    {
        var q = db.Tasks.AsNoTracking().Where(t => !t.IsDeleted);
        if (userId.HasValue)
            q = q.Where(t => t.UserId == userId.Value);
        if (!string.IsNullOrEmpty(query.StartDate) && DateOnly.TryParse(query.StartDate, out var sd))
            q = q.Where(t => t.PlanDate >= sd);
        if (!string.IsNullOrEmpty(query.EndDate) && DateOnly.TryParse(query.EndDate, out var ed))
            q = q.Where(t => t.PlanDate <= ed);

        var items = await q.OrderBy(t => t.PlanDate).ToListAsync(ct);

        var rows = items.Select(t => new Dictionary<string, object?>
        {
            ["计划日期"] = t.PlanDate.ToString("yyyy-MM-dd"),
            ["标题"] = t.Title,
            ["描述"] = t.Description,
            ["类型"] = t.Type.ToString(),
            ["来源"] = t.Source.ToString(),
            ["优先级"] = t.Priority.ToString(),
            ["状态"] = t.Status.ToString(),
            ["开始时间"] = t.StartTime,
            ["结束时间"] = t.EndTime,
            ["预估工时"] = t.EstimatedHours,
            ["实际工时"] = t.ActualHours,
            ["备注"] = t.Remark,
            ["完成时间"] = t.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            ["创建时间"] = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return query.Format switch
        {
            ExportFormat.Excel => GenerateExcel(rows, "任务列表"),
            ExportFormat.Csv => GenerateCsv(rows),
            ExportFormat.Json => GenerateJson(rows),
            _ => GenerateExcel(rows, "任务列表")
        };
    }

    public async Task<byte[]> ExportWorkLogsAsync(ExportQueryDto query, Guid? userId, CancellationToken ct)
    {
        var q = db.WorkLogs.AsNoTracking().Where(w => !w.IsDeleted);
        if (userId.HasValue)
            q = q.Where(w => w.UserId == userId.Value);
        if (!string.IsNullOrEmpty(query.StartDate) && DateOnly.TryParse(query.StartDate, out var sd))
            q = q.Where(w => w.WorkDate >= sd);
        if (!string.IsNullOrEmpty(query.EndDate) && DateOnly.TryParse(query.EndDate, out var ed))
            q = q.Where(w => w.WorkDate <= ed);

        var items = await q.Include(w => w.Project).OrderBy(w => w.WorkDate).ToListAsync(ct);

        var rows = items.Select(w => new Dictionary<string, object?>
        {
            ["工作日期"] = w.WorkDate.ToString("yyyy-MM-dd"),
            ["星期"] = w.WeekDay,
            ["项目"] = w.Project?.ProjectName,
            ["标题"] = w.Title,
            ["原始内容"] = w.OriginalContent,
            ["摘要"] = w.Summary,
            ["工时"] = w.TotalHours,
            ["状态"] = w.Status.ToString(),
            ["备注"] = w.Remark,
            ["创建时间"] = w.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return query.Format switch
        {
            ExportFormat.Excel => GenerateExcel(rows, "工作日志"),
            ExportFormat.Csv => GenerateCsv(rows),
            ExportFormat.Json => GenerateJson(rows),
            _ => GenerateExcel(rows, "工作日志")
        };
    }

    public async Task<byte[]> ExportHabitsAsync(ExportQueryDto query, Guid? userId, CancellationToken ct)
    {
        var q = db.Habits.AsNoTracking().Where(h => !h.IsDeleted);
        if (userId.HasValue)
            q = q.Where(h => h.UserId == userId.Value);

        var items = await q.Include(h => h.CheckIns).OrderBy(h => h.Name).ToListAsync(ct);

        var rows = items.Select(h => new Dictionary<string, object?>
        {
            ["习惯名称"] = h.Name,
            ["类型"] = h.HabitType,
            ["描述"] = h.Description,
            ["目标频率"] = h.TargetFrequency,
            ["状态"] = h.Status == 1 ? "启用" : "禁用",
            ["当前连续天数"] = h.CurrentStreak,
            ["最长连续天数"] = h.LongestStreak,
            ["总打卡次数"] = h.TotalCheckIns,
            ["最后打卡日期"] = h.LastCheckInDate?.ToString("yyyy-MM-dd"),
            ["创建时间"] = h.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return query.Format switch
        {
            ExportFormat.Excel => GenerateExcel(rows, "习惯记录"),
            ExportFormat.Csv => GenerateCsv(rows),
            ExportFormat.Json => GenerateJson(rows),
            _ => GenerateExcel(rows, "习惯记录")
        };
    }

    public async Task<byte[]> ExportIncomeAsync(ExportQueryDto query, Guid? userId, CancellationToken ct)
    {
        var q = db.Incomes.AsNoTracking();
        if (userId.HasValue)
            q = q.Where(i => i.UserId == userId.Value);
        if (!string.IsNullOrEmpty(query.StartDate))
            q = q.Where(i => string.Compare(i.IncomeDate, query.StartDate) >= 0);
        if (!string.IsNullOrEmpty(query.EndDate))
            q = q.Where(i => string.Compare(i.IncomeDate, query.EndDate) <= 0);

        var items = await q.OrderBy(i => i.IncomeDate).ToListAsync(ct);

        var rows = items.Select(i => new Dictionary<string, object?>
        {
            ["收入日期"] = i.IncomeDate,
            ["分类"] = i.Category,
            ["标题"] = i.Title,
            ["金额"] = i.Amount,
            ["描述"] = i.Description,
            ["备注"] = i.Remark,
            ["创建时间"] = i.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return query.Format switch
        {
            ExportFormat.Excel => GenerateExcel(rows, "收入记录"),
            ExportFormat.Csv => GenerateCsv(rows),
            ExportFormat.Json => GenerateJson(rows),
            _ => GenerateExcel(rows, "收入记录")
        };
    }

    public async Task<byte[]> ExportExpenseAsync(ExportQueryDto query, Guid? userId, CancellationToken ct)
    {
        var q = db.Expenses.AsNoTracking();
        if (userId.HasValue)
            q = q.Where(e => e.UserId == userId.Value);
        if (!string.IsNullOrEmpty(query.StartDate))
            q = q.Where(e => string.Compare(e.ExpenseDate, query.StartDate) >= 0);
        if (!string.IsNullOrEmpty(query.EndDate))
            q = q.Where(e => string.Compare(e.ExpenseDate, query.EndDate) <= 0);

        var items = await q.OrderBy(e => e.ExpenseDate).ToListAsync(ct);

        var rows = items.Select(e => new Dictionary<string, object?>
        {
            ["支出日期"] = e.ExpenseDate,
            ["分类"] = e.Category,
            ["标题"] = e.Title,
            ["金额"] = e.Amount,
            ["描述"] = e.Description,
            ["备注"] = e.Remark,
            ["创建时间"] = e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return query.Format switch
        {
            ExportFormat.Excel => GenerateExcel(rows, "支出记录"),
            ExportFormat.Csv => GenerateCsv(rows),
            ExportFormat.Json => GenerateJson(rows),
            _ => GenerateExcel(rows, "支出记录")
        };
    }

    private static byte[] GenerateExcel(List<Dictionary<string, object?>> rows, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        if (rows.Count == 0)
        {
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        var columns = rows[0].Keys.ToList();

        for (int col = 0; col < columns.Count; col++)
        {
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = columns[col];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        for (int row = 0; row < rows.Count; row++)
        {
            for (int col = 0; col < columns.Count; col++)
            {
                var value = rows[row][columns[col]];
                var cell = worksheet.Cell(row + 2, col + 1);

                if (value is null)
                    cell.Value = Blank.Value;
                else if (value is decimal dec)
                    cell.Value = dec;
                else if (value is int intVal)
                    cell.Value = intVal;
                else if (value is double dbl)
                    cell.Value = dbl;
                else
                    cell.Value = value.ToString();
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static byte[] GenerateCsv(List<Dictionary<string, object?>> rows)
    {
        if (rows.Count == 0)
            return Encoding.UTF8.GetBytes(string.Empty);

        var sb = new StringBuilder();
        var columns = rows[0].Keys.ToList();

        sb.AppendLine(string.Join(",", columns.Select(c => $"\"{c}\"")));

        foreach (var row in rows)
        {
            var values = columns.Select(c =>
            {
                var v = row[c];
                return v is null ? "" : $"\"{v.ToString()?.Replace("\"", "\"\"")}\"";
            });
            sb.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static byte[] GenerateJson(List<Dictionary<string, object?>> rows)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rows, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }));
    }
}

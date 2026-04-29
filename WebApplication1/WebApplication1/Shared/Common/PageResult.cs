namespace WebApplication1.Shared.Common;

public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];

    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(Total / (double)PageSize);

    public static PageResult<T> Create(IReadOnlyList<T> items, int total, int page, int pageSize)
    {
        return new PageResult<T>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}

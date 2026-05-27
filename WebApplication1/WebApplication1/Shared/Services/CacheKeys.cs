namespace WebApplication1.Shared.Services;

public static class CacheKeys
{
    private const string Prefix = "app:";

    public static class Analytics
    {
        public static string Dashboard(Guid userId) => $"{Prefix}analytics:dashboard:{userId}";
        public static string Trends(Guid userId, DateOnly startDate, DateOnly endDate) => $"{Prefix}analytics:trends:{userId}:{startDate}:{endDate}";
        public static string DistributionByType(Guid userId) => $"{Prefix}analytics:distribution:type:{userId}";
        public static string WorkVsGrowth(Guid userId) => $"{Prefix}analytics:workVsGrowth:{userId}";
        public static string PriorityDistribution(Guid userId) => $"{Prefix}analytics:priority:{userId}";

        public static string[] AllForUser(Guid userId) =>
        [
            Dashboard(userId),
            DistributionByType(userId),
            WorkVsGrowth(userId),
            PriorityDistribution(userId)
        ];
    }

    public static class Tasks
    {
        public static string Page(Guid userId, int page, int pageSize) => $"{Prefix}tasks:page:{userId}:{page}:{pageSize}";
        public static string Detail(Guid taskId) => $"{Prefix}tasks:detail:{taskId}";

        public static string[] AllForUser(Guid userId) =>
        [
            $"{Prefix}tasks:page:{userId}:*"
        ];
    }

    public static class Habits
    {
        public static string Page(Guid userId, int page, int pageSize) => $"{Prefix}habits:page:{userId}:{page}:{pageSize}";
        public static string Detail(Guid habitId) => $"{Prefix}habits:detail:{habitId}";

        public static string[] AllForUser(Guid userId) =>
        [
            $"{Prefix}habits:page:{userId}:*"
        ];
    }

    public static class Assets
    {
        public static string Summary(Guid userId) => $"{Prefix}assets:summary:{userId}";
        public static string IncomePage(Guid userId, int page, int pageSize) => $"{Prefix}assets:incomes:page:{userId}:{page}:{pageSize}";
        public static string ExpensePage(Guid userId, int page, int pageSize) => $"{Prefix}assets:expenses:page:{userId}:{page}:{pageSize}";
        public static string BudgetPage(Guid userId, int page, int pageSize) => $"{Prefix}assets:budgets:page:{userId}:{page}:{pageSize}";
        public static string InvestmentPage(Guid userId, int page, int pageSize) => $"{Prefix}assets:investments:page:{userId}:{page}:{pageSize}";

        public static string[] AllForUser(Guid userId) =>
        [
            Summary(userId),
            $"{Prefix}assets:incomes:page:{userId}:*",
            $"{Prefix}assets:expenses:page:{userId}:*",
            $"{Prefix}assets:budgets:page:{userId}:*",
            $"{Prefix}assets:investments:page:{userId}:*"
        ];
    }

    public static class Content
    {
        public static string ArticlePage(Guid userId, int page, int pageSize) => $"{Prefix}content:articles:page:{userId}:{page}:{pageSize}";
        public static string ArticleDetail(Guid articleId) => $"{Prefix}content:articles:detail:{articleId}";
        public static string MediaPage(Guid userId, int page, int pageSize) => $"{Prefix}content:media:page:{userId}:{page}:{pageSize}";
        public static string CalendarPage(Guid userId, int page, int pageSize) => $"{Prefix}content:calendar:page:{userId}:{page}:{pageSize}";

        public static string[] AllForUser(Guid userId) =>
        [
            $"{Prefix}content:articles:page:{userId}:*",
            $"{Prefix}content:media:page:{userId}:*",
            $"{Prefix}content:calendar:page:{userId}:*"
        ];
    }
}

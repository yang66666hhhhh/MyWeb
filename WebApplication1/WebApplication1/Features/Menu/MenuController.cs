using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Features.Menu;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    [HttpGet("all")]
    public ActionResult GetAllMenus()
    {
        var menus = new object[]
        {
            new
            {
                meta = new { icon = "lucide:home", order = -1, title = "首页" },
                name = "Dashboard",
                path = "/dashboard",
                redirect = "/dashboard/workspace",
                children = new object[]
                {
                    new
                    {
                        name = "Workspace",
                        path = "/dashboard/workspace",
                        component = "/dashboard/workspace/index",
                        meta = new { title = "工作台" }
                    },
                    new
                    {
                        name = "Analytics",
                        path = "/dashboard/analytics",
                        component = "/dashboard/analytics/index",
                        meta = new { affixTab = true, title = "数据分析" }
                    }
                }
            },
            new
            {
                meta = new { icon = "lucide:sprout", order = 10, title = "个人成长" },
                name = "Growth",
                path = "/growth",
                children = new object[]
                {
                    new
                    {
                        component = "/growth/dashboard/index",
                        meta = new { icon = "lucide:gauge", title = "成长看板" },
                        name = "GrowthDashboard",
                        path = "/growth/dashboard"
                    },
                    new
                    {
                        component = "/growth/daily-plan/index",
                        meta = new { icon = "lucide:calendar-check", keepAlive = true, title = "每日计划" },
                        name = "DailyPlanList",
                        path = "/growth/daily-plans"
                    },
                    new
                    {
                        component = "/growth/habit/index",
                        meta = new { icon = "lucide:badge-check", keepAlive = true, title = "习惯打卡" },
                        name = "HabitList",
                        path = "/growth/habits"
                    },
                    new
                    {
                        component = "/growth/work-log/index",
                        meta = new { icon = "lucide:book-open-check", title = "工作日志" },
                        name = "WorkLogList",
                        path = "/growth/work-logs"
                    },
                    new
                    {
                        component = "/growth/knowledge-base/index",
                        meta = new { icon = "lucide:library", title = "知识库" },
                        name = "KnowledgeBaseList",
                        path = "/growth/knowledge-base"
                    },
                    new
                    {
                        component = "/growth/postgraduate/index",
                        meta = new { icon = "lucide:graduation-cap", title = "备考中心" },
                        name = "PostgraduateList",
                        path = "/growth/postgraduate",
                        children = new object[]
                        {
                            new
                            {
                                component = "/growth/postgraduate/materials/index",
                                meta = new { icon = "lucide:file-text", title = "备考资料" },
                                name = "PostgraduateMaterials",
                                path = "/growth/postgraduate/materials"
                            },
                            new
                            {
                                component = "/growth/postgraduate/mistakes/index",
                                meta = new { icon = "lucide:x-circle", title = "错题本" },
                                name = "PostgraduateMistakes",
                                path = "/growth/postgraduate/mistakes"
                            },
                            new
                            {
                                component = "/growth/postgraduate/study-plans/index",
                                meta = new { icon = "lucide:list-todo", title = "学习计划" },
                                name = "PostgraduateStudyPlans",
                                path = "/growth/postgraduate/study-plans"
                            }
                        }
                    },
                    new
                    {
                        component = "/growth/project/index",
                        meta = new { icon = "lucide:kanban", title = "项目管理" },
                        name = "ProjectList",
                        path = "/growth/projects"
                    }
                }
            },
            new
            {
                meta = new { icon = "lucide:briefcase", order = 20, title = "工作中心" },
                name = "Work",
                path = "/work",
                children = new object[]
                {
                    new
                    {
                        component = "/work/dashboard/index",
                        meta = new { icon = "lucide:layout-dashboard", title = "工作看板" },
                        name = "WorkDashboard",
                        path = "/work/dashboard"
                    },
                    new
                    {
                        component = "/work/daily-plan/index",
                        meta = new { icon = "lucide:calendar-check", title = "每日计划" },
                        name = "WorkDailyPlan",
                        path = "/work/daily-plan"
                    },
                    new
                    {
                        component = "/work/task-type/index",
                        meta = new { icon = "lucide:tag", title = "任务类型" },
                        name = "WorkTaskType",
                        path = "/work/task-type"
                    },
                    new
                    {
                        component = "/work/device/index",
                        meta = new { icon = "lucide:device-desktop", title = "设备管理" },
                        name = "WorkDevice",
                        path = "/work/device"
                    },
                    new
                    {
                        component = "/work/project/index",
                        meta = new { icon = "lucide:folder-kanban", title = "工作项目" },
                        name = "WorkProject",
                        path = "/work/project"
                    },
                    new
                    {
                        component = "/work/log/index",
                        meta = new { icon = "lucide:clipboard-list", title = "工作日志" },
                        name = "WorkLog",
                        path = "/work/log"
                    },
                    new
                    {
                        component = "/work/import/index",
                        meta = new { icon = "lucide:upload", title = "数据导入" },
                        name = "WorkImport",
                        path = "/work/import"
                    },
                    new
                    {
                        component = "/work/statistics/index",
                        meta = new { icon = "lucide:bar-chart-3", title = "统计分析" },
                        name = "WorkStatistics",
                        path = "/work/statistics"
                    }
                }
            },
            new
            {
                meta = new { icon = "lucide:sparkles", order = 1000, title = "示例中心" },
                name = "Demos",
                path = "/demos",
                children = new object[]
                {
                    new
                    {
                        name = "AntdDemos",
                        path = "/demos/antd",
                        component = "/demos/antd/index",
                        meta = new { icon = "lucide:layout-grid", title = "Ant Design 示例" }
                    }
                }
            },
            new
            {
                meta = new { icon = "lucide:settings", order = 9997, title = "系统" },
                name = "System",
                path = "/system",
                component = "/system/index",
                children = Array.Empty<object>()
            },
            new
            {
                meta = new { icon = "lucide:external-link", order = 9998, title = "外部链接" },
                name = "ExternalLinks",
                path = "/external-links",
                component = "/external-links/index",
                children = new object[]
                {
                    new
                    {
                        name = "VbenDocument",
                        path = "/external-links/document",
                        component = "IFrameView",
                        type = "embedded",
                        meta = new { icon = "lucide:book-open", iframeSrc = "https://doc.vben.pro", title = "官方文档" }
                    },
                    new
                    {
                        name = "VbenGithub",
                        path = "/external-links/github",
                        component = "IFrameView",
                        type = "link",
                        meta = new { icon = "lucide:github", link = "https://github.com/vbenjs/vue-vben-admin", title = "Github" }
                    },
                    new
                    {
                        name = "VbenAntdv",
                        path = "/external-links/antdv",
                        component = "IFrameView",
                        type = "link",
                        meta = new { icon = "lucide:layout-grid", link = "https://ant.vben.pro", title = "Ant Design Vue Pro" }
                    }
                }
            },
            new
            {
                meta = new { icon = "lucide:info", order = 9999, title = "关于" },
                name = "About",
                path = "/about",
                component = "/about/index"
            }
        };

        return Ok(new { code = 0, data = menus });
    }
}
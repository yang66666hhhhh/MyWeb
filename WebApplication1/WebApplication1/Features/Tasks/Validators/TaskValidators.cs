using FluentValidation;
using WebApplication1.Features.Tasks;

namespace WebApplication1.Features.Tasks.Validators;

public class CreateTaskItemDtoValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("任务标题不能为空")
            .MaximumLength(200).WithMessage("任务标题长度不能超过200");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("描述长度不能超过2000")
            .When(x => x.Description != null);

        RuleFor(x => x.TaskType)
            .NotEmpty().WithMessage("任务类型不能为空")
            .Must(x => x == "Personal" || x == "Work" || x == "Study")
            .WithMessage("任务类型必须是 Personal、Work 或 Study");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("任务来源不能为空")
            .Must(x => x == "Growth" || x == "Work" || x == "AI")
            .WithMessage("任务来源必须是 Growth、Work 或 AI");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 4).WithMessage("优先级必须在1-4之间");

        RuleFor(x => x.StartTime)
            .MaximumLength(10).WithMessage("开始时间长度不能超过10")
            .When(x => x.StartTime != null);

        RuleFor(x => x.EndTime)
            .MaximumLength(10).WithMessage("结束时间长度不能超过10")
            .When(x => x.EndTime != null);

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("预计工时必须大于0")
            .LessThan(24).WithMessage("预计工时不能超过24小时")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.Remark)
            .MaximumLength(1000).WithMessage("备注长度不能超过1000")
            .When(x => x.Remark != null);
    }
}

public class UpdateTaskItemDtoValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("任务标题长度不能超过200")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("描述长度不能超过2000")
            .When(x => x.Description != null);

        RuleFor(x => x.TaskType)
            .Must(x => x == "Personal" || x == "Work" || x == "Study")
            .WithMessage("任务类型必须是 Personal、Work 或 Study")
            .When(x => x.TaskType != null);

        RuleFor(x => x.Source)
            .Must(x => x == "Growth" || x == "Work" || x == "AI")
            .WithMessage("任务来源必须是 Growth、Work 或 AI")
            .When(x => x.Source != null);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 4).WithMessage("优先级必须在1-4之间")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .InclusiveBetween(0, 3).WithMessage("状态值必须在0-3之间")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.StartTime)
            .MaximumLength(10).WithMessage("开始时间长度不能超过10")
            .When(x => x.StartTime != null);

        RuleFor(x => x.EndTime)
            .MaximumLength(10).WithMessage("结束时间长度不能超过10")
            .When(x => x.EndTime != null);

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("预计工时必须大于0")
            .LessThan(24).WithMessage("预计工时不能超过24小时")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.ActualHours)
            .GreaterThan(0).WithMessage("实际工时必须大于0")
            .LessThan(24).WithMessage("实际工时不能超过24小时")
            .When(x => x.ActualHours.HasValue);

        RuleFor(x => x.Remark)
            .MaximumLength(1000).WithMessage("备注长度不能超过1000")
            .When(x => x.Remark != null);
    }
}

public class ConvertTaskToLogDtoValidator : AbstractValidator<ConvertTaskToLogDto>
{
    public ConvertTaskToLogDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("任务ID不能为空");

        RuleFor(x => x.OriginalContent)
            .MaximumLength(4000).WithMessage("原始内容长度不能超过4000")
            .When(x => x.OriginalContent != null);

        RuleFor(x => x.TotalHours)
            .GreaterThan(0).WithMessage("总工时必须大于0")
            .LessThan(24).WithMessage("总工时不能超过24小时")
            .When(x => x.TotalHours.HasValue);
    }
}

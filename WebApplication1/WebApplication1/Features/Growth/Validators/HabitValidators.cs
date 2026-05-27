using FluentValidation;
using WebApplication1.Features.Growth.Dtos;

namespace WebApplication1.Features.Growth.Validators;

public class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
{
    public CreateHabitDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("习惯名称不能为空")
            .MaximumLength(100).WithMessage("习惯名称长度不能超过100");

        RuleFor(x => x.HabitType)
            .NotEmpty().WithMessage("习惯类型不能为空")
            .MaximumLength(50).WithMessage("习惯类型长度不能超过50");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述长度不能超过500")
            .When(x => x.Description != null);

        RuleFor(x => x.TargetFrequency)
            .NotEmpty().WithMessage("目标频率不能为空")
            .MaximumLength(20).WithMessage("目标频率长度不能超过20");
    }
}

public class UpdateHabitDtoValidator : AbstractValidator<UpdateHabitDto>
{
    public UpdateHabitDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("习惯名称长度不能超过100")
            .When(x => x.Name != null);

        RuleFor(x => x.HabitType)
            .MaximumLength(50).WithMessage("习惯类型长度不能超过50")
            .When(x => x.HabitType != null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述长度不能超过500")
            .When(x => x.Description != null);

        RuleFor(x => x.TargetFrequency)
            .MaximumLength(20).WithMessage("目标频率长度不能超过20")
            .When(x => x.TargetFrequency != null);

        RuleFor(x => x.Status)
            .InclusiveBetween(0, 2).WithMessage("状态值必须在0-2之间")
            .When(x => x.Status.HasValue);
    }
}

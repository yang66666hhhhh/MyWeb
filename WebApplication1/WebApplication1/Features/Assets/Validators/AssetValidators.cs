using FluentValidation;
using WebApplication1.Features.Assets.Dtos;

namespace WebApplication1.Features.Assets.Validators;

public class CreateIncomeDtoValidator : AbstractValidator<CreateIncomeDto>
{
    public CreateIncomeDtoValidator()
    {
        RuleFor(x => x.IncomeDate)
            .NotEmpty().WithMessage("收入日期不能为空")
            .MaximumLength(20).WithMessage("收入日期长度不能超过20");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("分类不能为空")
            .MaximumLength(50).WithMessage("分类长度不能超过50");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题长度不能超过200");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0");
    }
}

public class UpdateIncomeDtoValidator : AbstractValidator<UpdateIncomeDto>
{
    public UpdateIncomeDtoValidator()
    {
        RuleFor(x => x.IncomeDate)
            .MaximumLength(20).WithMessage("收入日期长度不能超过20")
            .When(x => x.IncomeDate != null);

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("分类长度不能超过50")
            .When(x => x.Category != null);

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("标题长度不能超过200")
            .When(x => x.Title != null);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0")
            .When(x => x.Amount.HasValue);
    }
}

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.ExpenseDate)
            .NotEmpty().WithMessage("支出日期不能为空")
            .MaximumLength(20).WithMessage("支出日期长度不能超过20");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("分类不能为空")
            .MaximumLength(50).WithMessage("分类长度不能超过50");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题长度不能超过200");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0");
    }
}

public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
{
    public UpdateExpenseDtoValidator()
    {
        RuleFor(x => x.ExpenseDate)
            .MaximumLength(20).WithMessage("支出日期长度不能超过20")
            .When(x => x.ExpenseDate != null);

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("分类长度不能超过50")
            .When(x => x.Category != null);

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("标题长度不能超过200")
            .When(x => x.Title != null);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0")
            .When(x => x.Amount.HasValue);
    }
}

public class CreateBudgetDtoValidator : AbstractValidator<CreateBudgetDto>
{
    public CreateBudgetDtoValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(2000).WithMessage("年份必须大于2000")
            .LessThan(2100).WithMessage("年份必须小于2100");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("月份必须在1-12之间");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("分类不能为空")
            .MaximumLength(50).WithMessage("分类长度不能超过50");

        RuleFor(x => x.PlannedAmount)
            .GreaterThanOrEqualTo(0).WithMessage("计划金额不能小于0");

        RuleFor(x => x.ActualAmount)
            .GreaterThanOrEqualTo(0).WithMessage("实际金额不能小于0");
    }
}

public class UpdateBudgetDtoValidator : AbstractValidator<UpdateBudgetDto>
{
    public UpdateBudgetDtoValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(2000).WithMessage("年份必须大于2000")
            .LessThan(2100).WithMessage("年份必须小于2100")
            .When(x => x.Year.HasValue);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("月份必须在1-12之间")
            .When(x => x.Month.HasValue);

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("分类长度不能超过50")
            .When(x => x.Category != null);

        RuleFor(x => x.PlannedAmount)
            .GreaterThanOrEqualTo(0).WithMessage("计划金额不能小于0")
            .When(x => x.PlannedAmount.HasValue);

        RuleFor(x => x.ActualAmount)
            .GreaterThanOrEqualTo(0).WithMessage("实际金额不能小于0")
            .When(x => x.ActualAmount.HasValue);
    }
}

public class CreateInvestmentDtoValidator : AbstractValidator<CreateInvestmentDto>
{
    public CreateInvestmentDtoValidator()
    {
        RuleFor(x => x.InvestmentDate)
            .NotEmpty().WithMessage("投资日期不能为空")
            .MaximumLength(20).WithMessage("投资日期长度不能超过20");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("投资类型不能为空")
            .MaximumLength(50).WithMessage("投资类型长度不能超过50");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("投资名称不能为空")
            .MaximumLength(200).WithMessage("投资名称长度不能超过200");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0");

        RuleFor(x => x.CurrentValue)
            .GreaterThanOrEqualTo(0).WithMessage("当前价值不能小于0")
            .When(x => x.CurrentValue.HasValue);

        RuleFor(x => x.ReturnRate)
            .InclusiveBetween(-100, 1000).WithMessage("回报率必须在-100到1000之间")
            .When(x => x.ReturnRate.HasValue);
    }
}

public class UpdateInvestmentDtoValidator : AbstractValidator<UpdateInvestmentDto>
{
    public UpdateInvestmentDtoValidator()
    {
        RuleFor(x => x.InvestmentDate)
            .MaximumLength(20).WithMessage("投资日期长度不能超过20")
            .When(x => x.InvestmentDate != null);

        RuleFor(x => x.Type)
            .MaximumLength(50).WithMessage("投资类型长度不能超过50")
            .When(x => x.Type != null);

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("投资名称长度不能超过200")
            .When(x => x.Name != null);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("金额必须大于0")
            .When(x => x.Amount.HasValue);

        RuleFor(x => x.CurrentValue)
            .GreaterThanOrEqualTo(0).WithMessage("当前价值不能小于0")
            .When(x => x.CurrentValue.HasValue);

        RuleFor(x => x.ReturnRate)
            .InclusiveBetween(-100, 1000).WithMessage("回报率必须在-100到1000之间")
            .When(x => x.ReturnRate.HasValue);
    }
}

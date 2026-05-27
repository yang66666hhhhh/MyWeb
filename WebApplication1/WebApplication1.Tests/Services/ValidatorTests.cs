using FluentValidation.TestHelper;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Features.Assets.Validators;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Features.Content.Validators;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Tasks.Validators;

namespace WebApplication1.Tests.Services;

public class ValidatorTests
{
    #region Asset Validators

    [Fact]
    public void CreateIncomeDtoValidator_ShouldHaveError_WhenTitleIsEmpty()
    {
        var validator = new CreateIncomeDtoValidator();
        var model = new CreateIncomeDto { Title = "" };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateIncomeDtoValidator_ShouldNotHaveError_WhenModelIsValid()
    {
        var validator = new CreateIncomeDtoValidator();
        var model = new CreateIncomeDto
        {
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000
        };

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateIncomeDtoValidator_ShouldHaveError_WhenAmountIsZero()
    {
        var validator = new CreateIncomeDtoValidator();
        var model = new CreateIncomeDto
        {
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 0
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreateExpenseDtoValidator_ShouldHaveError_WhenCategoryIsEmpty()
    {
        var validator = new CreateExpenseDtoValidator();
        var model = new CreateExpenseDto { Category = "" };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void CreateBudgetDtoValidator_ShouldHaveError_WhenMonthIsInvalid()
    {
        var validator = new CreateBudgetDtoValidator();
        var model = new CreateBudgetDto
        {
            Year = 2026,
            Month = 13,
            Category = "餐饮",
            PlannedAmount = 3000
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void CreateInvestmentDtoValidator_ShouldHaveError_WhenReturnRateIsTooHigh()
    {
        var validator = new CreateInvestmentDtoValidator();
        var model = new CreateInvestmentDto
        {
            InvestmentDate = "2026-01-15",
            Type = "股票",
            Name = "腾讯控股",
            Amount = 50000,
            ReturnRate = 1001
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ReturnRate);
    }

    #endregion

    #region Content Validators

    [Fact]
    public void CreateArticleDtoValidator_ShouldHaveError_WhenTitleIsEmpty()
    {
        var validator = new CreateArticleDtoValidator();
        var model = new CreateArticleDto { Title = "" };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateArticleDtoValidator_ShouldHaveError_WhenStatusIsInvalid()
    {
        var validator = new CreateArticleDtoValidator();
        var model = new CreateArticleDto
        {
            Title = "测试文章",
            Status = "invalid"
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void CreateArticleDtoValidator_ShouldNotHaveError_WhenModelIsValid()
    {
        var validator = new CreateArticleDtoValidator();
        var model = new CreateArticleDto
        {
            Title = "测试文章",
            Content = "这是测试内容",
            Status = "draft",
            Category = "技术"
        };

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateMediaItemDtoValidator_ShouldHaveError_WhenFileUrlIsInvalid()
    {
        var validator = new CreateMediaItemDtoValidator();
        var model = new CreateMediaItemDto
        {
            FileName = "test.jpg",
            FileUrl = "invalid-url",
            FileType = "image/jpeg",
            FileSize = 1024
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FileUrl);
    }

    [Fact]
    public void CreateMediaItemDtoValidator_ShouldHaveError_WhenFileSizeIsTooLarge()
    {
        var validator = new CreateMediaItemDtoValidator();
        var model = new CreateMediaItemDto
        {
            FileName = "test.jpg",
            FileUrl = "https://example.com/test.jpg",
            FileType = "image/jpeg",
            FileSize = 101 * 1024 * 1024 // 101MB
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FileSize);
    }

    [Fact]
    public void CreatePublishingCalendarDtoValidator_ShouldHaveError_WhenPlatformIsEmpty()
    {
        var validator = new CreatePublishingCalendarDtoValidator();
        var model = new CreatePublishingCalendarDto { Platform = "" };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Platform);
    }

    #endregion

    #region Task Validators

    [Fact]
    public void CreateTaskItemDtoValidator_ShouldHaveError_WhenTitleIsEmpty()
    {
        var validator = new CreateTaskItemDtoValidator();
        var model = new CreateTaskItemDto { Title = "" };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateTaskItemDtoValidator_ShouldHaveError_WhenTaskTypeIsInvalid()
    {
        var validator = new CreateTaskItemDtoValidator();
        var model = new CreateTaskItemDto
        {
            Title = "测试任务",
            TaskType = "Invalid"
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TaskType);
    }

    [Fact]
    public void CreateTaskItemDtoValidator_ShouldHaveError_WhenSourceIsInvalid()
    {
        var validator = new CreateTaskItemDtoValidator();
        var model = new CreateTaskItemDto
        {
            Title = "测试任务",
            TaskType = "Personal",
            Source = "Invalid"
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Source);
    }

    [Fact]
    public void CreateTaskItemDtoValidator_ShouldNotHaveError_WhenModelIsValid()
    {
        var validator = new CreateTaskItemDtoValidator();
        var model = new CreateTaskItemDto
        {
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Title = "测试任务",
            TaskType = "Personal",
            Source = "Growth",
            Priority = 2
        };

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateTaskItemDtoValidator_ShouldHaveError_WhenPriorityIsInvalid()
    {
        var validator = new UpdateTaskItemDtoValidator();
        var model = new UpdateTaskItemDto { Priority = 5 };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void UpdateTaskItemDtoValidator_ShouldHaveError_WhenStatusIsInvalid()
    {
        var validator = new UpdateTaskItemDtoValidator();
        var model = new UpdateTaskItemDto { Status = 4 };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    #endregion
}

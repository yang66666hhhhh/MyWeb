using FluentValidation;
using WebApplication1.Features.Content.Dtos;

namespace WebApplication1.Features.Content.Validators;

public class CreateArticleDtoValidator : AbstractValidator<CreateArticleDto>
{
    public CreateArticleDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题长度不能超过200");

        RuleFor(x => x.Content)
            .MaximumLength(50000).WithMessage("内容长度不能超过50000")
            .When(x => x.Content != null);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("状态不能为空")
            .Must(x => x == "draft" || x == "published" || x == "archived")
            .WithMessage("状态必须是 draft、published 或 archived");

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("标签长度不能超过500")
            .When(x => x.Tags != null);

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("分类长度不能超过100")
            .When(x => x.Category != null);
    }
}

public class UpdateArticleDtoValidator : AbstractValidator<UpdateArticleDto>
{
    public UpdateArticleDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("标题长度不能超过200")
            .When(x => x.Title != null);

        RuleFor(x => x.Content)
            .MaximumLength(50000).WithMessage("内容长度不能超过50000")
            .When(x => x.Content != null);

        RuleFor(x => x.Status)
            .Must(x => x == "draft" || x == "published" || x == "archived")
            .WithMessage("状态必须是 draft、published 或 archived")
            .When(x => x.Status != null);

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("标签长度不能超过500")
            .When(x => x.Tags != null);

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("分类长度不能超过100")
            .When(x => x.Category != null);
    }
}

public class CreateMediaItemDtoValidator : AbstractValidator<CreateMediaItemDto>
{
    public CreateMediaItemDtoValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("文件名不能为空")
            .MaximumLength(200).WithMessage("文件名长度不能超过200");

        RuleFor(x => x.FileUrl)
            .NotEmpty().WithMessage("文件URL不能为空")
            .MaximumLength(500).WithMessage("文件URL长度不能超过500")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("文件URL格式不正确");

        RuleFor(x => x.FileType)
            .NotEmpty().WithMessage("文件类型不能为空")
            .MaximumLength(50).WithMessage("文件类型长度不能超过50");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("文件大小必须大于0")
            .LessThan(100 * 1024 * 1024).WithMessage("文件大小不能超过100MB");

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("标签长度不能超过500")
            .When(x => x.Tags != null);
    }
}

public class UpdateMediaItemDtoValidator : AbstractValidator<UpdateMediaItemDto>
{
    public UpdateMediaItemDtoValidator()
    {
        RuleFor(x => x.FileName)
            .MaximumLength(200).WithMessage("文件名长度不能超过200")
            .When(x => x.FileName != null);

        RuleFor(x => x.FileUrl)
            .MaximumLength(500).WithMessage("文件URL长度不能超过500")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("文件URL格式不正确")
            .When(x => x.FileUrl != null);

        RuleFor(x => x.FileType)
            .MaximumLength(50).WithMessage("文件类型长度不能超过50")
            .When(x => x.FileType != null);

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("文件大小必须大于0")
            .LessThan(100 * 1024 * 1024).WithMessage("文件大小不能超过100MB")
            .When(x => x.FileSize.HasValue);

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("标签长度不能超过500")
            .When(x => x.Tags != null);
    }
}

public class CreatePublishingCalendarDtoValidator : AbstractValidator<CreatePublishingCalendarDto>
{
    public CreatePublishingCalendarDtoValidator()
    {
        RuleFor(x => x.PlannedDate)
            .NotEmpty().WithMessage("计划日期不能为空")
            .MaximumLength(20).WithMessage("计划日期长度不能超过20");

        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("平台不能为空")
            .MaximumLength(50).WithMessage("平台长度不能超过50");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题长度不能超过200");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("状态不能为空")
            .Must(x => x == "pending" || x == "published" || x == "cancelled")
            .WithMessage("状态必须是 pending、published 或 cancelled");
    }
}

public class UpdatePublishingCalendarDtoValidator : AbstractValidator<UpdatePublishingCalendarDto>
{
    public UpdatePublishingCalendarDtoValidator()
    {
        RuleFor(x => x.PlannedDate)
            .MaximumLength(20).WithMessage("计划日期长度不能超过20")
            .When(x => x.PlannedDate != null);

        RuleFor(x => x.Platform)
            .MaximumLength(50).WithMessage("平台长度不能超过50")
            .When(x => x.Platform != null);

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("标题长度不能超过200")
            .When(x => x.Title != null);

        RuleFor(x => x.Status)
            .Must(x => x == "pending" || x == "published" || x == "cancelled")
            .WithMessage("状态必须是 pending、published 或 cancelled")
            .When(x => x.Status != null);
    }
}

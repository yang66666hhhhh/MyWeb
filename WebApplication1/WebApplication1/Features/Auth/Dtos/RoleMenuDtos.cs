using System.ComponentModel.DataAnnotations;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Features.Auth.Dtos;

public class RoleMenuDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Component { get; set; }
    public int Sort { get; set; }
    public bool IsVisible { get; set; }
    public bool IsEnabled { get; set; }
    public string? Permission { get; set; }
    public string? Redirect { get; set; }
    public bool IsExternal { get; set; }
    public string? Badge { get; set; }
    public string? Tag { get; set; }
    public int MinRoleLevel { get; set; }
    public string? PersonaTag { get; set; }
    public bool IsBaseMenu { get; set; }
    public string MenuCategory { get; set; } = "General";
    public string? FeatureCode { get; set; }
    public List<RoleMenuDto> Children { get; set; } = new();
}

public class UpsertRoleMenuDto
{
    public Guid? ParentId { get; set; }

    [Required(ErrorMessage = "菜单名称不能为空")]
    [MaxLength(100, ErrorMessage = "菜单名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "菜单路径不能为空")]
    [MaxLength(200, ErrorMessage = "菜单路径不能超过200个字符")]
    public string Path { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "图标不能超过100个字符")]
    public string? Icon { get; set; }

    [MaxLength(300, ErrorMessage = "组件路径不能超过300个字符")]
    public string? Component { get; set; }

    public int Sort { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;

    [MaxLength(100, ErrorMessage = "权限码不能超过100个字符")]
    public string? Permission { get; set; }

    [MaxLength(200, ErrorMessage = "重定向地址不能超过200个字符")]
    public string? Redirect { get; set; }

    public bool IsExternal { get; set; }

    [MaxLength(100, ErrorMessage = "徽标不能超过100个字符")]
    public string? Badge { get; set; }

    [MaxLength(100, ErrorMessage = "标签不能超过100个字符")]
    public string? Tag { get; set; }

    [Range(1, 3, ErrorMessage = "角色等级必须是 1-3")]
    public int MinRoleLevel { get; set; } = 1;

    [MaxLength(100, ErrorMessage = "Persona 标识不能超过100个字符")]
    public string? PersonaTag { get; set; }

    public bool IsBaseMenu { get; set; } = true;

    [MaxLength(50, ErrorMessage = "菜单分类不能超过50个字符")]
    public string MenuCategory { get; set; } = "General";

    [MaxLength(100, ErrorMessage = "功能码不能超过100个字符")]
    public string? FeatureCode { get; set; }

    public RoleMenu ToEntity()
    {
        return new RoleMenu
        {
            ParentId = ParentId,
            Name = Name,
            Path = Path,
            Icon = Icon,
            Component = Component,
            Sort = Sort,
            IsVisible = IsVisible,
            IsEnabled = IsEnabled,
            Permission = Permission,
            Redirect = Redirect,
            IsExternal = IsExternal,
            Badge = Badge,
            Tag = Tag,
            MinRoleLevel = MinRoleLevel,
            PersonaTag = PersonaTag,
            IsBaseMenu = IsBaseMenu,
            MenuCategory = MenuCategory,
            FeatureCode = FeatureCode
        };
    }
}

public static class RoleMenuDtoMapper
{
    public static RoleMenuDto ToDto(this RoleMenu menu)
    {
        return new RoleMenuDto
        {
            Id = menu.Id,
            ParentId = menu.ParentId,
            Name = menu.Name,
            Path = menu.Path,
            Icon = menu.Icon,
            Component = menu.Component,
            Sort = menu.Sort,
            IsVisible = menu.IsVisible,
            IsEnabled = menu.IsEnabled,
            Permission = menu.Permission,
            Redirect = menu.Redirect,
            IsExternal = menu.IsExternal,
            Badge = menu.Badge,
            Tag = menu.Tag,
            MinRoleLevel = menu.MinRoleLevel,
            PersonaTag = menu.PersonaTag,
            IsBaseMenu = menu.IsBaseMenu,
            MenuCategory = menu.MenuCategory,
            FeatureCode = menu.FeatureCode,
            Children = menu.Children.Select(ToDto).ToList()
        };
    }
}

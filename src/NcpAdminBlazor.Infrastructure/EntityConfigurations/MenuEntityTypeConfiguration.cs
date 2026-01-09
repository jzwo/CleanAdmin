using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal sealed class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("菜单ID");

        builder.Property(x => x.MenuName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("菜单名称");

        builder.Property(x => x.MenuType)
            .IsRequired()
            .HasColumnType("int")
            .HasComment("菜单类型");

        builder.Property(x => x.ParentId)
            .HasComment("父菜单ID");

        builder.Property(x => x.RoutePath)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("路由路径");

        builder.Property(x => x.ComponentPath)
            .HasMaxLength(255)
            .HasComment("组件路径");

        builder.Property(x => x.Icon)
            .HasMaxLength(100)
            .HasComment("菜单图标");

        builder.Property(x => x.SortOrder)
            .IsRequired()
            .HasComment("排序号");

        builder.Property(x => x.IsExternal)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("是否为外链");

        builder.Property(x => x.IsVisible)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("是否可见");

        builder.Property(x => x.PermissionCode)
            .HasMaxLength(100)
            .HasComment("权限码");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(MenuStatus.Enabled)
            .HasComment("菜单状态");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasComment("创建时间");

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasComment("是否删除");

        builder.Property(x => x.DeletedAt)
            .IsRequired()
            .HasComment("删除时间");

        // 索引定义
        builder.HasIndex(x => x.ParentId);

        builder.HasIndex(x => new { x.ParentId, x.SortOrder });

        builder.HasIndex(x => x.RoutePath);

        builder.HasIndex(x => x.PermissionCode);

        builder.HasIndex(x => x.IsDeleted);
    }
}

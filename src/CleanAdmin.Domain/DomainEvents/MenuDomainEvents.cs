using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.Domain.DomainEvents;

/// <summary>
/// 菜单创建领域事件
/// </summary>
public record MenuCreatedDomainEvent(Menu Menu) : IDomainEvent;

/// <summary>
/// 菜单更新领域事件
/// </summary>
public record MenuUpdatedDomainEvent(Menu Menu) : IDomainEvent;

/// <summary>
/// 菜单删除领域事件
/// </summary>
public record MenuDeletedDomainEvent(Menu Menu) : IDomainEvent;

/// <summary>
/// 菜单排序号更改领域事件
/// </summary>
public record MenuSortOrderChangedDomainEvent(Menu Menu) : IDomainEvent;

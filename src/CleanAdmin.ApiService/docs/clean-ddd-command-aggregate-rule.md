# CleanDDD Command-Aggregate Rule

## 规则

- 一个 Command 只对应一个聚合方法调用。
- 构造函数属于聚合方法，创建场景应在构造函数中一次性完成聚合状态初始化。
- 更新场景应通过单一聚合行为方法一次性完成状态变更。
- 不要在同一个 Command 中先调用一个聚合方法，再调用第二个聚合方法补充状态（例如先 `new User(...)`，再 `AssignPermissions(...)`）。

## 落地方式

- 将 Command 所需的完整状态作为参数传入同一个聚合方法。
- 角色和权限等衍生数据可以在 Command Handler 中计算，但最终应一次性传给聚合方法。

## 示例

- 推荐：`new User(..., userRoles, userPermissions)`。
- 推荐：`user.UpdateInfo(..., userRoles, userPermissions)`。
- 不推荐：`new User(..., userRoles)` 后再 `user.AssignPermissions(userPermissions)`。

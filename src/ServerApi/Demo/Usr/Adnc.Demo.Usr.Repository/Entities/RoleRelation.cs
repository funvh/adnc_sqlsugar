namespace Adnc.Demo.Usr.Repository.Entities;

/// <summary>
/// 菜单角色关系
/// </summary>
[SugarTable("sys_rolerelation")]
public class RoleRelation : SqlSugarEntity
{
    public long MenuId { get; set; }

    public long RoleId { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(MenuId))]
    public virtual Menu Menu { get; set; } = default!;
}
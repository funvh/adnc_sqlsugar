namespace Adnc.Infra.IRepository.SqlSugar.Attributes
{
    /// <summary>
    /// 需要删除的导航属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NeedDeleteNavigationAttribute : Attribute
    {
    }
}

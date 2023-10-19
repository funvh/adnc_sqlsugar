namespace Adnc.Infra.IRepository
{
    public class Entity : IEntity<long>
    {
        public Entity()
        {

        }

        public virtual long Id { get; set; }
    }

    public class Entity<T> : IEntity<T> where T : struct
    {
        public virtual T Id { get; set; }
    }
}
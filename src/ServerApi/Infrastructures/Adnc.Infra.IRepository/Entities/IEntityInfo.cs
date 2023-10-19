using Adnc.Infra.IRepositories;

namespace Adnc.Infra.IRepository;  

public interface IEntityInfo
{
    Operater GetOperater();

    void OnModelCreating(dynamic modelBuilder);
}
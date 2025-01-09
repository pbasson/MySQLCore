using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MySQLCore.Infrastructure.Entities.Tables;

namespace MySQLCore.Infrastructure.Models;

public class MySQLCoreDBContext(DbContextOptions<MySQLCoreDBContext> option) : DbContext(option)
{
    public virtual DbSet<CRUDTransaction> CRUDTransaction {get; set;}


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetChanges();
        return base.SaveChangesAsync();
    }

    public override int SaveChanges()
    {
        SetChanges();
        return base.SaveChanges();
    }

    private void SetChanges()
    {
        foreach (var entry in ChangeTracker.Entries<BaseModel>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreatedChange(entry);
                    SetUpdatedChange(entry);
                    break;
                case EntityState.Modified:
                    SetUpdatedChange(entry);
                    break;
            }
        }
    }

    private const string User = "Preetpal Basson";
    private static void SetCreatedChange(EntityEntry<BaseModel> entry)
    {
        entry.Entity.CreatedDateTime = DateTime.Now;
        entry.Entity.CreatedBy = User;
    }
    
    private static void SetUpdatedChange(EntityEntry<BaseModel> entry)
    {
        entry.Entity.UpdatedDateTime = DateTime.Now;
        entry.Entity.UpdatedBy = User;
    }
}

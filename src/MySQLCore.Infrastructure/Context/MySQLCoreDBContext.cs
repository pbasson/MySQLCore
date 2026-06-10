using MySQLCore.Core.Messager.Models;

namespace MySQLCore.Infrastructure.Context;

public class MySQLCoreDBContext(DbContextOptions<MySQLCoreDBContext> option) : DbContext(option)
{
    public virtual DbSet<User> User {get; set;}
    public virtual DbSet<ImageGallery> ImageGallery {get; set;}
    public virtual DbSet<ImageFile> ImageFile {get; set;}
    public virtual DbSet<ProcessedMessage> ProcessedMessage {get; set;}
    public virtual DbSet<OutboxMessage> OutboxMessage {get; set;}

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

    private const string userBy = "Preetpal Basson";
    private static void SetCreatedChange(EntityEntry<BaseModel> entry)
    {
        entry.Entity.CreatedDateTime = DateTime.Now;
        entry.Entity.CreatedBy = userBy;
    }
    
    private static void SetUpdatedChange(EntityEntry<BaseModel> entry)
    {
        entry.Entity.UpdatedDateTime = DateTime.Now;
        entry.Entity.UpdatedBy = userBy;
    }
}

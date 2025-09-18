using Contatos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contatos.Infrastructure.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Contact>(cfg =>
        {
            cfg.ToTable("Contacts");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(120);

            cfg.Property(x => x.Sex)
               .IsRequired();

            cfg.Property(x => x.IsActive)
               .HasDefaultValue(true);

            cfg.Property(x => x.BirthDate).HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v)
            );
        });

        b.Entity<Contact>().HasQueryFilter(x => x.IsActive);
    }
}

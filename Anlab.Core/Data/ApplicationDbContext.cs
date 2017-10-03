using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Anlab.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public override int SaveChanges()
        {
            UpdateDates();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateDates();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateDates();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        

        private void UpdateDates()
        {
            var entitiesUpdated = ChangeTracker.Entries<IDatedEntity>()
                .Where(a => a.State == EntityState.Modified).Select(a => a.Entity).ToList();
            var entitiesCreated = ChangeTracker.Entries<IDatedEntity>()
                .Where(a => a.State == EntityState.Added).Select(a => a.Entity).ToList();
            var now = DateTime.UtcNow;
            foreach (var datedEntity in entitiesUpdated)
            {
                datedEntity.Updated = now;
            }
            foreach (var datedEntity in entitiesCreated)
            {
                datedEntity.Updated = now;
                datedEntity.Created = now;
            }
        }


        [Obsolete("Just use for tests")]
        public ApplicationDbContext() { }

        public new virtual DbSet<User> Users {get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<TestItem> TestItems { get; set; }

        public virtual DbSet<AnalysisMethod> AnalysisMethods { get; set; }

        public virtual DbSet<PaymentEvent> PaymentEvents { get; set; }

        public virtual DbSet<MailMessage> MailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // prevent duplicate FKs when running migrations
            //builder.Entity<User>()
            //    .HasMany(e => e.Roles)
            //    .WithOne()
            //    .HasForeignKey(e => e.UserId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

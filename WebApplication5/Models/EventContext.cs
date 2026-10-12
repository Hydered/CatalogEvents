using System.Data.Entity;

namespace WebApplication5.Models
{
    public class EventContext : DbContext
    {
        static EventContext()
        {
            Database.SetInitializer(new EventInitializer());
        }

        public EventContext() : base("EventContext")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<Event>()
                .HasRequired(item => item.Category)
                .WithMany(category => category.Events)
                .HasForeignKey(item => item.CategoryId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
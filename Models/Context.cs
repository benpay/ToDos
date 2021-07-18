using Microsoft.EntityFrameworkCore;

namespace ToDos.Models
{
    public class Context : DbContext
    {
        public DbSet<ToDoModel> ToDo { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(@"Data source=C:\\temp\\ToDoDatabase.db");
    }
}

using GrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
    }
}

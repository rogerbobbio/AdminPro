using Microsoft.EntityFrameworkCore;

namespace AdminPro.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PWEB_P2.Models;

namespace PWEB_P2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Curso> Cursos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
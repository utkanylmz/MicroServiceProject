using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.Api.Infrastructure.Context
{
    public partial class CatalogContext
    {
        public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext>
        {
            public CatalogContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>()
                    .UseSqlServer("Server=DESKTOP-O1SR9H9;Database=MicroServiceProject; Trusted_Connection=True;TrustServerCertificate=True;");

                return new CatalogContext(optionsBuilder.Options);
            }
        }
    }
}

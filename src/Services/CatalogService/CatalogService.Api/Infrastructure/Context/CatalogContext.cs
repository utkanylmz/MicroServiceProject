using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.Context
{
    public partial class CatalogContext:DbContext
    {
        public const string Default_Shema = "catalog";

        public CatalogContext(DbContextOptions<CatalogContext>options):base(options)
        {
            
        }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OnModelCreating metodunda veritabanında ki tabloların ve o tabloların kolonlarını configüre ettiğimiz metot
            // burada EntityConfigurations dosyasındakı configurasyonları uygula diyoruz.
            modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
        }
    }
}

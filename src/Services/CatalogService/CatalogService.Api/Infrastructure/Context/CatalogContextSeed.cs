using CatalogService.Api.Core.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
        {
            //Dışardan Çağıralacak metotdumuz eğer bir sqlException alırsan 3 kere dene
            var policy = Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on " +
                        "attempt {retry} of {retries}", nameof(CatalogContextSeed), exception.GetType().Name, exception.Message, retry, 3);
                }
                );
            //Buraya git bu benim setup klasörüm olacak 
            var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
            var picturePath = "Pics";
            //ProcessSeeding e gönderecek ve veri ekleme işlemleri çalışacak
            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));
        }

        private static async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger logger)
        {
            //Veri Tabanına git giç veri yoksa  GetCatalogBrandsFromFile metodunu çalıştır.
            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogTypes.Any())
            {
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogItems.Any())
            {
                await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath, context));
                await context.SaveChangesAsync();

                GetCatalogItemPictures(setupDirPath, picturePath);
            }
        }

        private static IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
        {
            static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
            {
                return new List<CatalogBrand>()
            {
                new() { Brand = "Azure"},
                new() { Brand = ".NET" },
                new() { Brand = "Visual Studio" },
                new() { Brand = "SQL Server" },
                new() { Brand = "Other" }
            };
            }

            var fileName = Path.Combine(contentPath, "BrandsTextFile.txt");

            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogBrands();
            }

            var fileContent = File.ReadAllLines(fileName);

            var list = fileContent.Select(i => new CatalogBrand()
            {
                Brand = i.Trim('"'),
            }).Where(x => x != null);

            return list ?? GetPreconfiguredCatalogBrands();
        }

        private static IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath)
        {
            static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
            {
                return new List<CatalogType>()
            {
                new() { Type = "Mug"},
                new() { Type = "T-Shirt" },
                new() { Type = "Sheet" },
                new() { Type = "USB Memory Stick" }
            };
            }

            string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogTypes();
            }

            var fileContent = File.ReadAllLines(fileName);

            var list = fileContent.Select(x => new CatalogType()
            {
                Type = x.Trim('"')
            }).Where(x => x != null);

            return list ?? GetPreconfiguredCatalogTypes();
        }

        private static IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
        {
           
            static IEnumerable<CatalogItem> GetPreconfiguredItems()
            {
                return new List<CatalogItem>()
            {
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
                new() { CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureFileName = "2.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
                new() { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" },
            };
            }

            string fileName = Path.Combine(contentPath, "CatalogItems.txt");
            //CatalogItems.txt diye bir dosya bulamazsan GetPreconfiguredItems metodunu çağır (inline metot) dosya bulunamazsa bile 
            //metot içindeki verileri veri tabanına atıyor
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredItems();
            }

            var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);
            // dosyayı bulursa dosyayı okumasını gerçekleştiyoruz
            var fileContent = File.ReadAllLines(fileName)
                        .Skip(1) //ilk satırı atla
                        .Select(row => row.Split(','))//virgülle ayır
                        .Select(column =>
                        {
                            return new CatalogItem()
                            {
                                CatalogTypeId = catalogTypeIdLookup[column[0]],
                                CatalogBrandId = catalogBrandIdLookup[column[1]],
                                Description = column[2].Trim('"').Trim(),
                                Name = column[3].Trim('"').Trim(),
                                Price = Decimal.Parse(column[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                                PictureFileName = column[5].Trim('"').Trim(),
                                AvailableStock = string.IsNullOrEmpty(column[6]) ? 0 : int.Parse(column[6]),
                                OnReorder = Convert.ToBoolean(column[7])
                            };
                        });

            return fileContent;
        }

        private static void GetCatalogItemPictures(string contentPath, string picturePath)
        {
            //Dışarıdan bir path gönderilmezse ana klasör tarafında pics isminde bir klasör oluşturulsun diyoruz pathteki klasörün
            //içine giriyoruz ve dosya varsa siliyoruz ve pathtteki klasöre zipteki dosyaları çıkartıyoruz.

            picturePath ??= "pics";

            if (picturePath != null)
            {
                var directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }

    }
}

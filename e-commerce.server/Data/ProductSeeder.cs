using CsvHelper;
using e_commerce.Data;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;

public static class ProductSeeder
{
    public class ProductCsvModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SellerId { get; set; }   
        public string CategoryName { get; set; }
        public string Images { get; set; }
    }
    public static async Task SeedProducts(ApplicationDbContext context)
    {
        if (context.Products.Any())
            return;

        // 🔥 Load categories once
        var categories = await context.Categories.ToListAsync();

        var categoryMap = categories.ToDictionary(
            c => c.Name,
            c => c.Id
        );

        using var reader = new StreamReader("Data/products.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ProductCsvModel>().ToList();

        var products = new List<Product>();

        foreach (var record in records)
        {
            var categoryKey = record.CategoryName;

            
            var product = new Product
            {
                Name = record.Name,
                Description = record.Description,
                Price = record.Price,
                Stock = record.Stock,
                SellerId = record.SellerId,
                CategoryId = categoryMap[categoryKey],
                CreatedAt = DateTime.UtcNow
            };

            // 🔥 Add images
            List<string> imageUrls = new();

            if (!string.IsNullOrEmpty(record.Images))
            {
                try
                {
                    imageUrls = JsonSerializer.Deserialize<List<string>>(record.Images);
                }
                catch
                {
                    throw new Exception($"Failed to parse images for product '{record.Name}'. Ensure the 'Images' field is a valid JSON array of strings.");
                }
            }
            foreach (var url in imageUrls)
            {
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                product.Images.Add(new ProductImage
                {
                    ImageUrl = url
                });
            }

            products.Add(product);
        }

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
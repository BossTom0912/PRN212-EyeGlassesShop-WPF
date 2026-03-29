using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations
{
    public class ProductRepository : IProductRepository
    {
        public List<VwProductVariantList> GetAllProducts()
        {
            using (var context = new GlassesShopContext())
            {
                return context.VwProductVariantLists
                    .Where(p => p.ProductIsActive == true
                             && p.VariantIsActive == true
                             && p.StockQuantity > 0)
                    .OrderByDescending(p => p.ProductId)
                    .ThenByDescending(p => p.VariantId)
                    .ToList();
            }
        }

        public List<VwProductVariantList> GetAllProductsForAdmin(bool includeInactive = true)
        {
            using (var context = new GlassesShopContext())
            {
                var query = context.VwProductVariantLists.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(p => p.ProductIsActive == true && p.VariantIsActive == true);
                }

                return query
                    .OrderByDescending(p => p.ProductId)
                    .ThenByDescending(p => p.VariantId)
                    .ToList();
            }
        }

        public List<Brand> GetActiveBrands()
        {
            using (var context = new GlassesShopContext())
            {
                return context.Brands
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .ToList();
            }
        }

        public List<Category> GetActiveCategories()
        {
            using (var context = new GlassesShopContext())
            {
                return context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToList();
            }
        }

        public Product? GetProductById(long productId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Products.FirstOrDefault(p => p.Id == productId);
            }
        }

        public ProductVariant? GetVariantById(long variantId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.ProductVariants.FirstOrDefault(v => v.Id == variantId);
            }
        }

        public bool SkuExists(string sku, long? excludeVariantId = null)
        {
            using (var context = new GlassesShopContext())
            {
                string normalizedSku = sku.Trim().ToUpper();

                return context.ProductVariants.Any(v =>
                    v.Sku.ToUpper() == normalizedSku &&
                    (!excludeVariantId.HasValue || v.Id != excludeVariantId.Value));
            }
        }

        public void CreateProductWithVariant(Product product, ProductVariant variant)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Products.Add(product);
                    context.SaveChanges();

                    variant.ProductId = product.Id;
                    context.ProductVariants.Add(variant);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateProductAndVariant(Product product, ProductVariant variant)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Products.Update(product);
                    context.ProductVariants.Update(variant);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void SoftDeleteProduct(long productId)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var product = context.Products
                        .Include(p => p.ProductVariants)
                        .FirstOrDefault(p => p.Id == productId);

                    if (product == null)
                        throw new Exception("Không tìm thấy sản phẩm.");

                    var now = DateTime.UtcNow;

                    product.IsActive = false;
                    product.UpdatedAt = now;

                    foreach (var variant in product.ProductVariants)
                    {
                        variant.IsActive = false;
                        variant.UpdatedAt = now;
                    }

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public List<VwProductVariantList> GetAllVariantsForAdmin(bool includeInactive = true)
        {
            using (var context = new GlassesShopContext())
            {
                var query = context.VwProductVariantLists.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(v => v.ProductIsActive == true && v.VariantIsActive == true);
                }

                return query
                    .OrderBy(v => v.ProductName)
                    .ThenBy(v => v.Color)
                    .ThenBy(v => v.Size)
                    .ThenBy(v => v.VariantId)
                    .ToList();
            }
        }
        public List<Product> GetActiveProducts()
        {
            using (var context = new GlassesShopContext())
            {
                return context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToList();
            }
        }
        public void CreateVariant(ProductVariant variant)
        {
            using (var context = new GlassesShopContext())
            {
                context.ProductVariants.Add(variant);
                context.SaveChanges();
            }
        }
        public void UpdateVariant(ProductVariant variant)
        {
            using (var context = new GlassesShopContext())
            {
                context.ProductVariants.Update(variant);
                context.SaveChanges();
            }
        }public void SoftDeleteVariant(long variantId)
{
    using (var context = new GlassesShopContext())
    {
        var variant = context.ProductVariants.FirstOrDefault(v => v.Id == variantId);

        if (variant == null)
            throw new Exception("Không tìm thấy biến thể.");

        variant.IsActive = false;
        variant.UpdatedAt = DateTime.UtcNow;

        context.SaveChanges();
    }
}

    }

}
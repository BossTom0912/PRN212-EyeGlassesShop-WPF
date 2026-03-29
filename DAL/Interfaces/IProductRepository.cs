using DAL.Models;

namespace DAL.Interfaces
{
    public interface IProductRepository
    {
        List<VwProductVariantList> GetAllProducts();

        List<VwProductVariantList> GetAllProductsForAdmin(bool includeInactive = true);

        List<Brand> GetActiveBrands();

        List<Category> GetActiveCategories();

        Product? GetProductById(long productId);

        ProductVariant? GetVariantById(long variantId);

        bool SkuExists(string sku, long? excludeVariantId = null);

        void CreateProductWithVariant(Product product, ProductVariant variant);

        void UpdateProductAndVariant(Product product, ProductVariant variant);

        void SoftDeleteProduct(long productId);

        List<VwProductVariantList> GetAllVariantsForAdmin(bool includeInactive = true);

        List<Product> GetActiveProducts();

        void CreateVariant(ProductVariant variant);

        void UpdateVariant(ProductVariant variant);

        void SoftDeleteVariant(long variantId);
    }
}
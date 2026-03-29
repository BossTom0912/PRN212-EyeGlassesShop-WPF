using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService()
        {
            _productRepo = new ProductRepository();
        }

        public List<VwProductVariantList> GetAllProducts()
        {
            return _productRepo.GetAllProducts();
        }

        public List<VwProductVariantList> GetAllProductsForAdmin(bool includeInactive = true)
        {
            return _productRepo.GetAllProductsForAdmin(includeInactive);
        }

        public List<Brand> GetActiveBrands()
        {
            return _productRepo.GetActiveBrands();
        }

        public List<Category> GetActiveCategories()
        {
            return _productRepo.GetActiveCategories();
        }

        public Product? GetProductById(long productId)
        {
            return _productRepo.GetProductById(productId);
        }

        public ProductVariant? GetVariantById(long variantId)
        {
            return _productRepo.GetVariantById(variantId);
        }

        public void CreateProductWithDefaultVariant(
            string productName,
            string? description,
            long categoryId,
            long brandId,
            string? baseImageUrl,
            string sku,
            string color,
            string size,
            string? material,
            decimal price,
            int stockQuantity,
            string? variantImageUrl)
        {
            ValidateProductInput(productName, categoryId, brandId, sku, color, size, price, stockQuantity);

            if (_productRepo.SkuExists(sku))
                throw new Exception("SKU đã tồn tại. Vui lòng nhập SKU khác.");

            var now = DateTime.UtcNow;

            var product = new Product
            {
                Name = productName.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                CategoryId = categoryId,
                BrandId = brandId,
                BaseImageUrl = string.IsNullOrWhiteSpace(baseImageUrl) ? null : baseImageUrl.Trim(),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            var variant = new ProductVariant
            {
                Sku = sku.Trim(),
                Color = color.Trim(),
                Size = size.Trim(),
                Material = string.IsNullOrWhiteSpace(material) ? null : material.Trim(),
                Price = price,
                StockQuantity = stockQuantity,
                ImageUrl = string.IsNullOrWhiteSpace(variantImageUrl) ? null : variantImageUrl.Trim(),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            _productRepo.CreateProductWithVariant(product, variant);
        }

        public void UpdateProductAndVariant(
            long productId,
            long variantId,
            string productName,
            string? description,
            long categoryId,
            long brandId,
            string? baseImageUrl,
            string sku,
            string color,
            string size,
            string? material,
            decimal price,
            int stockQuantity,
            string? variantImageUrl,
            bool productIsActive,
            bool variantIsActive)
        {
            ValidateProductInput(productName, categoryId, brandId, sku, color, size, price, stockQuantity);

            if (_productRepo.SkuExists(sku, variantId))
                throw new Exception("SKU đã tồn tại. Vui lòng nhập SKU khác.");

            var product = _productRepo.GetProductById(productId);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm cần cập nhật.");

            var variant = _productRepo.GetVariantById(variantId);
            if (variant == null)
                throw new Exception("Không tìm thấy biến thể cần cập nhật.");

            var now = DateTime.UtcNow;

            product.Name = productName.Trim();
            product.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            product.CategoryId = categoryId;
            product.BrandId = brandId;
            product.BaseImageUrl = string.IsNullOrWhiteSpace(baseImageUrl) ? null : baseImageUrl.Trim();
            product.IsActive = productIsActive;
            product.UpdatedAt = now;

            variant.Sku = sku.Trim();
            variant.Color = color.Trim();
            variant.Size = size.Trim();
            variant.Material = string.IsNullOrWhiteSpace(material) ? null : material.Trim();
            variant.Price = price;
            variant.StockQuantity = stockQuantity;
            variant.ImageUrl = string.IsNullOrWhiteSpace(variantImageUrl) ? null : variantImageUrl.Trim();
            variant.IsActive = variantIsActive;
            variant.UpdatedAt = now;

            _productRepo.UpdateProductAndVariant(product, variant);
        }

        public void SoftDeleteProduct(long productId)
        {
            _productRepo.SoftDeleteProduct(productId);
        }

        private void ValidateProductInput(
            string productName,
            long categoryId,
            long brandId,
            string sku,
            string color,
            string size,
            decimal price,
            int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new Exception("Tên sản phẩm không được để trống.");

            if (categoryId <= 0)
                throw new Exception("Vui lòng chọn danh mục.");

            if (brandId <= 0)
                throw new Exception("Vui lòng chọn thương hiệu.");

            if (string.IsNullOrWhiteSpace(sku))
                throw new Exception("SKU không được để trống.");

            if (string.IsNullOrWhiteSpace(color))
                throw new Exception("Màu sắc không được để trống.");

            if (string.IsNullOrWhiteSpace(size))
                throw new Exception("Kích thước không được để trống.");

            if (price < 0)
                throw new Exception("Giá không được âm.");

            if (stockQuantity < 0)
                throw new Exception("Số lượng tồn không được âm.");
        }
        public List<VwProductVariantList> GetAllVariantsForAdmin(bool includeInactive = true)
        {
            return _productRepo.GetAllVariantsForAdmin(includeInactive);
        }
        public List<Product> GetActiveProducts()
        {
            return _productRepo.GetActiveProducts();
        }
        public void CreateVariant(
    long productId,
    string sku,
    string color,
    string size,
    string? material,
    decimal price,
    int stockQuantity,
    string? imageUrl)
        {
            ValidateVariantInput(sku, color, size, price, stockQuantity);

            var product = _productRepo.GetProductById(productId);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm để thêm biến thể.");

            if (!product.IsActive)
                throw new Exception("Không thể thêm biến thể vào sản phẩm đang bị ẩn.");

            if (_productRepo.SkuExists(sku))
                throw new Exception("SKU đã tồn tại. Vui lòng nhập SKU khác.");

            var now = DateTime.UtcNow;

            var variant = new ProductVariant
            {
                ProductId = productId,
                Sku = sku.Trim(),
                Color = color.Trim(),
                Size = size.Trim(),
                Material = string.IsNullOrWhiteSpace(material) ? null : material.Trim(),
                Price = price,
                StockQuantity = stockQuantity,
                ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim(),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            _productRepo.CreateVariant(variant);
        }
        public void UpdateVariant(
    long variantId,
    string sku,
    string color,
    string size,
    string? material,
    decimal price,
    int stockQuantity,
    string? imageUrl,
    bool isActive)
        {
            ValidateVariantInput(sku, color, size, price, stockQuantity);

            if (_productRepo.SkuExists(sku, variantId))
                throw new Exception("SKU đã tồn tại. Vui lòng nhập SKU khác.");

            var variant = _productRepo.GetVariantById(variantId);
            if (variant == null)
                throw new Exception("Không tìm thấy biến thể cần cập nhật.");

            variant.Sku = sku.Trim();
            variant.Color = color.Trim();
            variant.Size = size.Trim();
            variant.Material = string.IsNullOrWhiteSpace(material) ? null : material.Trim();
            variant.Price = price;
            variant.StockQuantity = stockQuantity;
            variant.ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
            variant.IsActive = isActive;
            variant.UpdatedAt = DateTime.UtcNow;

            _productRepo.UpdateVariant(variant);
        }
        public void SoftDeleteVariant(long variantId)
        {
            _productRepo.SoftDeleteVariant(variantId);
        }
        private void ValidateVariantInput(
    string sku,
    string color,
    string size,
    decimal price,
    int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new Exception("SKU không được để trống.");

            if (string.IsNullOrWhiteSpace(color))
                throw new Exception("Màu sắc không được để trống.");

            if (string.IsNullOrWhiteSpace(size))
                throw new Exception("Kích thước không được để trống.");

            if (price < 0)
                throw new Exception("Giá không được âm.");

            if (stockQuantity < 0)
                throw new Exception("Tồn kho không được âm.");
        }
    }
}
using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;

        public CartService()
        {
            _cartRepo = new CartRepository();
        }

        public void AddToCart(int accountId, int variantId, int quantity, decimal unitPrice)
        {
            try
            {
                if (quantity <= 0)
                    throw new Exception("Số lượng thêm vào giỏ không hợp lệ.");

                var cart = _cartRepo.GetCartByAccountId(accountId);
                if (cart == null) throw new Exception("Không tìm thấy giỏ hàng của bạn!");

                ProductService productService = new ProductService();
                var productInfo = productService.GetAllProducts().FirstOrDefault(p => p.VariantId == variantId);

                if (productInfo == null)
                    throw new Exception("Sản phẩm không tồn tại hoặc đã ngừng bán.");

                int stock = productInfo.StockQuantity ?? 0;
                if (stock <= 0)
                    throw new Exception("Sản phẩm này đã hết hàng.");

                var existingItem = _cartRepo.GetCartItem((int)cart.Id, variantId);
                int currentQty = existingItem?.Quantity ?? 0;
                int newQty = currentQty + quantity;

                if (newQty > stock)
                    throw new Exception($"Chỉ còn {stock} sản phẩm trong kho.");

                if (existingItem != null)
                {
                    existingItem.Quantity = newQty;
                    existingItem.UnitPrice = unitPrice;
                    _cartRepo.UpdateCartItem(existingItem);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = (int)cart.Id,
                        ProductVariantId = variantId,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    };
                    _cartRepo.AddCartItem(newItem);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                if (ex.InnerException != null) error += "\nDB Lỗi: " + ex.InnerException.Message;
                throw new Exception(error);
            }
        }

        public List<CartItemDisplay> GetCartSummary(int accountId)
        {
            var cart = _cartRepo.GetCartByAccountId(accountId);
            if (cart == null) return new List<CartItemDisplay>();

            var cartItems = _cartRepo.GetAllCartItems((int)cart.Id);

            ProductService productService = new ProductService();
            var allProducts = productService.GetAllProducts();

            var result = new List<CartItemDisplay>();

            foreach (var item in cartItems)
            {
                var productInfo = allProducts.FirstOrDefault(p => p.VariantId == item.ProductVariantId);
                if (productInfo != null)
                {
                    result.Add(new CartItemDisplay
                    {
                        ProductVariantId = (int)item.ProductVariantId,
                        ProductName = productInfo.ProductName!,
                        Color = productInfo.Color!,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Quantity * item.UnitPrice
                    });
                }
            }
            return result;
        }

        public void UpdateQuantity(int accountId, int variantId, int quantityChange)
        {
            var cart = _cartRepo.GetCartByAccountId(accountId);
            if (cart == null) return;

            var existingItem = _cartRepo.GetCartItem((int)cart.Id, variantId);
            if (existingItem == null) return;

            int newQty = existingItem.Quantity + quantityChange;

            if (quantityChange > 0)
            {
                ProductService productService = new ProductService();
                var productInfo = productService.GetAllProducts().FirstOrDefault(p => p.VariantId == variantId);

                if (productInfo == null)
                    throw new Exception("Sản phẩm không còn khả dụng.");

                int stock = productInfo.StockQuantity ?? 0;
                if (newQty > stock)
                    throw new Exception($"Không thể tăng thêm. Chỉ còn {stock} sản phẩm trong kho.");
            }

            existingItem.Quantity = newQty;

            if (existingItem.Quantity <= 0)
            {
                _cartRepo.DeleteCartItem(existingItem);
            }
            else
            {
                _cartRepo.UpdateCartItem(existingItem);
            }
        }

        public void RemoveItemCompletely(int accountId, int variantId)
        {
            var cart = _cartRepo.GetCartByAccountId(accountId);
            if (cart == null) return;

            var existingItem = _cartRepo.GetCartItem((int)cart.Id, variantId);
            if (existingItem != null)
            {
                _cartRepo.DeleteCartItem(existingItem);
            }
        }
    }

    public class CartItemDisplay
    {
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = "";
        public string Color { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
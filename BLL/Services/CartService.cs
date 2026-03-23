using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;

        public CartService()
        {
            _cartRepo = new CartRepository();
        }

        // Thêm tham số decimal unitPrice vào hàm
        public void AddToCart(int accountId, int variantId, int quantity, decimal unitPrice)
        {
            try
            {
                var cart = _cartRepo.GetCartByAccountId(accountId);
                if (cart == null) throw new Exception("Không tìm thấy giỏ hàng của bạn!");

                var existingItem = _cartRepo.GetCartItem((int)cart.Id, variantId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
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
                        ProductName = productInfo.ProductName,
                        Color = productInfo.Color,
                        Quantity = (int)item.Quantity,
                        UnitPrice = (decimal)item.UnitPrice,
                        TotalPrice = (decimal)(item.Quantity * item.UnitPrice) 
                    });
                }
            }
            return result;
        }

        // 1. Hàm Tăng/Giảm số lượng (truyền +1 hoặc -1 vào)
        public void UpdateQuantity(int accountId, int variantId, int quantityChange)
        {
            var cart = _cartRepo.GetCartByAccountId(accountId);
            if (cart == null) return;

            var existingItem = _cartRepo.GetCartItem((int)cart.Id, variantId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantityChange;

                if (existingItem.Quantity <= 0)
                {
                    _cartRepo.DeleteCartItem(existingItem);
                }
                else
                {
                    _cartRepo.UpdateCartItem(existingItem);
                }
            }
        }

        // 2. Hàm Xóa luôn nha mấy ní
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
        public string ProductName { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

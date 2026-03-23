using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class CartRepository : ICartRepository
    {
        public Cart GetCartByAccountId(int accountId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Carts.FirstOrDefault(c => c.AccountId == accountId);
            }
        }

        public CartItem GetCartItem(long cartId, int variantId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.ProductVariantId == variantId);
            }
        }

        public void AddCartItem(CartItem item)
        {
            using (var context = new GlassesShopContext())
            {
                context.CartItems.Add(item);
                context.SaveChanges();
            }
        }

        public void UpdateCartItem(CartItem item)
        {
            using (var context = new GlassesShopContext())
            {
                context.CartItems.Update(item);
                context.SaveChanges();
            }
        }

        public List<CartItem> GetAllCartItems(int cartId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.CartItems.Where(ci => ci.CartId == cartId).ToList();
            }
        }
        public void DeleteCartItem(CartItem item)
        {
            using (var context = new GlassesShopContext())
            {
                context.CartItems.Remove(item);
                context.SaveChanges();
            }
        }
    }
}

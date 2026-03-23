using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCartByAccountId(int accountId);
        CartItem GetCartItem(long cartId, int variantId);
        void AddCartItem(CartItem item);
        void UpdateCartItem(CartItem item);
        List<CartItem> GetAllCartItems(int cartId);
        void DeleteCartItem(CartItem item);
    }
}

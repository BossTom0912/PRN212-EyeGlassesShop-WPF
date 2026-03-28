using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;

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
                    .ToList();
            }
        }
    }
}
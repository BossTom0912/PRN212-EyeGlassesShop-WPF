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
    public class ProductRepository : IProductRepository
    {
        public List<VwProductVariantList> GetAllProducts()
        {
            using (var context = new GlassesShopContext())
            {
                return context.VwProductVariantLists.ToList();
            }
        }
    }
}

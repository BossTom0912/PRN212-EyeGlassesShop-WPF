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
    }
}

﻿using Microsoft.IdentityModel.Tokens;
using Pharmacy.Data;

namespace Pharmacy.Models
{
    public class ProductModels
    {
        private readonly QlpharmacyContext _context;


        public ProductModels(QlpharmacyContext context)
        {
            _context = context;
        }


        public IEnumerable<Product> GetProducts(string search)
        {
            var ListProducts = _context.Products.OrderByDescending(s => s.ProductId).ToList();

            if (search != null)
            {
                List<Product> ProductFound = _context.Products.Where(item => item.ProductName.Contains(search)).ToList();

                return ProductFound;
            }

            return ListProducts;
        }

        public IEnumerable<Product> GetProductsActive(string search)
        {
            var ListProducts = _context.Products.Where(s=> s.ProductActive==true).OrderByDescending(s => s.ProductId ).ToList();

            if (search != null)
            {
                List<Product> ProductFound =  _context.Products
                                            .Where(item => item.ProductName.Contains(search))
                                            .ToList();
                return ProductFound;
            }

            return ListProducts;
        }

        public IEnumerable<Product> GetProductsPresciption()
        {
            var ListProducts = _context.Products.Where(s => s.ProductPrescription == false).OrderByDescending(s => s.ProductId).ToList();

            return ListProducts;
        }

        public IEnumerable<Product> GetProductsByCategoryId(int? id)
        {
            List<Product> listproduct = new List<Product>();
            if (id == 0)
            {
                listproduct = _context.Products.OrderByDescending(s => s.ProductId).ToList();
                return listproduct; 
            }
            listproduct = _context.Products.Where(p=> p.CategoryId == id && p.ProductActive == true).ToList();
            return listproduct;
        }

        public async Task CreatProduct(Product product)
        {
            _context.Products.Add(product);
           await _context.SaveChangesAsync();
        }

        public async void DeleteProduct(int id)
        {
            var item = _context.Products.Find(id);
            _context.Products.Remove(item);
           await _context.SaveChangesAsync();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }

        public async Task EditProductAsync(Product product, string url)
        {
            var updateitem = _context.Products.Find(product.ProductId);
            if( url != null)
            {
                updateitem.ProductName  = product.ProductName;
                updateitem.ProductPrice = product.ProductPrice;
                updateitem.ProductDetail = product.ProductDetail;
                updateitem.ProductImage = url;
                updateitem.ProductInventory = product.ProductInventory;
                updateitem.ProductExpiryDate    = product.ProductExpiryDate;
                updateitem.ProductPrescription = product.ProductPrescription;
                updateitem.ProductActive = product.ProductActive;
                updateitem.CategoryId = product.CategoryId;
                updateitem.DiscountId   = product.DiscountId;
                updateitem.SupplierId   = product.SupplierId;
                await _context.SaveChangesAsync();
            }
            else
            {
                updateitem.ProductName = product.ProductName;
                updateitem.ProductPrice = product.ProductPrice;
                updateitem.ProductDetail = product.ProductDetail;
                updateitem.ProductInventory = product.ProductInventory;
                updateitem.ProductExpiryDate = product.ProductExpiryDate;
                updateitem.ProductPrescription = product.ProductPrescription;
                updateitem.ProductActive = product.ProductActive;
                updateitem.CategoryId = product.CategoryId;
                updateitem.DiscountId = product.DiscountId;
                updateitem.SupplierId = product.SupplierId;
                await _context.SaveChangesAsync();
            }
            
        }

        
    }
}

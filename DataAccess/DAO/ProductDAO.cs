using BusinessObjects;
using BusinessObjects.Models;
using DataAccess.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        public static List<Product>? GetProducts(string? keyword, decimal? unitP) // Thay đổi kiểu của unitP thành decimal
        {
            List<Product>? listProducts = null;
            try
            {
                using (var context = new EStoreContext())
                {
                    var query = context.Products.AsQueryable();
                    if (keyword != null)
                    {
                        query = query.Where(i => !string.IsNullOrEmpty(i.ProductName) && i.ProductName.ToLower().Contains(keyword.ToLower()));
                    }
                    if (unitP > 0) // Kiểm tra xem unitP có lớn hơn 0 không
                    {
                        query = query.Where(i => i.UnitPrice == unitP); // So sánh với unitP
                    }

                    listProducts = query.Include(i => i.Category).AsNoTracking().ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listProducts;
        }

        public static Product FindProductById(int prodId)
        {
            Product p = new Product();
            try
            {
                using (var context = new EStoreContext())
                {
                    p = context.Products.Include(i => i.Category).AsNoTracking().SingleOrDefault(x => x.ProductId == prodId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }

        public static void SaveProduct(ProductRequestDto p)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    var prod = new Product()
                    {
                        CategoryId = p.CategoryId,
                        ProductName = p.ProductName,
                        Weight = p.Weight, // Đảm bảo p.Weight là decimal
                        UnitPrice = p.UnitPrice, // Đảm bảo p.UnitPrice là decimal
                        UnitsInStock = p.UnitsInStock
                    };
                    context.Products.Add(prod);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateProduct(ProductUpdateRequestDto p)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    var prod = new Product()
                    {
                        ProductId = p.ProductId,
                        CategoryId = p.CategoryId,
                        ProductName = p.ProductName,
                        Weight = (decimal)p.Weight, // Đảm bảo p.Weight là decimal
                        UnitPrice = p.UnitPrice, // Đảm bảo p.UnitPrice là decimal
                        UnitsInStock = p.UnitsInStock
                    };
                    context.Entry<Product>(prod).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteProduct(Product p)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    var p1 = context.Products.SingleOrDefault(
                        c => c.ProductId == p.ProductId);
                    if (p1 != null) // Kiểm tra nếu p1 không phải là null
                    {
                        context.Products.Remove(p1);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

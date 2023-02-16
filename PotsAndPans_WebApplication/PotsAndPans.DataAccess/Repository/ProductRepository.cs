using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            //directly use Db Context
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                //objFromDb.ISBN = obj.ISBN;
                objFromDb.ProductNumber = obj.ProductNumber;
                //objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                //objFromDb.Price50 = obj.Price50;
                //objFromDb.Price100 = obj.Price100;

                objFromDb.CategoryId = obj.CategoryId;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}

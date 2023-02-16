using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models;
using PotsAndPans.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    
    }
}

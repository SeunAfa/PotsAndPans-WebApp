using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models.Models;
using PotsAndPans.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.DataAccess.DbInitializer
{
    public class DbInitializer :IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;   
        }

        public void Initialize()
        {
            //Migration if they not applied 
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) 
            { 

            }

            //Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company)).GetAwaiter().GetResult();

                //if roles are not created, then create admin user

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName= "Admin@PotsAndPans.com",
                    Email = "Admin@PotsAndPans.com",
                    Name = "Admin User",
                    PhoneNumber = "500000000000",
                    StreetAddress = "21 Trafalgar Square",
                    City= "London",
                    Borough = "City of Westminster",
                    PostCode = "WC2N 5DN",
                }, "Admin500@*").GetAwaiter().GetResult();


                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@PotsAndPans.com");

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();   
            }
            return;

        }
    }
}

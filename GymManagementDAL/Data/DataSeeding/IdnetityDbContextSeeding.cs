using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.DataSeeding
{
    public static class IdnetityDbContextSeeding
    {
        public static bool SeedData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            try
            {
                var user = userManager.Users.Any();
                var role = roleManager.Roles.Any();
                if(user && role) return false;

                if (!role)
                {
                    var roles = new List<IdentityRole>
                    {
                        new () { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                        new () { Name = "Admin", NormalizedName = "ADMIN" }
                    };
                    foreach (var r in roles)
                    {
                        if (!roleManager.RoleExistsAsync(r.Name!).Result)
                        {
                            roleManager.CreateAsync(r).Wait();
                        }
                       
                    }
                }
                if (!user)
                {
                    var mainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Elsayed",
                        UserName = "Mohamed",
                        Email = "moha@gmail.com",
                        PhoneNumber = "01234567890",
                    };
                    userManager.CreateAsync(mainAdmin, "Moha@1234").Wait();
                    userManager.AddToRoleAsync(mainAdmin, "SuperAdmin").Wait();

                    var mainAdmin2 = new ApplicationUser()
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = "Admin",
                        Email = "moh@gmail.com",
                        PhoneNumber = "01234567891",

                    };
                    userManager.CreateAsync(mainAdmin2, "Admin@1234").Wait();
                    userManager.AddToRoleAsync(mainAdmin2, "Admin").Wait();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

using GymManagementBLL;
using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Classes.AttachmentService;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.Services.InterFaces.AttachmentService;
using GymManagementDAL.Data.Context;
using GymManagementDAL.Data.DataSeeding;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Classes;
using GymManagementDAL.Repositories.Interfaces;
using GymManagmentBLL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GymManagementPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GymDbContext>(option =>
            {   // Several ways to get connection string from appsettings.json -> I can use any of them 
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                // Suppress EF Core 9 pending model changes warning to allow migrations to run
                option.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            }); 
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddScoped<ISessionReposistory, SessionRepository>();
            builder.Services.AddAutoMapper(x=>x.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<IAnalysisService, AnalysisService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IMemberShipService, MemberShipService>();
            builder.Services.AddScoped<IMemberSessionService, MemberSessionService>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config=>
            {
                config.User.RequireUniqueEmail= true;
            }
            ).AddEntityFrameworkStores<GymDbContext>();
            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();
            #region Data-Seeding
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations != null && pendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }
            GymDbContextSeeding.SeedData(dbContext, app.Environment.ContentRootPath);
            IdnetityDbContextSeeding.SeedData(RoleManager, UserManager);
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

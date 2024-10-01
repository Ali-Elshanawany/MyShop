using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MyShop.DataAccess.Repositories;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using Microsoft.AspNetCore.Identity;
using MyShop.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyShop.Web.Helpers;

namespace MyShop.Web;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // HotReload FrontEnd Changes 
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            // Stripe injection  
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            // Register Dependencies
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();


            // Db Connection
            builder.Services.AddDbContext<ApplicationDbContext>
                (option =>
                          option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Set Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();

            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
               name: "Customer",
               pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}

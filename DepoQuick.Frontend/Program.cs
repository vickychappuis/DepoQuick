using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            
            // Database
            builder.Services.AddDbContextFactory<Context>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    providerOptions => providerOptions.EnableRetryOnFailure())
            );
            
            // Repositories
            builder.Services.AddSingleton<IRepo<Promotion, int>, PromotionRepo>();
            builder.Services.AddSingleton<IRepo<Warehouse, int>, WarehouseRepo>();
            builder.Services.AddSingleton<IRepo<Reservation, int>, ReservationRepo>();
            builder.Services.AddSingleton<IRepo<User, int>, UserRepo>();
            builder.Services.AddSingleton<IRepo<User, string>, UserRepo>();

            // Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<PromotionService>();
            builder.Services.AddSingleton<WarehouseService>();
            builder.Services.AddSingleton<PromotionService>();
            builder.Services.AddSingleton<ReservationService>();
            builder.Services.AddSingleton<PriceService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}

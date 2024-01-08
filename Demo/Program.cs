using AutoMapper;
using BUS;
using DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:7117");
                                  });
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DemoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapping());
            }).CreateMapper());
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //Tin tuc
            builder.Services.AddScoped<ITinTucRepository, TinTucRepository>();
            builder.Services.AddScoped<ITinTucService, TinTucService>();
            //Danh muc
            builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
            builder.Services.AddScoped<IDanhMucService, DanhMucService>();
            //danh muc tin tuc
            builder.Services.AddScoped<IDanhMucTinTucRepository, DanhMucTinTucRepository>();
            builder.Services.AddScoped<IDanhMucTinTucService, DanhMucTinTucService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

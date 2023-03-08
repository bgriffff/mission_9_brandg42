using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mission_9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mission_9
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<BookStoreContext>(options =>
            {
                options.UseSqlite(Configuration["ConnectionStrings:BookStoreDBConnection"]);
            });

            services.AddScoped<IBookStoreRepository, EFBookStoreRepository>();

            //Enables us to use razor pages
            services.AddRazorPages();

            //adds sessions for cart
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            //also for session
            app.UseSession();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                //categoryType from Default page
                endpoints.MapControllerRoute("typepage",
                    "{category}/Page{pageNum}",
                    new { Controller = "Home", action = "Index" });

                // creates better slugs (part at the end of a url to show page number)
                // This is an endpoint, they are executed in order. what ever is fist
                // will be run first
                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 }
                    );

                //pages after you choose a categoryType
                endpoints.MapControllerRoute("type",
                    "{category}",
                    new { Controller = "Home", action = "Index", pageNum = 1 }
                    );

                endpoints.MapDefaultControllerRoute();

                // enables razor pages
                endpoints.MapRazorPages();
            });
        }
    }
}

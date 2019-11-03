using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Taste.DataAccess;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.DataAccess.Data.Repository;
using Taste.Utility;
using Stripe;
using Taste.DataAccess.Data.Initializer;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Taste
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            //services.AddMvc(options => options.EnableEndpointRouting = false)
            //    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "2481959558526884";
                facebookOptions.AppSecret = "25afe44c4799c1c91dd02bfd4433cc7d";
            });

            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = "708aabec-01ae-4674-af5b-a2605b0ac2df";
                options.ClientSecret = "_BPXHMsO-A++jOfPks1cgsKjmey65r9H";
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            dbInitializer.Initialize();


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            //app.UseMvc();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
        }
    }
}

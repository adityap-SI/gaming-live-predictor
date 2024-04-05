#region Net2.1 Code

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ICC.Predictor.Contracts.Configuration;
//using ICC.Predictor.Library.Dependency;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Options;

//namespace ICC.Predictor.Admin
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

//            //User-defined extension method
//            services.AddServices(Configuration);
//            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
//            services.AddSingleton<Interfaces.Admin.ISession, App_Code.Authorization>();
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHostApplicationLifetime appLifetime,
//            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseDeveloperExceptionPage();
//                //app.UseExceptionHandler("/Error");
//                //app.UseHsts();
//            }

//            //app.UseHttpsRedirection();
//            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/admin" });
//            appLifetime.RegisterRedis(redis, appSettings);

//            app.UseForwardedHeaders(new ForwardedHeadersOptions
//            {
//                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//            });

//            app.UseEndpoints(routes =>
//            {
//                //routes.MapRoute(
//                //    name: "default",
//                //    template: "{controller=Home}/{action=Login}/{id?}");

//                routes.MapRoute(
//                    name: "admin",
//                    template: "admin/{action}/{id?}",
//                    defaults: new { Controller = "Home" });
//            });
//        }
//    }
//}

#endregion

using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Library.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ICC.Predictor.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServices(Configuration);
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddSingleton<Interfaces.Admin.ISession, App_Code.Authorization>();

            services.AddControllersWithViews(); // Add MVC services
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/admin" });
            appLifetime.RegisterRedis(redis, appSettings);



            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");

                endpoints.MapControllerRoute(
                    name: "admin",
                    pattern: "admin/{action}/{id?}",
                    defaults: new { Controller = "Home" });
            });




        }
    }
}

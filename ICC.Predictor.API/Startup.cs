#region Old Net2.1 Code

//using ICC.Predictor.Contracts.Configuration;
//using ICC.Predictor.Library.Dependency;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;

//namespace ICC.Predictor.API
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
//            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

//            //User-defined extension method
//            services.AddServices(Configuration);
//            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
//            services.AddSwagger();
//            services.AddCors();
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
//            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings)
//        {
//            if (env.IsDevelopment())
//                app.UseDeveloperExceptionPage();
//            //else
//            //    app.UseHsts();

//            //app.UseHttpsRedirection();
//            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/api" });
//            appLifetime.RegisterRedis(redis, appSettings);
//            app.RegisterSwagger(env);
//            //Added CORS to work on localhost
//            app.UseCors(options => options.WithOrigins("https://localhost:3000").AllowAnyMethod().AllowCredentials());

//            app.UseForwardedHeaders(new ForwardedHeadersOptions
//            {
//                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//            });

//            app.UseMvc();

//        }
//    }

//}

#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ICC.Predictor.Library.Dependency;
using ICC.Predictor.Contracts.Configuration;

namespace ICC.Predictor.API
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
            //services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddControllersWithViews();

            services.AddServices(Configuration);
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
            services.AddCors();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime,
            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            //app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/api" });
            appLifetime.RegisterRedis(redis, appSettings);
            app.RegisterSwagger(env);

            app.UseCors(options => options.WithOrigins("https://localhost:3000").AllowAnyMethod().AllowCredentials());

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


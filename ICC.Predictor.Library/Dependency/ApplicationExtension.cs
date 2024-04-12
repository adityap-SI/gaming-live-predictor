using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace ICC.Predictor.Library.Dependency
{
    public static class ApplicationExtension
    {
        
        public static IHostApplicationLifetime RegisterRedis(this IHostApplicationLifetime app, IRedis redis, IOptions<Application> appSettings)
        {
            if (appSettings.Value.Connection.Redis.Apply)
            {
                app.ApplicationStarted.Register(redis.RedisConnectMultiplexer);
                app.ApplicationStopped.Register(redis.RedisConnectDisposer);
            }

            return app;
        }

        public static IApplicationBuilder RegisterSwagger(this IApplicationBuilder app, IWebHostEnvironment env,IOptions<Application> appsettings)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            List<string> BasePathList = appsettings.Value.CustomSwaggerConfig.BasePathList;
            string swaggerConfig = appsettings.Value.CustomSwaggerConfig.SwaggerConfig;
            app.UseSwagger(c => c.PreSerializeFilters.Add((swaggerDoc,httpReq) =>
                    BasePathList.ForEach(BasePath => swaggerDoc.Servers.Add(new OpenApiServer() { Url = BasePath }))
            ));

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            

            if (env.IsDevelopment())
                swaggerConfig = "/swagger/v1/swagger.json";

            app.UseSwaggerUI(c =>
            {
                //#TOREMEMBER
                //reading swagger json from website's directory location.
                c.SwaggerEndpoint(swaggerConfig, "ICC.Predictor.API V1");
                
                c.RoutePrefix = "api";
            });

            return app;
        }
    }
}
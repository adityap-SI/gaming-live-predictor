using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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

        public static IApplicationBuilder RegisterSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            string swaggerConfig = "/api/config/swagger/ICC.Predictor.API.json";

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
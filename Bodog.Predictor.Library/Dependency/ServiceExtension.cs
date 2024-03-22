using Bodog.Predictor.Contracts.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Bodog.Predictor.Library.Dependency
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<Application>(configuration.GetSection("Application"));

            services.AddSingleton<Interfaces.AWS.IAWS, AWS.SES>();
            services.AddSingleton<Interfaces.Connection.IPostgre, Connection.Postgre>();
            services.AddSingleton<Interfaces.Connection.IRedis, Connection.Redis>();
            services.AddSingleton<Interfaces.Session.ICookies, Session.Cookies>();
            services.AddSingleton<Interfaces.Asset.IAsset, Asset.Constants>();

            return services;
        }

        public static ILoggerFactory UseCloudWatch(this ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            //Adding AWS CloudWatch services
            AWSLoggerConfigSection section = configuration.GetAWSLoggingConfigSection();
            //section.Config.Credentials = AWS.Credentials._AWSCredentials;

            loggerFactory.AddAWSProvider(section);

            return loggerFactory;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            //Adding Swagger services
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Bodog.Predictor.API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = "Bodog.Predictor.API.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
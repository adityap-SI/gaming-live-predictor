using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using ICC.Predictor.Library.Dependency;

namespace ICC.Predictor.Daemon
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
               .UseEnvironment(Environments.Production)//Explicitly setting to Production
               .ConfigureAppConfiguration((hostContext, config) =>
               {
                   var env = hostContext.HostingEnvironment;

                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                   config.SetBasePath(Directory.GetCurrentDirectory());
                   config.AddEnvironmentVariables(prefix: "ASPNETCORE_");

                   if (args != null)
                   {
                       config.AddCommandLine(args);
                   }
               })
               .ConfigureServices((hostContext, services) =>
               {
                   if (hostContext.HostingEnvironment.IsDevelopment())
                   {
                       // Development service configuration
                   }
                   else
                   {
                       // Non-development service configuration
                   }

                   services.AddOptions();
                   services.AddServices(hostContext.Configuration);
                   services.Configure<Contracts.Configuration.Daemon>(hostContext.Configuration.GetSection("Daemon"));
                   services.AddDefaultAWSOptions(hostContext.Configuration.GetAWSOptions());
                   //Background Services
                   //services.AddHostedService<BackgroundServices.PeriodicUpdate>();
                   //services.AddHostedService<BackgroundServices.PeriodicQuestionsUpdate>();
                   services.AddHostedService<BackgroundServices.GameLocking>();
                   //services.AddHostedService<BackgroundServices.MatchAnswerCalculation>();
                   //services.AddHostedService<BackgroundServices.PointsCalculation>();
                   //services.AddHostedService<BackgroundServices.Analytics>();

                   //CloudWatch logs
                   ILoggerFactory loggerFactory = new LoggerFactory();
                   loggerFactory.UseCloudWatch(hostContext.Configuration);
                   loggerFactory.CreateLogger<Program>();
                   services.AddSingleton(loggerFactory);
               });


            await builder.RunConsoleAsync();
        }
    }
}

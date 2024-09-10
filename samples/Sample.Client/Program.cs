// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedParameter.Local
using NLog;
using Sample.Client.Samples.Sample1;
using Sample.Client.Samples.Sample2;
using Sample.Client.Samples.RpcCallSample;
using Essentials.Configuration.Helpers;
using Essentials.Configuration.Extensions;
using static Essentials.Configuration.Helpers.LoggingHelpers;

var builder = WebApplication.CreateBuilder(args);
var environmentName = builder.Environment.EnvironmentName;

LogManager.Setup().LoadConfigurationFromFile(GetNLogConfigPath(environmentName));
var logger = LogManager.GetCurrentClassLogger();

try
{
    var applicationName = EnvironmentHelpers.GetApplicationName();

    logger.Info("Сервис {@appName} запускается...", applicationName);

    var application = builder
        .ConfigureDefault(
            configureServicesAction: (context, services) =>
            {
                TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
                {
                    logger.Error(eventArgs.Exception, "Unobserved task exception");
                };
                
                //services.ConfigureSample1Service(context.Configuration);
                //services.ConfigureSample2Service(context.Configuration);
                //services.ConfigureRpcCallSample(context.Configuration);
            })
        .Build();

    logger.Info("Сервис {@appName} собран. Старт сервиса...", applicationName);

    await application.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Ошибка запуска приложения");
    Thread.Sleep(1000);
}
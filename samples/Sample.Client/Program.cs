using NLog;
using Sample.Client;
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
                services.ConfigureServices(context.Configuration))
        .Build();

    logger.Info("Сервис {@appName} собран. Старт сервиса...", applicationName);

    await application.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Ошибка запуска приложения");
    Thread.Sleep(1000);
}
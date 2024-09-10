using Microsoft.Extensions.DependencyInjection;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureRabbitMqModelsBuilders(this IServiceCollection services) =>
        services.AddHostedService<AutoDeclareModelsHostedService>();
}
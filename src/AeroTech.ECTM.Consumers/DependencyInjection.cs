using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeroTech.ECTM.Consumers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            var broker = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

            ArgumentException.ThrowIfNullOrWhiteSpace(broker.Server, "RabbitMq:Server");
            ArgumentException.ThrowIfNullOrWhiteSpace(broker.UserName, "RabbitMq:UserName");
            ArgumentException.ThrowIfNullOrWhiteSpace(broker.Password, "RabbitMq:Password");

            services.AddMassTransit(bus =>
            {
                bus.SetKebabCaseEndpointNameFormatter();

                bus.UsingRabbitMq((context, rabbit) =>
                {
                    rabbit.Host(broker.Server, broker.Port, broker.VirtualHost, configurator =>
                    {
                        configurator.Username(broker.UserName);
                        configurator.Password(broker.Password);
                    });
                });
            });

            return services;
        }
    }
}

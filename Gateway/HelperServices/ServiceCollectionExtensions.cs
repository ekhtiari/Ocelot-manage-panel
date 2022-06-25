using Microsoft.Extensions.DependencyInjection;
using Ocelot.Authorization;
using System.Linq;

namespace Gateway.HelperServices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection DecorateClaimAuthoriser(this IServiceCollection services)
        {
            var serviceDescriptor = services.First(x => x.ServiceType == typeof(IClaimsAuthorizer));
            services.Remove(serviceDescriptor);

            if (serviceDescriptor.ImplementationType != null)
            {
                var newServiceDescriptor = new ServiceDescriptor(serviceDescriptor.ImplementationType, serviceDescriptor.ImplementationType, serviceDescriptor.Lifetime);
                services.Add(newServiceDescriptor);
            }

            services.AddTransient<IClaimsAuthorizer, ClaimAuthorizerDecorator>();

            return services;
        }
    }
}

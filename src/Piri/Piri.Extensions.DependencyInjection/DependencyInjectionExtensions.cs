using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Piri.Core;

namespace Piri.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds an <see cref="IMapper"/> to the <see cref="IServiceCollection"/> with the specified configuration options and service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the mapper to.</param>
        /// <param name="configure">A delegate to configure the <see cref="MapperConfigurationOptions"/>.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the mapper service. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> with the mapper service added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="configure"/> delegate is null.</exception>
        public static IServiceCollection AddPiriMapper(this IServiceCollection services,
            Action<MapperConfigurationOptions> configure,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            ArgumentNullException.ThrowIfNull(configure);
            services.TryAdd(new ServiceDescriptor(typeof(IMapper), provider => PiriMapper.Create(configure), lifetime));
            return services;
        }
    }
}
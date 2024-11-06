namespace Piri.Core
{
    public static class PiriMapper
    {
        /// <summary>
        /// Creates an instance of <see cref="IMapper"/> using the provided configuration options.
        /// </summary>
        /// <param name="configure">A delegate to configure the <see cref="MapperConfigurationOptions"/>.</param>
        /// <returns>An instance of <see cref="IMapper"/> configured with the specified options.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="configure"/> delegate is null.</exception>
        public static IMapper Create(Action<MapperConfigurationOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            var config = new MapperConfigurationOptions();
            configure(config);
            return new Mapper(config);
        }
    }
}
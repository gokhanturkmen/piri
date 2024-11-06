namespace Piri.Core
{
    public class MapperConfigurationOptions
    {
        internal Dictionary<(Type, Type), Func<object, object>> Maps = [];
        internal Dictionary<Type, Func<object, object>> DefaultMaps = [];
        internal bool DefaultMappingEnabled { get; private set; } = false;

        /// <summary>
        /// Adds a mapping function between the source type and the destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="mapFunction">The function that maps a source object to a destination object.</param>
        /// <returns>The current instance of <see cref="MapperConfigurationOptions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="mapFunction"/> is null.</exception>
        public MapperConfigurationOptions AddMap<TSource, TDestination>(Func<TSource, TDestination> mapFunction)
        {
            ArgumentNullException.ThrowIfNull(mapFunction);
            Maps.Add((typeof(TSource), typeof(TDestination)), source => mapFunction((TSource)source)!);
            return this;
        }

        /// <summary>
        /// Enables the default mapping functionality.
        /// </summary>
        /// <returns>The current instance of <see cref="MapperConfigurationOptions"/>.</returns>
        public MapperConfigurationOptions EnableDefaultMapping()
        {
            DefaultMappingEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the default mapping for a specific type.
        /// </summary>
        /// <typeparam name="T">The type for which the default mapping is enabled.</typeparam>
        /// <param name="defaultMapFunction">The function that maps an object to the specified type.</param>
        /// <returns>The current instance of <see cref="MapperConfigurationOptions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="defaultMapFunction"/> is null.</exception>
        public MapperConfigurationOptions EnableDefaultMappingFor<TDestination>(Func<object, TDestination> defaultMapFunction)
        {
            ArgumentNullException.ThrowIfNull(defaultMapFunction);
            DefaultMaps.Add(typeof(TDestination), source => defaultMapFunction(source)!);
            return this;
        }
    }
}
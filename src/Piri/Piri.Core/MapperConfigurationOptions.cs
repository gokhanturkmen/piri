using System.Reflection;

namespace Piri.Core
{
    public class MapperConfigurationOptions
    {
        internal readonly Dictionary<(Type, Type), Func<object, object>> Maps = [];
        internal readonly Dictionary<Type, Func<object, object>> DefaultMaps = [];
        internal bool DefaultMappingEnabled { get; private set; } = false;


        /// <summary>
        /// Adds the default mapping for a specific destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type for which the default mapping is added.</typeparam>
        /// <param name="defaultMapFunction">The function that maps an object to the specified destination type.</param>
        /// <returns>The current instance of <see cref="MapperConfigurationOptions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="defaultMapFunction"/> is null.</exception>
        public MapperConfigurationOptions AddDefaultMappingFor<TDestination>(Func<object, TDestination> defaultMapFunction)
        {
            ArgumentNullException.ThrowIfNull(defaultMapFunction);
            DefaultMaps.Add(typeof(TDestination), source => defaultMapFunction(source)!);
            return this;
        }

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

        public MapperConfigurationOptions AddMap<TSource, TDestination>()
        {
            AddDefaultMap<TSource, TDestination>(this);
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

        internal static void AddDefaultMap<TSource, TDestination>(MapperConfigurationOptions options)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            if (options.Maps.ContainsKey((sourceType, destinationType)))
            {
                return;
            }

            List<Func<object, TDestination, TDestination>> mapFunctions = [];

            foreach (var sourceProperty in sourceType.GetProperties().Where(p => p.CanRead))
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name, BindingFlags.SetProperty);
                if (destinationProperty == null
                    || sourceProperty.PropertyType != destinationProperty.PropertyType)
                {
                    continue;
                }
                TDestination mapFunction(object src, TDestination destination)
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(src));
                    return destination;
                }

                mapFunctions.Add(mapFunction);
            }

            foreach (var sourceField in sourceType.GetFields().Where(f => f.IsPublic))
            {
                var destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public | BindingFlags.SetField);
                if (destinationField == null
                    || sourceField.FieldType != destinationField.FieldType)
                {
                    continue;
                }
                TDestination mapFunction(object src, TDestination destination)
                {
                    destinationField.SetValue(destination, sourceField.GetValue(src));
                    return destination;
                }

                mapFunctions.Add(mapFunction);
            }

            object mainMapFunctionToBeCached(object src)
            {
                var destination = Activator.CreateInstance<TDestination>()
                    ?? throw new InvalidOperationException("Failed to create an instance of the destination type.");
                for (int i = 0; i < mapFunctions.Count; i++)
                {
                    destination = mapFunctions[i](src, destination);
                }
                return destination!;
            }

            options.Maps.TryAdd((sourceType, destinationType), mainMapFunctionToBeCached);
        }
    }
}
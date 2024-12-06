using System.Reflection;

namespace Piri.Core
{
    internal class Mapper(MapperConfigurationOptions config) : IMapper
    {
        /// <summary>
        /// Maps the source object to an instance of the specified destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>An instance of <typeparamref name="TDestination"/> with values mapped from the source object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source object is null.</exception>
        /// <exception cref="NotImplementedException">Thrown when no mapping function is found for the destination type.</exception>
        public TDestination Map<TDestination>(object source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);
            if (config.Maps.TryGetValue((sourceType, destinationType), out var mapFunction))
            {
                return (TDestination)mapFunction(source);
            }
            if (config.DefaultMaps.TryGetValue(destinationType, out var defaultMapFunction))
            {
                return (TDestination)defaultMapFunction(source);
            }
            if (config.DefaultMappingEnabled)
            {
                return DefaultMapNew<TDestination>(source);
            }

            throw new NotImplementedException($"Mapping function not found for destination type {destinationType.Name}");
        }



        /// <summary>
        /// Provides a default mapping implementation by copying properties and fields from the source object to a new instance of the destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>A new instance of <typeparamref name="TDestination"/> with values copied from the source object.</returns>
        private TDestination DefaultMapOld<TDestination>(object source)
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            var newObject = Activator.CreateInstance<TDestination>();
            foreach (var sourceProperty in sourceType.GetProperties().Where(p => p.CanRead))
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name, BindingFlags.SetProperty);
                if (destinationProperty == null
                    || sourceProperty.PropertyType != destinationProperty.PropertyType)
                {
                    continue;
                }
                destinationProperty.SetValue(newObject, sourceProperty.GetValue(source));
            }

            foreach (var sourceField in sourceType.GetFields().Where(f => f.IsPublic))
            {
                var destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public | BindingFlags.SetField);
                if (destinationField == null
                    || sourceField.FieldType != destinationField.FieldType)
                {
                    continue;
                }

                destinationField.SetValue(newObject, sourceField.GetValue(source));
            }

            return newObject;
        }

        /// <summary>
        /// Provides a default mapping implementation by copying properties and fields from the source object to a new instance of the destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>A new instance of <typeparamref name="TDestination"/> with values copied from the source object.</returns>
        internal TDestination DefaultMapNew<TDestination>(object source)
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            if (config.Maps.TryGetValue((sourceType, destinationType), out var mapFn))
            {
                return (TDestination)mapFn(source);
            }

            List<Func<object, TDestination, TDestination>> mapFunctions = [];
            var newObject = Activator.CreateInstance<TDestination>();

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
                newObject = mapFunction(source, newObject);
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
                newObject = mapFunction(source, newObject);
            }

            object mainMapFunctionToBeCached(object src)
            {
                var destination = Activator.CreateInstance<TDestination>();
                for (int i = 0; i < mapFunctions.Count; i++)
                {
                    destination = mapFunctions[i](src, destination);
                }
                return destination;
            }

            config.Maps.TryAdd((sourceType, destinationType), mainMapFunctionToBeCached);

            return newObject;
        }
    }
}
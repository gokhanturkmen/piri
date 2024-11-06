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
                return DefaultMap<TDestination>(source);
            }

            throw new NotImplementedException($"Mapping function not found for destination type {destinationType.Name}");
        }

        /// <summary>
        /// Provides a default mapping implementation by copying properties and fields from the source object to a new instance of the destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>A new instance of <typeparamref name="TDestination"/> with values copied from the source object.</returns>
        private static TDestination DefaultMap<TDestination>(object source)
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            var newObject = Activator.CreateInstance<TDestination>();
            foreach (var sourceProperty in sourceType.GetProperties().Where(p => p.CanRead))
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name);
                if (destinationProperty == null
                    || !destinationProperty.CanWrite
                    || sourceProperty.PropertyType != destinationProperty.PropertyType)
                {
                    continue;
                }
                destinationProperty.SetValue(newObject, sourceProperty.GetValue(source));
            }

            foreach (var sourceField in sourceType.GetFields().Where(f => f.IsPublic))
            {
                var destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public);
                if (destinationField == null
                    || destinationField.IsInitOnly
                    || destinationField.IsLiteral
                    || sourceField.FieldType != destinationField.FieldType)
                {
                    continue;
                }
                destinationField.SetValue(newObject, sourceField.GetValue(source));
            }

            return newObject;
        }
    }
}
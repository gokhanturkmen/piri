using System.Reflection;

namespace Piri.Core
{
    internal sealed class PredefinedMapping<TSource, TDestination> : IMappingRule<object, object>
    {
        private readonly List<Func<object, TDestination, TDestination>> mapFunctions = [];

        public PredefinedMapping(MemberTypes memberTypesToMap)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            if (memberTypesToMap.HasFlag(MemberTypes.Property))
            {
                AddPropertyMappings(sourceType, destinationType);
            }
            if (memberTypesToMap.HasFlag(MemberTypes.Field))
            {
                AddFieldMappings(sourceType, destinationType);
            }
        }

        private void AddPropertyMappings(Type sourceType, Type destinationType)
        {
            var sourceProperties = sourceType.GetProperties().Where(p => p.CanRead).ToDictionary(p => (p.Name, p.PropertyType), p => p);
            var destinationProperties = destinationType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => (p.Name, p.PropertyType), p => p);
            var functions = new List<Func<object, TDestination, TDestination>>();

            foreach (var sourceProperty in sourceProperties)
            {
                if (!destinationProperties.TryGetValue(sourceProperty.Key, out var destinationProperty))
                {
                    continue;
                }
                TDestination mapProperty(object src, TDestination destination)
                {
                    destinationProperty.SetValue(destination, sourceProperty.Value.GetValue(src));
                    return destination;
                }

                functions.Add(mapProperty);
            }

            TDestination mapFunction(object src, TDestination destination)
            {
                for (int i = 0; i < functions.Count; i++)
                {
                    destination = functions[i](src, destination);
                }
                return destination;
            }

            mapFunctions.Add(mapFunction);

        }

        private void AddFieldMappings(Type sourceType, Type destinationType)
        {
            var sourceFields = sourceType.GetFields().Where(f => f.IsPublic).ToDictionary(f => (f.Name, f.FieldType), f => f);
            var destinationFields = destinationType.GetFields().Where(f => f.IsPublic).ToDictionary(f => (f.Name, f.FieldType), f => f);
            var functions = new List<Func<object, TDestination, TDestination>>();

            foreach (var sourceField in sourceFields)
            {
                if (!destinationFields.TryGetValue(sourceField.Key, out var destinationField))
                {
                    continue;
                }
                TDestination mapField(object src, TDestination destination)
                {
                    destinationField.SetValue(destination, sourceField.Value.GetValue(src));
                    return destination;
                }
                functions.Add(mapField);
            }

            TDestination mapFunction(object src, TDestination destination)
            {
                for (int i = 0; i < functions.Count; i++)
                {
                    destination = functions[i](src, destination);
                }
                return destination;
            }

            mapFunctions.Add(mapFunction);
        }

        public object Map(object source)
        {
            var destination = Activator.CreateInstance<TDestination>()!;

            for (int i = 0; i < mapFunctions.Count; i++)
            {
                destination = mapFunctions[i](source, destination);
            }
            return destination!;
        }
    }
}
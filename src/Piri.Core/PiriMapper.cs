using System.Reflection;

namespace Piri.Core
{
    public class PiriMapper : IMapper
    {
        internal const MemberTypes PROPERTY_MAPPINGS_ONLY = MemberTypes.Property;
        private readonly Dictionary<(Type, Type), Func<object, object>> Maps = [];
        private readonly Dictionary<Type, Func<object, object>> DefaultMaps = [];

        private static PiriMapper? instance;
        private static readonly object syncRoot = new();

        private static PiriMapper PiriInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new PiriMapper();
                    }
                }
                return instance;
            }
        }

        public static IMapper Instance => PiriInstance;

        private PiriMapper()
        {
        }

        public TDestination Map<TDestination>(object source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var destinationType = typeof(TDestination);
            var sourceType = source.GetType();
            if (Maps.TryGetValue((sourceType, destinationType), out var mapFunction))
            {
                return (TDestination)mapFunction(source)!;
            }
            if (DefaultMaps.TryGetValue(destinationType, out var defaultMapFunction))
            {
                return (TDestination)defaultMapFunction(source)!;
            }

            var predefinedMappingType = typeof(PredefinedMapping<,>).MakeGenericType(sourceType, destinationType);

            if (Activator.CreateInstance(predefinedMappingType, PROPERTY_MAPPINGS_ONLY) is IMappingRule<object, object> mappingRule)
            {
                Maps.TryAdd((sourceType, destinationType), mappingRule.Map);
                return (TDestination)mappingRule.Map(source);
            }

            throw new InvalidOperationException($"Mapping function not found for destination type {destinationType.Name}");
        }

        public static PiriMapper AddMap<TSource, TDestination>(Func<TSource, TDestination> mapFunction)
        {
            ArgumentNullException.ThrowIfNull(mapFunction);
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            if (!PiriInstance.Maps.TryAdd((sourceType, destinationType), (source) => mapFunction((TSource)source)!))
            {
                PiriInstance.Maps[(sourceType, destinationType)] = (source) => mapFunction((TSource)source)!;
            }
            return PiriInstance;
        }

        public static PiriMapper AddMap<TDestination>(Func<object, TDestination> defaultMapFunction)
        {
            ArgumentNullException.ThrowIfNull(defaultMapFunction);
            var destinationType = typeof(TDestination);
            if (!PiriInstance.DefaultMaps.TryAdd(destinationType, source => defaultMapFunction(source)!))
            {
                PiriInstance.DefaultMaps[destinationType] = source => defaultMapFunction(source)!;
            }
            return PiriInstance;
        }

        public static PiriMapper AddMap<TSource, TDestination>(IMappingRule<TSource, TDestination> mappingRule)
        {
            ArgumentNullException.ThrowIfNull(mappingRule);

            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            if (!PiriInstance.Maps.TryAdd((sourceType, destinationType), source => mappingRule.Map((TSource)source)!))
            {
                PiriInstance.Maps[(sourceType, destinationType)] = source => mappingRule.Map((TSource)source)!;
            }

            return PiriInstance;
        }

        internal static PiriMapper AddMap<TSource, TDestination>(MemberTypes? memberTypesToMap = null)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var predefinedMapping = new PredefinedMapping<TSource, TDestination>(memberTypesToMap ?? PROPERTY_MAPPINGS_ONLY);

            if (!PiriInstance.Maps.TryAdd((sourceType, destinationType), predefinedMapping.Map))
            {
                PiriInstance.Maps[(sourceType, destinationType)] = predefinedMapping.Map;
            }
            return PiriInstance;
        }
    }
}
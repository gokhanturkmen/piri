namespace Piri.Core
{
    public interface IMappingRule<in TSource, out TDestination> : IMappingRule
    {
        TDestination Map(TSource source);
    }

    public interface IMappingRule
    {
    }
}

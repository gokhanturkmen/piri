namespace Piri.Core
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the source object to an instance of the specified destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>An instance of <typeparamref name="TDestination"/> with values mapped from the source object.</returns>
        TDestination Map<TDestination>(object source);
    }
}
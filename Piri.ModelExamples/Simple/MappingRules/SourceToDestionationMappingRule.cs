using Piri.Core;

namespace Piri.ModelExamples.Simple.MappingRules
{
    public class SourceToDestionationMappingRule : IMappingRule<Source, Destination>
    {
        public Destination Map(Source source)
        {
            return new Destination
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description
            };
        }
    }
}

namespace Piri.ModelExamples.Simple
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is Source source)
            {
                return Id == source.Id && Name == source.Name && Description == source.Description;
            }
            return obj == this;
        }
    }
}

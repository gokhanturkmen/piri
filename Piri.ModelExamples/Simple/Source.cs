namespace Piri.ModelExamples.Simple
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is Destination destination)
            {
                return Id == destination.Id && Name == destination.Name && Description == destination.Description;
            }
            return obj == this;
        }
    }
}

using Piri.Core;
using Piri.ModelExamples.Simple;
using Piri.ModelExamples.Simple.MappingRules;

namespace Piri.Tests.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void SimpleObjectMappingTests()
        {
            var source1 = new Source
            {
                Id = 1,
                Name = "Source",
                Description = "This is a source object."
            };
            var destination1 = PiriMapper.Instance.Map<Destination>(source1);

            Assert.Equal(source1.Id, destination1.Id);
            Assert.Equal(source1.Name, destination1.Name);
            Assert.Equal(source1.Description, destination1.Description);

            var source2 = new Source
            {
                Id = 2,
                Name = "Source 2",
                Description = "This is another source object."
            };

            var destination2 = PiriMapper.Instance.Map<Destination>(source2);

            Assert.Equal(source2.Id, destination2.Id);
            Assert.Equal(source2.Name, destination2.Name);
            Assert.Equal(source2.Description, destination2.Description);

            Assert.NotEqual(destination1, destination2);
        }

        [Fact]
        public void SimpleObjectMappingWithRulesTests()
        {
            PiriMapper.AddMap(new SourceToDestionationMappingRule());
            var source1 = new Source
            {
                Id = 1,
                Name = "Source",
                Description = "This is a source object."
            };
            var destination1 = PiriMapper.Instance.Map<Destination>(source1);

            Assert.Equal(source1.Id, destination1.Id);
            Assert.Equal(source1.Name, destination1.Name);
            Assert.Equal(source1.Description, destination1.Description);

            var source2 = new Source
            {
                Id = 2,
                Name = "Source 2",
                Description = "This is another source object."
            };

            var destination2 = PiriMapper.Instance.Map<Destination>(source2);

            Assert.Equal(source2.Id, destination2.Id);
            Assert.Equal(source2.Name, destination2.Name);
            Assert.Equal(source2.Description, destination2.Description);

            Assert.NotEqual(destination1, destination2);
        }

        //[Fact]
        //public void SimpleObjectMappingWithJsonTests()
        //{
        //    PiriMapper.AddMap(obj =>
        //    {
        //        return JsonSerializer.Deserialize<Destination>(JsonSerializer.Serialize(obj))!;
        //    });
        //    var source1 = new Source
        //    {
        //        Id = 1,
        //        Name = "Source",
        //        Description = "This is a source object."
        //    };
        //    var destination1 = PiriMapper.Instance.Map<Destination>(source1);

        //    Assert.Equal(source1.Id, destination1.Id);
        //    Assert.Equal(source1.Name, destination1.Name);
        //    Assert.Equal(source1.Description, destination1.Description);

        //    var source2 = new Source
        //    {
        //        Id = 2,
        //        Name = "Source 2",
        //        Description = "This is another source object."
        //    };

        //    var destination2 = PiriMapper.Instance.Map<Destination>(source2);

        //    Assert.Equal(source2.Id, destination2.Id);
        //    Assert.Equal(source2.Name, destination2.Name);
        //    Assert.Equal(source2.Description, destination2.Description);

        //    Assert.NotEqual(destination1, destination2);
        //}
    }
}
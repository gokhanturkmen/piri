using AutoMapper;
using BenchmarkDotNet.Attributes;
using Piri.Core;
using Piri.ModelExamples.Simple;
using IAutoMapper = AutoMapper.IMapper;

namespace Piri.Benchmarks.Benchmarks
{
    [MemoryDiagnoser(true)]
    [Orderer(methodOrderPolicy: BenchmarkDotNet.Order.MethodOrderPolicy.Declared)]
    public class SimpleObjectImplicitBenchmarks
    {
        private IAutoMapper _autoMapper;
        private Source _source;

        [GlobalSetup]
        public void Setup()
        {
            _source = new Source
            {
                Id = 1,
                Name = "Source",
                Description = "This is a source object."
            };

            _autoMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>();
            }).CreateMapper();

        }

        [Benchmark]
        public Destination ManualMappingSimpleObject()
        {
            return new Destination
            {
                Id = _source.Id,
                Name = _source.Name,
                Description = _source.Description
            };
        }

        [Benchmark]
        public Destination PiriMapperSimpleObject()
        {
            return PiriMapper.Instance.Map<Destination>(_source);
        }

        [Benchmark]
        public Destination AutoMapperMapSimpleObject()
        {
            return _autoMapper.Map<Destination>(_source);
        }
    }
}

using BenchmarkDotNet.Running;
using Piri.Benchmarks.Benchmarks;

//BenchmarkRunner.Run<SimpleObjectImplicitBenchmarks>();
BenchmarkRunner.Run<SimpleObjectExplicitBenchmarks>();
//BenchmarkRunner.Run(typeof(SimpleObjectImplicitBenchmarks).Assembly);
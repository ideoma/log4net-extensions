using BenchmarkDotNet.Running;

namespace Petabytle.log4net.Extensions.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MemoryMappedAppenderBenchmark>();
        }
    }
}
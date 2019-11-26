# Petabyte Log4net Extensions

Extends log4net with MemoryMappedAppender for fast and reliable logging.

## Getting Started

Step 1. Install nuget package ```log4net.Petabyte.Extensions```

Step 2. Configure MemoryMappedAppender the same way as RollingFileAppender configured

```xml
<appender name="MemoryMappedAppender" type="log4net.Petabyte.MemoryMappedAppender,log4net.Petabyte.Extensions">
    <mappingBufferSize value="2MB"/>
    <file value="logfile" />
    <appendToFile value="true" />
    <rollingStyle value="Composite" />
    <datePattern value="yyyyMMdd" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="10MB" />
    <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
    </layout>
    <root>
        <level value="INFO" />
        <appender-ref ref="MemoryMappedAppender" />
    </root>
</appender>

```

Note additional parameter mappingBufferSize, see more about it in Configuration section

Step 3. Use the logging in the usual way
```C#
var log = LogManager.GetLogger(GetType());
log.Info("message");
```

### Why Memory Mapped
In short - it is faster. It is the same speed as ImmediateFlush set to false while having same reliability. If the process crashes, OS will flush memory mapped block to the disk so you will not loose the last fatal message of the crashed process.

When every line of logging hits disk eventually logging becomes a bottleneck and you CPU cores more idle competing for a shared slow resource. Other attempts to speed up logging like making it batched or flushed on a separate thread have the common problem of loosing the last message before abnormal exit.

### Benchmarks

[Benchmark results](./Petabyte.log4net.Extensions.Benchmarks/BenchmarkDotNet.Artifacts/results/Petabytle.log4net.Extensions.Benchmarks.MemoryMappedAppenderBenchmark-report-github.md) ran on Windows, MacOS and Linux all show that when log4net RollingFileAppender has ```ImmediateFlush=false``` the speed is same with MemoryMappedAppender. When ```ImmediateFlush=true``` it becomes 3x times slower while MemoryMappedAppender ignores this settings and relies on OS to flush the data to the disk.

|                               Method | Runtime |     Toolchain | MaxMappingSize | MaxFileSize | ImmediateFlush | MaxRollingFiles | RowCount |       Mean |      Error |      StdDev |     Median | Ratio | RatioSD |
|------------------------------------- |-------- |-------------- |--------------- |------------ |--------------- |---------------- |--------- |-----------:|-----------:|------------:|-----------:|------:|--------:|
|         RollingFileAppenderBenchmark |     Clr |       Default |            2MB |        10MB |          False |               5 |   300000 |   595.9 ms |   8.841 ms |   8.2698 ms |   592.1 ms |  1.00 |    0.00 |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |          False |               5 |   300000 |   604.3 ms |  13.831 ms |  17.9838 ms |   601.7 ms |  1.02 |    0.03 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   300000 |   548.5 ms |   8.757 ms |   8.1912 ms |   546.2 ms |  0.92 |    0.02 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   300000 |   542.9 ms |   6.047 ms |   5.6561 ms |   542.3 ms |  0.91 |    0.01 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         RollingFileAppenderBenchmark |     Clr |       Default |            2MB |        10MB |           True |               5 |   300000 | 3,396.5 ms |  11.617 ms |  10.2982 ms | 3,395.6 ms |  1.00 |    0.00 |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |           True |               5 |   300000 |   586.3 ms |   5.335 ms |   4.9902 ms |   585.3 ms |  0.17 |    0.00 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   300000 | 3,356.9 ms |  12.359 ms |  10.9563 ms | 3,357.7 ms |  0.99 |    0.01 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   300000 |   543.1 ms |   5.913 ms |   5.2420 ms |   542.9 ms |  0.16 |    0.00 |

As it shows RollingFileAppender and RollingMemoryMappedAppender are neck to neck when flushing is switched OFF, and RollingFileAppender 6x times slower when flushing is ON. This is on desktop grade SSD disk, the slower the disk is the bigger is the difference.
    

### Prerequisites

.NET Standard 2.0 is required. Even though log4net targets .NET Standard 1.6 since, it is only in 2.0 the support of Memory Mapping exists.

## Contributing

Feel free to create a pull request, read the building and testing section

## Building

Simple. Build petabyte-log4net-extensions.sln

## Running the tests

Run tests in Petabyte.log4net.Extensions.Test. Some tests will start / stop a separate process of Petabyte.log4net.Extensions.DummyLogger

Run benchmarks with [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) from assembly Petabyte.log4net.Extensions.Benchmarks. Install the command line runner of version used in the benchmark project 

```
dotnet tool install --global BenchmarkDotNet.Tool --version 0.11.5
dotnet benchmark petabyte-log4net-extensions/Petabyte.log4net.Extensions.Benchmarks/bin/Release/netstandard2.0/Petabyte.log4net.Extensions.Benchmarks.dll -f * --runtimes clr core
```

See more about the ways to run benchmarks at https://benchmarkdotnet.org/

## Authors

* **Alex Pelagenko**

## License

This project is licensed under the Apache 2.0 license

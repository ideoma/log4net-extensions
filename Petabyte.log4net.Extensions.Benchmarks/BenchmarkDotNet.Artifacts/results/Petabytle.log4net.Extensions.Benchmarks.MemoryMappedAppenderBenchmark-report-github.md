``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.615 (1809/October2018Update/Redstone5)
Intel Core i5-4570 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.2.300
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  Job-PIQDXV : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3416.0
  Job-RINTFY : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|                               Method | Runtime |     Toolchain | MaxMappingSize | MaxFileSize | ImmediateFlush | MaxRollingFiles | RowCount |       Mean |      Error |      StdDev |     Median | Ratio | RatioSD |
|------------------------------------- |-------- |-------------- |--------------- |------------ |--------------- |---------------- |--------- |-----------:|-----------:|------------:|-----------:|------:|--------:|
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |        **10MB** |          **False** |               **5** |   **100000** |   **207.1 ms** |   **3.522 ms** |   **3.1221 ms** |   **206.9 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |          False |               5 |   100000 |   204.0 ms |   4.014 ms |   4.9295 ms |   202.9 ms |  0.99 |    0.04 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   100000 |   182.8 ms |   1.886 ms |   1.7643 ms |   182.8 ms |  0.88 |    0.01 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   100000 |   181.3 ms |   1.466 ms |   1.1443 ms |   181.4 ms |  0.88 |    0.01 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |        **10MB** |          **False** |               **5** |   **300000** |   **595.9 ms** |   **8.841 ms** |   **8.2698 ms** |   **592.1 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |          False |               5 |   300000 |   604.3 ms |  13.831 ms |  17.9838 ms |   601.7 ms |  1.02 |    0.03 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   300000 |   548.5 ms |   8.757 ms |   8.1912 ms |   546.2 ms |  0.92 |    0.02 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |          False |               5 |   300000 |   542.9 ms |   6.047 ms |   5.6561 ms |   542.3 ms |  0.91 |    0.01 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |        **10MB** |           **True** |               **5** |   **100000** | **1,144.8 ms** |   **6.132 ms** |   **5.7361 ms** | **1,144.1 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |           True |               5 |   100000 |   202.3 ms |   3.091 ms |   2.8913 ms |   201.0 ms |  0.18 |    0.00 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   100000 | 1,114.3 ms |   5.723 ms |   5.3536 ms | 1,112.2 ms |  0.97 |    0.01 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   100000 |   182.1 ms |   3.354 ms |   3.1373 ms |   181.5 ms |  0.16 |    0.00 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |        **10MB** |           **True** |               **5** |   **300000** | **3,396.5 ms** |  **11.617 ms** |  **10.2982 ms** | **3,395.6 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |        10MB |           True |               5 |   300000 |   586.3 ms |   5.335 ms |   4.9902 ms |   585.3 ms |  0.17 |    0.00 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   300000 | 3,356.9 ms |  12.359 ms |  10.9563 ms | 3,357.7 ms |  0.99 |    0.01 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |        10MB |           True |               5 |   300000 |   543.1 ms |   5.913 ms |   5.2420 ms |   542.9 ms |  0.16 |    0.00 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |         **2MB** |          **False** |               **5** |   **100000** |   **213.6 ms** |   **3.108 ms** |   **2.9068 ms** |   **213.3 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |         2MB |          False |               5 |   100000 |   211.7 ms |   7.432 ms |   6.9523 ms |   209.8 ms |  0.99 |    0.04 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |          False |               5 |   100000 |   187.1 ms |   2.164 ms |   1.9183 ms |   187.5 ms |  0.88 |    0.01 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |          False |               5 |   100000 |   188.6 ms |   3.702 ms |   5.4266 ms |   186.1 ms |  0.89 |    0.03 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |         **2MB** |          **False** |               **5** |   **300000** |   **650.6 ms** |  **13.318 ms** |  **24.3524 ms** |   **645.0 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |         2MB |          False |               5 |   300000 |   618.9 ms |   7.027 ms |   6.2290 ms |   618.0 ms |  0.94 |    0.05 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |          False |               5 |   300000 |   569.4 ms |   3.585 ms |   3.1782 ms |   569.1 ms |  0.86 |    0.04 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |          False |               5 |   300000 |   554.7 ms |   4.105 ms |   3.8398 ms |   555.4 ms |  0.84 |    0.04 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |         **2MB** |           **True** |               **5** |   **100000** | **1,163.1 ms** |  **14.620 ms** |  **12.9598 ms** | **1,161.8 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |         2MB |           True |               5 |   100000 |   201.0 ms |   1.145 ms |   0.9561 ms |   201.4 ms |  0.17 |    0.00 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |           True |               5 |   100000 | 1,141.1 ms |  15.867 ms |  12.3876 ms | 1,141.1 ms |  0.98 |    0.02 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |           True |               5 |   100000 |   186.1 ms |   1.579 ms |   1.4768 ms |   186.1 ms |  0.16 |    0.00 |
|                                      |         |               |                |             |                |                 |          |            |            |             |            |       |         |
|         **RollingFileAppenderBenchmark** |     **Clr** |       **Default** |            **2MB** |         **2MB** |           **True** |               **5** |   **300000** | **3,911.1 ms** | **147.110 ms** | **426.7942 ms** | **3,742.4 ms** |  **1.00** |    **0.00** |
| RollingMemoryMappedAppenderBenchmark |     Clr |       Default |            2MB |         2MB |           True |               5 |   300000 |   618.5 ms |   7.972 ms |   7.4572 ms |   619.1 ms |  0.15 |    0.02 |
|         RollingFileAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |           True |               5 |   300000 | 3,390.5 ms |  17.604 ms |  15.6058 ms | 3,391.0 ms |  0.83 |    0.08 |
| RollingMemoryMappedAppenderBenchmark |    Core | .NET Core 2.1 |            2MB |         2MB |           True |               5 |   300000 |   551.3 ms |   3.937 ms |   3.6824 ms |   551.9 ms |  0.14 |    0.01 |

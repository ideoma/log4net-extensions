using System.Text;
using BenchmarkDotNet.Attributes;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Petabyte;
using log4net.Repository.Hierarchy;

namespace Petabytle.log4net.Extensions.Benchmarks
{
    public class MemoryMappedAppenderBenchmark
    {
        private const string BenchmarkLogFileName = "benchmark.log";

        [Params("2MB")] public string MaxMappingSize;

        [Params("10MB")] public string MaxFileSize;
        
        [Params(true, false)] public bool ImmediateFlush;

        [Params( 5)] public int MaxRollingFiles;
        
        [Params( 100000, 300000)] public int RowCount;

        [GlobalSetup]
        public void GlobalSetup()
        {
            TestUtils.DeleteTestFiles(BenchmarkLogFileName);
        }

        [Benchmark(Baseline = true)]
        public void RollingFileAppenderBenchmark()
        {
            LogMessages(CreateRollingFIleAppender(), RowCount);
            ShutdownLogging();
        }

        [Benchmark]
        public void RollingMemoryMappedAppenderBenchmark()
        {
            LogMessages(CreateMemoryMappedAppender(), RowCount);
            ShutdownLogging();
        }

        private void LogMessages(ILog logger, int count)
        {
            for (int i = 0; i < count; i++)
            {
                logger.Info("How to Do Logging in C# With Log4net?");
            }
        }

        private void ShutdownLogging()
        {
            var h = LogManager.GetRepository("TestRepository");
            h.ResetConfiguration();
            h.Shutdown();
            ((Hierarchy) h).Clear();
            TestUtils.DeleteTestFiles(BenchmarkLogFileName);
        }

        private ILog CreateRollingFIleAppender()
        {
            Hierarchy h;
            try
            {
                h = (Hierarchy) LogManager.GetRepository("TestRepository");
            }
            catch (LogException)
            {
                h = (Hierarchy) LogManager.CreateRepository("TestRepository");
            }

            var appender = new RollingFileAppender
            {
                File = BenchmarkLogFileName,
                AppendToFile = true,
                CountDirection = 0,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaximumFileSize = MaxFileSize,
                Encoding = Encoding.ASCII,
                MaxSizeRollBackups = MaxRollingFiles,
                ImmediateFlush = ImmediateFlush
            };

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            h.Root.AddAppender(appender);
            h.Configured = true;

            var log = h.GetLogger("Logger");
            return new LogImpl(log);
        }

        private ILog CreateMemoryMappedAppender()
        {
            Hierarchy h;
            try
            {
                h = (Hierarchy) LogManager.GetRepository("TestRepository");
            }
            catch (LogException)
            {
                h = (Hierarchy) LogManager.CreateRepository("TestRepository");
            }

            var appender = new MemoryMappedAppender
            {
                File = BenchmarkLogFileName,
                AppendToFile = true,
                CountDirection = 0,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaximumFileSize = MaxFileSize,
                Encoding = Encoding.ASCII,
                MaxSizeRollBackups = MaxRollingFiles,
                MappingBufferSize = MaxMappingSize
            };

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            h.Root.AddAppender(appender);
            h.Configured = true;

            var log = h.GetLogger("Logger");
            return new LogImpl(log);
        }
    }
}
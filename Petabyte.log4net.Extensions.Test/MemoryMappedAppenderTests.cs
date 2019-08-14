using System;
using System.IO;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Petabyte;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Petabyte.log4net.Extensions.Test
{
    [TestFixture]
    public class MemoryMappedAppenderTests
    {
        private const string TestLogFileName = "MemoryMappedAppenderTests.log";
        
        [SetUp]
        public void Setup()
        {
            TestUtils.SetUp();
            TestUtils.DeleteTestFiles(TestLogFileName);
        }

        [Test]
        public void ShouldAppendToAbortedFile()
        {
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message",
                Abort = true,
                MaxSizeRollBackups = 10
            });
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message 2",
                Abort = false,
                MaxSizeRollBackups = 10
            });

            AssertFileEquals(TestLogFileName, "This is a message" + Environment.NewLine + "This is a message 2" + Environment.NewLine);
        }
        
        [Test]
        public void ShouldAppendToNonAbortedFile()
        {
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message",
                Abort = false,
                MaxSizeRollBackups = 10
            });
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message 2",
                Abort = false,
                MaxSizeRollBackups = 10
            });

            AssertFileEquals(TestLogFileName, "This is a message" + Environment.NewLine + "This is a message 2" + Environment.NewLine);
        }
        
        [Test]
        public void ShouldCreateNewFileIfAbortedAtTheEndOfFullFileLen()
        {
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message",
                Abort = false,
                MaxSizeRollBackups = 10,
                MappingBufferSize = "100",
                MaximumFileSize = "100",
                WriteBytes = "100"
            });
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message 2",
                Abort = false,
                MaxSizeRollBackups = 10,
                MappingBufferSize = "100",
                MaximumFileSize = "100",
            });

            AssertFileEquals(TestLogFileName, "This is a message 2" + Environment.NewLine);
        }
        
        [Test]
        public void ShouldWriteThroughMultipleBuffers()
        {
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message",
                Abort = false,
                MaxSizeRollBackups = 10,
                MappingBufferSize = "100",
                MaximumFileSize = "100",
                WriteBytes = "10kb"
            });

            var expected = TestUtils.BuildRepeatString("This is a message", 10 * 1024);

            AssertFileEquals(TestLogFileName, expected + Environment.NewLine);
        }
        
        [Test]
        public void ShouldLogFinalException()
        {
            TestUtils.StartLogProcess(new ProcessOptions
            {
                File = TestLogFileName,
                Text = "This is a message",
                Abort = false,
                MaxSizeRollBackups = 10,
                MappingBufferSize = "100kb",
                Throw = true
            });

            var expected = Environment.NewLine +
                           "System.NullReferenceException: Object reference not set to an instance of an object." +
                           Environment.NewLine + "   at Petabyte.log4net.Extensions.DummyLogger.Program";
            AssertFileEquals(TestLogFileName, expected, expected.Length);
        }
        
        [Test]
        public void ShouldAppendManyLines()
        {
            var logger = CreateLogger(TestLogFileName, new SilentErrorHandler(), "2MB", 2 * 1024 * 1024);
            for (int i = 0; i < 1000000; i++)
            {
                logger.Log(typeof(MemoryMappedAppenderTests), Level.Info, "How to Do Logging in C# With Log4net?", null);
            }
            DestroyLogger();
        }
        
        private static void DestroyLogger()
        {
            var h = (Hierarchy)LogManager.GetRepository("TestRepository");
            h.ResetConfiguration();
            //Replace the repository selector so that we can recreate the hierarchy with the same name if necessary
            LoggerManager.RepositorySelector = new DefaultRepositorySelector(typeof(Hierarchy));
        }
        
        private static void AssertFileEquals(string filename, string contents, int len = -1)
        {
#if NETSTANDARD1_3
			StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open));
#else
            var sr = new StreamReader(filename);
#endif
            string logcont = sr.ReadToEnd();
            sr.Close();
            if (len >= 0)
            {
                logcont = logcont.Substring(0, len);
            }
            
            Assert.AreEqual(contents, logcont, "Log contents is not what is expected");

            File.Delete(filename);
        }

        private static ILogger CreateLogger(string filename, IErrorHandler handler, string mapBufferSize = null, int maxFileSize = 100000, int maxSizeRollBackups = 0)
        {
            var h = (Hierarchy)LogManager.CreateRepository("TestRepository");
            var appender = new MemoryMappedAppender();
            appender.File = filename;
            appender.AppendToFile = false;
            appender.CountDirection = 0;
            appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.MaxFileSize = maxFileSize;
            appender.Encoding = Encoding.ASCII;
            appender.ErrorHandler = handler;
            appender.MaxSizeRollBackups = maxSizeRollBackups;
            appender.MappingBufferSize = mapBufferSize;
            
            var layout = new PatternLayout();
            layout.ConversionPattern = "%m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            h.Root.AddAppender(appender);
            h.Configured = true;

            ILogger log = h.GetLogger("Logger");
            return log;
        }
    }
}
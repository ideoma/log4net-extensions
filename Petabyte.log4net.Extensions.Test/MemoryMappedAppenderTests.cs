using System;
using System.Diagnostics;
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
        [SetUp]
        public void Setup()
        {
            TestUtils.SetUp();
        }

        [Test]
        public void TestLogOutput()
        {
            var filename = "test.log";
            StartLogProcess(new ProcessOptions
            {
                File = filename,
                Text = "This is a message",
                Abort = true
            });
            StartLogProcess(new ProcessOptions
            {
                File = filename,
                Text = "This is a message",
                Abort = true
            });

            AssertFileEquals(filename, "This is a message" + Environment.NewLine + "This is a message 2" + Environment.NewLine);
        }

        private void StartLogProcess(ProcessOptions options)
        {
            Process.Start("..\\..\\..\\..\\Petabytle.log4net.Extensions.Benchmarks\\bin\\");
        }

        private static void DestroyLogger()
        {
            var h = (Hierarchy)LogManager.GetRepository("TestRepository");
            h.ResetConfiguration();
            //Replace the repository selector so that we can recreate the hierarchy with the same name if necessary
            LoggerManager.RepositorySelector = new DefaultRepositorySelector(typeof(Hierarchy));
        }
        
        private static void AssertFileEquals(string filename, string contents)
        {
#if NETSTANDARD1_3
			StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open));
#else
            var sr = new StreamReader(filename);
#endif
            string logcont = sr.ReadToEnd();
            sr.Close();

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
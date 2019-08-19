using System;
using System.Text;
using CommandLine;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Petabyte;
using log4net.Repository.Hierarchy;
using log4net.Util;
using Petabyte.log4net.Extensions.Test;

namespace Petabyte.log4net.Extensions.DummyLogger
{
    
    public class Program
    {
        private static ILogger Logger;
        
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(
                (sender, exArgs) =>
                {
                    Logger.Log(typeof(Program), Level.Error, "", (Exception)exArgs.ExceptionObject);
                });
            
            Parser.Default.ParseArguments<ProcessOptions>(args)
                .WithParsed<ProcessOptions>(o =>
                {
                    StartLoggingWithProcessOptions(o);
                });
        }

        private static void StartLoggingWithProcessOptions(ProcessOptions processOptions)
        {
            Logger = CreateLogger(processOptions);
            if (processOptions.Throw)
            {
                throw new NullReferenceException();
            }

            var text = new StringBuilder(processOptions.Text);
            var writeSize = (int) OptionConverter.ToFileSize(processOptions.WriteBytes, -1);
            if (writeSize == -1)
            {
                writeSize = text.Length;
            }
            
            while (text.Length < writeSize)
            {
                var appendLen = Math.Min(writeSize - text.Length, processOptions.Text.Length);
                text.Append(processOptions.Text.Substring(0, appendLen));
            }
            
            Logger.Log(typeof(Program), Level.Info, text.ToString(), null);

            if (processOptions.Abort)
            {
                Environment.Exit(0);
            }
            else
            {
                var h = LogManager.GetRepository("TestRepository");
                h.ResetConfiguration();
            }
        }
        
        private static ILogger CreateLogger(ProcessOptions processOptions)
        {
            var h = (Hierarchy)LogManager.CreateRepository("TestRepository");

            var appender = new MemoryMappedAppender
            {
                File = processOptions.File,
                AppendToFile = true,
                CountDirection = 0,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaximumFileSize = processOptions.MaximumFileSize,
                Encoding = Encoding.ASCII,
                MaxSizeRollBackups = processOptions.MaxSizeRollBackups,
                MappingBufferSize = processOptions.MappingBufferSize
            };

            PatternLayout layout = new PatternLayout();
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
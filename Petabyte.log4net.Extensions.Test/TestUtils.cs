using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;
using log4net.Appender;
using log4net.Tests;
using log4net.Util;
using NUnit.Framework;


namespace Petabyte.log4net.Extensions.Test
{
    public static class TestUtils
    {
        public const string DefaultFileName = "test_41d3d834_4320f4da.log";

        public static void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            DeleteTestFiles();
        }
        
        /// <summary>
        /// Removes all test files that exist
        /// </summary>
        public static void DeleteTestFiles(string name = DefaultFileName)
        {
            ArrayList alFiles = GetExistingFiles(name);
            alFiles.AddRange(GetExistingFiles(name, true));
            foreach(string sFile in alFiles)
            {
                try
                {
                    Debug.WriteLine("Deleting test file " + sFile);
                    File.Delete(sFile);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Exception while deleting test file " + ex);
                }
            }
        }
        
        private static ArrayList GetExistingFiles(string baseFilePath)
        {
            return GetExistingFiles(baseFilePath, false);
        }
        
        private static ArrayList GetExistingFiles(string baseFilePath, bool preserveLogFileNameExtension)
        {
            var appender = new RollingFileAppender();
            appender.PreserveLogFileNameExtension = preserveLogFileNameExtension;
            appender.SecurityContext = NullSecurityContext.Instance;

            return (ArrayList)Utils.InvokeMethod(appender, "GetExistingFiles", baseFilePath);
        }
        
        public static void StartLogProcess(ProcessOptions options)
        {
            var executable = Path.Combine(TestContext.CurrentContext.TestDirectory, "Petabyte.log4net.Extensions.DummyLogger.exe");
            var args = SerializeOptions(options);
            
            var isCore = RuntimeInformation.FrameworkDescription.Contains("Core");
            if (isCore)
            {
                executable = "dotnet";
                args = Path.Combine(TestContext.CurrentContext.TestDirectory, "Petabyte.log4net.Extensions.DummyLogger.dll") + " " + args;
            }

            var process = Process.Start(executable, args);
            if (process != null)
            {
                process.WaitForExit();
            }
        }

        private static string SerializeOptions(ProcessOptions options)
        {
            var sb = new StringBuilder();
            foreach (var propertyInfo in typeof(ProcessOptions).GetProperties())
            {
                var value = propertyInfo.GetMethod.Invoke(options, new object[0]);
                if (value != null && (value.GetType() != typeof(int) || Convert.ToInt32(value) != 0))
                {
                    foreach (OptionAttribute attribute in propertyInfo.GetCustomAttributes(typeof(OptionAttribute)))
                    {
                        if (value is bool b && !b)
                        {
                            continue;
                        }
                        sb.Append(" -");
                        sb.Append(attribute.ShortName);
                        sb.Append(" \"");
                        sb.Append(value);
                        sb.Append("\"");
                    }
                }
            }

            return sb.ToString();
        }

        public static string BuildRepeatString(string txt, int writeSize)
        {
            var text = new StringBuilder(txt);
            if (writeSize == -1)
            {
                writeSize = text.Length;
            }
            
            while (text.Length < writeSize)
            {
                var appendLen = Math.Min(writeSize - text.Length, txt.Length);
                text.Append(txt.Substring(0, appendLen));
            }

            return text.ToString();
        }
    }
}
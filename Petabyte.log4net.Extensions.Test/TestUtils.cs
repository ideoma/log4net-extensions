using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
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
        public static void DeleteTestFiles()
        {
            ArrayList alFiles = GetExistingFiles(DefaultFileName);
            alFiles.AddRange(GetExistingFiles(DefaultFileName, true));
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
    }
}
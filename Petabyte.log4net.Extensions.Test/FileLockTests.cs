using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Petabyte.log4net.Extensions.Test
{
    public class FileLockTests
    {
        [Test]
        public void TestExclusiveLockLocks()
        {
            String filename = "test.log";
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            var bytes1 = Encoding.ASCII.GetBytes("Log line 1\n");
            fs.Write(bytes1, 0, bytes1.Length);
            fs.Flush();

            try
            {
                FileStream fs2 = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
            
            var bytes2 = Encoding.ASCII.GetBytes("Log line 2\n");
            fs.Write(bytes2, 0, bytes2.Length);
            fs.Flush();
            fs.Dispose();
            
            Assert.That(File.ReadAllText(filename), Is.EqualTo("Log line 1\n" + "Log line 2\n"));
        }

        [Test]
        public void OsVersion()
        {
            Console.WriteLine(GetNetCoreVersion());
            Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        }
        
        public static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return assemblyPath[netCoreAppIndex + 1];
            return null;
        }
    }
}
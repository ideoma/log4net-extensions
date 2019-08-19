using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net.Appender;
using log4net.Util;

namespace Petabytle.log4net.Extensions.Benchmarks
{
    public class TestUtils
    {
        
        /// <summary>
        /// Removes all test files that exist
        /// </summary>
        public static void DeleteTestFiles(string name)
        {
            ArrayList alFiles = GetExistingFiles(name);
            alFiles.AddRange(GetExistingFiles(name, true));
            foreach(string sFile in alFiles)
            {
                try
                {
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

            return (ArrayList)InvokeMethod(appender, "GetExistingFiles", baseFilePath);
        }
        
        public static object InvokeMethod(object target, string name, params object[] args)
        {
#if NETSTANDARD1_3
            var method = target.GetType().GetTypeInfo().GetDeclaredMethod(name);
            if (method == null)
            {
                method = target.GetType().BaseType.GetMethod(name);
            }
            return method.Invoke(target, args);
#else
			var method = target.GetType().GetMethod(name,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null,
				GetTypesArray(args), null);
			if (method == null)
			{
				method = target.GetType().BaseType.GetMethod(name,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null,
					GetTypesArray(args), null);
			}
			return method.Invoke(target, args);
#endif
        }
        
        private static Type[] GetTypesArray(object[] args)
        {
            Type[] types = new Type[args.Length];

            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    types[i] = typeof(object);
                }
                else
                {
                    types[i] = args[i].GetType();
                }
            }

            return types;
        }
    }
}
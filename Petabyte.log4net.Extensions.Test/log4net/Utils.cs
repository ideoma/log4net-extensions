#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using log4net.Repository;
using System;
using System.Reflection;

namespace log4net.Tests
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Utils
	{
		private Utils()
		{
		}

		public static object CreateInstance(string targetType)
		{
			return CreateInstance(Type.GetType(targetType, true, true));
		}

		public static object CreateInstance(Type targetType)
		{
			return targetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null).Invoke(null);
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

		public static object GetField(object target, string name)
		{
			var fieldInfo = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (fieldInfo == null)
				fieldInfo = target.GetType().BaseType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return fieldInfo.GetValue(target);
		}

		public static void SetField(object target, string name, object val)
		{
			var fieldInfo = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (fieldInfo == null)
			{
				fieldInfo = target.GetType().BaseType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			}
			fieldInfo.SetValue(target, val);
		}

		public static object GetProperty(object target, string name)
		{
			return target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).GetValue(target, null);
		}

		public static void SetProperty(object target, string name, object val)
		{
			target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SetValue(target, val, null);
		}

		public static object GetProperty(object target, string name, params object[] index)
		{
			return target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).GetValue(target, index);
		}

		public static void SetProperty(object target, string name, object val, params object[] index)
		{
			target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SetValue(target, val, index);
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

        internal const string PROPERTY_KEY = "prop1";

        internal static void RemovePropertyFromAllContexts() {
            GlobalContext.Properties.Remove(PROPERTY_KEY);
            ThreadContext.Properties.Remove(PROPERTY_KEY);
#if !NETCF
            LogicalThreadContext.Properties.Remove(PROPERTY_KEY);
#endif
        }

        // Wrappers because repository/logger retrieval APIs require an Assembly argument on NETSTANDARD1_3
        internal static ILog GetLogger(string name)
        {
#if NETSTANDARD1_3
            return LogManager.GetLogger(typeof(Utils).GetTypeInfo().Assembly, name);
#else
            return LogManager.GetLogger(name);
#endif
        }

        internal static ILoggerRepository GetRepository()
        {
#if NETSTANDARD1_3
            return LogManager.GetRepository(typeof(Utils).GetTypeInfo().Assembly);
#else
            return LogManager.GetRepository();
#endif
        }
    }
}
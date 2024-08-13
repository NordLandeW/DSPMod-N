using System;
using System.Reflection;
using BepInEx.Logging;

namespace tanu.AutoPilot
{
	internal static class LogManager
	{
		public static ManualLogSource Logger { private get; set; }

		public static void LogInfo(object data)
		{
			LogManager.Logger.LogInfo(data);
		}

		public static void LogInfo(MethodBase method)
		{
			LogManager.Logger.LogInfo(method.DeclaringType.Name + "." + method.Name);
		}

		public static void LogInfo(MethodBase method, object data)
		{
			LogManager.Logger.LogInfo(string.Concat(new string[]
			{
				method.DeclaringType.Name,
				".",
				method.Name,
				": ",
				(data != null) ? data.ToString() : null
			}));
		}

		public static void LogError(object data)
		{
			LogManager.Logger.LogError(data);
		}

		public static void LogError(MethodBase method, object data)
		{
			LogManager.Logger.LogError(string.Concat(new string[]
			{
				method.DeclaringType.Name,
				".",
				method.Name,
				": ",
				(data != null) ? data.ToString() : null
			}));
		}
	}
}

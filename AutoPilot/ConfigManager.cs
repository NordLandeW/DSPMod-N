using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace tanu.AutoPilot
{
	internal abstract class ConfigManager
	{
		public static ConfigFile Config { get; private set; }

		protected ConfigManager(ConfigFile config)
		{
			instance = this;
			Config = config;
			Config.SaveOnConfigSet = false;
		}

		public static void CheckConfig(Step step)
		{
			instance.CheckConfigImplements(step);
		}

		protected abstract void CheckConfigImplements(Step step);

		public static ConfigEntry<T> Bind<T>(ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null)
		{
			return Config.Bind<T>(configDefinition, defaultValue, configDescription);
		}

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, ConfigDescription configDescription = null)
		{
			return Config.Bind<T>(section, key, defaultValue, configDescription);
		}

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
		{
			return Config.Bind<T>(section, key, defaultValue, description);
		}

		public static ConfigEntry<T> GetEntry<T>(ConfigDefinition configDefinition)
		{
			ConfigEntry<T> result;
			try
			{
				result = (ConfigEntry<T>)Config[configDefinition];
			}
			catch (KeyNotFoundException ex)
			{
				LogManager.LogError(string.Format("{0}: configDefinition={1}", ex.GetType(), configDefinition));
				throw;
			}
			return result;
		}

		public static ConfigEntry<T> GetEntry<T>(string section, string key)
		{
			return GetEntry<T>(new ConfigDefinition(section, key));
		}

		public static T GetValue<T>(ConfigDefinition configDefinition)
		{
			return GetEntry<T>(configDefinition).Value;
		}

		public static T GetValue<T>(string section, string key)
		{
			return GetEntry<T>(section, key).Value;
		}

		public static bool ContainsKey(ConfigDefinition configDefinition)
		{
			return Config.ContainsKey(configDefinition);
		}

		public static bool ContainsKey(string section, string key)
		{
			return Config.ContainsKey(new ConfigDefinition(section, key));
		}

		public static bool UpdateEntry<T>(string section, string key, T value) where T : IComparable
		{
			ConfigEntry<T> entry = GetEntry<T>(section, key);
			T value2 = entry.Value;
			bool flag = value2.CompareTo(value) == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				entry.Value = value;
				result = true;
			}
			return result;
		}

		public static bool RemoveEntry(ConfigDefinition key)
		{
			return Config.Remove(key);
		}

		public static Dictionary<ConfigDefinition, string> GetOrphanedEntries()
		{
			bool flag = orphanedEntries == null;
			if (flag)
			{
				orphanedEntries = Traverse.Create(Config).Property<Dictionary<ConfigDefinition, string>>("OrphanedEntries").Value;
			}
			return orphanedEntries;
		}

		public static void Migration<T>(string newSection, string newKey, T defaultValue, string oldSection, string oldKey)
		{
			GetOrphanedEntries();
			ConfigDefinition key = new ConfigDefinition(oldSection, oldKey);
			string text;
			bool flag = orphanedEntries.TryGetValue(key, out text);
			if (flag)
			{
				Bind<T>(newSection, newKey, defaultValue).SetSerializedValue(text);
				orphanedEntries.Remove(key);
				LogManager.LogInfo(string.Concat(new string[]
				{
					"migration ",
					oldSection,
					".",
					oldKey,
					"(",
					text,
					") => ",
					newSection,
					".",
					newKey
				}));
			}
		}

		public static void Save(bool clearOrphanedEntries = false)
		{
			if (clearOrphanedEntries)
			{
				GetOrphanedEntries().Clear();
			}
			Config.Save();
			LogManager.LogInfo("save config.");
		}

		public static void Clear()
		{
			Config.Clear();
		}

		public static void Reload()
		{
			Config.Reload();
		}

		private static ConfigManager instance;

		private static Dictionary<ConfigDefinition, string> orphanedEntries;

		public enum Step
		{
			AWAKE,
			GAME_MAIN_BEGIN,
			STATE
		}
	}
}

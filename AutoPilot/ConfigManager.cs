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
			ConfigManager.instance = this;
			ConfigManager.Config = config;
			ConfigManager.Config.SaveOnConfigSet = false;
		}

		public static void CheckConfig(ConfigManager.Step step)
		{
			ConfigManager.instance.CheckConfigImplements(step);
		}

		protected abstract void CheckConfigImplements(ConfigManager.Step step);

		public static ConfigEntry<T> Bind<T>(ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null)
		{
			return ConfigManager.Config.Bind<T>(configDefinition, defaultValue, configDescription);
		}

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, ConfigDescription configDescription = null)
		{
			return ConfigManager.Config.Bind<T>(section, key, defaultValue, configDescription);
		}

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
		{
			return ConfigManager.Config.Bind<T>(section, key, defaultValue, description);
		}

		public static ConfigEntry<T> GetEntry<T>(ConfigDefinition configDefinition)
		{
			ConfigEntry<T> result;
			try
			{
				result = (ConfigEntry<T>)ConfigManager.Config[configDefinition];
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
			return ConfigManager.GetEntry<T>(new ConfigDefinition(section, key));
		}

		public static T GetValue<T>(ConfigDefinition configDefinition)
		{
			return ConfigManager.GetEntry<T>(configDefinition).Value;
		}

		public static T GetValue<T>(string section, string key)
		{
			return ConfigManager.GetEntry<T>(section, key).Value;
		}

		public static bool ContainsKey(ConfigDefinition configDefinition)
		{
			return ConfigManager.Config.ContainsKey(configDefinition);
		}

		public static bool ContainsKey(string section, string key)
		{
			return ConfigManager.Config.ContainsKey(new ConfigDefinition(section, key));
		}

		public static bool UpdateEntry<T>(string section, string key, T value) where T : IComparable
		{
			ConfigEntry<T> entry = ConfigManager.GetEntry<T>(section, key);
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
			return ConfigManager.Config.Remove(key);
		}

		public static Dictionary<ConfigDefinition, string> GetOrphanedEntries()
		{
			bool flag = ConfigManager.orphanedEntries == null;
			if (flag)
			{
				ConfigManager.orphanedEntries = Traverse.Create(ConfigManager.Config).Property<Dictionary<ConfigDefinition, string>>("OrphanedEntries").Value;
			}
			return ConfigManager.orphanedEntries;
		}

		public static void Migration<T>(string newSection, string newKey, T defaultValue, string oldSection, string oldKey)
		{
			ConfigManager.GetOrphanedEntries();
			ConfigDefinition key = new ConfigDefinition(oldSection, oldKey);
			string text;
			bool flag = ConfigManager.orphanedEntries.TryGetValue(key, out text);
			if (flag)
			{
				ConfigManager.Bind<T>(newSection, newKey, defaultValue).SetSerializedValue(text);
				ConfigManager.orphanedEntries.Remove(key);
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
				ConfigManager.GetOrphanedEntries().Clear();
			}
			ConfigManager.Config.Save();
			LogManager.LogInfo("save config.");
		}

		public static void Clear()
		{
			ConfigManager.Config.Clear();
		}

		public static void Reload()
		{
			ConfigManager.Config.Reload();
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

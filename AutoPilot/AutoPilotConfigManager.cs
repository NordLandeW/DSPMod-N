using System;
using BepInEx.Configuration;

namespace tanu.AutoPilot
{
	internal class AutoPilotConfigManager : ConfigManager
	{
		internal AutoPilotConfigManager(ConfigFile Config) : base(Config)
		{
		}

		protected override void CheckConfigImplements(ConfigManager.Step step)
		{
			bool flag = false;
			bool flag2 = step == ConfigManager.Step.AWAKE;
			if (flag2)
			{
				ConfigEntry<string> configEntry = ConfigManager.Bind<string>("Base", "ModVersion", "0.0.4", "Don't change.");
				configEntry.Value = "0.0.4";
				flag = true;
			}
			bool flag3 = step == ConfigManager.Step.AWAKE || step == ConfigManager.Step.GAME_MAIN_BEGIN;
			if (flag3)
			{
				AutoPilotDebugUI.Show = ConfigManager.Bind<bool>("Debug", "DebugWindowShow", false).Value;
				AutoPilotPlugin.Conf.MinEnergyPer = ConfigManager.Bind<int>("Setting", "MinEnergyPer", 20).Value;
				AutoPilotPlugin.Conf.MaxSpeed = ConfigManager.Bind<int>("Setting", "MaxSpeed", 2000).Value;
				AutoPilotPlugin.Conf.WarpMinRangeAU = ConfigManager.Bind<int>("Setting", "WarpMinRangeAU", 2).Value;
				AutoPilotPlugin.Conf.SpeedToWarp = ConfigManager.Bind<int>("Setting", "WarpSpeed", 1200).Value;
				AutoPilotPlugin.Conf.LocalWarpFlag = ConfigManager.Bind<bool>("Setting", "LocalWarp", false).Value;
				AutoPilotPlugin.Conf.AutoStartFlag = ConfigManager.Bind<bool>("Setting", "AutoStart", true).Value;
				AutoPilotPlugin.Conf.MainWindowJoinFlag = ConfigManager.Bind<bool>("Setting", "MainWindowJoin", true).Value;
				for (int i = 0; i < 2; i++)
				{
					AutoPilotMainUI.Rect[i].x = (float)ConfigManager.Bind<int>("State", string.Format("MainWindow{0}Left", i), 100).Value;
					AutoPilotMainUI.Rect[i].y = (float)ConfigManager.Bind<int>("State", string.Format("MainWindow{0}Top", i), 100).Value;
					AutoPilotConfigUI.Rect[i].x = (float)ConfigManager.Bind<int>("State", string.Format("ConfigWindow{0}Left", i), 100).Value;
					AutoPilotConfigUI.Rect[i].y = (float)ConfigManager.Bind<int>("State", string.Format("ConfigWindow{0}Top", i), 100).Value;
				}
				AutoPilotDebugUI.Rect.x = (float)ConfigManager.Bind<int>("State", "DebugWindowLeft", 100).Value;
				AutoPilotDebugUI.Rect.y = (float)ConfigManager.Bind<int>("State", "DebugWindowTop", 100).Value;
			}
			else
			{
				bool flag4 = step == ConfigManager.Step.STATE;
				if (flag4)
				{
					LogManager.LogInfo("check state.");
					flag |= ConfigManager.UpdateEntry<int>("Setting", "MinEnergyPer", AutoPilotPlugin.Conf.MinEnergyPer);
					flag |= ConfigManager.UpdateEntry<int>("Setting", "MaxSpeed", AutoPilotPlugin.Conf.MaxSpeed);
					flag |= ConfigManager.UpdateEntry<int>("Setting", "WarpMinRangeAU", AutoPilotPlugin.Conf.WarpMinRangeAU);
					flag |= ConfigManager.UpdateEntry<int>("Setting", "WarpSpeed", AutoPilotPlugin.Conf.SpeedToWarp);
					flag |= ConfigManager.UpdateEntry<bool>("Setting", "LocalWarp", AutoPilotPlugin.Conf.LocalWarpFlag);
					flag |= ConfigManager.UpdateEntry<bool>("Setting", "AutoStart", AutoPilotPlugin.Conf.AutoStartFlag);
					flag |= ConfigManager.UpdateEntry<bool>("Setting", "MainWindowJoin", AutoPilotPlugin.Conf.MainWindowJoinFlag);
					for (int j = 0; j < 2; j++)
					{
						flag |= ConfigManager.UpdateEntry<int>("State", string.Format("MainWindow{0}Left", j), (int)AutoPilotMainUI.Rect[j].x);
						flag |= ConfigManager.UpdateEntry<int>("State", string.Format("MainWindow{0}Top", j), (int)AutoPilotMainUI.Rect[j].y);
						flag |= ConfigManager.UpdateEntry<int>("State", string.Format("ConfigWindow{0}Left", j), (int)AutoPilotConfigUI.Rect[j].x);
						flag |= ConfigManager.UpdateEntry<int>("State", string.Format("ConfigWindow{0}Top", j), (int)AutoPilotConfigUI.Rect[j].y);
					}
					flag |= ConfigManager.UpdateEntry<int>("State", "DebugWindowLeft", (int)AutoPilotDebugUI.Rect.x);
					flag |= ConfigManager.UpdateEntry<int>("State", "DebugWindowTop", (int)AutoPilotDebugUI.Rect.y);
					AutoPilotMainUI.NextCheckGameTick = long.MaxValue;
				}
			}
			bool flag5 = flag;
			if (flag5)
			{
				ConfigManager.Save(false);
			}
		}
	}
}

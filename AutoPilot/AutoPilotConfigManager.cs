using BepInEx.Configuration;

namespace tanu.AutoPilot
{
    internal class AutoPilotConfigManager : ConfigManager
    {
        internal AutoPilotConfigManager(ConfigFile config) : base(config)
        {
        }

        protected override void CheckConfigImplements(Step step)
        {
            bool needsSave = false;

            switch (step)
            {
                case Step.AWAKE:
                    // Set mod version
                    ConfigEntry<string> modVersionEntry = Bind("Base", "ModVersion", AutoPilotPlugin.ModVersion, "Don't change.");
                    modVersionEntry.Value = AutoPilotPlugin.ModVersion;
                    needsSave = true;

                    // Load settings that are only needed on awake
                    LoadGeneralSettings();
                    break;

                case Step.GAME_MAIN_BEGIN:
                    // Load settings that can be reloaded in-game
                    LoadGeneralSettings();
                    break;

                case Step.STATE:
                    LogManager.LogInfo("Checking state and updating config.");

                    // Update settings
                    needsSave |= UpdateEntry("Setting", "MinEnergyPer", AutoPilotPlugin.Conf.MinEnergyPer);
                    needsSave |= UpdateEntry("Setting", "MaxSpeed", AutoPilotPlugin.Conf.MaxSpeed);
                    needsSave |= UpdateEntry("Setting", "WarpMinRangeAU", AutoPilotPlugin.Conf.WarpMinRangeAU);
                    needsSave |= UpdateEntry("Setting", "WarpSpeed", AutoPilotPlugin.Conf.SpeedToWarp);
                    needsSave |= UpdateEntry("Setting", "LocalWarp", AutoPilotPlugin.Conf.LocalWarpFlag);
                    needsSave |= UpdateEntry("Setting", "AutoStart", AutoPilotPlugin.Conf.AutoStartFlag);
                    needsSave |= UpdateEntry("Setting", "MainWindowJoin", AutoPilotPlugin.Conf.MainWindowJoinFlag);

                    // Update window positions
                    for (int i = 0; i < 2; i++)
                    {
                        needsSave |= UpdateEntry("State", $"MainWindow{i}Left", (int)AutoPilotMainUI.Rect[i].x);
                        needsSave |= UpdateEntry("State", $"MainWindow{i}Top", (int)AutoPilotMainUI.Rect[i].y);
                        needsSave |= UpdateEntry("State", $"ConfigWindow{i}Left", (int)AutoPilotConfigUI.Rect[i].x);
                        needsSave |= UpdateEntry("State", $"ConfigWindow{i}Top", (int)AutoPilotConfigUI.Rect[i].y);
                    }

                    needsSave |= UpdateEntry("State", "DebugWindowLeft", (int)AutoPilotDebugUI.Rect.x);
                    needsSave |= UpdateEntry("State", "DebugWindowTop", (int)AutoPilotDebugUI.Rect.y);

                    AutoPilotMainUI.NextCheckGameTick = long.MaxValue;
                    break;
            }

            if (needsSave)
            {
                Save(false);
            }
        }

        private void LoadGeneralSettings()
        {
            AutoPilotDebugUI.Show = Bind("Debug", "DebugWindowShow", false).Value;

            AutoPilotPlugin.Conf.MinEnergyPer = Bind("Setting", "MinEnergyPer", 20).Value;
            AutoPilotPlugin.Conf.MaxSpeed = Bind("Setting", "MaxSpeed", 2000).Value;
            AutoPilotPlugin.Conf.WarpMinRangeAU = Bind("Setting", "WarpMinRangeAU", 2).Value;
            AutoPilotPlugin.Conf.SpeedToWarp = Bind("Setting", "WarpSpeed", 400).Value;
            AutoPilotPlugin.Conf.LocalWarpFlag = Bind("Setting", "LocalWarp", false).Value;
            AutoPilotPlugin.Conf.AutoStartFlag = Bind("Setting", "AutoStart", true).Value;
            AutoPilotPlugin.Conf.MainWindowJoinFlag = Bind("Setting", "MainWindowJoin", true).Value;

            for (int i = 0; i < 2; i++)
            {
                AutoPilotMainUI.Rect[i].x = Bind("State", $"MainWindow{i}Left", 100).Value;
                AutoPilotMainUI.Rect[i].y = Bind("State", $"MainWindow{i}Top", 100).Value;
                AutoPilotConfigUI.Rect[i].x = Bind("State", $"ConfigWindow{i}Left", 100).Value;
                AutoPilotConfigUI.Rect[i].y = Bind("State", $"ConfigWindow{i}Top", 100).Value;
            }

            AutoPilotDebugUI.Rect.x = Bind("State", "DebugWindowLeft", 100).Value;
            AutoPilotDebugUI.Rect.y = Bind("State", "DebugWindowTop", 100).Value;
        }
    }
}

using BepInEx.Configuration;
using System.Collections.Generic;

namespace tanu.CruiseAssist
{
	public class CruiseAssistConfigManager : ConfigManager
	{
		public CruiseAssistConfigManager(ConfigFile Config) : base(Config)
		{
		}

		protected override void CheckConfigImplements(Step step)
		{
			bool saveFlag = false;

			if (step == Step.AWAKE)
			{
				var modVersion = Bind<string>("Base", "ModVersion", CruiseAssistPlugin.ModVersion, "Don't change.");
				modVersion.Value = CruiseAssistPlugin.ModVersion;

				Migration("State", "MainWindow0Left", 100, "State", "InfoWindowLeft");
				Migration("State", "MainWindow0Top", 100, "State", "InfoWindowTop");
				Migration("State", "MainWindow0Left", 100, "State", "MainWindowLeft");
				Migration("State", "MainWindow0Top", 100, "State", "MainWindowTop");
				Migration("State", "StarListWindow0Left", 100, "State", "StarListWindowLeft");
				Migration("State", "StarListWindow0Top", 100, "State", "StarListWindowTop");

				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.GAME_MAIN_BEGIN)
			{
				CruiseAssistDebugUI.Show = Bind("Debug", "DebugWindowShow", false).Value;

				CruiseAssistPlugin.Enable = Bind("Setting", "Enable", true).Value;

				CruiseAssistPlugin.MarkVisitedFlag = Bind("Setting", "MarkVisited", true).Value;
				CruiseAssistPlugin.SelectFocusFlag = Bind("Setting", "SelectFocus", true).Value;
				CruiseAssistPlugin.HideDuplicateHistoryFlag = Bind("Setting", "HideDuplicateHistory", true).Value;
				CruiseAssistPlugin.AutoDisableLockCursorFlag = Bind("Setting", "AutoDisableLockCursor", false).Value;
                CruiseAssistPlugin.TrackDarkFogSeedsFlag = Bind("Setting", "TrackDarkFogSeeds", true).Value;

                CruiseAssistMainUI.Scale = (float)Bind("Setting", "UIScale", 150).Value;

				var viewModeStr = Bind("Setting", "MainWindowViewMode", CruiseAssistMainUIViewMode.FULL.ToString()).Value;
				EnumUtils.TryParse<CruiseAssistMainUIViewMode>(viewModeStr, out CruiseAssistMainUI.ViewMode);

				for (int i = 0; i < 2; ++i)
				{
					CruiseAssistMainUI.Rect[i].x = (float)Bind("State", $"MainWindow{i}Left", 100).Value;
					CruiseAssistMainUI.Rect[i].y = (float)Bind("State", $"MainWindow{i}Top", 100).Value;
					CruiseAssistStarListUI.Rect[i].x = (float)Bind("State", $"StarListWindow{i}Left", 100).Value;
					CruiseAssistStarListUI.Rect[i].y = (float)Bind("State", $"StarListWindow{i}Top", 100).Value;
					CruiseAssistConfigUI.Rect[i].x = (float)Bind("State", $"ConfigWindow{i}Left", 100).Value;
					CruiseAssistConfigUI.Rect[i].y = (float)Bind("State", $"ConfigWindow{i}Top", 100).Value;
				}

				CruiseAssistStarListUI.ListSelected = Bind("State", "StarListWindowListSelected", 0).Value;

				CruiseAssistDebugUI.Rect.x = (float)Bind("State", "DebugWindowLeft", 100).Value;
				CruiseAssistDebugUI.Rect.y = (float)Bind("State", "DebugWindowTop", 100).Value;

				if (!DSPGame.IsMenuDemo && GameMain.galaxy != null)
				{
					CruiseAssistPlugin.History = ListUtils.ParseToIntList(Bind("Save", $"History_{GameMain.galaxy.seed}", "").Value);
					CruiseAssistPlugin.Bookmark = ListUtils.ParseToIntList(Bind("Save", $"Bookmark_{GameMain.galaxy.seed}", "").Value);
				}
				else
				{
					CruiseAssistPlugin.History = new List<int>();
					CruiseAssistPlugin.Bookmark = new List<int>();
				}
			}
			else if (step == Step.STATE)
			{
				LogManager.LogInfo("check state.");

				saveFlag |= UpdateEntry("Setting", "Enable", CruiseAssistPlugin.Enable);

				saveFlag |= UpdateEntry("Setting", "MarkVisited", CruiseAssistPlugin.MarkVisitedFlag);
				saveFlag |= UpdateEntry("Setting", "SelectFocus", CruiseAssistPlugin.SelectFocusFlag);
				saveFlag |= UpdateEntry("Setting", "HideDuplicateHistory", CruiseAssistPlugin.HideDuplicateHistoryFlag);
				saveFlag |= UpdateEntry("Setting", "AutoDisableLockCursor", CruiseAssistPlugin.AutoDisableLockCursorFlag);
                saveFlag |= UpdateEntry("Setting", "TrackDarkFogSeeds", CruiseAssistPlugin.TrackDarkFogSeedsFlag);

                saveFlag |= UpdateEntry("Setting", "UIScale", (int)CruiseAssistMainUI.Scale);

				saveFlag |= UpdateEntry("Setting", "MainWindowViewMode", CruiseAssistMainUI.ViewMode.ToString());

				for (int i = 0; i < 2; ++i)
				{
					saveFlag |= UpdateEntry("State", $"MainWindow{i}Left", (int)CruiseAssistMainUI.Rect[i].x);
					saveFlag |= UpdateEntry("State", $"MainWindow{i}Top", (int)CruiseAssistMainUI.Rect[i].y);
					saveFlag |= UpdateEntry("State", $"StarListWindow{i}Left", (int)CruiseAssistStarListUI.Rect[i].x);
					saveFlag |= UpdateEntry("State", $"StarListWindow{i}Top", (int)CruiseAssistStarListUI.Rect[i].y);
					saveFlag |= UpdateEntry("State", $"ConfigWindow{i}Left", (int)CruiseAssistConfigUI.Rect[i].x);
					saveFlag |= UpdateEntry("State", $"ConfigWindow{i}Top", (int)CruiseAssistConfigUI.Rect[i].y);
				}

				saveFlag |= UpdateEntry("State", "StarListWindowListSelected", CruiseAssistStarListUI.ListSelected);

				saveFlag |= UpdateEntry("State", "DebugWindowLeft", (int)CruiseAssistDebugUI.Rect.x);
				saveFlag |= UpdateEntry("State", "DebugWindowTop", (int)CruiseAssistDebugUI.Rect.y);

				if (!DSPGame.IsMenuDemo && GameMain.galaxy != null)
				{
					if (!ContainsKey("Save", $"History_{GameMain.galaxy.seed}") || !ContainsKey("Save", $"Bookmark_{GameMain.galaxy.seed}"))
					{
						Bind("Save", $"History_{GameMain.galaxy.seed}", "");
						Bind("Save", $"Bookmark_{GameMain.galaxy.seed}", "");
						saveFlag = true;
					}
					saveFlag |= UpdateEntry("Save", $"History_{GameMain.galaxy.seed}", ListUtils.ToString(CruiseAssistPlugin.History));
					saveFlag |= UpdateEntry("Save", $"Bookmark_{GameMain.galaxy.seed}", ListUtils.ToString(CruiseAssistPlugin.Bookmark));
				}

				CruiseAssistMainUI.NextCheckGameTick = long.MaxValue;
			}
			if (saveFlag)
			{
				Save(false);
			}
		}
	}
}

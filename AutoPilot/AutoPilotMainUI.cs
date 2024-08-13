using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
	internal class AutoPilotMainUI
	{
		public static void OnGUI()
		{
			AutoPilotMainUI.wIdx = CruiseAssistMainUI.wIdx;
			CruiseAssistMainUIViewMode viewMode = CruiseAssistMainUI.ViewMode;
			CruiseAssistMainUIViewMode cruiseAssistMainUIViewMode = viewMode;
			if (cruiseAssistMainUIViewMode != CruiseAssistMainUIViewMode.FULL)
			{
				if (cruiseAssistMainUIViewMode == CruiseAssistMainUIViewMode.MINI)
				{
					AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].width = CruiseAssistMainUI.Rect[AutoPilotMainUI.wIdx].width;
					AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].height = 70f;
				}
			}
			else
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].width = CruiseAssistMainUI.Rect[AutoPilotMainUI.wIdx].width;
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].height = 150f;
			}
			GUIStyle guistyle = new GUIStyle(CruiseAssistMainUI.WindowStyle);
			guistyle.fontSize = 11;
			AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx] = GUILayout.Window(99031291, AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx], new GUI.WindowFunction(AutoPilotMainUI.WindowFunction), "AutoPilot", guistyle, Array.Empty<GUILayoutOption>());
			float num = CruiseAssistMainUI.Scale / 100f;
			bool mainWindowJoinFlag = AutoPilotPlugin.Conf.MainWindowJoinFlag;
			if (mainWindowJoinFlag)
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x = CruiseAssistMainUI.Rect[CruiseAssistMainUI.wIdx].x;
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y = CruiseAssistMainUI.Rect[CruiseAssistMainUI.wIdx].yMax;
			}
			bool flag = (float)Screen.width / num < AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].xMax;
			if (flag)
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x = (float)Screen.width / num - AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].width;
			}
			bool flag2 = AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x < 0f;
			if (flag2)
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x = 0f;
			}
			bool flag3 = (float)Screen.height / num < AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].yMax;
			if (flag3)
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y = (float)Screen.height / num - AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].height;
			}
			bool flag4 = AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y < 0f;
			if (flag4)
			{
				AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y = 0f;
			}
			bool flag5 = AutoPilotMainUI.lastCheckWindowLeft[AutoPilotMainUI.wIdx] != float.MinValue;
			if (flag5)
			{
				bool flag6 = AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x != AutoPilotMainUI.lastCheckWindowLeft[AutoPilotMainUI.wIdx] || AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y != AutoPilotMainUI.lastCheckWindowTop[AutoPilotMainUI.wIdx];
				if (flag6)
				{
					AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
				}
			}
			AutoPilotMainUI.lastCheckWindowLeft[AutoPilotMainUI.wIdx] = AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].x;
			AutoPilotMainUI.lastCheckWindowTop[AutoPilotMainUI.wIdx] = AutoPilotMainUI.Rect[AutoPilotMainUI.wIdx].y;
			bool flag7 = AutoPilotMainUI.NextCheckGameTick <= GameMain.gameTick;
			if (flag7)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			bool flag = CruiseAssistMainUI.ViewMode == CruiseAssistMainUIViewMode.FULL;
			if (flag)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUIStyle guistyle = new GUIStyle(GUI.skin.label);
				guistyle.fontSize = 12;
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				string text = (AutoPilotPlugin.State == AutoPilotState.INACTIVE) ? "---" : (((double)AutoPilotPlugin.Conf.MinEnergyPer < AutoPilotPlugin.EnergyPer) ? "OK" : "NG");
				guistyle.normal.textColor = ((text == "OK") ? Color.cyan : ((text == "NG") ? Color.red : Color.white));
				GUILayout.Label("Energy : " + text, guistyle, Array.Empty<GUILayoutOption>());
				string text2 = (AutoPilotPlugin.State == AutoPilotState.INACTIVE) ? "---" : ((CruiseAssistPlugin.TargetStar == null) ? "---" : (GameMain.mainPlayer.warping ? "---" : ((!AutoPilotPlugin.Conf.LocalWarpFlag && GameMain.localStar != null && CruiseAssistPlugin.TargetStar.id == GameMain.localStar.id) ? "---" : ((CruiseAssistPlugin.TargetRange < (double)(AutoPilotPlugin.Conf.WarpMinRangeAU * 40000)) ? "---" : ((AutoPilotPlugin.WarperCount < 1) ? "NG" : "OK")))));
				guistyle.normal.textColor = ((text2 == "OK") ? Color.cyan : ((text2 == "NG") ? Color.red : Color.white));
				GUILayout.Label("Warper : " + text2, guistyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndVertical();
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				string text3 = (AutoPilotPlugin.State == AutoPilotState.INACTIVE) ? "---" : (AutoPilotPlugin.LeavePlanet ? "ON" : "OFF");
				guistyle.normal.textColor = ((text3 == "ON") ? Color.cyan : Color.white);
				GUILayout.Label("Leave Planet : " + text3, guistyle, Array.Empty<GUILayoutOption>());
				string text4 = (AutoPilotPlugin.State == AutoPilotState.INACTIVE) ? "---" : (AutoPilotPlugin.SpeedUp ? "ON" : "OFF");
				guistyle.normal.textColor = ((text4 == "ON") ? Color.cyan : Color.white);
				GUILayout.Label("Speed UP : " + text4, guistyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUIStyle guistyle2 = new GUIStyle(GUI.skin.label);
			guistyle2.fixedWidth = 160f;
			guistyle2.fixedHeight = 32f;
			guistyle2.fontSize = 14;
			guistyle2.alignment = TextAnchor.MiddleLeft;
			bool flag2 = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			if (flag2)
			{
				GUILayout.Label("Auto Pilot Inactive.", guistyle2, Array.Empty<GUILayoutOption>());
			}
			else
			{
				guistyle2.normal.textColor = Color.cyan;
				GUILayout.Label("Auto Pilot Active.", guistyle2, Array.Empty<GUILayoutOption>());
			}
			GUILayout.FlexibleSpace();
			GUIStyle guistyle3 = new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
			guistyle3.fixedWidth = 50f;
			guistyle3.fixedHeight = 18f;
			guistyle3.fontSize = 11;
			guistyle3.alignment = TextAnchor.MiddleCenter;
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			bool flag3 = GUILayout.Button("Config", guistyle3, Array.Empty<GUILayoutOption>());
			if (flag3)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				bool[] show = AutoPilotConfigUI.Show;
				int num = AutoPilotMainUI.wIdx;
				show[num] = !show[num];
				bool flag4 = AutoPilotConfigUI.Show[AutoPilotMainUI.wIdx];
				if (flag4)
				{
					AutoPilotConfigUI.TempMinEnergyPer = AutoPilotPlugin.Conf.MinEnergyPer.ToString();
					AutoPilotConfigUI.TempMaxSpeed = AutoPilotPlugin.Conf.MaxSpeed.ToString();
					AutoPilotConfigUI.TempWarpMinRangeAU = AutoPilotPlugin.Conf.WarpMinRangeAU.ToString();
					AutoPilotConfigUI.TempSpeedToWarp = AutoPilotPlugin.Conf.SpeedToWarp.ToString();
				}
			}
			bool flag5 = GUILayout.Button("Start", guistyle3, Array.Empty<GUILayoutOption>());
			if (flag5)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotPlugin.State = AutoPilotState.ACTIVE;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Button("-", guistyle3, Array.Empty<GUILayoutOption>());
			bool flag6 = GUILayout.Button("Stop", guistyle3, Array.Empty<GUILayoutOption>());
			if (flag6)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotPlugin.State = AutoPilotState.INACTIVE;
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		private static int wIdx = 0;

		public const float WindowWidthFull = 398f;

		public const float WindowHeightFull = 150f;

		public const float WindowWidthMini = 288f;

		public const float WindowHeightMini = 70f;

		public static Rect[] Rect = new Rect[]
		{
			new Rect(0f, 0f, 398f, 150f),
			new Rect(0f, 0f, 398f, 150f)
		};

		private static float[] lastCheckWindowLeft = new float[]
		{
			float.MinValue,
			float.MinValue
		};

		private static float[] lastCheckWindowTop = new float[]
		{
			float.MinValue,
			float.MinValue
		};

		public static long NextCheckGameTick = long.MaxValue;
	}
}

using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
	internal class AutoPilotConfigUI
	{
		public static void OnGUI()
		{
			AutoPilotConfigUI.wIdx = CruiseAssistMainUI.wIdx;
			AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx] = GUILayout.Window(99031292, AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx], new GUI.WindowFunction(AutoPilotConfigUI.WindowFunction), "AutoPilot - Config", CruiseAssistMainUI.WindowStyle, Array.Empty<GUILayoutOption>());
			float num = CruiseAssistMainUI.Scale / 100f;
			bool flag = (float)Screen.width / num < AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].xMax;
			if (flag)
			{
				AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].x = (float)Screen.width / num - AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].width;
			}
			bool flag2 = AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].x < 0f;
			if (flag2)
			{
				AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].x = 0f;
			}
			bool flag3 = (float)Screen.height / num < AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].yMax;
			if (flag3)
			{
				AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].y = (float)Screen.height / num - AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].height;
			}
			bool flag4 = AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].y < 0f;
			if (flag4)
			{
				AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].y = 0f;
			}
			bool flag5 = AutoPilotConfigUI.lastCheckWindowLeft[AutoPilotConfigUI.wIdx] != float.MinValue;
			if (flag5)
			{
				bool flag6 = AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].x != AutoPilotConfigUI.lastCheckWindowLeft[AutoPilotConfigUI.wIdx] || AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].y != AutoPilotConfigUI.lastCheckWindowTop[AutoPilotConfigUI.wIdx];
				if (flag6)
				{
					AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
				}
			}
			AutoPilotConfigUI.lastCheckWindowLeft[AutoPilotConfigUI.wIdx] = AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].x;
			AutoPilotConfigUI.lastCheckWindowTop[AutoPilotConfigUI.wIdx] = AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].y;
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.fontSize = 12;
			guistyle.fixedHeight = 20f;
			guistyle.alignment = TextAnchor.MiddleLeft;
			GUIStyle guistyle2 = new GUIStyle(CruiseAssistMainUI.BaseTextFieldStyle);
			guistyle2.fontSize = 12;
			guistyle2.fixedWidth = 60f;
			guistyle.fixedHeight = 20f;
			guistyle2.alignment = TextAnchor.MiddleRight;
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			guistyle.fixedWidth = 240f;
			GUILayout.Label(Localization.Translate("Min Energy Percent (0-100 default:20)"), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			string instr = GUILayout.TextField(AutoPilotConfigUI.TempMinEnergyPer, guistyle2, Array.Empty<GUILayoutOption>());
			AutoPilotConfigUI.SetValue(ref AutoPilotConfigUI.TempMinEnergyPer, instr, 0, 100, ref AutoPilotPlugin.Conf.MinEnergyPer);
			guistyle.fixedWidth = 20f;
			GUILayout.Label("%", guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			guistyle.fixedWidth = 240f;
			GUILayout.Label(Localization.Translate("Max Speed (0-2000 default:2000)"), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			string instr2 = GUILayout.TextField(AutoPilotConfigUI.TempMaxSpeed, guistyle2, Array.Empty<GUILayoutOption>());
			AutoPilotConfigUI.SetValue(ref AutoPilotConfigUI.TempMaxSpeed, instr2, 0, 2000, ref AutoPilotPlugin.Conf.MaxSpeed);
			guistyle.fixedWidth = 20f;
			GUILayout.Label("m/s", guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			guistyle.fixedWidth = 240f;
			GUILayout.Label(Localization.Translate("Warp Min Range (1-60 default:2)"), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			string instr3 = GUILayout.TextArea(AutoPilotConfigUI.TempWarpMinRangeAU, guistyle2, Array.Empty<GUILayoutOption>());
			AutoPilotConfigUI.SetValue(ref AutoPilotConfigUI.TempWarpMinRangeAU, instr3, 1, 60, ref AutoPilotPlugin.Conf.WarpMinRangeAU);
			guistyle.fixedWidth = 20f;
			GUILayout.Label("AU", guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			guistyle.fixedWidth = 240f;
			GUILayout.Label(Localization.Translate("Speed to warp (0-2000 default:1200)"), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			string instr4 = GUILayout.TextArea(AutoPilotConfigUI.TempSpeedToWarp, guistyle2, Array.Empty<GUILayoutOption>());
			AutoPilotConfigUI.SetValue(ref AutoPilotConfigUI.TempSpeedToWarp, instr4, 0, 2000, ref AutoPilotPlugin.Conf.SpeedToWarp);
			guistyle.fixedWidth = 20f;
			GUILayout.Label("m/s", guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUIStyle guistyle3 = new GUIStyle(CruiseAssistMainUI.BaseToggleStyle);
			guistyle3.fixedHeight = 20f;
			guistyle3.fontSize = 12;
			guistyle3.alignment = TextAnchor.LowerLeft;
			GUI.changed = false;
			AutoPilotPlugin.Conf.LocalWarpFlag = GUILayout.Toggle(AutoPilotPlugin.Conf.LocalWarpFlag, Localization.Translate("Warp to planet in local system."), guistyle3, Array.Empty<GUILayoutOption>());
			bool changed = GUI.changed;
			if (changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
			}
			GUI.changed = false;
			AutoPilotPlugin.Conf.AutoStartFlag = GUILayout.Toggle(AutoPilotPlugin.Conf.AutoStartFlag, Localization.Translate("Start AutoPilot when set target planet."), guistyle3, Array.Empty<GUILayoutOption>());
			bool changed2 = GUI.changed;
			if (changed2)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
			}
			GUI.changed = false;
			AutoPilotPlugin.Conf.MainWindowJoinFlag = GUILayout.Toggle(AutoPilotPlugin.Conf.MainWindowJoinFlag, Localization.Translate("Join AutoPilot window to CruiseAssist window."), guistyle3, Array.Empty<GUILayoutOption>());
			bool changed3 = GUI.changed;
			if (changed3)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
			}
			GUILayout.EndVertical();
			bool flag = GUI.Button(new Rect(AutoPilotConfigUI.Rect[AutoPilotConfigUI.wIdx].width - 16f, 1f, 16f, 16f), "", CruiseAssistMainUI.CloseButtonStyle);
			if (flag)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
				AutoPilotConfigUI.Show[AutoPilotConfigUI.wIdx] = false;
			}
			GUI.DragWindow();
		}

		private static bool SetValue(ref string temp, string instr, int min, int max, ref int value)
		{
			bool flag = string.IsNullOrEmpty(instr);
			bool result;
			if (flag)
			{
				temp = string.Empty;
				result = false;
			}
			else
			{
				int num;
				bool flag2 = int.TryParse(instr, out num);
				if (flag2)
				{
					bool flag3 = num < min;
					if (flag3)
					{
						num = min;
					}
					else
					{
						bool flag4 = max < num;
						if (flag4)
						{
							num = max;
						}
					}
					value = num;
					temp = value.ToString();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static int wIdx = 0;

		public const float WindowWidth = 400f;

		public const float WindowHeight = 400f;

		public static bool[] Show = new bool[2];

		public static Rect[] Rect = new Rect[]
		{
			new Rect(0f, 0f, 400f, 400f),
			new Rect(0f, 0f, 400f, 400f)
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

		public static string TempMinEnergyPer;

		public static string TempMaxSpeed;

		public static string TempWarpMinRangeAU;

		public static string TempSpeedToWarp;
	}
}

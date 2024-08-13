using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
	internal class AutoPilotDebugUI
	{
		public static void OnGUI()
		{
            GUIStyle guistyle = new GUIStyle(GUI.skin.window);
			guistyle.fontSize = 11;
			AutoPilotDebugUI.Rect = GUILayout.Window(99031293, AutoPilotDebugUI.Rect, new GUI.WindowFunction(AutoPilotDebugUI.WindowFunction), "AutoPilot - Debug", guistyle, Array.Empty<GUILayoutOption>());
			float num = CruiseAssistMainUI.Scale / 100f;
			bool flag = (float)Screen.width < AutoPilotDebugUI.Rect.xMax;
			if (flag)
			{
				AutoPilotDebugUI.Rect.x = (float)Screen.width - AutoPilotDebugUI.Rect.width;
			}
			bool flag2 = AutoPilotDebugUI.Rect.x < 0f;
			if (flag2)
			{
				AutoPilotDebugUI.Rect.x = 0f;
			}
			bool flag3 = (float)Screen.height < AutoPilotDebugUI.Rect.yMax;
			if (flag3)
			{
				AutoPilotDebugUI.Rect.y = (float)Screen.height - AutoPilotDebugUI.Rect.height;
			}
			bool flag4 = AutoPilotDebugUI.Rect.y < 0f;
			if (flag4)
			{
				AutoPilotDebugUI.Rect.y = 0f;
			}
			bool flag5 = AutoPilotDebugUI.lastCheckWindowLeft != float.MinValue;
			if (flag5)
			{
				bool flag6 = AutoPilotDebugUI.Rect.x != AutoPilotDebugUI.lastCheckWindowLeft || AutoPilotDebugUI.Rect.y != AutoPilotDebugUI.lastCheckWindowTop;
				if (flag6)
				{
					AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
				}
			}
			AutoPilotDebugUI.lastCheckWindowLeft = AutoPilotDebugUI.Rect.x;
			AutoPilotDebugUI.lastCheckWindowTop = AutoPilotDebugUI.Rect.y;
			bool flag7 = AutoPilotMainUI.NextCheckGameTick <= GameMain.gameTick;
			if (flag7)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.fontSize = 12;
			AutoPilotDebugUI.scrollPos = GUILayout.BeginScrollView(AutoPilotDebugUI.scrollPos, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("GameMain.mainPlayer.uPosition={0}", GameMain.mainPlayer.uPosition), guistyle, Array.Empty<GUILayoutOption>());
			bool flag = GameMain.localPlanet != null && CruiseAssistPlugin.TargetUPos != VectorLF3.zero;
			if (flag)
			{
				Player mainPlayer = GameMain.mainPlayer;
				VectorLF3 targetUPos = CruiseAssistPlugin.TargetUPos;
				double magnitude = (targetUPos - mainPlayer.uPosition).magnitude;
				double magnitude2 = (targetUPos - GameMain.localPlanet.uPosition).magnitude;
				VectorLF3 vec = mainPlayer.uPosition - GameMain.localPlanet.uPosition;
				VectorLF3 vec2 = CruiseAssistPlugin.TargetUPos - GameMain.localPlanet.uPosition;
				GUILayout.Label("range1=" + AutoPilotDebugUI.RangeToString(magnitude), guistyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label("range2=" + AutoPilotDebugUI.RangeToString(magnitude2), guistyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("range1>range2={0}", magnitude > magnitude2), guistyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("angle={0}", Vector3.Angle(vec, vec2)), guistyle, Array.Empty<GUILayoutOption>());
			}
			Mecha mecha = GameMain.mainPlayer.mecha;
			GUILayout.Label(string.Format("mecha.coreEnergy={0}", mecha.coreEnergy), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("mecha.coreEnergyCap={0}", mecha.coreEnergyCap), guistyle, Array.Empty<GUILayoutOption>());
			double num = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
			GUILayout.Label(string.Format("energyPer={0}", num), guistyle, Array.Empty<GUILayoutOption>());
			double magnitude3 = GameMain.mainPlayer.controller.actionSail.visual_uvel.magnitude;
			GUILayout.Label("spped=" + AutoPilotDebugUI.RangeToString(magnitude3), guistyle, Array.Empty<GUILayoutOption>());
			EMovementState movementStateInFrame = GameMain.mainPlayer.controller.movementStateInFrame;
			GUILayout.Label(string.Format("movementStateInFrame={0}", movementStateInFrame), guistyle, Array.Empty<GUILayoutOption>());
			GUIStyle guistyle2 = new GUIStyle(GUI.skin.toggle);
			guistyle2.fixedHeight = 20f;
			guistyle2.fontSize = 12;
			guistyle2.alignment = TextAnchor.LowerLeft;
			GUI.changed = false;
			AutoPilotPlugin.Conf.IgnoreGravityFlag = GUILayout.Toggle(AutoPilotPlugin.Conf.IgnoreGravityFlag, "Ignore gravity.", guistyle2, Array.Empty<GUILayoutOption>());
			bool changed = GUI.changed;
			if (changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		public static string RangeToString(double range)
		{
			bool flag = range < 10000.0;
			string result;
			if (flag)
			{
				result = ((int)(range + 0.5)).ToString() + "m ";
			}
			else
			{
				bool flag2 = range < 600000.0;
				if (flag2)
				{
					result = (range / 40000.0).ToString("0.00") + "AU";
				}
				else
				{
					result = (range / 2400000.0).ToString("0.00") + "Ly";
				}
			}
			return result;
		}

		public static bool Show = false;

		public static Rect Rect = new Rect(0f, 0f, 400f, 400f);

		private static float lastCheckWindowLeft = float.MinValue;

		private static float lastCheckWindowTop = float.MinValue;

		private static Vector2 scrollPos = Vector2.zero;
	}
}

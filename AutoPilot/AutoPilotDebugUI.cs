using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
	internal class AutoPilotDebugUI
	{
		public static void OnGUI()
		{
			GUIStyle windowStyle = new GUIStyle(GUI.skin.window) { fontSize = 11 };

			AutoPilotDebugUI.Rect = GUILayout.Window(
				99031293,
				AutoPilotDebugUI.Rect,
				new GUI.WindowFunction(AutoPilotDebugUI.WindowFunction),
				"AutoPilot - Debug",
				windowStyle,
				Array.Empty<GUILayoutOption>());

			float uiScaleFactor = CruiseAssistMainUI.Scale / 100f;

			if ((float)Screen.width < AutoPilotDebugUI.Rect.xMax)
			{
				AutoPilotDebugUI.Rect.x = (float)Screen.width - AutoPilotDebugUI.Rect.width;
			}
			if (AutoPilotDebugUI.Rect.x < 0f)
			{
				AutoPilotDebugUI.Rect.x = 0f;
			}
			if ((float)Screen.height < AutoPilotDebugUI.Rect.yMax)
			{
				AutoPilotDebugUI.Rect.y = (float)Screen.height - AutoPilotDebugUI.Rect.height;
			}
			if (AutoPilotDebugUI.Rect.y < 0f)
			{
				AutoPilotDebugUI.Rect.y = 0f;
			}

			if (AutoPilotDebugUI.lastCheckWindowLeft != float.MinValue)
			{
				bool moved =
					AutoPilotDebugUI.Rect.x != AutoPilotDebugUI.lastCheckWindowLeft ||
					AutoPilotDebugUI.Rect.y != AutoPilotDebugUI.lastCheckWindowTop;

				if (moved)
				{
					AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
				}
			}

			AutoPilotDebugUI.lastCheckWindowLeft = AutoPilotDebugUI.Rect.x;
			AutoPilotDebugUI.lastCheckWindowTop = AutoPilotDebugUI.Rect.y;

			if (AutoPilotMainUI.NextCheckGameTick <= GameMain.gameTick)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

			GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };

			AutoPilotDebugUI.scrollPos = GUILayout.BeginScrollView(AutoPilotDebugUI.scrollPos, Array.Empty<GUILayoutOption>());

			GUILayout.Label(string.Format("GameMain.mainPlayer.uPosition={0}", GameMain.mainPlayer.uPosition), labelStyle, Array.Empty<GUILayoutOption>());

			if (GameMain.localPlanet != null && CruiseAssistPlugin.TargetUPos != VectorLF3.zero)
			{
				Player player = GameMain.mainPlayer;
				VectorLF3 targetUPos = CruiseAssistPlugin.TargetUPos;

				double rangeToTarget = (targetUPos - player.uPosition).magnitude;
				double rangeTargetFromPlanet = (targetUPos - GameMain.localPlanet.uPosition).magnitude;
				VectorLF3 playerFromPlanet = player.uPosition - GameMain.localPlanet.uPosition;
				VectorLF3 targetFromPlanet = targetUPos - GameMain.localPlanet.uPosition;

				GUILayout.Label("range1=" + AutoPilotDebugUI.RangeToString(rangeToTarget), labelStyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label("range2=" + AutoPilotDebugUI.RangeToString(rangeTargetFromPlanet), labelStyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("range1>range2={0}", rangeToTarget > rangeTargetFromPlanet), labelStyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("angle={0}", Vector3.Angle(playerFromPlanet, targetFromPlanet)), labelStyle, Array.Empty<GUILayoutOption>());
			}

			Mecha mecha = GameMain.mainPlayer.mecha;
			GUILayout.Label(string.Format("mecha.coreEnergy={0}", mecha.coreEnergy), labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("mecha.coreEnergyCap={0}", mecha.coreEnergyCap), labelStyle, Array.Empty<GUILayoutOption>());

			double energyPercent = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
			GUILayout.Label(string.Format("energyPer={0}", energyPercent), labelStyle, Array.Empty<GUILayoutOption>());

			double speedMagnitude = GameMain.mainPlayer.controller.actionSail.visual_uvel.magnitude;
			GUILayout.Label("spped=" + AutoPilotDebugUI.RangeToString(speedMagnitude), labelStyle, Array.Empty<GUILayoutOption>());

			EMovementState movementStateInFrame = GameMain.mainPlayer.controller.movementStateInFrame;
			GUILayout.Label(string.Format("movementStateInFrame={0}", movementStateInFrame), labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("safeToGo={0}", AutoPilotPlugin.safeToGo), labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("player.navigation.navigating={0}", GameMain.mainPlayer.navigation.navigating), labelStyle, Array.Empty<GUILayoutOption>());

			GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
			toggleStyle.fixedHeight = 20f;
			toggleStyle.fontSize = 12;
			toggleStyle.alignment = TextAnchor.LowerLeft;

			GUI.changed = false;
			AutoPilotPlugin.Conf.IgnoreGravityFlag = GUILayout.Toggle(AutoPilotPlugin.Conf.IgnoreGravityFlag, "Ignore gravity.", toggleStyle, Array.Empty<GUILayoutOption>());

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
			if (range < 10000.0)
			{
				return ((int)(range + 0.5)).ToString() + "m ";
			}

			if (range < 600000.0)
			{
				return (range / 40000.0).ToString("0.00") + "AU";
			}

			return (range / 2400000.0).ToString("0.00") + "Ly";
		}

		public static bool Show = false;

		public static Rect Rect = new Rect(0f, 0f, 400f, 400f);

		private static float lastCheckWindowLeft = float.MinValue;

		private static float lastCheckWindowTop = float.MinValue;

		private static Vector2 scrollPos = Vector2.zero;
	}
}

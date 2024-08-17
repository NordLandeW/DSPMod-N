using System.Linq;
using UnityEngine;

namespace tanu.CruiseAssist
{
	public class CruiseAssistDebugUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, 400f, 400f);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2 scrollPos = Vector2.zero;

		public static int trackedTinders = 0;

		public static void OnGUI()
		{
			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect = GUILayout.Window(99030294, Rect, WindowFunction, "CruiseAssist - Debug", windowStyle);

			var scale = CruiseAssistMainUI.Scale / 100.0f;

			if (Screen.width < Rect.xMax)
			{
				Rect.x = Screen.width - Rect.width;
			}
			if (Rect.x < 0)
			{
				Rect.x = 0;
			}

			if (Screen.height < Rect.yMax)
			{
				Rect.y = Screen.height - Rect.height;
			}
			if (Rect.y < 0)
			{
				Rect.y = 0;
			}

			if (lastCheckWindowLeft != float.MinValue)
			{
				if (Rect.x != lastCheckWindowLeft || Rect.y != lastCheckWindowTop)
				{
					nextCheckGameTick = GameMain.gameTick + 300;
				}
			}

			lastCheckWindowLeft = Rect.x;
			lastCheckWindowTop = Rect.y;

			if (nextCheckGameTick <= GameMain.gameTick)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
				nextCheckGameTick = long.MaxValue;
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical();

			var labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.fontSize = 12;

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GUILayout.Label($"trackedTinders={trackedTinders}", labelStyle);
			GUILayout.Label($"CruiseAssist.ReticuleTargetStar.id={CruiseAssistPlugin.ReticuleTargetStar?.id}", labelStyle);
            GUILayout.Label($"CruiseAssist.ReticuleTargetPlanet.id={CruiseAssistPlugin.ReticuleTargetPlanet?.id}", labelStyle);
			GUILayout.Label($"CruiseAssist.SelectTargetStar.id={CruiseAssistPlugin.SelectTargetStar?.id}", labelStyle);
			GUILayout.Label($"CruiseAssist.SelectTargetPlanet.id={CruiseAssistPlugin.SelectTargetPlanet?.id}", labelStyle);
            GUILayout.Label($"CruiseAssist.SelectTargetMsg.protoId={CruiseAssistPlugin.SelectTargetMsg?.protoId}", labelStyle);
            GUILayout.Label($"CruiseAssist.TargetUPos={CruiseAssistPlugin.TargetUPos}", labelStyle);
            GUILayout.Label($"GameMain.mainPlayer.uPosition={GameMain.mainPlayer.uPosition}", labelStyle);
            GUILayout.Label($"GameMain.mainPlayer.navigation.indicatorAstroId={GameMain.mainPlayer.navigation.indicatorAstroId}", labelStyle);
            GUILayout.Label($"GameMain.mainPlayer.navigation.indicatorEnemyId={GameMain.mainPlayer.navigation.indicatorEnemyId}", labelStyle);
            GUILayout.Label($"GameMain.mainPlayer.navigation.indicatorMsgId={GameMain.mainPlayer.navigation.indicatorMsgId}", labelStyle);
            GUILayout.Label($"GameMain.mainPlayer.controller.input0.w={GameMain.mainPlayer.controller.input0.w}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.x={GameMain.mainPlayer.controller.input0.x}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.y={GameMain.mainPlayer.controller.input0.y}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.z={GameMain.mainPlayer.controller.input0.z}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.w={GameMain.mainPlayer.controller.input1.w}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.x={GameMain.mainPlayer.controller.input1.x}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.y={GameMain.mainPlayer.controller.input1.y}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.z={GameMain.mainPlayer.controller.input1.z}", labelStyle);
			GUILayout.Label($"VFInput._sailSpeedUp={VFInput._sailSpeedUp}", labelStyle);
			GUILayout.Label($"CruiseAssist.Enable={CruiseAssistPlugin.Enable}", labelStyle);
			GUILayout.Label($"CruiseAssist.History={CruiseAssistPlugin.History.Count()}", labelStyle);
			GUILayout.Label($"CruiseAssist.History={ListUtils.ToString(CruiseAssistPlugin.History)}", labelStyle);
			GUILayout.Label($"GUI.skin.window.margin.top={GUI.skin.window.margin.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.border.top={GUI.skin.window.border.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.padding.top={GUI.skin.window.padding.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.overflow.top={GUI.skin.window.overflow.top}", labelStyle);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

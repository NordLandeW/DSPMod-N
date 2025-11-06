using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
    internal class AutoPilotConfigUI
    {
        public static GUIStyle LabelStyle = null;
        public static GUIStyle FieldStyle = null;
        public static GUIStyle ToggleStyle = null;

        private const int WindowId = 99031292;

        public static void OnGUI()
        {
            wIdx = CruiseAssistMainUI.wIdx;
            Rect[wIdx] = GUILayout.Window(WindowId, Rect[wIdx], WindowFunction, "AutoPilot - Config", CruiseAssistMainUI.WindowStyle);

            float scale = CruiseAssistMainUI.Scale / 100f;

            float maxX = (float)Screen.width / scale - Rect[wIdx].width;
            if (Rect[wIdx].x > maxX) Rect[wIdx].x = maxX;
            if (Rect[wIdx].x < 0f) Rect[wIdx].x = 0f;

            float maxY = (float)Screen.height / scale - Rect[wIdx].height;
            if (Rect[wIdx].y > maxY) Rect[wIdx].y = maxY;
            if (Rect[wIdx].y < 0f) Rect[wIdx].y = 0f;

            if (lastCheckWindowLeft[wIdx] != float.MinValue)
            {
                if (Rect[wIdx].x != lastCheckWindowLeft[wIdx] || Rect[wIdx].y != lastCheckWindowTop[wIdx])
                {
                    AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
                }
            }

            lastCheckWindowLeft[wIdx] = Rect[wIdx].x;
            lastCheckWindowTop[wIdx] = Rect[wIdx].y;
        }

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (FieldStyle == null)
            {
                FieldStyle = new GUIStyle(CruiseAssistMainUI.BaseTextFieldStyle)
                {
                    fontSize = 12,
                    fixedWidth = 60f,
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleRight
                };
            }

            const float labelWidth = 240f;
            const float unitWidth = 20f;

            GUILayout.BeginHorizontal();
            LabelStyle.fixedWidth = labelWidth;
            GUILayout.Label(Localization.Translate("Min Energy Percent (0-100 default:20)"), LabelStyle);
            GUILayout.FlexibleSpace();
            string in1 = GUILayout.TextField(TempMinEnergyPer, FieldStyle);
            SetValue(ref TempMinEnergyPer, in1, 0, 100, ref AutoPilotPlugin.Conf.MinEnergyPer);
            LabelStyle.fixedWidth = unitWidth;
            GUILayout.Label("%", LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            LabelStyle.fixedWidth = labelWidth;
            GUILayout.Label(Localization.Translate("Max Speed (0-2000 default:2000)"), LabelStyle);
            GUILayout.FlexibleSpace();
            string in2 = GUILayout.TextField(TempMaxSpeed, FieldStyle);
            SetValue(ref TempMaxSpeed, in2, 0, 2000, ref AutoPilotPlugin.Conf.MaxSpeed);
            LabelStyle.fixedWidth = unitWidth;
            GUILayout.Label("m/s", LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            LabelStyle.fixedWidth = labelWidth;
            GUILayout.Label(Localization.Translate("Warp Min Range (0-60 default:2)"), LabelStyle);
            GUILayout.FlexibleSpace();
            string in3 = GUILayout.TextField(TempWarpMinRangeAU, FieldStyle);
            SetValue(ref TempWarpMinRangeAU, in3, 0, 60, ref AutoPilotPlugin.Conf.WarpMinRangeAU);
            LabelStyle.fixedWidth = unitWidth;
            GUILayout.Label("AU", LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            LabelStyle.fixedWidth = labelWidth;
            GUILayout.Label(Localization.Translate("Speed to warp (0-2000 default:400)"), LabelStyle);
            GUILayout.FlexibleSpace();
            string in4 = GUILayout.TextField(TempSpeedToWarp, FieldStyle);
            SetValue(ref TempSpeedToWarp, in4, 0, 2000, ref AutoPilotPlugin.Conf.SpeedToWarp);
            LabelStyle.fixedWidth = unitWidth;
            GUILayout.Label("m/s", LabelStyle);
            GUILayout.EndHorizontal();

            if (ToggleStyle == null)
            {
                ToggleStyle = new GUIStyle(CruiseAssistMainUI.BaseToggleStyle);
                ToggleStyle.fixedHeight = 20f;
                ToggleStyle.fontSize = 12;
                ToggleStyle.alignment = TextAnchor.LowerLeft;
            }

            ToggleOption(ref AutoPilotPlugin.Conf.LocalWarpFlag, "Warp to planet in local system.");
            ToggleOption(ref AutoPilotPlugin.Conf.AutoStartFlag, "Start AutoPilot when set target planet.");
			ToggleOption(ref AutoPilotPlugin.Conf.MainWindowJoinFlag, "Join AutoPilot window to CruiseAssist window.");
			ToggleOption(ref AutoPilotPlugin.Conf.SpeedUpWhenFlying, "Speed up during takeoff.");

            GUILayout.EndVertical();

            bool close = GUI.Button(new Rect(Rect[wIdx].width - 16f, 1f, 16f, 16f), "", CruiseAssistMainUI.CloseButtonStyle);
            if (close)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
                Show[wIdx] = false;
            }
            GUI.DragWindow();
        }

        private static void ToggleOption(ref bool option, string label)
        {
            GUI.changed = false;
            option = GUILayout.Toggle(option, Localization.Translate(label), ToggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
                AutoPilotMainUI.NextCheckGameTick = GameMain.gameTick + 300L;
            }
        }

        private static bool SetValue(ref string temp, string input, int min, int max, ref int value)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                temp = string.Empty;
                return false;
            }

            if (int.TryParse(input, out int num))
            {
                num = Mathf.Clamp(num, min, max);
                value = num;
                temp = value.ToString();
                return true;
            }

            return false;
        }

        private static int wIdx = 0;

        public const float WindowWidth = 400f;
        public const float WindowHeight = 400f;

        public static bool[] Show = new bool[2];

        public static Rect[] Rect = new Rect[]
        {
            new Rect(0f, 0f, WindowWidth, WindowHeight),
            new Rect(0f, 0f, WindowWidth, WindowHeight)
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

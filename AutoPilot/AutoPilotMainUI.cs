using System;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
    internal class AutoPilotMainUI
    {
        // Fields
        public static GUIStyle statusLabelStyle = null;
        public static GUIStyle mainStatusLabelStyle = null;
        public static GUIStyle buttonStyle = null;

        private static int wIdx = 0;

        public const float WindowWidthFull = 398f;
        public const float WindowHeightFull = 150f;
        public const float WindowWidthMini = 288f;
        public const float WindowHeightMini = 70f;

        public static Rect[] Rect =
        {
            new Rect(0f, 0f, 398f, 150f),
            new Rect(0f, 0f, 398f, 150f)
        };

        private static readonly float[] lastCheckWindowLeft = { float.MinValue, float.MinValue };
        private static readonly float[] lastCheckWindowTop = { float.MinValue, float.MinValue };

        public static long NextCheckGameTick = long.MaxValue;


        // Methods
        public static void OnGUI()
        {
            wIdx = CruiseAssistMainUI.wIdx;
            CruiseAssistMainUIViewMode viewMode = CruiseAssistMainUI.ViewMode;

            switch (viewMode)
            {
                case CruiseAssistMainUIViewMode.FULL:
                    Rect[wIdx].width = CruiseAssistMainUI.Rect[wIdx].width;
                    Rect[wIdx].height = 150f;
                    break;
                case CruiseAssistMainUIViewMode.MINI:
                    Rect[wIdx].width = CruiseAssistMainUI.Rect[wIdx].width;
                    Rect[wIdx].height = 70f;
                    break;
            }

            GUIStyle windowStyle = CruiseAssistMainUI.WindowStyle;
            windowStyle.fontSize = 11;

            Rect[wIdx] = GUILayout.Window(99031291, Rect[wIdx], WindowFunction, "AutoPilot", windowStyle);

            float scale = CruiseAssistMainUI.Scale / 100f;

            if (AutoPilotPlugin.Conf.MainWindowJoinFlag)
            {
                Rect[wIdx].x = CruiseAssistMainUI.Rect[CruiseAssistMainUI.wIdx].x;
                Rect[wIdx].y = CruiseAssistMainUI.Rect[CruiseAssistMainUI.wIdx].yMax;
            }

            // Keep window on screen
            if (Screen.width / scale < Rect[wIdx].xMax)
            {
                Rect[wIdx].x = Screen.width / scale - Rect[wIdx].width;
            }

            if (Rect[wIdx].x < 0f)
            {
                Rect[wIdx].x = 0f;
            }

            if (Screen.height / scale < Rect[wIdx].yMax)
            {
                Rect[wIdx].y = Screen.height / scale - Rect[wIdx].height;
            }

            if (Rect[wIdx].y < 0f)
            {
                Rect[wIdx].y = 0f;
            }

            if (lastCheckWindowLeft[wIdx] != float.MinValue)
            {
                if (Rect[wIdx].x != lastCheckWindowLeft[wIdx] || Rect[wIdx].y != lastCheckWindowTop[wIdx])
                {
                    NextCheckGameTick = GameMain.gameTick + 300L;
                }
            }

            lastCheckWindowLeft[wIdx] = Rect[wIdx].x;
            lastCheckWindowTop[wIdx] = Rect[wIdx].y;

            if (NextCheckGameTick <= GameMain.gameTick)
            {
                ConfigManager.CheckConfig(ConfigManager.Step.STATE);
            }
        }

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            if (CruiseAssistMainUI.ViewMode == CruiseAssistMainUIViewMode.FULL)
            {
                GUILayout.BeginHorizontal();

                if (statusLabelStyle == null)
                {
                    statusLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 12
                    };
                }

                // Left Column
                GUILayout.BeginVertical();

                // Energy Status
                string energyText;
                if (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
                {
                    energyText = "---";
                    statusLabelStyle.normal.textColor = Color.white;
                }
                else if (AutoPilotPlugin.Conf.MinEnergyPer < AutoPilotPlugin.EnergyPer)
                {
                    energyText = "OK";
                    statusLabelStyle.normal.textColor = Color.cyan;
                }
                else
                {
                    energyText = "NG";
                    statusLabelStyle.normal.textColor = Color.red;
                }

                GUILayout.Label("Energy : " + energyText, statusLabelStyle);

                // Warper Status
                string warperText;
                if (AutoPilotPlugin.State == AutoPilotState.INACTIVE || CruiseAssistPlugin.TargetStar == null ||
                    GameMain.mainPlayer.warping)
                {
                    warperText = "---";
                }
                else if (!AutoPilotPlugin.Conf.LocalWarpFlag && GameMain.localStar != null &&
                         CruiseAssistPlugin.TargetStar.id == GameMain.localStar.id)
                {
                    warperText = "---";
                }
                else if (CruiseAssistPlugin.TargetRange < AutoPilotPlugin.Conf.WarpMinRangeAU * 40000)
                {
                    warperText = "---";
                }
                else if (AutoPilotPlugin.WarperCount < 1)
                {
                    warperText = "NG";
                }
                else
                {
                    warperText = "OK";
                }

                switch (warperText)
                {
                    case "OK":
                        statusLabelStyle.normal.textColor = Color.cyan;
                        break;
                    case "NG":
                        statusLabelStyle.normal.textColor = Color.red;
                        break;
                    default:
                        statusLabelStyle.normal.textColor = Color.white;
                        break;
                }

                GUILayout.Label("Warper : " + warperText, statusLabelStyle);

                GUILayout.EndVertical();

                // Right Column
                GUILayout.BeginVertical();

                // Leave Planet Status
                string leavePlanetText = (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
                    ? "---"
                    : (AutoPilotPlugin.LeavePlanet ? "ON" : "OFF");
                statusLabelStyle.normal.textColor = (leavePlanetText == "ON") ? Color.cyan : Color.white;
                GUILayout.Label("Leave Planet : " + leavePlanetText, statusLabelStyle);

                // Speed Up Status
                string speedUpText = (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
                    ? "---"
                    : (AutoPilotPlugin.SpeedUp ? "ON" : "OFF");
                statusLabelStyle.normal.textColor = (speedUpText == "ON") ? Color.cyan : Color.white;
                GUILayout.Label("Speed UP : " + speedUpText, statusLabelStyle);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }

            GUILayout.BeginHorizontal();

            if (mainStatusLabelStyle == null)
            {
                mainStatusLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fixedWidth = 160f,
                    fixedHeight = 32f,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
            {
                mainStatusLabelStyle.normal.textColor = Color.white;
                GUILayout.Label("Auto Pilot Inactive.", mainStatusLabelStyle);
            }
            else
            {
                mainStatusLabelStyle.normal.textColor = Color.cyan;
                GUILayout.Label("Auto Pilot Active.", mainStatusLabelStyle);
            }

            GUILayout.FlexibleSpace();

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(CruiseAssistMainUI.BaseButtonStyle)
                {
                    fixedWidth = 50f,
                    fixedHeight = 18f,
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            // Buttons - Left Column
            GUILayout.BeginVertical();
            if (GUILayout.Button("Config", buttonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
                AutoPilotConfigUI.Show[wIdx] = !AutoPilotConfigUI.Show[wIdx];
                if (AutoPilotConfigUI.Show[wIdx])
                {
                    AutoPilotConfigUI.TempMinEnergyPer = AutoPilotPlugin.Conf.MinEnergyPer.ToString();
                    AutoPilotConfigUI.TempMaxSpeed = AutoPilotPlugin.Conf.MaxSpeed.ToString();
                    AutoPilotConfigUI.TempWarpMinRangeAU = AutoPilotPlugin.Conf.WarpMinRangeAU.ToString();
                    AutoPilotConfigUI.TempSpeedToWarp = AutoPilotPlugin.Conf.SpeedToWarp.ToString();
                }
            }

            if (GUILayout.Button("Start", buttonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
                AutoPilotPlugin.State = AutoPilotState.ACTIVE;
                CruiseAssistPlugin.ClearSelectedTarget();
            }

            GUILayout.EndVertical();

            // Buttons - Right Column
            GUILayout.BeginVertical();
            GUILayout.Button("-", buttonStyle); // This button has no action
            if (GUILayout.Button("Stop", buttonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0, -1, -1L);
                AutoPilotPlugin.State = AutoPilotState.INACTIVE;
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}

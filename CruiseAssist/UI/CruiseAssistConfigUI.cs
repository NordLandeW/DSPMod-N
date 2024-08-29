using UnityEngine;

namespace tanu.CruiseAssist
{
    public class CruiseAssistConfigUI
    {
        private static int wIdx = 0;

        public const float WindowWidth = 400f;
        public const float WindowHeight = 400f;

        public static bool[] Show = { false, false };
        public static Rect[] Rect = {
            new Rect(0f, 0f, WindowWidth, WindowHeight),
            new Rect(0f, 0f, WindowWidth, WindowHeight) };

        private static float lastCheckWindowLeft = float.MinValue;
        private static float lastCheckWindowTop = float.MinValue;

        public static float TempScale = 150.0f;

        private static GUIStyle windowStyle;
        private static GUIStyle mainWindowStyleLabelStyle;
        private static GUIStyle mainWindowStyleButtonStyle;
        private static GUIStyle uiScaleTitleLabelStyle;
        private static GUIStyle scaleSliderStyle;
        private static GUIStyle scaleSliderThumbStyle;
        private static GUIStyle uiScaleValueLabelStyle;
        private static GUIStyle scaleSetButtonStyle;
        private static GUIStyle toggleStyle;
        private static GUIStyle closeButtonStyle;

        public static void OnGUI()
        {
            CruiseAssistMainUI.StyleStuff();
            wIdx = CruiseAssistMainUI.wIdx;

            windowStyle = windowStyle ?? new GUIStyle(CruiseAssistMainUI.WindowStyle) { fontSize = 11 };

            Rect[wIdx] = GUILayout.Window(99030293, Rect[wIdx], WindowFunction, "CruiseAssist - Config", windowStyle);

            var scale = CruiseAssistMainUI.Scale / 100.0f;

            if (Screen.width / scale < Rect[wIdx].xMax)
            {
                Rect[wIdx].x = Screen.width / scale - Rect[wIdx].width;
            }
            if (Rect[wIdx].x < 0)
            {
                Rect[wIdx].x = 0;
            }

            if (Screen.height / scale < Rect[wIdx].yMax)
            {
                Rect[wIdx].y = Screen.height / scale - Rect[wIdx].height;
            }
            if (Rect[wIdx].y < 0)
            {
                Rect[wIdx].y = 0;
            }

            if (lastCheckWindowLeft != float.MinValue)
            {
                if (Rect[wIdx].x != lastCheckWindowLeft || Rect[wIdx].y != lastCheckWindowTop)
                {
                    CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
                }
            }

            lastCheckWindowLeft = Rect[wIdx].x;
            lastCheckWindowTop = Rect[wIdx].y;
        }

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            mainWindowStyleLabelStyle = mainWindowStyleLabelStyle ?? new GUIStyle(GUI.skin.label)
            {
                fixedWidth = 120,
                fixedHeight = 20,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft
            };

            GUILayout.Label("Main Window Style :", mainWindowStyleLabelStyle);

            mainWindowStyleButtonStyle = mainWindowStyleButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseToolbarButtonStyle)
            {
                fixedWidth = 80,
                fixedHeight = 20,
                fontSize = 12
            };

            string[] texts = { "FULL", "MINI" };
            int listSelected = CruiseAssistMainUI.ViewMode == CruiseAssistMainUIViewMode.FULL ? 0 : 1;
            GUI.changed = false;
            var selected = GUILayout.Toolbar(listSelected, texts, mainWindowStyleButtonStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
            }
            if (selected != listSelected)
            {
                switch (selected)
                {
                    case 0:
                        CruiseAssistMainUI.ViewMode = CruiseAssistMainUIViewMode.FULL;
                        break;
                    case 1:
                        CruiseAssistMainUI.ViewMode = CruiseAssistMainUIViewMode.MINI;
                        break;
                }
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            uiScaleTitleLabelStyle = uiScaleTitleLabelStyle ?? new GUIStyle(GUI.skin.label)
            {
                fixedWidth = 60,
                fixedHeight = 20,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft
            };

            GUILayout.Label("UI Scale :", uiScaleTitleLabelStyle);

            scaleSliderStyle = scaleSliderStyle ?? new GUIStyle(CruiseAssistMainUI.BaseHorizontalSliderStyle)
            {
                fixedWidth = 180,
                margin = { top = 10 },
                alignment = TextAnchor.MiddleLeft
            };
            scaleSliderThumbStyle = scaleSliderThumbStyle ?? new GUIStyle(CruiseAssistMainUI.BaseHorizontalSliderThumbStyle);

            TempScale = GUILayout.HorizontalSlider(TempScale, 80.0f, 240.0f, scaleSliderStyle, scaleSliderThumbStyle);

            TempScale = (int)TempScale / 5 * 5;

            uiScaleValueLabelStyle = uiScaleValueLabelStyle ?? new GUIStyle(GUI.skin.label)
            {
                fixedWidth = 40,
                fixedHeight = 20,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft
            };

            GUILayout.Label(TempScale.ToString("0") + "%", uiScaleValueLabelStyle);

            scaleSetButtonStyle = scaleSetButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle)
            {
                fixedWidth = 60,
                fixedHeight = 18,
                margin = { top = 6 },
                fontSize = 12
            };

            if (GUILayout.Button("SET", scaleSetButtonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.Scale = TempScale;
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUILayout.EndHorizontal();

            toggleStyle = toggleStyle ?? new GUIStyle(CruiseAssistMainUI.BaseToggleStyle)
            {
                fixedHeight = 20,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft
            };

            GUI.changed = false;
            CruiseAssistPlugin.MarkVisitedFlag = GUILayout.Toggle(CruiseAssistPlugin.MarkVisitedFlag, "Mark the visited system and planet.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUI.changed = false;
            CruiseAssistPlugin.SelectFocusFlag = GUILayout.Toggle(CruiseAssistPlugin.SelectFocusFlag, "Focus when target selected.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUI.changed = false;
            CruiseAssistPlugin.HideDuplicateHistoryFlag = GUILayout.Toggle(CruiseAssistPlugin.HideDuplicateHistoryFlag, "Hide duplicate history.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUI.changed = false;
            CruiseAssistPlugin.AutoDisableLockCursorFlag = GUILayout.Toggle(CruiseAssistPlugin.AutoDisableLockCursorFlag, "Disable lock cursor when starting sail mode.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUI.changed = false;
            CruiseAssistPlugin.TrackDarkFogSeedsFlag = GUILayout.Toggle(CruiseAssistPlugin.TrackDarkFogSeedsFlag, "Display all active Dark Fog seeds in starlist.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }


            GUI.changed = false;
            CruiseAssistPlugin.PreventNonTargetLoadingFlag = GUILayout.Toggle(CruiseAssistPlugin.PreventNonTargetLoadingFlag, "Prevent loading of non-target stars when CruiseAssist is active.", toggleStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            closeButtonStyle = closeButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle)
            {
                fixedWidth = 80,
                fixedHeight = 20,
                fontSize = 12
            };

            if (GUILayout.Button("Close", closeButtonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                Show[wIdx] = false;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}

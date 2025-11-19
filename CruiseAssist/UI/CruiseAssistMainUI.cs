using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tanu.CruiseAssist
{
    public class CruiseAssistMainUI
    {
        public static float Scale = 150.0f;

        public static int wIdx = 0;

        public static CruiseAssistMainUIViewMode ViewMode = CruiseAssistMainUIViewMode.FULL;

        public const float WindowWidthFull = 450f;
        public const float WindowHeightFull = 150f;
        public const float WindowWidthMini = 300f;
        public const float WindowHeightMini = 70f;

        public static Rect[] Rect = {
            new Rect(0f, 0f, WindowWidthFull, WindowHeightFull),
            new Rect(0f, 0f, WindowWidthFull, WindowHeightFull) };

        private static float lastCheckWindowLeft = float.MinValue;
        private static float lastCheckWindowTop = float.MinValue;

        public static long NextCheckGameTick = long.MaxValue;
        public static GUIStyle WindowStyle = null;
        public static GUIStyle BaseButtonStyle = null;
        public static GUIStyle BaseToolbarButtonStyle = null;
        public static GUIStyle BaseVerticalScrollBarStyle = null;
        public static GUIStyle BaseHorizontalSliderStyle = null;
        public static GUIStyle BaseHorizontalSliderThumbStyle = null;
        public static GUIStyle BaseToggleStyle = null;
        public static GUIStyle BaseTextFieldStyle = null;
        public static GUIStyle CloseButtonStyle = null;
        private static List<GUIStyle> verticalScrollBarSkins = null;
        public static Texture2D WhiteBorderBackgroundTexture = null;
        public static Texture2D GrayBorderBackgroundTexture = null;
        public static Texture2D WhiteBorderTexture = null;
        public static Texture2D GrayBorderTexture = null;
        public static Texture2D BlackTexture = null;
        public static Texture2D WhiteTexture = null;
        public static Texture2D ToggleOnTexture = null;
        public static Texture2D ToggleOffTexture = null;
        public static Texture2D CloseButtonGrayBorderTexture = null;
        public static Texture2D CloseButtonWhiteBorderTexture = null;

        public static void StyleStuff()
        {
            EnsureBackgroundTextures();
            EnsureBorderTextures();
            EnsureSolidColorTextures();
            EnsureToggleTextures();
            EnsureCloseButtonTextures();

            EnsureBaseButtonAndToolbarStyles();
            EnsureScrollbarAndSliderStyles();
            EnsureToggleAndTextFieldStyles();
            EnsureVerticalScrollbarCustomSkins();
            EnsureWindowStyle();
        }

        #region Texture initialization helpers

        private static void EnsureBackgroundTextures()
        {
            const int size = 64;

            if (WhiteBorderBackgroundTexture == null)
            {
                Color32 borderColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 innerColor = new Color32(0, 0, 0, 224);
                WhiteBorderBackgroundTexture = CreateBorderTexture(size, borderColor, innerColor);
            }

            if (GrayBorderBackgroundTexture == null)
            {
                Color32 borderColor = new Color32(64, 64, 64, byte.MaxValue);
                Color32 innerColor = new Color32(0, 0, 0, 224);
                GrayBorderBackgroundTexture = CreateBorderTexture(size, borderColor, innerColor);
            }
        }

        private static void EnsureBorderTextures()
        {
            const int size = 64;

            if (WhiteBorderTexture == null)
            {
                Color32 borderColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 innerColor = new Color32(0, 0, 0, byte.MaxValue);
                WhiteBorderTexture = CreateBorderTexture(size, borderColor, innerColor);
            }

            if (GrayBorderTexture == null)
            {
                Color32 borderColor = new Color32(64, 64, 64, byte.MaxValue);
                Color32 innerColor = new Color32(0, 0, 0, byte.MaxValue);
                GrayBorderTexture = CreateBorderTexture(size, borderColor, innerColor);
            }
        }

        private static void EnsureSolidColorTextures()
        {
            const int size = 64;

            if (BlackTexture == null)
            {
                Color32 black = new Color32(0, 0, 0, byte.MaxValue);
                BlackTexture = CreateSolidColorTexture(size, black);
            }

            if (WhiteTexture == null)
            {
                Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                WhiteTexture = CreateSolidColorTexture(size, white);
            }
        }

        private static void EnsureToggleTextures()
        {
            if (ToggleOnTexture == null)
            {
                ToggleOnTexture = CreateToggleOnTexture();
            }

            if (ToggleOffTexture == null)
            {
                ToggleOffTexture = CreateToggleOffTexture();
            }
        }

        private static void EnsureCloseButtonTextures()
        {
            if (CloseButtonGrayBorderTexture == null)
            {
                Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 grayBorder = new Color32(64, 64, 64, byte.MaxValue);
                CloseButtonGrayBorderTexture = CreateCloseButtonTexture(grayBorder, white);
            }

            if (CloseButtonWhiteBorderTexture == null)
            {
                Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                CloseButtonWhiteBorderTexture = CreateCloseButtonTexture(white, white);
            }
        }

        private static Texture2D CreateBorderTexture(int size, Color32 borderColor, Color32 innerColor)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    texture.SetPixel(x, y, isBorder ? borderColor : innerColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateSolidColorTexture(int size, Color32 color)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateToggleOnTexture()
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool inside = x >= 1 && x <= 12 && y >= 2 && y <= 13;
                    texture.SetPixel(x, y, inside ? white : transparent);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateToggleOffTexture()
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            Color32 black = new Color32(0, 0, 0, byte.MaxValue);
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool insideBounds = x >= 1 && x <= 12 && y >= 2 && y <= 13;
                    if (!insideBounds)
                    {
                        texture.SetPixel(x, y, transparent);
                    }
                    else
                    {
                        bool innerArea = x > 1 && x < 12 && y > 2 && y < 13;
                        texture.SetPixel(x, y, innerArea ? black : white);
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateCloseButtonTexture(Color32 borderColor, Color32 crossColor)
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color32 black = new Color32(0, 0, 0, byte.MaxValue);
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool insideBounds = x >= 1 && x <= 12 && y >= 2 && y <= 13;
                    if (!insideBounds)
                    {
                        texture.SetPixel(x, y, transparent);
                    }
                    else
                    {
                        bool innerArea = x > 1 && x < 12 && y > 2 && y < 13;
                        texture.SetPixel(x, y, innerArea ? black : borderColor);
                    }
                }
            }

            for (int i = 4; i <= 9; i++)
            {
                texture.SetPixel(i, i + 1, crossColor);
                texture.SetPixel(i, 14 - i, crossColor);
            }

            texture.Apply();
            return texture;
        }

        #endregion

        #region GUIStyle initialization helpers

        private static void EnsureBaseButtonAndToolbarStyles()
        {
            if (BaseButtonStyle == null)
            {
                BaseButtonStyle = new GUIStyle(GUI.skin.button);

                BaseButtonStyle.normal.textColor = Color.white;
                BaseButtonStyle.hover.textColor = Color.white;
                BaseButtonStyle.active.textColor = Color.white;
                BaseButtonStyle.focused.textColor = Color.white;
                BaseButtonStyle.onNormal.textColor = Color.white;
                BaseButtonStyle.onHover.textColor = Color.white;
                BaseButtonStyle.onActive.textColor = Color.white;
                BaseButtonStyle.onFocused.textColor = Color.white;

                BaseButtonStyle.normal.background = GrayBorderTexture;
                BaseButtonStyle.hover.background = WhiteBorderTexture;
                BaseButtonStyle.active.background = WhiteBorderTexture;
                BaseButtonStyle.focused.background = WhiteBorderTexture;
                BaseButtonStyle.onNormal.background = GrayBorderTexture;
                BaseButtonStyle.onHover.background = WhiteBorderTexture;
                BaseButtonStyle.onActive.background = WhiteBorderTexture;
                BaseButtonStyle.onFocused.background = WhiteBorderTexture;
            }

            if (BaseToolbarButtonStyle == null)
            {
                BaseToolbarButtonStyle = new GUIStyle(BaseButtonStyle);

                BaseToolbarButtonStyle.normal.textColor = Color.gray;
                BaseToolbarButtonStyle.hover.textColor = Color.gray;
                BaseToolbarButtonStyle.active.textColor = Color.gray;
                BaseToolbarButtonStyle.focused.textColor = Color.gray;

                BaseToolbarButtonStyle.onNormal.background = WhiteBorderBackgroundTexture;
            }
        }

        private static void EnsureScrollbarAndSliderStyles()
        {
            if (BaseVerticalScrollBarStyle == null)
            {
                BaseVerticalScrollBarStyle = new GUIStyle(GUI.skin.verticalScrollbar)
                {
                    name = "cruiseassist.verticalscrollbar"
                };

                BaseVerticalScrollBarStyle.normal.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.hover.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.active.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.focused.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.onNormal.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.onHover.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.onActive.background = GrayBorderTexture;
                BaseVerticalScrollBarStyle.onFocused.background = GrayBorderTexture;
            }

            if (BaseHorizontalSliderStyle == null)
            {
                BaseHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
                BaseHorizontalSliderStyle.normal.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.hover.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.active.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.focused.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.onNormal.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.onHover.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.onActive.background = GrayBorderTexture;
                BaseHorizontalSliderStyle.onFocused.background = GrayBorderTexture;
            }

            if (BaseHorizontalSliderThumbStyle == null)
            {
                BaseHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
                BaseHorizontalSliderThumbStyle.normal.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.hover.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.active.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.focused.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.onNormal.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.onHover.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.onActive.background = WhiteBorderTexture;
                BaseHorizontalSliderThumbStyle.onFocused.background = WhiteBorderTexture;
            }
        }

        private static void EnsureToggleAndTextFieldStyles()
        {
            if (BaseToggleStyle == null)
            {
                BaseToggleStyle = new GUIStyle(GUI.skin.toggle);
                BaseToggleStyle.normal.background = ToggleOffTexture;
                BaseToggleStyle.hover.background = ToggleOffTexture;
                BaseToggleStyle.active.background = ToggleOffTexture;
                BaseToggleStyle.focused.background = ToggleOffTexture;
                BaseToggleStyle.onNormal.background = ToggleOnTexture;
                BaseToggleStyle.onHover.background = ToggleOnTexture;
                BaseToggleStyle.onActive.background = ToggleOnTexture;
                BaseToggleStyle.onFocused.background = ToggleOnTexture;
            }

            if (BaseTextFieldStyle == null)
            {
                BaseTextFieldStyle = new GUIStyle(GUI.skin.textField);
                BaseTextFieldStyle.normal.background = WhiteBorderTexture;
                BaseTextFieldStyle.hover.background = WhiteBorderTexture;
                BaseTextFieldStyle.active.background = WhiteBorderTexture;
                BaseTextFieldStyle.focused.background = WhiteBorderTexture;
                BaseTextFieldStyle.onNormal.background = WhiteBorderTexture;
                BaseTextFieldStyle.onHover.background = WhiteBorderTexture;
                BaseTextFieldStyle.onActive.background = WhiteBorderTexture;
                BaseTextFieldStyle.onFocused.background = WhiteBorderTexture;
            }
        }

        private static void EnsureCloseButtonStyle()
        {
            if (CloseButtonStyle == null)
            {
                CloseButtonStyle = new GUIStyle(GUI.skin.button);
                CloseButtonStyle.normal.background = CloseButtonGrayBorderTexture;
                CloseButtonStyle.hover.background = CloseButtonWhiteBorderTexture;
                CloseButtonStyle.active.background = CloseButtonWhiteBorderTexture;
                CloseButtonStyle.focused.background = CloseButtonWhiteBorderTexture;
                CloseButtonStyle.onNormal.background = CloseButtonGrayBorderTexture;
                CloseButtonStyle.onHover.background = CloseButtonWhiteBorderTexture;
                CloseButtonStyle.onActive.background = CloseButtonWhiteBorderTexture;
                CloseButtonStyle.onFocused.background = CloseButtonWhiteBorderTexture;
            }
        }

        private static void EnsureVerticalScrollbarCustomSkins()
        {
            if (verticalScrollBarSkins != null)
            {
                return;
            }

            verticalScrollBarSkins = new List<GUIStyle>();

            GUIStyle thumb = new GUIStyle(GUI.skin.verticalScrollbarThumb)
            {
                name = "cruiseassist.verticalscrollbarthumb"
            };
            thumb.normal.background = WhiteBorderTexture;
            thumb.hover.background = WhiteBorderTexture;
            thumb.active.background = WhiteBorderTexture;
            thumb.focused.background = WhiteBorderTexture;
            thumb.onNormal.background = WhiteBorderTexture;
            thumb.onHover.background = WhiteBorderTexture;
            thumb.onActive.background = WhiteBorderTexture;
            thumb.onFocused.background = WhiteBorderTexture;
            verticalScrollBarSkins.Add(thumb);

            GUIStyle upButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton)
            {
                name = "cruiseassist.verticalscrollbarupbutton"
            };
            upButton.normal.background = BlackTexture;
            upButton.hover.background = BlackTexture;
            upButton.active.background = BlackTexture;
            upButton.focused.background = BlackTexture;
            upButton.onNormal.background = BlackTexture;
            upButton.onHover.background = BlackTexture;
            upButton.onActive.background = BlackTexture;
            upButton.onFocused.background = BlackTexture;
            verticalScrollBarSkins.Add(upButton);

            GUIStyle downButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton)
            {
                name = "cruiseassist.verticalscrollbardownbutton"
            };
            downButton.normal.background = BlackTexture;
            downButton.hover.background = BlackTexture;
            downButton.active.background = BlackTexture;
            downButton.focused.background = BlackTexture;
            downButton.onNormal.background = BlackTexture;
            downButton.onHover.background = BlackTexture;
            downButton.onActive.background = BlackTexture;
            downButton.onFocused.background = BlackTexture;
            verticalScrollBarSkins.Add(downButton);

            GUI.skin.customStyles = GUI.skin.customStyles.Concat(verticalScrollBarSkins).ToArray();
        }

        private static void EnsureWindowStyle()
        {
            if (WindowStyle != null)
            {
                return;
            }

            WindowStyle = new GUIStyle(GUI.skin.window)
            {
                fontSize = 11
            };

            WindowStyle.normal.textColor = Color.white;
            WindowStyle.hover.textColor = Color.white;
            WindowStyle.active.textColor = Color.white;
            WindowStyle.focused.textColor = Color.white;
            WindowStyle.onNormal.textColor = Color.white;
            WindowStyle.onHover.textColor = Color.white;
            WindowStyle.onActive.textColor = Color.white;
            WindowStyle.onFocused.textColor = Color.white;

            WindowStyle.normal.background = GrayBorderBackgroundTexture;
            WindowStyle.hover.background = GrayBorderBackgroundTexture;
            WindowStyle.active.background = GrayBorderBackgroundTexture;
            WindowStyle.focused.background = GrayBorderBackgroundTexture;
            WindowStyle.onNormal.background = WhiteBorderBackgroundTexture;
            WindowStyle.onHover.background = WhiteBorderBackgroundTexture;
            WindowStyle.onActive.background = WhiteBorderBackgroundTexture;
            WindowStyle.onFocused.background = WhiteBorderBackgroundTexture;

            EnsureCloseButtonStyle();
        }

        #endregion

        public static void OnGUI()
        {
            StyleStuff();
            switch (ViewMode)
            {
                case CruiseAssistMainUIViewMode.FULL:
                    Rect[wIdx].width = WindowWidthFull;
                    Rect[wIdx].height = WindowHeightFull;
                    break;
                case CruiseAssistMainUIViewMode.MINI:
                    Rect[wIdx].width = WindowWidthMini;
                    Rect[wIdx].height = WindowHeightMini;
                    break;
            }

            Rect[wIdx] = GUILayout.Window(99030291, Rect[wIdx], WindowFunction, "CruiseAssist", WindowStyle);

            //LogManager.LogInfo($"Rect[wIdx].width={Rect[wIdx].width}, Rect[wIdx].height={Rect[wIdx].height}");

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
                    NextCheckGameTick = GameMain.gameTick + 300;
                }
            }

            lastCheckWindowLeft = Rect[wIdx].x;
            lastCheckWindowTop = Rect[wIdx].y;

            if (NextCheckGameTick <= GameMain.gameTick)
            {
                ConfigManager.CheckConfig(ConfigManager.Step.STATE);
                NextCheckGameTick = long.MaxValue;
            }
        }

        public static GUIStyle targetSystemTitleLabelStyle = null;
        public static GUIStyle targetPlanetTitleLabelStyle = null;
        public static GUIStyle targetSystemNameLabelStyle = null;
        public static GUIStyle targetPlanetNameLabelStyle = null;
        public static GUIStyle targetSystemRangeTimeLabelStyle = null;
        public static GUIStyle targetPlanetRangeTimeLabelStyle = null;
        public static GUIStyle cruiseAssistAciviteLabelStyle = null;
        public static GUIStyle buttonStyle = null;

        public static Color systemTextColor;
        public static Color planetTextColor;
        public static Color hiveTextColor;
        public static Color msgTextColor;
        public static Color enemyTextColor;

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            systemTextColor = CruiseAssistPlugin.State == CruiseAssistState.TO_STAR ? Color.cyan : Color.white;
            planetTextColor = CruiseAssistPlugin.State == CruiseAssistState.TO_PLANET ? Color.cyan : Color.white;
            hiveTextColor = CruiseAssistPlugin.State == CruiseAssistState.TO_HIVE ? new Color(173f / 255f, 73f / 255f, 225f / 255f) : Color.white;
            msgTextColor = CruiseAssistPlugin.State == CruiseAssistState.TO_MSG ? Color.cyan : Color.white;
            enemyTextColor = CruiseAssistPlugin.State == CruiseAssistState.TO_ENEMY ? new Color(255f / 255f, 130f / 255f, 37f / 255f) : Color.white;

            if (ViewMode == CruiseAssistMainUIViewMode.FULL)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                {
                    targetSystemTitleLabelStyle = targetSystemTitleLabelStyle ?? new GUIStyle(GUI.skin.label)
                    {
                        fixedWidth = 75,
                        fixedHeight = 36,
                        fontSize = 12,
                        alignment = TextAnchor.UpperLeft
                    };
                    targetPlanetTitleLabelStyle = targetPlanetTitleLabelStyle ?? new GUIStyle(targetSystemTitleLabelStyle);

                    targetSystemTitleLabelStyle.normal.textColor = systemTextColor;
                    GUILayout.Label("Target\n System:", targetSystemTitleLabelStyle);

                    switch(CruiseAssistPlugin.State)
                    {
                        default:
                        case CruiseAssistState.TO_PLANET:
                            targetPlanetTitleLabelStyle.normal.textColor = planetTextColor;
                            GUILayout.Label("Target\n Planet:", targetPlanetTitleLabelStyle);
                            break;
                        case CruiseAssistState.TO_HIVE:
                            targetPlanetTitleLabelStyle.normal.textColor = hiveTextColor;
                            GUILayout.Label("Target\n Hive:", targetPlanetTitleLabelStyle);
                            break;
                        case CruiseAssistState.TO_ENEMY:
                            targetPlanetTitleLabelStyle.normal.textColor = enemyTextColor;
                            GUILayout.Label("Target\n Enemy:", targetPlanetTitleLabelStyle);
                            break;
                        case CruiseAssistState.TO_MSG:
                            targetPlanetTitleLabelStyle.normal.textColor = msgTextColor;
                            GUILayout.Label("Target\n Message:", targetPlanetTitleLabelStyle);
                            break;
                    }
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    targetSystemNameLabelStyle = targetSystemNameLabelStyle ?? new GUIStyle(GUI.skin.label)
                    {
                        fixedWidth = 240,
                        fixedHeight = 36,
                        fontSize = 14,
                        alignment = TextAnchor.MiddleLeft
                    };
                    targetPlanetNameLabelStyle = targetPlanetNameLabelStyle ?? new GUIStyle(targetSystemNameLabelStyle);

                    if (CruiseAssistPlugin.TargetStar != null)
                    {
                        targetSystemNameLabelStyle.normal.textColor = systemTextColor;
                        GUILayout.Label(CruiseAssistPlugin.GetStarName(CruiseAssistPlugin.TargetStar), targetSystemNameLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" ", targetSystemNameLabelStyle);
                    }

                    if (CruiseAssistPlugin.TargetPlanet != null)
                    {
                        targetPlanetNameLabelStyle.normal.textColor = planetTextColor;
                        GUILayout.Label(CruiseAssistPlugin.GetPlanetName(CruiseAssistPlugin.TargetPlanet), targetPlanetNameLabelStyle);
                    }
                    else if(CruiseAssistPlugin.TargetHive != null)
                    {
                        targetPlanetNameLabelStyle.normal.textColor = hiveTextColor;
                        GUILayout.Label(CruiseAssistPlugin.GetHiveName(CruiseAssistPlugin.TargetHive), targetPlanetNameLabelStyle);
                    }
                    else if (CruiseAssistPlugin.TargetMsg != null)
                    {
                        targetPlanetNameLabelStyle.normal.textColor = msgTextColor;
                        GUILayout.Label(CruiseAssistPlugin.GetMsgName(CruiseAssistPlugin.TargetMsg), targetPlanetNameLabelStyle);
                    }
                    else if(CruiseAssistPlugin.TargetEnemyId != 0)
                    {
                        targetPlanetNameLabelStyle.normal.textColor = enemyTextColor;
                        GUILayout.Label(CruiseAssistPlugin.GetEnemyName(CruiseAssistPlugin.TargetEnemy), targetPlanetNameLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" ", targetPlanetNameLabelStyle);
                    }
                }
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                {
                    targetSystemRangeTimeLabelStyle = targetSystemRangeTimeLabelStyle ?? new GUIStyle(GUI.skin.label)
                    {
                        fixedWidth = 80,
                        fixedHeight = 36,
                        fontSize = 12,
                        alignment = TextAnchor.MiddleRight
                    };
                    targetPlanetRangeTimeLabelStyle = targetPlanetRangeTimeLabelStyle ?? new GUIStyle(targetSystemRangeTimeLabelStyle);

                    double velocity = GameMain.mainPlayer.warping
                        ? (GameMain.mainPlayer.controller.actionSail.visual_uvel + GameMain.mainPlayer.controller.actionSail.currentWarpVelocity).magnitude
                        : GameMain.mainPlayer.controller.actionSail.visual_uvel.magnitude;

                    targetPlanetRangeTimeLabelStyle.normal.textColor = targetPlanetNameLabelStyle.normal.textColor;
                    if (CruiseAssistPlugin.TargetStar != null && velocity > 0.001)
                    {
                        double range = CruiseAssistPlugin.TargetRange;
                        GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), targetSystemRangeTimeLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" \n ", targetSystemRangeTimeLabelStyle);
                    }

                    if ((CruiseAssistPlugin.TargetPlanet != null || CruiseAssistPlugin.TargetEnemyId != 0 || CruiseAssistPlugin.TargetHive != null || CruiseAssistPlugin.TargetMsg != null) && velocity > 0.001)
                    {
                        double range = CruiseAssistPlugin.TargetRange;
                        GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), targetPlanetRangeTimeLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" \n ", targetPlanetRangeTimeLabelStyle);
                    }
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            {
                cruiseAssistAciviteLabelStyle = cruiseAssistAciviteLabelStyle ?? new GUIStyle(GUI.skin.label)
                {
                    fixedWidth = 145,
                    fixedHeight = 32,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft
                };

                if (!CruiseAssistPlugin.Enable)
                {
                    cruiseAssistAciviteLabelStyle.normal.textColor = Color.gray;
                    GUILayout.Label("Cruise Assist Disabled.", cruiseAssistAciviteLabelStyle);
                }
                else if (CruiseAssistPlugin.State == CruiseAssistState.INACTIVE || CruiseAssistPlugin.Interrupt)
                {
                    cruiseAssistAciviteLabelStyle.normal.textColor = Color.white;
                    GUILayout.Label("Cruise Assist Inactive.", cruiseAssistAciviteLabelStyle);
                }
                else
                {
                    cruiseAssistAciviteLabelStyle.normal.textColor = Color.cyan;
                    GUILayout.Label("Cruise Assist Active.", cruiseAssistAciviteLabelStyle);
                }

                GUILayout.FlexibleSpace();

                buttonStyle = buttonStyle ?? new GUIStyle(BaseButtonStyle)
                {
                    fixedWidth = 56,
                    fixedHeight = 18,
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter
                };

                GUILayout.BeginVertical();

                if (GUILayout.Button("Config", buttonStyle))
                {
                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                    CruiseAssistConfigUI.Show[wIdx] ^= true;
                    if (CruiseAssistConfigUI.Show[wIdx])
                    {
                        CruiseAssistConfigUI.TempScale = CruiseAssistMainUI.Scale;
                    }
                }

                if (GUILayout.Button(CruiseAssistPlugin.Enable ? "Enable" : "Disable", buttonStyle))
                {
                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                    CruiseAssistPlugin.Enable ^= true;
                    NextCheckGameTick = GameMain.gameTick + 300;
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                if (GUILayout.Button("StarList", buttonStyle))
                {
                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                    CruiseAssistStarListUI.Show[wIdx] ^= true;
                }

                if (GUILayout.Button("Cancel", buttonStyle))
                {
                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                    CruiseAssistStarListUI.SelectStar(null, null);
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }


        public static string RangeToString(double range)
        {
            if (range < 10000.0)
            {
                return ((int)(range + 0.5)).ToString() + "m ";
            }
            else
                if (range < 600000.0)
            {
                return (range / 40000.0).ToString("0.00") + "AU";
            }
            else
            {
                return (range / 2400000.0).ToString("0.00") + "Ly";
            }
        }

        public static string TimeToString(double time)
        {
            int s = (int)(time + 0.5);
            int m = s / 60;
            int h = m / 60;
            s %= 60;
            m %= 60;
            return string.Format("{0:00} {1:00} {2:00}", h, m, s);
        }
    }
}

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

        public const float WindowWidthFull = 398f;
        public const float WindowHeightFull = 150f;
        public const float WindowWidthMini = 273f;
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

        public static void Style_Stuff()
        {
            if (WhiteBorderBackgroundTexture == null)
            {
                WhiteBorderBackgroundTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color2 = new Color32(0, 0, 0, 224);
                for (int i = 0; i < 64; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        Color32 color3 = ((i <= 0 || j <= 0 || i >= 63 || j >= 63) ? color : color2);
                        WhiteBorderBackgroundTexture.SetPixel(j, i, color3);
                    }
                }
                WhiteBorderBackgroundTexture.Apply();
            }
            if (GrayBorderBackgroundTexture == null)
            {
                GrayBorderBackgroundTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color4 = new Color32(64, 64, 64, byte.MaxValue);
                Color32 color5 = new Color32(0, 0, 0, 224);
                for (int k = 0; k < 64; k++)
                {
                    for (int l = 0; l < 64; l++)
                    {
                        Color32 color6 = ((k <= 0 || l <= 0 || k >= 63 || l >= 63) ? color4 : color5);
                        GrayBorderBackgroundTexture.SetPixel(l, k, color6);
                    }
                }
                GrayBorderBackgroundTexture.Apply();
            }
            if (WhiteBorderTexture == null)
            {
                WhiteBorderTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color7 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color8 = new Color32(0, 0, 0, byte.MaxValue);
                for (int m = 0; m < 64; m++)
                {
                    for (int n = 0; n < 64; n++)
                    {
                        Color32 color9 = ((m <= 0 || n <= 0 || m >= 63 || n >= 63) ? color7 : color8);
                        WhiteBorderTexture.SetPixel(n, m, color9);
                    }
                }
                WhiteBorderTexture.Apply();
            }
            if (GrayBorderTexture == null)
            {
                GrayBorderTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color10 = new Color32(64, 64, 64, byte.MaxValue);
                Color32 color11 = new Color32(0, 0, 0, byte.MaxValue);
                for (int num = 0; num < 64; num++)
                {
                    for (int num2 = 0; num2 < 64; num2++)
                    {
                        Color32 color12 = ((num <= 0 || num2 <= 0 || num >= 63 || num2 >= 63) ? color10 : color11);
                        GrayBorderTexture.SetPixel(num2, num, color12);
                    }
                }
                GrayBorderTexture.Apply();
            }
            if (BlackTexture == null)
            {
                BlackTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color13 = new Color32(0, 0, 0, byte.MaxValue);
                for (int num3 = 0; num3 < 64; num3++)
                {
                    for (int num4 = 0; num4 < 64; num4++)
                    {
                        BlackTexture.SetPixel(num4, num3, color13);
                    }
                }
                BlackTexture.Apply();
            }
            if (WhiteTexture == null)
            {
                WhiteTexture = new Texture2D(64, 64, TextureFormat.RGBA32, mipChain: false);
                Color32 color14 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                for (int num5 = 0; num5 < 64; num5++)
                {
                    for (int num6 = 0; num6 < 64; num6++)
                    {
                        WhiteTexture.SetPixel(num6, num5, color14);
                    }
                }
                WhiteTexture.Apply();
            }
            if (ToggleOnTexture == null)
            {
                ToggleOnTexture = new Texture2D(16, 16, TextureFormat.RGBA32, mipChain: false);
                Color32 color15 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color16 = new Color32(0, 0, 0, 0);
                for (int num7 = 0; num7 < 16; num7++)
                {
                    for (int num8 = 0; num8 < 16; num8++)
                    {
                        Color32 color17 = ((num8 >= 1 && num8 <= 12 && num7 >= 2 && num7 <= 13) ? color15 : color16);
                        ToggleOnTexture.SetPixel(num8, num7, color17);
                    }
                }
                ToggleOnTexture.Apply();
            }
            if (ToggleOffTexture == null)
            {
                ToggleOffTexture = new Texture2D(16, 16, TextureFormat.RGBA32, mipChain: false);
                Color32 color18 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color19 = new Color32(0, 0, 0, byte.MaxValue);
                Color32 color20 = new Color32(0, 0, 0, 0);
                for (int num9 = 0; num9 < 16; num9++)
                {
                    for (int num10 = 0; num10 < 16; num10++)
                    {
                        Color32 color21 = ((num10 < 1 || num10 > 12 || num9 < 2 || num9 > 13) ? color20 : ((num10 > 1 && num10 < 12 && num9 > 2 && num9 < 13) ? color19 : color18));
                        ToggleOffTexture.SetPixel(num10, num9, color21);
                    }
                }
                ToggleOffTexture.Apply();
            }
            if (CloseButtonGrayBorderTexture == null)
            {
                CloseButtonGrayBorderTexture = new Texture2D(16, 16, TextureFormat.RGBA32, mipChain: false);
                Color32 color22 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color23 = new Color32(64, 64, 64, byte.MaxValue);
                Color32 color24 = new Color32(0, 0, 0, byte.MaxValue);
                Color32 color25 = new Color32(0, 0, 0, 0);
                for (int num11 = 0; num11 < 16; num11++)
                {
                    for (int num12 = 0; num12 < 16; num12++)
                    {
                        Color32 color26 = ((num12 < 1 || num12 > 12 || num11 < 2 || num11 > 13) ? color25 : ((num12 > 1 && num12 < 12 && num11 > 2 && num11 < 13) ? color24 : color23));
                        CloseButtonGrayBorderTexture.SetPixel(num12, num11, color26);
                    }
                }
                for (int num13 = 4; num13 <= 9; num13++)
                {
                    CloseButtonGrayBorderTexture.SetPixel(num13, num13 + 1, color22);
                    CloseButtonGrayBorderTexture.SetPixel(num13, 14 - num13, color22);
                }
                CloseButtonGrayBorderTexture.Apply();
            }
            if (CloseButtonWhiteBorderTexture == null)
            {
                CloseButtonWhiteBorderTexture = new Texture2D(16, 16, TextureFormat.RGBA32, mipChain: false);
                Color32 color27 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color28 = new Color32(0, 0, 0, byte.MaxValue);
                Color32 color29 = new Color32(0, 0, 0, 0);
                for (int num14 = 0; num14 < 16; num14++)
                {
                    for (int num15 = 0; num15 < 16; num15++)
                    {
                        Color32 color30 = ((num15 < 1 || num15 > 12 || num14 < 2 || num14 > 13) ? color29 : ((num15 > 1 && num15 < 12 && num14 > 2 && num14 < 13) ? color28 : color27));
                        CloseButtonWhiteBorderTexture.SetPixel(num15, num14, color30);
                    }
                }
                for (int num16 = 4; num16 <= 9; num16++)
                {
                    CloseButtonWhiteBorderTexture.SetPixel(num16, num16 + 1, color27);
                    CloseButtonWhiteBorderTexture.SetPixel(num16, 14 - num16, color27);
                }
                CloseButtonWhiteBorderTexture.Apply();
            }
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
            if (BaseVerticalScrollBarStyle == null)
            {
                BaseVerticalScrollBarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                BaseVerticalScrollBarStyle.name = "cruiseassist.verticalscrollbar";
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
            if (verticalScrollBarSkins == null)
            {
                verticalScrollBarSkins = new List<GUIStyle>();
                GUIStyle gUIStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                gUIStyle.name = "cruiseassist.verticalscrollbarthumb";
                gUIStyle.normal.background = WhiteBorderTexture;
                gUIStyle.hover.background = WhiteBorderTexture;
                gUIStyle.active.background = WhiteBorderTexture;
                gUIStyle.focused.background = WhiteBorderTexture;
                gUIStyle.onNormal.background = WhiteBorderTexture;
                gUIStyle.onHover.background = WhiteBorderTexture;
                gUIStyle.onActive.background = WhiteBorderTexture;
                gUIStyle.onFocused.background = WhiteBorderTexture;
                verticalScrollBarSkins.Add(gUIStyle);
                GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
                gUIStyle2.name = "cruiseassist.verticalscrollbarupbutton";
                gUIStyle2.normal.background = BlackTexture;
                gUIStyle2.hover.background = BlackTexture;
                gUIStyle2.active.background = BlackTexture;
                gUIStyle2.focused.background = BlackTexture;
                gUIStyle2.onNormal.background = BlackTexture;
                gUIStyle2.onHover.background = BlackTexture;
                gUIStyle2.onActive.background = BlackTexture;
                gUIStyle2.onFocused.background = BlackTexture;
                verticalScrollBarSkins.Add(gUIStyle2);
                GUIStyle gUIStyle3 = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
                gUIStyle3.name = "cruiseassist.verticalscrollbardownbutton";
                gUIStyle3.normal.background = BlackTexture;
                gUIStyle3.hover.background = BlackTexture;
                gUIStyle3.active.background = BlackTexture;
                gUIStyle3.focused.background = BlackTexture;
                gUIStyle3.onNormal.background = BlackTexture;
                gUIStyle3.onHover.background = BlackTexture;
                gUIStyle3.onActive.background = BlackTexture;
                gUIStyle3.onFocused.background = BlackTexture;
                verticalScrollBarSkins.Add(gUIStyle3);
                GUI.skin.customStyles = GUI.skin.customStyles.Concat(verticalScrollBarSkins).ToArray();
            }
            if (WindowStyle == null)
            {
                WindowStyle = new GUIStyle(GUI.skin.window);
                WindowStyle.fontSize = 11;
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
            }
        }

        public static void OnGUI()
        {
            Style_Stuff();
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

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            if (ViewMode == CruiseAssistMainUIViewMode.FULL)
            {
                GUILayout.BeginHorizontal();

                Color systemTextColor = CruiseAssist.State == CruiseAssistState.TO_STAR ? Color.cyan : Color.white;
                Color planetTextColor = CruiseAssist.State == CruiseAssistState.TO_PLANET ? Color.cyan : Color.white;

                GUILayout.BeginVertical();
                {
                    targetSystemTitleLabelStyle = targetSystemTitleLabelStyle ?? new GUIStyle(GUI.skin.label)
                    {
                        fixedWidth = 50,
                        fixedHeight = 36,
                        fontSize = 12,
                        alignment = TextAnchor.UpperLeft
                    };
                    targetPlanetTitleLabelStyle = targetPlanetTitleLabelStyle ?? new GUIStyle(targetSystemTitleLabelStyle);

                    targetSystemTitleLabelStyle.normal.textColor = systemTextColor;
                    GUILayout.Label("Target\n System:", targetSystemTitleLabelStyle);

                    targetPlanetTitleLabelStyle.normal.textColor = planetTextColor;
                    GUILayout.Label("Target\n Planet:", targetPlanetTitleLabelStyle);
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

                    if (CruiseAssist.TargetStar != null)
                    {
                        targetSystemNameLabelStyle.normal.textColor = systemTextColor;
                        GUILayout.Label(CruiseAssist.GetStarName(CruiseAssist.TargetStar), targetSystemNameLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" ", targetSystemNameLabelStyle);
                    }

                    if (CruiseAssist.TargetPlanet != null)
                    {
                        targetPlanetNameLabelStyle.normal.textColor = planetTextColor;
                        GUILayout.Label(CruiseAssist.GetPlanetName(CruiseAssist.TargetPlanet), targetPlanetNameLabelStyle);
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

                    if (CruiseAssist.TargetStar != null)
                    {
                        targetSystemRangeTimeLabelStyle.normal.textColor = systemTextColor;
                        double range = (CruiseAssist.TargetStar.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)(CruiseAssist.TargetStar.viewRadius - 120f);
                        GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), targetSystemRangeTimeLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(" \n ", targetSystemRangeTimeLabelStyle);
                    }

                    if (CruiseAssist.TargetPlanet != null)
                    {
                        targetPlanetRangeTimeLabelStyle.normal.textColor = planetTextColor;
                        double range = (CruiseAssist.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssist.TargetPlanet.realRadius;
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

                if (CruiseAssist.State == CruiseAssistState.INACTIVE)
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
                    fixedWidth = 50,
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

                if (GUILayout.Button(CruiseAssist.Enable ? "Enable" : "Disable", buttonStyle))
                {
                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                    CruiseAssist.Enable ^= true;
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

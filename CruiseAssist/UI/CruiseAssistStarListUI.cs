using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tanu.CruiseAssist
{
    public class CruiseAssistStarListUI
    {
        private static int wIdx = 0;

        public const float WindowWidth = 440f;
        public const float WindowHeight = 480f;

        public static bool[] Show = { false, false };
        public static Rect[] Rect = {
            new Rect(0f, 0f, WindowWidth, WindowHeight),
            new Rect(0f, 0f, WindowWidth, WindowHeight) };
        public static int ListSelected = 0;
        public static int[] actionSelected = { 0, 0, 0 };

        private static float lastCheckWindowLeft = float.MinValue;
        private static float lastCheckWindowTop = float.MinValue;

        private static readonly Vector2[] scrollPos = { Vector2.zero, Vector2.zero, Vector2.zero };

        private const string VisitedMark = "● ";
        private const string NonVisitMark = "";
        public static GUIStyle windowStyle = null;
        public static GUIStyle mainWindowStyleButtonStyle = null;
        public static GUIStyle nameLabelStyle = null;
        public static GUIStyle nRangeLabelStyle = null;
        public static GUIStyle hRangeLabelStyle = null;
        public static GUIStyle nActionButtonStyle = null;
        public static GUIStyle hActionButtonStyle = null;
        public static GUIStyle nSortButtonStyle = null;
        public static GUIStyle hSortButtonStyle = null;
        public static GUIStyle nViewButtonStyle = null;
        public static GUIStyle hViewButtonStyle = null;
        public static GUIStyle buttonStyle = null;
        public static GUIContent cheapText = null;

        public static string[][] listButtonModeName =
            {
                // Normal
                new string[] { "Target", "Bookmark" },
                // History
                new string[] { "Target", "Bookmark", "Delete" },
                // Bookmark
                new string[] { "Target", "Sort", "Delete" },
            };

        public static GUIContent GetCheapGUIContent(string text)
        {
            cheapText = cheapText ?? new GUIContent();
            cheapText.text = text;
            return cheapText;
        }

        public static void OnGUI()
        {
            wIdx = CruiseAssistMainUI.wIdx;

            windowStyle = windowStyle ?? new GUIStyle(CruiseAssistMainUI.WindowStyle);
            windowStyle.fontSize = 11;

            Rect[wIdx] = GUILayout.Window(99030292, Rect[wIdx], WindowFunction, "CruiseAssist - StarList", windowStyle);

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

        private static bool CanDisplayViewButton
        {
            get { return UIRoot.instance.uiGame.starmap.active; }
        }

        public static void WindowFunction(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            mainWindowStyleButtonStyle = mainWindowStyleButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
            mainWindowStyleButtonStyle.fixedWidth = 80;
            mainWindowStyleButtonStyle.fixedHeight = 20;
            mainWindowStyleButtonStyle.fontSize = 12;

            string[] texts = { "Normal", "History", "Bookmark" };
            GUI.changed = false;
            var selected = GUILayout.Toolbar(ListSelected, texts, mainWindowStyleButtonStyle);
            if (GUI.changed)
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
            }
            if (selected != ListSelected)
            {
                ListSelected = selected;
                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
            }

            GUILayout.EndHorizontal();

            scrollPos[ListSelected] = GUILayout.BeginScrollView(scrollPos[ListSelected], GUIStyle.none, CruiseAssistMainUI.BaseVerticalScrollBarStyle);

            nameLabelStyle = nameLabelStyle ?? new GUIStyle(GUI.skin.label);
            nameLabelStyle.fixedWidth = 240;
            nameLabelStyle.stretchHeight = true;
            nameLabelStyle.fontSize = 14;
            nameLabelStyle.alignment = TextAnchor.MiddleLeft;

            nRangeLabelStyle = nRangeLabelStyle ?? new GUIStyle(GUI.skin.label);
            nRangeLabelStyle.fixedWidth = 60;
            nRangeLabelStyle.fixedHeight = 20;
            nRangeLabelStyle.fontSize = 14;
            nRangeLabelStyle.alignment = TextAnchor.MiddleRight;

            hRangeLabelStyle = hRangeLabelStyle ?? new GUIStyle(nRangeLabelStyle);
            hRangeLabelStyle.fixedHeight = 40;

            nActionButtonStyle = nActionButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
            nActionButtonStyle.fixedWidth = 40;
            nActionButtonStyle.fixedHeight = 18;
            nActionButtonStyle.margin.top = 6;
            nActionButtonStyle.fontSize = 12;

            hActionButtonStyle = hActionButtonStyle ?? new GUIStyle(nActionButtonStyle);
            hActionButtonStyle.margin.top = 16;

            nSortButtonStyle = nSortButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
            nSortButtonStyle.fixedWidth = 20;
            nSortButtonStyle.fixedHeight = 18;
            nSortButtonStyle.margin.top = 6;
            nSortButtonStyle.fontSize = 12;

            hSortButtonStyle = hSortButtonStyle ?? new GUIStyle(nSortButtonStyle);
            hSortButtonStyle.margin.top = 16;

            nViewButtonStyle = nViewButtonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
            nViewButtonStyle.fixedWidth = 20;
            nViewButtonStyle.fixedHeight = 18;
            nViewButtonStyle.margin.top = 6;
            nViewButtonStyle.fontSize = 12;

            hViewButtonStyle = hViewButtonStyle ?? new GUIStyle(nViewButtonStyle);
            hViewButtonStyle.margin.top = 16;

            var uiGame = UIRoot.instance.uiGame;

            if (ListSelected == 0)
            {
                // Track active seeds.
                List<DFTinderComponent> activeTinders = null;
                CruiseAssistDebugUI.trackedTinders = 0;

                if (CruiseAssistPlugin.TrackDarkFogSeedsFlag)
                {
                    activeTinders = new List<DFTinderComponent>();
                    foreach (var hive in GameMain.spaceSector.dfHivesByAstro)
                    {
                        if (hive == null)
                            continue;

                        if (hive.tinderCount == 0)
                            continue;

                        CruiseAssistDebugUI.trackedTinders += hive.tinders.cursor;
                        for (int i = 1; i < hive.tinders.cursor; i++)
                        {
                            if (hive.tinders.buffer[i].ID > 0)
                            {
                                var tinder = hive.tinders.buffer[i];
                                if (tinder.stage >= -1 && tinder.direction > 0)
                                {
                                    activeTinders.Add(tinder);
                                }
                            }
                        }
                    }
                }

                GameMain.galaxy.stars.Select(star => new Tuple<StarData, double>(star, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).OrderBy(tuple => tuple.v2).Do(tuple =>
                {
                    var star = tuple.v1;
                    var range = tuple.v2;
                    var starName = CruiseAssistPlugin.GetStarName(star);
                    bool viewPlanetFlag = false;

                    // Track seeds.
                    List<Tuple<EnemyData, double>> enemiesWithDistances = new List<Tuple<EnemyData, double>>();

                    if (activeTinders != null)
                    {
                        foreach (var tinder in activeTinders)
                        {
                            var hive = GameMain.spaceSector.GetHiveByAstroId(tinder.targetHiveAstroId);
                            if (hive != null && hive.starData.id == star.id)
                            {
                                var enemy = GameMain.spaceSector.enemyPool[tinder.enemyId];
                                var distance = (enemy.pos - GameMain.mainPlayer.uPosition).magnitude;
                                enemiesWithDistances.Add(new Tuple<EnemyData, double>(enemy, distance));
                            }
                        }

                        enemiesWithDistances.Sort((tuple1, tuple2) => tuple1.v2.CompareTo(tuple2.v2));

                        foreach (var tuple2 in enemiesWithDistances)
                        {
                            GUILayout.BeginHorizontal();

                            var enemy = tuple2.v1;
                            var range2 = tuple2.v2;
                            nameLabelStyle.normal.textColor = Color.red;
                            nRangeLabelStyle.normal.textColor = Color.red;
                            float textHeight;

                            if (CruiseAssistPlugin.TargetEnemyId != 0 && enemy.id == CruiseAssistPlugin.TargetEnemyId)
                            {
                                nameLabelStyle.normal.textColor = CruiseAssistMainUI.enemyTextColor;
                                nRangeLabelStyle.normal.textColor = CruiseAssistMainUI.enemyTextColor;
                            }
                            var text = starName + " ← " + CruiseAssistPlugin.GetEnemyName(enemy);
                            GUILayout.Label(text, nameLabelStyle);
                            textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);

                            GUILayout.FlexibleSpace();

                            GUILayout.Label(CruiseAssistMainUI.RangeToString(range2), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

                            var actionName = actionSelected[ListSelected] == 0 ? "SET" : "-";

                            if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
                            {
                                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                                if (actionSelected[ListSelected] == 0)
                                {
                                    SelectEnemy(enemy.id);
                                }
                            }

                            if (actionSelected[ListSelected] == 0 && GUILayout.Button("-", textHeight < 30 ? nSortButtonStyle : hSortButtonStyle))
                            {
                                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                            }

                            GUILayout.EndHorizontal();
                        }
                    }

                    var starmap = UIRoot.instance.uiGame.starmap;
                    if (GameMain.localStar != null && star.id == GameMain.localStar.id)
                    {
                        viewPlanetFlag = true;
                    }
                    else if (
                        (star.id == CruiseAssistPlugin.SelectTargetStar?.id ||
                        star.id == starmap.focusStar?.star.id ||
                        star.id == starmap.focusPlanet?.planet.star.id ||
                        star.id == starmap.focusHive?.hive.starData.id ||
                        star.id == starmap.viewStar?.id ||
                        star.id == starmap.viewPlanet?.star.id ||
                        star.id == starmap.viewHive?.starData.id) &&
                        GameMain.history.universeObserveLevel >= (range >= 14400000.0 ? 4 : 3)
                    )
                    {
                        viewPlanetFlag = true;
                    }
                    if (viewPlanetFlag)
                    {
                        // List Hives
                        GameMain.spaceSector.dfHivesByAstro
                        .Where(hive => hive != null && hive.starData.id == star.id && hive.hasAnyStructureOrUnit)
                        .Select(hive => new Tuple<EnemyDFHiveSystem, double>(hive, (GameMain.spaceSector.astros[hive.hiveAstroId - 1000000].uPos - GameMain.mainPlayer.uPosition).magnitude))
                        .OrderBy(tuple2 => tuple2.v2)
                        .Do(tuple2 =>
                        {
                            GUILayout.BeginHorizontal();

                            var hive = tuple2.v1;
                            var range2 = tuple2.v2;
                            nameLabelStyle.normal.textColor = Color.white;
                            nRangeLabelStyle.normal.textColor = Color.white;
                            float textHeight;

                            if (CruiseAssistPlugin.SelectTargetHive != null && hive.hiveAstroId == CruiseAssistPlugin.SelectTargetHive.hiveAstroId)
                            {
                                nameLabelStyle.normal.textColor = CruiseAssistMainUI.hiveTextColor;
                                nRangeLabelStyle.normal.textColor = CruiseAssistMainUI.hiveTextColor;
                            }
                            var text = starName + " - " + CruiseAssistPlugin.GetHiveName(hive);
                            GUILayout.Label(text, nameLabelStyle);
                            textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);

                            GUILayout.FlexibleSpace();

                            GUILayout.Label(CruiseAssistMainUI.RangeToString(range2), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

                            var actionName =
                                actionSelected[ListSelected] == 0 ? "SET" : "-";

                            // Set button
                            if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
                            {
                                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                                if (actionSelected[ListSelected] == 0)
                                {
                                    SelectHive(star, hive);
                                }
                            }
                            // View button
                            if (CanDisplayViewButton && actionSelected[ListSelected] == 0 && GUILayout.Button("V", textHeight < 30 ? nViewButtonStyle : hViewButtonStyle))
                            {
                                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                                if (actionSelected[ListSelected] == 0)
                                {
                                    SelectHive(star, hive, false);
                                }
                            }

                            GUILayout.EndHorizontal();
                        });

                        // List Planets & Stars
                        star.planets.
                            Select(planet => new Tuple<PlanetData, double>(planet, (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
                            AddItem(new Tuple<PlanetData, double>(null, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
                            OrderBy(tuple2 => tuple2.v2).
                            Do(tuple2 =>
                            {
                                GUILayout.BeginHorizontal();

                                var planet = tuple2.v1;
                                var range2 = tuple2.v2;
                                nameLabelStyle.normal.textColor = Color.white;
                                nRangeLabelStyle.normal.textColor = Color.white;
                                float textHeight;

                                if (planet == null)
                                {
                                    if (CruiseAssistPlugin.SelectTargetPlanet == null && CruiseAssistPlugin.SelectTargetStar != null && star.id == CruiseAssistPlugin.SelectTargetStar.id)
                                    {
                                        nameLabelStyle.normal.textColor = Color.cyan;
                                        nRangeLabelStyle.normal.textColor = Color.cyan;
                                    }
                                    var text = starName;
                                    if (CruiseAssistPlugin.MarkVisitedFlag)
                                    {
                                        text = (star.planets.Where(p => p.factory != null).Count() > 0 ? VisitedMark : NonVisitMark) + text;
                                    }
                                    GUILayout.Label(text, nameLabelStyle);
                                    textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);
                                }
                                else
                                {
                                    if (CruiseAssistPlugin.SelectTargetPlanet != null && planet.id == CruiseAssistPlugin.SelectTargetPlanet.id)
                                    {
                                        nameLabelStyle.normal.textColor = Color.cyan;
                                        nRangeLabelStyle.normal.textColor = Color.cyan;
                                    }
                                    var text = starName + " - " + CruiseAssistPlugin.GetPlanetName(planet);
                                    if (CruiseAssistPlugin.MarkVisitedFlag)
                                    {
                                        text = (planet.factory != null ? VisitedMark : NonVisitMark) + text;
                                    }
                                    GUILayout.Label(text, nameLabelStyle);
                                    textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);
                                }

                                GUILayout.FlexibleSpace();

                                GUILayout.Label(CruiseAssistMainUI.RangeToString(planet == null ? range : range2), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

                                var actionName =
                                    actionSelected[ListSelected] == 0 ? "SET" :
                                    planet == null ? "-" :
                                    CruiseAssistPlugin.Bookmark.Contains(planet.id) ? "DEL" : "ADD";

                                if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
                                {
                                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                                    if (actionSelected[ListSelected] == 0)
                                    {
                                        SelectStar(star, planet);
                                    }
                                    else if (planet != null)
                                    {
                                        if (CruiseAssistPlugin.Bookmark.Contains(planet.id))
                                        {
                                            CruiseAssistPlugin.Bookmark.Remove(planet.id);
                                        }
                                        else
                                        {
                                            if (CruiseAssistPlugin.Bookmark.Count <= 128)
                                            {
                                                CruiseAssistPlugin.Bookmark.Add(planet.id);
                                                CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
                                            }
                                        }
                                    }
                                }

                                if (CanDisplayViewButton && actionSelected[ListSelected] == 0 && GUILayout.Button("V", textHeight < 30 ? nViewButtonStyle : hViewButtonStyle))
                                {
                                    VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                                    if (actionSelected[ListSelected] == 0)
                                    {
                                        SelectStar(star, planet, false);
                                    }
                                }

                                GUILayout.EndHorizontal();
                            });
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        float textHeight;

                        nameLabelStyle.normal.textColor = Color.white;
                        nRangeLabelStyle.normal.textColor = Color.white;

                        if (CruiseAssistPlugin.SelectTargetStar != null && star.id == CruiseAssistPlugin.SelectTargetStar.id)
                        {
                            nameLabelStyle.normal.textColor = Color.cyan;
                            nRangeLabelStyle.normal.textColor = Color.cyan;
                        }

                        var text = starName;
                        if (CruiseAssistPlugin.MarkVisitedFlag)
                        {
                            text = (star.planets.Where(p => p.factory != null).Count() > 0 ? VisitedMark : NonVisitMark) + text;
                        }
                        GUILayout.Label(text, nameLabelStyle);
                        textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);

                        GUILayout.FlexibleSpace();

                        GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

                        var actionName =
                            actionSelected[ListSelected] == 0 ? "SET" : "-";

                        if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (actionSelected[ListSelected] == 0)
                            {
                                SelectStar(star, null);
                            }
                        }

                        if (CanDisplayViewButton && actionSelected[ListSelected] == 0 && GUILayout.Button("V", textHeight < 30 ? nViewButtonStyle : hViewButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (actionSelected[ListSelected] == 0)
                            {
                                SelectStar(star, null, false);
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                });
            }
            else if (ListSelected == 1 || ListSelected == 2)
            {
                bool highlighted = false;

                var list = ListSelected == 1 ? CruiseAssistPlugin.History.Reverse<int>() : CruiseAssistPlugin.Bookmark.ToList();

                if (ListSelected == 1 && actionSelected[ListSelected] != 2 && CruiseAssistPlugin.HideDuplicateHistoryFlag)
                {
                    list = list.Distinct();
                }

                var listIndex = -1;

                list.Do(id =>
                {
                    ++listIndex;

                    var planet = GameMain.galaxy.PlanetById(id);
                    if (planet == null)
                    {
                        return;
                    }
                    var star = planet.star;
                    var starName = CruiseAssistPlugin.GetStarName(star);
                    var range = (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude;
                    nameLabelStyle.normal.textColor = Color.white;
                    nRangeLabelStyle.normal.textColor = Color.white;
                    float textHeight;

                    if (!highlighted && CruiseAssistPlugin.SelectTargetPlanet != null && planet.id == CruiseAssistPlugin.SelectTargetPlanet.id)
                    {
                        nameLabelStyle.normal.textColor = Color.cyan;
                        nRangeLabelStyle.normal.textColor = Color.cyan;
                        highlighted = true;
                    }

                    GUILayout.BeginHorizontal();

                    var text = starName + " - " + CruiseAssistPlugin.GetPlanetName(planet);
                    if (CruiseAssistPlugin.MarkVisitedFlag)
                    {
                        text = (planet.factory != null ? VisitedMark : NonVisitMark) + text;
                    }
                    GUILayout.Label(text, nameLabelStyle);
                    textHeight = nameLabelStyle.CalcHeight(GetCheapGUIContent(text), nameLabelStyle.fixedWidth);

                    GUILayout.FlexibleSpace();

                    GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

                    if (ListSelected == 2 && actionSelected[ListSelected] == 1)
                    {
                        // BookmarkのSort

                        var index = CruiseAssistPlugin.Bookmark.IndexOf(id);
                        bool first = index == 0;
                        bool last = index == CruiseAssistPlugin.Bookmark.Count - 1;

                        if (GUILayout.Button(last ? "-" : "↓", textHeight < 30 ? nSortButtonStyle : hSortButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (!last)
                            {
                                CruiseAssistPlugin.Bookmark.RemoveAt(index);
                                CruiseAssistPlugin.Bookmark.Insert(index + 1, id);
                            }
                        }
                        if (GUILayout.Button(first ? "-" : "↑", textHeight < 30 ? nSortButtonStyle : hSortButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (!first)
                            {
                                CruiseAssistPlugin.Bookmark.RemoveAt(index);
                                CruiseAssistPlugin.Bookmark.Insert(index - 1, id);
                            }
                        }
                    }
                    else
                    {
                        var actionName =
                            actionSelected[ListSelected] == 0 ? "SET" :
                            actionSelected[ListSelected] == 2 ? (ListSelected == 1 && listIndex == 0 ? "-" : "DEL") :
                            CruiseAssistPlugin.Bookmark.Contains(id) ? "DEL" : "ADD";

                        if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (actionSelected[ListSelected] == 0)
                            {
                                // 0番目(SET)を押したとき、対応する惑星を選択
                                SelectStar(star, planet);
                            }
                            else if (actionSelected[ListSelected] == 1)
                            {
                                // 1番目を押したとき

                                if (ListSelected == 1)
                                {
                                    // History(1番目はADD)のとき

                                    if (CruiseAssistPlugin.Bookmark.Contains(id))
                                    {
                                        CruiseAssistPlugin.Bookmark.Remove(id);
                                    }
                                    else
                                    {
                                        if (CruiseAssistPlugin.Bookmark.Count <= 128)
                                        {
                                            CruiseAssistPlugin.Bookmark.Add(id);
                                            CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
                                        }
                                    }
                                }
                            }
                            else if (actionSelected[ListSelected] == 2)
                            {
                                // 2番目を押したとき

                                if (ListSelected == 1)
                                {
                                    // History(2番目はDEL)のとき

                                    if (listIndex != 0)
                                    {
                                        CruiseAssistPlugin.History.RemoveAt(CruiseAssistPlugin.History.Count - 1 - listIndex);
                                        CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
                                    }
                                }
                                else if (ListSelected == 2)
                                {
                                    // Bookmark(2番目はDEL)のとき

                                    CruiseAssistPlugin.Bookmark.Remove(planet.id);
                                    CruiseAssistMainUI.NextCheckGameTick = GameMain.gameTick + 300;
                                }
                            }
                        }

                        if (CanDisplayViewButton && actionSelected[ListSelected] == 0 && GUILayout.Button("V", textHeight < 30 ? nViewButtonStyle : hViewButtonStyle))
                        {
                            VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

                            if (actionSelected[ListSelected] == 0)
                            {
                                SelectStar(star, planet, false);
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                });
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            buttonStyle = buttonStyle ?? new GUIStyle(CruiseAssistMainUI.BaseButtonStyle);
            buttonStyle.fixedWidth = 80;
            buttonStyle.fixedHeight = 20;
            buttonStyle.fontSize = 12;

            if (GUILayout.Button(listButtonModeName[ListSelected][actionSelected[ListSelected]], buttonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                ++actionSelected[ListSelected];
                actionSelected[ListSelected] %= listButtonModeName[ListSelected].Length;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", buttonStyle))
            {
                VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
                Show[wIdx] = false;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        public static void SelectStar(StarData star, PlanetData planet, bool set = true)
        {
            var uiGame = UIRoot.instance.uiGame;

            if (CruiseAssistPlugin.SelectFocusFlag && uiGame.starmap.active)
            {
                if (star != null)
                {
                    var uiStar = uiGame.starmap.starUIs.Where(s => s.star.id == star.id).FirstOrDefault();
                    if (uiStar != null)
                    {
                        UIStarmap_OnStarClick(uiGame.starmap, uiStar);
                        uiGame.starmap.OnCursorFunction2Click(0);
                    }
                }
                if (planet != null)
                {
                    var uiPlanet = uiGame.starmap.planetUIs.Where(p => p.planet.id == planet.id).FirstOrDefault();
                    if (uiPlanet != null)
                    {
                        UIStarmap_OnPlanetClick(uiGame.starmap, uiPlanet);
                        uiGame.starmap.OnCursorFunction2Click(0);
                    }
                }
            }

            if (!set) return;

            if (planet != null)
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = planet.id;
            }
            else if (star != null)
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = star.id * 100;
            }
            else
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = 0;
            }
        }


        public static void SelectHive(StarData star, EnemyDFHiveSystem hive, bool set = true)
        {
            var uiGame = UIRoot.instance.uiGame;

            if (CruiseAssistPlugin.SelectFocusFlag && uiGame.starmap.active)
            {
                if (star != null)
                {
                    var uiStar = uiGame.starmap.starUIs.Where(s => s.star.id == star.id).FirstOrDefault();
                    if (uiStar != null)
                    {
                        UIStarmap_OnStarClick(uiGame.starmap, uiStar);
                        uiGame.starmap.OnCursorFunction2Click(0);
                    }
                }
                if (hive != null)
                {
                    var uiHive = uiGame.starmap.dfHiveUIs.Where(p => p.hive.hiveAstroId == hive.hiveAstroId).FirstOrDefault();
                    if (uiHive != null)
                    {
                        UIStarmap_OnHiveClick(uiGame.starmap, uiHive);
                        uiGame.starmap.OnCursorFunction2Click(0);
                    }
                }
            }

            if (!set) return;

            if (hive != null)
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = hive.hiveAstroId;
            }
            else if (star != null)
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = star.id * 100;
            }
            else
            {
                GameMain.mainPlayer.navigation.indicatorAstroId = 0;
            }
        }


        public static void SelectEnemy(int enemyId)
        {
            if (enemyId != 0)
            {
                GameMain.mainPlayer.navigation.indicatorEnemyId = enemyId;
            }
        }

        private static void UIStarmap_OnStarClick(UIStarmap starmap, UIStarmapStar star)
        {
            var starmapTraverse = Traverse.Create(starmap);
            if (starmap.focusStar != star)
            {
                starmap.screenCameraController.DisablePositionLock();
                starmap.focusPlanet = null;
                starmap.focusHive = null;
                starmap.focusStar = star;
                starmapTraverse.Field("_lastClickTime").SetValue(0.0);
            }
            starmapTraverse.Field("forceUpdateCursorView").SetValue(true);
        }

        private static void UIStarmap_OnPlanetClick(UIStarmap starmap, UIStarmapPlanet planet)
        {
            var starmapTraverse = Traverse.Create(starmap);
            if (starmap.focusPlanet != planet)
            {
                if ((starmap.viewPlanet != null && planet.planet != starmap.viewPlanet) || starmap.viewStar != null)
                {
                    starmap.screenCameraController.DisablePositionLock();
                }
                starmap.focusPlanet = planet;
                starmap.focusHive = null;
                starmap.focusStar = null;
                starmapTraverse.Field("_lastClickTime").SetValue(0.0);
            }
            starmapTraverse.Field("forceUpdateCursorView").SetValue(true);
        }
        private static void UIStarmap_OnHiveClick(UIStarmap starmap, UIStarmapDFHive hive)
        {
            var starmapTraverse = Traverse.Create(starmap);
            if (starmap.focusHive != hive)
            {
                starmap.screenCameraController.DisablePositionLock();
                starmap.focusHive = hive;
                starmap.focusPlanet = null;
                starmap.focusStar = null;
                starmapTraverse.Field("_lastClickTime").SetValue(0.0);
            }
            starmapTraverse.Field("forceUpdateCursorView").SetValue(true);
        }
    }
}

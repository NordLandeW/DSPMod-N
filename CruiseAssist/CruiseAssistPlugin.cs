using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// https://docs.unity3d.com/ja/2018.4/Manual/ExecutionOrder.html

namespace tanu.CruiseAssist
{
    public interface CruiseAssistExtensionAPI
    {
        void CheckConfig(string step);
        void SetTargetAstroId(int astroId);
        bool OperateWalk(PlayerMove_Walk __instance);
        bool OperateDrift(PlayerMove_Drift __instance);
        bool OperateFly(PlayerMove_Fly __instance);
        bool OperateSail(PlayerMove_Sail __instance);
        void SetInactive();
        void CancelOperate();
        void OnGUI();
		bool CheckActive();
    }

    [BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssistPlugin : BaseUnityPlugin
	{
		public const string ModGuid = "nord.CruiseAssist";
		public const string ModName = "CruiseAssist-N";
		public const string ModVersion = "0.1.0";

		public const double HIVE_IN_RANGE = 30000.0;
        public const double ENEMY_IN_RANGE = 1000.0;

        public static bool Enable = true;
		public static bool Interrupt = false;
		public static bool TargetSelected = false;
        public static bool MarkVisitedFlag = true;
		public static bool SelectFocusFlag = false;
		public static bool HideDuplicateHistoryFlag = true;
		public static bool AutoDisableLockCursorFlag = false;
		public static StarData ReticuleTargetStar = null;
		public static PlanetData ReticuleTargetPlanet = null;
		public static StarData SelectTargetStar = null;
		public static PlanetData SelectTargetPlanet = null;
		public static EnemyDFHiveSystem SelectTargetHive = null;
		public static EnemyData SelectTargetEnemy => GameMain.spaceSector.enemyPool[SelectTargetEnemyId];
        public static int SelectTargetAstroId = 0;
		public static int SelectTargetEnemyId = 0;
        public static int SelectTargetEnemyIdF = 0;
        public static StarData TargetStar = null;
		public static PlanetData TargetPlanet = null;
		public static EnemyDFHiveSystem TargetHive = null;
		public static int TargetEnemyId = 0;
		public static EnemyData TargetEnemy => GameMain.spaceSector.enemyPool[TargetEnemyId];
		public static CruiseAssistState State = CruiseAssistState.INACTIVE;
		public static CruiseAssistState lastState = CruiseAssistState.INACTIVE;

		public static List<int> History = new List<int>();
		public static List<int> Bookmark = new List<int>();

		public static Func<StarData, string> GetStarName = star => star.displayName;
		public static Func<PlanetData, string> GetPlanetName = planet => planet.displayName;
        public static Func<EnemyDFHiveSystem, string> GetHiveName = hive => hive.displayName;
        public static Func<EnemyData, string> GetEnemyName = enemy => LDB.enemies.Select(enemy.protoId).name;

        private Harmony harmony;

        internal static List<CruiseAssistExtensionAPI> extensions = new List<CruiseAssistExtensionAPI>();
		public static VectorLF3 TargetUPos
		{
			get
			{
                if (TargetPlanet != null)
                {
                    return TargetPlanet.uPosition;
                }
                else if (TargetHive != null)
                {
                    return GameMain.spaceSector.astros[TargetHive.hiveAstroId - 1000000].uPos;
                }
                else if (TargetEnemyId != 0)
                {
                    return TargetEnemy.pos;
                }
                else
                {
                    if (TargetStar == null)
                    {
                        return GameMain.mainPlayer.uPosition;
                    }
                    return TargetStar.uPosition;
                }
            }
		}
		public static double TargetRange = .0;

		public static bool CheckActive() => State != CruiseAssistState.INACTIVE;

        public void Awake()
		{
			LogManager.Logger = base.Logger;
			new CruiseAssistConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			harmony = new Harmony($"{ModGuid}.Patch");
            harmony.PatchAll(typeof(Patch_GameMain));
            harmony.PatchAll(typeof(Patch_UISailPanel));
            harmony.PatchAll(typeof(Patch_UIStarmap));
            harmony.PatchAll(typeof(Patch_UITechTree));
            harmony.PatchAll(typeof(Patch_PlayerMoveWalk));
            harmony.PatchAll(typeof(Patch_PlayerMoveDrift));
            harmony.PatchAll(typeof(Patch_PlayerMoveFly));
            harmony.PatchAll(typeof(Patch_PlayerMoveSail));
        }

		public void OnDestroy()
		{
			harmony.UnpatchSelf();
		}

		public void OnGUI()
		{
			if (DSPGame.IsMenuDemo || GameMain.mainPlayer == null)
			{
				return;
			}
			var uiGame = UIRoot.instance.uiGame;
			if (!uiGame.guideComplete || uiGame.techTree.active || uiGame.escMenu.active || uiGame.dysonEditor.active || uiGame.hideAllUI0 || uiGame.hideAllUI1 ||
				(UIMilkyWayLoadingSplash.instance != null && UIMilkyWayLoadingSplash.instance.active) || 
				(UIRoot.instance.uiMilkyWay != null && UIRoot.instance.uiMilkyWay.active))
			{
				return;
			}

			var extensionActive = false;
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
				extensionActive |= extension.CheckActive();
            });
            if (GameMain.mainPlayer.sailing || uiGame.starmap.active || extensionActive)
			{
				CruiseAssistMainUI.wIdx = uiGame.starmap.active ? 1 : 0;

				var scale = CruiseAssistMainUI.Scale / 100.0f;

				GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);

				CruiseAssistMainUI.OnGUI();
				if (CruiseAssistStarListUI.Show[CruiseAssistMainUI.wIdx])
				{
					CruiseAssistStarListUI.OnGUI();
				}
				if (CruiseAssistConfigUI.Show[CruiseAssistMainUI.wIdx])
				{
					CruiseAssistConfigUI.OnGUI();
				}
				if (CruiseAssistDebugUI.Show)
				{
					CruiseAssistDebugUI.OnGUI();
				}

				bool resetInputFlag = false;

				resetInputFlag = ResetInput(CruiseAssistMainUI.Rect[CruiseAssistMainUI.wIdx], scale);

				if (!resetInputFlag && CruiseAssistStarListUI.Show[CruiseAssistMainUI.wIdx])
				{
					resetInputFlag = ResetInput(CruiseAssistStarListUI.Rect[CruiseAssistMainUI.wIdx], scale);
				}

				if (!resetInputFlag && CruiseAssistConfigUI.Show[CruiseAssistMainUI.wIdx])
				{
					resetInputFlag = ResetInput(CruiseAssistConfigUI.Rect[CruiseAssistMainUI.wIdx], scale);
				}

				if (!resetInputFlag && CruiseAssistDebugUI.Show)
				{
					resetInputFlag = ResetInput(CruiseAssistDebugUI.Rect, scale);
				}

                extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
                {
                    extension.OnGUI();
                });
            }
		}

		private bool ResetInput(Rect rect, float scale)
		{
			var left = rect.xMin * scale;
			var right = rect.xMax * scale;
			var top = rect.yMin * scale;
			var bottom = rect.yMax * scale;
			var inputX = Input.mousePosition.x;
			var inputY = Screen.height - Input.mousePosition.y;
			if (left <= inputX && inputX <= right && top <= inputY && inputY <= bottom)
			{
				int[] zot = { 0, 1, 2 };
				if (zot.Any(Input.GetMouseButton) || Input.mouseScrollDelta.y != 0)
				{
					Input.ResetInputAxes();
					return true;
				}
			}
			return false;
		}

        public static void RegistExtension(CruiseAssistExtensionAPI extension)
        {
            extensions.Add(extension);
        }

        public static void UnregistExtension(Type type)
        {
            extensions.RemoveAll((CruiseAssistExtensionAPI extension) => extension.GetType().FullName == type.FullName);
        }

		public static void CheckDeactivate()
		{
            SelectTargetStar = null;
            SelectTargetHive = null;
            SelectTargetPlanet = null;
            SelectTargetAstroId = 0;
            SelectTargetEnemyId = 0;
            GameMain.mainPlayer.navigation.indicatorAstroId = 0;
            GameMain.mainPlayer.navigation.indicatorEnemyId = 0;
            extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.SetInactive();
            });

            var player = GameMain.mainPlayer;

            // If on arrival and not interrupted
            if (lastState != CruiseAssistState.INACTIVE && CruiseAssistPlugin.State == CruiseAssistState.INACTIVE)
            {
                if (player.warping && player.warpCommand)
                {
                    player.warpCommand = false;
                    VFAudio.Create("warp-end", player.transform, Vector3.zero, true);
                }
            }
        }
    }
}

using System.Linq;
using HarmonyLib;
using tanu.CruiseAssist;
using UnityEngine;

[HarmonyPatch(typeof(PlayerMove_Walk))]
internal class Patch_PlayerMoveWalk
{
    [HarmonyPatch("GameTick")]
    [HarmonyPrefix]
    public static void GameTick_Prefix(PlayerMove_Walk __instance)
    {
        CruiseAssistPlugin.lastState = CruiseAssistPlugin.State;

        if(CruiseAssistPlugin.Interrupt)
        {
            CruiseAssistPlugin.State = CruiseAssistState.INACTIVE;
            CruiseAssistPlugin.SelectTargetPlanet = null;
            CruiseAssistPlugin.SelectTargetStar = null;
            CruiseAssistPlugin.SelectTargetHive = null;
            CruiseAssistPlugin.SelectTargetEnemyIdF = 0;
            CruiseAssistPlugin.lastState = CruiseAssistState.INACTIVE;
        }

        CruiseAssistPlugin.State = CruiseAssistState.INACTIVE;
        CruiseAssistPlugin.Interrupt = false;
        CruiseAssistPlugin.TargetStar = null;
        CruiseAssistPlugin.TargetPlanet = null;
        CruiseAssistPlugin.TargetHive = null;
        CruiseAssistPlugin.TargetEnemyId = 0;
        CruiseAssistPlugin.TargetRange = 0.0;
        CruiseAssistPlugin.TargetSelected = false;
        if (GameMain.localPlanet != null && (CruiseAssistPlugin.History.Count == 0 || CruiseAssistPlugin.History.Last() != GameMain.localPlanet.id))
        {
            if (CruiseAssistPlugin.History.Count >= 128)
            {
                CruiseAssistPlugin.History.RemoveAt(0);
            }
            CruiseAssistPlugin.History.Add(GameMain.localPlanet.id);
            ConfigManager.CheckConfig(ConfigManager.Step.STATE);
        }
        if (!CruiseAssistPlugin.Enable)
        {
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.SetInactive();
            });
            return;
        }
        int indicatorAstroId = GameMain.mainPlayer.navigation.indicatorAstroId;
        int indicatorEnemyId = GameMain.mainPlayer.navigation.indicatorEnemyId;
        if (UIRoot.instance.uiGame.starmap.focusHive != null)
        {
            return;
        }
        if (CruiseAssistPlugin.SelectTargetAstroId != indicatorAstroId)
        {
            CruiseAssistPlugin.SelectTargetAstroId = indicatorAstroId;
            if(indicatorAstroId == 0)
            {
                CruiseAssistPlugin.SelectTargetPlanet = null;
                CruiseAssistPlugin.SelectTargetStar = null;
            }
            else
            {
                if(indicatorAstroId > 1000000)
                {
                    CruiseAssistPlugin.SelectTargetHive = GameMain.spaceSector.dfHivesByAstro[indicatorAstroId - 1000000];
                    CruiseAssistPlugin.SelectTargetPlanet = null;
                    CruiseAssistPlugin.SelectTargetStar = CruiseAssistPlugin.SelectTargetHive.starData;
                }
                else if (indicatorAstroId % 100 != 0)
                {
                    CruiseAssistPlugin.SelectTargetPlanet = GameMain.galaxy.PlanetById(indicatorAstroId);
                    CruiseAssistPlugin.SelectTargetStar = CruiseAssistPlugin.SelectTargetPlanet.star;
                }
                else
                {
                    CruiseAssistPlugin.SelectTargetPlanet = null;
                    CruiseAssistPlugin.SelectTargetStar = GameMain.galaxy.StarById(indicatorAstroId / 100);
                }
                CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
                {
                    extension.SetTargetAstroId(indicatorAstroId);
                });
            }
        }
        if(CruiseAssistPlugin.SelectTargetEnemyId != indicatorEnemyId)
        {
            CruiseAssistPlugin.SelectTargetEnemyId = indicatorEnemyId;
            CruiseAssistPlugin.SelectTargetEnemyIdF = indicatorEnemyId;
            CruiseAssistPlugin.SelectTargetPlanet = null;
            CruiseAssistPlugin.SelectTargetStar = null;
            CruiseAssistPlugin.SelectTargetHive = null;
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.SetTargetAstroId(indicatorAstroId);
            });
        }

        if (CruiseAssistPlugin.SelectTargetStar != null)
        {
            if (GameMain.localStar != null && CruiseAssistPlugin.SelectTargetStar.id == GameMain.localStar.id)
            {
                if (CruiseAssistPlugin.SelectTargetPlanet == null && CruiseAssistPlugin.SelectTargetHive == null && GameMain.localPlanet != null)
                {
                    CruiseAssistPlugin.SelectTargetStar = null;
                    CruiseAssistPlugin.SelectTargetEnemyId = 0;
                    CruiseAssistPlugin.SelectTargetAstroId = 0;
                    GameMain.mainPlayer.navigation.indicatorAstroId = 0;
                    CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
                    {
                        extension.SetInactive();
                    });
                    return;
                }
                if (CruiseAssistPlugin.SelectTargetPlanet != null)
                {
                    if (GameMain.localPlanet != null && CruiseAssistPlugin.SelectTargetPlanet.id == GameMain.localPlanet.id)
                    {
                        CruiseAssistPlugin.CheckDeactivate();
                        return;
                    }
                    CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.SelectTargetPlanet;
                }
                else if(CruiseAssistPlugin.SelectTargetHive != null)
                {
                    if ((GameMain.spaceSector.astros[CruiseAssistPlugin.SelectTargetHive.hiveAstroId - 1000000].uPos - GameMain.mainPlayer.uPosition).magnitude < CruiseAssistPlugin.HIVE_IN_RANGE)
                    {
                        CruiseAssistPlugin.CheckDeactivate();
                        return;
                    }
                    CruiseAssistPlugin.TargetHive = CruiseAssistPlugin.SelectTargetHive;
                }
                else if (CruiseAssistPlugin.ReticuleTargetPlanet != null)
                {
                    CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.ReticuleTargetPlanet;
                }
            }
            else
            {
                CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.SelectTargetStar;
                CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.SelectTargetPlanet;
                CruiseAssistPlugin.TargetHive = CruiseAssistPlugin.SelectTargetHive;
            }
        }
        else if(CruiseAssistPlugin.SelectTargetEnemyIdF != 0)
        {
            if((CruiseAssistPlugin.SelectTargetEnemy.pos - GameMain.mainPlayer.uPosition).magnitude < CruiseAssistPlugin.ENEMY_IN_RANGE)
            {
                CruiseAssistPlugin.CheckDeactivate();
                return;
            }
            CruiseAssistPlugin.TargetEnemyId = CruiseAssistPlugin.SelectTargetEnemyId;
        }
        else if (CruiseAssistPlugin.ReticuleTargetPlanet != null)
        {
            CruiseAssistPlugin.SelectTargetPlanet = CruiseAssistPlugin.ReticuleTargetPlanet;
            CruiseAssistPlugin.SelectTargetStar = CruiseAssistPlugin.SelectTargetPlanet.star;
            CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.ReticuleTargetPlanet;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.TargetPlanet.star;
        }
        else if (CruiseAssistPlugin.ReticuleTargetStar != null)
        {
            CruiseAssistPlugin.SelectTargetPlanet = null;
            CruiseAssistPlugin.SelectTargetStar = CruiseAssistPlugin.ReticuleTargetStar;
            CruiseAssistPlugin.TargetPlanet = null;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.ReticuleTargetStar;
        }
        Player player = __instance.player;

        if (CruiseAssistPlugin.TargetPlanet != null)
        {
            CruiseAssistPlugin.State = CruiseAssistState.TO_PLANET;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.TargetPlanet.star;
            CruiseAssistPlugin.TargetRange = (CruiseAssistPlugin.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssistPlugin.TargetPlanet.realRadius;
            CruiseAssistPlugin.TargetSelected = true;
        }
        else if(CruiseAssistPlugin.TargetHive != null)
        {
            CruiseAssistPlugin.State = CruiseAssistState.TO_HIVE;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.TargetHive.starData;
            CruiseAssistPlugin.TargetRange = (GameMain.spaceSector.astros[CruiseAssistPlugin.TargetHive.hiveAstroId - 1000000].uPos - GameMain.mainPlayer.uPosition).magnitude - CruiseAssistPlugin.HIVE_IN_RANGE;
            CruiseAssistPlugin.TargetSelected= true;
        }
        else if(CruiseAssistPlugin.TargetEnemyId != 0)
        {
            CruiseAssistPlugin.State = CruiseAssistState.TO_ENEMY;
            CruiseAssistPlugin.TargetRange = (CruiseAssistPlugin.TargetEnemy.pos - GameMain.mainPlayer.uPosition).magnitude;
            CruiseAssistPlugin.TargetSelected = true;
        }
        else
        {
            if (CruiseAssistPlugin.TargetStar == null)
            {
                CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
                {
                    extension.SetInactive();
                });
                return;
            }
            CruiseAssistPlugin.State = CruiseAssistState.TO_STAR;
            CruiseAssistPlugin.TargetRange = (CruiseAssistPlugin.TargetStar.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)(CruiseAssistPlugin.TargetStar.viewRadius - 120f);
            CruiseAssistPlugin.TargetSelected = true;
        }

        if (__instance.controller.movementStateInFrame > EMovementState.Walk)
        {
            return;
        }
        if (VFInput._jump.pressing)
        {
            CruiseAssistPlugin.Interrupt = true;
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.CancelOperate();
            });
        }
        else
        {
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.OperateWalk(__instance);
            });
        }
    }
}

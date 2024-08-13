using System.Linq;
using HarmonyLib;
using tanu.CruiseAssist;

[HarmonyPatch(typeof(PlayerMove_Walk))]
internal class Patch_PlayerMoveWalk
{
    [HarmonyPatch("GameTick")]
    [HarmonyPrefix]
    public static void GameTick_Prefix(PlayerMove_Walk __instance)
    {
        CruiseAssistPlugin.State = CruiseAssistState.INACTIVE;
        CruiseAssistPlugin.Interrupt = false;
        CruiseAssistPlugin.TargetStar = null;
        CruiseAssistPlugin.TargetPlanet = null;
        CruiseAssistPlugin.TargetUPos = VectorLF3.zero;
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
        if (UIRoot.instance.uiGame.starmap.focusHive != null)
        {
            return;
        }
        if (indicatorAstroId != 0 && CruiseAssistPlugin.SelectTargetAstroId != indicatorAstroId)
        {
            CruiseAssistPlugin.SelectTargetAstroId = indicatorAstroId;
            if (indicatorAstroId % 100 != 0)
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
        if (CruiseAssistPlugin.SelectTargetStar != null)
        {
            if (GameMain.localStar != null && CruiseAssistPlugin.SelectTargetStar.id == GameMain.localStar.id)
            {
                if (CruiseAssistPlugin.SelectTargetPlanet == null && GameMain.localPlanet != null)
                {
                    CruiseAssistPlugin.SelectTargetStar = null;
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
                        CruiseAssistPlugin.SelectTargetStar = null;
                        CruiseAssistPlugin.SelectTargetPlanet = null;
                        CruiseAssistPlugin.SelectTargetAstroId = 0;
                        GameMain.mainPlayer.navigation.indicatorAstroId = 0;
                        CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
                        {
                            extension.SetInactive();
                        });
                        return;
                    }
                    CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.SelectTargetPlanet;
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
            }
        }
        else if (CruiseAssistPlugin.ReticuleTargetPlanet != null)
        {
            CruiseAssistPlugin.TargetPlanet = CruiseAssistPlugin.ReticuleTargetPlanet;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.TargetPlanet.star;
            CruiseAssistPlugin.Interrupt = false;
        }
        else if (CruiseAssistPlugin.ReticuleTargetStar != null)
        {
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.ReticuleTargetStar;
            CruiseAssistPlugin.Interrupt = false;
        }
        Player player = __instance.player;

        if (CruiseAssistPlugin.TargetPlanet != null)
        {
            CruiseAssistPlugin.State = CruiseAssistState.TO_PLANET;
            CruiseAssistPlugin.TargetStar = CruiseAssistPlugin.TargetPlanet.star;
            CruiseAssistPlugin.TargetRange = (CruiseAssistPlugin.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssistPlugin.TargetPlanet.realRadius;
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

using HarmonyLib;
using tanu.CruiseAssist;
using UnityEngine;

[HarmonyPatch(typeof(PlayerMove_Sail))]
internal class Patch_PlayerMoveSail
{
    [HarmonyPatch("GameTick")]
    [HarmonyPrefix]
    public static void GameTick_Prefix(PlayerMove_Sail __instance)
    {
        if (!CruiseAssistPlugin.Enable || !CruiseAssistPlugin.TargetSelected)
        {
            return;
        }
        Player player = __instance.player;
        if (!player.sailing)
        {
            return;
        }
        if (GameMain.mainPlayer.controller.input0 != Vector4.zero || GameMain.mainPlayer.controller.input1 != Vector4.zero)
        {
            CruiseAssistPlugin.Interrupt = true;
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.CancelOperate();
            });
            return;
        }
        if (CruiseAssistPlugin.TargetPlanet != null)
        {
            CruiseAssistPlugin.TargetUPos = CruiseAssistPlugin.TargetPlanet.uPosition;
        }
        else
        {
            if (CruiseAssistPlugin.TargetStar == null)
            {
                return;
            }
            CruiseAssistPlugin.TargetUPos = CruiseAssistPlugin.TargetStar.uPosition;
        }
        bool operate = false;
        CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
        {
            operate |= extension.OperateSail(__instance);
        });
        if (!operate)
        {
            VectorLF3 vectorLF = CruiseAssistPlugin.TargetUPos - player.uPosition;
            float b = Vector3.Angle(vectorLF, player.uVelocity);
            float t = 1.6f / Mathf.Max(10f, b);
            double magnitude = player.controller.actionSail.visual_uvel.magnitude;
            player.uVelocity = Vector3.Slerp(player.uVelocity, vectorLF.normalized * magnitude, t);
        }
    }
}

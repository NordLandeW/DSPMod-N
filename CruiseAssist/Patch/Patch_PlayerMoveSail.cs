using HarmonyLib;
using System;
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
        if (GameMain.mainPlayer.controller.input0 != Vector4.zero || (GameMain.mainPlayer.controller.input1 != Vector4.zero && GameMain.mainPlayer.controller.input1.y >= 0))
        {
            CruiseAssistPlugin.Interrupt = true;
            CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
            {
                extension.CancelOperate();
            });
            return;
        }
        bool operate = false;
        CruiseAssistPlugin.extensions.ForEach(delegate (CruiseAssistExtensionAPI extension)
        {
            operate |= extension.OperateSail(__instance);
        });
        if (!operate)
        {
            VectorLF3 dist = CruiseAssistPlugin.TargetUPos - player.uPosition;
            float b = Vector3.Angle(dist, player.uVelocity);
            float t = 1.6f / Mathf.Max(10f, b);
            double magnitude = player.controller.actionSail.visual_uvel.magnitude;
            player.uVelocity = Vector3.Slerp(player.uVelocity, dist.normalized * magnitude, t);

            // Add speed control when approaching enemy
            if(CruiseAssistPlugin.State == CruiseAssistState.TO_ENEMY)
            {
                player.controller.actionSail.warpSpeedControl = 1.0 - Math.Pow(Maths.Clamp01(1.0 - (dist.magnitude - 3000.0) / 400000.0), 2.0) * 0.65;
            }
        }
    }
}

using HarmonyLib;
using tanu.CruiseAssist;

[HarmonyPatch(typeof(PlayerMove_Drift))]
internal class Patch_PlayerMoveDrift
{
    [HarmonyPatch("GameTick")]
    [HarmonyPrefix]
    public static void GameTick_Prefix(PlayerMove_Drift __instance)
    {
        if (!CruiseAssistPlugin.Enable || !CruiseAssistPlugin.TargetSelected || __instance.controller.movementStateInFrame != EMovementState.Drift)
        {
            return;
        }
        if (VFInput._moveForward.pressing || VFInput._pullUp.pressing)
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
                extension.OperateDrift(__instance);
            });
        }
    }
}

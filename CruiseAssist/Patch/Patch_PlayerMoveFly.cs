using HarmonyLib;
using tanu.CruiseAssist;

[HarmonyPatch(typeof(PlayerMove_Fly))]
internal class Patch_PlayerMoveFly
{
    [HarmonyPatch("GameTick")]
    [HarmonyPrefix]
    public static void GameTick_Prefix(PlayerMove_Fly __instance)
    {
        if (!CruiseAssistPlugin.Enable || !CruiseAssistPlugin.TargetSelected || __instance.controller.movementStateInFrame != EMovementState.Fly)
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
                extension.OperateFly(__instance);
            });
        }
    }
}

using System;
using HarmonyLib;

namespace tanu.AutoPilot
{
	[HarmonyPatch(typeof(VFInput))]
	internal class Patch_VFInput
	{
		[HarmonyPatch("_sailSpeedUp", MethodType.Getter)]
		[HarmonyPostfix]
		public static void SailSpeedUp_Postfix(ref bool __result)
		{
			if (AutoPilotPlugin.State != AutoPilotState.INACTIVE)
			{
				if (AutoPilotPlugin.InputSailSpeedUp)
					__result = true;
			}
		}
	}
}

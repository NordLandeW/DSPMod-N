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
			bool flag = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			if (!flag)
			{
				bool inputSailSpeedUp = AutoPilotPlugin.InputSailSpeedUp;
				if (inputSailSpeedUp)
				{
					__result = true;
				}
			}
		}
	}
}

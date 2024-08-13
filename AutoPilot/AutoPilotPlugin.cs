using System;
using BepInEx;
using HarmonyLib;
using tanu.CruiseAssist;

namespace tanu.AutoPilot
{
	[BepInDependency("nord.CruiseAssist", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin("nord.AutoPilot", "AutoPilot-N", "0.1.0")]
	public class AutoPilotPlugin : BaseUnityPlugin
	{
		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new AutoPilotConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			this.harmony = new Harmony("tanu.AutoPilot.Patch");
			this.harmony.PatchAll(typeof(Patch_VFInput));
			CruiseAssistPlugin.RegistExtension(new AutoPilotExtension());
		}

		public void OnDestroy()
		{
			CruiseAssistPlugin.UnregistExtension(typeof(AutoPilotExtension));
			this.harmony.UnpatchSelf();
		}

		public static double EnergyPer = 0.0;

		public static double Speed = 0.0;

		public static int WarperCount = 0;

		public static bool LeavePlanet = false;

		public static bool SpeedUp = false;

		public static AutoPilotState State = AutoPilotState.INACTIVE;

		public static bool InputSailSpeedUp = false;

		private Harmony harmony;

		public class Conf
		{
			public static int MinEnergyPer = 20;

			public static int MaxSpeed = 2000;

			public static int WarpMinRangeAU = 2;

			public static int SpeedToWarp = 1200;

			public static bool LocalWarpFlag = false;

			public static bool AutoStartFlag = true;

			public static bool IgnoreGravityFlag = true;

			public static bool MainWindowJoinFlag = true;
		}
	}
}

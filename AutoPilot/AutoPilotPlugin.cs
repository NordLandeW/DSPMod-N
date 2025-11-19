using System;
using BepInEx;
using HarmonyLib;
using tanu.CruiseAssist;

namespace tanu.AutoPilot
{
	[BepInDependency("nord.CruiseAssist", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class AutoPilotPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "nord.AutoPilot";
        public const string ModName = "Autopilot-N";
        public const string ModVersion = "1.4.3";

        public void Awake()
		{
			LogManager.Logger = Logger;
			new AutoPilotConfigManager(Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_VFInput));
			CruiseAssistPlugin.RegistExtension(new AutoPilotExtension());
		}

		public void OnDestroy()
		{
			CruiseAssistPlugin.UnregistExtension(typeof(AutoPilotExtension));
			harmony.UnpatchSelf();
		}

		public static bool IsLocalPlanetSatellite()
		{
			return GameMain.localPlanet?.orbitAround != 0;
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

			public static int SpeedToWarp = 400;

			public static bool LocalWarpFlag = false;

			public static bool AutoStartFlag = true;

			public static bool IgnoreGravityFlag = false;

			public static bool MainWindowJoinFlag = true;

			public static bool SpeedUpWhenFlying = true;

			public static bool AvoidGasGiants = true;
		}

        public static bool safeToGo = false;
		public static PlanetData lastVisitedPlanet = null;
		public static bool warped = false;
	}
}

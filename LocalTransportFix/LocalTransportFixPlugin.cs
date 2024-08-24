using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using BepInEx.Configuration;

namespace nord.LocalTransportFix
{
    public class LocalTransportFixConfig : ConfigManager
    {
        public LocalTransportFixConfig(ConfigFile Config) : base(Config)
        {
        }

        protected override void CheckConfigImplements(Step step)
        {
            bool saveFlag = false;

            if (step == Step.AWAKE)
            {
                var modVersion = Bind("Base", "ModVersion", LocalTransportFixPlugin.ModVersion, "Don't change.");
                modVersion.Value = LocalTransportFixPlugin.ModVersion;

                LocalTransportFixPlugin.TicksBetweenUpdate = Bind("Setting", "TicksBetweenUpdate", LocalTransportFixPlugin.TicksBetweenUpdate, "The trigger frequency of the Logistics Drone in game ticks (vanilla setting is 10 ticks). Minimum allowed value is 1 tick.").Value;

                saveFlag = true;
            }
            if (saveFlag)
            {
                Save(false);
            }
        }
    }

    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class LocalTransportFixPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "nord.LocalTransportFix";
        public const string ModName = "LocalTransportFix";
        public const string ModVersion = "1.0.0";

        public static int TicksBetweenUpdate = 3;
        private Harmony harmony;

        public void Awake()
        {
            LogManager.Logger = base.Logger;

            new LocalTransportFixConfig(base.Config);
            ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);

            harmony = new Harmony($"{ModGuid}.Patch");
            harmony.PatchAll(typeof(LocalTransportFix));
        }

        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }

    public class LocalTransportFix
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.InternalTickLocal))]
        public static IEnumerable<CodeInstruction> InternalTickLocal_Patch(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldc_I4_S));

            matcher.MatchForward(true,
                    new CodeMatch(OpCodes.Ldarg_2),
                    new CodeMatch(OpCodes.Ldc_I4_S));

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, LocalTransportFixPlugin.TicksBetweenUpdate));

            matcher.Advance(1)
                .MatchForward(true, new CodeMatch(OpCodes.Ldc_I4_S));
            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, LocalTransportFixPlugin.TicksBetweenUpdate));

            LogManager.LogInfo($"Update tick set to {LocalTransportFixPlugin.TicksBetweenUpdate}.");

            return matcher.InstructionEnumeration();
        }
    }
}

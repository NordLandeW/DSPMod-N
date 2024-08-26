using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace tanu.CruiseAssist
{
    [HarmonyPatch(typeof(GameData), nameof(GameData.ArriveStar))]
    public class Patch_ArriveStar
    {
        [HarmonyPrefix]
        public static bool ArriveStar_Prefix(StarData star)
        {
            if(CruiseAssistPlugin.preloadStar != null && CruiseAssistPlugin.preloadStar.id != star?.id)
            {
                //LogManager.LogInfo("Skip star loading.");
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(GameData), nameof(GameData.DetermineLocalPlanet))]
    public class Patch_DeterminLocalPlanet
    {
        [HarmonyPrefix]
        public static bool DetermineLocalPlanet_Prefix(ref bool __result)
        {
            StarData nStar = null;
            PlanetData nPlanet = null;
            GameMain.data.GetNearestStarPlanet(ref nStar, ref nPlanet);
            // Prevent loading other stars & planets if target star is set.
            if (CruiseAssistPlugin.preloadStar != null && nStar?.id != CruiseAssistPlugin.preloadStar.id)
            {
                //LogManager.LogInfo("Skip planet determine.");
                __result = false;
                return false;
            }
            return true;
        }
    }
}

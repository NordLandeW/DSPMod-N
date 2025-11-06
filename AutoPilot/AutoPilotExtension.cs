using System;
using System.Linq;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
    internal class AutoPilotExtension : CruiseAssistExtensionAPI
    {
        public bool CheckActive()
        {
            return AutoPilotPlugin.State == AutoPilotState.ACTIVE;
        }

        public void CheckConfig(string step)
        {
            if (Enum.TryParse(step, out ConfigManager.Step stepEnum))
            {
                ConfigManager.CheckConfig(stepEnum);
            }
        }

        public void SetTargetAstroId(int astroId)
        {
            LogManager.LogInfo("Trying set auto start.");
            AutoPilotPlugin.State = AutoPilotPlugin.Conf.AutoStartFlag ? AutoPilotState.ACTIVE : AutoPilotState.INACTIVE;
            SetSpeedUpInput(false);
        }

        public bool OperateWalk(PlayerMove_Walk __instance)
        {
            return OperateLocomotion(__instance.player, false);
        }

        public bool OperateDrift(PlayerMove_Drift __instance)
        {
            return OperateLocomotion(__instance.player, false);
        }

        public bool OperateFly(PlayerMove_Fly __instance)
        {
            return OperateLocomotion(__instance.player, true);
        }

        public bool OperateSail(PlayerMove_Sail __instance)
        {
            UpdateSafetyStatus(__instance.player);

            if (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
            {
                return false;
            }

            Player player = __instance.player;
            Mecha mecha = player.mecha;

            UpdatePlayerSailState(player, mecha);

            if (player.warping)
            {
                AutoPilotPlugin.warped = true;
                return false;
            }

            if (AutoPilotPlugin.EnergyPer < AutoPilotPlugin.Conf.MinEnergyPer)
            {
                return false;
            }

            HandleGravity(player);
            HandleSpeedUp();

            if (GameMain.localPlanet == null && AutoPilotPlugin.safeToGo)
            {
                HandleWarp(player, mecha);
                return false;
            }

            return HandlePlanetProximity(player);
        }

        public void SetInactive()
        {
            AutoPilotPlugin.State = AutoPilotState.INACTIVE;
            AutoPilotPlugin.InputSailSpeedUp = false;
            AutoPilotPlugin.warped = false;
        }

        public void CancelOperate()
        {
            SetInactive();
        }

        public void OnGUI()
        {
            float scale = CruiseAssistMainUI.Scale / 100f;

            AutoPilotMainUI.OnGUI();

            if (AutoPilotConfigUI.Show[CruiseAssistMainUI.wIdx])
            {
                AutoPilotConfigUI.OnGUI();
            }

            if (AutoPilotDebugUI.Show)
            {
                AutoPilotDebugUI.OnGUI();
            }

            bool inputReset = ResetInputOnMouseActivity(AutoPilotMainUI.Rect[CruiseAssistMainUI.wIdx], scale);
            if (!inputReset && AutoPilotConfigUI.Show[CruiseAssistMainUI.wIdx])
            {
                inputReset = ResetInputOnMouseActivity(AutoPilotConfigUI.Rect[CruiseAssistMainUI.wIdx], scale);
            }
            if (!inputReset && AutoPilotDebugUI.Show)
            {
                inputReset = ResetInputOnMouseActivity(AutoPilotDebugUI.Rect, scale);
            }
        }

        private bool OperateLocomotion(Player player, bool isFlying)
        {
            if (AutoPilotPlugin.State == AutoPilotState.INACTIVE)
            {
                return false;
            }

            UpdatePlayerLocomotionState(player);

            if (isFlying)
            {
                player.controller.input0.y += 1f;
                player.controller.input1.y += 1f;

                if (AutoPilotPlugin.Conf.SpeedUpWhenFlying)
                    SetSpeedUpInput(true);
            }
            else
            {
                player.controller.input0.z = 1f;

                SetSpeedUpInput(false);
            }

            return true;
        }

        private void UpdatePlayerLocomotionState(Player player)
        {
            Mecha mecha = player.mecha;
            AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
            AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
            AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
            AutoPilotPlugin.LeavePlanet = true;
            AutoPilotPlugin.SpeedUp = false;
            AutoPilotPlugin.InputSailSpeedUp = false;

            HandleGravity(player);
        }

        private void UpdatePlayerSailState(Player player, Mecha mecha)
        {
            AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
            AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
            AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
            AutoPilotPlugin.LeavePlanet = false;
            AutoPilotPlugin.SpeedUp = false;
            AutoPilotPlugin.InputSailSpeedUp = false;
        }

        private void UpdateSafetyStatus(Player player)
        {
            if (AutoPilotPlugin.lastVisitedPlanet == null || GameMain.localStar == null)
            {
                AutoPilotPlugin.safeToGo = true;
                return;
            }

            VectorLF3 playerPos = player.uPosition;
            VectorLF3 targetPos = CruiseAssistPlugin.TargetUPos;
            VectorLF3 planetPos = AutoPilotPlugin.lastVisitedPlanet.uPosition;

            VectorLF3 so = planetPos - playerPos;
            VectorLF3 sd = targetPos - playerPos;

            double dotSoSd = VectorLF3.Dot(so, sd);
            double sdMagnitude = sd.magnitude;

            if (dotSoSd < 0 || sdMagnitude == 0)
            {
                AutoPilotPlugin.safeToGo = true;
                return;
            }

            double projectionLength = Math.Abs(dotSoSd) / sdMagnitude;
            if (sdMagnitude < projectionLength)
            {
                AutoPilotPlugin.safeToGo = true;
                return;
            }

            VectorLF3 projectionVector = sd.normalized * projectionLength;
            VectorLF3 planetToPath = projectionVector - so;

            AutoPilotPlugin.safeToGo = planetToPath.magnitude > (AutoPilotPlugin.lastVisitedPlanet.realRadius + 400.0);
        }

        private void HandleGravity(Player player)
        {
            if (AutoPilotPlugin.Conf.IgnoreGravityFlag)
            {
                player.controller.universalGravity = VectorLF3.zero;
                player.controller.localGravity = VectorLF3.zero;
            }
        }

        private void HandleSpeedUp()
        {
            if (AutoPilotPlugin.Speed < AutoPilotPlugin.Conf.MaxSpeed)
            {
                SetSpeedUpInput(true);
            }
        }

        private void SetSpeedUpInput(bool enable)
        {
            AutoPilotPlugin.InputSailSpeedUp = enable;
            AutoPilotPlugin.SpeedUp = enable;
        }

        private void HandleWarp(Player player, Mecha mecha)
        {
            bool canWarpToTarget = AutoPilotPlugin.Conf.LocalWarpFlag ||
                                   GameMain.localStar == null ||
                                   (CruiseAssistPlugin.TargetStar != null && CruiseAssistPlugin.TargetStar.id != GameMain.localStar.id) ||
                                   CruiseAssistPlugin.TargetEnemyId != 0;

            if (!canWarpToTarget) return;

            bool inRange = AutoPilotPlugin.Conf.WarpMinRangeAU * 40000.0 <= CruiseAssistPlugin.TargetRange;
            bool fastEnough = AutoPilotPlugin.Conf.SpeedToWarp <= AutoPilotPlugin.Speed;
            bool hasWarper = AutoPilotPlugin.WarperCount >= 1;
            bool hasEnoughEnergy = mecha.coreEnergy > mecha.warpStartPowerPerSpeed * mecha.maxWarpSpeed;

            if (inRange && fastEnough && hasWarper && hasEnoughEnergy && !AutoPilotPlugin.warped)
            {
                if (mecha.UseWarper())
                {
                    player.warpCommand = true;
                    AutoPilotPlugin.warped = true;
                    VFAudio.Create("warp-begin", player.transform, Vector3.zero, true, 0, -1, -1L);
                }
            }
        }

        private bool HandlePlanetProximity(Player player)
        {
            if (GameMain.localPlanet != null)
            {
                AutoPilotPlugin.lastVisitedPlanet = GameMain.localPlanet;
            }

            if (AutoPilotPlugin.lastVisitedPlanet == null)
            {
                return false;
            }

            VectorLF3 playerToPlanetVec = player.uPosition - AutoPilotPlugin.lastVisitedPlanet.uPosition;
            double distToPlanet = playerToPlanetVec.magnitude;

            if (AutoPilotPlugin.safeToGo && (GameMain.localPlanet == null || distToPlanet > AutoPilotPlugin.lastVisitedPlanet.realRadius + 400.0))
            {
                return false;
            }

            // Too close or unsafe, actively steer away
            AutoPilotPlugin.LeavePlanet = true;

            VectorLF3 awayFromPlanetDir = playerToPlanetVec.normalized;
            float angle = Vector3.Angle(player.uVelocity, awayFromPlanetDir);
            float t = 1.6f / Mathf.Max(10f, angle);

            VectorLF3 targetVelocity = awayFromPlanetDir * Math.Max(AutoPilotPlugin.Speed, 200f);
            Vector3 slerpedVelocity = Vector3.Slerp(player.uVelocity, targetVelocity, t);
            VectorLF3 velocityChange = (VectorLF3)slerpedVelocity - player.uVelocity;

            player.controller.actionSail.UseSailEnergy(ref velocityChange, 1.5);
            player.uVelocity += velocityChange;
            return true;
        }

        private bool ResetInputOnMouseActivity(Rect windowRect, float scale)
        {
            float xMin = windowRect.xMin * scale;
            float xMax = windowRect.xMax * scale;
            float yMin = windowRect.yMin * scale;
            float yMax = windowRect.yMax * scale;

            float mouseX = Input.mousePosition.x;
            float mouseY = Screen.height - Input.mousePosition.y;

            bool isMouseOver = xMin <= mouseX && mouseX <= xMax && yMin <= mouseY && mouseY <= yMax;

            if (isMouseOver)
            {
                bool isMouseButtonPressed = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
                bool isMouseScrolled = Input.mouseScrollDelta.y != 0f;

                if (isMouseButtonPressed || isMouseScrolled)
                {
                    Input.ResetInputAxes();
                    return true;
                }
            }

            return false;
        }
    }
}

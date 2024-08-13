using System;
using System.Linq;
using tanu.CruiseAssist;
using UnityEngine;

namespace tanu.AutoPilot
{
	internal class AutoPilotExtension : CruiseAssistExtensionAPI
	{
		public void CheckConfig(string step)
		{
			ConfigManager.Step step2;
			EnumUtils.TryParse<ConfigManager.Step>(step, out step2);
			ConfigManager.CheckConfig(step2);
		}

		public void SetTargetAstroId(int astroId)
		{
			AutoPilotPlugin.State = (AutoPilotPlugin.Conf.AutoStartFlag ? AutoPilotState.ACTIVE : AutoPilotState.INACTIVE);
			AutoPilotPlugin.InputSailSpeedUp = false;
		}

		public bool OperateWalk(PlayerMove_Walk __instance)
		{
			bool flag = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Player player = __instance.player;
				Mecha mecha = player.mecha;
				AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
				AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
				AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
				AutoPilotPlugin.LeavePlanet = true;
				AutoPilotPlugin.SpeedUp = false;
				AutoPilotPlugin.InputSailSpeedUp = false;
				bool ignoreGravityFlag = AutoPilotPlugin.Conf.IgnoreGravityFlag;
				if (ignoreGravityFlag)
				{
					player.controller.universalGravity = VectorLF3.zero;
					player.controller.localGravity = VectorLF3.zero;
				}
				__instance.controller.input0.z = 1f;
				result = true;
			}
			return result;
		}

		public bool OperateDrift(PlayerMove_Drift __instance)
		{
			bool flag = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Player player = __instance.player;
				Mecha mecha = player.mecha;
				AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
				AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
				AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
				AutoPilotPlugin.LeavePlanet = true;
				AutoPilotPlugin.SpeedUp = false;
				AutoPilotPlugin.InputSailSpeedUp = false;
				bool ignoreGravityFlag = AutoPilotPlugin.Conf.IgnoreGravityFlag;
				if (ignoreGravityFlag)
				{
					player.controller.universalGravity = VectorLF3.zero;
					player.controller.localGravity = VectorLF3.zero;
				}
				__instance.controller.input0.z = 1f;
				result = true;
			}
			return result;
		}

		public bool OperateFly(PlayerMove_Fly __instance)
		{
			bool flag = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Player player = __instance.player;
				Mecha mecha = player.mecha;
				AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
				AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
				AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
				AutoPilotPlugin.LeavePlanet = true;
				AutoPilotPlugin.SpeedUp = false;
				AutoPilotPlugin.InputSailSpeedUp = false;
				bool ignoreGravityFlag = AutoPilotPlugin.Conf.IgnoreGravityFlag;
				if (ignoreGravityFlag)
				{
					player.controller.universalGravity = VectorLF3.zero;
					player.controller.localGravity = VectorLF3.zero;
				}
				PlayerController controller = __instance.controller;
				controller.input0.y = controller.input0.y + 1f;
				PlayerController controller2 = __instance.controller;
				controller2.input1.y = controller2.input1.y + 1f;
				result = true;
			}
			return result;
		}

		public bool OperateSail(PlayerMove_Sail __instance)
		{
			bool flag = AutoPilotPlugin.State == AutoPilotState.INACTIVE;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Player player = __instance.player;
				Mecha mecha = player.mecha;
				AutoPilotPlugin.EnergyPer = mecha.coreEnergy / mecha.coreEnergyCap * 100.0;
				AutoPilotPlugin.Speed = player.controller.actionSail.visual_uvel.magnitude;
				AutoPilotPlugin.WarperCount = mecha.warpStorage.GetItemCount(1210);
				AutoPilotPlugin.LeavePlanet = false;
				AutoPilotPlugin.SpeedUp = false;
				AutoPilotPlugin.InputSailSpeedUp = false;
				bool warping = player.warping;
				if (warping)
				{
					result = false;
				}
				else
				{
					bool flag2 = AutoPilotPlugin.EnergyPer < (double)AutoPilotPlugin.Conf.MinEnergyPer;
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool ignoreGravityFlag = AutoPilotPlugin.Conf.IgnoreGravityFlag;
						if (ignoreGravityFlag)
						{
							player.controller.universalGravity = VectorLF3.zero;
							player.controller.localGravity = VectorLF3.zero;
						}
						bool flag3 = AutoPilotPlugin.Speed < (double)AutoPilotPlugin.Conf.MaxSpeed;
						if (flag3)
						{
							AutoPilotPlugin.InputSailSpeedUp = true;
							AutoPilotPlugin.SpeedUp = true;
						}
						bool flag4 = GameMain.localPlanet == null;
						if (flag4)
						{
							bool flag5 = AutoPilotPlugin.Conf.LocalWarpFlag || GameMain.localStar == null || CruiseAssistPlugin.TargetStar.id != GameMain.localStar.id;
							if (flag5)
							{
								bool flag6 = (double)AutoPilotPlugin.Conf.WarpMinRangeAU * 40000.0 <= CruiseAssistPlugin.TargetRange && (double)AutoPilotPlugin.Conf.SpeedToWarp <= AutoPilotPlugin.Speed && 1 <= AutoPilotPlugin.WarperCount;
								if (flag6)
								{
									bool flag7 = mecha.coreEnergy > mecha.warpStartPowerPerSpeed * (double)mecha.maxWarpSpeed;
									if (flag7)
									{
										bool flag8 = mecha.UseWarper();
										if (flag8)
										{
											player.warpCommand = true;
											VFAudio.Create("warp-begin", player.transform, Vector3.zero, true, 0, -1, -1L);
										}
									}
								}
							}
							result = false;
						}
						else
						{
							VectorLF3 vectorLF = player.uPosition - GameMain.localPlanet.uPosition;
							bool flag9 = 120.0 < AutoPilotPlugin.Speed && (double)Math.Max(GameMain.localPlanet.realRadius, 800f) < vectorLF.magnitude - (double)GameMain.localPlanet.realRadius;
							if (flag9)
							{
								result = false;
							}
							else
							{
								VectorLF3 vec = player.uPosition - GameMain.localPlanet.uPosition;
								VectorLF3 vec2 = CruiseAssistPlugin.TargetUPos - GameMain.localPlanet.uPosition;
								bool flag10 = Vector3.Angle(vec, vec2) > 90f;
								VectorLF3 vec3;
								if (flag10)
								{
									vec3 = vectorLF;
									AutoPilotPlugin.LeavePlanet = true;
								}
								else
								{
									VectorLF3 vectorLF2 = CruiseAssistPlugin.TargetUPos - player.uPosition;
									vec3 = vectorLF2;
								}
								float b = Vector3.Angle(vec3, player.uVelocity);
								float t = 1.6f / Mathf.Max(10f, b);
								double rhs = Math.Min(AutoPilotPlugin.Speed, 120.0);
								player.uVelocity = Vector3.Slerp(player.uVelocity, vec3.normalized * rhs, t);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		public void SetInactive()
		{
			AutoPilotPlugin.State = AutoPilotState.INACTIVE;
			AutoPilotPlugin.InputSailSpeedUp = false;
		}

		public void CancelOperate()
		{
			AutoPilotPlugin.State = AutoPilotState.INACTIVE;
			AutoPilotPlugin.InputSailSpeedUp = false;
		}

		public void OnGUI()
		{
			UIGame uiGame = UIRoot.instance.uiGame;
			float scale = CruiseAssistMainUI.Scale / 100f;
			AutoPilotMainUI.OnGUI();
			bool flag = AutoPilotConfigUI.Show[CruiseAssistMainUI.wIdx];
			if (flag)
			{
				AutoPilotConfigUI.OnGUI();
			}
			bool show = AutoPilotDebugUI.Show;
			if (show)
			{
				AutoPilotDebugUI.OnGUI();
			}
			bool flag2 = this.ResetInput(AutoPilotMainUI.Rect[CruiseAssistMainUI.wIdx], scale);
			bool flag3 = !flag2 && AutoPilotConfigUI.Show[CruiseAssistMainUI.wIdx];
			if (flag3)
			{
				flag2 = this.ResetInput(AutoPilotConfigUI.Rect[CruiseAssistMainUI.wIdx], scale);
			}
			bool flag4 = !flag2 && AutoPilotDebugUI.Show;
			if (flag4)
			{
				flag2 = this.ResetInput(AutoPilotDebugUI.Rect, scale);
			}
		}

		private bool ResetInput(Rect rect, float scale)
		{
			float num = rect.xMin * scale;
			float num2 = rect.xMax * scale;
			float num3 = rect.yMin * scale;
			float num4 = rect.yMax * scale;
			float x = Input.mousePosition.x;
			float num5 = (float)Screen.height - Input.mousePosition.y;
			bool flag = num <= x && x <= num2 && num3 <= num5 && num5 <= num4;
			if (flag)
			{
				int[] array = new int[]
				{
					0,
					1,
					2
				};
				bool flag2 = Enumerable.Any<int>(array, new Func<int, bool>(Input.GetMouseButton)) || Input.mouseScrollDelta.y != 0f;
				if (flag2)
				{
					Input.ResetInputAxes();
					return true;
				}
			}
			return false;
		}
	}
}

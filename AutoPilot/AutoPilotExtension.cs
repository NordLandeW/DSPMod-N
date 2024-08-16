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
			ConfigManager.Step step2;
			EnumUtils.TryParse<ConfigManager.Step>(step, out step2);
			ConfigManager.CheckConfig(step2);
		}

		public void SetTargetAstroId(int astroId)
		{
			LogManager.LogInfo("Trying set auto start.");
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
            // Check if safe to go
            if (AutoPilotPlugin.lastVisitedPlanet != null)
            {
				if (GameMain.localStar != null)
				{
                    VectorLF3 S = __instance.player.uPosition;
                    VectorLF3 D = CruiseAssistPlugin.TargetUPos;
                    VectorLF3 O = AutoPilotPlugin.lastVisitedPlanet.uPosition;
                    VectorLF3 SO = O - S, SD = D - S;
                    double STm = Math.Abs(VectorLF3.Dot(SO, SD)) / SD.magnitude;
                    VectorLF3 ST = SD.normalized * STm;
                    VectorLF3 OT = ST - SO;
                    if (VectorLF3.Dot(SO, SD) < 0 || SD.magnitude < STm || OT.magnitude > (AutoPilotPlugin.lastVisitedPlanet.realRadius + 400.0))
                        AutoPilotPlugin.safeToGo = true;
                    else
                        AutoPilotPlugin.safeToGo = false;
                }
				else
				{
					AutoPilotPlugin.safeToGo = true;
				}
            }
			else
			{
				AutoPilotPlugin.safeToGo = true;
			}

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
						if (AutoPilotPlugin.Speed < (double)AutoPilotPlugin.Conf.MaxSpeed)
						{
							AutoPilotPlugin.InputSailSpeedUp = true;
							AutoPilotPlugin.SpeedUp = true;
						}

                        if (GameMain.localPlanet == null && AutoPilotPlugin.safeToGo)
						{
							if (AutoPilotPlugin.Conf.LocalWarpFlag || GameMain.localStar == null || CruiseAssistPlugin.TargetStar.id != GameMain.localStar.id || CruiseAssistPlugin.TargetEnemyId != 0)
							{
								if ((double) AutoPilotPlugin.Conf.WarpMinRangeAU * 40000.0 <= CruiseAssistPlugin.TargetRange && (double)AutoPilotPlugin.Conf.SpeedToWarp <= AutoPilotPlugin.Speed && 1 <= AutoPilotPlugin.WarperCount)
								{
									if (mecha.coreEnergy > mecha.warpStartPowerPerSpeed * (double)mecha.maxWarpSpeed)
									{
										if (mecha.UseWarper())
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
							if(GameMain.localPlanet != null)
								AutoPilotPlugin.lastVisitedPlanet = GameMain.localPlanet;   // Update the last visited planet.

							if (AutoPilotPlugin.lastVisitedPlanet == null)
								return false;
							else
							{
                                VectorLF3 vectorLF = player.uPosition - AutoPilotPlugin.lastVisitedPlanet.uPosition;
                                if (AutoPilotPlugin.safeToGo && (GameMain.localPlanet == null || vectorLF.magnitude > AutoPilotPlugin.lastVisitedPlanet.realRadius + 400.0))
                                {
                                    result = false;
                                }
                                else
                                {
                                    VectorLF3 vec3;
                                    vec3 = vectorLF;
                                    AutoPilotPlugin.LeavePlanet = true;
                                    float b = Vector3.Angle(vec3, player.uVelocity);
                                    float t = 1.6f / Mathf.Max(10f, b);
                                    VectorLF3 v = Vector3.Slerp(player.uVelocity, vec3.normalized * Math.Max(AutoPilotPlugin.Speed, 200f), t);
									v -= player.uVelocity;
									player.controller.actionSail.UseSailEnergy(ref v, 1.5);
									player.uVelocity += v;
                                    result = true;
                                }
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

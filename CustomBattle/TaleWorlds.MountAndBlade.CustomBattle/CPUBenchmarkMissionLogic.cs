using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CPUBenchmarkMissionLogic : MissionLogic
	{
		public CPUBenchmarkMissionLogic(int attackerInfCount, int attackerRangedCount, int attackerCavCount, int defenderInfCount, int defenderCavCount)
		{
			this._attackerInfCount = attackerInfCount;
			this._attackerRangedCount = attackerRangedCount;
			this._attackerCavCount = attackerCavCount;
			this._defenderInfCount = defenderInfCount;
			this._defenderCavCount = defenderCavCount;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			Utilities.EnableSingleGPUQueryPerFrame();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			this._paths = base.Mission.Scene.GetPathsWithNamePrefix("CameraPath");
			this._targets = base.Mission.Scene.GetPathsWithNamePrefix("CameraTarget");
			Array.Sort<Path>(this._paths, (Path x, Path y) => x.GetName().CompareTo(y.GetName()));
			Array.Sort<Path>(this._targets, (Path x, Path y) => x.GetName().CompareTo(y.GetName()));
			if (this._paths.Length != 0)
			{
				this._curPath = 0;
				this._cameraPassedDistanceOnPath = 0f;
				string name = this._paths[this._curPath].GetName();
				int num = name.LastIndexOf('_');
				this._curPathSpeed = (this._cameraSpeed = float.Parse(name.Substring(num + 1)));
				this._curPathLenght = this._paths[this._curPath].GetTotalLength();
				if (this._paths.Length > this._curPath + 1)
				{
					string name2 = this._paths[this._curPath + 1].GetName();
					int num2 = name2.LastIndexOf('_');
					this._nextPathSpeed = float.Parse(name2.Substring(num2 + 1));
				}
			}
		}

		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(10, true);
			if (!CPUBenchmarkMissionLogic._isSiege)
			{
				base.Mission.DefenderTeam.ClearTacticOptions();
				base.Mission.AttackerTeam.ClearTacticOptions();
				base.Mission.DefenderTeam.AddTacticOption(new TacticStop(base.Mission.Teams.Defender));
				base.Mission.AttackerTeam.AddTacticOption(new TacticStop(base.Mission.Teams.Attacker));
			}
		}

		private void SetupFormations()
		{
			if (CPUBenchmarkMissionLogic._isSiege)
			{
				this._showResultTime = 295f;
				Mission.Current.MainAgent = Mission.Current.AttackerTeam.ActiveAgents[0];
				Utilities.ConstructMainThreadJob(new CPUBenchmarkMissionLogic.MainThreadJobDelegate(Mission.Current.GetMissionBehavior<SiegeDeploymentHandler>().FinishDeployment), Array.Empty<object>());
			}
			else
			{
				MatrixFrame globalFrame = base.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
				MatrixFrame globalFrame2 = base.Mission.Scene.FindEntityWithTag("defend_mid").GetGlobalFrame();
				MatrixFrame globalFrame3 = base.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
				MatrixFrame globalFrame4 = base.Mission.Scene.FindEntityWithTag("attacker_right").GetGlobalFrame();
				MatrixFrame globalFrame5 = base.Mission.Scene.FindEntityWithTag("attacker_mid").GetGlobalFrame();
				MatrixFrame globalFrame6 = base.Mission.Scene.FindEntityWithTag("attacker_left").GetGlobalFrame();
				this._defLeftInf = base.Mission.DefenderTeam.GetFormation(0);
				this._defMidCav = base.Mission.DefenderTeam.GetFormation(1);
				this._defRightInf = base.Mission.DefenderTeam.GetFormation(2);
				this._defLeftBInf = base.Mission.DefenderTeam.GetFormation(3);
				this._defMidBInf = base.Mission.DefenderTeam.GetFormation(4);
				this._defRightBInf = base.Mission.DefenderTeam.GetFormation(5);
				this._attLeftInf = base.Mission.AttackerTeam.GetFormation(0);
				this._attRightInf = base.Mission.AttackerTeam.GetFormation(1);
				this._attLeftRanged = base.Mission.AttackerTeam.GetFormation(2);
				this._attRightRanged = base.Mission.AttackerTeam.GetFormation(3);
				this._attLeftCav = base.Mission.AttackerTeam.GetFormation(4);
				this._attRightCav = base.Mission.AttackerTeam.GetFormation(6);
				int num = this._defenderInfCount / 6;
				float num2 = (float)this._defenderInfCount / 3.8f;
				int num3 = 0;
				int num4 = this._attackerInfCount / 2;
				int num5 = 0;
				int num6 = this._attackerRangedCount / 2;
				int num7 = 0;
				int num8 = this._attackerCavCount / 2;
				int num9 = 0;
				foreach (Agent agent in base.Mission.Agents)
				{
					if (agent.Team != null && agent.Character != null)
					{
						if (agent.Team.IsDefender)
						{
							if (agent.Character.DefaultFormationClass == 2)
							{
								agent.Formation = this._defMidCav;
							}
							else if ((float)num3 < num2)
							{
								num3++;
								agent.Formation = this._defLeftInf;
							}
							else if ((float)num3 < num2 * 2f)
							{
								num3++;
								agent.Formation = this._defRightInf;
							}
							else if ((float)num3 < num2 * 2f + (float)num)
							{
								num3++;
								agent.Formation = this._defLeftBInf;
							}
							else if ((float)num3 < num2 * 2f + (float)(num * 2))
							{
								num3++;
								agent.Formation = this._defMidBInf;
							}
							else
							{
								agent.Formation = this._defRightBInf;
							}
						}
						else if (agent.Team.IsAttacker)
						{
							switch (agent.Character.DefaultFormationClass)
							{
							case 0:
								if (num5 < num4)
								{
									num5++;
									agent.Formation = this._attLeftInf;
								}
								else
								{
									agent.Formation = this._attRightInf;
								}
								break;
							case 1:
								if (num7 < num6)
								{
									num7++;
									agent.Formation = this._attLeftRanged;
								}
								else
								{
									agent.Formation = this._attRightRanged;
								}
								break;
							case 2:
								if (num9 < num8)
								{
									num9++;
									agent.Formation = this._attLeftCav;
								}
								else
								{
									agent.Formation = this._attRightCav;
								}
								break;
							}
						}
					}
				}
				base.Mission.IsTeleportingAgents = true;
				this._defLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defMidCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defLeftBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defMidBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defRightBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._attLeftRanged.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				this._attRightRanged.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				this._attLeftCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._attRightCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				this._defLeftInf.FormOrder = FormOrder.FormOrderCustom(35f);
				this._defMidCav.FormOrder = FormOrder.FormOrderCustom(30f);
				this._defRightInf.FormOrder = FormOrder.FormOrderCustom(35f);
				this._defLeftBInf.FormOrder = FormOrder.FormOrderCustom(25f);
				this._defMidBInf.FormOrder = FormOrder.FormOrderCustom(25f);
				this._defRightBInf.FormOrder = FormOrder.FormOrderCustom(25f);
				this._attLeftInf.FormOrder = FormOrder.FormOrderCustom(25f);
				this._attRightInf.FormOrder = FormOrder.FormOrderCustom(25f);
				this._attLeftRanged.FormOrder = FormOrder.FormOrderCustom(50f);
				this._attRightRanged.FormOrder = FormOrder.FormOrderCustom(50f);
				this._attLeftCav.FormOrder = FormOrder.FormOrderCustom(30f);
				this._attRightCav.FormOrder = FormOrder.FormOrderCustom(30f);
				this._defLeftInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame3.origin + globalFrame3.rotation.f * 20f * 1.125f + 8f * globalFrame3.rotation.s)), null, null);
				this._defMidCav.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame2.origin - globalFrame2.rotation.f * 20f)), null, null);
				this._defRightInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame.origin + globalFrame.rotation.f * 20f * 1.125f - 8f * globalFrame.rotation.s)), null, null);
				this._defLeftBInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame3.origin - globalFrame3.rotation.s * 10f)), null, null);
				this._defMidBInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame2.origin)), null, null);
				this._defRightBInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame.origin + globalFrame.rotation.s * 10f)), null, null);
				Vec3 vec = globalFrame5.origin - globalFrame6.origin;
				Vec3 vec2 = globalFrame5.origin - globalFrame4.origin;
				this._attLeftInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame6.origin + 0.65f * vec)), null, null);
				this._attRightInf.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame4.origin + 0.65f * vec2)), null, null);
				this._attLeftRanged.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame6.origin + globalFrame6.rotation.f * 20f - 0.3f * vec)), null, null);
				this._attRightRanged.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame4.origin + globalFrame4.rotation.f * 20f - 0.3f * vec2)), null, null);
				this._attLeftCav.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame6.origin - globalFrame6.rotation.f * 20f * 0.1f - globalFrame6.rotation.s * 25f)), null, null);
				this._attRightCav.SetPositioning(new WorldPosition?(new WorldPosition(base.Mission.Scene, globalFrame4.origin - globalFrame4.rotation.f * 20f * 0.1f + globalFrame4.rotation.s * 25f)), null, null);
				this._defLeftInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._defLeftInf.CreateNewOrderWorldPosition(0)));
				this._defMidCav.SetMovementOrder(MovementOrder.MovementOrderMove(this._defMidCav.CreateNewOrderWorldPosition(0)));
				this._defRightInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._defRightInf.CreateNewOrderWorldPosition(0)));
				this._defLeftBInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._defLeftBInf.CreateNewOrderWorldPosition(0)));
				this._defMidBInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._defMidBInf.CreateNewOrderWorldPosition(0)));
				this._defRightBInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._defRightBInf.CreateNewOrderWorldPosition(0)));
				this._attLeftInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._attLeftInf.CreateNewOrderWorldPosition(0)));
				this._attRightInf.SetMovementOrder(MovementOrder.MovementOrderMove(this._attRightInf.CreateNewOrderWorldPosition(0)));
				this._attLeftRanged.SetMovementOrder(MovementOrder.MovementOrderMove(this._attLeftRanged.CreateNewOrderWorldPosition(0)));
				this._attRightRanged.SetMovementOrder(MovementOrder.MovementOrderMove(this._attRightRanged.CreateNewOrderWorldPosition(0)));
				this._attLeftCav.SetMovementOrder(MovementOrder.MovementOrderMove(this._attLeftCav.CreateNewOrderWorldPosition(0)));
				this._attRightCav.SetMovementOrder(MovementOrder.MovementOrderMove(this._attRightCav.CreateNewOrderWorldPosition(0)));
				foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.SetControlledByAI(false, false);
						formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
					}
				}
				foreach (Formation formation2 in base.Mission.DefenderTeam.FormationsIncludingEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						formation2.SetControlledByAI(false, false);
						formation2.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
					}
				}
			}
			this._formationsSetUp = true;
		}

		public override void OnMissionTick(float dt)
		{
			this._benchmarkStarted = true;
		}

		protected override void OnEndMission()
		{
			Utilities.SetBenchmarkStatus(0, "");
		}

		public override void OnPreMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this._benchmarkStarted)
			{
				return;
			}
			if (!this._formationsSetUp && (CPUBenchmarkMissionLogic._isSiege || this._missionAgentSpawnLogic.IsDeploymentOver))
			{
				this.SetupFormations();
				Utilities.SetBenchmarkStatus(1, CPUBenchmarkMissionLogic._isSiege ? "#" : "");
			}
			if (this._formationsSetUp && !CPUBenchmarkMissionLogic._isSiege)
			{
				this.Check();
			}
			this._totalTime += dt;
			Utilities.SetBenchmarkStatus(3, string.Concat(new object[]
			{
				"Battle Size: ",
				this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount,
				" (",
				base.Mission.AttackerTeam.ActiveAgents.Count,
				") vs (",
				base.Mission.DefenderTeam.ActiveAgents.Count,
				") ",
				this._defenderCavCount + this._defenderInfCount
			}));
			if (this._benchmarkExit != 0f && !this._benchmarkFinished && this._totalTime - this._benchmarkExit >= 0.05f)
			{
				Utilities.SetBenchmarkStatus(2, "");
				MouseManager.ShowCursor(true);
				this._benchmarkFinished = true;
			}
			if (Input.IsKeyPressed(1) && this._benchmarkExit == 0f)
			{
				this._benchmarkExit = this._totalTime;
			}
			if (Input.IsKeyReleased(1) && this._benchmarkExit != 0f && this._totalTime - this._benchmarkExit < 0.05f)
			{
				this._benchmarkExit = 0f;
			}
			if (!this._benchmarkFinished && this._totalTime > this._showResultTime)
			{
				Utilities.SetBenchmarkStatus(2, "");
				MouseManager.ShowCursor(true);
				this._benchmarkFinished = true;
				this._benchmarkExit = this._totalTime;
			}
			if (this._benchmarkExit != 0f && this._totalTime - this._benchmarkExit > 9f)
			{
				Utilities.SetBenchmarkStatus(0, string.Concat(new object[]
				{
					"Battle Size: ",
					this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount,
					" vs ",
					this._defenderCavCount + this._defenderInfCount
				}));
				Mission.Current.EndMission();
			}
			MissionScreen missionScreen;
			if ((missionScreen = ScreenManager.TopScreen as MissionScreen) != null)
			{
				Camera combatCamera = missionScreen.CombatCamera;
				if (combatCamera != null && this._curPath < this._paths.Length)
				{
					if (this._benchmarkCamera == null)
					{
						this._benchmarkCamera = Camera.CreateCamera();
						this._benchmarkCamera.SetFovHorizontal(combatCamera.HorizontalFov, combatCamera.GetAspectRatio(), combatCamera.Near, combatCamera.Far);
					}
					if (this._cameraPassedDistanceOnPath < this._curPathLenght && this._cameraPassedDistanceOnPath > this._curPathLenght / 6f * 5f)
					{
						this._cameraSpeed = MathF.Lerp(this._curPathSpeed, (this._curPath != this._paths.Length - 1) ? ((this._nextPathSpeed + this._curPathSpeed) / 2f) : 5f, (this._cameraPassedDistanceOnPath - this._curPathLenght / 6f * 5f) / (this._curPathLenght / 6f), 1E-05f);
					}
					if (this._cameraPassedDistanceOnPath < this._curPathLenght / 6f)
					{
						this._cameraSpeed = MathF.Lerp((this._curPath != 0) ? ((this._curPathSpeed + this._prevPathSpeed) / 2f) : 5f, this._curPathSpeed, this._cameraPassedDistanceOnPath / (this._curPathLenght / 6f), 1E-05f);
					}
					this._cameraPassedDistanceOnPath += this._cameraSpeed * dt;
					if (this._cameraPassedDistanceOnPath >= this._paths[this._curPath].GetTotalLength() && this._curPath != this._paths.Length - 1)
					{
						this._curPath++;
						this._curPathLenght = this._paths[this._curPath].GetTotalLength();
						this._prevPathSpeed = this._curPathSpeed;
						this._curPathSpeed = this._nextPathSpeed;
						this._cameraPassedDistanceOnPath = this._cameraSpeed * dt;
						if (this._paths.Length > this._curPath + 1)
						{
							string name = this._paths[this._curPath + 1].GetName();
							int num = name.LastIndexOf('_');
							this._nextPathSpeed = float.Parse(name.Substring(num + 1));
						}
					}
					MatrixFrame frameForDistance = this._paths[this._curPath].GetFrameForDistance(MathF.Min(this._paths[this._curPath].GetTotalLength(), this._cameraPassedDistanceOnPath));
					MatrixFrame frameForDistance2 = this._targets[this._curPath].GetFrameForDistance(MathF.Min(1f, this._cameraPassedDistanceOnPath / this._paths[this._curPath].GetTotalLength()) * this._targets[this._curPath].GetTotalLength());
					this._benchmarkCamera.LookAt(frameForDistance.origin, frameForDistance2.origin, Vec3.Up);
					missionScreen.UpdateFreeCamera(this._benchmarkCamera.Frame);
					missionScreen.CustomCamera = missionScreen.CombatCamera;
				}
				if (Utilities.IsBenchmarkQuited())
				{
					Utilities.SetBenchmarkStatus(0, string.Concat(new object[]
					{
						"Battle Size: ",
						this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount,
						" vs ",
						this._defenderCavCount + this._defenderInfCount
					}));
					Mission.Current.EndMission();
				}
			}
		}

		private void Check()
		{
			float currentTime = base.Mission.CurrentTime;
			if (this._battlePhase == CPUBenchmarkMissionLogic.BattlePhase.Start && currentTime >= 5f)
			{
				base.Mission.IsTeleportingAgents = false;
				this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.ArrowShower;
				return;
			}
			if (this._battlePhase != CPUBenchmarkMissionLogic.BattlePhase.Start)
			{
				if (!this._isCurPhaseInPlay)
				{
					Debug.Print("State: " + this._battlePhase, 0, 7, 64UL);
					switch (this._battlePhase)
					{
					case CPUBenchmarkMissionLogic.BattlePhase.ArrowShower:
						this._attLeftRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._attRightRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defLeftBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defRightBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defMidBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
						this._defRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
						this._defLeftInf.FormOrder = FormOrder.FormOrderCustom(35f);
						this._defRightInf.FormOrder = FormOrder.FormOrderCustom(35f);
						this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
						this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
						break;
					case CPUBenchmarkMissionLogic.BattlePhase.MeleePosition:
					{
						Vec2 vec = -(this._attLeftInf.OrderPosition - this._defRightInf.OrderPosition);
						Vec2 vec2 = -(this._attRightInf.OrderPosition - this._defLeftInf.OrderPosition);
						vec.RotateCCW(0.08726646f);
						vec2.RotateCCW(-0.08726646f);
						WorldPosition worldPosition = this._attLeftInf.CreateNewOrderWorldPosition(0);
						worldPosition.SetVec2(worldPosition.AsVec2 + vec);
						this._attLeftInf.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
						WorldPosition worldPosition2 = this._attRightInf.CreateNewOrderWorldPosition(0);
						worldPosition2.SetVec2(worldPosition2.AsVec2 + vec2);
						this._attRightInf.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition2));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos:
					{
						Vec2 vec3 = this._attLeftRanged.OrderPosition;
						Vec2 direction = this._attLeftRanged.Direction;
						vec3 -= 15f * direction;
						direction.RotateCCW(1.5707964f);
						vec3 += 60f * direction;
						WorldPosition worldPosition3 = this._attLeftRanged.CreateNewOrderWorldPosition(0);
						worldPosition3.SetVec2(vec3);
						this._attLeftCav.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition3));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef:
					{
						MatrixFrame globalFrame = base.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
						Vec3 vec4 = globalFrame.origin + 40f * globalFrame.rotation.s;
						this._defMidCav.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec4)));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition:
					{
						Vec2 vec5 = this._attRightRanged.OrderPosition;
						Vec2 direction2 = this._attRightRanged.Direction;
						vec5 += 20f * direction2;
						direction2.RotateCCW(-1.5707964f);
						vec5 += 80f * direction2;
						WorldPosition worldPosition4 = this._attRightRanged.CreateNewOrderWorldPosition(0);
						worldPosition4.SetVec2(vec5);
						this._attRightCav.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition4));
						this._attLeftInf.SetMovementOrder(MovementOrder.MovementOrderCharge);
						this._attRightInf.SetMovementOrder(MovementOrder.MovementOrderCharge);
						this._defLeftBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack:
						this._defLeftInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defMidBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._defRightBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
						this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
						this._attLeftInf.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, this._defRightInf.GetAveragePositionOfUnits(true, false).ToVec3(0f))));
						this._attRightInf.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, this._defLeftInf.GetAveragePositionOfUnits(true, false).ToVec3(0f))));
						break;
					case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance:
					{
						Vec3 vec6 = this._attLeftRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - 0.15f * (this._attLeftRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - this._defRightInf.GetAveragePositionOfUnits(true, false).ToVec3(0f));
						Vec3 vec7 = this._attRightRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - 0.15f * (this._attRightRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - this._defLeftInf.GetAveragePositionOfUnits(true, false).ToVec3(0f));
						this._attLeftRanged.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec6)));
						this._attRightRanged.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec7)));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance:
					{
						base.Mission.Scene.FindEntityWithTag("attacker_mid").GetGlobalFrame();
						MatrixFrame globalFrame2 = base.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
						base.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
						Vec3 vec8 = globalFrame2.origin + globalFrame2.rotation.s * 68f;
						vec8 += 10f * this._attLeftRanged.Direction.ToVec3(0f);
						this._attLeftCav.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec8)));
						this._defMidCav.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec8)));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge:
					{
						MatrixFrame globalFrame3 = base.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
						this._defLeftBInf.FacingOrder = FacingOrder.FacingOrderLookAtDirection((this._attRightCav.CurrentPosition - this._defLeftBInf.CurrentPosition).Normalized());
						this._defLeftBInf.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, globalFrame3.origin - globalFrame3.rotation.s * 10f)));
						this._attRightCav.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(this._defLeftBInf));
						this._attLeftCav.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(this._attLeftInf));
						this._defMidCav.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(this._attRightInf));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2:
						this._attRightCav.SetMovementOrder(MovementOrder.MovementOrderMove(this._defLeftBInf.CreateNewOrderWorldPosition(0)));
						this._attLeftRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._attLeftRanged.SetMovementOrder(MovementOrder.MovementOrderAdvance);
						this._attRightRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
						this._attRightRanged.SetMovementOrder(MovementOrder.MovementOrderAdvance);
						break;
					case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2:
					{
						Vec3 vec9 = this._attLeftRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - 0.15f * (this._attLeftRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - this._defRightInf.GetAveragePositionOfUnits(true, false).ToVec3(0f));
						Vec3 vec10 = this._attRightRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - 0.15f * (this._attRightRanged.GetAveragePositionOfUnits(true, false).ToVec3(0f) - this._defLeftInf.GetAveragePositionOfUnits(true, false).ToVec3(0f));
						this._attLeftRanged.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec9)));
						this._attRightRanged.SetMovementOrder(MovementOrder.MovementOrderMove(new WorldPosition(base.Mission.Scene, vec10)));
						break;
					}
					case CPUBenchmarkMissionLogic.BattlePhase.FullCharge:
						foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingEmpty)
						{
							if (formation.CountOfUnits > 0 && formation != this._attLeftRanged && formation != this._attRightRanged && formation != this._attRightCav)
							{
								formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
							}
						}
						break;
					}
					this._isCurPhaseInPlay = true;
					return;
				}
				switch (this._battlePhase)
				{
				case CPUBenchmarkMissionLogic.BattlePhase.ArrowShower:
					if (currentTime > 14f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.MeleePosition;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.MeleePosition:
					if (currentTime > 19f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos:
					if (currentTime > 19f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef:
					if (currentTime > 24f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition:
					if (currentTime > 74.5f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack:
					if (currentTime > 19f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance:
					if (currentTime > 60f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance:
					if (currentTime > 30f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge:
					if (currentTime > 92f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2:
					if (currentTime > 93f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2;
						return;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2:
					if (currentTime > 94f)
					{
						this._isCurPhaseInPlay = false;
						this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.FullCharge;
					}
					break;
				case CPUBenchmarkMissionLogic.BattlePhase.FullCharge:
					break;
				default:
					return;
				}
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cpu_benchmark_mission", "benchmark")]
		public static string CPUBenchmarkMission(List<string> strings)
		{
			CPUBenchmarkMissionLogic.OpenCPUBenchmarkMission("benchmark_battle_11");
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cpu_benchmark", "benchmark")]
		public static string CPUBenchmark(List<string> strings)
		{
			using (List<string>.Enumerator enumerator = strings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == "siege")
					{
						CPUBenchmarkMissionLogic._isSiege = true;
					}
				}
			}
			MBGameManager.StartNewGame(new CustomGameManager());
			return "";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("benchmark_start", "state_string")]
		public static string BenchmarkStateStart(List<string> strings)
		{
			GameState activeState = GameStateManager.Current.ActiveState;
			if (activeState is InitialState)
			{
				MBGameManager.StartNewGame(new CustomGameManager());
			}
			else if (activeState is CustomBattleState)
			{
				GameStateManager.StateActivateCommand = "state_string.benchmark_end";
				if (!CPUBenchmarkMissionLogic._isSiege)
				{
					CPUBenchmarkMissionLogic.OpenCPUBenchmarkMission("benchmark_battle_11");
				}
				else
				{
					CPUBenchmarkMissionLogic.OpenCPUBenchmarkMission("benchmark_siege");
				}
			}
			return "";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("benchmark_end", "state_string")]
		public static string BenchmarkStateEnd(List<string> strings)
		{
			if (GameStateManager.Current.ActiveState is CustomBattleState)
			{
				GameStateManager.StateActivateCommand = null;
				Game.Current.GameStateManager.PopState(0);
			}
			return "";
		}

		public static Mission OpenCPUBenchmarkMission(string scene)
		{
			int realBattleSize = BannerlordConfig.GetRealBattleSize();
			IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>("empire");
			Banner banner = new Banner("11.4.124.4345.4345.768.768.1.0.0.163.0.5.512.512.769.764.1.0.0");
			Banner banner2 = new Banner("11.45.126.4345.4345.768.768.1.0.0.462.0.13.512.512.769.764.1.0.0");
			CustomBattleCombatant playerParty = new CustomBattleCombatant(new TextObject("{=!}Player Party", null), @object, banner);
			CustomBattleCombatant enemyParty = new CustomBattleCombatant(new TextObject("{=!}Enemy Party", null), @object, banner2);
			if (!CPUBenchmarkMissionLogic._isSiege)
			{
				int attackerInfCount = realBattleSize / 100 * 18;
				int attackerRangedCount = realBattleSize / 100 * 10;
				int attackerCavCount = realBattleSize / 100 * 8;
				int defenderInfCount = realBattleSize / 100 * 59;
				int defenderCavCount = realBattleSize / 100 * 5;
				playerParty.Side = 1;
				playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_legionary"), attackerInfCount);
				playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_palatine_guard"), attackerRangedCount);
				playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_cataphract"), attackerCavCount);
				enemyParty.Side = 0;
				enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_wildling"), defenderInfCount);
				enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_horseman"), defenderCavCount);
				CustomBattleTroopSupplier customBattleTroopSupplier = new CustomBattleTroopSupplier(playerParty, true, false, false, null);
				troopSuppliers[playerParty.Side] = customBattleTroopSupplier;
				CustomBattleTroopSupplier customBattleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false, false, false, null);
				troopSuppliers[enemyParty.Side] = customBattleTroopSupplier2;
				string text = "CPUBenchmarkMission";
				MissionInitializerRecord missionInitializerRecord;
				missionInitializerRecord..ctor(scene);
				missionInitializerRecord.DoNotUseLoadingScreen = false;
				missionInitializerRecord.PlayingInCampaignMode = false;
				return MissionState.OpenNew(text, missionInitializerRecord, (Mission missionController) => new MissionBehavior[]
				{
					new MissionCombatantsLogic(null, playerParty, enemyParty, playerParty, 1, false),
					new MissionAgentSpawnLogic(troopSuppliers, 1, 0),
					new BattlePowerCalculationLogic(),
					new CPUBenchmarkMissionSpawnHandler(enemyParty, playerParty),
					new CPUBenchmarkMissionLogic(attackerInfCount, attackerRangedCount, attackerCavCount, defenderInfCount, defenderCavCount),
					new AgentHumanAILogic(),
					new AgentVictoryLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler()
				}, true, true);
			}
			int num = realBattleSize / 100 * 30;
			int num2 = realBattleSize / 100 * 25;
			int num3 = realBattleSize / 100 * 20;
			int num4 = realBattleSize / 100 * 25;
			playerParty.Side = 1;
			playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("commander_1"), 1);
			playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_legionary"), num);
			playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_palatine_guard"), num2);
			enemyParty.Side = 0;
			enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("commander_2"), 1);
			enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_wildling"), num3);
			enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_militia_archer"), num4);
			CustomBattleTroopSupplier customBattleTroopSupplier3 = new CustomBattleTroopSupplier(playerParty, true, false, false, null);
			troopSuppliers[playerParty.Side] = customBattleTroopSupplier3;
			CustomBattleTroopSupplier customBattleTroopSupplier4 = new CustomBattleTroopSupplier(enemyParty, false, false, false, null);
			troopSuppliers[enemyParty.Side] = customBattleTroopSupplier4;
			SiegeEngineType object2 = MBObjectManager.Instance.GetObject<SiegeEngineType>("fire_ballista");
			MBObjectManager.Instance.GetObject<SiegeEngineType>("fire_onager");
			MBObjectManager.Instance.GetObject<SiegeEngineType>("fire_catapult");
			SiegeEngineType object3 = MBObjectManager.Instance.GetObject<SiegeEngineType>("trebuchet");
			SiegeEngineType object4 = MBObjectManager.Instance.GetObject<SiegeEngineType>("ram");
			SiegeEngineType object5 = MBObjectManager.Instance.GetObject<SiegeEngineType>("siege_tower_level2");
			List<MissionSiegeWeapon> list = new List<MissionSiegeWeapon>();
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object3));
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object3));
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object5));
			list.Add(MissionSiegeWeapon.CreateDefaultWeapon(object4));
			List<MissionSiegeWeapon> list2 = new List<MissionSiegeWeapon>();
			list2.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			list2.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			list2.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			list2.Add(MissionSiegeWeapon.CreateDefaultWeapon(object2));
			float[] array = new float[] { 1f, 1f };
			Mission mission = BannerlordMissions.OpenSiegeMissionWithDeployment(scene, MBObjectManager.Instance.GetObject<BasicCharacterObject>("commander_1"), playerParty, enemyParty, true, array, true, list, list2, true, 3, "", false, false, 6f);
			mission.AddMissionBehavior(new CPUBenchmarkMissionLogic(num, num2, 0, num3, 0));
			return mission;
		}

		private const float FormationDistDiff = 20f;

		private const float PressTimeForExit = 0.05f;

		private const float ResultTime = 9f;

		private readonly int _attackerInfCount;

		private readonly int _attackerRangedCount;

		private readonly int _attackerCavCount;

		private readonly int _defenderInfCount;

		private readonly int _defenderCavCount;

		private int _curPath;

		private float _benchmarkExit;

		private bool _benchmarkFinished;

		private static bool _isSiege;

		private float _showResultTime = 92f;

		private Path[] _paths;

		private Path[] _targets;

		private float _cameraSpeed;

		private float _curPathSpeed;

		private float _curPathLenght;

		private float _nextPathSpeed;

		private float _prevPathSpeed;

		private float _cameraPassedDistanceOnPath;

		private MissionAgentSpawnLogic _missionAgentSpawnLogic;

		private bool _formationsSetUp;

		private Formation _defLeftInf;

		private Formation _defMidCav;

		private Formation _defRightInf;

		private Formation _defLeftBInf;

		private Formation _defMidBInf;

		private Formation _defRightBInf;

		private Formation _attLeftInf;

		private Formation _attRightInf;

		private Formation _attLeftRanged;

		private Formation _attRightRanged;

		private Formation _attLeftCav;

		private Formation _attRightCav;

		private Camera _benchmarkCamera;

		private CPUBenchmarkMissionLogic.BattlePhase _battlePhase;

		private bool _isCurPhaseInPlay;

		private float _totalTime;

		private bool _benchmarkStarted;

		private delegate void MainThreadJobDelegate();

		private enum BattlePhase
		{
			Start,
			ArrowShower,
			MeleePosition,
			Cav1Pos,
			Cav1PosDef,
			CavalryPosition,
			MeleeAttack,
			RangedAdvance,
			CavalryAdvance,
			CavalryCharge,
			CavalryCharge2,
			RangedAdvance2,
			FullCharge
		}

		private enum BenchmarkStatus
		{
			Inactive,
			Active,
			Result,
			SetDefinition
		}
	}
}

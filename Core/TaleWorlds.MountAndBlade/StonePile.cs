using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class StonePile : UsableMachine, IDetachment
	{
		public int AmmoCount { get; protected set; }

		public bool HasThrowingPointUsed
		{
			get
			{
				foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
				{
					if (throwingPoint.StandingPoint.HasUser || throwingPoint.StandingPoint.HasAIMovingTo || (throwingPoint.WaitingPoint != null && (throwingPoint.WaitingPoint.HasUser || throwingPoint.WaitingPoint.HasAIMovingTo)))
					{
						return true;
					}
				}
				return false;
			}
		}

		public virtual BattleSideEnum Side
		{
			get
			{
				return BattleSideEnum.Defender;
			}
		}

		public override int MaxUserCount
		{
			get
			{
				return this._throwingPoints.Count;
			}
		}

		protected StonePile()
		{
		}

		protected void ConsumeAmmo()
		{
			int ammoCount = this.AmmoCount;
			this.AmmoCount = ammoCount - 1;
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetStonePileAmmo(base.Id, this.AmmoCount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.UpdateAmmoMesh();
			this.CheckAmmo();
		}

		public void SetAmmo(int ammoLeft)
		{
			if (this.AmmoCount != ammoLeft)
			{
				this.AmmoCount = ammoLeft;
				this.UpdateAmmoMesh();
				this.CheckAmmo();
			}
		}

		protected virtual void CheckAmmo()
		{
			if (this.AmmoCount <= 0)
			{
				foreach (StandingPoint standingPoint in base.AmmoPickUpPoints)
				{
					standingPoint.IsDeactivated = true;
				}
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._tickOccasionallyTimer = new Timer(Mission.Current.CurrentTime, 0.5f + MBRandom.RandomFloat * 0.5f, true);
			this._givenItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.GivenItemID);
			MBList<VolumeBox> mblist = base.GameEntity.CollectObjects<VolumeBox>();
			this._throwingPoints = new List<StonePile.ThrowingPoint>();
			this._volumeBoxTimerPairs = new List<StonePile.VolumeBoxTimerPair>();
			foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in base.StandingPoints.OfType<StandingPointWithWeaponRequirement>())
			{
				if (standingPointWithWeaponRequirement.GameEntity.HasTag(this.AmmoPickUpTag))
				{
					standingPointWithWeaponRequirement.InitGivenWeapon(this._givenItem);
					standingPointWithWeaponRequirement.SetHasAlternative(true);
					standingPointWithWeaponRequirement.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
				}
				else if (standingPointWithWeaponRequirement.GameEntity.HasTag("throwing"))
				{
					standingPointWithWeaponRequirement.InitRequiredWeapon(this._givenItem);
					StonePile.ThrowingPoint throwingPoint = new StonePile.ThrowingPoint();
					throwingPoint.StandingPoint = standingPointWithWeaponRequirement as StandingPointWithVolumeBox;
					throwingPoint.AmmoPickUpPoint = null;
					throwingPoint.AttackEntity = null;
					throwingPoint.AttackEntityNearbyAgentsCheckRadius = 0f;
					List<StandingPointWithWeaponRequirement> list = standingPointWithWeaponRequirement.GameEntity.CollectObjectsWithTag("wait_to_throw");
					if (list != null && list.Count > 0)
					{
						throwingPoint.WaitingPoint = list[0];
						throwingPoint.WaitingPoint.InitRequiredWeapon(this._givenItem);
					}
					else
					{
						throwingPoint.WaitingPoint = null;
					}
					bool flag = false;
					int num = 0;
					while (num < this._volumeBoxTimerPairs.Count && !flag)
					{
						if (this._volumeBoxTimerPairs[num].VolumeBox.GameEntity.HasTag(throwingPoint.StandingPoint.VolumeBoxTag))
						{
							throwingPoint.EnemyInRangeTimer = this._volumeBoxTimerPairs[num].Timer;
							flag = true;
						}
						num++;
					}
					if (!flag)
					{
						VolumeBox volumeBox = mblist.FirstOrDefault((VolumeBox vb) => vb.GameEntity.HasTag(throwingPoint.StandingPoint.VolumeBoxTag));
						StonePile.VolumeBoxTimerPair volumeBoxTimerPair = default(StonePile.VolumeBoxTimerPair);
						volumeBoxTimerPair.VolumeBox = volumeBox;
						volumeBoxTimerPair.Timer = new Timer(-3.5f, 0.5f, false);
						throwingPoint.EnemyInRangeTimer = volumeBoxTimerPair.Timer;
						this._volumeBoxTimerPairs.Add(volumeBoxTimerPair);
					}
					this._throwingPoints.Add(throwingPoint);
				}
			}
			this.EnemyRangeToStopUsing = 5f;
			this.AmmoCount = this.StartingAmmoCount;
			this.UpdateAmmoMesh();
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this._throwingTargets = base.Scene.FindEntitiesWithTag("throwing_target").ToList<GameEntity>();
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this.AmmoCount = this.StartingAmmoCount;
			this.UpdateAmmoMesh();
			foreach (StandingPoint standingPoint in base.AmmoPickUpPoints)
			{
				standingPoint.IsDeactivated = false;
			}
			foreach (StonePile.VolumeBoxTimerPair volumeBoxTimerPair in this._volumeBoxTimerPairs)
			{
				volumeBoxTimerPair.Timer.Reset(-3.5f);
			}
			foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
			{
				throwingPoint.AmmoPickUpPoint = null;
			}
		}

		public override void AfterMissionStart()
		{
			if (base.AmmoPickUpPoints != null)
			{
				foreach (StandingPoint standingPoint in base.AmmoPickUpPoints)
				{
					standingPoint.LockUserFrames = true;
				}
			}
			if (this._throwingPoints != null)
			{
				foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
				{
					throwingPoint.StandingPoint.IsDisabledForPlayers = true;
					throwingPoint.StandingPoint.LockUserFrames = false;
					throwingPoint.StandingPoint.LockUserPositions = true;
				}
			}
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			if (usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
			{
				TextObject textObject = new TextObject("{=jfcceEoE}{PILE_TYPE} Pile", null);
				textObject.SetTextVariable("PILE_TYPE", new TextObject("{=1CPdu9K0}Stone", null));
				return textObject;
			}
			return TextObject.Empty;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (gameEntity.HasTag(this.AmmoPickUpTag))
			{
				TextObject textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
				textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
				return textObject.ToString();
			}
			return string.Empty;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new StonePileAI(this);
		}

		public override bool IsInRangeToCheckAlternativePoints(Agent agent)
		{
			float num = ((base.StandingPoints.Count > 0) ? (agent.GetInteractionDistanceToUsable(base.StandingPoints[0]) + 2f) : 2f);
			return base.GameEntity.GlobalPosition.DistanceSquared(agent.Position) < num * num;
		}

		public override StandingPoint GetBestPointAlternativeTo(StandingPoint standingPoint, Agent agent)
		{
			if (base.AmmoPickUpPoints.Contains(standingPoint))
			{
				float num = standingPoint.GameEntity.GlobalPosition.DistanceSquared(agent.Position);
				StandingPoint standingPoint2 = standingPoint;
				foreach (StandingPoint standingPoint3 in base.AmmoPickUpPoints)
				{
					float num2 = standingPoint3.GameEntity.GlobalPosition.DistanceSquared(agent.Position);
					if (num2 < num && ((!standingPoint3.HasUser && !standingPoint3.HasAIMovingTo) || standingPoint3.IsInstantUse) && !standingPoint3.IsDeactivated && !standingPoint3.IsDisabledForAgent(agent))
					{
						num = num2;
						standingPoint2 = standingPoint3;
					}
				}
				return standingPoint2;
			}
			return standingPoint;
		}

		private void TickOccasionally()
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				if (this.AmmoCount <= 0 && !this.HasThrowingPointUsed)
				{
					this.ReleaseAllUserAgentsAndFormations(BattleSideEnum.None, true);
					return;
				}
				if (this.IsDisabledForBattleSideAI(this.Side))
				{
					this.ReleaseAllUserAgentsAndFormations(this.Side, false);
					return;
				}
				bool flag = this._volumeBoxTimerPairs.Count == 0;
				foreach (StonePile.VolumeBoxTimerPair volumeBoxTimerPair in this._volumeBoxTimerPairs)
				{
					if (volumeBoxTimerPair.VolumeBox.HasAgentsInAttackerSide())
					{
						flag = true;
						if (volumeBoxTimerPair.Timer.ElapsedTime() > 3.5f)
						{
							volumeBoxTimerPair.Timer.Reset(Mission.Current.CurrentTime);
						}
						else
						{
							volumeBoxTimerPair.Timer.Reset(Mission.Current.CurrentTime - 0.5f);
						}
					}
				}
				MBReadOnlyList<Formation> userFormations = base.UserFormations;
				if (flag && userFormations.CountQ((Formation f) => f.Team.Side == this.Side) == 0)
				{
					float minDistanceSquared = float.MaxValue;
					Formation bestFormation = null;
					foreach (Team team in Mission.Current.Teams)
					{
						if (team.Side == this.Side)
						{
							using (List<Formation>.Enumerator enumerator3 = team.FormationsIncludingEmpty.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									Formation formation = enumerator3.Current;
									if (formation.CountOfUnits > 0 && formation.CountOfUnitsWithoutLooseDetachedOnes >= this.MaxUserCount && formation.CountOfUnitsWithoutLooseDetachedOnes > 0)
									{
										formation.ApplyActionOnEachUnit(delegate(Agent agent)
										{
											float num = agent.Position.DistanceSquared(this.GameEntity.GlobalPosition);
											if (minDistanceSquared > num)
											{
												minDistanceSquared = num;
												bestFormation = formation;
											}
										}, null);
									}
								}
							}
						}
					}
					Formation bestFormation2 = bestFormation;
					if (bestFormation2 == null)
					{
						return;
					}
					bestFormation2.StartUsingMachine(this, false);
					return;
				}
				else if (!flag)
				{
					if (userFormations.Count > 0)
					{
						this.ReleaseAllUserAgentsAndFormations(BattleSideEnum.None, true);
						return;
					}
				}
				else
				{
					if (userFormations.All((Formation f) => f.Team.Side == this.Side && f.UnitsWithoutLooseDetachedOnes.Count == 0))
					{
						if (base.StandingPoints.Count((StandingPoint sp) => sp.HasUser || sp.HasAIMovingTo) == 0)
						{
							this.ReleaseAllUserAgentsAndFormations(BattleSideEnum.None, true);
							return;
						}
					}
					this.UpdateThrowingPointAttackEntities();
				}
			}
		}

		private void ReleaseAllUserAgentsAndFormations(BattleSideEnum sideFilterForAIControlledAgents, bool disableForNonAIControlledAgents)
		{
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				Agent agent = (standingPoint.HasUser ? standingPoint.UserAgent : (standingPoint.HasAIMovingTo ? standingPoint.MovingAgent : null));
				if (agent != null)
				{
					if (agent.IsAIControlled)
					{
						if (sideFilterForAIControlledAgents == BattleSideEnum.None)
						{
							goto IL_6E;
						}
						Team team = agent.Team;
						if (team != null && team.Side == sideFilterForAIControlledAgents)
						{
							goto IL_6E;
						}
					}
					if (agent.IsAIControlled || !disableForNonAIControlledAgents)
					{
						continue;
					}
					IL_6E:
					if (agent.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.ExtraWeaponSlot && agent.Equipment[EquipmentIndex.ExtraWeaponSlot].Item == this._givenItem)
					{
						agent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
					}
					base.Ai.StopUsingStandingPoint(standingPoint);
				}
			}
			MBReadOnlyList<Formation> userFormations = base.UserFormations;
			for (int i = userFormations.Count - 1; i >= 0; i--)
			{
				Formation formation = userFormations[i];
				if (formation.Team.Side == this.Side)
				{
					formation.StopUsingMachine(this, false);
				}
			}
		}

		private void UpdateThrowingPointAttackEntities()
		{
			bool flag = false;
			List<GameEntity> list = null;
			foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
			{
				if (throwingPoint.StandingPoint.HasAIUser)
				{
					if (!flag)
					{
						list = this.GetEnemySiegeWeapons();
						flag = true;
						if (list == null)
						{
							foreach (StonePile.ThrowingPoint throwingPoint2 in this._throwingPoints)
							{
								throwingPoint2.AttackEntity = null;
								throwingPoint2.AttackEntityNearbyAgentsCheckRadius = 0f;
							}
							if (this._throwingTargets.Count == 0)
							{
								break;
							}
						}
					}
					Agent userAgent = throwingPoint.StandingPoint.UserAgent;
					GameEntity attackEntity = throwingPoint.AttackEntity;
					if (attackEntity != null)
					{
						bool flag2 = false;
						if (!this.CanShootAtEntity(userAgent, attackEntity, false))
						{
							flag2 = true;
						}
						else if (this._throwingTargets.Contains(attackEntity))
						{
							flag2 = !throwingPoint.CanUseAttackEntity();
						}
						else if (!list.Contains(attackEntity))
						{
							flag2 = true;
						}
						if (flag2)
						{
							throwingPoint.AttackEntity = null;
							throwingPoint.AttackEntityNearbyAgentsCheckRadius = 0f;
						}
					}
					if (!(throwingPoint.AttackEntity == null))
					{
						continue;
					}
					bool flag3 = false;
					if (this._throwingTargets.Count > 0)
					{
						foreach (GameEntity gameEntity in this._throwingTargets)
						{
							if (attackEntity != gameEntity && this.CanShootAtEntity(userAgent, gameEntity, true))
							{
								throwingPoint.AttackEntity = gameEntity;
								throwingPoint.AttackEntityNearbyAgentsCheckRadius = 1.31f;
								flag3 = true;
								break;
							}
						}
					}
					if (flag3 || list == null)
					{
						continue;
					}
					using (List<GameEntity>.Enumerator enumerator3 = list.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							GameEntity gameEntity2 = enumerator3.Current;
							if (attackEntity != gameEntity2 && this.CanShootAtEntity(userAgent, gameEntity2, false))
							{
								throwingPoint.AttackEntity = gameEntity2;
								throwingPoint.AttackEntityNearbyAgentsCheckRadius = 0f;
								break;
							}
						}
						continue;
					}
				}
				throwingPoint.AttackEntity = null;
			}
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!GameNetwork.IsClientOrReplay && this._tickOccasionallyTimer.Check(Mission.Current.CurrentTime))
			{
				this.TickOccasionally();
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				StandingPoint.StackArray8StandingPoint stackArray8StandingPoint = default(StandingPoint.StackArray8StandingPoint);
				int num = 0;
				Agent.StackArray8Agent stackArray8Agent = default(Agent.StackArray8Agent);
				int num2 = 0;
				foreach (StandingPoint standingPoint in base.AmmoPickUpPoints)
				{
					if (standingPoint.HasUser)
					{
						ActionIndexValueCache currentActionValue = standingPoint.UserAgent.GetCurrentActionValue(1);
						if (!(currentActionValue == StonePile.act_pickup_boulder_begin))
						{
							if (currentActionValue == StonePile.act_pickup_boulder_end)
							{
								MissionWeapon missionWeapon = new MissionWeapon(this._givenItem, null, null, 1);
								Agent userAgent = standingPoint.UserAgent;
								userAgent.EquipWeaponToExtraSlotAndWield(ref missionWeapon);
								base.Ai.StopUsingStandingPoint(standingPoint);
								this.ConsumeAmmo();
								if (userAgent.IsAIControlled)
								{
									stackArray8Agent[num2++] = userAgent;
								}
							}
							else if (!standingPoint.UserAgent.SetActionChannel(1, StonePile.act_pickup_boulder_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
							{
								base.Ai.StopUsingStandingPoint(standingPoint);
							}
						}
					}
					if (standingPoint.HasAIUser || standingPoint.HasAIMovingTo)
					{
						stackArray8StandingPoint[num++] = standingPoint;
					}
				}
				StonePile.ThrowingPoint.StackArray8ThrowingPoint stackArray8ThrowingPoint = default(StonePile.ThrowingPoint.StackArray8ThrowingPoint);
				int num3 = 0;
				foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
				{
					throwingPoint.AmmoPickUpPoint = null;
					if (throwingPoint.AttackEntity != null || (throwingPoint.EnemyInRangeTimer.Check(Mission.Current.CurrentTime) && throwingPoint.EnemyInRangeTimer.ElapsedTime() < 3.5f))
					{
						if (!this.UpdateThrowingPointIfHasAnyInteractingAgent(throwingPoint))
						{
							stackArray8ThrowingPoint[num3++] = throwingPoint;
						}
					}
					else
					{
						throwingPoint.StandingPoint.IsDeactivated = true;
						if (throwingPoint.WaitingPoint != null)
						{
							throwingPoint.WaitingPoint.IsDeactivated = true;
						}
					}
				}
				for (int i = 0; i < num; i++)
				{
					if (num3 > i)
					{
						StandingPointWithWeaponRequirement standingPointWithWeaponRequirement = stackArray8StandingPoint[i] as StandingPointWithWeaponRequirement;
						stackArray8ThrowingPoint[i].AmmoPickUpPoint = standingPointWithWeaponRequirement;
					}
					else if (stackArray8StandingPoint[i].HasUser || stackArray8StandingPoint[i].HasAIMovingTo)
					{
						base.Ai.StopUsingStandingPoint(stackArray8StandingPoint[i]);
					}
				}
				for (int j = 0; j < num2; j++)
				{
					Agent agent = stackArray8Agent[j];
					StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(this.Side, agent, null, null);
					this.AssignAgentToStandingPoint(suitableStandingPointFor, agent);
				}
			}
		}

		private bool ShouldStandAtWaitingPoint(StonePile.ThrowingPoint throwingPoint)
		{
			bool flag = false;
			if (throwingPoint.WaitingPoint != null)
			{
				flag = true;
				Vec2 asVec = throwingPoint.StandingPoint.GameEntity.GlobalPosition.AsVec2;
				if (AgentProximityMap.CanSearchRadius(this._givenItemRange))
				{
					AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, asVec, this._givenItemRange, false);
					while (proximityMapSearchStruct.LastFoundAgent != null)
					{
						if (proximityMapSearchStruct.LastFoundAgent.State == AgentState.Active && proximityMapSearchStruct.LastFoundAgent.Team != null && proximityMapSearchStruct.LastFoundAgent.Team.Side == BattleSideEnum.Attacker)
						{
							flag = false;
							break;
						}
						AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
					}
				}
				else
				{
					float num = this._givenItemRange * this._givenItemRange;
					if (Mission.Current.AttackerTeam != null)
					{
						MBReadOnlyList<Agent> activeAgents = Mission.Current.AttackerTeam.ActiveAgents;
						int count = activeAgents.Count;
						for (int i = 0; i < count; i++)
						{
							if (activeAgents[i].Position.AsVec2.DistanceSquared(asVec) <= num)
							{
								flag = false;
								break;
							}
						}
					}
					if (Mission.Current.AttackerAllyTeam != null)
					{
						MBReadOnlyList<Agent> activeAgents2 = Mission.Current.AttackerAllyTeam.ActiveAgents;
						int count2 = activeAgents2.Count;
						for (int j = 0; j < count2; j++)
						{
							if (activeAgents2[j].Position.AsVec2.DistanceSquared(asVec) <= num)
							{
								flag = true;
								break;
							}
						}
					}
				}
			}
			return flag;
		}

		private bool UpdateThrowingPointIfHasAnyInteractingAgent(StonePile.ThrowingPoint throwingPoint)
		{
			Agent agent = null;
			StandingPoint standingPoint = null;
			throwingPoint.StandingPoint.IsDeactivated = false;
			if (throwingPoint.StandingPoint.HasAIMovingTo)
			{
				agent = throwingPoint.StandingPoint.MovingAgent;
				standingPoint = throwingPoint.StandingPoint;
			}
			else if (throwingPoint.StandingPoint.HasUser)
			{
				agent = throwingPoint.StandingPoint.UserAgent;
				standingPoint = throwingPoint.StandingPoint;
			}
			if (throwingPoint.WaitingPoint != null)
			{
				throwingPoint.WaitingPoint.IsDeactivated = false;
				if (throwingPoint.WaitingPoint.HasAIMovingTo)
				{
					agent = throwingPoint.WaitingPoint.MovingAgent;
					standingPoint = throwingPoint.WaitingPoint;
				}
				else if (throwingPoint.WaitingPoint.HasUser)
				{
					agent = throwingPoint.WaitingPoint.UserAgent;
					standingPoint = throwingPoint.WaitingPoint;
				}
			}
			bool flag = agent != null;
			if (flag && agent.Controller == Agent.ControllerType.AI)
			{
				EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex == EquipmentIndex.None || agent.Equipment[wieldedItemIndex].Item != this._givenItem)
				{
					base.Ai.StopUsingStandingPoint(standingPoint);
					throwingPoint.AttackEntity = null;
					return flag;
				}
				if (standingPoint == throwingPoint.WaitingPoint)
				{
					if (!this.ShouldStandAtWaitingPoint(throwingPoint))
					{
						base.Ai.StopUsingStandingPoint(standingPoint);
						this.AssignAgentToStandingPoint(throwingPoint.StandingPoint, agent);
						return flag;
					}
				}
				else if (agent.IsUsingGameObject && throwingPoint.AttackEntity != null)
				{
					if (throwingPoint.CanUseAttackEntity())
					{
						agent.SetScriptedTargetEntityAndPosition(throwingPoint.AttackEntity, new WorldPosition(throwingPoint.AttackEntity.Scene, UIntPtr.Zero, throwingPoint.AttackEntity.GlobalPosition, false), Agent.AISpecialCombatModeFlags.None, true);
						return flag;
					}
					agent.DisableScriptedCombatMovement();
					throwingPoint.AttackEntity = null;
					return flag;
				}
				else if (this.ShouldStandAtWaitingPoint(throwingPoint))
				{
					base.Ai.StopUsingStandingPoint(standingPoint);
					this.AssignAgentToStandingPoint(throwingPoint.WaitingPoint, agent);
				}
			}
			return flag;
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		float? IDetachment.GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> candidates, out Agent match)
		{
			BattleSideEnum side = candidates[0].Item1.Team.Side;
			StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(side, null, null, candidates);
			if (suitableStandingPointFor == null)
			{
				match = null;
				return null;
			}
			float? weightOfNextSlot = ((IDetachment)this).GetWeightOfNextSlot(side);
			match = StonePileAI.GetSuitableAgentForStandingPoint(this, suitableStandingPointFor, candidates, new List<Agent>(), weightOfNextSlot.Value);
			if (match == null)
			{
				return null;
			}
			float num = 1f;
			float? num2 = weightOfNextSlot;
			float num3 = num;
			if (num2 == null)
			{
				return null;
			}
			return new float?(num2.GetValueOrDefault() * num3);
		}

		float? IDetachment.GetWeightOfAgentAtNextSlot(List<Agent> candidates, out Agent match)
		{
			BattleSideEnum side = candidates[0].Team.Side;
			StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(side, null, candidates, null);
			if (suitableStandingPointFor == null)
			{
				match = null;
				return null;
			}
			match = StonePileAI.GetSuitableAgentForStandingPoint(this, suitableStandingPointFor, candidates, new List<Agent>());
			if (match == null)
			{
				return null;
			}
			float? weightOfNextSlot = ((IDetachment)this).GetWeightOfNextSlot(side);
			float num = 1f;
			float? num2 = weightOfNextSlot;
			float num3 = num;
			if (num2 == null)
			{
				return null;
			}
			return new float?(num2.GetValueOrDefault() * num3);
		}

		protected override StandingPoint GetSuitableStandingPointFor(BattleSideEnum side, Agent agent = null, List<Agent> agents = null, List<ValueTuple<Agent, float>> agentValuePairs = null)
		{
			List<Agent> list = new List<Agent>();
			if (agents == null)
			{
				if (agent != null)
				{
					list.Add(agent);
					goto IL_5A;
				}
				if (agentValuePairs == null)
				{
					goto IL_5A;
				}
				using (List<ValueTuple<Agent, float>>.Enumerator enumerator = agentValuePairs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ValueTuple<Agent, float> valueTuple = enumerator.Current;
						list.Add(valueTuple.Item1);
					}
					goto IL_5A;
				}
			}
			list.AddRange(agents);
			IL_5A:
			bool flag = false;
			bool flag2 = false;
			StandingPoint standingPoint = null;
			int num = 0;
			while (num < this._throwingPoints.Count && (standingPoint == null || flag2))
			{
				StonePile.ThrowingPoint throwingPoint = this._throwingPoints[num];
				if (this.IsThrowingPointAssignable(throwingPoint))
				{
					StandingPoint standingPoint2 = throwingPoint.StandingPoint;
					bool flag3 = this.ShouldStandAtWaitingPoint(throwingPoint);
					if (flag3)
					{
						standingPoint2 = throwingPoint.WaitingPoint;
					}
					bool flag4 = false;
					int num2 = 0;
					while (!flag4 && num2 < list.Count)
					{
						flag4 = !standingPoint2.IsDisabledForAgent(list[num2]);
						num2++;
					}
					if (flag4)
					{
						flag2 = flag3;
						standingPoint = standingPoint2;
					}
					else
					{
						flag = true;
					}
				}
				num++;
			}
			int num3 = 0;
			while (num3 < base.StandingPoints.Count && standingPoint == null)
			{
				StandingPoint standingPoint3 = base.StandingPoints[num3];
				if (!standingPoint3.IsDeactivated && (standingPoint3.IsInstantUse || (!standingPoint3.HasUser && !standingPoint3.HasAIMovingTo)) && !standingPoint3.GameEntity.HasTag("throwing") && !standingPoint3.GameEntity.HasTag("wait_to_throw") && (flag || !standingPoint3.GameEntity.HasTag(this.AmmoPickUpTag)))
				{
					int num4 = 0;
					while (num4 < list.Count && standingPoint == null)
					{
						if (!standingPoint3.IsDisabledForAgent(list[num4]))
						{
							standingPoint = standingPoint3;
						}
						num4++;
					}
					if (list.Count == 0)
					{
						standingPoint = standingPoint3;
					}
				}
				num3++;
			}
			return standingPoint;
		}

		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			if (this.IsDisabledForBattleSideAI(side))
			{
				return float.MinValue;
			}
			this._usableStandingPoints.Clear();
			int num = 0;
			foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
			{
				if (this.IsThrowingPointAssignable(throwingPoint))
				{
					num++;
				}
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < base.StandingPoints.Count; i++)
			{
				StandingPoint standingPoint = base.StandingPoints[i];
				if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag) && num > 0)
				{
					num--;
					if (standingPoint.IsUsableBySide(side))
					{
						if (!standingPoint.HasAIMovingTo)
						{
							if (!flag2)
							{
								this._usableStandingPoints.Clear();
							}
							flag2 = true;
						}
						else if (flag2 || standingPoint.MovingAgent.Formation.Team.Side != side)
						{
							goto IL_E8;
						}
						flag = true;
						this._usableStandingPoints.Add(new ValueTuple<int, StandingPoint>(i, standingPoint));
					}
				}
				IL_E8:;
			}
			this._areUsableStandingPointsVacant = flag2;
			if (!flag)
			{
				return float.MinValue;
			}
			if (flag2)
			{
				return 1f;
			}
			if (!this._isDetachmentRecentlyEvaluated)
			{
				return 0.1f;
			}
			return 0.01f;
		}

		protected virtual void UpdateAmmoMesh()
		{
			int num = 20 - this.AmmoCount;
			if (base.GameEntity != null)
			{
				for (int i = 0; i < base.GameEntity.MultiMeshComponentCount; i++)
				{
					MetaMesh metaMesh = base.GameEntity.GetMetaMesh(i);
					for (int j = 0; j < metaMesh.MeshCount; j++)
					{
						metaMesh.GetMeshAtIndex(j).SetVectorArgument(0f, (float)num, 0f, 0f);
					}
				}
			}
		}

		private bool CanShootAtEntity(Agent agent, GameEntity entity, bool canShootEvenIfRayCastHitsNothing = false)
		{
			bool flag = false;
			if (agent.GetEyeGlobalPosition().DistanceSquared(entity.GlobalPosition) < this._givenItemRange * this._givenItemRange)
			{
				float num;
				GameEntity parent;
				if (base.Scene.RayCastForClosestEntityOrTerrain(agent.GetEyeGlobalPosition(), entity.GlobalPosition, out num, out parent, 0.01f, BodyFlags.CommonFocusRayCastExcludeFlags))
				{
					while (parent != null)
					{
						if (parent == entity)
						{
							flag = true;
							break;
						}
						parent = parent.Parent;
					}
				}
				else
				{
					flag = canShootEvenIfRayCastHitsNothing;
				}
			}
			return flag;
		}

		private List<GameEntity> GetEnemySiegeWeapons()
		{
			List<GameEntity> list = null;
			if (Mission.Current.Teams.Attacker.TeamAI is TeamAISiegeComponent)
			{
				using (List<IPrimarySiegeWeapon>.Enumerator enumerator = ((TeamAISiegeComponent)Mission.Current.Teams.Attacker.TeamAI).PrimarySiegeWeapons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SiegeWeapon siegeWeapon;
						if ((siegeWeapon = enumerator.Current as SiegeWeapon) != null && siegeWeapon.GameEntity.GetFirstScriptOfType<DestructableComponent>() != null && siegeWeapon.IsUsed)
						{
							if (list == null)
							{
								list = new List<GameEntity>();
							}
							list.Add(siegeWeapon.GameEntity);
						}
					}
				}
			}
			return list;
		}

		private bool IsThrowingPointAssignable(StonePile.ThrowingPoint throwingPoint)
		{
			return throwingPoint.AmmoPickUpPoint == null && !throwingPoint.StandingPoint.IsDeactivated && !throwingPoint.StandingPoint.HasUser && !throwingPoint.StandingPoint.HasAIMovingTo && (throwingPoint.WaitingPoint == null || (!throwingPoint.WaitingPoint.IsDeactivated && !throwingPoint.WaitingPoint.HasUser && !throwingPoint.WaitingPoint.HasAIMovingTo));
		}

		private bool AssignAgentToStandingPoint(StandingPoint standingPoint, Agent agent)
		{
			if (standingPoint == null || agent == null || !StonePileAI.IsAgentAssignable(agent))
			{
				return false;
			}
			int num = base.StandingPoints.IndexOf(standingPoint);
			if (num >= 0)
			{
				((IDetachment)this).AddAgent(agent, num);
				if (agent.Formation != null)
				{
					agent.Formation.DetachUnit(agent, ((IDetachment)this).IsLoose);
					agent.Detachment = this;
					agent.DetachmentWeight = this.GetWeightOfStandingPoint(standingPoint);
					return true;
				}
			}
			return false;
		}

		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		private const string ThrowingTargetTag = "throwing_target";

		private const string ThrowingPointTag = "throwing";

		private const string WaitingPointTag = "wait_to_throw";

		private const float EnemyInRangeTimerDuration = 0.5f;

		private const float EnemyWaitTimeLimit = 3f;

		private const float ThrowingTargetRadius = 1.31f;

		public int StartingAmmoCount = 12;

		public string GivenItemID;

		[EditableScriptComponentVariable(true)]
		private float _givenItemRange = 15f;

		private ItemObject _givenItem;

		private List<GameEntity> _throwingTargets;

		private List<StonePile.ThrowingPoint> _throwingPoints;

		private List<StonePile.VolumeBoxTimerPair> _volumeBoxTimerPairs;

		private Timer _tickOccasionallyTimer;

		[DefineSynchedMissionObjectType(typeof(StonePile))]
		public struct StonePileRecord : ISynchedMissionObjectReadableRecord
		{
			public int ReadAmmoCount { get; private set; }

			public StonePileRecord(int readAmmoCount)
			{
				this.ReadAmmoCount = readAmmoCount;
			}

			public bool ReadFromNetwork(ref bool bufferReadValid)
			{
				this.ReadAmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref bufferReadValid);
				return bufferReadValid;
			}
		}

		private class ThrowingPoint
		{
			public bool CanUseAttackEntity()
			{
				bool flag = true;
				if (this.AttackEntityNearbyAgentsCheckRadius > 0f)
				{
					float currentTime = Mission.Current.CurrentTime;
					if (currentTime >= this._cachedCanUseAttackEntityExpireTime)
					{
						this._cachedCanUseAttackEntity = Mission.Current.HasAnyAgentsOfSideInRange(this.AttackEntity.GlobalPosition, this.AttackEntityNearbyAgentsCheckRadius, BattleSideEnum.Attacker);
						this._cachedCanUseAttackEntityExpireTime = currentTime + 1f;
					}
					flag = this._cachedCanUseAttackEntity;
				}
				return flag;
			}

			private const float CachedCanUseAttackEntityUpdateInterval = 1f;

			public StandingPointWithVolumeBox StandingPoint;

			public StandingPointWithWeaponRequirement AmmoPickUpPoint;

			public StandingPointWithWeaponRequirement WaitingPoint;

			public Timer EnemyInRangeTimer;

			public GameEntity AttackEntity;

			public float AttackEntityNearbyAgentsCheckRadius;

			private float _cachedCanUseAttackEntityExpireTime;

			private bool _cachedCanUseAttackEntity;

			public struct StackArray8ThrowingPoint
			{
				public StonePile.ThrowingPoint this[int index]
				{
					get
					{
						switch (index)
						{
						case 0:
							return this._element0;
						case 1:
							return this._element1;
						case 2:
							return this._element2;
						case 3:
							return this._element3;
						case 4:
							return this._element4;
						case 5:
							return this._element5;
						case 6:
							return this._element6;
						case 7:
							return this._element7;
						default:
							return null;
						}
					}
					set
					{
						switch (index)
						{
						case 0:
							this._element0 = value;
							return;
						case 1:
							this._element1 = value;
							return;
						case 2:
							this._element2 = value;
							return;
						case 3:
							this._element3 = value;
							return;
						case 4:
							this._element4 = value;
							return;
						case 5:
							this._element5 = value;
							return;
						case 6:
							this._element6 = value;
							return;
						case 7:
							this._element7 = value;
							return;
						default:
							return;
						}
					}
				}

				private StonePile.ThrowingPoint _element0;

				private StonePile.ThrowingPoint _element1;

				private StonePile.ThrowingPoint _element2;

				private StonePile.ThrowingPoint _element3;

				private StonePile.ThrowingPoint _element4;

				private StonePile.ThrowingPoint _element5;

				private StonePile.ThrowingPoint _element6;

				private StonePile.ThrowingPoint _element7;

				public const int Length = 8;
			}
		}

		private struct VolumeBoxTimerPair
		{
			public VolumeBox VolumeBox;

			public Timer Timer;
		}
	}
}

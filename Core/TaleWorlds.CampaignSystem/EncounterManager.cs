using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public static class EncounterManager
	{
		public static EncounterModel EncounterModel
		{
			get
			{
				return Campaign.Current.Models.EncounterModel;
			}
		}

		public static void Tick(float dt)
		{
			EncounterManager.HandleEncounters(dt);
		}

		private static void HandleEncounters(float dt)
		{
			if (Campaign.Current.TimeControlMode != CampaignTimeControlMode.Stop)
			{
				for (int i = 0; i < Campaign.Current.MobileParties.Count; i++)
				{
					EncounterManager.HandleEncounterForMobileParty(Campaign.Current.MobileParties[i], dt);
				}
			}
		}

		public static void HandleEncounterForMobileParty(MobileParty mobileParty, float dt)
		{
			if (mobileParty.IsActive && mobileParty.AttachedTo == null && mobileParty.MapEventSide == null && (mobileParty.CurrentSettlement == null || mobileParty.IsGarrison) && (mobileParty.BesiegedSettlement == null || mobileParty.ShortTermBehavior == AiBehavior.AssaultSettlement) && (mobileParty.IsCurrentlyEngagingParty || mobileParty.IsCurrentlyEngagingSettlement || (mobileParty.Ai.AiBehaviorMapEntity != null && mobileParty.ShortTermBehavior == AiBehavior.GoToPoint && !(mobileParty.Ai.AiBehaviorMapEntity is Settlement) && !(mobileParty.Ai.AiBehaviorMapEntity is MobileParty) && (mobileParty.Party != PartyBase.MainParty || PlayerEncounter.Current == null))))
			{
				if (mobileParty.IsCurrentlyEngagingSettlement && mobileParty.ShortTermTargetSettlement != null && mobileParty.ShortTermTargetSettlement == mobileParty.CurrentSettlement)
				{
					return;
				}
				if (mobileParty.IsCurrentlyEngagingParty && (!mobileParty.ShortTermTargetParty.IsActive || (mobileParty.ShortTermTargetParty.CurrentSettlement != null && (mobileParty.ShortTermTargetParty.MapEvent == null || (mobileParty.ShortTermTargetParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker).MapFaction != mobileParty.MapFaction && mobileParty.ShortTermTargetParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender).MapFaction != mobileParty.MapFaction)))))
				{
					return;
				}
				Vec2 vec;
				float num;
				EncounterManager.GetEncounterTargetPoint(dt, mobileParty, out vec, out num);
				float length = (mobileParty.Position2D - vec).Length;
				if ((mobileParty.BesiegedSettlement != null && mobileParty.BesiegedSettlement == mobileParty.TargetSettlement) || length < num)
				{
					mobileParty.Ai.AiBehaviorMapEntity.OnPartyInteraction(mobileParty);
				}
			}
		}

		public static void StartPartyEncounter(PartyBase attackerParty, PartyBase defenderParty)
		{
			bool flag = PartyBase.MainParty.MapEvent != null && (PartyBase.MainParty.MapEvent.InvolvedParties.Contains(attackerParty) || PartyBase.MainParty.MapEvent.InvolvedParties.Contains(defenderParty));
			if (defenderParty == PartyBase.MainParty && PlayerSiege.PlayerSiegeEvent != null)
			{
				Debug.Print("\nPlayerSiege is interrupted\n", 0, Debug.DebugColor.DarkGreen, 64UL);
			}
			if (attackerParty == PartyBase.MainParty || defenderParty == PartyBase.MainParty || flag)
			{
				if (PartyBase.MainParty.MapEvent != null && PlayerEncounter.IsActive && ((PartyBase.MainParty.MapEvent.AttackerSide.TroopCount > 0 && PartyBase.MainParty.MapEvent.DefenderSide.TroopCount > 0) || PartyBase.MainParty.MapEvent.PartiesOnSide(PlayerEncounter.Current.OpponentSide).FindIndex((MapEventParty party) => party.Party == defenderParty) >= 0 || (PartyBase.MainParty.MapEvent.AttackerSide.LeaderParty != MobileParty.MainParty.Party && PartyBase.MainParty.MapEvent.DefenderSide.LeaderParty != MobileParty.MainParty.Party)))
				{
					PlayerEncounter.Current.OnPartyJoinEncounter(attackerParty.MobileParty);
				}
				else if (((attackerParty == PartyBase.MainParty || defenderParty == PartyBase.MainParty) && !PlayerEncounter.IsActive) || PlayerEncounter.EncounterSettlement == Settlement.CurrentSettlement)
				{
					EncounterManager.RestartPlayerEncounter(attackerParty, defenderParty);
				}
			}
			else if (attackerParty.IsActive && defenderParty.IsActive)
			{
				if (attackerParty.MobileParty.Army != null && defenderParty == PartyBase.MainParty)
				{
					MergePartiesAction.Apply(defenderParty, attackerParty);
				}
				else
				{
					StartBattleAction.Apply(attackerParty, defenderParty);
				}
			}
			if (defenderParty.SiegeEvent != null && defenderParty != PartyBase.MainParty && defenderParty.SiegeEvent.BesiegerCamp != null && defenderParty.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(PartyBase.MainParty, MapEvent.BattleTypes.Siege) && (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty))
			{
				EncounterManager.StartPartyEncounter(PartyBase.MainParty, attackerParty);
			}
			if (attackerParty != PartyBase.MainParty && attackerParty.MapEvent != null && attackerParty.MapEvent.IsSallyOut && attackerParty.MapEvent.MapEventSettlement == MobileParty.MainParty.CurrentSettlement && MobileParty.MainParty.Army == null)
			{
				GameMenu.SwitchToMenu("join_sally_out");
			}
		}

		public static void StartSettlementEncounter(MobileParty attackerParty, Settlement settlement)
		{
			if (attackerParty.DefaultBehavior == AiBehavior.BesiegeSettlement && attackerParty.TargetSettlement == settlement && attackerParty.ShortTermBehavior != AiBehavior.AssaultSettlement)
			{
				if (attackerParty.BesiegedSettlement == null)
				{
					if (settlement.SiegeEvent == null)
					{
						Campaign.Current.SiegeEventManager.StartSiegeEvent(settlement, attackerParty);
					}
					else
					{
						attackerParty.BesiegerCamp = settlement.SiegeEvent.BesiegerCamp;
						if (settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapEventSide != null)
						{
							attackerParty.MapEventSide = settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapEventSide;
						}
					}
				}
				if (settlement.Party.MapEvent == null)
				{
					return;
				}
			}
			if (!attackerParty.IsVillager && attackerParty != MobileParty.MainParty && settlement.IsVillage && settlement.Village.VillageState == Village.VillageStates.Looted)
			{
				attackerParty.Ai.SetMoveModeHold();
				return;
			}
			if (attackerParty == MobileParty.MainParty)
			{
				PlayerEncounter.Start();
				PlayerEncounter.Current.Init(attackerParty.Party, settlement.Party, settlement);
				return;
			}
			if (attackerParty.Aggressiveness > 0.01f && PartyBase.MainParty.MapEvent != null && PartyBase.MainParty.MapEvent.MapEventSettlement == settlement)
			{
				if (PartyBase.MainParty.MapEvent != null && PlayerEncounter.IsActive)
				{
					if (attackerParty.MapFaction == MobileParty.MainParty.MapFaction || (PartyBase.MainParty.MapEvent.AttackerSide.LeaderParty != PartyBase.MainParty && PartyBase.MainParty.MapEvent.DefenderSide.LeaderParty != PartyBase.MainParty))
					{
						PlayerEncounter.Current.OnPartyJoinEncounter(attackerParty);
					}
					else
					{
						if (PlayerEncounter.IsActive)
						{
							PlayerEncounter.Finish(true);
						}
						EncounterManager.RestartPlayerEncounter(attackerParty.Party, PartyBase.MainParty);
					}
				}
			}
			else
			{
				bool flag = MobileParty.MainParty.CurrentSettlement == settlement;
				MapEvent mapEvent = settlement.Party.MapEvent;
				if (mapEvent != null && !mapEvent.IsFinalized && (mapEvent.AttackerSide.MapFaction == attackerParty.MapFaction || mapEvent.DefenderSide.MapFaction == attackerParty.MapFaction))
				{
					if (flag && attackerParty.AttachedTo == null)
					{
						PlayerEncounter.Finish(true);
					}
					settlement.Party.MapEventSide = ((mapEvent.AttackerSide.MapFaction == attackerParty.MapFaction) ? mapEvent.DefenderSide : mapEvent.AttackerSide);
				}
				else if (settlement.Party.MapEvent == null && attackerParty != MobileParty.MainParty && attackerParty.ShortTermBehavior == AiBehavior.RaidSettlement && attackerParty.ShortTermTargetSettlement == settlement && FactionManager.IsAtWarAgainstFaction(attackerParty.MapFaction, settlement.MapFaction))
				{
					if (flag)
					{
						PlayerEncounter.Finish(false);
					}
					if (settlement.SettlementHitPoints > 0.001f)
					{
						StartBattleAction.ApplyStartRaid(attackerParty, settlement);
					}
					if (flag)
					{
						if (MobileParty.MainParty.MapFaction == settlement.MapFaction)
						{
							PlayerEncounter.Start();
							PlayerEncounter.Current.Init(attackerParty.Party, settlement.Party, settlement);
						}
						else
						{
							LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
						}
					}
				}
				else if (attackerParty != MobileParty.MainParty && attackerParty.ShortTermBehavior == AiBehavior.AssaultSettlement && attackerParty.ShortTermTargetSettlement == settlement && FactionManager.IsAtWarAgainstFaction(attackerParty.MapFaction, settlement.MapFaction))
				{
					if (flag)
					{
						PlayerEncounter.Finish(false);
					}
					bool flag2 = settlement.Party.MapEvent == null;
					StartBattleAction.ApplyStartAssaultAgainstWalls(attackerParty, settlement);
					if (attackerParty.MapEvent.DefenderSide.TroopCount == 0 && (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSide != BattleSideEnum.Defender || MobileParty.MainParty.CurrentSettlement != settlement))
					{
						if (MobileParty.MainParty.BesiegedSettlement == settlement)
						{
							settlement.Town.IsTaken = true;
							if (PartyBase.MainParty.MapEvent == null)
							{
								if (PlayerEncounter.Current == null)
								{
									EncounterManager.StartSettlementEncounter(MobileParty.MainParty, settlement);
								}
								PlayerEncounter.JoinBattle((Hero.MainHero.CurrentSettlement == settlement && !settlement.Party.MapEvent.IsSallyOut) ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
							}
						}
						attackerParty.MapEvent.SetOverrideWinner(BattleSideEnum.Attacker);
						attackerParty.MapEvent.FinalizeEvent();
						if (settlement.Town.IsTaken)
						{
							if (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
							{
								EncounterManager.StartSettlementEncounter(MobileParty.MainParty, settlement);
							}
							else if (MobileParty.MainParty.Army != null)
							{
								MobileParty.MainParty.Army.LeaderParty.Ai.SetMoveGoToSettlement(settlement);
								MobileParty.MainParty.Army.LeaderParty.Ai.RecalculateShortTermAi();
								EncounterManager.StartSettlementEncounter(MobileParty.MainParty.Army.LeaderParty, settlement);
							}
							GameMenu.SwitchToMenu("menu_settlement_taken");
						}
						return;
					}
					if (attackerParty.ShortTermBehavior == AiBehavior.AssaultSettlement && flag2 && attackerParty != MobileParty.MainParty && PlayerEncounter.Current != null && PlayerEncounter.EncounterSettlement == settlement && MobileParty.MainParty.CurrentSettlement == null)
					{
						PlayerEncounter.Finish(true);
					}
					if (MobileParty.MainParty.BesiegedSettlement == settlement && (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty))
					{
						EncounterManager.StartSettlementEncounter(MobileParty.MainParty, settlement);
					}
					else if (flag)
					{
						if (MobileParty.MainParty.MapFaction == settlement.MapFaction)
						{
							PlayerEncounter.Start();
							PlayerEncounter.Current.Init(attackerParty.Party, settlement.Party, settlement);
						}
						else
						{
							LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
						}
					}
				}
				else if ((attackerParty.ShortTermBehavior == AiBehavior.GoToSettlement && attackerParty.ShortTermTargetSettlement == settlement) || attackerParty.Ai.IsDisabled || (attackerParty.Army != null && attackerParty.Army.LeaderParty.AttachedParties.Contains(attackerParty) && attackerParty.Army.LeaderParty.CurrentSettlement == settlement))
				{
					EnterSettlementAction.ApplyForParty(attackerParty, settlement);
				}
			}
			bool flag3 = attackerParty != null && (attackerParty.Army == null || attackerParty.Army.LeaderParty == attackerParty) && attackerParty.CurrentSettlement == settlement && !attackerParty.IsVillager && !attackerParty.IsMilitia && attackerParty != MobileParty.MainParty && attackerParty.MapEvent == null && settlement != null && settlement.IsVillage;
			if (attackerParty.Army != null && attackerParty.Army.LeaderParty == attackerParty && attackerParty != MobileParty.MainParty && !flag3)
			{
				foreach (MobileParty mobileParty in attackerParty.Army.LeaderParty.AttachedParties)
				{
					if (mobileParty.MapEvent == null)
					{
						EncounterManager.StartSettlementEncounter(mobileParty, settlement);
					}
				}
			}
			if (flag3)
			{
				LeaveSettlementAction.ApplyForParty(attackerParty);
				attackerParty.Ai.SetMoveModeHold();
				if (attackerParty != MobileParty.MainParty && (MobileParty.MainParty.Army == null || attackerParty != MobileParty.MainParty.Army.LeaderParty))
				{
					attackerParty.Ai.RethinkAtNextHourlyTick = true;
				}
			}
		}

		private static void GetEncounterTargetPoint(float dt, MobileParty mobileParty, out Vec2 targetPoint, out float neededMaximumDistanceForEncountering)
		{
			if (mobileParty.Army != null)
			{
				neededMaximumDistanceForEncountering = MathF.Clamp(EncounterManager.EncounterModel.NeededMaximumDistanceForEncounteringMobileParty * MathF.Sqrt((float)(mobileParty.Army.LeaderParty.AttachedParties.Count + 1)), MathF.Max(EncounterManager.EncounterModel.NeededMaximumDistanceForEncounteringMobileParty, dt * EncounterManager.EncounterModel.EstimatedMaximumMobilePartySpeedExceptPlayer), MathF.Max(EncounterManager.EncounterModel.MaximumAllowedDistanceForEncounteringMobilePartyInArmy, dt * (EncounterManager.EncounterModel.EstimatedMaximumMobilePartySpeedExceptPlayer + 0.01f)));
			}
			else
			{
				neededMaximumDistanceForEncountering = MathF.Max(EncounterManager.EncounterModel.NeededMaximumDistanceForEncounteringMobileParty, dt * EncounterManager.EncounterModel.EstimatedMaximumMobilePartySpeedExceptPlayer);
			}
			if (mobileParty.IsCurrentlyEngagingSettlement)
			{
				targetPoint = mobileParty.ShortTermTargetSettlement.GatePosition;
				neededMaximumDistanceForEncountering = (mobileParty.ShortTermTargetSettlement.IsTown ? EncounterManager.EncounterModel.NeededMaximumDistanceForEncounteringTown : EncounterManager.EncounterModel.NeededMaximumDistanceForEncounteringVillage);
				return;
			}
			if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty && mobileParty.ShortTermTargetParty.MapEvent != null && mobileParty.ShortTermTargetParty.MapEvent == mobileParty.Army.LeaderParty.MapEvent && mobileParty.Army.LeaderParty.AttachedParties.Contains(mobileParty))
			{
				targetPoint = mobileParty.Position2D;
				return;
			}
			if (mobileParty.CurrentSettlement != null && mobileParty.ShortTermTargetParty.BesiegedSettlement == mobileParty.CurrentSettlement)
			{
				targetPoint = mobileParty.CurrentSettlement.GatePosition;
				return;
			}
			targetPoint = mobileParty.Ai.AiBehaviorMapEntity.InteractionPosition;
		}

		private static void RestartPlayerEncounter(PartyBase attackerParty, PartyBase defenderParty)
		{
			Settlement settlement = null;
			if (MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.IsRaid)
			{
				settlement = MobileParty.MainParty.MapEvent.MapEventSettlement;
			}
			if (PlayerEncounter.Current != null)
			{
				if ((PlayerEncounter.EncounteredParty == attackerParty && PartyBase.MainParty == defenderParty) || (PlayerEncounter.EncounteredParty == defenderParty && PartyBase.MainParty == attackerParty))
				{
					MapEvent battle = PlayerEncounter.Battle;
					if (battle == null || !battle.IsFinalized)
					{
						goto IL_6F;
					}
				}
				PlayerEncounter.Finish(false);
			}
			IL_6F:
			if (PlayerEncounter.Current == null)
			{
				PlayerEncounter.Start();
			}
			if (attackerParty == PartyBase.MainParty && defenderParty.IsMobile && defenderParty.MobileParty.IsCurrentlyEngagingParty && defenderParty.MobileParty.ShortTermTargetParty == MobileParty.MainParty)
			{
				attackerParty = defenderParty;
				defenderParty = PartyBase.MainParty;
			}
			PlayerEncounter.Current.Init(attackerParty, defenderParty, settlement);
		}
	}
}

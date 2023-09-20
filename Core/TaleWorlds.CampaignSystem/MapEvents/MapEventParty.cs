﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapEvents
{
	// Token: 0x020002BE RID: 702
	public class MapEventParty
	{
		// Token: 0x060028A7 RID: 10407 RVA: 0x000ACE65 File Offset: 0x000AB065
		internal static void AutoGeneratedStaticCollectObjectsMapEventParty(object o, List<object> collectedObjects)
		{
			((MapEventParty)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060028A8 RID: 10408 RVA: 0x000ACE73 File Offset: 0x000AB073
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._roster);
			collectedObjects.Add(this._woundedInBattle);
			collectedObjects.Add(this._diedInBattle);
			collectedObjects.Add(this.Party);
		}

		// Token: 0x060028A9 RID: 10409 RVA: 0x000ACEA5 File Offset: 0x000AB0A5
		internal static object AutoGeneratedGetMemberValueParty(object o)
		{
			return ((MapEventParty)o).Party;
		}

		// Token: 0x060028AA RID: 10410 RVA: 0x000ACEB2 File Offset: 0x000AB0B2
		internal static object AutoGeneratedGetMemberValueGainedRenown(object o)
		{
			return ((MapEventParty)o).GainedRenown;
		}

		// Token: 0x060028AB RID: 10411 RVA: 0x000ACEC4 File Offset: 0x000AB0C4
		internal static object AutoGeneratedGetMemberValueGainedInfluence(object o)
		{
			return ((MapEventParty)o).GainedInfluence;
		}

		// Token: 0x060028AC RID: 10412 RVA: 0x000ACED6 File Offset: 0x000AB0D6
		internal static object AutoGeneratedGetMemberValueMoraleChange(object o)
		{
			return ((MapEventParty)o).MoraleChange;
		}

		// Token: 0x060028AD RID: 10413 RVA: 0x000ACEE8 File Offset: 0x000AB0E8
		internal static object AutoGeneratedGetMemberValuePlunderedGold(object o)
		{
			return ((MapEventParty)o).PlunderedGold;
		}

		// Token: 0x060028AE RID: 10414 RVA: 0x000ACEFA File Offset: 0x000AB0FA
		internal static object AutoGeneratedGetMemberValueGoldLost(object o)
		{
			return ((MapEventParty)o).GoldLost;
		}

		// Token: 0x060028AF RID: 10415 RVA: 0x000ACF0C File Offset: 0x000AB10C
		internal static object AutoGeneratedGetMemberValue_roster(object o)
		{
			return ((MapEventParty)o)._roster;
		}

		// Token: 0x060028B0 RID: 10416 RVA: 0x000ACF19 File Offset: 0x000AB119
		internal static object AutoGeneratedGetMemberValue_contributionToBattle(object o)
		{
			return ((MapEventParty)o)._contributionToBattle;
		}

		// Token: 0x060028B1 RID: 10417 RVA: 0x000ACF2B File Offset: 0x000AB12B
		internal static object AutoGeneratedGetMemberValue_healthyManCountAtStart(object o)
		{
			return ((MapEventParty)o)._healthyManCountAtStart;
		}

		// Token: 0x060028B2 RID: 10418 RVA: 0x000ACF3D File Offset: 0x000AB13D
		internal static object AutoGeneratedGetMemberValue_woundedInBattle(object o)
		{
			return ((MapEventParty)o)._woundedInBattle;
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x000ACF4A File Offset: 0x000AB14A
		internal static object AutoGeneratedGetMemberValue_diedInBattle(object o)
		{
			return ((MapEventParty)o)._diedInBattle;
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x060028B4 RID: 10420 RVA: 0x000ACF57 File Offset: 0x000AB157
		// (set) Token: 0x060028B5 RID: 10421 RVA: 0x000ACF5F File Offset: 0x000AB15F
		[SaveableProperty(1)]
		public PartyBase Party { get; private set; }

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x060028B6 RID: 10422 RVA: 0x000ACF68 File Offset: 0x000AB168
		public int HealthyManCountAtStart
		{
			get
			{
				return this._healthyManCountAtStart;
			}
		}

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x060028B7 RID: 10423 RVA: 0x000ACF70 File Offset: 0x000AB170
		internal TroopRoster DiedInBattle
		{
			get
			{
				return this._diedInBattle;
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x060028B8 RID: 10424 RVA: 0x000ACF78 File Offset: 0x000AB178
		internal TroopRoster WoundedInBattle
		{
			get
			{
				return this._woundedInBattle;
			}
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x060028B9 RID: 10425 RVA: 0x000ACF80 File Offset: 0x000AB180
		public int ContributionToBattle
		{
			get
			{
				return this._contributionToBattle;
			}
		}

		// Token: 0x060028BA RID: 10426 RVA: 0x000ACF88 File Offset: 0x000AB188
		internal void ResetContributionToBattleToStrength()
		{
			this._contributionToBattle = (int)MathF.Sqrt(this.Party.TotalStrength);
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x060028BB RID: 10427 RVA: 0x000ACFA1 File Offset: 0x000AB1A1
		public FlattenedTroopRoster Troops
		{
			get
			{
				return this._roster;
			}
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x000ACFAC File Offset: 0x000AB1AC
		internal MapEventParty(PartyBase party)
		{
			this.Party = party;
			this.Update();
			this._healthyManCountAtStart = party.NumberOfHealthyMembers;
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x000ACFFC File Offset: 0x000AB1FC
		public void Update()
		{
			if (this._roster == null)
			{
				this._roster = new FlattenedTroopRoster(this.Party.MemberRoster.TotalManCount);
			}
			this._roster.Clear();
			foreach (TroopRosterElement troopRosterElement in this.Party.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					if (!this._woundedInBattle.Contains(troopRosterElement.Character) && !this._diedInBattle.Contains(troopRosterElement.Character))
					{
						this._roster.Add(troopRosterElement.Character, troopRosterElement.Character.HeroObject.IsWounded, troopRosterElement.Xp);
					}
				}
				else
				{
					this._roster.Add(troopRosterElement.Character, troopRosterElement.Number, troopRosterElement.WoundedNumber);
				}
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x060028BE RID: 10430 RVA: 0x000AD104 File Offset: 0x000AB304
		public bool IsNpcParty
		{
			get
			{
				return this.Party != PartyBase.MainParty;
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x060028BF RID: 10431 RVA: 0x000AD116 File Offset: 0x000AB316
		public TroopRoster RosterToReceiveLootMembers
		{
			get
			{
				if (!this.IsNpcParty)
				{
					return PlayerEncounter.Current.RosterToReceiveLootMembers;
				}
				return this.Party.MemberRoster;
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x060028C0 RID: 10432 RVA: 0x000AD136 File Offset: 0x000AB336
		public TroopRoster RosterToReceiveLootPrisoners
		{
			get
			{
				if (!this.IsNpcParty)
				{
					return PlayerEncounter.Current.RosterToReceiveLootPrisoners;
				}
				return this.Party.PrisonRoster;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x060028C1 RID: 10433 RVA: 0x000AD156 File Offset: 0x000AB356
		public ItemRoster RosterToReceiveLootItems
		{
			get
			{
				if (!this.IsNpcParty)
				{
					return PlayerEncounter.Current.RosterToReceiveLootItems;
				}
				return this.Party.ItemRoster;
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x060028C2 RID: 10434 RVA: 0x000AD176 File Offset: 0x000AB376
		// (set) Token: 0x060028C3 RID: 10435 RVA: 0x000AD17E File Offset: 0x000AB37E
		[SaveableProperty(7)]
		public float GainedRenown { get; set; }

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x060028C4 RID: 10436 RVA: 0x000AD187 File Offset: 0x000AB387
		// (set) Token: 0x060028C5 RID: 10437 RVA: 0x000AD18F File Offset: 0x000AB38F
		[SaveableProperty(8)]
		public float GainedInfluence { get; set; }

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x060028C6 RID: 10438 RVA: 0x000AD198 File Offset: 0x000AB398
		// (set) Token: 0x060028C7 RID: 10439 RVA: 0x000AD1A0 File Offset: 0x000AB3A0
		[SaveableProperty(9)]
		public float MoraleChange { get; set; }

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x060028C8 RID: 10440 RVA: 0x000AD1A9 File Offset: 0x000AB3A9
		// (set) Token: 0x060028C9 RID: 10441 RVA: 0x000AD1B1 File Offset: 0x000AB3B1
		[SaveableProperty(10)]
		public int PlunderedGold { get; set; }

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x060028CA RID: 10442 RVA: 0x000AD1BA File Offset: 0x000AB3BA
		// (set) Token: 0x060028CB RID: 10443 RVA: 0x000AD1C2 File Offset: 0x000AB3C2
		[SaveableProperty(11)]
		public int GoldLost { get; set; }

		// Token: 0x060028CC RID: 10444 RVA: 0x000AD1CC File Offset: 0x000AB3CC
		public void OnTroopKilled(UniqueTroopDescriptor troopSeed)
		{
			FlattenedTroopRosterElement flattenedTroopRosterElement = this._roster[troopSeed];
			CharacterObject troop = flattenedTroopRosterElement.Troop;
			this.Party.MemberRoster.AddTroopTempXp(troop, -flattenedTroopRosterElement.XpGained);
			if (!troop.IsHero && this.Party.IsActive)
			{
				this.Party.MemberRoster.RemoveTroop(troop, 1, troopSeed, 0);
			}
			this._roster.OnTroopKilled(troopSeed);
			this.DiedInBattle.AddToCounts(this._roster[troopSeed].Troop, 1, false, 0, 0, true, -1);
			this._contributionToBattle++;
		}

		// Token: 0x060028CD RID: 10445 RVA: 0x000AD270 File Offset: 0x000AB470
		public void OnTroopWounded(UniqueTroopDescriptor troopSeed)
		{
			this.Party.MemberRoster.WoundTroop(this._roster[troopSeed].Troop, 1, troopSeed);
			this._roster.OnTroopWounded(troopSeed);
			this.WoundedInBattle.AddToCounts(this._roster[troopSeed].Troop, 1, false, 1, 0, true, -1);
		}

		// Token: 0x060028CE RID: 10446 RVA: 0x000AD2D5 File Offset: 0x000AB4D5
		public void OnTroopRouted(UniqueTroopDescriptor troopSeed)
		{
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x000AD2D8 File Offset: 0x000AB4D8
		public CharacterObject GetTroop(UniqueTroopDescriptor troopSeed)
		{
			return this._roster[troopSeed].Troop;
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x000AD2FC File Offset: 0x000AB4FC
		public void OnTroopScoreHit(UniqueTroopDescriptor attackerTroopDesc, CharacterObject attackedTroop, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon, bool isSimulatedHit)
		{
			CharacterObject troop = this._roster[attackerTroopDesc].Troop;
			if (!isTeamKill)
			{
				int num;
				Campaign.Current.Models.CombatXpModel.GetXpFromHit(troop, null, attackedTroop, this.Party, damage, isFatal, isSimulatedHit ? CombatXpModel.MissionTypeEnum.SimulationBattle : CombatXpModel.MissionTypeEnum.Battle, out num);
				num += MBRandom.RoundRandomized((float)num);
				if (!troop.IsHero)
				{
					if (num > 0)
					{
						int num2 = this._roster.OnTroopGainXp(attackerTroopDesc, num);
						this.Party.MemberRoster.AddTroopTempXp(troop, num2);
					}
				}
				else
				{
					CampaignEventDispatcher.Instance.OnHeroCombatHit(troop, attackedTroop, this.Party, attackerWeapon, isFatal, num);
				}
				this._contributionToBattle += num;
			}
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000AD3AC File Offset: 0x000AB5AC
		public void CommitXpGain()
		{
			if (this.Party.MobileParty == null)
			{
				return;
			}
			int num = 0;
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in this._roster)
			{
				CharacterObject troop = flattenedTroopRosterElement.Troop;
				bool flag = Campaign.Current.Models.PartyTroopUpgradeModel.CanTroopGainXp(this.Party, troop);
				if (!flattenedTroopRosterElement.IsKilled && flattenedTroopRosterElement.XpGained > 0 && flag)
				{
					int num2 = Campaign.Current.Models.PartyTrainingModel.CalculateXpGainFromBattles(flattenedTroopRosterElement, this.Party);
					int num3 = Campaign.Current.Models.PartyTrainingModel.GenerateSharedXp(troop, num2, this.Party.MobileParty);
					if (num3 > 0)
					{
						num += num3;
						num2 -= num3;
					}
					if (!troop.IsHero)
					{
						this.Party.MemberRoster.AddXpToTroop(num2, troop);
					}
				}
			}
			MobilePartyHelper.PartyAddSharedXp(this.Party.MobileParty, (float)num);
			SkillLevelingManager.OnBattleEnd(this.Party, this._roster);
		}

		// Token: 0x04000C61 RID: 3169
		[SaveableField(2)]
		private FlattenedTroopRoster _roster;

		// Token: 0x04000C62 RID: 3170
		[SaveableField(3)]
		private int _contributionToBattle = 1;

		// Token: 0x04000C63 RID: 3171
		[SaveableField(9)]
		private int _healthyManCountAtStart = 1;

		// Token: 0x04000C64 RID: 3172
		[SaveableField(7)]
		private TroopRoster _woundedInBattle = TroopRoster.CreateDummyTroopRoster();

		// Token: 0x04000C65 RID: 3173
		[SaveableField(8)]
		private TroopRoster _diedInBattle = TroopRoster.CreateDummyTroopRoster();
	}
}

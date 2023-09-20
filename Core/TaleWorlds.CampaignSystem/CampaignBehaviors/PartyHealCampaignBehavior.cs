﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003B8 RID: 952
	public class PartyHealCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060038A3 RID: 14499 RVA: 0x00102250 File Offset: 0x00100450
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanHourlyTick));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.OnWeeklyTick));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnQuarterDailyPartyTick.AddNonSerializedListener(this, new Action<MobileParty>(this.OnQuarterDailyPartyTick));
		}

		// Token: 0x060038A4 RID: 14500 RVA: 0x001022E7 File Offset: 0x001004E7
		private void OnQuarterDailyPartyTick(MobileParty mobileParty)
		{
			if (!mobileParty.IsMainParty)
			{
				this.TryHealOrWoundParty(mobileParty, false);
			}
		}

		// Token: 0x060038A5 RID: 14501 RVA: 0x001022FC File Offset: 0x001004FC
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._overflowedHealingForRegulars.ContainsKey(mobileParty.Party))
			{
				this._overflowedHealingForRegulars.Remove(mobileParty.Party);
				if (this._overflowedHealingForHeroes.ContainsKey(mobileParty.Party))
				{
					this._overflowedHealingForHeroes.Remove(mobileParty.Party);
				}
			}
		}

		// Token: 0x060038A6 RID: 14502 RVA: 0x00102354 File Offset: 0x00100554
		private void OnWeeklyTick()
		{
			List<PartyBase> list = new List<PartyBase>();
			foreach (KeyValuePair<PartyBase, float> keyValuePair in this._overflowedHealingForRegulars)
			{
				PartyBase key = keyValuePair.Key;
				if (!key.IsActive && !key.IsValid)
				{
					list.Add(key);
				}
			}
			foreach (PartyBase partyBase in list)
			{
				this._overflowedHealingForRegulars.Remove(partyBase);
				if (this._overflowedHealingForHeroes.ContainsKey(partyBase))
				{
					this._overflowedHealingForHeroes.Remove(partyBase);
				}
			}
		}

		// Token: 0x060038A7 RID: 14503 RVA: 0x00102428 File Offset: 0x00100628
		public void OnMapEventEnded(MapEvent mapEvent)
		{
			this.OnBattleEndCheckPerkEffects(mapEvent);
		}

		// Token: 0x060038A8 RID: 14504 RVA: 0x00102434 File Offset: 0x00100634
		private void OnBattleEndCheckPerkEffects(MapEvent mapEvent)
		{
			if ((mapEvent.EventType == MapEvent.BattleTypes.FieldBattle || mapEvent.EventType == MapEvent.BattleTypes.Siege || mapEvent.EventType == MapEvent.BattleTypes.SiegeOutside || mapEvent.EventType == MapEvent.BattleTypes.Hideout || mapEvent.EventType == MapEvent.BattleTypes.SallyOut) && mapEvent.HasWinner)
			{
				foreach (PartyBase partyBase in mapEvent.InvolvedParties)
				{
					foreach (TroopRosterElement troopRosterElement in partyBase.MemberRoster.GetTroopRoster())
					{
						if (troopRosterElement.Character.IsHero)
						{
							Hero heroObject = troopRosterElement.Character.HeroObject;
							int battleEndHealingAmount = Campaign.Current.Models.PartyHealingModel.GetBattleEndHealingAmount(partyBase.MobileParty, heroObject.CharacterObject);
							if (battleEndHealingAmount > 0)
							{
								heroObject.Heal(battleEndHealingAmount, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x060038A9 RID: 14505 RVA: 0x0010254C File Offset: 0x0010074C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<PartyBase, float>>("_overflowedHealingForRegulars", ref this._overflowedHealingForRegulars);
			dataStore.SyncData<Dictionary<PartyBase, float>>("_overflowedHealingForHeroes", ref this._overflowedHealingForHeroes);
		}

		// Token: 0x060038AA RID: 14506 RVA: 0x00102574 File Offset: 0x00100774
		private void OnClanHourlyTick(Clan clan)
		{
			foreach (Hero hero in clan.Heroes)
			{
				if (hero.PartyBelongedTo == null && hero.PartyBelongedToAsPrisoner == null)
				{
					int num = MBRandom.RoundRandomized(0.5f);
					if (hero.HitPoints < hero.MaxHitPoints)
					{
						int num2 = MathF.Min(num, hero.MaxHitPoints - hero.HitPoints);
						hero.HitPoints += num2;
					}
				}
			}
		}

		// Token: 0x060038AB RID: 14507 RVA: 0x0010260C File Offset: 0x0010080C
		private void OnHourlyTick()
		{
			this.TryHealOrWoundParty(MobileParty.MainParty, true);
		}

		// Token: 0x060038AC RID: 14508 RVA: 0x0010261C File Offset: 0x0010081C
		private void TryHealOrWoundParty(MobileParty mobileParty, bool isCheckingForPlayerRelatedParty)
		{
			if (mobileParty.IsActive && mobileParty.MapEvent == null)
			{
				float num;
				if (!this._overflowedHealingForHeroes.TryGetValue(mobileParty.Party, out num))
				{
					this._overflowedHealingForHeroes.Add(mobileParty.Party, 0f);
				}
				float num2;
				if (!this._overflowedHealingForRegulars.TryGetValue(mobileParty.Party, out num2))
				{
					this._overflowedHealingForRegulars.Add(mobileParty.Party, 0f);
				}
				float num3 = (isCheckingForPlayerRelatedParty ? (mobileParty.HealingRateForHeroes / 24f) : mobileParty.HealingRateForHeroes);
				float num4 = (isCheckingForPlayerRelatedParty ? (mobileParty.HealingRateForRegulars / 24f) : mobileParty.HealingRateForRegulars);
				num += num3;
				num2 += num4;
				if (num >= 1f)
				{
					PartyHealCampaignBehavior.HealHeroes(mobileParty, ref num);
				}
				else if (num <= -1f)
				{
					PartyHealCampaignBehavior.ReduceHpHeroes(mobileParty, ref num);
				}
				if (num2 >= 1f)
				{
					PartyHealCampaignBehavior.HealRegulars(mobileParty, ref num2);
				}
				else if (num2 <= -1f)
				{
					PartyHealCampaignBehavior.ReduceHpRegulars(mobileParty, ref num2);
				}
				this._overflowedHealingForHeroes[mobileParty.Party] = num;
				this._overflowedHealingForRegulars[mobileParty.Party] = num2;
			}
		}

		// Token: 0x060038AD RID: 14509 RVA: 0x00102738 File Offset: 0x00100938
		private static void HealHeroes(MobileParty mobileParty, ref float heroesHealingValue)
		{
			int num = MathF.Floor(heroesHealingValue);
			heroesHealingValue -= (float)num;
			TroopRoster memberRoster = mobileParty.MemberRoster;
			if (memberRoster.TotalHeroes > 0)
			{
				for (int i = 0; i < memberRoster.Count; i++)
				{
					Hero heroObject = memberRoster.GetCharacterAtIndex(i).HeroObject;
					if (heroObject != null && !heroObject.IsHealthFull())
					{
						heroObject.Heal(num, true);
					}
				}
			}
			TroopRoster prisonRoster = mobileParty.PrisonRoster;
			if (prisonRoster.TotalHeroes > 0)
			{
				for (int j = 0; j < prisonRoster.Count; j++)
				{
					Hero heroObject2 = prisonRoster.GetCharacterAtIndex(j).HeroObject;
					if (heroObject2 != null && !heroObject2.IsHealthFull())
					{
						heroObject2.Heal(1, false);
					}
				}
			}
		}

		// Token: 0x060038AE RID: 14510 RVA: 0x001027E4 File Offset: 0x001009E4
		private static void ReduceHpHeroes(MobileParty mobileParty, ref float heroesHealingValue)
		{
			int num = MathF.Ceiling(heroesHealingValue);
			heroesHealingValue = -(-heroesHealingValue % 1f);
			for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
			{
				Hero heroObject = mobileParty.MemberRoster.GetCharacterAtIndex(i).HeroObject;
				if (heroObject != null && heroObject.HitPoints > 0)
				{
					int num2 = MathF.Min(num, heroObject.HitPoints);
					heroObject.HitPoints += num2;
				}
			}
		}

		// Token: 0x060038AF RID: 14511 RVA: 0x00102854 File Offset: 0x00100A54
		private static void HealRegulars(MobileParty mobileParty, ref float regularsHealingValue)
		{
			TroopRoster memberRoster = mobileParty.MemberRoster;
			if (memberRoster.TotalWoundedRegulars == 0)
			{
				regularsHealingValue = 0f;
				return;
			}
			int num = MathF.Floor(regularsHealingValue);
			regularsHealingValue -= (float)num;
			int num2 = 0;
			float num3 = 0f;
			int num4 = MBRandom.RandomInt(memberRoster.Count);
			int num5 = 0;
			while (num5 < memberRoster.Count && num > 0)
			{
				int num6 = (num4 + num5) % memberRoster.Count;
				CharacterObject characterAtIndex = memberRoster.GetCharacterAtIndex(num6);
				if (characterAtIndex.IsRegular)
				{
					int num7 = MathF.Min(num, memberRoster.GetElementWoundedNumber(num6));
					if (num7 > 0)
					{
						memberRoster.AddToCountsAtIndex(num6, 0, -num7, 0, true);
						num -= num7;
						num2 += num7;
						num3 += (float)(characterAtIndex.Tier * num7);
					}
				}
				num5++;
			}
			if (num2 > 0)
			{
				SkillLevelingManager.OnRegularTroopHealedWhileWaiting(mobileParty, num2, num3 / (float)num2);
			}
		}

		// Token: 0x060038B0 RID: 14512 RVA: 0x00102924 File Offset: 0x00100B24
		private static void ReduceHpRegulars(MobileParty mobileParty, ref float regularsHealingValue)
		{
			TroopRoster memberRoster = mobileParty.MemberRoster;
			if (memberRoster.TotalRegulars - memberRoster.TotalWoundedRegulars == 0)
			{
				regularsHealingValue = 0f;
				return;
			}
			int num = MathF.Floor(-regularsHealingValue);
			regularsHealingValue = -(-regularsHealingValue % 1f);
			int num2 = MBRandom.RandomInt(memberRoster.Count);
			int num3 = 0;
			while (num3 < memberRoster.Count && num > 0)
			{
				int num4 = (num2 + num3) % memberRoster.Count;
				if (memberRoster.GetCharacterAtIndex(num4).IsRegular)
				{
					int num5 = MathF.Min(memberRoster.GetElementNumber(num4) - memberRoster.GetElementWoundedNumber(num4), num);
					if (num5 > 0)
					{
						memberRoster.AddToCountsAtIndex(num4, 0, num5, 0, true);
						num -= num5;
					}
				}
				num3++;
			}
		}

		// Token: 0x040011B1 RID: 4529
		private Dictionary<PartyBase, float> _overflowedHealingForRegulars = new Dictionary<PartyBase, float>();

		// Token: 0x040011B2 RID: 4530
		private Dictionary<PartyBase, float> _overflowedHealingForHeroes = new Dictionary<PartyBase, float>();
	}
}

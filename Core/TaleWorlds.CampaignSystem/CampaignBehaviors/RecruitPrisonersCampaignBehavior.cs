using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003CD RID: 973
	public class RecruitPrisonersCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003A7D RID: 14973 RVA: 0x0010F758 File Offset: 0x0010D958
		public override void RegisterEvents()
		{
			CampaignEvents.OnMainPartyPrisonerRecruitedEvent.AddNonSerializedListener(this, new Action<FlattenedTroopRoster>(this.OnMainPartyPrisonerRecruited));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickAIMobileParty));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTickMainParty));
		}

		// Token: 0x06003A7E RID: 14974 RVA: 0x0010F7AC File Offset: 0x0010D9AC
		private void HourlyTickMainParty()
		{
			MobileParty mainParty = MobileParty.MainParty;
			TroopRoster memberRoster = mainParty.MemberRoster;
			TroopRoster prisonRoster = mainParty.PrisonRoster;
			if (memberRoster.Count != 0 && memberRoster.TotalManCount > 0 && prisonRoster.Count != 0 && prisonRoster.TotalRegulars > 0 && mainParty.MapEvent == null)
			{
				int num = MBRandom.RandomInt(0, prisonRoster.Count);
				bool flag = false;
				for (int i = num; i < prisonRoster.Count + num; i++)
				{
					int num2 = i % prisonRoster.Count;
					CharacterObject characterAtIndex = prisonRoster.GetCharacterAtIndex(num2);
					if (characterAtIndex.IsRegular)
					{
						CharacterObject characterObject = characterAtIndex;
						int elementNumber = mainParty.PrisonRoster.GetElementNumber(num2);
						int num3 = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.CalculateRecruitableNumber(mainParty.Party, characterObject);
						if (!flag && num3 < elementNumber)
						{
							flag = this.GenerateConformityForTroop(mainParty, characterObject, 1);
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06003A7F RID: 14975 RVA: 0x0010F898 File Offset: 0x0010DA98
		private void DailyTickAIMobileParty(MobileParty mobileParty)
		{
			TroopRoster prisonRoster = mobileParty.PrisonRoster;
			if (!mobileParty.IsMainParty && mobileParty.IsLordParty && prisonRoster.Count != 0 && prisonRoster.TotalRegulars > 0 && mobileParty.MapEvent == null)
			{
				int num = MBRandom.RandomInt(0, prisonRoster.Count);
				bool flag = false;
				for (int i = num; i < prisonRoster.Count + num; i++)
				{
					int num2 = i % prisonRoster.Count;
					CharacterObject characterAtIndex = prisonRoster.GetCharacterAtIndex(num2);
					if (characterAtIndex.IsRegular)
					{
						CharacterObject characterObject = characterAtIndex;
						int elementNumber = mobileParty.PrisonRoster.GetElementNumber(num2);
						int num3 = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.CalculateRecruitableNumber(mobileParty.Party, characterObject);
						if (!flag && num3 < elementNumber)
						{
							flag = this.GenerateConformityForTroop(mobileParty, characterObject, 24);
						}
						if (Campaign.Current.Models.PrisonerRecruitmentCalculationModel.ShouldPartyRecruitPrisoners(mobileParty.Party))
						{
							int num4;
							if (this.IsPrisonerRecruitable(mobileParty, characterObject, out num4))
							{
								int num5 = mobileParty.LimitedPartySize - mobileParty.MemberRoster.TotalManCount;
								int num6 = MathF.Min((num5 > 0) ? ((num5 > num3) ? num3 : num5) : 0, prisonRoster.GetElementNumber(characterObject));
								if (num6 > 0)
								{
									this.RecruitPrisonersAi(mobileParty, characterObject, num6, num4);
								}
							}
						}
						else if (flag)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06003A80 RID: 14976 RVA: 0x0010F9EC File Offset: 0x0010DBEC
		private bool GenerateConformityForTroop(MobileParty mobileParty, CharacterObject troop, int hours = 1)
		{
			int num = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetConformityChangePerHour(mobileParty.Party, troop) * hours;
			mobileParty.PrisonRoster.AddXpToTroop(num, troop);
			return true;
		}

		// Token: 0x06003A81 RID: 14977 RVA: 0x0010FA28 File Offset: 0x0010DC28
		private void ApplyPrisonerRecruitmentEffects(MobileParty mobileParty, CharacterObject troop, int num)
		{
			int prisonerRecruitmentMoraleEffect = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetPrisonerRecruitmentMoraleEffect(mobileParty.Party, troop, num);
			mobileParty.RecentEventsMorale += (float)prisonerRecruitmentMoraleEffect;
		}

		// Token: 0x06003A82 RID: 14978 RVA: 0x0010FA64 File Offset: 0x0010DC64
		private void RecruitPrisonersAi(MobileParty mobileParty, CharacterObject troop, int num, int conformityCost)
		{
			mobileParty.PrisonRoster.GetElementNumber(troop);
			mobileParty.PrisonRoster.GetElementXp(troop);
			mobileParty.PrisonRoster.AddToCounts(troop, -num, false, 0, -conformityCost * num, true, -1);
			mobileParty.MemberRoster.AddToCounts(troop, num, false, 0, 0, true, -1);
			CampaignEventDispatcher.Instance.OnTroopRecruited(mobileParty.LeaderHero, null, null, troop, num);
			this.ApplyPrisonerRecruitmentEffects(mobileParty, troop, num);
		}

		// Token: 0x06003A83 RID: 14979 RVA: 0x0010FAD3 File Offset: 0x0010DCD3
		private bool IsPrisonerRecruitable(MobileParty mobileParty, CharacterObject character, out int conformityNeeded)
		{
			return Campaign.Current.Models.PrisonerRecruitmentCalculationModel.IsPrisonerRecruitable(mobileParty.Party, character, out conformityNeeded);
		}

		// Token: 0x06003A84 RID: 14980 RVA: 0x0010FAF4 File Offset: 0x0010DCF4
		private void OnMainPartyPrisonerRecruited(FlattenedTroopRoster flattenedTroopRosters)
		{
			foreach (CharacterObject characterObject in flattenedTroopRosters.Troops)
			{
				CampaignEventDispatcher.Instance.OnUnitRecruited(characterObject, 1);
				this.ApplyPrisonerRecruitmentEffects(MobileParty.MainParty, characterObject, 1);
			}
		}

		// Token: 0x06003A85 RID: 14981 RVA: 0x0010FB54 File Offset: 0x0010DD54
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}

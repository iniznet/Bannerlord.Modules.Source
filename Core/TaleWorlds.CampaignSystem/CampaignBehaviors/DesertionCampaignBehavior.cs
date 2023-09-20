using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class DesertionCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void DailyTickParty(MobileParty mobileParty)
		{
			if (!Campaign.Current.DesertionEnabled)
			{
				return;
			}
			if (mobileParty.IsActive && !mobileParty.IsDisbanding && mobileParty.Party.MapEvent == null && (mobileParty.IsLordParty || mobileParty.IsCaravan))
			{
				TroopRoster troopRoster = null;
				if (mobileParty.MemberRoster.TotalRegulars > 0)
				{
					this.PartiesCheckDesertionDueToMorale(mobileParty, ref troopRoster);
					this.PartiesCheckDesertionDueToPartySizeExceedsPaymentRatio(mobileParty, ref troopRoster);
				}
				if (troopRoster != null && troopRoster.Count > 0)
				{
					CampaignEventDispatcher.Instance.OnTroopsDeserted(mobileParty, troopRoster);
				}
				if (mobileParty.Party.NumberOfAllMembers <= 0)
				{
					DestroyPartyAction.Apply(null, mobileParty);
				}
			}
		}

		private void PartiesCheckForTroopDesertionEffectiveMorale(MobileParty party, int stackNo, int desertIfMoraleIsLessThanValue, out int numberOfDeserters, out int numberOfWoundedDeserters)
		{
			int num = 0;
			int num2 = 0;
			float morale = party.Morale;
			if (party.IsActive && party.MemberRoster.Count > 0)
			{
				TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(stackNo);
				float num3 = MathF.Pow((float)elementCopyAtIndex.Character.Level / 100f, 0.1f * (((float)desertIfMoraleIsLessThanValue - morale) / (float)desertIfMoraleIsLessThanValue));
				for (int i = 0; i < elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber; i++)
				{
					if (num3 < MBRandom.RandomFloat)
					{
						num++;
					}
				}
				for (int j = 0; j < elementCopyAtIndex.WoundedNumber; j++)
				{
					if (num3 < MBRandom.RandomFloat)
					{
						num2++;
					}
				}
			}
			numberOfDeserters = num;
			numberOfWoundedDeserters = num2;
		}

		private void PartiesCheckDesertionDueToPartySizeExceedsPaymentRatio(MobileParty mobileParty, ref TroopRoster desertedTroopList)
		{
			int partySizeLimit = mobileParty.Party.PartySizeLimit;
			if ((mobileParty.IsLordParty || mobileParty.IsCaravan) && mobileParty.Party.NumberOfAllMembers > partySizeLimit && mobileParty != MobileParty.MainParty && mobileParty.MapEvent == null)
			{
				int num = mobileParty.Party.NumberOfAllMembers - partySizeLimit;
				for (int i = 0; i < num; i++)
				{
					CharacterObject characterObject = mobileParty.MapFaction.BasicTroop;
					int num2 = 99;
					bool flag = false;
					for (int j = 0; j < mobileParty.MemberRoster.Count; j++)
					{
						CharacterObject characterAtIndex = mobileParty.MemberRoster.GetCharacterAtIndex(j);
						if (!characterAtIndex.IsHero && characterAtIndex.Level < num2 && mobileParty.MemberRoster.GetElementNumber(j) > 0)
						{
							num2 = characterAtIndex.Level;
							characterObject = characterAtIndex;
							flag = mobileParty.MemberRoster.GetElementWoundedNumber(j) > 0;
						}
					}
					if (num2 < 99)
					{
						if (flag)
						{
							mobileParty.MemberRoster.AddToCounts(characterObject, -1, false, -1, 0, true, -1);
						}
						else
						{
							mobileParty.MemberRoster.AddToCounts(characterObject, -1, false, 0, 0, true, -1);
						}
					}
				}
			}
			bool flag2 = mobileParty.IsWageLimitExceeded();
			if (mobileParty.Party.NumberOfAllMembers > mobileParty.LimitedPartySize || flag2)
			{
				int numberOfDeserters = Campaign.Current.Models.PartyDesertionModel.GetNumberOfDeserters(mobileParty);
				int num3 = 0;
				while (num3 < numberOfDeserters && mobileParty.MemberRoster.TotalRegulars > 0)
				{
					int num4 = -1;
					int num5 = 9;
					int num6 = 1;
					int num7 = 0;
					while (num7 < mobileParty.MemberRoster.Count && mobileParty.MemberRoster.TotalRegulars > 0)
					{
						CharacterObject characterAtIndex2 = mobileParty.MemberRoster.GetCharacterAtIndex(num7);
						int elementNumber = mobileParty.MemberRoster.GetElementNumber(num7);
						if (!characterAtIndex2.IsHero && elementNumber > 0 && characterAtIndex2.Tier < num5)
						{
							num5 = characterAtIndex2.Tier;
							num4 = num7;
							num6 = Math.Min(elementNumber, numberOfDeserters - num3);
						}
						num7++;
					}
					MobilePartyHelper.DesertTroopsFromParty(mobileParty, num4, num6, 0, ref desertedTroopList);
					num3 += num6;
				}
			}
		}

		private bool PartiesCheckDesertionDueToMorale(MobileParty party, ref TroopRoster desertedTroopList)
		{
			int moraleThresholdForTroopDesertion = Campaign.Current.Models.PartyDesertionModel.GetMoraleThresholdForTroopDesertion(party);
			bool flag = false;
			if (party.Morale < (float)moraleThresholdForTroopDesertion && party.MemberRoster.TotalManCount > 0)
			{
				for (int i = party.MemberRoster.Count - 1; i >= 0; i--)
				{
					if (!party.MemberRoster.GetCharacterAtIndex(i).IsHero)
					{
						int num = 0;
						int num2 = 0;
						this.PartiesCheckForTroopDesertionEffectiveMorale(party, i, moraleThresholdForTroopDesertion, out num, out num2);
						if (num + num2 > 0)
						{
							if (party.IsLordParty && party.MapFaction.IsKingdomFaction)
							{
								this._numberOfDesertersFromLordParty++;
							}
							flag = true;
							MobilePartyHelper.DesertTroopsFromParty(party, i, num, num2, ref desertedTroopList);
						}
					}
				}
			}
			return flag;
		}

		private int _numberOfDesertersFromLordParty;
	}
}

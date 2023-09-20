using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class RetrainOutlawPartyMembersBehavior : CampaignBehaviorBase, IRetrainOutlawPartyMembersCampaignBehavior, ICampaignBehavior
	{
		private int GetRetrainedNumberInternal(CharacterObject character)
		{
			int num;
			if (!this._retrainTable.TryGetValue(character, out num))
			{
				return 0;
			}
			return num;
		}

		private int SetRetrainedNumberInternal(CharacterObject character, int numberRetrained)
		{
			this._retrainTable[character] = numberRetrained;
			return numberRetrained;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		private void DailyTick()
		{
			if (MBRandom.RandomFloat > 0.5f)
			{
				int num = MBRandom.RandomInt(MobileParty.MainParty.MemberRoster.Count);
				bool flag = false;
				int num2 = 0;
				while (num2 < MobileParty.MainParty.MemberRoster.Count && !flag)
				{
					int num3 = (num2 + num) % MobileParty.MainParty.MemberRoster.Count;
					CharacterObject characterAtIndex = MobileParty.MainParty.MemberRoster.GetCharacterAtIndex(num3);
					if (characterAtIndex.Occupation == Occupation.Bandit)
					{
						int elementNumber = MobileParty.MainParty.MemberRoster.GetElementNumber(num3);
						int num4 = this.GetRetrainedNumberInternal(characterAtIndex);
						if (num4 < elementNumber && !flag)
						{
							num4++;
							this.SetRetrainedNumberInternal(characterAtIndex, num4);
						}
						else if (num4 > elementNumber)
						{
							this.SetRetrainedNumberInternal(characterAtIndex, elementNumber);
						}
					}
					num2++;
				}
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<CharacterObject, int>>("_retrainTable", ref this._retrainTable);
		}

		public int GetRetrainedNumber(CharacterObject character)
		{
			if (character.Occupation == Occupation.Bandit)
			{
				int retrainedNumberInternal = this.GetRetrainedNumberInternal(character);
				int troopCount = MobileParty.MainParty.MemberRoster.GetTroopCount(character);
				return MathF.Min(retrainedNumberInternal, troopCount);
			}
			return 0;
		}

		public void SetRetrainedNumber(CharacterObject character, int number)
		{
			this.SetRetrainedNumberInternal(character, number);
		}

		private Dictionary<CharacterObject, int> _retrainTable = new Dictionary<CharacterObject, int>();
	}
}

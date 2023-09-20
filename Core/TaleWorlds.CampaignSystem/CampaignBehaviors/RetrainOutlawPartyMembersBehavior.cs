using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003CE RID: 974
	public class RetrainOutlawPartyMembersBehavior : CampaignBehaviorBase, IRetrainOutlawPartyMembersCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x06003A87 RID: 14983 RVA: 0x0010FB60 File Offset: 0x0010DD60
		private int GetRetrainedNumberInternal(CharacterObject character)
		{
			int num;
			if (!this._retrainTable.TryGetValue(character, out num))
			{
				return 0;
			}
			return num;
		}

		// Token: 0x06003A88 RID: 14984 RVA: 0x0010FB80 File Offset: 0x0010DD80
		private int SetRetrainedNumberInternal(CharacterObject character, int numberRetrained)
		{
			this._retrainTable[character] = numberRetrained;
			return numberRetrained;
		}

		// Token: 0x06003A89 RID: 14985 RVA: 0x0010FB9D File Offset: 0x0010DD9D
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		// Token: 0x06003A8A RID: 14986 RVA: 0x0010FBB8 File Offset: 0x0010DDB8
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

		// Token: 0x06003A8B RID: 14987 RVA: 0x0010FC85 File Offset: 0x0010DE85
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<CharacterObject, int>>("_retrainTable", ref this._retrainTable);
		}

		// Token: 0x06003A8C RID: 14988 RVA: 0x0010FC9C File Offset: 0x0010DE9C
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

		// Token: 0x06003A8D RID: 14989 RVA: 0x0010FCD3 File Offset: 0x0010DED3
		public void SetRetrainedNumber(CharacterObject character, int number)
		{
			this.SetRetrainedNumberInternal(character, number);
		}

		// Token: 0x04001202 RID: 4610
		private Dictionary<CharacterObject, int> _retrainTable = new Dictionary<CharacterObject, int>();
	}
}

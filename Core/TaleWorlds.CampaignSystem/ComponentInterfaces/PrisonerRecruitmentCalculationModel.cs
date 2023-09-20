using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C1 RID: 449
	public abstract class PrisonerRecruitmentCalculationModel : GameModel
	{
		// Token: 0x06001B3A RID: 6970
		public abstract int GetConformityNeededToRecruitPrisoner(CharacterObject character);

		// Token: 0x06001B3B RID: 6971
		public abstract int GetConformityChangePerHour(PartyBase party, CharacterObject character);

		// Token: 0x06001B3C RID: 6972
		public abstract int GetPrisonerRecruitmentMoraleEffect(PartyBase party, CharacterObject character, int num);

		// Token: 0x06001B3D RID: 6973
		public abstract bool IsPrisonerRecruitable(PartyBase party, CharacterObject character, out int conformityNeeded);

		// Token: 0x06001B3E RID: 6974
		public abstract bool ShouldPartyRecruitPrisoners(PartyBase party);

		// Token: 0x06001B3F RID: 6975
		public abstract int CalculateRecruitableNumber(PartyBase party, CharacterObject character);
	}
}

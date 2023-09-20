using System;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x0200029A RID: 666
	public enum PlayerEncounterState
	{
		// Token: 0x04000B06 RID: 2822
		Begin,
		// Token: 0x04000B07 RID: 2823
		Wait,
		// Token: 0x04000B08 RID: 2824
		PrepareResults,
		// Token: 0x04000B09 RID: 2825
		ApplyResults,
		// Token: 0x04000B0A RID: 2826
		PlayerVictory,
		// Token: 0x04000B0B RID: 2827
		PlayerTotalDefeat,
		// Token: 0x04000B0C RID: 2828
		CaptureHeroes,
		// Token: 0x04000B0D RID: 2829
		FreeHeroes,
		// Token: 0x04000B0E RID: 2830
		LootParty,
		// Token: 0x04000B0F RID: 2831
		LootInventory,
		// Token: 0x04000B10 RID: 2832
		End
	}
}

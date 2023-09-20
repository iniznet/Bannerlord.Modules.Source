using System;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001E5 RID: 485
	public interface ICharacterCreationStateHandler
	{
		// Token: 0x06001C3F RID: 7231
		void OnCharacterCreationFinalized();

		// Token: 0x06001C40 RID: 7232
		void OnRefresh();

		// Token: 0x06001C41 RID: 7233
		void OnStageCreated(CharacterCreationStageBase stage);
	}
}

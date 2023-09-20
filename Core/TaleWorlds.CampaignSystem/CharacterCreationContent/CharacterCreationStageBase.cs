using System;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001E2 RID: 482
	public abstract class CharacterCreationStageBase
	{
		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06001C26 RID: 7206 RVA: 0x0007E3F4 File Offset: 0x0007C5F4
		public CharacterCreationState CharacterCreationState { get; }

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x0007E3FC File Offset: 0x0007C5FC
		// (set) Token: 0x06001C28 RID: 7208 RVA: 0x0007E404 File Offset: 0x0007C604
		public ICharacterCreationStageListener Listener { get; set; }

		// Token: 0x06001C29 RID: 7209 RVA: 0x0007E40D File Offset: 0x0007C60D
		protected CharacterCreationStageBase(CharacterCreationState state)
		{
			this.CharacterCreationState = state;
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x0007E41C File Offset: 0x0007C61C
		protected internal virtual void OnFinalize()
		{
			ICharacterCreationStageListener listener = this.Listener;
			if (listener == null)
			{
				return;
			}
			listener.OnStageFinalize();
		}
	}
}

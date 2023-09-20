using System;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	// Token: 0x0200011C RID: 284
	public class FocusAddedByPlayerEvent : EventBase
	{
		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06001BD7 RID: 7127 RVA: 0x0006456D File Offset: 0x0006276D
		// (set) Token: 0x06001BD8 RID: 7128 RVA: 0x00064575 File Offset: 0x00062775
		public Hero AddedPlayer { get; private set; }

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x0006457E File Offset: 0x0006277E
		// (set) Token: 0x06001BDA RID: 7130 RVA: 0x00064586 File Offset: 0x00062786
		public SkillObject AddedSkill { get; private set; }

		// Token: 0x06001BDB RID: 7131 RVA: 0x0006458F File Offset: 0x0006278F
		public FocusAddedByPlayerEvent(Hero addedPlayer, SkillObject addedSkill)
		{
			this.AddedPlayer = addedPlayer;
			this.AddedSkill = addedSkill;
		}
	}
}

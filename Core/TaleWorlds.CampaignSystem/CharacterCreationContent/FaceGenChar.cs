using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001D7 RID: 471
	public class FaceGenChar
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x0007D9CC File Offset: 0x0007BBCC
		// (set) Token: 0x06001BBA RID: 7098 RVA: 0x0007D9D4 File Offset: 0x0007BBD4
		public BodyProperties BodyProperties { get; private set; }

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x0007D9DD File Offset: 0x0007BBDD
		// (set) Token: 0x06001BBC RID: 7100 RVA: 0x0007D9E5 File Offset: 0x0007BBE5
		public int Race { get; private set; }

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x0007D9EE File Offset: 0x0007BBEE
		// (set) Token: 0x06001BBE RID: 7102 RVA: 0x0007D9F6 File Offset: 0x0007BBF6
		public Equipment Equipment { get; private set; }

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06001BBF RID: 7103 RVA: 0x0007D9FF File Offset: 0x0007BBFF
		// (set) Token: 0x06001BC0 RID: 7104 RVA: 0x0007DA07 File Offset: 0x0007BC07
		public bool IsFemale { get; private set; }

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x0007DA10 File Offset: 0x0007BC10
		// (set) Token: 0x06001BC2 RID: 7106 RVA: 0x0007DA18 File Offset: 0x0007BC18
		public string ActionName { get; set; }

		// Token: 0x06001BC3 RID: 7107 RVA: 0x0007DA21 File Offset: 0x0007BC21
		public FaceGenChar(BodyProperties bodyProperties, int race, Equipment equipment, bool isFemale, string actionName = "act_inventory_idle_start")
		{
			this.BodyProperties = bodyProperties;
			this.Race = race;
			this.Equipment = equipment;
			this.IsFemale = isFemale;
			this.ActionName = actionName;
		}
	}
}

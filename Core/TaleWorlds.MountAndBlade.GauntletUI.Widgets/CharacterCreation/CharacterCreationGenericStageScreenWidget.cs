using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation
{
	// Token: 0x02000166 RID: 358
	public class CharacterCreationGenericStageScreenWidget : Widget
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x0600125E RID: 4702 RVA: 0x00032C43 File Offset: 0x00030E43
		// (set) Token: 0x0600125F RID: 4703 RVA: 0x00032C4B File Offset: 0x00030E4B
		public ButtonWidget NextButton { get; set; }

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06001260 RID: 4704 RVA: 0x00032C54 File Offset: 0x00030E54
		// (set) Token: 0x06001261 RID: 4705 RVA: 0x00032C5C File Offset: 0x00030E5C
		public ButtonWidget PreviousButton { get; set; }

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06001262 RID: 4706 RVA: 0x00032C65 File Offset: 0x00030E65
		// (set) Token: 0x06001263 RID: 4707 RVA: 0x00032C6D File Offset: 0x00030E6D
		public ListPanel ItemList { get; set; }

		// Token: 0x06001264 RID: 4708 RVA: 0x00032C76 File Offset: 0x00030E76
		public CharacterCreationGenericStageScreenWidget(UIContext context)
			: base(context)
		{
		}
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	// Token: 0x02000050 RID: 80
	public class SaveLoadMainHeroVisualWidget : Widget
	{
		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x0000D7AF File Offset: 0x0000B9AF
		// (set) Token: 0x0600043C RID: 1084 RVA: 0x0000D7B7 File Offset: 0x0000B9B7
		public Widget DefaultVisualWidget { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x0000D7C0 File Offset: 0x0000B9C0
		// (set) Token: 0x0600043E RID: 1086 RVA: 0x0000D7C8 File Offset: 0x0000B9C8
		public SaveLoadHeroTableauWidget SaveLoadHeroTableau { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x0000D7D1 File Offset: 0x0000B9D1
		// (set) Token: 0x06000440 RID: 1088 RVA: 0x0000D7D9 File Offset: 0x0000B9D9
		public bool IsVisualDisabledForMemoryPurposes { get; set; }

		// Token: 0x06000441 RID: 1089 RVA: 0x0000D7E2 File Offset: 0x0000B9E2
		public SaveLoadMainHeroVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x0000D7EC File Offset: 0x0000B9EC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.DefaultVisualWidget != null)
			{
				if (this.IsVisualDisabledForMemoryPurposes)
				{
					this.DefaultVisualWidget.IsVisible = true;
					this.SaveLoadHeroTableau.IsVisible = false;
					return;
				}
				this.DefaultVisualWidget.IsVisible = string.IsNullOrEmpty(this.SaveLoadHeroTableau.HeroVisualCode) || !this.SaveLoadHeroTableau.IsVersionCompatible;
				this.SaveLoadHeroTableau.IsVisible = !this.DefaultVisualWidget.IsVisible;
			}
		}
	}
}

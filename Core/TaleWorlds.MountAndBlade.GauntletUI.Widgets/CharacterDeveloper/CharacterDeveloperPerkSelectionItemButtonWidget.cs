using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x0200015E RID: 350
	public class CharacterDeveloperPerkSelectionItemButtonWidget : ButtonWidget
	{
		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x00031EE8 File Offset: 0x000300E8
		// (set) Token: 0x06001209 RID: 4617 RVA: 0x00031EF0 File Offset: 0x000300F0
		public Widget PerkSelectionIndicatorWidget { get; set; }

		// Token: 0x0600120A RID: 4618 RVA: 0x00031EF9 File Offset: 0x000300F9
		public CharacterDeveloperPerkSelectionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00031F04 File Offset: 0x00030104
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.PerkSelectionIndicatorWidget != null)
			{
				if (base.ParentWidget.ChildCount == 1)
				{
					this.PerkSelectionIndicatorWidget.VerticalAlignment = VerticalAlignment.Center;
					return;
				}
				this.PerkSelectionIndicatorWidget.VerticalAlignment = ((base.GetSiblingIndex() % 2 == 0) ? VerticalAlignment.Bottom : VerticalAlignment.Top);
			}
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00031F54 File Offset: 0x00030154
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x00031F5C File Offset: 0x0003015C
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
		}
	}
}

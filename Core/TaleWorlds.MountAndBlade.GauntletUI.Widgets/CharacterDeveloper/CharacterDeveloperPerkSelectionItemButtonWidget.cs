using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class CharacterDeveloperPerkSelectionItemButtonWidget : ButtonWidget
	{
		public Widget PerkSelectionIndicatorWidget { get; set; }

		public CharacterDeveloperPerkSelectionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
		}
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	public class SaveLoadMainHeroVisualWidget : Widget
	{
		public Widget DefaultVisualWidget { get; set; }

		public SaveLoadHeroTableauWidget SaveLoadHeroTableau { get; set; }

		public bool IsVisualDisabledForMemoryPurposes { get; set; }

		public SaveLoadMainHeroVisualWidget(UIContext context)
			: base(context)
		{
		}

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

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class IconOffsetButtonWidget : ButtonWidget
	{
		public int NormalXOffset { get; set; }

		public int NormalYOffset { get; set; }

		public int PressedXOffset { get; set; }

		public int PressedYOffset { get; set; }

		public Widget ButtonIcon { get; set; }

		public IconOffsetButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ButtonIcon != null)
			{
				if (base.IsPressed || base.IsSelected)
				{
					this.ButtonIcon.PositionYOffset = (float)this.PressedYOffset;
					this.ButtonIcon.PositionXOffset = (float)this.PressedXOffset;
					return;
				}
				this.ButtonIcon.PositionYOffset = (float)this.NormalYOffset;
				this.ButtonIcon.PositionXOffset = (float)this.NormalXOffset;
			}
		}

		protected override void RefreshState()
		{
			if (base.IsSelected)
			{
				this.SetState("Selected");
				return;
			}
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			if (base.IsPressed)
			{
				this.SetState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetState("Hovered");
				return;
			}
			this.SetState("Default");
		}
	}
}

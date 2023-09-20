using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class ImageWidget : BrushWidget
	{
		public bool OverrideDefaultStateSwitchingEnabled { get; set; }

		public bool OverrideDefaultStateSwitchingDisabled
		{
			get
			{
				return !this.OverrideDefaultStateSwitchingEnabled;
			}
			set
			{
				if (value != !this.OverrideDefaultStateSwitchingEnabled)
				{
					this.OverrideDefaultStateSwitchingEnabled = !value;
				}
			}
		}

		public ImageWidget(UIContext context)
			: base(context)
		{
			base.AddState("Pressed");
			base.AddState("Hovered");
			base.AddState("Disabled");
		}

		protected override void RefreshState()
		{
			if (!this.OverrideDefaultStateSwitchingEnabled)
			{
				if (base.IsDisabled)
				{
					this.SetState("Disabled");
				}
				else if (base.IsPressed)
				{
					this.SetState("Pressed");
				}
				else if (base.IsHovered)
				{
					this.SetState("Hovered");
				}
				else
				{
					this.SetState("Default");
				}
			}
			base.RefreshState();
		}
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Credits
{
	public class CreditsWidget : Widget
	{
		public CreditsWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.RootItemWidget != null)
			{
				this.RootItemWidget.PositionYOffset = this._currentOffset;
				this._currentOffset -= dt * 75f;
				if (this._currentOffset < -this.RootItemWidget.Size.Y * base._inverseScaleToUse)
				{
					this._currentOffset = 1080f;
				}
			}
		}

		[Editor(false)]
		public Widget RootItemWidget
		{
			get
			{
				return this._rootItemWidget;
			}
			set
			{
				if (this._rootItemWidget != value)
				{
					this._rootItemWidget = value;
					base.OnPropertyChanged<Widget>(value, "RootItemWidget");
				}
			}
		}

		private float _currentOffset = 1080f;

		private Widget _rootItemWidget;
	}
}

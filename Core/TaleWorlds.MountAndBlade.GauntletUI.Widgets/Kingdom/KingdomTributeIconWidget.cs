using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomTributeIconWidget : Widget
	{
		public KingdomTributeIconWidget(UIContext context)
			: base(context)
		{
		}

		public void UpdateIcons(int tribute)
		{
			if (this.PayIcon != null && this.ReceiveIcon != null)
			{
				this.PayIcon.IsVisible = tribute > 0;
				this.ReceiveIcon.IsVisible = tribute < 0;
			}
		}

		public int Tribute
		{
			set
			{
				this.UpdateIcons(value);
			}
		}

		public Widget PayIcon
		{
			get
			{
				return this._payIcon;
			}
			set
			{
				if (value != this._payIcon)
				{
					this._payIcon = value;
				}
			}
		}

		public Widget ReceiveIcon
		{
			get
			{
				return this._receiveIcon;
			}
			set
			{
				if (value != this._receiveIcon)
				{
					this._receiveIcon = value;
				}
			}
		}

		private Widget _payIcon;

		private Widget _receiveIcon;
	}
}

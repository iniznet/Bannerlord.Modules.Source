using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerEndOfRoundPanelBrushWidget : BrushWidget
	{
		public MultiplayerEndOfRoundPanelBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void IsShownUpdated()
		{
			if (this.IsShown)
			{
				string text = (this.IsRoundWinner ? "Victory" : "Defeat");
				base.EventFired(text, Array.Empty<object>());
			}
		}

		[DataSourceProperty]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChanged(value, "IsShown");
					this.IsShownUpdated();
				}
			}
		}

		[DataSourceProperty]
		public bool IsRoundWinner
		{
			get
			{
				return this._isRoundWinner;
			}
			set
			{
				if (value != this._isRoundWinner)
				{
					this._isRoundWinner = value;
					base.OnPropertyChanged(value, "IsRoundWinner");
				}
			}
		}

		private bool _isShown;

		private bool _isRoundWinner;
	}
}

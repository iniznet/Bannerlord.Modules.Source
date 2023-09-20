using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardStatsParentWidget : Widget
	{
		public MultiplayerScoreboardStatsParentWidget(UIContext context)
			: base(context)
		{
		}

		private void RefreshActiveState()
		{
			float num = (this.IsActive ? this.ActiveAlpha : this.InactiveAlpha);
			foreach (Widget widget in base.AllChildren)
			{
				RichTextWidget richTextWidget;
				TextWidget textWidget;
				if ((richTextWidget = widget as RichTextWidget) != null)
				{
					richTextWidget.SetAlpha(num);
				}
				else if ((textWidget = widget as TextWidget) != null)
				{
					textWidget.SetAlpha(num);
				}
			}
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.RefreshActiveState();
				}
			}
		}

		public bool IsInactive
		{
			get
			{
				return !this.IsActive;
			}
			set
			{
				if (value == this.IsActive)
				{
					this.IsActive = !value;
					base.OnPropertyChanged(value, "IsInactive");
				}
			}
		}

		public float ActiveAlpha
		{
			get
			{
				return this._activeAlpha;
			}
			set
			{
				if (value != this._activeAlpha)
				{
					this._activeAlpha = value;
					base.OnPropertyChanged(value, "ActiveAlpha");
				}
			}
		}

		public float InactiveAlpha
		{
			get
			{
				return this._inactiveAlpha;
			}
			set
			{
				if (value != this._inactiveAlpha)
				{
					this._inactiveAlpha = value;
					base.OnPropertyChanged(value, "InactiveAlpha");
				}
			}
		}

		private bool _isActive;

		private float _activeAlpha;

		private float _inactiveAlpha;
	}
}

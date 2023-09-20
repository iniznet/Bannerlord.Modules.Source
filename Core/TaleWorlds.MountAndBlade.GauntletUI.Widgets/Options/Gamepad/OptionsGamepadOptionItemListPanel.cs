using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	public class OptionsGamepadOptionItemListPanel : ListPanel
	{
		public event OptionsGamepadOptionItemListPanel.OnActionTextChangeEvent OnActionTextChanged;

		public OptionsGamepadKeyLocationWidget TargetKey { get; private set; }

		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (this._actionText != value)
				{
					this._actionText = value;
					OptionsGamepadOptionItemListPanel.OnActionTextChangeEvent onActionTextChanged = this.OnActionTextChanged;
					if (onActionTextChanged == null)
					{
						return;
					}
					onActionTextChanged();
				}
			}
		}

		public OptionsGamepadOptionItemListPanel(UIContext context)
			: base(context)
		{
		}

		public void SetKeyProperties(OptionsGamepadKeyLocationWidget currentTarget, Widget parentAreaWidget)
		{
			this.TargetKey = currentTarget;
			this.TargetKey.SetKeyProperties(this.ActionText, parentAreaWidget);
		}

		public int KeyId
		{
			get
			{
				return this._keyId;
			}
			set
			{
				if (value != this._keyId)
				{
					this._keyId = value;
				}
			}
		}

		private string _actionText;

		private int _keyId;

		public delegate void OnActionTextChangeEvent();
	}
}

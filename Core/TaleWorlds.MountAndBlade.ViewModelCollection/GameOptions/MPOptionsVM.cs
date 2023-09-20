using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class MPOptionsVM : OptionsVM
	{
		public MPOptionsVM(bool autoHandleClose, Action onChangeBrightnessRequest, Action onChangeExposureRequest, Action<KeyOptionVM> onKeybindRequest)
			: base(autoHandleClose, OptionsVM.OptionsMode.Multiplayer, onKeybindRequest, onChangeBrightnessRequest, onChangeExposureRequest)
		{
			this.RefreshValues();
		}

		public MPOptionsVM(Action onClose, Action<KeyOptionVM> onKeybindRequest)
			: base(OptionsVM.OptionsMode.Multiplayer, onClose, onKeybindRequest, null, null)
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
			this.RevertText = new TextObject("{=Npqlj5Ln}Revert Changes", null).ToString();
		}

		public new void ExecuteCancel()
		{
			base.ExecuteCancel();
		}

		public void ExecuteApply()
		{
			bool flag = base.IsOptionsChanged();
			base.OnDone();
			InformationManager.DisplayMessage(new InformationMessage(flag ? this._changesAppliedTextObject.ToString() : this._noChangesMadeTextObject.ToString()));
		}

		public void ForceCancel()
		{
			base.HandleCancel(false);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string ApplyText
		{
			get
			{
				return this._applyText;
			}
			set
			{
				if (value != this._applyText)
				{
					this._applyText = value;
					base.OnPropertyChangedWithValue<string>(value, "ApplyText");
				}
			}
		}

		[DataSourceProperty]
		public string RevertText
		{
			get
			{
				return this._revertText;
			}
			set
			{
				if (value != this._revertText)
				{
					this._revertText = value;
					base.OnPropertyChangedWithValue<string>(value, "RevertText");
				}
			}
		}

		private TextObject _changesAppliedTextObject = new TextObject("{=SfsnlbyK}Changes applied.", null);

		private TextObject _noChangesMadeTextObject = new TextObject("{=jS5rrX8M}There are no changes to apply.", null);

		private bool _isEnabled;

		private string _applyText;

		private string _revertText;
	}
}

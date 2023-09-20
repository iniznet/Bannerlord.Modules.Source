using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class ActionOptionDataVM : GenericOptionDataVM
	{
		public ActionOptionDataVM(Action onAction, OptionsVM optionsVM, IOptionData option, TextObject name, TextObject optionActionName, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.ActionOption)
		{
			this._onAction = onAction;
			this._optionActionName = optionActionName;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._optionActionName != null)
			{
				this.ActionName = this._optionActionName.ToString();
			}
		}

		private void ExecuteAction()
		{
			Action onAction = this._onAction;
			if (onAction == null)
			{
				return;
			}
			onAction.DynamicInvokeWithLog(Array.Empty<object>());
		}

		public override void Cancel()
		{
		}

		public override bool IsChanged()
		{
			return false;
		}

		public override void ResetData()
		{
		}

		public override void SetValue(float value)
		{
		}

		public override void UpdateValue()
		{
		}

		public override void ApplyValue()
		{
		}

		[DataSourceProperty]
		public string ActionName
		{
			get
			{
				return this._actionName;
			}
			set
			{
				if (value != this._actionName)
				{
					this._actionName = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionName");
				}
			}
		}

		private readonly Action _onAction;

		private readonly TextObject _optionActionName;

		private string _actionName;
	}
}

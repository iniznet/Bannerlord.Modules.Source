using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu
{
	public class InitialMenuOptionVM : ViewModel
	{
		public InitialMenuOptionVM(InitialStateOption initialStateOption)
		{
			this.InitialStateOption = initialStateOption;
			this.DisabledHint = new HintViewModel(initialStateOption.IsDisabledAndReason().Item2, null);
			this.EnabledHint = new HintViewModel(initialStateOption.EnabledHint, null);
		}

		public void ExecuteAction()
		{
			InitialState initialState = GameStateManager.Current.ActiveState as InitialState;
			if (initialState != null)
			{
				initialState.OnExecutedInitialStateOption(this.InitialStateOption);
				this.InitialStateOption.DoAction();
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisabledHint.HintText = this.InitialStateOption.IsDisabledAndReason().Item2;
		}

		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EnabledHint
		{
			get
			{
				return this._enabledHint;
			}
			set
			{
				if (value != this._enabledHint)
				{
					this._enabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EnabledHint");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this.InitialStateOption.Name.ToString();
			}
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this.InitialStateOption.IsDisabledAndReason().Item1;
			}
		}

		public readonly InitialStateOption InitialStateOption;

		private HintViewModel _disabledHint;

		private HintViewModel _enabledHint;
	}
}

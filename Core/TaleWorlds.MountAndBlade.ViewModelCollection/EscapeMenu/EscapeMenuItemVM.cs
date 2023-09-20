using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	public class EscapeMenuItemVM : ViewModel
	{
		public EscapeMenuItemVM(TextObject item, Action<object> onExecute, object identifier, Func<Tuple<bool, TextObject>> getIsDisabledAndReason, bool isPositiveBehaviored = false)
		{
			this._onExecute = onExecute;
			this._identifier = identifier;
			this._itemObj = item;
			this.ActionText = this._itemObj.ToString();
			this.IsPositiveBehaviored = isPositiveBehaviored;
			this._getIsDisabledAndReason = getIsDisabledAndReason;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			Func<Tuple<bool, TextObject>> getIsDisabledAndReason = this._getIsDisabledAndReason;
			Tuple<bool, TextObject> tuple = ((getIsDisabledAndReason != null) ? getIsDisabledAndReason() : null);
			this.IsDisabled = tuple.Item1;
			this.DisabledHint = new HintViewModel(tuple.Item2, null);
			this.ActionText = this._itemObj.ToString();
		}

		public void ExecuteAction()
		{
			this._onExecute(this._identifier);
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
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPositiveBehaviored
		{
			get
			{
				return this._isPositiveBehaviored;
			}
			set
			{
				if (value != this._isPositiveBehaviored)
				{
					this._isPositiveBehaviored = value;
					base.OnPropertyChangedWithValue(value, "IsPositiveBehaviored");
				}
			}
		}

		private readonly object _identifier;

		private readonly Action<object> _onExecute;

		private readonly TextObject _itemObj;

		private readonly Func<Tuple<bool, TextObject>> _getIsDisabledAndReason;

		private HintViewModel _disabledHint;

		private string _actionText;

		private bool _isDisabled;

		private bool _isPositiveBehaviored;
	}
}

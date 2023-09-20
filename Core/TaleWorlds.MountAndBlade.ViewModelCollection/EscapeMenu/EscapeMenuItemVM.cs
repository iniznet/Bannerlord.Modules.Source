using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	// Token: 0x02000108 RID: 264
	public class EscapeMenuItemVM : ViewModel
	{
		// Token: 0x060017C8 RID: 6088 RVA: 0x0004EB8E File Offset: 0x0004CD8E
		public EscapeMenuItemVM(TextObject item, Action<object> onExecute, object identifier, Func<Tuple<bool, TextObject>> getIsDisabledAndReason, bool isPositiveBehaviored = false)
		{
			this._onExecute = onExecute;
			this._identifier = identifier;
			this._itemObj = item;
			this.ActionText = this._itemObj.ToString();
			this.IsPositiveBehaviored = isPositiveBehaviored;
			this._getIsDisabledAndReason = getIsDisabledAndReason;
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x0004EBCC File Offset: 0x0004CDCC
		public override void RefreshValues()
		{
			base.RefreshValues();
			Func<Tuple<bool, TextObject>> getIsDisabledAndReason = this._getIsDisabledAndReason;
			Tuple<bool, TextObject> tuple = ((getIsDisabledAndReason != null) ? getIsDisabledAndReason() : null);
			this.IsDisabled = tuple.Item1;
			this.DisabledHint = new HintViewModel(tuple.Item2, null);
			this.ActionText = this._itemObj.ToString();
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x0004EC21 File Offset: 0x0004CE21
		public void ExecuteAction()
		{
			this._onExecute(this._identifier);
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x0004EC34 File Offset: 0x0004CE34
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x0004EC3C File Offset: 0x0004CE3C
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

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x060017CD RID: 6093 RVA: 0x0004EC5A File Offset: 0x0004CE5A
		// (set) Token: 0x060017CE RID: 6094 RVA: 0x0004EC62 File Offset: 0x0004CE62
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

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x060017CF RID: 6095 RVA: 0x0004EC85 File Offset: 0x0004CE85
		// (set) Token: 0x060017D0 RID: 6096 RVA: 0x0004EC8D File Offset: 0x0004CE8D
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

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x060017D1 RID: 6097 RVA: 0x0004ECAB File Offset: 0x0004CEAB
		// (set) Token: 0x060017D2 RID: 6098 RVA: 0x0004ECB3 File Offset: 0x0004CEB3
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

		// Token: 0x04000B5E RID: 2910
		private readonly object _identifier;

		// Token: 0x04000B5F RID: 2911
		private readonly Action<object> _onExecute;

		// Token: 0x04000B60 RID: 2912
		private readonly TextObject _itemObj;

		// Token: 0x04000B61 RID: 2913
		private readonly Func<Tuple<bool, TextObject>> _getIsDisabledAndReason;

		// Token: 0x04000B62 RID: 2914
		private HintViewModel _disabledHint;

		// Token: 0x04000B63 RID: 2915
		private string _actionText;

		// Token: 0x04000B64 RID: 2916
		private bool _isDisabled;

		// Token: 0x04000B65 RID: 2917
		private bool _isPositiveBehaviored;
	}
}

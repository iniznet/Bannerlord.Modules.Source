using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class SelectableFiefItemPropertyVM : SelectableItemPropertyVM
	{
		public SelectableFiefItemPropertyVM(string name, string value, int changeAmount, SelectableItemPropertyVM.PropertyType type, BasicTooltipViewModel hint = null, bool isWarning = false)
			: base(name, value, hint)
		{
			this.ChangeAmount = changeAmount;
			this.IsWarning = isWarning;
			base.Type = (int)type;
		}

		[DataSourceProperty]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChangedWithValue(value, "IsWarning");
				}
			}
		}

		[DataSourceProperty]
		public int ChangeAmount
		{
			get
			{
				return this._changeAmount;
			}
			set
			{
				if (value != this._changeAmount)
				{
					this._changeAmount = value;
					base.OnPropertyChangedWithValue(value, "ChangeAmount");
				}
			}
		}

		private bool _isWarning;

		private int _changeAmount;
	}
}

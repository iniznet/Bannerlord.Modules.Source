using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class SelectableFiefItemPropertyVM : SelectableItemPropertyVM
	{
		public SelectableFiefItemPropertyVM(string name, string value, int changeAmount, SelectableItemPropertyVM.PropertyType type, BasicTooltipViewModel hint = null, bool isWarning = false)
			: base(name, value, isWarning, hint)
		{
			this.ChangeAmount = changeAmount;
			base.Type = (int)type;
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

		private int _changeAmount;
	}
}

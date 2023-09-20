using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationOptionVM : StringItemWithActionVM
	{
		public CharacterCreationOptionVM(Action<object> onExecute, string item, object identifier)
			: base(onExecute, item, identifier)
		{
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					base.ExecuteAction();
				}
			}
		}

		private bool _isSelected;
	}
}

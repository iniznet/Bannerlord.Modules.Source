using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationCultureFeatVM : ViewModel
	{
		public CharacterCreationCultureFeatVM(bool isPositive, string description)
		{
			this.IsPositive = isPositive;
			this.Description = description;
		}

		[DataSourceProperty]
		public bool IsPositive
		{
			get
			{
				return this._isPositive;
			}
			set
			{
				if (value != this._isPositive)
				{
					this._isPositive = value;
					base.OnPropertyChangedWithValue(value, "IsPositive");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		private bool _isPositive;

		private string _description;
	}
}

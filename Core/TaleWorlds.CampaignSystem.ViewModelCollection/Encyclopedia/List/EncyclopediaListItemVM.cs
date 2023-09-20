using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListItemVM : ViewModel
	{
		public object Object { get; private set; }

		public EncyclopediaListItem ListItem { get; }

		public EncyclopediaListItemVM(EncyclopediaListItem listItem)
		{
			this.Object = listItem.Object;
			this.Id = listItem.Id;
			this._type = listItem.TypeName;
			this.ListItem = listItem;
			this.PlayerCanSeeValues = listItem.PlayerCanSeeValues;
			this._onShowTooltip = listItem.OnShowTooltip;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.ListItem.Name;
		}

		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._type, this.Id);
		}

		public void SetComparedValue(EncyclopediaListItemComparerBase comparer)
		{
			this.ComparedValue = comparer.GetComparedValueText(this.ListItem);
		}

		public void ExecuteBeginTooltip()
		{
			Action onShowTooltip = this._onShowTooltip;
			if (onShowTooltip == null)
			{
				return;
			}
			onShowTooltip();
		}

		public void ExecuteEndTooltip()
		{
			if (this._onShowTooltip != null)
			{
				MBInformationManager.HideInformations();
			}
		}

		[DataSourceProperty]
		public bool IsFiltered
		{
			get
			{
				return this._isFiltered;
			}
			set
			{
				if (value != this._isFiltered)
				{
					this._isFiltered = value;
					base.OnPropertyChangedWithValue(value, "IsFiltered");
				}
			}
		}

		[DataSourceProperty]
		public bool PlayerCanSeeValues
		{
			get
			{
				return this._playerCanSeeValues;
			}
			set
			{
				if (value != this._playerCanSeeValues)
				{
					this._playerCanSeeValues = value;
					base.OnPropertyChangedWithValue(value, "PlayerCanSeeValues");
				}
			}
		}

		[DataSourceProperty]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value != this._id)
				{
					this._id = value;
					base.OnPropertyChangedWithValue<string>(value, "Id");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string ComparedValue
		{
			get
			{
				return this._comparedValue;
			}
			set
			{
				if (value != this._comparedValue)
				{
					this._comparedValue = value;
					base.OnPropertyChangedWithValue<string>(value, "ComparedValue");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBookmarked
		{
			get
			{
				return this._isBookmarked;
			}
			set
			{
				if (value != this._isBookmarked)
				{
					this._isBookmarked = value;
					base.OnPropertyChangedWithValue(value, "IsBookmarked");
				}
			}
		}

		private readonly string _type;

		private readonly Action _onShowTooltip;

		private string _id;

		private string _name;

		private string _comparedValue;

		private bool _isFiltered;

		private bool _isBookmarked;

		private bool _playerCanSeeValues;
	}
}

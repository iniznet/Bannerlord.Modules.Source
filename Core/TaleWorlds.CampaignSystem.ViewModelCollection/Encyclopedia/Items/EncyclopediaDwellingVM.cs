using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	public class EncyclopediaDwellingVM : ViewModel
	{
		public EncyclopediaDwellingVM(WorkshopType workshop)
		{
			this._workshop = workshop;
			this.FileName = workshop.StringId;
			this.RefreshValues();
		}

		public EncyclopediaDwellingVM(VillageType villageType)
		{
			this._villageType = villageType;
			this.FileName = villageType.StringId;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._workshop != null)
			{
				this.NameText = this._workshop.Name.ToString();
				return;
			}
			if (this._villageType != null)
			{
				this.NameText = this._villageType.ShortName.ToString();
			}
		}

		[DataSourceProperty]
		public string FileName
		{
			get
			{
				return this._fileName;
			}
			set
			{
				if (value != this._fileName)
				{
					this._fileName = value;
					base.OnPropertyChangedWithValue<string>(value, "FileName");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		private readonly WorkshopType _workshop;

		private readonly VillageType _villageType;

		private string _fileName;

		private string _nameText;
	}
}

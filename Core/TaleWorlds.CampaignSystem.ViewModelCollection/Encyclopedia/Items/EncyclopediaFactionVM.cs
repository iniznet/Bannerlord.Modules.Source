using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	public class EncyclopediaFactionVM : ViewModel
	{
		public IFaction Faction { get; private set; }

		public EncyclopediaFactionVM(IFaction faction)
		{
			this.Faction = faction;
			if (faction != null)
			{
				this.ImageIdentifier = new ImageIdentifierVM(BannerCode.CreateFrom(faction.Banner), true);
				this.IsDestroyed = faction.IsEliminated;
			}
			else
			{
				this.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.IsDestroyed = false;
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Faction != null)
			{
				this.NameText = this.Faction.Name.ToString();
				return;
			}
			this.NameText = new TextObject("{=2abtb4xu}Independent", null).ToString();
		}

		public void ExecuteLink()
		{
			if (this.Faction != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Faction.EncyclopediaLink);
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChanged("Banner");
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

		[DataSourceProperty]
		public bool IsDestroyed
		{
			get
			{
				return this._isDestroyed;
			}
			set
			{
				if (value != this._isDestroyed)
				{
					this._isDestroyed = value;
					base.OnPropertyChangedWithValue(value, "IsDestroyed");
				}
			}
		}

		private ImageIdentifierVM _imageIdentifier;

		private string _nameText;

		private bool _isDestroyed;
	}
}

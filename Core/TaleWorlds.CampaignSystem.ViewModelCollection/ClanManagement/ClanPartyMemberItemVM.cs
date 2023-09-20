using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanPartyMemberItemVM : ViewModel
	{
		public Hero HeroObject { get; private set; }

		public ClanPartyMemberItemVM(Hero hero, MobileParty party)
		{
			this.HeroObject = hero;
			this.IsLeader = hero == party.LeaderHero;
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false);
			this.Visual = new ImageIdentifierVM(characterCode);
			this.HeroModel = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.HeroModel.FillFrom(this.HeroObject, -1, false, false);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroObject.Name.ToString();
			this.UpdateProperties();
		}

		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		public void UpdateProperties()
		{
			this.HeroModel = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.HeroModel.FillFrom(this.HeroObject, -1, false, false);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(this.HeroObject.ClanBanner), true);
		}

		public void ExecuteLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.HeroObject.EncyclopediaLink);
		}

		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[] { this.HeroObject, true });
		}

		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroModel.OnFinalize();
		}

		[DataSourceProperty]
		public HeroViewModel HeroModel
		{
			get
			{
				return this._heroModel;
			}
			set
			{
				if (value != this._heroModel)
				{
					this._heroModel = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "HeroModel");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Banner_9
		{
			get
			{
				return this._banner_9;
			}
			set
			{
				if (value != this._banner_9)
				{
					this._banner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner_9");
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
		public bool IsLeader
		{
			get
			{
				return this._isLeader;
			}
			set
			{
				if (value != this._isLeader)
				{
					this._isLeader = value;
					base.OnPropertyChangedWithValue(value, "IsLeader");
				}
			}
		}

		private ImageIdentifierVM _visual;

		private ImageIdentifierVM _banner_9;

		private string _name;

		private bool _isLeader;

		private HeroViewModel _heroModel;
	}
}

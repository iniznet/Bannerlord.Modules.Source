using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MarriageOfferPopup
{
	public class MarriageOfferPopupHeroVM : ViewModel
	{
		public Hero Hero { get; }

		public MarriageOfferPopupHeroVM(Hero hero)
		{
			this.Hero = hero;
			this.Model = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.FillHeroInformation();
			this.CreateClanBanner();
			this.RefreshValues();
		}

		public void Update()
		{
			TextObject textObject;
			if (!this._modelCreated && !CampaignUIHelper.IsHeroInformationHidden(this.Hero, out textObject))
			{
				this._modelCreated = true;
				this.CreateHeroModel();
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.EncyclopediaLinkWithName = this.Hero.EncyclopediaLinkWithName.ToString();
			this.AgeString = ((int)this.Hero.Age).ToString();
			this.OccupationString = CampaignUIHelper.GetHeroOccupationName(this.Hero);
			this.Relation = (int)this.Hero.GetRelationWithPlayer();
		}

		public override void OnFinalize()
		{
			HeroViewModel model = this.Model;
			if (model != null)
			{
				model.OnFinalize();
			}
			MBBindingList<EncyclopediaTraitItemVM> traits = this.Traits;
			if (traits != null)
			{
				traits.Clear();
			}
			MBBindingList<MarriageOfferPopupHeroAttributeVM> skills = this.Skills;
			if (skills != null)
			{
				skills.Clear();
			}
			base.OnFinalize();
		}

		public void ExecuteHeroLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.Hero.EncyclopediaLink);
		}

		public void ExecuteClanLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.Hero.Clan.EncyclopediaLink);
		}

		private void CreateClanBanner()
		{
			this.ClanName = this.Hero.Clan.Name.ToString();
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.Hero.ClanBanner), true);
		}

		private void CreateHeroModel()
		{
			this.Model.FillFrom(this.Hero, -1, true, true);
			this.Model.SetEquipment(EquipmentIndex.ArmorItemEndSlot, default(EquipmentElement));
		}

		private void FillHeroInformation()
		{
			this.Traits = new MBBindingList<EncyclopediaTraitItemVM>();
			this.Skills = new MBBindingList<MarriageOfferPopupHeroAttributeVM>();
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				this.Skills.Add(new MarriageOfferPopupHeroAttributeVM(this.Hero, characterAttribute));
			}
			foreach (TraitObject traitObject in CampaignUIHelper.GetHeroTraits())
			{
				if (this.Hero.GetTraitLevel(traitObject) != 0)
				{
					this.Traits.Add(new EncyclopediaTraitItemVM(traitObject, this.Hero));
				}
			}
		}

		[DataSourceProperty]
		public string EncyclopediaLinkWithName
		{
			get
			{
				return this._encyclopediaLinkWithName;
			}
			set
			{
				if (value != this._encyclopediaLinkWithName)
				{
					this._encyclopediaLinkWithName = value;
					base.OnPropertyChangedWithValue<string>(value, "EncyclopediaLinkWithName");
				}
			}
		}

		[DataSourceProperty]
		public string AgeString
		{
			get
			{
				return this._ageString;
			}
			set
			{
				if (value != this._ageString)
				{
					this._ageString = value;
					base.OnPropertyChangedWithValue<string>(value, "AgeString");
				}
			}
		}

		[DataSourceProperty]
		public string OccupationString
		{
			get
			{
				return this._occupationString;
			}
			set
			{
				if (value != this._occupationString)
				{
					this._occupationString = value;
					base.OnPropertyChangedWithValue<string>(value, "OccupationString");
				}
			}
		}

		[DataSourceProperty]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (value != this._relation)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		[DataSourceProperty]
		public string ClanName
		{
			get
			{
				return this._clanName;
			}
			set
			{
				if (value != this._clanName)
				{
					this._clanName = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanName");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		[DataSourceProperty]
		public HeroViewModel Model
		{
			get
			{
				return this._model;
			}
			set
			{
				if (value != this._model)
				{
					this._model = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "Model");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EncyclopediaTraitItemVM> Traits
		{
			get
			{
				return this._traits;
			}
			set
			{
				if (value != this._traits)
				{
					this._traits = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTraitItemVM>>(value, "Traits");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MarriageOfferPopupHeroAttributeVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<MarriageOfferPopupHeroAttributeVM>>(value, "Skills");
				}
			}
		}

		private bool _modelCreated;

		private string _encyclopediaLinkWithName;

		private string _ageString;

		private string _occupationString;

		private int _relation;

		private string _clanName;

		private ImageIdentifierVM _clanBanner;

		private HeroViewModel _model;

		private MBBindingList<EncyclopediaTraitItemVM> _traits;

		private MBBindingList<MarriageOfferPopupHeroAttributeVM> _skills;
	}
}

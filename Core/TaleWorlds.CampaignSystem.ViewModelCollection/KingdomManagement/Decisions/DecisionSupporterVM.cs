using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	public class DecisionSupporterVM : ViewModel
	{
		public DecisionSupporterVM(TextObject name, string imagePath, Clan clan, Supporter.SupportWeights weight)
		{
			this._nameObj = name;
			this._clan = clan;
			this._weight = weight;
			this.SupportWeightImagePath = DecisionSupporterVM.GetSupporterWeightImagePath(weight);
			this.RefreshValues();
			this._hero = Hero.FindFirst((Hero H) => H.Name == name);
			if (this._hero != null)
			{
				this.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(this._hero.CharacterObject, false));
				return;
			}
			this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
		}

		private void ExecuteBeginHint()
		{
			if (this._hero != null)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[] { this._hero, false });
			}
		}

		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		internal static string GetSupporterWeightImagePath(Supporter.SupportWeights weight)
		{
			switch (weight)
			{
			case Supporter.SupportWeights.SlightlyFavor:
				return "SPKingdom\\voter_strength1";
			case Supporter.SupportWeights.StronglyFavor:
				return "SPKingdom\\voter_strength2";
			case Supporter.SupportWeights.FullyPush:
				return "SPKingdom\\voter_strength3";
			}
			return string.Empty;
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
		public int SupportStrength
		{
			get
			{
				return this._supportStrength;
			}
			set
			{
				if (value != this._supportStrength)
				{
					this._supportStrength = value;
					base.OnPropertyChangedWithValue(value, "SupportStrength");
				}
			}
		}

		[DataSourceProperty]
		public string SupportWeightImagePath
		{
			get
			{
				return this._supportWeightImagePath;
			}
			set
			{
				if (value != this._supportWeightImagePath)
				{
					this._supportWeightImagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "SupportWeightImagePath");
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
					base.OnPropertyChanged("string");
				}
			}
		}

		private Supporter.SupportWeights _weight;

		private Clan _clan;

		private TextObject _nameObj;

		private Hero _hero;

		private ImageIdentifierVM _visual;

		private string _name;

		private int _supportStrength;

		private string _supportWeightImagePath;
	}
}

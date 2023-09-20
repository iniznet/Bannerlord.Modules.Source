using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	// Token: 0x02000065 RID: 101
	public class DecisionSupporterVM : ViewModel
	{
		// Token: 0x060008B2 RID: 2226 RVA: 0x0002444C File Offset: 0x0002264C
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

		// Token: 0x060008B3 RID: 2227 RVA: 0x000244E2 File Offset: 0x000226E2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x000244FB File Offset: 0x000226FB
		private void ExecuteBeginHint()
		{
			if (this._hero != null)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[] { this._hero, false });
			}
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x0002452C File Offset: 0x0002272C
		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x00024533 File Offset: 0x00022733
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

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x00024568 File Offset: 0x00022768
		// (set) Token: 0x060008B8 RID: 2232 RVA: 0x00024570 File Offset: 0x00022770
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

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x0002458E File Offset: 0x0002278E
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x00024596 File Offset: 0x00022796
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

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x000245B4 File Offset: 0x000227B4
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x000245BC File Offset: 0x000227BC
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

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x000245DF File Offset: 0x000227DF
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x000245E7 File Offset: 0x000227E7
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

		// Token: 0x040003EB RID: 1003
		private Supporter.SupportWeights _weight;

		// Token: 0x040003EC RID: 1004
		private Clan _clan;

		// Token: 0x040003ED RID: 1005
		private TextObject _nameObj;

		// Token: 0x040003EE RID: 1006
		private Hero _hero;

		// Token: 0x040003EF RID: 1007
		private ImageIdentifierVM _visual;

		// Token: 0x040003F0 RID: 1008
		private string _name;

		// Token: 0x040003F1 RID: 1009
		private int _supportStrength;

		// Token: 0x040003F2 RID: 1010
		private string _supportWeightImagePath;
	}
}

using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	// Token: 0x02000074 RID: 116
	public class KingdomArmyPartyItemVM : ViewModel
	{
		// Token: 0x06000A51 RID: 2641 RVA: 0x00029355 File Offset: 0x00027555
		public KingdomArmyPartyItemVM(MobileParty party)
		{
			this._party = party;
			Hero leaderHero = party.LeaderHero;
			this.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode((leaderHero != null) ? leaderHero.CharacterObject : null, false));
			this.RefreshValues();
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0002938D File Offset: 0x0002758D
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._party.Name.ToString();
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x000293AB File Offset: 0x000275AB
		private void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(MobileParty), new object[] { this._party, true, false });
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x000293DD File Offset: 0x000275DD
		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x000293E4 File Offset: 0x000275E4
		public void ExecuteLink()
		{
			if (this._party != null && this._party.LeaderHero != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._party.LeaderHero.EncyclopediaLink);
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x0002941A File Offset: 0x0002761A
		// (set) Token: 0x06000A57 RID: 2647 RVA: 0x00029422 File Offset: 0x00027622
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

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x00029440 File Offset: 0x00027640
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x00029448 File Offset: 0x00027648
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

		// Token: 0x040004A6 RID: 1190
		private MobileParty _party;

		// Token: 0x040004A7 RID: 1191
		private ImageIdentifierVM _visual;

		// Token: 0x040004A8 RID: 1192
		private string _name;
	}
}

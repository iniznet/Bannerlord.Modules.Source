using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	// Token: 0x0200006F RID: 111
	public class KingdomClanFiefItemVM : ViewModel
	{
		// Token: 0x060009C2 RID: 2498 RVA: 0x00027AF8 File Offset: 0x00025CF8
		public KingdomClanFiefItemVM(Settlement settlement)
		{
			this.Settlement = settlement;
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.VisualPath = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.RefreshValues();
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00027B3F File Offset: 0x00025D3F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FiefName = this.Settlement.Name.ToString();
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00027B5D File Offset: 0x00025D5D
		private void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00027B86 File Offset: 0x00025D86
		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00027B8D File Offset: 0x00025D8D
		public void ExecuteLink()
		{
			if (this.Settlement != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060009C7 RID: 2503 RVA: 0x00027BB1 File Offset: 0x00025DB1
		// (set) Token: 0x060009C8 RID: 2504 RVA: 0x00027BB9 File Offset: 0x00025DB9
		[DataSourceProperty]
		public string VisualPath
		{
			get
			{
				return this._visualPath;
			}
			set
			{
				if (value != this._visualPath)
				{
					this._visualPath = value;
					base.OnPropertyChanged("FileName");
				}
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x00027BDB File Offset: 0x00025DDB
		// (set) Token: 0x060009CA RID: 2506 RVA: 0x00027BE3 File Offset: 0x00025DE3
		[DataSourceProperty]
		public string FiefName
		{
			get
			{
				return this._fiefName;
			}
			set
			{
				if (value != this._fiefName)
				{
					this._fiefName = value;
					base.OnPropertyChangedWithValue<string>(value, "FiefName");
				}
			}
		}

		// Token: 0x04000465 RID: 1125
		private readonly Settlement Settlement;

		// Token: 0x04000466 RID: 1126
		private string _visualPath;

		// Token: 0x04000467 RID: 1127
		private string _fiefName;
	}
}

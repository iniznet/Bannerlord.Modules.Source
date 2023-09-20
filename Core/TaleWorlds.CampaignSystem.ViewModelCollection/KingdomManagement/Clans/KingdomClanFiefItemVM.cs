using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	public class KingdomClanFiefItemVM : ViewModel
	{
		public KingdomClanFiefItemVM(Settlement settlement)
		{
			this.Settlement = settlement;
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.VisualPath = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FiefName = this.Settlement.Name.ToString();
		}

		private void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
		}

		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteLink()
		{
			if (this.Settlement != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
			}
		}

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

		private readonly Settlement Settlement;

		private string _visualPath;

		private string _fiefName;
	}
}

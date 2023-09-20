using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C8 RID: 200
	public class EncyclopediaSettlementVM : ViewModel
	{
		// Token: 0x0600131A RID: 4890 RVA: 0x00049884 File Offset: 0x00047A84
		public EncyclopediaSettlementVM(Settlement settlement)
		{
			if (!settlement.IsHideout)
			{
				this._settlement = settlement;
			}
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.FileName = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.RefreshValues();
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x000498D3 File Offset: 0x00047AD3
		public override void RefreshValues()
		{
			base.RefreshValues();
			Settlement settlement = this._settlement;
			this.NameText = ((settlement != null) ? settlement.Name.ToString() : null) ?? "";
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x00049901 File Offset: 0x00047B01
		public void ExecuteLink()
		{
			if (this._settlement != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._settlement.EncyclopediaLink);
			}
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x00049925 File Offset: 0x00047B25
		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0004992C File Offset: 0x00047B2C
		public void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[] { this._settlement, true });
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x00049955 File Offset: 0x00047B55
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x0004995D File Offset: 0x00047B5D
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

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06001321 RID: 4897 RVA: 0x00049980 File Offset: 0x00047B80
		// (set) Token: 0x06001322 RID: 4898 RVA: 0x00049988 File Offset: 0x00047B88
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

		// Token: 0x040008DA RID: 2266
		private Settlement _settlement;

		// Token: 0x040008DB RID: 2267
		private string _fileName;

		// Token: 0x040008DC RID: 2268
		private string _nameText;
	}
}

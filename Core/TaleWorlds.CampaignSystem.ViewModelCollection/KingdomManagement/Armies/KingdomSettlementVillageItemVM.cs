using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	// Token: 0x02000077 RID: 119
	public class KingdomSettlementVillageItemVM : ViewModel
	{
		// Token: 0x06000AB6 RID: 2742 RVA: 0x0002A4FD File Offset: 0x000286FD
		public KingdomSettlementVillageItemVM(Village village)
		{
			this._village = village;
			this.VisualPath = village.BackgroundMeshName + "_t";
			this.RefreshValues();
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0002A528 File Offset: 0x00028728
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._village.Name.ToString();
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0002A546 File Offset: 0x00028746
		private void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[]
			{
				this._village.Settlement,
				true
			});
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0002A574 File Offset: 0x00028774
		private void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0002A57B File Offset: 0x0002877B
		public void ExecuteLink()
		{
			if (this._village != null && this._village.Settlement != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._village.Settlement.EncyclopediaLink);
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x0002A5B1 File Offset: 0x000287B1
		// (set) Token: 0x06000ABC RID: 2748 RVA: 0x0002A5B9 File Offset: 0x000287B9
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

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x0002A5D7 File Offset: 0x000287D7
		// (set) Token: 0x06000ABE RID: 2750 RVA: 0x0002A5DF File Offset: 0x000287DF
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

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000ABF RID: 2751 RVA: 0x0002A602 File Offset: 0x00028802
		// (set) Token: 0x06000AC0 RID: 2752 RVA: 0x0002A60A File Offset: 0x0002880A
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
					base.OnPropertyChangedWithValue<string>(value, "VisualPath");
				}
			}
		}

		// Token: 0x040004D7 RID: 1239
		private Village _village;

		// Token: 0x040004D8 RID: 1240
		private ImageIdentifierVM _visual;

		// Token: 0x040004D9 RID: 1241
		private string _name;

		// Token: 0x040004DA RID: 1242
		private string _visualPath;
	}
}

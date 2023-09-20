using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x02000018 RID: 24
	public class SettlementNameplatePartyMarkerItemVM : ViewModel
	{
		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000B7BD File Offset: 0x000099BD
		// (set) Token: 0x06000246 RID: 582 RVA: 0x0000B7C5 File Offset: 0x000099C5
		public MobileParty Party { get; private set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000247 RID: 583 RVA: 0x0000B7CE File Offset: 0x000099CE
		// (set) Token: 0x06000248 RID: 584 RVA: 0x0000B7D6 File Offset: 0x000099D6
		public int SortIndex { get; private set; }

		// Token: 0x06000249 RID: 585 RVA: 0x0000B7E0 File Offset: 0x000099E0
		public SettlementNameplatePartyMarkerItemVM(MobileParty mobileParty)
		{
			this.Party = mobileParty;
			if (mobileParty.IsCaravan)
			{
				this.IsCaravan = true;
				this.SortIndex = 1;
				return;
			}
			if (mobileParty.IsLordParty && mobileParty.LeaderHero != null)
			{
				this.IsLord = true;
				IFaction mapFaction = mobileParty.MapFaction;
				this.Visual = new ImageIdentifierVM(BannerCode.CreateFrom((mapFaction != null) ? mapFaction.Banner : null), true);
				this.SortIndex = 0;
				return;
			}
			this.IsDefault = true;
			this.SortIndex = 2;
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000B861 File Offset: 0x00009A61
		// (set) Token: 0x0600024B RID: 587 RVA: 0x0000B869 File Offset: 0x00009A69
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600024C RID: 588 RVA: 0x0000B887 File Offset: 0x00009A87
		// (set) Token: 0x0600024D RID: 589 RVA: 0x0000B88F File Offset: 0x00009A8F
		public bool IsCaravan
		{
			get
			{
				return this._isCaravan;
			}
			set
			{
				if (value != this._isCaravan)
				{
					this._isCaravan = value;
					base.OnPropertyChangedWithValue(value, "IsCaravan");
				}
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000B8AD File Offset: 0x00009AAD
		// (set) Token: 0x0600024F RID: 591 RVA: 0x0000B8B5 File Offset: 0x00009AB5
		public bool IsLord
		{
			get
			{
				return this._isLord;
			}
			set
			{
				if (value != this._isLord)
				{
					this._isLord = value;
					base.OnPropertyChangedWithValue(value, "IsLord");
				}
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000B8D3 File Offset: 0x00009AD3
		// (set) Token: 0x06000251 RID: 593 RVA: 0x0000B8DB File Offset: 0x00009ADB
		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
			set
			{
				if (value != this._isDefault)
				{
					this._isDefault = value;
					base.OnPropertyChangedWithValue(value, "IsDefault");
				}
			}
		}

		// Token: 0x04000117 RID: 279
		private ImageIdentifierVM _visual;

		// Token: 0x04000118 RID: 280
		private bool _isCaravan;

		// Token: 0x04000119 RID: 281
		private bool _isLord;

		// Token: 0x0400011A RID: 282
		private bool _isDefault;
	}
}

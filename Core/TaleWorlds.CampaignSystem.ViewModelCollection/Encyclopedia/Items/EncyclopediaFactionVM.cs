using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C5 RID: 197
	public class EncyclopediaFactionVM : ViewModel
	{
		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06001304 RID: 4868 RVA: 0x0004960E File Offset: 0x0004780E
		// (set) Token: 0x06001305 RID: 4869 RVA: 0x00049616 File Offset: 0x00047816
		public IFaction Faction { get; private set; }

		// Token: 0x06001306 RID: 4870 RVA: 0x00049620 File Offset: 0x00047820
		public EncyclopediaFactionVM(IFaction faction)
		{
			this.Faction = faction;
			if (faction != null)
			{
				this.ImageIdentifier = new ImageIdentifierVM(BannerCode.CreateFrom(faction.Banner), true);
				this.IsDestroyed = faction.IsEliminated;
			}
			else
			{
				this.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.IsDestroyed = false;
			}
			this.RefreshValues();
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0004967B File Offset: 0x0004787B
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Faction != null)
			{
				this.NameText = this.Faction.Name.ToString();
				return;
			}
			this.NameText = new TextObject("{=2abtb4xu}Independent", null).ToString();
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000496B8 File Offset: 0x000478B8
		public void ExecuteLink()
		{
			if (this.Faction != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Faction.EncyclopediaLink);
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x000496DC File Offset: 0x000478DC
		// (set) Token: 0x0600130A RID: 4874 RVA: 0x000496E4 File Offset: 0x000478E4
		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChanged("Banner");
				}
			}
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x00049701 File Offset: 0x00047901
		// (set) Token: 0x0600130C RID: 4876 RVA: 0x00049709 File Offset: 0x00047909
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

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x0600130D RID: 4877 RVA: 0x0004972C File Offset: 0x0004792C
		// (set) Token: 0x0600130E RID: 4878 RVA: 0x00049734 File Offset: 0x00047934
		[DataSourceProperty]
		public bool IsDestroyed
		{
			get
			{
				return this._isDestroyed;
			}
			set
			{
				if (value != this._isDestroyed)
				{
					this._isDestroyed = value;
					base.OnPropertyChangedWithValue(value, "IsDestroyed");
				}
			}
		}

		// Token: 0x040008D2 RID: 2258
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x040008D3 RID: 2259
		private string _nameText;

		// Token: 0x040008D4 RID: 2260
		private bool _isDestroyed;
	}
}

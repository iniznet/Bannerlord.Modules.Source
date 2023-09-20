using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C4 RID: 196
	public class EncyclopediaDwellingVM : ViewModel
	{
		// Token: 0x060012FD RID: 4861 RVA: 0x00049524 File Offset: 0x00047724
		public EncyclopediaDwellingVM(WorkshopType workshop)
		{
			this._workshop = workshop;
			this.FileName = workshop.StringId;
			this.RefreshValues();
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x00049545 File Offset: 0x00047745
		public EncyclopediaDwellingVM(VillageType villageType)
		{
			this._villageType = villageType;
			this.FileName = villageType.StringId;
			this.RefreshValues();
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x00049568 File Offset: 0x00047768
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._workshop != null)
			{
				this.NameText = this._workshop.Name.ToString();
				return;
			}
			if (this._villageType != null)
			{
				this.NameText = this._villageType.ShortName.ToString();
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001300 RID: 4864 RVA: 0x000495B8 File Offset: 0x000477B8
		// (set) Token: 0x06001301 RID: 4865 RVA: 0x000495C0 File Offset: 0x000477C0
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

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001302 RID: 4866 RVA: 0x000495E3 File Offset: 0x000477E3
		// (set) Token: 0x06001303 RID: 4867 RVA: 0x000495EB File Offset: 0x000477EB
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

		// Token: 0x040008CD RID: 2253
		private readonly WorkshopType _workshop;

		// Token: 0x040008CE RID: 2254
		private readonly VillageType _villageType;

		// Token: 0x040008CF RID: 2255
		private string _fileName;

		// Token: 0x040008D0 RID: 2256
		private string _nameText;
	}
}

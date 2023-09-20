using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000DC RID: 220
	public class CraftingItemFlagVM : ItemFlagVM
	{
		// Token: 0x06001498 RID: 5272 RVA: 0x0004DE6D File Offset: 0x0004C06D
		public CraftingItemFlagVM(string iconPath, TextObject hint, bool isDisplayed)
			: base(iconPath, hint)
		{
			this.IsDisplayed = isDisplayed;
			this.IconPath = "SPGeneral\\" + iconPath;
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x0004DE8F File Offset: 0x0004C08F
		// (set) Token: 0x0600149A RID: 5274 RVA: 0x0004DE97 File Offset: 0x0004C097
		[DataSourceProperty]
		public bool IsDisplayed
		{
			get
			{
				return this._isDisplayed;
			}
			set
			{
				if (value != this._isDisplayed)
				{
					this._isDisplayed = value;
					base.OnPropertyChangedWithValue(value, "IsDisplayed");
				}
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x0004DEB5 File Offset: 0x0004C0B5
		// (set) Token: 0x0600149C RID: 5276 RVA: 0x0004DEBD File Offset: 0x0004C0BD
		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
				}
			}
		}

		// Token: 0x04000998 RID: 2456
		private bool _isDisplayed;

		// Token: 0x04000999 RID: 2457
		private string _iconPath;
	}
}

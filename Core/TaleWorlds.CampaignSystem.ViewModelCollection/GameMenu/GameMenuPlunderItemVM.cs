using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	// Token: 0x02000087 RID: 135
	public class GameMenuPlunderItemVM : ViewModel
	{
		// Token: 0x06000D4E RID: 3406 RVA: 0x00035C30 File Offset: 0x00033E30
		public GameMenuPlunderItemVM(ItemRosterElement item)
		{
			this._item = item;
			this.Visual = new ImageIdentifierVM(item.EquipmentElement.Item, "");
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00035C6C File Offset: 0x00033E6C
		public void ExecuteBeginTooltip()
		{
			if (this._item.EquipmentElement.Item != null)
			{
				InformationManager.ShowTooltip(typeof(ItemObject), new object[] { this._item.EquipmentElement });
			}
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00035CB6 File Offset: 0x00033EB6
		public void ExecuteEndTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x00035CBD File Offset: 0x00033EBD
		// (set) Token: 0x06000D52 RID: 3410 RVA: 0x00035CC5 File Offset: 0x00033EC5
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

		// Token: 0x04000624 RID: 1572
		private ItemRosterElement _item;

		// Token: 0x04000625 RID: 1573
		private ImageIdentifierVM _visual;
	}
}

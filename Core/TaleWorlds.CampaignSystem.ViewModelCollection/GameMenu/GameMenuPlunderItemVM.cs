using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	public class GameMenuPlunderItemVM : ViewModel
	{
		public GameMenuPlunderItemVM(ItemRosterElement item)
		{
			this._item = item;
			this.Visual = new ImageIdentifierVM(item.EquipmentElement.Item, "");
		}

		public void ExecuteBeginTooltip()
		{
			if (this._item.EquipmentElement.Item != null)
			{
				InformationManager.ShowTooltip(typeof(ItemObject), new object[] { this._item.EquipmentElement });
			}
		}

		public void ExecuteEndTooltip()
		{
			MBInformationManager.HideInformations();
		}

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

		private ItemRosterElement _item;

		private ImageIdentifierVM _visual;
	}
}

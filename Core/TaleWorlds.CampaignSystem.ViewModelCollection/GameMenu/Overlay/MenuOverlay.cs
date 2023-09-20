using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	public class MenuOverlay : Attribute
	{
		public MenuOverlay(string typeId)
		{
			this.TypeId = typeId;
		}

		public new string TypeId;
	}
}

using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	public class PerkSelectionToggleEvent : EventBase
	{
		public bool IsCurrentlyActive { get; private set; }

		public PerkSelectionToggleEvent(bool isCurrentlyActive)
		{
			this.IsCurrentlyActive = isCurrentlyActive;
		}
	}
}

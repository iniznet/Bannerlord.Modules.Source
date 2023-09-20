using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public class FaceGenChar
	{
		public BodyProperties BodyProperties { get; private set; }

		public int Race { get; private set; }

		public Equipment Equipment { get; private set; }

		public bool IsFemale { get; private set; }

		public string ActionName { get; set; }

		public FaceGenChar(BodyProperties bodyProperties, int race, Equipment equipment, bool isFemale, string actionName = "act_inventory_idle_start")
		{
			this.BodyProperties = bodyProperties;
			this.Race = race;
			this.Equipment = equipment;
			this.IsFemale = isFemale;
			this.ActionName = actionName;
		}
	}
}

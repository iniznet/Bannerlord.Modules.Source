using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	public class PartyMovement
	{
		public PartyMovementType MoveType { get; set; }

		public MobileParty Party { get; set; }

		public Settlement Settlement { get; set; }

		public float Score { get; set; }

		private PartyMovement()
		{
			this.MoveType = PartyMovementType.None;
			this.Settlement = null;
			this.Party = null;
			this.Score = 0f;
		}
	}
}

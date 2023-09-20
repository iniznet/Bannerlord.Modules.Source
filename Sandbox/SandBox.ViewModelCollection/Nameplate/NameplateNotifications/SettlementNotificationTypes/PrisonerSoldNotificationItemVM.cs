using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	public class PrisonerSoldNotificationItemVM : SettlementNotificationItemBaseVM
	{
		public MobileParty Party { get; private set; }

		public PrisonerSoldNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, MobileParty party, TroopRoster prisoners, int createdTick)
			: base(onRemove, createdTick)
		{
			this._prisonersAmount = prisoners.TotalManCount;
			base.Text = SandBoxUIHelper.GetPrisonersSoldNotificationText(this._prisonersAmount);
			this.Party = party;
			base.CharacterName = ((party.LeaderHero != null) ? party.LeaderHero.Name.ToString() : party.Name.ToString());
			base.CharacterVisual = new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(PartyBaseHelper.GetVisualPartyLeader(party.Party), false));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			if (party.LeaderHero != null)
			{
				base.RelationType = (party.LeaderHero.Clan.IsAtWarWith(Hero.MainHero.Clan) ? (-1) : 1);
			}
		}

		public void AddNewPrisoners(TroopRoster newPrisoners)
		{
			this._prisonersAmount += newPrisoners.Count;
			base.Text = SandBoxUIHelper.GetPrisonersSoldNotificationText(this._prisonersAmount);
		}

		private int _prisonersAmount;
	}
}

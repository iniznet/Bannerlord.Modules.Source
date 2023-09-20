using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	public class TroopGivenToSettlementNotificationItemVM : SettlementNotificationItemBaseVM
	{
		public Hero GiverHero { get; private set; }

		public TroopRoster Troops { get; private set; }

		public TroopGivenToSettlementNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, Hero giverHero, TroopRoster troops, int createdTick)
			: base(onRemove, createdTick)
		{
			this.GiverHero = giverHero;
			this.Troops = troops;
			base.Text = SandBoxUIHelper.GetTroopGivenToSettlementNotificationText(this.Troops.TotalManCount);
			base.CharacterName = ((this.GiverHero != null) ? this.GiverHero.Name.ToString() : "null hero");
			base.CharacterVisual = ((this.GiverHero != null) ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(this.GiverHero.CharacterObject, false)) : new ImageIdentifierVM(0));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			if (this.GiverHero != null)
			{
				base.RelationType = (this.GiverHero.Clan.IsAtWarWith(Hero.MainHero.Clan) ? (-1) : 1);
			}
		}

		public void AddNewAction(TroopRoster newTroops)
		{
			this.Troops.Add(newTroops);
			base.Text = SandBoxUIHelper.GetTroopGivenToSettlementNotificationText(this.Troops.TotalManCount);
		}
	}
}

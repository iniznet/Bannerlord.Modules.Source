using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class ArmyCreationNotificationItemVM : MapNotificationItemBaseVM
	{
		public Army Army { get; }

		public ArmyCreationNotificationItemVM(ArmyCreationMapNotification data)
			: base(data)
		{
			this.Army = data.CreatedArmy;
			base.NotificationIdentifier = "armycreation";
			this._onInspect = delegate
			{
				Army army = this.Army;
				Vec2? vec;
				if (army == null)
				{
					vec = null;
				}
				else
				{
					MobileParty leaderParty = army.LeaderParty;
					vec = ((leaderParty != null) ? new Vec2?(leaderParty.Position2D) : null);
				}
				base.GoToMapPosition(vec ?? MobileParty.MainParty.Position2D);
			};
			CampaignEvents.OnPartyJoinedArmyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyJoinedArmy));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
		}

		private void OnArmyDispersed(Army arg1, Army.ArmyDispersionReason arg2, bool isPlayersArmy)
		{
			if (arg1 == this.Army)
			{
				base.ExecuteRemove();
			}
		}

		private void OnPartyJoinedArmy(MobileParty party)
		{
			if (party == MobileParty.MainParty && party.Army == this.Army)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnPartyJoinedArmyEvent.ClearListeners(this);
			CampaignEvents.ArmyDispersed.ClearListeners(this);
		}
	}
}

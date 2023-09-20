using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000035 RID: 53
	public class ArmyCreationNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x00019CBC File Offset: 0x00017EBC
		public Army Army { get; }

		// Token: 0x06000503 RID: 1283 RVA: 0x00019CC4 File Offset: 0x00017EC4
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

		// Token: 0x06000504 RID: 1284 RVA: 0x00019D2F File Offset: 0x00017F2F
		private void OnArmyDispersed(Army arg1, Army.ArmyDispersionReason arg2, bool isPlayersArmy)
		{
			if (arg1 == this.Army)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x00019D40 File Offset: 0x00017F40
		private void OnPartyJoinedArmy(MobileParty party)
		{
			if (party == MobileParty.MainParty && party.Army == this.Army)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00019D5E File Offset: 0x00017F5E
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnPartyJoinedArmyEvent.ClearListeners(this);
			CampaignEvents.ArmyDispersed.ClearListeners(this);
		}
	}
}

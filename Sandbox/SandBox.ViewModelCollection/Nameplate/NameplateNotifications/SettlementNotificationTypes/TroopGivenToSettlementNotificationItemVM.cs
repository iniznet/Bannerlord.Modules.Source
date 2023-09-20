using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	// Token: 0x02000022 RID: 34
	public class TroopGivenToSettlementNotificationItemVM : SettlementNotificationItemBaseVM
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000DECD File Offset: 0x0000C0CD
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0000DED5 File Offset: 0x0000C0D5
		public Hero GiverHero { get; private set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000DEDE File Offset: 0x0000C0DE
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x0000DEE6 File Offset: 0x0000C0E6
		public TroopRoster Troops { get; private set; }

		// Token: 0x060002D4 RID: 724 RVA: 0x0000DEF0 File Offset: 0x0000C0F0
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

		// Token: 0x060002D5 RID: 725 RVA: 0x0000DFB8 File Offset: 0x0000C1B8
		public void AddNewAction(TroopRoster newTroops)
		{
			this.Troops.Add(newTroops);
			base.Text = SandBoxUIHelper.GetTroopGivenToSettlementNotificationText(this.Troops.TotalManCount);
		}
	}
}

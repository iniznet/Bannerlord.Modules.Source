using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	// Token: 0x02000023 RID: 35
	public class TroopRecruitmentNotificationItemVM : SettlementNotificationItemBaseVM
	{
		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000DFDC File Offset: 0x0000C1DC
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000DFE4 File Offset: 0x0000C1E4
		public Hero RecruiterHero { get; private set; }

		// Token: 0x060002D8 RID: 728 RVA: 0x0000DFF0 File Offset: 0x0000C1F0
		public TroopRecruitmentNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, Hero recruiterHero, int amount, int createdTick)
			: base(onRemove, createdTick)
		{
			base.Text = SandBoxUIHelper.GetRecruitNotificationText(amount);
			this._recruitAmount = amount;
			this.RecruiterHero = recruiterHero;
			base.CharacterName = ((recruiterHero != null) ? recruiterHero.Name.ToString() : "null hero");
			base.CharacterVisual = ((recruiterHero != null) ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(recruiterHero.CharacterObject, false)) : new ImageIdentifierVM(0));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			if (recruiterHero != null)
			{
				base.RelationType = (recruiterHero.Clan.IsAtWarWith(Hero.MainHero.Clan) ? (-1) : 1);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000E090 File Offset: 0x0000C290
		public void AddNewAction(int addedAmount)
		{
			this._recruitAmount += addedAmount;
			base.Text = SandBoxUIHelper.GetRecruitNotificationText(this._recruitAmount);
		}

		// Token: 0x04000169 RID: 361
		private int _recruitAmount;
	}
}

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	public class TroopRecruitmentNotificationItemVM : SettlementNotificationItemBaseVM
	{
		public Hero RecruiterHero { get; private set; }

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

		public void AddNewAction(int addedAmount)
		{
			this._recruitAmount += addedAmount;
			base.Text = SandBoxUIHelper.GetRecruitNotificationText(this._recruitAmount);
		}

		private int _recruitAmount;
	}
}

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	public class IssueSolvedByLordNotificationItemVM : SettlementNotificationItemBaseVM
	{
		public IssueSolvedByLordNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, Hero hero, int createdTick)
			: base(onRemove, createdTick)
		{
			base.Text = new TextObject("{=TFJTOYea}Solved an issue", null).ToString();
			string text;
			if (hero == null)
			{
				text = null;
			}
			else
			{
				TextObject name = hero.Name;
				text = ((name != null) ? name.ToString() : null);
			}
			base.CharacterName = text ?? "";
			base.CharacterVisual = new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(hero.CharacterObject, false));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			int num;
			if (hero != null)
			{
				Clan clan = hero.Clan;
				bool? flag = ((clan != null) ? new bool?(clan.IsAtWarWith(Hero.MainHero.Clan)) : null);
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					num = -1;
					goto IL_B2;
				}
			}
			num = 1;
			IL_B2:
			base.RelationType = num;
		}
	}
}

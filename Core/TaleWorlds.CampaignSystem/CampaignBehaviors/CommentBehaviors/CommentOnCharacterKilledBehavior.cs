using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	public class CommentOnCharacterKilledBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim.Clan != null && !Clan.BanditFactions.Contains(victim.Clan))
			{
				CharacterKilledLogEntry characterKilledLogEntry = new CharacterKilledLogEntry(victim, killer, detail);
				LogEntry.AddLogEntry(characterKilledLogEntry);
				if (this.IsRelatedToPlayer(victim))
				{
					Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new DeathMapNotification(victim, killer, characterKilledLogEntry.GetEncyclopediaText(), detail, CampaignTime.Now));
				}
			}
		}

		private bool IsRelatedToPlayer(Hero victim)
		{
			bool flag = victim == Hero.MainHero.Mother || victim == Hero.MainHero.Father || victim == Hero.MainHero.Spouse || victim == Hero.MainHero;
			if (!flag)
			{
				foreach (Hero hero in Hero.MainHero.Children)
				{
					if (victim == hero)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (Hero hero2 in Hero.MainHero.Siblings)
				{
					if (victim == hero2)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}
	}
}

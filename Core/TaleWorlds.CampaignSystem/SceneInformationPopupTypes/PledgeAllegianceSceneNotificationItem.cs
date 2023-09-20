using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class PledgeAllegianceSceneNotificationItem : SceneNotificationData
	{
		public Hero PlayerHero { get; }

		public bool PlayerWantsToRestore { get; }

		public override string SceneID
		{
			get
			{
				return "scn_pledge_allegiance_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				TextObject textObject = GameTexts.FindText("str_pledge_notification_title", null);
				textObject.SetCharacterProperties("RULER", this.PlayerHero.Clan.Kingdom.Leader.CharacterObject, false);
				textObject.SetTextVariable("PLAYER_WANTS_RESTORE", this.PlayerWantsToRestore ? 1 : 0);
				textObject.SetTextVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				textObject.SetTextVariable("YEAR", this._creationCampaignTime.GetYear);
				return textObject;
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			List<Banner> list = new List<Banner>();
			list.Add(Hero.MainHero.ClanBanner);
			Clan clan = this.PlayerHero.Clan.Kingdom.Leader.Clan;
			list.Add(((clan != null) ? clan.Kingdom.Banner : null) ?? this.PlayerHero.Clan.Kingdom.Leader.ClanBanner);
			return list;
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			ItemObject itemObject = null;
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.PlayerHero.BattleEquipment.Clone(false);
			if (equipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
			{
				itemObject = CampaignSceneNotificationHelper.GetDefaultHorseItem();
				equipment[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement(itemObject, null, null, false);
			}
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.PlayerHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, true));
			Equipment equipment2 = this.PlayerHero.Clan.Kingdom.Leader.BattleEquipment.Clone(false);
			if (equipment2[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
			{
				if (itemObject == null)
				{
					itemObject = CampaignSceneNotificationHelper.GetDefaultHorseItem();
				}
				equipment2[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement(itemObject, null, null, false);
			}
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.PlayerHero.Clan.Kingdom.Leader, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, true));
			IFaction mapFaction = this.PlayerHero.Clan.Kingdom.Leader.MapFaction;
			CultureObject cultureObject = ((((mapFaction != null) ? mapFaction.Culture : null) != null) ? this.PlayerHero.Clan.Kingdom.Leader.MapFaction.Culture : this.PlayerHero.MapFaction.Culture);
			for (int i = 0; i < 24; i++)
			{
				CharacterObject randomTroopForCulture = CampaignSceneNotificationHelper.GetRandomTroopForCulture(cultureObject);
				Equipment equipment3 = randomTroopForCulture.FirstBattleEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, false, false);
				BodyProperties bodyProperties = randomTroopForCulture.GetBodyProperties(equipment3, MBRandom.RandomInt(100));
				list.Add(new SceneNotificationData.SceneNotificationCharacter(randomTroopForCulture, equipment3, bodyProperties, false, uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		public PledgeAllegianceSceneNotificationItem(Hero playerHero, bool playerWantsToRestore)
		{
			this.PlayerHero = playerHero;
			this.PlayerWantsToRestore = playerWantsToRestore;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private const int NumberOfTroops = 24;

		private readonly CampaignTime _creationCampaignTime;
	}
}

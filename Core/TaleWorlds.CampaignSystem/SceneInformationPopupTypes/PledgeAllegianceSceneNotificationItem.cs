using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000C3 RID: 195
	public class PledgeAllegianceSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001250 RID: 4688 RVA: 0x00053F58 File Offset: 0x00052158
		public Hero PlayerHero { get; }

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06001251 RID: 4689 RVA: 0x00053F60 File Offset: 0x00052160
		public bool PlayerWantsToRestore { get; }

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x00053F68 File Offset: 0x00052168
		public override string SceneID
		{
			get
			{
				return "scn_pledge_allegiance_notification";
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06001253 RID: 4691 RVA: 0x00053F70 File Offset: 0x00052170
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

		// Token: 0x06001254 RID: 4692 RVA: 0x00053FF8 File Offset: 0x000521F8
		public override IEnumerable<Banner> GetBanners()
		{
			List<Banner> list = new List<Banner>();
			list.Add(Hero.MainHero.ClanBanner);
			Clan clan = this.PlayerHero.Clan.Kingdom.Leader.Clan;
			list.Add(((clan != null) ? clan.Kingdom.Banner : null) ?? this.PlayerHero.Clan.Kingdom.Leader.ClanBanner);
			return list;
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0005406C File Offset: 0x0005226C
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

		// Token: 0x06001256 RID: 4694 RVA: 0x00054221 File Offset: 0x00052421
		public PledgeAllegianceSceneNotificationItem(Hero playerHero, bool playerWantsToRestore)
		{
			this.PlayerHero = playerHero;
			this.PlayerWantsToRestore = playerWantsToRestore;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000675 RID: 1653
		private const int NumberOfTroops = 24;

		// Token: 0x04000678 RID: 1656
		private readonly CampaignTime _creationCampaignTime;
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B1 RID: 177
	public abstract class EmpireConspiracySupportsSceneNotificationItemBase : SceneNotificationData
	{
		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x060011D6 RID: 4566 RVA: 0x00051C6D File Offset: 0x0004FE6D
		public Hero King { get; }

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x060011D7 RID: 4567 RVA: 0x00051C75 File Offset: 0x0004FE75
		public override string SceneID
		{
			get
			{
				return "scn_empire_conspiracy_supports_notification";
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x060011D8 RID: 4568 RVA: 0x00051C7C File Offset: 0x0004FE7C
		public override TextObject AffirmativeText
		{
			get
			{
				return GameTexts.FindText("str_ok", null);
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x00051C89 File Offset: 0x0004FE89
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.King.MapFaction.Banner,
				this.King.MapFaction.Banner
			};
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x00051CBC File Offset: 0x0004FEBC
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.King.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.King, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("villager_battania");
			Equipment equipment2 = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("conspirator_cutscene_template").DefaultEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
			BodyProperties bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			list.Add(CampaignSceneNotificationHelper.GetBodyguardOfCulture(this.King.MapFaction.Culture));
			list.Add(CampaignSceneNotificationHelper.GetBodyguardOfCulture(this.King.MapFaction.Culture));
			return list;
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00051DD6 File Offset: 0x0004FFD6
		protected EmpireConspiracySupportsSceneNotificationItemBase(Hero kingHero)
		{
			this.King = kingHero;
		}
	}
}

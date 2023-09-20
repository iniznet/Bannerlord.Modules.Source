using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B9 RID: 185
	public class HeroExecutionSceneNotificationData : SceneNotificationData
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x060011FE RID: 4606 RVA: 0x0005246E File Offset: 0x0005066E
		public Hero Executer { get; }

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x060011FF RID: 4607 RVA: 0x00052476 File Offset: 0x00050676
		public Hero Victim { get; }

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x0005247E File Offset: 0x0005067E
		public override bool IsNegativeOptionShown { get; }

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06001201 RID: 4609 RVA: 0x00052486 File Offset: 0x00050686
		public override string SceneID
		{
			get
			{
				return "scn_execution_notification";
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06001202 RID: 4610 RVA: 0x0005248D File Offset: 0x0005068D
		public override TextObject NegativeText
		{
			get
			{
				return GameTexts.FindText("str_execution_negative_action", null);
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x0005249A File Offset: 0x0005069A
		public override bool IsAffirmativeOptionShown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x0005249D File Offset: 0x0005069D
		public override TextObject TitleText { get; }

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x000524A5 File Offset: 0x000506A5
		public override TextObject AffirmativeText { get; }

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06001206 RID: 4614 RVA: 0x000524AD File Offset: 0x000506AD
		public override TextObject AffirmativeTitleText { get; }

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x000524B5 File Offset: 0x000506B5
		public override TextObject AffirmativeHintText { get; }

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x000524BD File Offset: 0x000506BD
		public override TextObject AffirmativeHintTextExtended { get; }

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06001209 RID: 4617 RVA: 0x000524C5 File Offset: 0x000506C5
		public override TextObject AffirmativeDescriptionText { get; }

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x000524CD File Offset: 0x000506CD
		public override SceneNotificationData.RelevantContextType RelevantContext { get; }

		// Token: 0x0600120B RID: 4619 RVA: 0x000524D8 File Offset: 0x000506D8
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			Equipment equipment = this.Victim.BattleEquipment.Clone(true);
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.NumAllWeaponSlots, default(EquipmentElement));
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.WeaponItemBeginSlot, default(EquipmentElement));
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon1, default(EquipmentElement));
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon2, default(EquipmentElement));
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon3, default(EquipmentElement));
			equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.ExtraWeaponSlot, default(EquipmentElement));
			ItemObject itemObject = Items.All.FirstOrDefault((ItemObject i) => i.StringId == "execution_axe");
			Equipment equipment2 = this.Executer.BattleEquipment.Clone(true);
			equipment2.AddEquipmentToSlotWithoutAgent(EquipmentIndex.WeaponItemBeginSlot, new EquipmentElement(itemObject, null, null, false));
			equipment2.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon1, default(EquipmentElement));
			equipment2.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon2, default(EquipmentElement));
			equipment2.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon3, default(EquipmentElement));
			equipment2.AddEquipmentToSlotWithoutAgent(EquipmentIndex.ExtraWeaponSlot, default(EquipmentElement));
			return new List<SceneNotificationData.SceneNotificationCharacter>
			{
				CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.Victim, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false),
				CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.Executer, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false)
			};
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00052628 File Offset: 0x00050828
		private HeroExecutionSceneNotificationData(Hero executingHero, Hero dyingHero, TextObject titleText, TextObject affirmativeTitleText, TextObject affirmativeActionText, TextObject affirmativeActionDescriptionText, TextObject affirmativeActionHintText, TextObject affirmativeActionHintExtendedText, bool isNegativeOptionShown, Action onAffirmativeAction, SceneNotificationData.RelevantContextType relevantContextType = SceneNotificationData.RelevantContextType.Any)
		{
			this.Executer = executingHero;
			this.Victim = dyingHero;
			this.TitleText = titleText;
			this.AffirmativeTitleText = affirmativeTitleText;
			this.AffirmativeText = affirmativeActionText;
			this.AffirmativeDescriptionText = affirmativeActionDescriptionText;
			this.AffirmativeHintText = affirmativeActionHintText;
			this.AffirmativeHintTextExtended = affirmativeActionHintExtendedText;
			this.IsNegativeOptionShown = isNegativeOptionShown;
			this.RelevantContext = relevantContextType;
			this._onAffirmativeAction = onAffirmativeAction;
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x00052690 File Offset: 0x00050890
		public override void OnAffirmativeAction()
		{
			if (this._onAffirmativeAction != null)
			{
				this._onAffirmativeAction();
				return;
			}
			if (this.Victim != Hero.MainHero)
			{
				KillCharacterAction.ApplyByExecution(this.Victim, this.Executer, true, true);
			}
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x000526C8 File Offset: 0x000508C8
		public static HeroExecutionSceneNotificationData CreateForPlayerExecutingHero(Hero dyingHero, Action onAffirmativeAction, SceneNotificationData.RelevantContextType relevantContextType = SceneNotificationData.RelevantContextType.Any)
		{
			GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(CampaignTime.Now));
			GameTexts.SetVariable("YEAR", CampaignTime.Now.GetYear);
			GameTexts.SetVariable("NAME", dyingHero.Name);
			TextObject textObject = GameTexts.FindText("str_execution_positive_action", null);
			textObject.SetCharacterProperties("DYING_HERO", dyingHero.CharacterObject, false);
			return new HeroExecutionSceneNotificationData(Hero.MainHero, dyingHero, GameTexts.FindText("str_executing_prisoner", null), GameTexts.FindText("str_executed_prisoner", null), textObject, GameTexts.FindText("str_execute_prisoner_desc", null), HeroExecutionSceneNotificationData.GetExecuteTroopHintText(dyingHero, false), HeroExecutionSceneNotificationData.GetExecuteTroopHintText(dyingHero, true), true, onAffirmativeAction, relevantContextType);
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x0005276C File Offset: 0x0005096C
		public static HeroExecutionSceneNotificationData CreateForInformingPlayer(Hero executingHero, Hero dyingHero, SceneNotificationData.RelevantContextType relevantContextType = SceneNotificationData.RelevantContextType.Any)
		{
			GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(CampaignTime.Now));
			GameTexts.SetVariable("YEAR", CampaignTime.Now.GetYear);
			GameTexts.SetVariable("NAME", dyingHero.Name);
			TextObject textObject = new TextObject("{=uYjEknNX}{VICTIM.NAME}'s execution by {EXECUTER.NAME}", null);
			textObject.SetCharacterProperties("VICTIM", dyingHero.CharacterObject, false);
			textObject.SetCharacterProperties("EXECUTER", executingHero.CharacterObject, false);
			return new HeroExecutionSceneNotificationData(executingHero, dyingHero, textObject, GameTexts.FindText("str_executed_prisoner", null), GameTexts.FindText("str_proceed", null), null, null, null, false, null, relevantContextType);
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x00052808 File Offset: 0x00050A08
		private static TextObject GetExecuteTroopHintText(Hero dyingHero, bool showAll)
		{
			Dictionary<Clan, int> dictionary = new Dictionary<Clan, int>();
			GameTexts.SetVariable("LEFT", new TextObject("{=jxypVgl2}Relation Changes", null));
			string text = GameTexts.FindText("str_LEFT_colon", null).ToString();
			foreach (Clan clan in Clan.All)
			{
				foreach (Hero hero in clan.Heroes)
				{
					if (!hero.IsHumanPlayerCharacter && hero.IsAlive && hero != dyingHero && (!hero.IsLord || hero.Clan.Leader == hero))
					{
						bool flag;
						int relationChangeForExecutingHero = Campaign.Current.Models.ExecutionRelationModel.GetRelationChangeForExecutingHero(dyingHero, hero, out flag);
						if (relationChangeForExecutingHero != 0)
						{
							if (dictionary.ContainsKey(clan))
							{
								if (relationChangeForExecutingHero < dictionary[clan])
								{
									dictionary[clan] = relationChangeForExecutingHero;
								}
							}
							else
							{
								dictionary.Add(clan, relationChangeForExecutingHero);
							}
						}
					}
				}
			}
			GameTexts.SetVariable("newline", "\n");
			List<KeyValuePair<Clan, int>> list = dictionary.OrderBy((KeyValuePair<Clan, int> change) => change.Value).ToList<KeyValuePair<Clan, int>>();
			int num = 0;
			foreach (KeyValuePair<Clan, int> keyValuePair in list)
			{
				Clan key = keyValuePair.Key;
				int value = keyValuePair.Value;
				GameTexts.SetVariable("LEFT", key.Name);
				GameTexts.SetVariable("RIGHT", value);
				string text2 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", text2);
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
				num++;
				if (!showAll && num == HeroExecutionSceneNotificationData.MaxShownRelationChanges)
				{
					TextObject textObject = new TextObject("{=DPTPuyip}And {NUMBER} more...", null);
					GameTexts.SetVariable("NUMBER", dictionary.Count - num);
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", textObject);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
					TextObject textObject2 = new TextObject("{=u12ocP9f}Hold '{EXTEND_KEY}' for more info.", null);
					textObject2.SetTextVariable("EXTEND_KEY", GameTexts.FindText("str_game_key_text", "anyalt"));
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", textObject2);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
					break;
				}
			}
			return new TextObject("{=!}" + text, null);
		}

		// Token: 0x04000652 RID: 1618
		private readonly Action _onAffirmativeAction;

		// Token: 0x04000653 RID: 1619
		protected static int MaxShownRelationChanges = 8;
	}
}

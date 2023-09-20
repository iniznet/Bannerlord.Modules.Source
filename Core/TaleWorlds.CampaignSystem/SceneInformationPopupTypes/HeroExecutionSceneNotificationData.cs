using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class HeroExecutionSceneNotificationData : SceneNotificationData
	{
		public Hero Executer { get; }

		public Hero Victim { get; }

		public override bool IsNegativeOptionShown { get; }

		public override string SceneID
		{
			get
			{
				return "scn_execution_notification";
			}
		}

		public override TextObject NegativeText
		{
			get
			{
				return GameTexts.FindText("str_execution_negative_action", null);
			}
		}

		public override bool IsAffirmativeOptionShown
		{
			get
			{
				return true;
			}
		}

		public override TextObject TitleText { get; }

		public override TextObject AffirmativeText { get; }

		public override TextObject AffirmativeTitleText { get; }

		public override TextObject AffirmativeHintText { get; }

		public override TextObject AffirmativeHintTextExtended { get; }

		public override TextObject AffirmativeDescriptionText { get; }

		public override SceneNotificationData.RelevantContextType RelevantContext { get; }

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

		public static HeroExecutionSceneNotificationData CreateForPlayerExecutingHero(Hero dyingHero, Action onAffirmativeAction, SceneNotificationData.RelevantContextType relevantContextType = SceneNotificationData.RelevantContextType.Any)
		{
			GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(CampaignTime.Now));
			GameTexts.SetVariable("YEAR", CampaignTime.Now.GetYear);
			GameTexts.SetVariable("NAME", dyingHero.Name);
			TextObject textObject = GameTexts.FindText("str_execution_positive_action", null);
			textObject.SetCharacterProperties("DYING_HERO", dyingHero.CharacterObject, false);
			return new HeroExecutionSceneNotificationData(Hero.MainHero, dyingHero, GameTexts.FindText("str_executing_prisoner", null), GameTexts.FindText("str_executed_prisoner", null), textObject, GameTexts.FindText("str_execute_prisoner_desc", null), HeroExecutionSceneNotificationData.GetExecuteTroopHintText(dyingHero, false), HeroExecutionSceneNotificationData.GetExecuteTroopHintText(dyingHero, true), true, onAffirmativeAction, relevantContextType);
		}

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

		private readonly Action _onAffirmativeAction;

		protected static int MaxShownRelationChanges = 8;
	}
}

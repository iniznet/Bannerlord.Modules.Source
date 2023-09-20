using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors
{
	public class BarberCampaignBehavior : CampaignBehaviorBase, IFacegenCampaignBehavior, ICampaignBehavior
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		public override void SyncData(IDataStore store)
		{
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("barber_start_talk_beggar", "start", "close_window", "{=pWzdxd7O}May the Heavens bless you, my poor {?PLAYER.GENDER}lady{?}fellow{\\?}, but I can't spare a coin right now.", new ConversationSentence.OnConditionDelegate(this.InDisguiseSpeakingToBarber), new ConversationSentence.OnConsequenceDelegate(this.InitializeBarberConversation), 100, null);
			campaignGameStarter.AddDialogLine("barber_start_talk", "start", "barber_question1", "{=2aXYYNBG}Come to have your hair cut, {?PLAYER.GENDER}my lady{?}my lord{\\?}? A new look for a new day?", new ConversationSentence.OnConditionDelegate(this.IsConversationAgentBarber), new ConversationSentence.OnConsequenceDelegate(this.InitializeBarberConversation), 100, null);
			campaignGameStarter.AddPlayerLine("player_accept_haircut", "barber_question1", "start_cut_token", "{=Q7wBRXtR}Yes, I have. ({GOLD_COST} {GOLD_ICON})", new ConversationSentence.OnConditionDelegate(this.GivePlayerAHaircutCondition), new ConversationSentence.OnConsequenceDelegate(this.GivePlayerAHaircut), 100, new ConversationSentence.OnClickableConditionDelegate(this.DoesPlayerHaveEnoughGold), null);
			campaignGameStarter.AddPlayerLine("player_refuse_haircut", "barber_question1", "no_haircut_conversation_token", "{=xPAAZAaI}My hair is fine as it is, thank you.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("barber_ask_if_done", "start_cut_token", "finish_cut_token", "{=M3K8wUOO}So... Does this please you, {?PLAYER.GENDER}my lady{?}my lord{\\?}?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("player_done_with_haircut", "finish_cut_token", "finish_barber", "{=zTF4bJm0}Yes, it's fine.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_not_done_with_haircut", "finish_cut_token", "start_cut_token", "{=BnoSOi3r}Actually...", new ConversationSentence.OnConditionDelegate(this.GivePlayerAHaircutCondition), new ConversationSentence.OnConsequenceDelegate(this.GivePlayerAHaircut), 100, new ConversationSentence.OnClickableConditionDelegate(this.DoesPlayerHaveEnoughGold), null);
			campaignGameStarter.AddDialogLine("barber_no_haircut_talk", "no_haircut_conversation_token", "close_window", "{=BusYGTrN}Excellent! Have a good day, then, {?PLAYER.GENDER}my lady{?}my lord{\\?}.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("barber_haircut_finished", "finish_barber", "player_had_a_haircut_token", "{=akqJbZpH}Marvellous! You cut a splendid appearance, {?PLAYER.GENDER}my lady{?}my lord{\\?}, if you don't mind my saying. Most splendid.", new ConversationSentence.OnConditionDelegate(this.DidPlayerHaveAHaircut), new ConversationSentence.OnConsequenceDelegate(this.ChargeThePlayer), 100, null);
			campaignGameStarter.AddDialogLine("barber_haircut_no_change", "finish_barber", "player_did_not_cut_token", "{=yLIZlaS1}Very well. Do come back when you're ready, {?PLAYER.GENDER}my lady{?}my lord{\\?}.", new ConversationSentence.OnConditionDelegate(this.DidPlayerNotHaveAHaircut), null, 100, null);
			campaignGameStarter.AddPlayerLine("player_no_haircut_finish_talk", "player_did_not_cut_token", "close_window", "{=oPUVNuhN}I'll keep you in mind", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_haircut_finish_talk", "player_had_a_haircut_token", "close_window", "{=F9Xjbchh}Thank you.", null, null, 100, null, null);
		}

		private bool InDisguiseSpeakingToBarber()
		{
			return this.IsConversationAgentBarber() && Campaign.Current.IsMainHeroDisguised;
		}

		private bool DoesPlayerHaveEnoughGold(out TextObject explanation)
		{
			if (Hero.MainHero.Gold < 100)
			{
				explanation = new TextObject("{=RYJdU43V}Not Enough Gold", null);
				return false;
			}
			explanation = TextObject.Empty;
			return true;
		}

		private void ChargeThePlayer()
		{
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 100, false);
		}

		private bool DidPlayerNotHaveAHaircut()
		{
			return !this.DidPlayerHaveAHaircut();
		}

		private bool DidPlayerHaveAHaircut()
		{
			return Hero.MainHero.BodyProperties.StaticProperties != this._previousBodyProperties;
		}

		private bool IsConversationAgentBarber()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return ((currentSettlement != null) ? currentSettlement.Culture.Barber : null) == CharacterObject.OneToOneConversationCharacter;
		}

		private bool GivePlayerAHaircutCondition()
		{
			MBTextManager.SetTextVariable("GOLD_COST", 100);
			return true;
		}

		private void GivePlayerAHaircut()
		{
			this._isOpenedFromBarberDialogue = true;
			BarberState barberState = Game.Current.GameStateManager.CreateState<BarberState>(new object[]
			{
				Hero.MainHero.CharacterObject,
				this.GetFaceGenFilter()
			});
			this._isOpenedFromBarberDialogue = false;
			GameStateManager.Current.PushState(barberState, 0);
		}

		private void InitializeBarberConversation()
		{
			this._previousBodyProperties = Hero.MainHero.BodyProperties.StaticProperties;
		}

		private LocationCharacter CreateBarber(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject barber = culture.Barber;
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(barber, ref num, ref num2, "Barber");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(barber, -1, null, default(UniqueTroopDescriptor))).Monster(FaceGen.GetMonsterWithSuffix(barber.Race, "_settlement_slow")).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_barber", true, relation, null, true, false, null, false, false, true);
		}

		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
			int num;
			if (CampaignMission.Current.Location == locationWithId && Campaign.Current.IsDay && unusedUsablePointCount.TryGetValue("sp_merchant_notary", out num))
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateBarber), Settlement.CurrentSettlement.Culture, 0, 1);
			}
		}

		public IFaceGeneratorCustomFilter GetFaceGenFilter()
		{
			return new BarberCampaignBehavior.BarberFaceGeneratorCustomFilter(!this._isOpenedFromBarberDialogue, this.GetAvailableHaircuts(), this.GetAvailableFacialHairs());
		}

		private int[] GetAvailableFacialHairs()
		{
			List<int> list = new List<int>();
			CultureCode cultureCode = ((this._isOpenedFromBarberDialogue && Settlement.CurrentSettlement != null) ? Settlement.CurrentSettlement.Culture.GetCultureCode() : (-1));
			if (!Hero.MainHero.IsFemale)
			{
				switch (cultureCode)
				{
				case 0:
					list.AddRange(new int[] { 5, 6, 9, 12, 23, 24, 25, 26 });
					break;
				case 1:
					list.AddRange(new int[]
					{
						1, 2, 4, 8, 9, 10, 11, 12, 13, 14,
						15, 16, 17, 18, 19, 20, 21, 22, 24, 25,
						26, 29, 32, 34, 35
					});
					break;
				case 2:
					list.AddRange(new int[] { 36, 37, 38, 39, 40, 41 });
					break;
				case 3:
					list.AddRange(new int[]
					{
						1, 2, 3, 5, 6, 7, 8, 9, 10, 12,
						13, 14, 22, 23, 24, 25, 26, 32
					});
					break;
				case 4:
					list.AddRange(new int[] { 0, 28, 29, 31, 32, 33 });
					break;
				case 5:
					list.AddRange(new int[]
					{
						0, 1, 2, 4, 8, 10, 11, 12, 13, 14,
						15, 16, 17, 18, 19, 20, 21, 22, 24, 29,
						31, 32, 34, 35
					});
					break;
				}
				list.AddRange(new int[1]);
			}
			return list.Distinct<int>().ToArray<int>();
		}

		private int[] GetAvailableHaircuts()
		{
			List<int> list = new List<int>();
			CultureCode cultureCode = ((this._isOpenedFromBarberDialogue && Settlement.CurrentSettlement != null) ? Settlement.CurrentSettlement.Culture.GetCultureCode() : (-1));
			if (Hero.MainHero.IsFemale)
			{
				switch (cultureCode)
				{
				case 0:
					list.AddRange(new int[0]);
					break;
				case 1:
					list.AddRange(new int[] { 8, 9, 13, 15 });
					break;
				case 2:
					list.AddRange(new int[] { 13, 17, 18, 19, 20 });
					break;
				case 3:
					list.AddRange(new int[0]);
					break;
				case 4:
					list.AddRange(new int[] { 7, 12, 13 });
					break;
				case 5:
					list.AddRange(new int[] { 8, 9, 15 });
					break;
				}
				list.AddRange(new int[]
				{
					0, 1, 2, 3, 4, 5, 6, 10, 11, 14,
					16, 21
				});
			}
			else
			{
				switch (cultureCode)
				{
				case 0:
					list.AddRange(new int[] { 1, 4, 5, 8, 14, 15 });
					break;
				case 1:
					list.AddRange(new int[]
					{
						1, 2, 3, 4, 5, 8, 10, 16, 18, 20,
						27
					});
					break;
				case 2:
					list.AddRange(new int[]
					{
						1, 2, 3, 4, 5, 21, 22, 23, 24, 25,
						26
					});
					break;
				case 3:
					list.AddRange(new int[] { 1, 4, 5, 8, 11, 14, 15, 28 });
					break;
				case 4:
					list.AddRange(new int[] { 12, 17, 28 });
					break;
				case 5:
					list.AddRange(new int[]
					{
						1, 2, 3, 4, 5, 7, 8, 10, 16, 17,
						18, 19, 20
					});
					break;
				}
				list.AddRange(new int[] { 0, 6, 9, 13 });
			}
			return list.Distinct<int>().ToArray<int>();
		}

		private const int BarberCost = 100;

		private bool _isOpenedFromBarberDialogue;

		private StaticBodyProperties _previousBodyProperties;

		private class BarberFaceGeneratorCustomFilter : IFaceGeneratorCustomFilter
		{
			public BarberFaceGeneratorCustomFilter(bool useDefaultStages, int[] haircutIndices, int[] faircutIndices)
			{
				this._haircutIndices = haircutIndices;
				this._facialHairIndices = faircutIndices;
				this._defaultStages = useDefaultStages;
			}

			public int[] GetHaircutIndices(BasicCharacterObject character)
			{
				return this._haircutIndices;
			}

			public int[] GetFacialHairIndices(BasicCharacterObject character)
			{
				return this._facialHairIndices;
			}

			public FaceGeneratorStage[] GetAvailableStages()
			{
				if (this._defaultStages)
				{
					FaceGeneratorStage[] array = new FaceGeneratorStage[7];
					RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.50567A6578C37E24118E2B7EE8F5C7930666F62F).FieldHandle);
					return array;
				}
				return new FaceGeneratorStage[] { 5 };
			}

			private readonly int[] _haircutIndices;

			private readonly int[] _facialHairIndices;

			private readonly bool _defaultStages;
		}
	}
}

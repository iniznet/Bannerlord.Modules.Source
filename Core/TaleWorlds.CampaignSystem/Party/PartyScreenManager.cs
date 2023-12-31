﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Party
{
	public class PartyScreenManager
	{
		public bool IsDonating { get; private set; }

		public PartyScreenMode CurrentMode
		{
			get
			{
				return this._currentMode;
			}
		}

		public static PartyScreenManager Instance
		{
			get
			{
				return Campaign.Current.PartyScreenManager;
			}
		}

		public static PartyScreenLogic PartyScreenLogic
		{
			get
			{
				return PartyScreenManager.Instance._partyScreenLogic;
			}
		}

		private void OpenPartyScreen()
		{
			Game game = Game.Current;
			this._partyScreenLogic = new PartyScreenLogic();
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = new PartyScreenLogicInitializationData
			{
				LeftOwnerParty = null,
				RightOwnerParty = PartyBase.MainParty,
				LeftMemberRoster = TroopRoster.CreateDummyTroopRoster(),
				LeftPrisonerRoster = TroopRoster.CreateDummyTroopRoster(),
				RightMemberRoster = PartyBase.MainParty.MemberRoster,
				RightPrisonerRoster = PartyBase.MainParty.PrisonRoster,
				LeftLeaderHero = null,
				RightLeaderHero = PartyBase.MainParty.LeaderHero,
				LeftPartyMembersSizeLimit = 0,
				LeftPartyPrisonersSizeLimit = 0,
				RightPartyMembersSizeLimit = PartyBase.MainParty.PartySizeLimit,
				RightPartyPrisonersSizeLimit = PartyBase.MainParty.PrisonerSizeLimit,
				LeftPartyName = null,
				RightPartyName = PartyBase.MainParty.Name,
				TroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate),
				PartyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DefaultDoneHandler),
				PartyPresentationDoneButtonConditionDelegate = null,
				PartyPresentationCancelButtonActivateDelegate = null,
				PartyPresentationCancelButtonDelegate = null,
				IsDismissMode = true,
				IsTroopUpgradesDisabled = false,
				Header = null,
				PartyScreenClosedDelegate = null,
				TransferHealthiesGetWoundedsFirst = false,
				ShowProgressBar = false,
				MemberTransferState = PartyScreenLogic.TransferState.Transferable,
				PrisonerTransferState = PartyScreenLogic.TransferState.Transferable,
				AccompanyingTransferState = PartyScreenLogic.TransferState.NotTransferable
			};
			this._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = game.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(this._partyScreenLogic);
			this._currentMode = PartyScreenMode.Normal;
			game.GameStateManager.PushState(partyState, 0);
		}

		public static void CloseScreen(bool isForced, bool fromCancel = false)
		{
			PartyScreenManager.Instance.ClosePartyPresentation(isForced, fromCancel);
		}

		private void ClosePartyPresentation(bool isForced, bool fromCancel)
		{
			bool flag = true;
			if (!fromCancel)
			{
				flag = this._partyScreenLogic.DoneLogic(isForced);
			}
			if (flag)
			{
				this._partyScreenLogic.OnPartyScreenClosed(fromCancel);
				this._partyScreenLogic = null;
				Game.Current.GameStateManager.PopState(0);
			}
		}

		public static void OpenScreenAsCheat()
		{
			if (!Game.Current.CheatMode)
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=!}Cheat mode is not enabled!", null), 0, null, "");
				return;
			}
			PartyScreenManager.Instance.IsDonating = false;
			Game game = Game.Current;
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = new PartyScreenLogicInitializationData
			{
				LeftOwnerParty = null,
				RightOwnerParty = PartyBase.MainParty,
				LeftMemberRoster = PartyScreenManager.GetRosterWithAllGameTroops(),
				LeftPrisonerRoster = TroopRoster.CreateDummyTroopRoster(),
				RightMemberRoster = PartyBase.MainParty.MemberRoster,
				RightPrisonerRoster = PartyBase.MainParty.PrisonRoster,
				LeftLeaderHero = null,
				RightLeaderHero = PartyBase.MainParty.LeaderHero,
				LeftPartyMembersSizeLimit = 0,
				LeftPartyPrisonersSizeLimit = 0,
				RightPartyMembersSizeLimit = PartyBase.MainParty.PartySizeLimit,
				RightPartyPrisonersSizeLimit = PartyBase.MainParty.PrisonerSizeLimit,
				LeftPartyName = null,
				RightPartyName = PartyBase.MainParty.Name,
				TroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate),
				PartyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DefaultDoneHandler),
				PartyPresentationDoneButtonConditionDelegate = null,
				PartyPresentationCancelButtonActivateDelegate = null,
				PartyPresentationCancelButtonDelegate = null,
				IsDismissMode = true,
				IsTroopUpgradesDisabled = false,
				Header = null,
				PartyScreenClosedDelegate = null,
				TransferHealthiesGetWoundedsFirst = false,
				ShowProgressBar = false,
				MemberTransferState = PartyScreenLogic.TransferState.Transferable,
				PrisonerTransferState = PartyScreenLogic.TransferState.Transferable,
				AccompanyingTransferState = PartyScreenLogic.TransferState.NotTransferable
			};
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = game.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			PartyScreenManager.Instance._currentMode = PartyScreenMode.Normal;
			game.GameStateManager.PushState(partyState, 0);
		}

		private static TroopRoster GetRosterWithAllGameTroops()
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			List<CharacterObject> list = new List<CharacterObject>();
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(CharacterObject));
			for (int i = 0; i < CharacterObject.All.Count; i++)
			{
				CharacterObject characterObject = CharacterObject.All[i];
				if (pageOf.IsValidEncyclopediaItem(characterObject))
				{
					list.Add(characterObject);
				}
			}
			list.Sort((CharacterObject a, CharacterObject b) => a.Name.ToString().CompareTo(b.Name.ToString()));
			for (int j = 0; j < list.Count; j++)
			{
				CharacterObject characterObject2 = list[j];
				troopRoster.AddToCounts(characterObject2, PartyScreenManager._countToAddForEachTroopCheatMode, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		public static void OpenScreenAsNormal()
		{
			if (Game.Current.CheatMode)
			{
				PartyScreenManager.OpenScreenAsCheat();
				return;
			}
			PartyScreenManager.Instance.IsDonating = false;
			PartyScreenManager.Instance.OpenPartyScreen();
		}

		public static void OpenScreenAsRansom()
		{
			PartyScreenManager.Instance._currentMode = PartyScreenMode.Ransom;
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance.IsDonating = false;
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.TransferableWithTrade;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.NotTransferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate);
			PartyBase partyBase = null;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.SellPrisonersDoneHandler);
			TextObject textObject = new TextObject("{=SvahUNo6}Ransom Prisoners", null);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(troopRoster, troopRoster2, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, GameTexts.FindText("str_ransom_broker", null), textObject, null, 0, 0, partyPresentationDoneButtonDelegate, null, null, null, null, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsLoot(TroopRoster leftMemberRoster, TroopRoster leftPrisonerRoster, TextObject leftPartyName, int leftPartySizeLimit, PartyScreenClosedDelegate partyScreenClosedDelegate = null)
		{
			PartyScreenManager.Instance._currentMode = PartyScreenMode.Loot;
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate);
			PartyBase partyBase = null;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DefaultDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(leftMemberRoster, leftPrisonerRoster, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, leftPartyName, new TextObject("{=EOQcQa5l}Aftermath", null), null, leftPartySizeLimit, 0, partyPresentationDoneButtonDelegate, null, null, null, partyScreenClosedDelegate, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsManageTroopsAndPrisoners(MobileParty leftParty, PartyScreenClosedDelegate onPartyScreenClosed = null)
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.Normal;
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.ClanManageTroopAndPrisonerTransferableDelegate);
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.ManageTroopsAndPrisonersDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainPartyAndOther(leftParty, transferState, transferState2, transferState3, isTroopTransferableDelegate, new TextObject("{=uQgNPJnc}Manage Troops", null), partyPresentationDoneButtonDelegate, null, null, null, onPartyScreenClosed, false, false, true, false);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsReceiveTroops(TroopRoster leftMemberParty, TextObject leftPartyName, PartyScreenClosedDelegate partyScreenClosedDelegate = null)
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate);
			PartyBase partyBase = null;
			int totalManCount = leftMemberParty.TotalManCount;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DefaultDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(leftMemberParty, troopRoster, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, leftPartyName, new TextObject("{=uQgNPJnc}Manage Troops", null), null, totalManCount, 0, partyPresentationDoneButtonDelegate, null, null, null, partyScreenClosedDelegate, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsManageTroops(MobileParty leftParty)
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainPartyAndOther(leftParty, PartyScreenLogic.TransferState.Transferable, PartyScreenLogic.TransferState.NotTransferable, PartyScreenLogic.TransferState.Transferable, new IsTroopTransferableDelegate(PartyScreenManager.ClanManageTroopTransferableDelegate), new TextObject("{=uQgNPJnc}Manage Troops", null), new PartyPresentationDoneButtonDelegate(PartyScreenManager.DefaultDoneHandler), null, null, null, null, false, false, true, false);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsDonateTroops(MobileParty leftParty)
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			PartyScreenManager.Instance.IsDonating = leftParty.Owner.Clan != Clan.PlayerClan;
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.DonateModeTroopTransferableDelegate);
			PartyPresentationDoneButtonConditionDelegate partyPresentationDoneButtonConditionDelegate = new PartyPresentationDoneButtonConditionDelegate(PartyScreenManager.DonateDonePossibleDelegate);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainPartyAndOther(leftParty, transferState, transferState2, transferState3, isTroopTransferableDelegate, new TextObject("{=4YfjgtO2}Donate Troops", null), null, partyPresentationDoneButtonConditionDelegate, null, null, null, false, false, true, false);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsDonateGarrisonWithCurrentSettlement()
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			PartyScreenManager.Instance.IsDonating = true;
			if (Hero.MainHero.CurrentSettlement.Town.GarrisonParty == null)
			{
				Hero.MainHero.CurrentSettlement.AddGarrisonParty(false);
			}
			MobileParty garrisonParty = Hero.MainHero.CurrentSettlement.Town.GarrisonParty;
			int num = Math.Max(garrisonParty.Party.PartySizeLimit - garrisonParty.Party.NumberOfAllMembers, 0);
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate);
			PartyBase partyBase = null;
			TextObject name = garrisonParty.Name;
			int num2 = num;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DonateGarrisonDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(troopRoster, troopRoster2, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, name, new TextObject("{=uQgNPJnc}Manage Troops", null), null, num2, 0, partyPresentationDoneButtonDelegate, null, null, null, null, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsDonatePrisoners()
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.PrisonerManage;
			PartyScreenManager.Instance.IsDonating = true;
			if (Hero.MainHero.CurrentSettlement.Town.GarrisonParty == null)
			{
				Hero.MainHero.CurrentSettlement.AddGarrisonParty(false);
			}
			TroopRoster prisonRoster = Hero.MainHero.CurrentSettlement.Party.PrisonRoster;
			int num = Math.Max(Hero.MainHero.CurrentSettlement.Party.PrisonerSizeLimit - prisonRoster.Count, 0);
			TextObject textObject = new TextObject("{=SDzIAtiA}Prisoners of {SETTLEMENT_NAME}", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", Hero.MainHero.CurrentSettlement.Name);
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = prisonRoster;
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.NotTransferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.DonatePrisonerTransferableDelegate);
			PartyBase partyBase = null;
			TextObject textObject2 = textObject;
			int num2 = num;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.DonatePrisonersDoneHandler);
			PartyPresentationDoneButtonConditionDelegate partyPresentationDoneButtonConditionDelegate = new PartyPresentationDoneButtonConditionDelegate(PartyScreenManager.DonateDonePossibleDelegate);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(troopRoster, troopRoster2, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, textObject2, new TextObject("{=Z212GSiV}Leave Prisoners", null), null, 0, num2, partyPresentationDoneButtonDelegate, partyPresentationDoneButtonConditionDelegate, null, null, null, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		private static Tuple<bool, TextObject> DonateDonePossibleDelegate(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, int leftLimitNum, int rightLimitNum)
		{
			if (PartyScreenManager.PartyScreenLogic.CurrentData.TransferredPrisonersHistory.Any((Tuple<CharacterObject, int> p) => p.Item2 > 0))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=hI7eDbXs}You cannot take prisoners.", null));
			}
			if (PartyScreenManager.PartyScreenLogic.HaveRightSideGainedTroops())
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=pvkl6pZh}You cannot take troops.", null));
			}
			if ((PartyScreenManager.PartyScreenLogic.MemberTransferState != PartyScreenLogic.TransferState.NotTransferable || PartyScreenManager.PartyScreenLogic.AccompanyingTransferState != PartyScreenLogic.TransferState.NotTransferable) && PartyScreenManager.PartyScreenLogic.LeftPartyMembersSizeLimit < PartyScreenManager.PartyScreenLogic.MemberRosters[0].TotalManCount)
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=R7wiHjcL}Donated troops exceed party capacity.", null));
			}
			if (PartyScreenManager.PartyScreenLogic.PrisonerTransferState != PartyScreenLogic.TransferState.NotTransferable && PartyScreenManager.PartyScreenLogic.LeftPartyPrisonersSizeLimit < PartyScreenManager.PartyScreenLogic.PrisonerRosters[0].TotalManCount)
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=3nfPGbN0}Donated prisoners exceed party capacity.", null));
			}
			return new Tuple<bool, TextObject>(true, TextObject.Empty);
		}

		public static bool DonatePrisonerTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase LeftOwnerParty)
		{
			return side == PartyScreenLogic.PartyRosterSide.Right && type == PartyScreenLogic.TroopType.Prisoner;
		}

		public static void OpenScreenAsManagePrisoners()
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.PrisonerManage;
			TroopRoster prisonRoster = Hero.MainHero.CurrentSettlement.Party.PrisonRoster;
			TextObject textObject = new TextObject("{=SDzIAtiA}Prisoners of {SETTLEMENT_NAME}", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", Hero.MainHero.CurrentSettlement.Name);
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = prisonRoster;
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.NotTransferable;
			IsTroopTransferableDelegate isTroopTransferableDelegate = new IsTroopTransferableDelegate(PartyScreenManager.TroopTransferableDelegate);
			PartyBase partyBase = null;
			TextObject textObject2 = textObject;
			int prisonerSizeLimit = Hero.MainHero.CurrentSettlement.Party.PrisonerSizeLimit;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.ManageGarrisonDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(troopRoster, troopRoster2, transferState, transferState2, transferState3, isTroopTransferableDelegate, partyBase, textObject2, new TextObject("{=aadTnAEg}Manage Prisoners", null), null, 0, prisonerSizeLimit, partyPresentationDoneButtonDelegate, null, null, null, null, false, false, false, false, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static bool TroopTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase leftOwnerParty)
		{
			Hero hero = ((leftOwnerParty != null) ? leftOwnerParty.LeaderHero : null);
			bool flag;
			if ((hero == null || hero.Clan != Clan.PlayerClan) && (leftOwnerParty == null || !leftOwnerParty.IsMobile || !leftOwnerParty.MobileParty.IsCaravan || leftOwnerParty.Owner != Hero.MainHero))
			{
				if (leftOwnerParty != null && leftOwnerParty.IsMobile && leftOwnerParty.MobileParty.IsGarrison)
				{
					Settlement currentSettlement = leftOwnerParty.MobileParty.CurrentSettlement;
					flag = ((currentSettlement != null) ? currentSettlement.OwnerClan : null) == Clan.PlayerClan;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			return !character.IsHero || (character.IsHero && character.HeroObject.Clan != Clan.PlayerClan && (!character.HeroObject.IsPlayerCompanion || (character.HeroObject.IsPlayerCompanion && flag2)));
		}

		public static bool ClanManageTroopAndPrisonerTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase LeftOwnerParty)
		{
			return !character.IsHero || character.HeroObject.IsPrisoner;
		}

		public static bool ClanManageTroopTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase LeftOwnerParty)
		{
			return !character.IsHero;
		}

		public static bool DonateModeTroopTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase LeftOwnerParty)
		{
			return !character.IsHero && side == PartyScreenLogic.PartyRosterSide.Right;
		}

		public static void OpenScreenWithCondition(IsTroopTransferableDelegate isTroopTransferable, PartyPresentationDoneButtonConditionDelegate doneButtonCondition, PartyPresentationDoneButtonDelegate onDoneClicked, PartyPresentationCancelButtonDelegate onCancelClicked, PartyScreenLogic.TransferState memberTransferState, PartyScreenLogic.TransferState prisonerTransferState, TextObject leftPartyName, int limit, bool showProgressBar, bool isDonating, PartyScreenMode screenMode = PartyScreenMode.Normal, TroopRoster memberRosterLeft = null, TroopRoster prisonerRosterLeft = null)
		{
			if (memberRosterLeft == null)
			{
				memberRosterLeft = TroopRoster.CreateDummyTroopRoster();
			}
			if (prisonerRosterLeft == null)
			{
				prisonerRosterLeft = TroopRoster.CreateDummyTroopRoster();
			}
			PartyScreenManager.Instance._currentMode = screenMode;
			PartyScreenManager.Instance.IsDonating = isDonating;
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(memberRosterLeft, prisonerRosterLeft, memberTransferState, prisonerTransferState, PartyScreenLogic.TransferState.NotTransferable, isTroopTransferable, null, leftPartyName, new TextObject("{=nZaeTlj8}Exchange Troops", null), null, limit, 0, onDoneClicked, doneButtonCondition, onCancelClicked, null, null, false, false, false, showProgressBar, 0);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenForManagingAlley(TroopRoster memberRosterLeft, IsTroopTransferableDelegate isTroopTransferable, PartyPresentationDoneButtonConditionDelegate doneButtonCondition, PartyPresentationDoneButtonDelegate onDoneClicked, TextObject leftPartyName, PartyPresentationCancelButtonDelegate onCancelButtonClicked)
		{
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(memberRosterLeft, TroopRoster.CreateDummyTroopRoster(), PartyScreenLogic.TransferState.Transferable, PartyScreenLogic.TransferState.NotTransferable, PartyScreenLogic.TransferState.NotTransferable, isTroopTransferable, null, leftPartyName, null, null, Campaign.Current.Models.AlleyModel.MaximumTroopCountInPlayerOwnedAlley + 1, 0, onDoneClicked, doneButtonCondition, onCancelButtonClicked, null, null, false, false, false, true, 0);
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenAsQuest(TroopRoster leftMemberRoster, TextObject leftPartyName, int leftPartySizeLimit, int questDaysMultiplier, PartyPresentationDoneButtonConditionDelegate doneButtonCondition, PartyScreenClosedDelegate onPartyScreenClosed, IsTroopTransferableDelegate isTroopTransferable, PartyPresentationCancelButtonActivateDelegate partyPresentationCancelButtonActivateDelegate = null)
		{
			Debug.Print("PartyScreenManager::OpenScreenAsQuest", 0, Debug.DebugColor.White, 17592186044416UL);
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.QuestTroopManage;
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			PartyScreenLogic.TransferState transferState = PartyScreenLogic.TransferState.Transferable;
			PartyScreenLogic.TransferState transferState2 = PartyScreenLogic.TransferState.NotTransferable;
			PartyScreenLogic.TransferState transferState3 = PartyScreenLogic.TransferState.Transferable;
			PartyBase partyBase = null;
			PartyPresentationDoneButtonDelegate partyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.ManageTroopsAndPrisonersDoneHandler);
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(leftMemberRoster, troopRoster, transferState, transferState2, transferState3, isTroopTransferable, partyBase, leftPartyName, new TextObject("{=nZaeTlj8}Exchange Troops", null), null, leftPartySizeLimit, 0, partyPresentationDoneButtonDelegate, doneButtonCondition, null, partyPresentationCancelButtonActivateDelegate, onPartyScreenClosed, false, true, false, true, questDaysMultiplier);
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenWithDummyRoster(TroopRoster leftMemberRoster, TroopRoster leftPrisonerRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonerRoster, TextObject leftPartyName, TextObject rightPartyName, int leftPartySizeLimit, int rightPartySizeLimit, PartyPresentationDoneButtonConditionDelegate doneButtonCondition, PartyScreenClosedDelegate onPartyScreenClosed, IsTroopTransferableDelegate isTroopTransferable, PartyPresentationCancelButtonActivateDelegate partyPresentationCancelButtonActivateDelegate = null)
		{
			Debug.Print("PartyScreenManager::OpenScreenWithDummyRoster", 0, Debug.DebugColor.White, 17592186044416UL);
			PartyScreenManager.Instance._partyScreenLogic = new PartyScreenLogic();
			PartyScreenManager.Instance._currentMode = PartyScreenMode.TroopsManage;
			PartyScreenLogicInitializationData partyScreenLogicInitializationData = new PartyScreenLogicInitializationData
			{
				LeftOwnerParty = null,
				RightOwnerParty = MobileParty.MainParty.Party,
				LeftMemberRoster = leftMemberRoster,
				LeftPrisonerRoster = leftPrisonerRoster,
				RightMemberRoster = rightMemberRoster,
				RightPrisonerRoster = rightPrisonerRoster,
				LeftLeaderHero = null,
				RightLeaderHero = PartyBase.MainParty.LeaderHero,
				LeftPartyMembersSizeLimit = leftPartySizeLimit,
				LeftPartyPrisonersSizeLimit = 0,
				RightPartyMembersSizeLimit = rightPartySizeLimit,
				RightPartyPrisonersSizeLimit = 0,
				LeftPartyName = leftPartyName,
				RightPartyName = rightPartyName,
				TroopTransferableDelegate = isTroopTransferable,
				PartyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenManager.ManageTroopsAndPrisonersDoneHandler),
				PartyPresentationDoneButtonConditionDelegate = doneButtonCondition,
				PartyPresentationCancelButtonActivateDelegate = partyPresentationCancelButtonActivateDelegate,
				PartyPresentationCancelButtonDelegate = null,
				PartyScreenClosedDelegate = onPartyScreenClosed,
				IsDismissMode = true,
				IsTroopUpgradesDisabled = true,
				Header = null,
				TransferHealthiesGetWoundedsFirst = true,
				ShowProgressBar = false,
				MemberTransferState = PartyScreenLogic.TransferState.Transferable,
				PrisonerTransferState = PartyScreenLogic.TransferState.NotTransferable,
				AccompanyingTransferState = PartyScreenLogic.TransferState.Transferable
			};
			PartyScreenManager.Instance._partyScreenLogic.Initialize(partyScreenLogicInitializationData);
			PartyScreenManager.Instance.IsDonating = false;
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.InitializeLogic(PartyScreenManager.Instance._partyScreenLogic);
			Game.Current.GameStateManager.PushState(partyState, 0);
		}

		public static void OpenScreenWithDummyRosterWithMainParty(TroopRoster leftMemberRoster, TroopRoster leftPrisonerRoster, TextObject leftPartyName, int leftPartySizeLimit, PartyPresentationDoneButtonConditionDelegate doneButtonCondition, PartyScreenClosedDelegate onPartyScreenClosed, IsTroopTransferableDelegate isTroopTransferable, PartyPresentationCancelButtonActivateDelegate partyPresentationCancelButtonActivateDelegate = null)
		{
			Debug.Print("PartyScreenManager::OpenScreenWithDummyRosterWithMainParty", 0, Debug.DebugColor.White, 17592186044416UL);
			PartyScreenManager.OpenScreenWithDummyRoster(leftMemberRoster, leftPrisonerRoster, MobileParty.MainParty.MemberRoster, MobileParty.MainParty.PrisonRoster, leftPartyName, MobileParty.MainParty.Name, leftPartySizeLimit, MobileParty.MainParty.Party.PartySizeLimit, doneButtonCondition, onPartyScreenClosed, isTroopTransferable, partyPresentationCancelButtonActivateDelegate);
		}

		public static void OpenScreenAsCreateClanPartyForHero(Hero hero, PartyScreenClosedDelegate onScreenClosed = null, IsTroopTransferableDelegate isTroopTransferable = null)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster3 = MobileParty.MainParty.MemberRoster.CloneRosterData();
			TroopRoster troopRoster4 = MobileParty.MainParty.PrisonRoster.CloneRosterData();
			troopRoster.AddToCounts(hero.CharacterObject, 1, false, 0, 0, true, -1);
			troopRoster3.AddToCounts(hero.CharacterObject, -1, false, 0, 0, true, -1);
			TextObject textObject = GameTexts.FindText("str_lord_party_name", null);
			textObject.SetCharacterProperties("TROOP", hero.CharacterObject, false);
			PartyScreenManager.OpenScreenWithDummyRoster(troopRoster, troopRoster2, troopRoster3, troopRoster4, textObject, MobileParty.MainParty.Name, Campaign.Current.Models.PartySizeLimitModel.GetAssumedPartySizeForLordParty(hero, hero.Clan.MapFaction, hero.Clan), MobileParty.MainParty.LimitedPartySize, null, onScreenClosed ?? new PartyScreenClosedDelegate(PartyScreenManager.OpenScreenAsCreateClanPartyForHeroPartyScreenClosed), isTroopTransferable ?? new IsTroopTransferableDelegate(PartyScreenManager.OpenScreenAsCreateClanPartyForHeroTroopTransferableDelegate), null);
		}

		private static void OpenScreenAsCreateClanPartyForHeroPartyScreenClosed(PartyBase leftOwnerParty, TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, PartyBase rightOwnerParty, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, bool fromCancel)
		{
			if (!fromCancel)
			{
				Hero hero = null;
				for (int i = 0; i < leftMemberRoster.data.Length; i++)
				{
					CharacterObject character = leftMemberRoster.data[i].Character;
					if (character != null && character.IsHero)
					{
						hero = leftMemberRoster.data[i].Character.HeroObject;
					}
				}
				MobileParty mobileParty = hero.Clan.CreateNewMobileParty(hero);
				foreach (TroopRosterElement troopRosterElement in leftMemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character != hero.CharacterObject)
					{
						mobileParty.MemberRoster.Add(troopRosterElement);
						rightOwnerParty.MemberRoster.AddToCounts(troopRosterElement.Character, -troopRosterElement.Number, false, -troopRosterElement.WoundedNumber, -troopRosterElement.Xp, true, -1);
					}
				}
				foreach (TroopRosterElement troopRosterElement2 in leftPrisonRoster.GetTroopRoster())
				{
					mobileParty.PrisonRoster.Add(troopRosterElement2);
					rightOwnerParty.PrisonRoster.AddToCounts(troopRosterElement2.Character, -troopRosterElement2.Number, false, -troopRosterElement2.WoundedNumber, -troopRosterElement2.Xp, true, -1);
				}
			}
		}

		private static bool OpenScreenAsCreateClanPartyForHeroTroopTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase LeftOwnerParty)
		{
			return !character.IsHero;
		}

		private static bool SellPrisonersDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			SellPrisonersAction.ApplyForSelectedPrisoners(MobileParty.MainParty, leftPrisonRoster, Hero.MainHero.CurrentSettlement);
			return true;
		}

		private static bool DonateGarrisonDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
			MobileParty mobileParty = currentSettlement.Town.GarrisonParty;
			if (mobileParty == null)
			{
				currentSettlement.AddGarrisonParty(false);
				mobileParty = currentSettlement.Town.GarrisonParty;
			}
			for (int i = 0; i < leftMemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = leftMemberRoster.GetElementCopyAtIndex(i);
				mobileParty.AddElementToMemberRoster(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false);
				if (elementCopyAtIndex.Character.IsHero)
				{
					EnterSettlementAction.ApplyForCharacterOnly(elementCopyAtIndex.Character.HeroObject, currentSettlement);
				}
			}
			return true;
		}

		private static bool DonatePrisonersDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster leftSideTransferredPrisonerRoster, FlattenedTroopRoster rightSideTransferredPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			if (!rightSideTransferredPrisonerRoster.IsEmpty<FlattenedTroopRosterElement>())
			{
				Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
				foreach (CharacterObject characterObject in rightSideTransferredPrisonerRoster.Troops)
				{
					if (characterObject.IsHero)
					{
						EnterSettlementAction.ApplyForPrisoner(characterObject.HeroObject, currentSettlement);
					}
				}
				CampaignEventDispatcher.Instance.OnPrisonerDonatedToSettlement(rightParty.MobileParty, rightSideTransferredPrisonerRoster, currentSettlement);
			}
			return true;
		}

		private static bool ManageGarrisonDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
			for (int i = 0; i < leftMemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = leftMemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character.IsHero)
				{
					EnterSettlementAction.ApplyForCharacterOnly(elementCopyAtIndex.Character.HeroObject, currentSettlement);
				}
			}
			for (int j = 0; j < leftPrisonRoster.Count; j++)
			{
				TroopRosterElement elementCopyAtIndex2 = leftPrisonRoster.GetElementCopyAtIndex(j);
				if (elementCopyAtIndex2.Character.IsHero)
				{
					EnterSettlementAction.ApplyForPrisoner(elementCopyAtIndex2.Character.HeroObject, currentSettlement);
				}
			}
			return true;
		}

		private static bool ManageTroopsAndPrisonersDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			return true;
		}

		private static bool DefaultDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			PartyScreenManager.HandleReleasedAndTakenPrisoners(takenPrisonerRoster, releasedPrisonerRoster);
			return true;
		}

		private static void HandleReleasedAndTakenPrisoners(FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster)
		{
			if (!releasedPrisonerRoster.IsEmpty<FlattenedTroopRosterElement>())
			{
				EndCaptivityAction.ApplyByReleasedByChoice(releasedPrisonerRoster);
			}
			if (!takenPrisonerRoster.IsEmpty<FlattenedTroopRosterElement>())
			{
				TakePrisonerAction.ApplyByTakenFromPartyScreen(takenPrisonerRoster);
			}
		}

		private PartyScreenMode _currentMode;

		private PartyScreenLogic _partyScreenLogic;

		private static readonly int _countToAddForEachTroopCheatMode = 10;
	}
}

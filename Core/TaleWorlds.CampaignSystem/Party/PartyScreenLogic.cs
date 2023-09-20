using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Party
{
	// Token: 0x020002AD RID: 685
	public class PartyScreenLogic
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060026A4 RID: 9892 RVA: 0x000A36A4 File Offset: 0x000A18A4
		// (remove) Token: 0x060026A5 RID: 9893 RVA: 0x000A36DC File Offset: 0x000A18DC
		public event PartyScreenLogic.PartyGoldDelegate PartyGoldChange;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060026A6 RID: 9894 RVA: 0x000A3714 File Offset: 0x000A1914
		// (remove) Token: 0x060026A7 RID: 9895 RVA: 0x000A374C File Offset: 0x000A194C
		public event PartyScreenLogic.PartyMoraleDelegate PartyMoraleChange;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060026A8 RID: 9896 RVA: 0x000A3784 File Offset: 0x000A1984
		// (remove) Token: 0x060026A9 RID: 9897 RVA: 0x000A37BC File Offset: 0x000A19BC
		public event PartyScreenLogic.PartyInfluenceDelegate PartyInfluenceChange;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060026AA RID: 9898 RVA: 0x000A37F4 File Offset: 0x000A19F4
		// (remove) Token: 0x060026AB RID: 9899 RVA: 0x000A382C File Offset: 0x000A1A2C
		public event PartyScreenLogic.PartyHorseDelegate PartyHorseChange;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060026AC RID: 9900 RVA: 0x000A3864 File Offset: 0x000A1A64
		// (remove) Token: 0x060026AD RID: 9901 RVA: 0x000A389C File Offset: 0x000A1A9C
		public event PartyScreenLogic.PresentationUpdate Update;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060026AE RID: 9902 RVA: 0x000A38D4 File Offset: 0x000A1AD4
		// (remove) Token: 0x060026AF RID: 9903 RVA: 0x000A390C File Offset: 0x000A1B0C
		public event PartyScreenClosedDelegate PartyScreenClosedEvent;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060026B0 RID: 9904 RVA: 0x000A3944 File Offset: 0x000A1B44
		// (remove) Token: 0x060026B1 RID: 9905 RVA: 0x000A397C File Offset: 0x000A1B7C
		public event PartyScreenLogic.AfterResetDelegate AfterReset;

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060026B2 RID: 9906 RVA: 0x000A39B1 File Offset: 0x000A1BB1
		// (set) Token: 0x060026B3 RID: 9907 RVA: 0x000A39B9 File Offset: 0x000A1BB9
		public PartyScreenLogic.TroopSortType ActiveOtherPartySortType
		{
			get
			{
				return this._activeOtherPartySortType;
			}
			set
			{
				this._activeOtherPartySortType = value;
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x060026B4 RID: 9908 RVA: 0x000A39C2 File Offset: 0x000A1BC2
		// (set) Token: 0x060026B5 RID: 9909 RVA: 0x000A39CA File Offset: 0x000A1BCA
		public PartyScreenLogic.TroopSortType ActiveMainPartySortType
		{
			get
			{
				return this._activeMainPartySortType;
			}
			set
			{
				this._activeMainPartySortType = value;
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060026B6 RID: 9910 RVA: 0x000A39D3 File Offset: 0x000A1BD3
		// (set) Token: 0x060026B7 RID: 9911 RVA: 0x000A39DB File Offset: 0x000A1BDB
		public bool IsOtherPartySortAscending
		{
			get
			{
				return this._isOtherPartySortAscending;
			}
			set
			{
				this._isOtherPartySortAscending = value;
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x060026B8 RID: 9912 RVA: 0x000A39E4 File Offset: 0x000A1BE4
		// (set) Token: 0x060026B9 RID: 9913 RVA: 0x000A39EC File Offset: 0x000A1BEC
		public bool IsMainPartySortAscending
		{
			get
			{
				return this._isMainPartySortAscending;
			}
			set
			{
				this._isMainPartySortAscending = value;
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x060026BA RID: 9914 RVA: 0x000A39F5 File Offset: 0x000A1BF5
		// (set) Token: 0x060026BB RID: 9915 RVA: 0x000A39FD File Offset: 0x000A1BFD
		public PartyScreenLogic.TransferState MemberTransferState { get; private set; }

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x060026BC RID: 9916 RVA: 0x000A3A06 File Offset: 0x000A1C06
		// (set) Token: 0x060026BD RID: 9917 RVA: 0x000A3A0E File Offset: 0x000A1C0E
		public PartyScreenLogic.TransferState PrisonerTransferState { get; private set; }

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x060026BE RID: 9918 RVA: 0x000A3A17 File Offset: 0x000A1C17
		// (set) Token: 0x060026BF RID: 9919 RVA: 0x000A3A1F File Offset: 0x000A1C1F
		public PartyScreenLogic.TransferState AccompanyingTransferState { get; private set; }

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x060026C0 RID: 9920 RVA: 0x000A3A28 File Offset: 0x000A1C28
		// (set) Token: 0x060026C1 RID: 9921 RVA: 0x000A3A30 File Offset: 0x000A1C30
		public TextObject LeftPartyName { get; private set; }

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x060026C2 RID: 9922 RVA: 0x000A3A39 File Offset: 0x000A1C39
		// (set) Token: 0x060026C3 RID: 9923 RVA: 0x000A3A41 File Offset: 0x000A1C41
		public TextObject RightPartyName { get; private set; }

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x060026C4 RID: 9924 RVA: 0x000A3A4A File Offset: 0x000A1C4A
		// (set) Token: 0x060026C5 RID: 9925 RVA: 0x000A3A52 File Offset: 0x000A1C52
		public TextObject Header { get; private set; }

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x060026C6 RID: 9926 RVA: 0x000A3A5B File Offset: 0x000A1C5B
		// (set) Token: 0x060026C7 RID: 9927 RVA: 0x000A3A63 File Offset: 0x000A1C63
		public int LeftPartyMembersSizeLimit { get; private set; }

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x060026C8 RID: 9928 RVA: 0x000A3A6C File Offset: 0x000A1C6C
		// (set) Token: 0x060026C9 RID: 9929 RVA: 0x000A3A74 File Offset: 0x000A1C74
		public int LeftPartyPrisonersSizeLimit { get; private set; }

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x060026CA RID: 9930 RVA: 0x000A3A7D File Offset: 0x000A1C7D
		// (set) Token: 0x060026CB RID: 9931 RVA: 0x000A3A85 File Offset: 0x000A1C85
		public int RightPartyMembersSizeLimit { get; private set; }

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x060026CC RID: 9932 RVA: 0x000A3A8E File Offset: 0x000A1C8E
		// (set) Token: 0x060026CD RID: 9933 RVA: 0x000A3A96 File Offset: 0x000A1C96
		public int RightPartyPrisonersSizeLimit { get; private set; }

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x060026CE RID: 9934 RVA: 0x000A3A9F File Offset: 0x000A1C9F
		// (set) Token: 0x060026CF RID: 9935 RVA: 0x000A3AA7 File Offset: 0x000A1CA7
		public bool ShowProgressBar { get; private set; }

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x060026D0 RID: 9936 RVA: 0x000A3AB0 File Offset: 0x000A1CB0
		// (set) Token: 0x060026D1 RID: 9937 RVA: 0x000A3AB8 File Offset: 0x000A1CB8
		public string DoneReasonString { get; private set; }

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x060026D2 RID: 9938 RVA: 0x000A3AC1 File Offset: 0x000A1CC1
		// (set) Token: 0x060026D3 RID: 9939 RVA: 0x000A3AC9 File Offset: 0x000A1CC9
		public bool IsTroopUpgradesDisabled { get; private set; }

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x060026D4 RID: 9940 RVA: 0x000A3AD2 File Offset: 0x000A1CD2
		// (set) Token: 0x060026D5 RID: 9941 RVA: 0x000A3ADA File Offset: 0x000A1CDA
		public CharacterObject RightPartyLeader { get; private set; }

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x060026D6 RID: 9942 RVA: 0x000A3AE3 File Offset: 0x000A1CE3
		// (set) Token: 0x060026D7 RID: 9943 RVA: 0x000A3AEB File Offset: 0x000A1CEB
		public CharacterObject LeftPartyLeader { get; private set; }

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x060026D8 RID: 9944 RVA: 0x000A3AF4 File Offset: 0x000A1CF4
		// (set) Token: 0x060026D9 RID: 9945 RVA: 0x000A3AFC File Offset: 0x000A1CFC
		public PartyBase LeftOwnerParty { get; private set; }

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x060026DA RID: 9946 RVA: 0x000A3B05 File Offset: 0x000A1D05
		// (set) Token: 0x060026DB RID: 9947 RVA: 0x000A3B0D File Offset: 0x000A1D0D
		public PartyBase RightOwnerParty { get; private set; }

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x060026DC RID: 9948 RVA: 0x000A3B16 File Offset: 0x000A1D16
		// (set) Token: 0x060026DD RID: 9949 RVA: 0x000A3B1E File Offset: 0x000A1D1E
		public PartyScreenData CurrentData { get; private set; }

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x060026DE RID: 9950 RVA: 0x000A3B27 File Offset: 0x000A1D27
		// (set) Token: 0x060026DF RID: 9951 RVA: 0x000A3B2F File Offset: 0x000A1D2F
		public bool TransferHealthiesGetWoundedsFirst { get; private set; }

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x060026E0 RID: 9952 RVA: 0x000A3B38 File Offset: 0x000A1D38
		// (set) Token: 0x060026E1 RID: 9953 RVA: 0x000A3B40 File Offset: 0x000A1D40
		public int QuestModeWageDaysMultiplier { get; private set; }

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x060026E2 RID: 9954 RVA: 0x000A3B49 File Offset: 0x000A1D49
		// (set) Token: 0x060026E3 RID: 9955 RVA: 0x000A3B51 File Offset: 0x000A1D51
		public Game Game
		{
			get
			{
				return this._game;
			}
			set
			{
				this._game = value;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x060026E4 RID: 9956 RVA: 0x000A3B5A File Offset: 0x000A1D5A
		private PartyScreenMode CurrentMode
		{
			get
			{
				return PartyScreenManager.Instance.CurrentMode;
			}
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000A3B68 File Offset: 0x000A1D68
		public PartyScreenLogic()
		{
			this._game = Game.Current;
			this.MemberRosters = new TroopRoster[2];
			this.PrisonerRosters = new TroopRoster[2];
			this.CurrentData = new PartyScreenData();
			this._initialData = new PartyScreenData();
			this._defaultComparers = new Dictionary<PartyScreenLogic.TroopSortType, PartyScreenLogic.TroopComparer>
			{
				{
					PartyScreenLogic.TroopSortType.Custom,
					new PartyScreenLogic.TroopDefaultComparer()
				},
				{
					PartyScreenLogic.TroopSortType.Type,
					new PartyScreenLogic.TroopTypeComparer()
				},
				{
					PartyScreenLogic.TroopSortType.Name,
					new PartyScreenLogic.TroopNameComparer()
				},
				{
					PartyScreenLogic.TroopSortType.Count,
					new PartyScreenLogic.TroopCountComparer()
				},
				{
					PartyScreenLogic.TroopSortType.Tier,
					new PartyScreenLogic.TroopTierComparer()
				}
			};
			this.IsTroopUpgradesDisabled = false;
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x000A3C04 File Offset: 0x000A1E04
		public void Initialize(PartyScreenLogicInitializationData initializationData)
		{
			this.MemberRosters[1] = initializationData.RightMemberRoster;
			this.PrisonerRosters[1] = initializationData.RightPrisonerRoster;
			this.MemberRosters[0] = initializationData.LeftMemberRoster;
			this.PrisonerRosters[0] = initializationData.LeftPrisonerRoster;
			Hero rightLeaderHero = initializationData.RightLeaderHero;
			this.RightPartyLeader = ((rightLeaderHero != null) ? rightLeaderHero.CharacterObject : null);
			Hero leftLeaderHero = initializationData.LeftLeaderHero;
			this.LeftPartyLeader = ((leftLeaderHero != null) ? leftLeaderHero.CharacterObject : null);
			this.RightOwnerParty = initializationData.RightOwnerParty;
			this.LeftOwnerParty = initializationData.LeftOwnerParty;
			this.RightPartyName = initializationData.RightPartyName;
			this.RightPartyMembersSizeLimit = initializationData.RightPartyMembersSizeLimit;
			this.RightPartyPrisonersSizeLimit = initializationData.RightPartyPrisonersSizeLimit;
			this.LeftPartyName = initializationData.LeftPartyName;
			this.LeftPartyMembersSizeLimit = initializationData.LeftPartyMembersSizeLimit;
			this.LeftPartyPrisonersSizeLimit = initializationData.LeftPartyPrisonersSizeLimit;
			this.Header = initializationData.Header;
			this.QuestModeWageDaysMultiplier = initializationData.QuestModeWageDaysMultiplier;
			this.TransferHealthiesGetWoundedsFirst = initializationData.TransferHealthiesGetWoundedsFirst;
			this.SetPartyGoldChangeAmount(0);
			this.SetHorseChangeAmount(0);
			this.SetInfluenceChangeAmount(0, 0, 0);
			this.SetMoraleChangeAmount(0);
			this.CurrentData.BindRostersFrom(this.MemberRosters[1], this.PrisonerRosters[1], this.MemberRosters[0], this.PrisonerRosters[0], this.RightOwnerParty, this.LeftOwnerParty);
			this._initialData.InitializeCopyFrom(initializationData.RightOwnerParty, initializationData.LeftOwnerParty);
			this._initialData.CopyFromPartyAndRoster(this.MemberRosters[1], this.PrisonerRosters[1], this.MemberRosters[0], this.PrisonerRosters[0], this.RightOwnerParty);
			if (initializationData.PartyPresentationDoneButtonDelegate == null)
			{
				Debug.FailedAssert("Done handler is given null for party screen!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyScreenLogic.cs", "Initialize", 238);
				initializationData.PartyPresentationDoneButtonDelegate = new PartyPresentationDoneButtonDelegate(PartyScreenLogic.DefaultDoneHandler);
			}
			this.PartyPresentationDoneButtonDelegate = initializationData.PartyPresentationDoneButtonDelegate;
			this.PartyPresentationDoneButtonConditionDelegate = initializationData.PartyPresentationDoneButtonConditionDelegate;
			this.PartyPresentationCancelButtonActivateDelegate = initializationData.PartyPresentationCancelButtonActivateDelegate;
			this.PartyPresentationCancelButtonDelegate = initializationData.PartyPresentationCancelButtonDelegate;
			this.IsTroopUpgradesDisabled = initializationData.IsTroopUpgradesDisabled || initializationData.RightOwnerParty == null;
			this.MemberTransferState = initializationData.MemberTransferState;
			this.PrisonerTransferState = initializationData.PrisonerTransferState;
			this.AccompanyingTransferState = initializationData.AccompanyingTransferState;
			this.IsTroopTransferableDelegate = initializationData.TroopTransferableDelegate;
			this.PartyPresentationCancelButtonActivateDelegate = initializationData.PartyPresentationCancelButtonActivateDelegate;
			this.PartyPresentationCancelButtonDelegate = initializationData.PartyPresentationCancelButtonDelegate;
			this.PartyScreenClosedEvent = initializationData.PartyScreenClosedDelegate;
			this.ShowProgressBar = initializationData.ShowProgressBar;
			if (this.CurrentMode == PartyScreenMode.QuestTroopManage)
			{
				int num = -this.MemberRosters[0].Sum((TroopRosterElement t) => t.Character.TroopWage * t.Number * this.QuestModeWageDaysMultiplier);
				this._initialData.PartyGoldChangeAmount = num;
				this.SetPartyGoldChangeAmount(num);
			}
		}

		// Token: 0x060026E7 RID: 9959 RVA: 0x000A3EAF File Offset: 0x000A20AF
		private void SetPartyGoldChangeAmount(int newTotalAmount)
		{
			this.CurrentData.PartyGoldChangeAmount = newTotalAmount;
			PartyScreenLogic.PartyGoldDelegate partyGoldChange = this.PartyGoldChange;
			if (partyGoldChange == null)
			{
				return;
			}
			partyGoldChange();
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x000A3ECD File Offset: 0x000A20CD
		private void SetMoraleChangeAmount(int newAmount)
		{
			this.CurrentData.PartyMoraleChangeAmount = newAmount;
			PartyScreenLogic.PartyMoraleDelegate partyMoraleChange = this.PartyMoraleChange;
			if (partyMoraleChange == null)
			{
				return;
			}
			partyMoraleChange();
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000A3EEB File Offset: 0x000A20EB
		private void SetHorseChangeAmount(int newAmount)
		{
			this.CurrentData.PartyHorseChangeAmount = newAmount;
			PartyScreenLogic.PartyHorseDelegate partyHorseChange = this.PartyHorseChange;
			if (partyHorseChange == null)
			{
				return;
			}
			partyHorseChange();
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000A3F09 File Offset: 0x000A2109
		private void SetInfluenceChangeAmount(int heroInfluence, int troopInfluence, int prisonerInfluence)
		{
			this.CurrentData.PartyInfluenceChangeAmount = new ValueTuple<int, int, int>(heroInfluence, troopInfluence, prisonerInfluence);
			PartyScreenLogic.PartyInfluenceDelegate partyInfluenceChange = this.PartyInfluenceChange;
			if (partyInfluenceChange == null)
			{
				return;
			}
			partyInfluenceChange();
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x000A3F30 File Offset: 0x000A2130
		private void ProcessCommand(PartyScreenLogic.PartyCommand command)
		{
			switch (command.Code)
			{
			case PartyScreenLogic.PartyCommandCode.TransferTroop:
				this.TransferTroop(command, true);
				return;
			case PartyScreenLogic.PartyCommandCode.UpgradeTroop:
				this.UpgradeTroop(command);
				return;
			case PartyScreenLogic.PartyCommandCode.TransferPartyLeaderTroop:
				this.TransferPartyLeaderTroop(command);
				return;
			case PartyScreenLogic.PartyCommandCode.TransferTroopToLeaderSlot:
				this.TransferTroopToLeaderSlot(command);
				return;
			case PartyScreenLogic.PartyCommandCode.ShiftTroop:
				this.ShiftTroop(command);
				return;
			case PartyScreenLogic.PartyCommandCode.RecruitTroop:
				this.RecruitPrisoner(command);
				return;
			case PartyScreenLogic.PartyCommandCode.ExecuteTroop:
				this.ExecuteTroop(command);
				return;
			case PartyScreenLogic.PartyCommandCode.TransferAllTroops:
				this.TransferAllTroops(command);
				return;
			case PartyScreenLogic.PartyCommandCode.SortTroops:
				this.SortTroops(command);
				return;
			default:
				return;
			}
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x000A3FB7 File Offset: 0x000A21B7
		public void AddCommand(PartyScreenLogic.PartyCommand command)
		{
			this.ProcessCommand(command);
		}

		// Token: 0x060026ED RID: 9965 RVA: 0x000A3FC0 File Offset: 0x000A21C0
		public bool ValidateCommand(PartyScreenLogic.PartyCommand command)
		{
			if (command.Code == PartyScreenLogic.PartyCommandCode.TransferTroop || command.Code == PartyScreenLogic.PartyCommandCode.TransferTroopToLeaderSlot)
			{
				CharacterObject character = command.Character;
				if (character == CharacterObject.PlayerCharacter)
				{
					return false;
				}
				int num;
				if (command.Type == PartyScreenLogic.TroopType.Member)
				{
					num = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character);
					bool flag = num != -1 && this.MemberRosters[(int)command.RosterSide].GetElementNumber(num) >= command.TotalNumber;
					bool flag2 = command.RosterSide != PartyScreenLogic.PartyRosterSide.Left || command.Index != 0;
					return flag && flag2;
				}
				num = this.PrisonerRosters[(int)command.RosterSide].FindIndexOfTroop(character);
				return num != -1 && this.PrisonerRosters[(int)command.RosterSide].GetElementNumber(num) >= command.TotalNumber;
			}
			else if (command.Code == PartyScreenLogic.PartyCommandCode.ShiftTroop)
			{
				CharacterObject character2 = command.Character;
				if (character2 == this.LeftPartyLeader || character2 == this.RightPartyLeader || ((command.RosterSide != PartyScreenLogic.PartyRosterSide.Left || (this.LeftPartyLeader != null && command.Index == 0)) && (command.RosterSide != PartyScreenLogic.PartyRosterSide.Right || (this.RightPartyLeader != null && command.Index == 0))))
				{
					return false;
				}
				int num2;
				if (command.Type == PartyScreenLogic.TroopType.Member)
				{
					num2 = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character2);
					return num2 != -1 && num2 != command.Index;
				}
				num2 = this.PrisonerRosters[(int)command.RosterSide].FindIndexOfTroop(character2);
				return num2 != -1 && num2 != command.Index;
			}
			else
			{
				if (command.Code == PartyScreenLogic.PartyCommandCode.TransferPartyLeaderTroop)
				{
					CharacterObject character3 = command.Character;
					BasicCharacterObject playerTroop = this._game.PlayerTroop;
					return false;
				}
				if (command.Code == PartyScreenLogic.PartyCommandCode.UpgradeTroop)
				{
					CharacterObject character4 = command.Character;
					int num3 = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character4);
					if (num3 == -1 || this.MemberRosters[(int)command.RosterSide].GetElementNumber(num3) < command.TotalNumber || character4.UpgradeTargets.Length == 0)
					{
						return false;
					}
					if (command.UpgradeTarget >= character4.UpgradeTargets.Length)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=kaQ7DsW3}Character does not have upgrade target.", null), 0, null, "");
						return false;
					}
					CharacterObject characterObject = character4.UpgradeTargets[command.UpgradeTarget];
					int upgradeXpCost = character4.GetUpgradeXpCost(PartyBase.MainParty, command.UpgradeTarget);
					int upgradeGoldCost = character4.GetUpgradeGoldCost(PartyBase.MainParty, command.UpgradeTarget);
					if (this.MemberRosters[(int)command.RosterSide].GetElementXp(num3) < upgradeXpCost * command.TotalNumber)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=m1bIfPf1}Character does not have enough experience for upgrade.", null), 0, null, "");
						return false;
					}
					CharacterObject characterObject2 = ((command.RosterSide == PartyScreenLogic.PartyRosterSide.Left) ? this.LeftPartyLeader : this.RightPartyLeader);
					int? num4 = ((characterObject2 != null) ? new int?(characterObject2.HeroObject.Gold) : null) + this.CurrentData.PartyGoldChangeAmount;
					int num5 = upgradeGoldCost * command.TotalNumber;
					if (!((num4.GetValueOrDefault() >= num5) & (num4 != null)))
					{
						MBTextManager.SetTextVariable("VALUE", upgradeGoldCost);
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_gold_needed_for_upgrade", null), 0, null, "");
						return false;
					}
					if (characterObject.UpgradeRequiresItemFromCategory == null)
					{
						return true;
					}
					foreach (ItemRosterElement itemRosterElement in this.RightOwnerParty.ItemRoster)
					{
						if (itemRosterElement.EquipmentElement.Item.ItemCategory == characterObject.UpgradeRequiresItemFromCategory)
						{
							return true;
						}
					}
					MBTextManager.SetTextVariable("REQUIRED_ITEM", characterObject.UpgradeRequiresItemFromCategory.GetName(), false);
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_item_needed_for_upgrade", null), 0, null, "");
					return false;
				}
				else
				{
					if (command.Code == PartyScreenLogic.PartyCommandCode.RecruitTroop)
					{
						return this.IsPrisonerRecruitable(command.Type, command.Character, command.RosterSide);
					}
					if (command.Code == PartyScreenLogic.PartyCommandCode.ExecuteTroop)
					{
						return this.IsExecutable(command.Type, command.Character, command.RosterSide);
					}
					if (command.Code == PartyScreenLogic.PartyCommandCode.TransferAllTroops)
					{
						return this.GetRoster(command.RosterSide, command.Type).Count != 0;
					}
					if (command.Code == PartyScreenLogic.PartyCommandCode.SortTroops)
					{
						return this.GetActiveSortTypeForSide(command.RosterSide) != command.SortType || this.GetIsAscendingSortForSide(command.RosterSide) != command.IsSortAscending;
					}
					throw new MBUnknownTypeException("Unknown command type in ValidateCommand.");
				}
			}
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000A4470 File Offset: 0x000A2670
		private void OnReset(bool fromCancel)
		{
			PartyScreenLogic.AfterResetDelegate afterReset = this.AfterReset;
			if (afterReset == null)
			{
				return;
			}
			afterReset(this, fromCancel);
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000A4484 File Offset: 0x000A2684
		protected void TransferTroopToLeaderSlot(PartyScreenLogic.PartyCommand command)
		{
			bool flag = false;
			if (this.ValidateCommand(command))
			{
				CharacterObject character = command.Character;
				if (command.Type == PartyScreenLogic.TroopType.Member)
				{
					int num = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character);
					TroopRosterElement elementCopyAtIndex = this.MemberRosters[(int)command.RosterSide].GetElementCopyAtIndex(num);
					int num2 = command.TotalNumber * (elementCopyAtIndex.Xp / elementCopyAtIndex.Number);
					this.MemberRosters[(int)command.RosterSide].AddToCounts(character, -command.TotalNumber, false, -command.WoundedNumber, 0, true, num);
					this.MemberRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddToCounts(character, command.TotalNumber, false, command.WoundedNumber, 0, true, 0);
					if (elementCopyAtIndex.Number != command.TotalNumber)
					{
						this.MemberRosters[(int)command.RosterSide].AddXpToTroop(-num2, character);
					}
					this.MemberRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddXpToTroop(num2, character);
				}
				flag = true;
			}
			if (flag)
			{
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update == null)
				{
					return;
				}
				update(command);
			}
		}

		// Token: 0x060026F0 RID: 9968 RVA: 0x000A45A4 File Offset: 0x000A27A4
		protected void TransferTroop(PartyScreenLogic.PartyCommand command, bool invokeUpdate)
		{
			bool flag = false;
			if (this.ValidateCommand(command))
			{
				CharacterObject troop = command.Character;
				if (command.Type == PartyScreenLogic.TroopType.Member)
				{
					int num = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(troop);
					TroopRosterElement elementCopyAtIndex = this.MemberRosters[(int)command.RosterSide].GetElementCopyAtIndex(num);
					int num2 = ((troop.UpgradeTargets.Length != 0) ? troop.UpgradeTargets.Max((CharacterObject x) => Campaign.Current.Models.PartyTroopUpgradeModel.GetXpCostForUpgrade(PartyBase.MainParty, troop, x)) : 0);
					int num4;
					if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
					{
						int num3 = (elementCopyAtIndex.Number - command.TotalNumber) * num2;
						num4 = ((elementCopyAtIndex.Xp >= num3 && num3 >= 0) ? (elementCopyAtIndex.Xp - num3) : 0);
					}
					else
					{
						int num5 = command.TotalNumber * num2;
						num4 = ((elementCopyAtIndex.Xp > num5 && num5 >= 0) ? num5 : elementCopyAtIndex.Xp);
						this.MemberRosters[(int)command.RosterSide].AddXpToTroop(-num4, troop);
					}
					this.MemberRosters[(int)command.RosterSide].AddToCounts(troop, -command.TotalNumber, false, -command.WoundedNumber, 0, false, num);
					this.MemberRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddToCounts(troop, command.TotalNumber, false, command.WoundedNumber, 0, false, command.Index);
					this.MemberRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddXpToTroop(num4, troop);
				}
				else
				{
					int num6 = this.PrisonerRosters[(int)command.RosterSide].FindIndexOfTroop(troop);
					TroopRosterElement elementCopyAtIndex2 = this.PrisonerRosters[(int)command.RosterSide].GetElementCopyAtIndex(num6);
					int conformityNeededToRecruitPrisoner = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetConformityNeededToRecruitPrisoner(elementCopyAtIndex2.Character);
					int num8;
					if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
					{
						this.UpdatePrisonerTransferHistory(troop, -command.TotalNumber);
						int num7 = (elementCopyAtIndex2.Number - command.TotalNumber) * conformityNeededToRecruitPrisoner;
						num8 = ((elementCopyAtIndex2.Xp >= num7 && num7 >= 0) ? (elementCopyAtIndex2.Xp - num7) : 0);
					}
					else
					{
						this.UpdatePrisonerTransferHistory(troop, command.TotalNumber);
						int num9 = command.TotalNumber * conformityNeededToRecruitPrisoner;
						num8 = ((elementCopyAtIndex2.Xp > num9 && num9 >= 0) ? num9 : elementCopyAtIndex2.Xp);
						this.PrisonerRosters[(int)command.RosterSide].AddXpToTroop(-num8, troop);
					}
					this.PrisonerRosters[(int)command.RosterSide].AddToCounts(troop, -command.TotalNumber, false, -command.WoundedNumber, 0, false, command.Index);
					this.PrisonerRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddToCounts(troop, command.TotalNumber, false, command.WoundedNumber, 0, false, command.Index);
					this.PrisonerRosters[(int)(PartyScreenLogic.PartyRosterSide.Right - command.RosterSide)].AddXpToTroop(num8, troop);
					if (this.CurrentData.RightRecruitableData.ContainsKey(troop))
					{
						this.CurrentData.RightRecruitableData[troop] = MathF.Max(MathF.Min(this.CurrentData.RightRecruitableData[troop], this.PrisonerRosters[1].GetElementNumber(troop)), Campaign.Current.Models.PrisonerRecruitmentCalculationModel.CalculateRecruitableNumber(PartyBase.MainParty, troop));
					}
				}
				flag = true;
			}
			if (flag)
			{
				if (this.PrisonerTransferState == PartyScreenLogic.TransferState.TransferableWithTrade && command.Type == PartyScreenLogic.TroopType.Prisoner)
				{
					int num10 = ((command.RosterSide == PartyScreenLogic.PartyRosterSide.Right) ? 1 : (-1));
					this.SetPartyGoldChangeAmount(this.CurrentData.PartyGoldChangeAmount + Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(command.Character, Hero.MainHero) * command.TotalNumber * num10);
				}
				if (this.CurrentMode == PartyScreenMode.QuestTroopManage)
				{
					int num11 = ((command.RosterSide == PartyScreenLogic.PartyRosterSide.Right) ? (-1) : 1);
					this.SetPartyGoldChangeAmount(this.CurrentData.PartyGoldChangeAmount + command.Character.TroopWage * command.TotalNumber * this.QuestModeWageDaysMultiplier * num11);
				}
				if (PartyScreenManager.Instance.IsDonating)
				{
					Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
					float num12 = 0f;
					float num13 = 0f;
					float num14 = 0f;
					foreach (TroopTradeDifference troopTradeDifference in this._initialData.GetTroopTradeDifferencesFromTo(this.CurrentData))
					{
						int num15 = troopTradeDifference.FromCount - troopTradeDifference.ToCount;
						if (num15 > 0)
						{
							if (!troopTradeDifference.IsPrisoner)
							{
								num13 += (float)num15 * Campaign.Current.Models.PrisonerDonationModel.CalculateInfluenceGainAfterTroopDonation(PartyBase.MainParty, troopTradeDifference.Troop, currentSettlement);
							}
							else if (troopTradeDifference.Troop.IsHero)
							{
								num12 += Campaign.Current.Models.PrisonerDonationModel.CalculateInfluenceGainAfterPrisonerDonation(PartyBase.MainParty, troopTradeDifference.Troop, currentSettlement);
							}
							else
							{
								num14 += (float)num15 * Campaign.Current.Models.PrisonerDonationModel.CalculateInfluenceGainAfterPrisonerDonation(PartyBase.MainParty, troopTradeDifference.Troop, currentSettlement);
							}
						}
					}
					this.SetInfluenceChangeAmount((int)num12, (int)num13, (int)num14);
				}
				if (invokeUpdate)
				{
					PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
					if (updateDelegate != null)
					{
						updateDelegate(command);
					}
					PartyScreenLogic.PresentationUpdate update = this.Update;
					if (update == null)
					{
						return;
					}
					update(command);
				}
			}
		}

		// Token: 0x060026F1 RID: 9969 RVA: 0x000A4B40 File Offset: 0x000A2D40
		protected void ShiftTroop(PartyScreenLogic.PartyCommand command)
		{
			bool flag = false;
			if (this.ValidateCommand(command))
			{
				CharacterObject character = command.Character;
				if (command.Type == PartyScreenLogic.TroopType.Member)
				{
					int num = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character);
					TroopRosterElement elementCopyAtIndex = this.MemberRosters[(int)command.RosterSide].GetElementCopyAtIndex(num);
					int num2 = ((num < command.Index) ? (command.Index - 1) : command.Index);
					this.MemberRosters[(int)command.RosterSide].AddToCounts(character, -elementCopyAtIndex.Number, false, -elementCopyAtIndex.WoundedNumber, 0, true, num);
					this.MemberRosters[(int)command.RosterSide].AddToCounts(character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.Xp, true, (this.MemberRosters[(int)command.RosterSide].Count < num2) ? (-1) : num2);
				}
				else
				{
					int num3 = this.PrisonerRosters[(int)command.RosterSide].FindIndexOfTroop(character);
					TroopRosterElement elementCopyAtIndex2 = this.PrisonerRosters[(int)command.RosterSide].GetElementCopyAtIndex(num3);
					int num4 = ((num3 < command.Index) ? (command.Index - 1) : command.Index);
					this.PrisonerRosters[(int)command.RosterSide].AddToCounts(character, -elementCopyAtIndex2.Number, false, -elementCopyAtIndex2.WoundedNumber, 0, true, num3);
					this.PrisonerRosters[(int)command.RosterSide].AddToCounts(character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, elementCopyAtIndex2.Xp, true, (this.PrisonerRosters[(int)command.RosterSide].Count < num4) ? (-1) : num4);
				}
				flag = true;
			}
			if (flag)
			{
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update == null)
				{
					return;
				}
				update(command);
			}
		}

		// Token: 0x060026F2 RID: 9970 RVA: 0x000A4D02 File Offset: 0x000A2F02
		protected void TransferPartyLeaderTroop(PartyScreenLogic.PartyCommand command)
		{
			if (this.ValidateCommand(command))
			{
				PartyBase partyBase = ((command.RosterSide == PartyScreenLogic.PartyRosterSide.Left) ? this.LeftOwnerParty : this.RightOwnerParty);
			}
		}

		// Token: 0x060026F3 RID: 9971 RVA: 0x000A4D24 File Offset: 0x000A2F24
		protected void UpgradeTroop(PartyScreenLogic.PartyCommand command)
		{
			if (this.ValidateCommand(command))
			{
				CharacterObject character = command.Character;
				CharacterObject characterObject = character.UpgradeTargets[command.UpgradeTarget];
				TroopRoster roster = this.GetRoster(command.RosterSide, command.Type);
				int num = roster.FindIndexOfTroop(character);
				int num2 = character.GetUpgradeXpCost(PartyBase.MainParty, command.UpgradeTarget) * command.TotalNumber;
				roster.SetElementXp(num, roster.GetElementXp(num) - num2);
				List<ValueTuple<EquipmentElement, int>> list = null;
				this.SetPartyGoldChangeAmount(this.CurrentData.PartyGoldChangeAmount - character.GetUpgradeGoldCost(PartyBase.MainParty, command.UpgradeTarget) * command.TotalNumber);
				if (characterObject.UpgradeRequiresItemFromCategory != null)
				{
					list = this.RemoveItemFromItemRoster(characterObject.UpgradeRequiresItemFromCategory, command.TotalNumber);
				}
				int num3 = 0;
				foreach (TroopRosterElement troopRosterElement in roster.GetTroopRoster())
				{
					if (troopRosterElement.Character == character && command.TotalNumber > troopRosterElement.Number - troopRosterElement.WoundedNumber)
					{
						num3 = command.TotalNumber - (troopRosterElement.Number - troopRosterElement.WoundedNumber);
					}
				}
				roster.AddToCounts(character, -command.TotalNumber, false, -num3, 0, true, -1);
				roster.AddToCounts(characterObject, command.TotalNumber, false, num3, 0, true, command.Index);
				this.AddUpgradeToHistory(character, characterObject, command.TotalNumber);
				this.AddUsedHorsesToHistory(list);
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate == null)
				{
					return;
				}
				updateDelegate(command);
			}
		}

		// Token: 0x060026F4 RID: 9972 RVA: 0x000A4EB4 File Offset: 0x000A30B4
		protected void RecruitPrisoner(PartyScreenLogic.PartyCommand command)
		{
			bool flag = false;
			if (this.ValidateCommand(command))
			{
				CharacterObject character = command.Character;
				TroopRoster troopRoster = this.PrisonerRosters[(int)command.RosterSide];
				int num = MathF.Min(this.CurrentData.RightRecruitableData[character], command.TotalNumber);
				if (num > 0)
				{
					Dictionary<CharacterObject, int> rightRecruitableData = this.CurrentData.RightRecruitableData;
					CharacterObject characterObject = character;
					rightRecruitableData[characterObject] -= num;
					int conformityNeededToRecruitPrisoner = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetConformityNeededToRecruitPrisoner(character);
					troopRoster.AddXpToTroop(-conformityNeededToRecruitPrisoner * num, character);
					troopRoster.AddToCounts(character, -num, false, 0, 0, true, -1);
					this.MemberRosters[(int)command.RosterSide].AddToCounts(command.Character, num, false, 0, 0, true, command.Index);
					this.AddRecruitToHistory(character, num);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update == null)
				{
					return;
				}
				update(command);
			}
		}

		// Token: 0x060026F5 RID: 9973 RVA: 0x000A4FB8 File Offset: 0x000A31B8
		protected void ExecuteTroop(PartyScreenLogic.PartyCommand command)
		{
			bool flag = false;
			if (this.ValidateCommand(command))
			{
				CharacterObject character = command.Character;
				this.PrisonerRosters[(int)command.RosterSide].AddToCounts(character, -1, false, 0, 0, true, -1);
				KillCharacterAction.ApplyByExecution(character.HeroObject, Hero.MainHero, true, false);
				flag = true;
			}
			if (flag)
			{
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update != null)
				{
					update(command);
				}
				this._initialData.LeftPrisonerRoster.AddToCounts(command.Character, -1, false, 0, 0, true, -1);
				this._initialData.RightPrisonerRoster.AddToCounts(command.Character, -1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x000A5068 File Offset: 0x000A3268
		protected void TransferAllTroops(PartyScreenLogic.PartyCommand command)
		{
			if (this.ValidateCommand(command))
			{
				PartyScreenLogic.PartyRosterSide partyRosterSide = PartyScreenLogic.PartyRosterSide.Right - command.RosterSide;
				TroopRoster roster = this.GetRoster(command.RosterSide, command.Type);
				List<TroopRosterElement> listFromRoster = this.GetListFromRoster(roster);
				int num = -1;
				if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
				{
					if (command.Type == PartyScreenLogic.TroopType.Prisoner)
					{
						num = this.LeftPartyPrisonersSizeLimit - this.PrisonerRosters[0].TotalManCount;
					}
					else
					{
						num = this.LeftPartyMembersSizeLimit - this.MemberRosters[0].TotalManCount;
					}
				}
				else if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Left)
				{
					if (command.Type == PartyScreenLogic.TroopType.Prisoner)
					{
						num = this.RightPartyPrisonersSizeLimit - this.PrisonerRosters[1].TotalManCount;
					}
					else
					{
						num = this.RightPartyMembersSizeLimit - this.MemberRosters[1].TotalManCount;
					}
				}
				if (num <= 0)
				{
					num = listFromRoster.Sum((TroopRosterElement x) => x.Number);
				}
				IEnumerable<string> enumerable = ((command.Type == PartyScreenLogic.TroopType.Member) ? Campaign.Current.GetCampaignBehavior<IViewDataTracker>().GetPartyTroopLocks() : Campaign.Current.GetCampaignBehavior<IViewDataTracker>().GetPartyPrisonerLocks());
				int num2 = 0;
				while (num2 < listFromRoster.Count && num > 0)
				{
					TroopRosterElement troopRosterElement = listFromRoster[num2];
					if ((command.RosterSide != PartyScreenLogic.PartyRosterSide.Right || !enumerable.Contains(troopRosterElement.Character.StringId)) && this.IsTroopTransferable(command.Type, troopRosterElement.Character, (int)command.RosterSide))
					{
						PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
						int num3 = MBMath.ClampInt(troopRosterElement.Number, 0, num);
						partyCommand.FillForTransferTroop(command.RosterSide, command.Type, troopRosterElement.Character, num3, troopRosterElement.WoundedNumber, -1);
						this.TransferTroop(partyCommand, false);
						num -= num3;
					}
					num2++;
				}
				PartyScreenLogic.TroopSortType activeSortTypeForSide = this.GetActiveSortTypeForSide(partyRosterSide);
				if (activeSortTypeForSide != PartyScreenLogic.TroopSortType.Custom)
				{
					TroopRoster roster2 = this.GetRoster(partyRosterSide, command.Type);
					this.SortRosterByType(roster2, activeSortTypeForSide);
				}
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update == null)
				{
					return;
				}
				update(command);
			}
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x000A5274 File Offset: 0x000A3474
		protected void SortTroops(PartyScreenLogic.PartyCommand command)
		{
			if (this.ValidateCommand(command))
			{
				this.SetActiveSortTypeForSide(command.RosterSide, command.SortType);
				this.SetIsAscendingForSide(command.RosterSide, command.IsSortAscending);
				this.UpdateComparersAscendingOrder(command.IsSortAscending);
				if (command.SortType != PartyScreenLogic.TroopSortType.Custom)
				{
					TroopRoster roster = this.GetRoster(command.RosterSide, PartyScreenLogic.TroopType.Member);
					TroopRoster roster2 = this.GetRoster(command.RosterSide, PartyScreenLogic.TroopType.Prisoner);
					this.SortRosterByType(roster, command.SortType);
					this.SortRosterByType(roster2, command.SortType);
				}
				PartyScreenLogic.PresentationUpdate updateDelegate = this.UpdateDelegate;
				if (updateDelegate != null)
				{
					updateDelegate(command);
				}
				PartyScreenLogic.PresentationUpdate update = this.Update;
				if (update == null)
				{
					return;
				}
				update(command);
			}
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000A5320 File Offset: 0x000A3520
		public int GetIndexToInsertTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, TroopRosterElement troop)
		{
			PartyScreenLogic.TroopSortType activeSortTypeForSide = this.GetActiveSortTypeForSide(side);
			if (activeSortTypeForSide != PartyScreenLogic.TroopSortType.Custom)
			{
				return -1;
			}
			PartyScreenLogic.TroopComparer comparer = this.GetComparer(activeSortTypeForSide);
			TroopRoster roster = this.GetRoster(side, type);
			for (int i = 0; i < roster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = roster.GetElementCopyAtIndex(i);
				if (!elementCopyAtIndex.Character.IsHero)
				{
					if (elementCopyAtIndex.Character.StringId == troop.Character.StringId)
					{
						return -1;
					}
					if (comparer.Compare(elementCopyAtIndex, troop) < 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x000A53A2 File Offset: 0x000A35A2
		public PartyScreenLogic.TroopSortType GetActiveSortTypeForSide(PartyScreenLogic.PartyRosterSide side)
		{
			if (side == PartyScreenLogic.PartyRosterSide.Left)
			{
				return this.ActiveOtherPartySortType;
			}
			if (side == PartyScreenLogic.PartyRosterSide.Right)
			{
				return this.ActiveMainPartySortType;
			}
			return PartyScreenLogic.TroopSortType.Invalid;
		}

		// Token: 0x060026FA RID: 9978 RVA: 0x000A53BA File Offset: 0x000A35BA
		private void SetActiveSortTypeForSide(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopSortType sortType)
		{
			if (side == PartyScreenLogic.PartyRosterSide.Left)
			{
				this.ActiveOtherPartySortType = sortType;
				return;
			}
			if (side == PartyScreenLogic.PartyRosterSide.Right)
			{
				this.ActiveMainPartySortType = sortType;
			}
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x000A53D2 File Offset: 0x000A35D2
		public bool GetIsAscendingSortForSide(PartyScreenLogic.PartyRosterSide side)
		{
			if (side == PartyScreenLogic.PartyRosterSide.Left)
			{
				return this.IsOtherPartySortAscending;
			}
			return side == PartyScreenLogic.PartyRosterSide.Right && this.IsMainPartySortAscending;
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x000A53EA File Offset: 0x000A35EA
		private void SetIsAscendingForSide(PartyScreenLogic.PartyRosterSide side, bool isAscending)
		{
			if (side == PartyScreenLogic.PartyRosterSide.Left)
			{
				this.IsOtherPartySortAscending = isAscending;
				return;
			}
			if (side == PartyScreenLogic.PartyRosterSide.Right)
			{
				this.IsMainPartySortAscending = isAscending;
			}
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x000A5404 File Offset: 0x000A3604
		private List<TroopRosterElement> GetListFromRoster(TroopRoster roster)
		{
			List<TroopRosterElement> list = new List<TroopRosterElement>();
			for (int i = 0; i < roster.Count; i++)
			{
				list.Add(roster.GetElementCopyAtIndex(i));
			}
			return list;
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x000A5438 File Offset: 0x000A3638
		private CharacterObject GetLeaderOfRoster(TroopRoster roster)
		{
			if (roster == this.MemberRosters[0] || roster == this.PrisonerRosters[0])
			{
				return this.LeftPartyLeader;
			}
			if (roster == this.MemberRosters[1] || roster == this.PrisonerRosters[1])
			{
				return this.RightPartyLeader;
			}
			return null;
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x000A5494 File Offset: 0x000A3694
		private void SyncRosterWithlist(List<TroopRosterElement> list, TroopRoster roster)
		{
			CharacterObject leaderOfRoster = this.GetLeaderOfRoster(roster);
			for (int i = roster.Count - 1; i >= 0; i--)
			{
				TroopRosterElement elementCopyAtIndex = roster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != leaderOfRoster)
				{
					roster.AddToCountsAtIndex(i, -elementCopyAtIndex.Number, -elementCopyAtIndex.WoundedNumber, 0, true);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Character != leaderOfRoster)
				{
					roster.Add(list[j]);
				}
			}
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x000A5514 File Offset: 0x000A3714
		[Conditional("DEBUG")]
		private void EnsureRosterIsSyncedWithList(TroopRoster roster, List<TroopRosterElement> list)
		{
			if (roster.Count != list.Count)
			{
				Debug.FailedAssert("Roster count is not synced with the list count", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyScreenLogic.cs", "EnsureRosterIsSyncedWithList", 1077);
				return;
			}
			for (int i = 0; i < roster.Count; i++)
			{
				if (roster.GetCharacterAtIndex(i).StringId != list[i].Character.StringId)
				{
					Debug.FailedAssert("Roster is not synced with the list at index: " + i, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyScreenLogic.cs", "EnsureRosterIsSyncedWithList", 1087);
					return;
				}
			}
		}

		// Token: 0x06002701 RID: 9985 RVA: 0x000A55A4 File Offset: 0x000A37A4
		private void SortRosterByType(TroopRoster roster, PartyScreenLogic.TroopSortType sortType)
		{
			PartyScreenLogic.TroopComparer troopComparer = this._defaultComparers[sortType];
			if (!this.IsRosterOrdered(roster, troopComparer))
			{
				List<TroopRosterElement> listFromRoster = this.GetListFromRoster(roster);
				listFromRoster.Sort(this._defaultComparers[sortType]);
				this.SyncRosterWithlist(listFromRoster, roster);
			}
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x000A55EC File Offset: 0x000A37EC
		private bool IsRosterOrdered(TroopRoster roster, PartyScreenLogic.TroopComparer comparer)
		{
			for (int i = 1; i < roster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = roster.GetElementCopyAtIndex(i - 1);
				TroopRosterElement elementCopyAtIndex2 = roster.GetElementCopyAtIndex(i);
				if (comparer.Compare(elementCopyAtIndex, elementCopyAtIndex2) >= 1)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002703 RID: 9987 RVA: 0x000A562C File Offset: 0x000A382C
		public bool IsDoneActive()
		{
			object obj = Hero.MainHero.Gold < -this.CurrentData.PartyGoldChangeAmount && this.CurrentData.PartyGoldChangeAmount < 0;
			PartyPresentationDoneButtonConditionDelegate partyPresentationDoneButtonConditionDelegate = this.PartyPresentationDoneButtonConditionDelegate;
			Tuple<bool, TextObject> tuple = ((partyPresentationDoneButtonConditionDelegate != null) ? partyPresentationDoneButtonConditionDelegate(this.MemberRosters[0], this.PrisonerRosters[0], this.MemberRosters[1], this.PrisonerRosters[1], this.LeftPartyMembersSizeLimit, 0) : null);
			bool flag = this.PartyPresentationDoneButtonConditionDelegate == null || (tuple != null && tuple.Item1);
			this.DoneReasonString = null;
			object obj2 = obj;
			if (obj2 != null)
			{
				this.DoneReasonString = GameTexts.FindText("str_inventory_popup_player_not_enough_gold", null).ToString();
			}
			else
			{
				string text;
				if (tuple == null)
				{
					text = null;
				}
				else
				{
					TextObject item = tuple.Item2;
					text = ((item != null) ? item.ToString() : null);
				}
				this.DoneReasonString = text ?? string.Empty;
			}
			return obj2 == 0 && flag;
		}

		// Token: 0x06002704 RID: 9988 RVA: 0x000A5702 File Offset: 0x000A3902
		public bool IsCancelActive()
		{
			return this.PartyPresentationCancelButtonActivateDelegate == null || this.PartyPresentationCancelButtonActivateDelegate();
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x000A571C File Offset: 0x000A391C
		public bool DoneLogic(bool isForced)
		{
			if (Hero.MainHero.Gold < -this.CurrentData.PartyGoldChangeAmount && this.CurrentData.PartyGoldChangeAmount < 0)
			{
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_inventory_popup_player_not_enough_gold", null), 0, null, "");
				return false;
			}
			FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(4);
			FlattenedTroopRoster flattenedTroopRoster2 = new FlattenedTroopRoster(4);
			foreach (Tuple<CharacterObject, int> tuple in this.CurrentData.TransferredPrisonersHistory)
			{
				int num = MathF.Abs(tuple.Item2);
				if (tuple.Item2 < 0)
				{
					flattenedTroopRoster.Add(tuple.Item1, num, 0);
				}
				else if (tuple.Item2 > 0)
				{
					flattenedTroopRoster2.Add(tuple.Item1, num, 0);
				}
			}
			if (Settlement.CurrentSettlement != null && !flattenedTroopRoster2.IsEmpty<FlattenedTroopRosterElement>())
			{
				CampaignEventDispatcher.Instance.OnPrisonersChangeInSettlement(Settlement.CurrentSettlement, flattenedTroopRoster2, null, true);
			}
			bool flag = this.PartyPresentationDoneButtonDelegate(this.MemberRosters[0], this.PrisonerRosters[0], this.MemberRosters[1], this.PrisonerRosters[1], flattenedTroopRoster2, flattenedTroopRoster, isForced, this.LeftOwnerParty, this.RightOwnerParty);
			if (flag)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.CurrentData.PartyGoldChangeAmount, false);
				if (this.CurrentData.PartyInfluenceChangeAmount.Item2 != 0)
				{
					GainKingdomInfluenceAction.ApplyForLeavingTroopToGarrison(Hero.MainHero, (float)this.CurrentData.PartyInfluenceChangeAmount.Item2);
				}
				this._initialData.CopyFromScreenData(this.CurrentData);
				this.FireCampaignRelatedEvents();
			}
			return flag;
		}

		// Token: 0x06002706 RID: 9990 RVA: 0x000A58B4 File Offset: 0x000A3AB4
		public void OnPartyScreenClosed(bool fromCancel)
		{
			if (fromCancel)
			{
				PartyPresentationCancelButtonDelegate partyPresentationCancelButtonDelegate = this.PartyPresentationCancelButtonDelegate;
				if (partyPresentationCancelButtonDelegate != null)
				{
					partyPresentationCancelButtonDelegate();
				}
			}
			PartyScreenClosedDelegate partyScreenClosedEvent = this.PartyScreenClosedEvent;
			if (partyScreenClosedEvent == null)
			{
				return;
			}
			partyScreenClosedEvent(this.LeftOwnerParty, this.MemberRosters[0], this.PrisonerRosters[0], this.RightOwnerParty, this.MemberRosters[1], this.PrisonerRosters[1], fromCancel);
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x000A5914 File Offset: 0x000A3B14
		private void UpdateComparersAscendingOrder(bool isAscending)
		{
			foreach (KeyValuePair<PartyScreenLogic.TroopSortType, PartyScreenLogic.TroopComparer> keyValuePair in this._defaultComparers)
			{
				keyValuePair.Value.SetIsAscending(isAscending);
			}
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x000A5970 File Offset: 0x000A3B70
		private void FireCampaignRelatedEvents()
		{
			foreach (Tuple<CharacterObject, CharacterObject, int> tuple in this.CurrentData.UpgradedTroopsHistory)
			{
				CampaignEventDispatcher.Instance.OnPlayerUpgradedTroops(tuple.Item1, tuple.Item2, tuple.Item3);
			}
			FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(4);
			foreach (Tuple<CharacterObject, int> tuple2 in this.CurrentData.RecruitedPrisonersHistory)
			{
				flattenedTroopRoster.Add(tuple2.Item1, tuple2.Item2, 0);
			}
			if (!flattenedTroopRoster.IsEmpty<FlattenedTroopRosterElement>())
			{
				CampaignEventDispatcher.Instance.OnMainPartyPrisonerRecruited(flattenedTroopRoster);
			}
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x000A5A50 File Offset: 0x000A3C50
		public bool IsTroopTransferable(PartyScreenLogic.TroopType troopType, CharacterObject character, int side)
		{
			return this.IsTroopRosterTransferable(troopType) && !character.IsNotTransferableInPartyScreen && character != CharacterObject.PlayerCharacter && (this.IsTroopTransferableDelegate == null || this.IsTroopTransferableDelegate(character, troopType, (PartyScreenLogic.PartyRosterSide)side, this.LeftOwnerParty));
		}

		// Token: 0x0600270A RID: 9994 RVA: 0x000A5A8C File Offset: 0x000A3C8C
		public bool IsTroopRosterTransferable(PartyScreenLogic.TroopType troopType)
		{
			if (troopType == PartyScreenLogic.TroopType.Prisoner)
			{
				return this.PrisonerTransferState == PartyScreenLogic.TransferState.Transferable || this.PrisonerTransferState == PartyScreenLogic.TransferState.TransferableWithTrade;
			}
			return troopType == PartyScreenLogic.TroopType.Member && (this.MemberTransferState == PartyScreenLogic.TransferState.Transferable || this.MemberTransferState == PartyScreenLogic.TransferState.TransferableWithTrade);
		}

		// Token: 0x0600270B RID: 9995 RVA: 0x000A5AC1 File Offset: 0x000A3CC1
		public bool IsPrisonerRecruitable(PartyScreenLogic.TroopType troopType, CharacterObject character, PartyScreenLogic.PartyRosterSide side)
		{
			return side == PartyScreenLogic.PartyRosterSide.Right && troopType == PartyScreenLogic.TroopType.Prisoner && !character.IsHero && this.CurrentData.RightRecruitableData.ContainsKey(character) && this.CurrentData.RightRecruitableData[character] > 0;
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x000A5B00 File Offset: 0x000A3D00
		public string GetRecruitableReasonText(CharacterObject character, bool isRecruitable, int troopCount, string fiveStackShortcutKeyText, string entireStackShortcutKeyText)
		{
			GameTexts.SetVariable("newline", "\n");
			if (isRecruitable && !string.IsNullOrEmpty(entireStackShortcutKeyText))
			{
				GameTexts.SetVariable("KEY_NAME", entireStackShortcutKeyText);
				string text = GameTexts.FindText("str_entire_stack_shortcut_recruit_units", null).ToString();
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", "");
				if (troopCount >= 5 && !string.IsNullOrEmpty(fiveStackShortcutKeyText))
				{
					GameTexts.SetVariable("KEY_NAME", fiveStackShortcutKeyText);
					string text2 = GameTexts.FindText("str_five_stack_shortcut_recruit_units", null).ToString();
					GameTexts.SetVariable("STR2", text2);
				}
				string text3 = GameTexts.FindText("str_string_newline_string", null).ToString();
				GameTexts.SetVariable("STR2", text3);
			}
			if (this.RightOwnerParty.PartySizeLimit <= this.MemberRosters[1].TotalManCount)
			{
				GameTexts.SetVariable("STR2", "");
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_recruit_party_size_limit", null));
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			if (character.IsHero)
			{
				GameTexts.SetVariable("STR2", "");
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_cannot_recruit_hero", null));
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_recruit_prisoner", null));
			return GameTexts.FindText("str_string_newline_string", null).ToString();
		}

		// Token: 0x0600270D RID: 9997 RVA: 0x000A5C68 File Offset: 0x000A3E68
		public bool IsExecutable(PartyScreenLogic.TroopType troopType, CharacterObject character, PartyScreenLogic.PartyRosterSide side)
		{
			return troopType == PartyScreenLogic.TroopType.Prisoner && side == PartyScreenLogic.PartyRosterSide.Right && character.IsHero && character.HeroObject.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && (PlayerEncounter.Current == null || PlayerEncounter.Current.EncounterState == PlayerEncounterState.Begin) && FaceGen.GetMaturityTypeWithAge(character.Age) > BodyMeshMaturityType.Tween;
		}

		// Token: 0x0600270E RID: 9998 RVA: 0x000A5CCA File Offset: 0x000A3ECA
		public string GetExecutableReasonText(CharacterObject character, bool isExecutable)
		{
			if (isExecutable)
			{
				return GameTexts.FindText("str_execute_prisoner", null).ToString();
			}
			if (!character.IsHero)
			{
				return GameTexts.FindText("str_cannot_execute_nonhero", null).ToString();
			}
			return GameTexts.FindText("str_cannot_execute_hero", null).ToString();
		}

		// Token: 0x0600270F RID: 9999 RVA: 0x000A5D09 File Offset: 0x000A3F09
		public int GetCurrentQuestCurrentCount()
		{
			return this.MemberRosters[0].Sum((TroopRosterElement item) => item.Number);
		}

		// Token: 0x06002710 RID: 10000 RVA: 0x000A5D37 File Offset: 0x000A3F37
		public int GetCurrentQuestRequiredCount()
		{
			return this.LeftPartyMembersSizeLimit;
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x000A5D3F File Offset: 0x000A3F3F
		private static bool DefaultDoneHandler(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty = null, PartyBase rightParty = null)
		{
			return true;
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x000A5D44 File Offset: 0x000A3F44
		private void AddUpgradeToHistory(CharacterObject fromTroop, CharacterObject toTroop, int num)
		{
			Tuple<CharacterObject, CharacterObject, int> tuple = this.CurrentData.UpgradedTroopsHistory.Find((Tuple<CharacterObject, CharacterObject, int> t) => t.Item1 == fromTroop && t.Item2 == toTroop);
			if (tuple != null)
			{
				int item = tuple.Item3;
				this.CurrentData.UpgradedTroopsHistory.Remove(tuple);
				this.CurrentData.UpgradedTroopsHistory.Add(new Tuple<CharacterObject, CharacterObject, int>(fromTroop, toTroop, num + item));
				return;
			}
			this.CurrentData.UpgradedTroopsHistory.Add(new Tuple<CharacterObject, CharacterObject, int>(fromTroop, toTroop, num));
		}

		// Token: 0x06002713 RID: 10003 RVA: 0x000A5DE8 File Offset: 0x000A3FE8
		private void AddUsedHorsesToHistory(List<ValueTuple<EquipmentElement, int>> usedHorses)
		{
			if (usedHorses != null)
			{
				using (List<ValueTuple<EquipmentElement, int>>.Enumerator enumerator = usedHorses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ValueTuple<EquipmentElement, int> usedHorse = enumerator.Current;
						Tuple<EquipmentElement, int> tuple = this.CurrentData.UsedUpgradeHorsesHistory.Find((Tuple<EquipmentElement, int> t) => t.Equals(usedHorse.Item1));
						if (tuple != null)
						{
							int item = tuple.Item2;
							this.CurrentData.UsedUpgradeHorsesHistory.Remove(tuple);
							this.CurrentData.UsedUpgradeHorsesHistory.Add(new Tuple<EquipmentElement, int>(usedHorse.Item1, item + usedHorse.Item2));
						}
						else
						{
							this.CurrentData.UsedUpgradeHorsesHistory.Add(new Tuple<EquipmentElement, int>(usedHorse.Item1, usedHorse.Item2));
						}
					}
				}
				PartyScreenData currentData = this.CurrentData;
				this.SetHorseChangeAmount(currentData.PartyHorseChangeAmount += usedHorses.Sum((ValueTuple<EquipmentElement, int> t) => t.Item2));
			}
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x000A5F1C File Offset: 0x000A411C
		private void UpdatePrisonerTransferHistory(CharacterObject troop, int amount)
		{
			Tuple<CharacterObject, int> tuple = this.CurrentData.TransferredPrisonersHistory.Find((Tuple<CharacterObject, int> t) => t.Item1 == troop);
			if (tuple != null)
			{
				int item = tuple.Item2;
				this.CurrentData.TransferredPrisonersHistory.Remove(tuple);
				this.CurrentData.TransferredPrisonersHistory.Add(new Tuple<CharacterObject, int>(troop, amount + item));
				return;
			}
			this.CurrentData.TransferredPrisonersHistory.Add(new Tuple<CharacterObject, int>(troop, amount));
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x000A5FAC File Offset: 0x000A41AC
		private void AddRecruitToHistory(CharacterObject troop, int amount)
		{
			Tuple<CharacterObject, int> tuple = this.CurrentData.RecruitedPrisonersHistory.Find((Tuple<CharacterObject, int> t) => t.Item1 == troop);
			if (tuple != null)
			{
				int item = tuple.Item2;
				this.CurrentData.RecruitedPrisonersHistory.Remove(tuple);
				this.CurrentData.RecruitedPrisonersHistory.Add(new Tuple<CharacterObject, int>(troop, amount + item));
			}
			else
			{
				this.CurrentData.RecruitedPrisonersHistory.Add(new Tuple<CharacterObject, int>(troop, amount));
			}
			int prisonerRecruitmentMoraleEffect = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetPrisonerRecruitmentMoraleEffect(this.RightOwnerParty, troop, amount);
			this.SetMoraleChangeAmount(this.CurrentData.PartyMoraleChangeAmount + prisonerRecruitmentMoraleEffect);
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x000A6070 File Offset: 0x000A4270
		private string GetItemLockStringID(EquipmentElement equipmentElement)
		{
			return equipmentElement.Item.StringId + ((equipmentElement.ItemModifier != null) ? equipmentElement.ItemModifier.StringId : "");
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x000A60A0 File Offset: 0x000A42A0
		private List<ValueTuple<EquipmentElement, int>> RemoveItemFromItemRoster(ItemCategory itemCategory, int numOfItemsLeftToRemove = 1)
		{
			List<ValueTuple<EquipmentElement, int>> list = new List<ValueTuple<EquipmentElement, int>>();
			IEnumerable<string> lockedItems = Campaign.Current.GetCampaignBehavior<IViewDataTracker>().GetInventoryLocks();
			foreach (ItemRosterElement itemRosterElement in from x in this.RightOwnerParty.ItemRoster.Where(delegate(ItemRosterElement x)
				{
					ItemObject item = x.EquipmentElement.Item;
					return ((item != null) ? item.ItemCategory : null) == itemCategory;
				})
				orderby x.EquipmentElement.Item.Value
				orderby lockedItems.Contains(this.GetItemLockStringID(x.EquipmentElement))
				select x)
			{
				int num = MathF.Min(numOfItemsLeftToRemove, itemRosterElement.Amount);
				this.RightOwnerParty.ItemRoster.AddToCounts(itemRosterElement.EquipmentElement, -num);
				numOfItemsLeftToRemove -= num;
				list.Add(new ValueTuple<EquipmentElement, int>(itemRosterElement.EquipmentElement, num));
				if (numOfItemsLeftToRemove <= 0)
				{
					break;
				}
			}
			if (numOfItemsLeftToRemove > 0)
			{
				Debug.FailedAssert("Couldn't find enough upgrade req items in the inventory.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Party\\PartyScreenLogic.cs", "RemoveItemFromItemRoster", 1500);
			}
			return list;
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x000A61C8 File Offset: 0x000A43C8
		public void Reset(bool fromCancel)
		{
			this.ResetLogic(fromCancel);
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x000A61D1 File Offset: 0x000A43D1
		private void ResetLogic(bool fromCancel)
		{
			if (this.CurrentData != this._initialData)
			{
				this.CurrentData.ResetUsing(this._initialData);
				PartyScreenLogic.AfterResetDelegate afterReset = this.AfterReset;
				if (afterReset == null)
				{
					return;
				}
				afterReset(this, fromCancel);
			}
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x000A6209 File Offset: 0x000A4409
		public void SavePartyScreenData()
		{
			this._savedData = new PartyScreenData();
			this._savedData.InitializeCopyFrom(this.CurrentData.RightParty, this.CurrentData.LeftParty);
			this._savedData.CopyFromScreenData(this.CurrentData);
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x000A6248 File Offset: 0x000A4448
		public void ResetToLastSavedPartyScreenData(bool fromCancel)
		{
			if (this.CurrentData != this._savedData)
			{
				this.CurrentData.ResetUsing(this._savedData);
				PartyScreenLogic.AfterResetDelegate afterReset = this.AfterReset;
				if (afterReset == null)
				{
					return;
				}
				afterReset(this, fromCancel);
			}
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000A6280 File Offset: 0x000A4480
		public void RemoveZeroCounts()
		{
			for (int i = 0; i < this.MemberRosters.Length; i++)
			{
				this.MemberRosters[i].RemoveZeroCounts();
			}
			for (int j = 0; j < this.PrisonerRosters.Length; j++)
			{
				this.PrisonerRosters[j].RemoveZeroCounts();
			}
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000A62CD File Offset: 0x000A44CD
		public int GetTroopRecruitableAmount(CharacterObject troop)
		{
			if (!this.CurrentData.RightRecruitableData.ContainsKey(troop))
			{
				return 0;
			}
			return this.CurrentData.RightRecruitableData[troop];
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000A62F5 File Offset: 0x000A44F5
		public TroopRoster GetRoster(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType troopType)
		{
			if (troopType == PartyScreenLogic.TroopType.Member)
			{
				return this.MemberRosters[(int)side];
			}
			if (troopType == PartyScreenLogic.TroopType.Prisoner)
			{
				return this.PrisonerRosters[(int)side];
			}
			return null;
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000A6312 File Offset: 0x000A4512
		internal void OnDoneEvent(List<TroopTradeDifference> freshlySellList)
		{
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000A6314 File Offset: 0x000A4514
		public bool IsThereAnyChanges()
		{
			return this._initialData.GetTroopTradeDifferencesFromTo(this.CurrentData).Count != 0;
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000A6330 File Offset: 0x000A4530
		public bool HaveRightSideGainedTroops()
		{
			foreach (TroopTradeDifference troopTradeDifference in this._initialData.GetTroopTradeDifferencesFromTo(this.CurrentData))
			{
				if (!troopTradeDifference.IsPrisoner && troopTradeDifference.FromCount < troopTradeDifference.ToCount)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000A63A8 File Offset: 0x000A45A8
		public PartyScreenLogic.TroopComparer GetComparer(PartyScreenLogic.TroopSortType sortType)
		{
			return this._defaultComparers[sortType];
		}

		// Token: 0x04000BBA RID: 3002
		public PartyPresentationDoneButtonDelegate PartyPresentationDoneButtonDelegate;

		// Token: 0x04000BBB RID: 3003
		public PartyPresentationDoneButtonConditionDelegate PartyPresentationDoneButtonConditionDelegate;

		// Token: 0x04000BBC RID: 3004
		public PartyPresentationCancelButtonActivateDelegate PartyPresentationCancelButtonActivateDelegate;

		// Token: 0x04000BBD RID: 3005
		public PartyPresentationCancelButtonDelegate PartyPresentationCancelButtonDelegate;

		// Token: 0x04000BBE RID: 3006
		public PartyScreenLogic.PresentationUpdate UpdateDelegate;

		// Token: 0x04000BBF RID: 3007
		public IsTroopTransferableDelegate IsTroopTransferableDelegate;

		// Token: 0x04000BC7 RID: 3015
		private PartyScreenLogic.TroopSortType _activeOtherPartySortType;

		// Token: 0x04000BC8 RID: 3016
		private PartyScreenLogic.TroopSortType _activeMainPartySortType;

		// Token: 0x04000BC9 RID: 3017
		private bool _isOtherPartySortAscending;

		// Token: 0x04000BCA RID: 3018
		private bool _isMainPartySortAscending;

		// Token: 0x04000BDF RID: 3039
		public TroopRoster[] MemberRosters;

		// Token: 0x04000BE0 RID: 3040
		public TroopRoster[] PrisonerRosters;

		// Token: 0x04000BE1 RID: 3041
		public bool IsConsumablesChanges;

		// Token: 0x04000BE2 RID: 3042
		private readonly Dictionary<PartyScreenLogic.TroopSortType, PartyScreenLogic.TroopComparer> _defaultComparers;

		// Token: 0x04000BE3 RID: 3043
		private readonly PartyScreenData _initialData;

		// Token: 0x04000BE4 RID: 3044
		private PartyScreenData _savedData;

		// Token: 0x04000BE5 RID: 3045
		private Game _game;

		// Token: 0x020005B5 RID: 1461
		public enum TroopSortType
		{
			// Token: 0x040017C5 RID: 6085
			Invalid = -1,
			// Token: 0x040017C6 RID: 6086
			Custom,
			// Token: 0x040017C7 RID: 6087
			Type,
			// Token: 0x040017C8 RID: 6088
			Name,
			// Token: 0x040017C9 RID: 6089
			Count,
			// Token: 0x040017CA RID: 6090
			Tier
		}

		// Token: 0x020005B6 RID: 1462
		public enum PartyRosterSide : byte
		{
			// Token: 0x040017CC RID: 6092
			None = 99,
			// Token: 0x040017CD RID: 6093
			Right = 1,
			// Token: 0x040017CE RID: 6094
			Left = 0
		}

		// Token: 0x020005B7 RID: 1463
		[Flags]
		public enum TroopType
		{
			// Token: 0x040017D0 RID: 6096
			Member = 1,
			// Token: 0x040017D1 RID: 6097
			Prisoner = 2,
			// Token: 0x040017D2 RID: 6098
			None = 3
		}

		// Token: 0x020005B8 RID: 1464
		public enum PartyCommandCode
		{
			// Token: 0x040017D4 RID: 6100
			TransferTroop,
			// Token: 0x040017D5 RID: 6101
			UpgradeTroop,
			// Token: 0x040017D6 RID: 6102
			TransferPartyLeaderTroop,
			// Token: 0x040017D7 RID: 6103
			TransferTroopToLeaderSlot,
			// Token: 0x040017D8 RID: 6104
			ShiftTroop,
			// Token: 0x040017D9 RID: 6105
			RecruitTroop,
			// Token: 0x040017DA RID: 6106
			ExecuteTroop,
			// Token: 0x040017DB RID: 6107
			TransferAllTroops,
			// Token: 0x040017DC RID: 6108
			SortTroops
		}

		// Token: 0x020005B9 RID: 1465
		public enum TransferState
		{
			// Token: 0x040017DE RID: 6110
			NotTransferable,
			// Token: 0x040017DF RID: 6111
			Transferable,
			// Token: 0x040017E0 RID: 6112
			TransferableWithTrade
		}

		// Token: 0x020005BA RID: 1466
		// (Invoke) Token: 0x06004561 RID: 17761
		public delegate void PresentationUpdate(PartyScreenLogic.PartyCommand command);

		// Token: 0x020005BB RID: 1467
		// (Invoke) Token: 0x06004565 RID: 17765
		public delegate void PartyGoldDelegate();

		// Token: 0x020005BC RID: 1468
		// (Invoke) Token: 0x06004569 RID: 17769
		public delegate void PartyMoraleDelegate();

		// Token: 0x020005BD RID: 1469
		// (Invoke) Token: 0x0600456D RID: 17773
		public delegate void PartyInfluenceDelegate();

		// Token: 0x020005BE RID: 1470
		// (Invoke) Token: 0x06004571 RID: 17777
		public delegate void PartyHorseDelegate();

		// Token: 0x020005BF RID: 1471
		// (Invoke) Token: 0x06004575 RID: 17781
		public delegate void AfterResetDelegate(PartyScreenLogic partyScreenLogic, bool fromCancel);

		// Token: 0x020005C0 RID: 1472
		public class PartyCommand : ISerializableObject
		{
			// Token: 0x17000DE9 RID: 3561
			// (get) Token: 0x06004578 RID: 17784 RVA: 0x0013AC17 File Offset: 0x00138E17
			// (set) Token: 0x06004579 RID: 17785 RVA: 0x0013AC1F File Offset: 0x00138E1F
			public PartyScreenLogic.PartyCommandCode Code { get; private set; }

			// Token: 0x17000DEA RID: 3562
			// (get) Token: 0x0600457A RID: 17786 RVA: 0x0013AC28 File Offset: 0x00138E28
			// (set) Token: 0x0600457B RID: 17787 RVA: 0x0013AC30 File Offset: 0x00138E30
			public PartyScreenLogic.PartyRosterSide RosterSide { get; private set; }

			// Token: 0x17000DEB RID: 3563
			// (get) Token: 0x0600457C RID: 17788 RVA: 0x0013AC39 File Offset: 0x00138E39
			// (set) Token: 0x0600457D RID: 17789 RVA: 0x0013AC41 File Offset: 0x00138E41
			public CharacterObject Character { get; private set; }

			// Token: 0x17000DEC RID: 3564
			// (get) Token: 0x0600457E RID: 17790 RVA: 0x0013AC4A File Offset: 0x00138E4A
			// (set) Token: 0x0600457F RID: 17791 RVA: 0x0013AC52 File Offset: 0x00138E52
			public int TotalNumber { get; private set; }

			// Token: 0x17000DED RID: 3565
			// (get) Token: 0x06004580 RID: 17792 RVA: 0x0013AC5B File Offset: 0x00138E5B
			// (set) Token: 0x06004581 RID: 17793 RVA: 0x0013AC63 File Offset: 0x00138E63
			public int WoundedNumber { get; private set; }

			// Token: 0x17000DEE RID: 3566
			// (get) Token: 0x06004582 RID: 17794 RVA: 0x0013AC6C File Offset: 0x00138E6C
			// (set) Token: 0x06004583 RID: 17795 RVA: 0x0013AC74 File Offset: 0x00138E74
			public int Index { get; private set; }

			// Token: 0x17000DEF RID: 3567
			// (get) Token: 0x06004584 RID: 17796 RVA: 0x0013AC7D File Offset: 0x00138E7D
			// (set) Token: 0x06004585 RID: 17797 RVA: 0x0013AC85 File Offset: 0x00138E85
			public int UpgradeTarget { get; private set; }

			// Token: 0x17000DF0 RID: 3568
			// (get) Token: 0x06004586 RID: 17798 RVA: 0x0013AC8E File Offset: 0x00138E8E
			// (set) Token: 0x06004587 RID: 17799 RVA: 0x0013AC96 File Offset: 0x00138E96
			public PartyScreenLogic.TroopType Type { get; private set; }

			// Token: 0x17000DF1 RID: 3569
			// (get) Token: 0x06004588 RID: 17800 RVA: 0x0013AC9F File Offset: 0x00138E9F
			// (set) Token: 0x06004589 RID: 17801 RVA: 0x0013ACA7 File Offset: 0x00138EA7
			public PartyScreenLogic.TroopSortType SortType { get; private set; }

			// Token: 0x17000DF2 RID: 3570
			// (get) Token: 0x0600458A RID: 17802 RVA: 0x0013ACB0 File Offset: 0x00138EB0
			// (set) Token: 0x0600458B RID: 17803 RVA: 0x0013ACB8 File Offset: 0x00138EB8
			public bool IsSortAscending { get; private set; }

			// Token: 0x0600458D RID: 17805 RVA: 0x0013ACC9 File Offset: 0x00138EC9
			public void FillForTransferTroop(PartyScreenLogic.PartyRosterSide fromSide, PartyScreenLogic.TroopType type, CharacterObject character, int totalNumber, int woundedNumber, int targetIndex)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.TransferTroop;
				this.RosterSide = fromSide;
				this.TotalNumber = totalNumber;
				this.WoundedNumber = woundedNumber;
				this.Character = character;
				this.Type = type;
				this.Index = targetIndex;
			}

			// Token: 0x0600458E RID: 17806 RVA: 0x0013ACFF File Offset: 0x00138EFF
			public void FillForShiftTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character, int targetIndex)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.ShiftTroop;
				this.RosterSide = side;
				this.Character = character;
				this.Type = type;
				this.Index = targetIndex;
			}

			// Token: 0x0600458F RID: 17807 RVA: 0x0013AD25 File Offset: 0x00138F25
			public void FillForTransferTroopToLeaderSlot(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character, int totalNumber, int woundedNumber, int targetIndex)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.TransferTroopToLeaderSlot;
				this.RosterSide = side;
				this.TotalNumber = totalNumber;
				this.WoundedNumber = woundedNumber;
				this.Character = character;
				this.Type = type;
				this.Index = targetIndex;
			}

			// Token: 0x06004590 RID: 17808 RVA: 0x0013AD5B File Offset: 0x00138F5B
			public void FillForTransferPartyLeaderTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character, int totalNumber)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.TransferPartyLeaderTroop;
				this.RosterSide = side;
				this.TotalNumber = totalNumber;
				this.Character = character;
				this.Type = type;
			}

			// Token: 0x06004591 RID: 17809 RVA: 0x0013AD81 File Offset: 0x00138F81
			public void FillForUpgradeTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character, int number, int upgradeTargetType, int index)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.UpgradeTroop;
				this.RosterSide = side;
				this.TotalNumber = number;
				this.Character = character;
				this.UpgradeTarget = upgradeTargetType;
				this.Type = type;
				this.Index = index;
			}

			// Token: 0x06004592 RID: 17810 RVA: 0x0013ADB7 File Offset: 0x00138FB7
			public void FillForRecruitTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character, int number, int index)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.RecruitTroop;
				this.RosterSide = side;
				this.Character = character;
				this.Type = type;
				this.TotalNumber = number;
				this.Index = index;
			}

			// Token: 0x06004593 RID: 17811 RVA: 0x0013ADE5 File Offset: 0x00138FE5
			public void FillForExecuteTroop(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type, CharacterObject character)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.ExecuteTroop;
				this.RosterSide = side;
				this.Character = character;
				this.Type = type;
			}

			// Token: 0x06004594 RID: 17812 RVA: 0x0013AE03 File Offset: 0x00139003
			public void FillForTransferAllTroops(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopType type)
			{
				this.Code = PartyScreenLogic.PartyCommandCode.TransferAllTroops;
				this.RosterSide = side;
				this.Type = type;
			}

			// Token: 0x06004595 RID: 17813 RVA: 0x0013AE1A File Offset: 0x0013901A
			public void FillForSortTroops(PartyScreenLogic.PartyRosterSide side, PartyScreenLogic.TroopSortType sortType, bool isAscending)
			{
				this.RosterSide = side;
				this.Code = PartyScreenLogic.PartyCommandCode.SortTroops;
				this.SortType = sortType;
				this.IsSortAscending = isAscending;
			}

			// Token: 0x06004596 RID: 17814 RVA: 0x0013AE38 File Offset: 0x00139038
			void ISerializableObject.SerializeTo(IWriter writer)
			{
				writer.WriteByte((byte)this.Code);
				writer.WriteByte((byte)this.RosterSide);
				writer.WriteUInt(this.Character.Id.InternalValue);
				writer.WriteInt(this.TotalNumber);
				writer.WriteInt(this.WoundedNumber);
				writer.WriteInt(this.UpgradeTarget);
				writer.WriteByte((byte)this.Type);
			}

			// Token: 0x06004597 RID: 17815 RVA: 0x0013AEA8 File Offset: 0x001390A8
			void ISerializableObject.DeserializeFrom(IReader reader)
			{
				this.Code = (PartyScreenLogic.PartyCommandCode)reader.ReadByte();
				this.RosterSide = (PartyScreenLogic.PartyRosterSide)reader.ReadByte();
				MBGUID mbguid = new MBGUID(reader.ReadUInt());
				this.Character = (CharacterObject)MBObjectManager.Instance.GetObject(mbguid);
				this.TotalNumber = reader.ReadInt();
				this.WoundedNumber = reader.ReadInt();
				this.UpgradeTarget = reader.ReadInt();
				this.Type = (PartyScreenLogic.TroopType)reader.ReadByte();
			}
		}

		// Token: 0x020005C1 RID: 1473
		public abstract class TroopComparer : IComparer<TroopRosterElement>
		{
			// Token: 0x06004598 RID: 17816 RVA: 0x0013AF20 File Offset: 0x00139120
			public void SetIsAscending(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06004599 RID: 17817 RVA: 0x0013AF29 File Offset: 0x00139129
			private int GetHeroComparisonResult(TroopRosterElement x, TroopRosterElement y)
			{
				if (x.Character.HeroObject != null)
				{
					if (x.Character.HeroObject == Hero.MainHero)
					{
						return -2;
					}
					if (y.Character.HeroObject == null)
					{
						return -1;
					}
				}
				return 0;
			}

			// Token: 0x0600459A RID: 17818 RVA: 0x0013AF60 File Offset: 0x00139160
			public int Compare(TroopRosterElement x, TroopRosterElement y)
			{
				int num = (this._isAscending ? 1 : (-1));
				int num2 = this.GetHeroComparisonResult(x, y);
				if (num2 != 0)
				{
					return num2;
				}
				num2 = this.GetHeroComparisonResult(y, x);
				if (num2 != 0)
				{
					return num2 * -1;
				}
				return this.CompareTroops(x, y) * num;
			}

			// Token: 0x0600459B RID: 17819
			protected abstract int CompareTroops(TroopRosterElement x, TroopRosterElement y);

			// Token: 0x040017EB RID: 6123
			private bool _isAscending;
		}

		// Token: 0x020005C2 RID: 1474
		private class TroopDefaultComparer : PartyScreenLogic.TroopComparer
		{
			// Token: 0x0600459D RID: 17821 RVA: 0x0013AFAA File Offset: 0x001391AA
			protected override int CompareTroops(TroopRosterElement x, TroopRosterElement y)
			{
				return 0;
			}
		}

		// Token: 0x020005C3 RID: 1475
		private class TroopTypeComparer : PartyScreenLogic.TroopComparer
		{
			// Token: 0x0600459F RID: 17823 RVA: 0x0013AFB8 File Offset: 0x001391B8
			protected override int CompareTroops(TroopRosterElement x, TroopRosterElement y)
			{
				int defaultFormationClass = (int)x.Character.DefaultFormationClass;
				int defaultFormationClass2 = (int)y.Character.DefaultFormationClass;
				return defaultFormationClass.CompareTo(defaultFormationClass2);
			}
		}

		// Token: 0x020005C4 RID: 1476
		private class TroopNameComparer : PartyScreenLogic.TroopComparer
		{
			// Token: 0x060045A1 RID: 17825 RVA: 0x0013AFED File Offset: 0x001391ED
			protected override int CompareTroops(TroopRosterElement x, TroopRosterElement y)
			{
				return x.Character.Name.ToString().CompareTo(y.Character.Name.ToString());
			}
		}

		// Token: 0x020005C5 RID: 1477
		private class TroopCountComparer : PartyScreenLogic.TroopComparer
		{
			// Token: 0x060045A3 RID: 17827 RVA: 0x0013B01C File Offset: 0x0013921C
			protected override int CompareTroops(TroopRosterElement x, TroopRosterElement y)
			{
				return x.Number.CompareTo(y.Number);
			}
		}

		// Token: 0x020005C6 RID: 1478
		private class TroopTierComparer : PartyScreenLogic.TroopComparer
		{
			// Token: 0x060045A5 RID: 17829 RVA: 0x0013B048 File Offset: 0x00139248
			protected override int CompareTroops(TroopRosterElement x, TroopRosterElement y)
			{
				return x.Character.Tier.CompareTo(y.Character.Tier);
			}
		}
	}
}

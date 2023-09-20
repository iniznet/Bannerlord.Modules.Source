using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class BattleSimulation : IBattleObserver
	{
		public bool IsSimulationPaused { get; private set; }

		public bool IsSimulationFinished { get; private set; }

		private bool IsPlayerJoinedBattle
		{
			get
			{
				return PlayerEncounter.Current.IsJoinedBattle;
			}
		}

		public MapEvent MapEvent
		{
			get
			{
				return this._mapEvent;
			}
		}

		public IBattleObserver BattleObserver
		{
			get
			{
				return this._battleObserver;
			}
			set
			{
				this._battleObserver = value;
			}
		}

		public List<List<BattleResultPartyData>> Teams { get; private set; }

		public BattleSimulation(FlattenedTroopRoster selectedTroopsForPlayerSide, FlattenedTroopRoster selectedTroopsForOtherSide)
		{
			this.IsSimulationPaused = true;
			float applicationTime = Game.Current.ApplicationTime;
			this._timer = new Timer(applicationTime, 1f, true);
			this._mapEvent = PlayerEncounter.Battle ?? PlayerEncounter.StartBattle();
			this._mapEvent.IsPlayerSimulation = true;
			this._mapEvent.BattleObserver = this;
			this.SelectedTroops[(int)this._mapEvent.PlayerSide] = selectedTroopsForPlayerSide;
			this.SelectedTroops[(int)this._mapEvent.GetOtherSide(this._mapEvent.PlayerSide)] = selectedTroopsForOtherSide;
			this._mapEvent.GetNumberOfInvolvedMen();
			if (this._mapEvent.IsSiegeAssault)
			{
				PlayerSiege.StartPlayerSiege(MobileParty.MainParty.Party.Side, true, this._mapEvent.MapEventSettlement);
			}
			List<List<BattleResultPartyData>> list = new List<List<BattleResultPartyData>>
			{
				new List<BattleResultPartyData>(),
				new List<BattleResultPartyData>()
			};
			foreach (PartyBase partyBase in this._mapEvent.InvolvedParties)
			{
				BattleResultPartyData battleResultPartyData = default(BattleResultPartyData);
				bool flag = false;
				foreach (BattleResultPartyData battleResultPartyData2 in list[(int)partyBase.Side])
				{
					if (battleResultPartyData2.Party == partyBase)
					{
						flag = true;
						battleResultPartyData = battleResultPartyData2;
						break;
					}
				}
				if (!flag)
				{
					battleResultPartyData = new BattleResultPartyData(partyBase);
					list[(int)partyBase.Side].Add(battleResultPartyData);
				}
				for (int i = 0; i < partyBase.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = partyBase.MemberRoster.GetElementCopyAtIndex(i);
					if (!battleResultPartyData.Characters.Contains(elementCopyAtIndex.Character))
					{
						battleResultPartyData.Characters.Add(elementCopyAtIndex.Character);
					}
				}
			}
			this.Teams = list;
			this.tempRosterRefs = new List<TroopRoster>();
			foreach (PartyBase partyBase2 in this._mapEvent.InvolvedParties)
			{
				this.tempRosterRefs.Add(partyBase2.MemberRoster);
			}
		}

		public void Play()
		{
			this._simulationState = BattleSimulation.SimulationState.Play;
		}

		public void FastForward()
		{
			this._simulationState = BattleSimulation.SimulationState.FastForward;
		}

		public void Skip()
		{
			this._simulationState = BattleSimulation.SimulationState.Skip;
		}

		public void OnReturn()
		{
			foreach (PartyBase partyBase in this._mapEvent.InvolvedParties)
			{
				partyBase.MemberRoster.RemoveZeroCounts();
			}
			GameMenu.ActivateGameMenu("encounter");
		}

		private void BattleEndLogic()
		{
			if (PlayerEncounter.Battle == null)
			{
				GameMenu.SwitchToMenu("encounter");
				return;
			}
			BattleSideEnum side = PartyBase.MainParty.Side;
			if (PlayerEncounter.Battle.GetMapEventSide(side).NumRemainingSimulationTroops > 0)
			{
				GameMenu.SwitchToMenu("encounter");
				return;
			}
			PlayerEncounter.Finish(true);
		}

		private void TickSimulateBattle()
		{
			BattleSimulation.SimulateBattleFromUi();
		}

		public void Tick(float dt)
		{
			if (this.IsSimulationFinished)
			{
				return;
			}
			if (PlayerEncounter.Current == null)
			{
				Debug.FailedAssert("PlayerEncounter.Current == null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\BattleSimulation.cs", "Tick", 222);
				this.IsSimulationFinished = true;
				return;
			}
			int num = 0;
			if (PlayerEncounter.BattleState == BattleState.None)
			{
				foreach (PartyBase partyBase in this.MapEvent.InvolvedParties)
				{
					if (partyBase.Side == MobileParty.MainParty.Party.Side && partyBase != MobileParty.MainParty.Party)
					{
						num += partyBase.NumberOfHealthyMembers;
					}
				}
			}
			if ((MobileParty.MainParty.MapEvent == this.MapEvent && MobileParty.MainParty.Party.NumberOfHealthyMembers == 1 && !Hero.MainHero.IsWounded && num == 0) || PlayerEncounter.BattleState == BattleState.AttackerVictory || PlayerEncounter.BattleState == BattleState.DefenderVictory)
			{
				this.IsSimulationFinished = true;
				return;
			}
			if (this._simulationState == BattleSimulation.SimulationState.Skip)
			{
				while (PlayerEncounter.BattleState == BattleState.None || PlayerEncounter.BattleState == BattleState.DefenderPullBack)
				{
					this.TickSimulateBattle();
					num = 0;
					if (PlayerEncounter.BattleState == BattleState.None || PlayerEncounter.BattleState == BattleState.DefenderPullBack)
					{
						foreach (PartyBase partyBase2 in this.MapEvent.InvolvedParties)
						{
							if (partyBase2.Side == MobileParty.MainParty.Party.Side && partyBase2 != MobileParty.MainParty.Party)
							{
								num += partyBase2.NumberOfHealthyMembers;
							}
						}
					}
					if (MobileParty.MainParty.MapEvent == this.MapEvent && MobileParty.MainParty.Party.NumberOfHealthyMembers <= 1 && num == 0)
					{
						return;
					}
				}
				return;
			}
			this._numTicks += dt;
			if (this._simulationState == BattleSimulation.SimulationState.FastForward)
			{
				this._numTicks *= 3f;
			}
			while (this._numTicks >= 1f)
			{
				this.TickSimulateBattle();
				this._numTicks -= 1f;
			}
		}

		public static void SimulateBattleFromUi()
		{
			PlayerEncounter.SimulateBattle();
		}

		public void ResetSimulation()
		{
			this.MapEvent.SimulateBattleSetup(PlayerEncounter.CurrentBattleSimulation.SelectedTroops);
		}

		public void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberKilled = 0, int numberWounded = 0, int numberRouted = 0, int killCount = 0, int numberReadyToUpgrade = 0)
		{
			IBattleObserver battleObserver = this.BattleObserver;
			if (battleObserver == null)
			{
				return;
			}
			battleObserver.TroopNumberChanged(side, battleCombatant, character, number, numberKilled, numberWounded, numberRouted, killCount, numberReadyToUpgrade);
		}

		public void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject skill)
		{
			IBattleObserver battleObserver = this.BattleObserver;
			if (battleObserver == null)
			{
				return;
			}
			battleObserver.HeroSkillIncreased(side, battleCombatant, heroCharacter, skill);
		}

		public void BattleResultsReady()
		{
			IBattleObserver battleObserver = this.BattleObserver;
			if (battleObserver == null)
			{
				return;
			}
			battleObserver.BattleResultsReady();
		}

		public void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character)
		{
			IBattleObserver battleObserver = this.BattleObserver;
			if (battleObserver == null)
			{
				return;
			}
			battleObserver.TroopSideChanged(prevSide, newSide, battleCombatant, character);
		}

		private readonly MapEvent _mapEvent;

		public List<TroopRoster> tempRosterRefs;

		private IBattleObserver _battleObserver;

		private Timer _timer;

		public readonly FlattenedTroopRoster[] SelectedTroops = new FlattenedTroopRoster[2];

		private BattleSimulation.SimulationState _simulationState;

		private float _numTicks;

		private enum SimulationState
		{
			Play,
			FastForward,
			Skip
		}
	}
}

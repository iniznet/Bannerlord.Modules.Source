using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000031 RID: 49
	public class OrderOfBattleVM : ViewModel
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060003CC RID: 972 RVA: 0x00010790 File Offset: 0x0000E990
		// (set) Token: 0x060003CD RID: 973 RVA: 0x00010798 File Offset: 0x0000E998
		public bool IsOrderPreconfigured { get; protected set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060003CE RID: 974 RVA: 0x000107A1 File Offset: 0x0000E9A1
		// (set) Token: 0x060003CF RID: 975 RVA: 0x000107A9 File Offset: 0x0000E9A9
		public List<ValueTuple<int, List<int>>> CurrentConfiguration { get; private set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x000107B2 File Offset: 0x0000E9B2
		protected int TotalFormationCount
		{
			get
			{
				return this._mission.PlayerTeam.FormationsIncludingEmpty.Count;
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x000107CC File Offset: 0x0000E9CC
		public OrderOfBattleVM()
		{
			this._allFormations = new List<OrderOfBattleFormationItemVM>();
			this._allHeroes = new List<OrderOfBattleHeroItemVM>();
			this._selectedHeroes = new List<OrderOfBattleHeroItemVM>();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00010890 File Offset: 0x0000EA90
		public override void RefreshValues()
		{
			this.BeginMissionText = new TextObject("{=SYYOSOoa}Ready", null).ToString();
			Mission mission = this._mission;
			if (mission != null && mission.IsSiegeBattle)
			{
				this.AutoDeployText = GameTexts.FindText("str_auto_deploy", null).ToString();
			}
			else
			{
				this.AutoDeployText = new TextObject("{=ADKHovtz}Reset Deployment", null).ToString();
			}
			this.MissingFormationsHint = new HintViewModel(this._missingFormationsHintText, null);
			this.SelectAllHint = new HintViewModel(this._selectAllHintText, null);
			this.ClearSelectionHint = new HintViewModel(this._clearSelectionHintText, null);
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.RefreshValues();
			});
			MBBindingList<OrderOfBattleHeroItemVM> unassignedHeroes = this.UnassignedHeroes;
			if (unassignedHeroes == null)
			{
				return;
			}
			unassignedHeroes.ApplyActionOnAllItems(delegate(OrderOfBattleHeroItemVM c)
			{
				c.RefreshValues();
			});
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00010984 File Offset: 0x0000EB84
		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.FinalizeFormationCallbacks();
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000109B0 File Offset: 0x0000EBB0
		private void InitializeFormationCallbacks()
		{
			OrderOfBattleFormationItemVM.OnClassSelectionToggled = new Action<OrderOfBattleFormationItemVM>(this.OnClassSelectionToggled);
			OrderOfBattleFormationItemVM.OnFilterSelectionToggled = new Action<OrderOfBattleFormationItemVM>(this.OnFilterSelectionToggled);
			OrderOfBattleFormationItemVM.OnHeroesChanged = new Action(this.OnHeroesChanged);
			OrderOfBattleFormationItemVM.OnFilterUseToggled = new Action<OrderOfBattleFormationItemVM>(this.OnFilterUseToggled);
			OrderOfBattleFormationItemVM.OnSelection = new Action<OrderOfBattleFormationItemVM>(this.SelectFormationItem);
			OrderOfBattleFormationItemVM.OnDeselection = new Action<OrderOfBattleFormationItemVM>(this.DeselectFormationItem);
			OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter = new Func<DeploymentFormationClass, FormationFilterType, int>(this.GetTroopCountWithFilter);
			OrderOfBattleFormationItemVM.GetFormationWithCondition = new Func<Func<OrderOfBattleFormationItemVM, bool>, IEnumerable<OrderOfBattleFormationItemVM>>(this.GetFormationItemsWithCondition);
			OrderOfBattleFormationItemVM.HasAnyTroopWithClass = new Func<FormationClass, bool>(this.HasAnyTroopWithClass);
			OrderOfBattleFormationItemVM.OnAcceptCommander = new Action<OrderOfBattleFormationItemVM>(this.OnFormationAcceptCommander);
			OrderOfBattleFormationItemVM.OnAcceptHeroTroops = new Action<OrderOfBattleFormationItemVM>(this.OnFormationAcceptHeroTroops);
			OrderOfBattleFormationClassVM.OnWeightAdjustedCallback = new Action<OrderOfBattleFormationClassVM>(this.OnWeightAdjusted);
			OrderOfBattleFormationClassVM.OnClassChanged = new Action<OrderOfBattleFormationClassVM, FormationClass>(this.OnFormationClassChanged);
			OrderOfBattleFormationClassVM.CanAdjustWeight = new Func<OrderOfBattleFormationClassVM, bool>(this.CanAdjustWeight);
			OrderOfBattleFormationClassVM.GetTotalCountOfTroopType = new Func<FormationClass, int>(this.GetVisibleTotalTroopCountOfType);
			OrderOfBattleHeroItemVM.OnHeroAssignmentBegin = new Action<OrderOfBattleHeroItemVM>(this.OnHeroAssignmentBegin);
			OrderOfBattleHeroItemVM.OnHeroAssignmentEnd = new Action<OrderOfBattleHeroItemVM>(this.OnHeroAssignmentEnd);
			OrderOfBattleHeroItemVM.GetAgentTooltip = new Func<Agent, List<TooltipProperty>>(this.GetAgentTooltip);
			OrderOfBattleHeroItemVM.OnHeroSelection = new Action<OrderOfBattleHeroItemVM>(this.OnHeroSelection);
			OrderOfBattleHeroItemVM.OnHeroAssignedFormationChanged = new Action<OrderOfBattleHeroItemVM>(this.OnHeroAssignedFormationChanged);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00010B14 File Offset: 0x0000ED14
		private void FinalizeFormationCallbacks()
		{
			OrderOfBattleFormationItemVM.OnClassSelectionToggled = null;
			OrderOfBattleFormationItemVM.OnFilterSelectionToggled = null;
			OrderOfBattleFormationItemVM.OnHeroesChanged = null;
			OrderOfBattleFormationItemVM.OnFilterUseToggled = null;
			OrderOfBattleFormationItemVM.OnSelection = null;
			OrderOfBattleFormationItemVM.OnDeselection = null;
			OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter = null;
			OrderOfBattleFormationItemVM.GetFormationWithCondition = null;
			OrderOfBattleFormationItemVM.HasAnyTroopWithClass = null;
			OrderOfBattleFormationItemVM.OnAcceptCommander = null;
			OrderOfBattleFormationItemVM.OnAcceptHeroTroops = null;
			OrderOfBattleFormationClassVM.OnWeightAdjustedCallback = null;
			OrderOfBattleFormationClassVM.OnClassChanged = null;
			OrderOfBattleFormationClassVM.CanAdjustWeight = null;
			OrderOfBattleFormationClassVM.GetTotalCountOfTroopType = null;
			OrderOfBattleHeroItemVM.OnHeroAssignmentBegin = null;
			OrderOfBattleHeroItemVM.OnHeroAssignmentEnd = null;
			OrderOfBattleHeroItemVM.GetAgentTooltip = null;
			OrderOfBattleHeroItemVM.OnHeroSelection = null;
			OrderOfBattleHeroItemVM.OnHeroAssignedFormationChanged = null;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00010B9C File Offset: 0x0000ED9C
		public void Tick()
		{
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				if (orderOfBattleFormationItemVM != null)
				{
					orderOfBattleFormationItemVM.Tick();
				}
				if (orderOfBattleFormationItemVM != null)
				{
					this.EnsureAllFormationTypesAreSet(orderOfBattleFormationItemVM);
				}
			}
			if (this._isInitialized)
			{
				if (this._isHeroSelectionDirty)
				{
					this.UpdateHeroItemSelection();
					this._isHeroSelectionDirty = false;
				}
				if (this._isTroopCountsDirty)
				{
					this.UpdateTroopTypeLookUpTable();
					this._isTroopCountsDirty = false;
				}
				if (this._isMissingFormationsDirty)
				{
					this.RefreshMissingFormations();
					this._isMissingFormationsDirty = false;
				}
				if (!this._isUnitDeployRefreshed)
				{
					this.OnUnitDeployed();
					this._isUnitDeployRefreshed = true;
				}
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00010C58 File Offset: 0x0000EE58
		[Conditional("DEBUG")]
		private void EnsureAllFormationPercentagesAreValid()
		{
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00010C5C File Offset: 0x0000EE5C
		private void EnsureAllFormationTypesAreSet(OrderOfBattleFormationItemVM f)
		{
			if (this.IsPlayerGeneral && f.OrderOfBattleFormationClassInt == 0 && f.Formation.CountOfUnits > 0)
			{
				bool flag = this._orderController.BackupAndDisableGesturesEnabled();
				for (int i = 0; i < this._allFormations.Count; i++)
				{
					OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations[i];
					if (this._orderController.SelectedFormations.Contains((orderOfBattleFormationItemVM != null) ? orderOfBattleFormationItemVM.Formation : null))
					{
						this._orderController.DeselectFormation((orderOfBattleFormationItemVM != null) ? orderOfBattleFormationItemVM.Formation : null);
					}
				}
				Func<OrderOfBattleFormationClassVM, bool> <>9__2;
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM2 = this._allFormations.Find(delegate(OrderOfBattleFormationItemVM other)
				{
					IEnumerable<OrderOfBattleFormationClassVM> classes = other.Classes;
					Func<OrderOfBattleFormationClassVM, bool> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = (OrderOfBattleFormationClassVM fc) => fc.Class == f.Formation.PrimaryClass);
					}
					return classes.Any(func);
				});
				if (orderOfBattleFormationItemVM2 == null)
				{
					orderOfBattleFormationItemVM2 = this._allFormations.Find((OrderOfBattleFormationItemVM other) => other.OrderOfBattleFormationClassInt != 0);
				}
				if (orderOfBattleFormationItemVM2 != null)
				{
					Formation formation = orderOfBattleFormationItemVM2.Formation;
					this._orderController.SelectFormation(f.Formation);
					this._orderController.SetOrderWithFormationAndNumber(OrderType.Transfer, formation, f.Formation.CountOfUnits);
					for (int j = 0; j < this._allFormations.Count; j++)
					{
						OrderOfBattleFormationItemVM orderOfBattleFormationItemVM3 = this._allFormations[j];
						if (this._orderController.SelectedFormations.Contains(orderOfBattleFormationItemVM3.Formation))
						{
							this._orderController.DeselectFormation(orderOfBattleFormationItemVM3.Formation);
						}
					}
					orderOfBattleFormationItemVM2.OnSizeChanged();
					f.OnSizeChanged();
					this.RefreshWeights();
					this._orderController.RestoreGesturesEnabled(flag);
				}
			}
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00010E10 File Offset: 0x0000F010
		public void Initialize(Mission mission, Camera missionCamera, Action<int> selectFormationAtIndex, Action<int> deselectFormationAtIndex, Action clearFormationSelection, Action onAutoDeploy, Action onBeginMission, Dictionary<int, Agent> formationIndicesAndSergeants)
		{
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._selectFormationAtIndex = selectFormationAtIndex;
			this._deselectFormationAtIndex = deselectFormationAtIndex;
			this._clearFormationSelection = clearFormationSelection;
			this._onAutoDeploy = onAutoDeploy;
			this._onBeginMission = onBeginMission;
			this._bannerBearerLogic = mission.GetMissionBehavior<BannerBearerLogic>();
			if (this._bannerBearerLogic != null)
			{
				this._bannerBearerLogic.OnBannerBearersUpdated += this.OnBannerBearersUpdated;
			}
			this.InitializeFormationCallbacks();
			this._isInitialized = false;
			this._orderController = Mission.Current.PlayerTeam.PlayerOrderController;
			this._orderController.OnSelectedFormationsChanged += this.OnSelectedFormationsChanged;
			this._orderController.OnOrderIssued += this.OnOrderIssued;
			this.CurrentConfiguration = new List<ValueTuple<int, List<int>>>();
			this._availableTroopTypes = MissionGameModels.Current.BattleInitializationModel.GetAllAvailableTroopTypes();
			this.IsPlayerGeneral = this._mission.PlayerTeam.IsPlayerGeneral;
			this.FormationsFirstHalf = new MBBindingList<OrderOfBattleFormationItemVM>();
			this.FormationsSecondHalf = new MBBindingList<OrderOfBattleFormationItemVM>();
			this.UnassignedHeroes = new MBBindingList<OrderOfBattleHeroItemVM>();
			this._visibleTroopTypeCountLookup = new Dictionary<FormationClass, int>
			{
				{
					FormationClass.Infantry,
					0
				},
				{
					FormationClass.Ranged,
					0
				},
				{
					FormationClass.Cavalry,
					0
				},
				{
					FormationClass.HorseArcher,
					0
				}
			};
			for (int i = 0; i < this.TotalFormationCount; i++)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = new OrderOfBattleFormationItemVM(this._missionCamera);
				if (i < this.TotalFormationCount / 2)
				{
					this.FormationsFirstHalf.Add(orderOfBattleFormationItemVM);
				}
				else
				{
					this.FormationsSecondHalf.Add(orderOfBattleFormationItemVM);
				}
				this._allFormations.Add(orderOfBattleFormationItemVM);
				Formation formation = this._mission.PlayerTeam.FormationsIncludingEmpty.ElementAt(i);
				orderOfBattleFormationItemVM.RefreshFormation(formation, DeploymentFormationClass.Unset, false);
			}
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.OnSizeChanged();
			});
			using (List<Agent>.Enumerator enumerator = this._mission.PlayerTeam.GetHeroAgents().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Agent heroAgent = enumerator.Current;
					this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => heroAgent.Formation == f.Formation);
					OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = new OrderOfBattleHeroItemVM(heroAgent);
					this._allHeroes.Add(orderOfBattleHeroItemVM);
					if (this.IsPlayerGeneral || heroAgent.IsMainAgent)
					{
						this.UnassignedHeroes.Add(orderOfBattleHeroItemVM);
					}
				}
			}
			if (!this.IsPlayerGeneral)
			{
				using (Dictionary<int, Agent>.Enumerator enumerator2 = formationIndicesAndSergeants.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<int, Agent> preAssignedCommander = enumerator2.Current;
						this._allHeroes.First((OrderOfBattleHeroItemVM h) => h.Agent == preAssignedCommander.Value).SetIsPreAssigned(true);
						this.AssignCommander(preAssignedCommander.Value, this._allFormations[preAssignedCommander.Key]);
					}
				}
			}
			this.IsEnabled = true;
			this.SetAllFormationsLockState(true);
			this.LoadConfiguration();
			this.SetAllFormationsLockState(false);
			this.SetInitialHeroFormations();
			this.DistributeAllTroops();
			this._isInitialized = true;
			this.RefreshWeights();
			this.DeselectAllFormations();
			this.OnUnitDeployed();
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.UpdateAdjustable();
			});
			if (!this.IsPlayerGeneral)
			{
				this.SelectHeroItem(this._allHeroes.FirstOrDefault((OrderOfBattleHeroItemVM h) => h.Agent.IsMainAgent));
			}
			this._isMissingFormationsDirty = true;
			this._isTroopCountsDirty = true;
			this.RefreshValues();
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000111E4 File Offset: 0x0000F3E4
		private void SetAllFormationsLockState(bool isLocked)
		{
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				for (int j = 0; j < this._allFormations[i].Classes.Count; j++)
				{
					this._allFormations[i].Classes[j].SetWeightAdjustmentLock(isLocked);
				}
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00011248 File Offset: 0x0000F448
		private void UpdateTroopTypeLookUpTable()
		{
			for (FormationClass formationClass = FormationClass.Infantry; formationClass < FormationClass.NumberOfDefaultFormations; formationClass++)
			{
				this._visibleTroopTypeCountLookup[formationClass] = 0;
			}
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				if (this._allFormations[i].Formation != null)
				{
					for (int j = 0; j < this._allFormations[i].Classes.Count; j++)
					{
						OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = this._allFormations[i].Classes[j];
						if (!orderOfBattleFormationClassVM.IsUnset)
						{
							int visibleCountOfUnitsInClass = OrderOfBattleUIHelper.GetVisibleCountOfUnitsInClass(orderOfBattleFormationClassVM);
							Dictionary<FormationClass, int> visibleTroopTypeCountLookup = this._visibleTroopTypeCountLookup;
							FormationClass @class = orderOfBattleFormationClassVM.Class;
							visibleTroopTypeCountLookup[@class] += visibleCountOfUnitsInClass;
						}
					}
				}
			}
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				orderOfBattleFormationItemVM.OnSizeChanged();
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00011350 File Offset: 0x0000F550
		private void OnBannerBearersUpdated(Formation formation)
		{
			if (this._isInitialized)
			{
				foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
				{
					orderOfBattleFormationItemVM.Formation.QuerySystem.Expire();
				}
				this._isTroopCountsDirty = true;
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x000113BC File Offset: 0x0000F5BC
		private OrderOfBattleFormationItemVM GetFirstAvailableFormationWithAnyClass(params FormationClass[] classes)
		{
			OrderOfBattleVM.<>c__DisplayClass50_0 CS$<>8__locals1 = new OrderOfBattleVM.<>c__DisplayClass50_0();
			CS$<>8__locals1.classes = classes;
			int i;
			int j;
			for (i = 0; i < CS$<>8__locals1.classes.Length; i = j + 1)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.HasClass(CS$<>8__locals1.classes[i]));
				if (orderOfBattleFormationItemVM != null)
				{
					return orderOfBattleFormationItemVM;
				}
				j = i;
			}
			return null;
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00011430 File Offset: 0x0000F630
		private OrderOfBattleFormationItemVM GetInitialHeroFormation(OrderOfBattleHeroItemVM hero)
		{
			FormationClass heroClass = FormationClass.NumberOfAllFormations;
			for (FormationClass formationClass = FormationClass.Infantry; formationClass < FormationClass.NumberOfDefaultFormations; formationClass++)
			{
				if (OrderOfBattleUIHelper.IsAgentInFormationClass(hero.Agent, formationClass))
				{
					heroClass = formationClass;
				}
			}
			if (heroClass == FormationClass.NumberOfAllFormations)
			{
				return null;
			}
			OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = null;
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM2 in this._allFormations)
			{
				if (orderOfBattleFormationItemVM2.Commander.Agent == hero.Agent || orderOfBattleFormationItemVM2.HeroTroops.Contains(hero))
				{
					hero.Agent.Formation = orderOfBattleFormationItemVM2.Formation;
					return orderOfBattleFormationItemVM2;
				}
				if (orderOfBattleFormationItemVM2.Formation == hero.Agent.Formation)
				{
					for (int i = orderOfBattleFormationItemVM2.Classes.Count - 1; i >= 0; i--)
					{
						if (!orderOfBattleFormationItemVM2.Classes[i].IsUnset && orderOfBattleFormationItemVM2.Classes[i].Class == heroClass)
						{
							return orderOfBattleFormationItemVM2;
						}
					}
				}
				if (orderOfBattleFormationItemVM != null)
				{
					break;
				}
			}
			if (!this.UnassignedHeroes.Contains(hero))
			{
				this.UnassignedHeroes.Add(hero);
			}
			Func<OrderOfBattleFormationClassVM, bool> <>9__1;
			OrderOfBattleFormationItemVM orderOfBattleFormationItemVM3 = this._allFormations.FirstOrDefault(delegate(OrderOfBattleFormationItemVM x)
			{
				IEnumerable<OrderOfBattleFormationClassVM> classes = x.Classes;
				Func<OrderOfBattleFormationClassVM, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (OrderOfBattleFormationClassVM c) => c.Class == heroClass);
				}
				return classes.Any(func);
			});
			if (orderOfBattleFormationItemVM3 != null)
			{
				hero.Agent.Formation = orderOfBattleFormationItemVM3.Formation;
				return orderOfBattleFormationItemVM3;
			}
			FormationClass[] array = null;
			if (heroClass == FormationClass.HorseArcher)
			{
				FormationClass[] array2 = new FormationClass[3];
				array2[0] = FormationClass.Cavalry;
				array2[1] = FormationClass.Ranged;
				array = array2;
			}
			else if (heroClass == FormationClass.Cavalry)
			{
				FormationClass[] array3 = new FormationClass[2];
				array3[0] = FormationClass.Ranged;
				array = array3;
			}
			else if (heroClass == FormationClass.Ranged)
			{
				array = new FormationClass[1];
			}
			if (array != null)
			{
				OrderOfBattleFormationItemVM firstAvailableFormationWithAnyClass = this.GetFirstAvailableFormationWithAnyClass(array);
				if (firstAvailableFormationWithAnyClass != null)
				{
					hero.Agent.Formation = firstAvailableFormationWithAnyClass.Formation;
					return firstAvailableFormationWithAnyClass;
				}
			}
			return null;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00011628 File Offset: 0x0000F828
		[return: TupleElementNames(new string[] { "Hero", "WasCommander" })]
		private List<ValueTuple<OrderOfBattleHeroItemVM, bool>> ClearAllHeroAssignments()
		{
			List<ValueTuple<OrderOfBattleHeroItemVM, bool>> list = new List<ValueTuple<OrderOfBattleHeroItemVM, bool>>();
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				if (this._allFormations[i].HasCommander)
				{
					OrderOfBattleHeroItemVM commander = this._allFormations[i].Commander;
					list.Add(new ValueTuple<OrderOfBattleHeroItemVM, bool>(commander, true));
					this.ClearHeroAssignment(commander);
				}
				for (int j = 0; j < this._allFormations[i].HeroTroops.Count; j++)
				{
					OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._allFormations[i].HeroTroops[j];
					list.Add(new ValueTuple<OrderOfBattleHeroItemVM, bool>(orderOfBattleHeroItemVM, false));
					this.ClearHeroAssignment(orderOfBattleHeroItemVM);
				}
			}
			return list;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000116E4 File Offset: 0x0000F8E4
		private void AssignHeroesToInitialFormations([TupleElementNames(new string[] { "Hero", "WasCommander" })] List<ValueTuple<OrderOfBattleHeroItemVM, bool>> heroes)
		{
			for (int i = 0; i < heroes.Count; i++)
			{
				if (heroes[i].Item2)
				{
					this.AssignCommander(heroes[i].Item1.Agent, heroes[i].Item1.InitialFormationItem);
				}
				else
				{
					heroes[i].Item1.InitialFormationItem.AddHeroTroop(heroes[i].Item1);
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0001175C File Offset: 0x0000F95C
		private void SetInitialHeroFormations()
		{
			for (int i = 0; i < this._allHeroes.Count; i++)
			{
				OrderOfBattleFormationItemVM initialHeroFormation = this.GetInitialHeroFormation(this._allHeroes[i]);
				if (initialHeroFormation != null)
				{
					this._allHeroes[i].SetInitialFormation(initialHeroFormation);
				}
				else
				{
					OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations.FirstOrDefault(delegate(OrderOfBattleFormationItemVM f)
					{
						if (f.HasFormation)
						{
							return f.Classes.Any((OrderOfBattleFormationClassVM c) => !c.IsUnset);
						}
						return false;
					});
					if (orderOfBattleFormationItemVM != null)
					{
						this._allHeroes[i].SetInitialFormation(orderOfBattleFormationItemVM);
					}
					else
					{
						Debug.FailedAssert("Failed to find an initial formation for hero: " + this._allHeroes[i].Agent.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleVM.cs", "SetInitialHeroFormations", 622);
					}
				}
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00011825 File Offset: 0x0000FA25
		protected virtual void LoadConfiguration()
		{
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00011827 File Offset: 0x0000FA27
		protected virtual void SaveConfiguration()
		{
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0001182C File Offset: 0x0000FA2C
		protected virtual List<TooltipProperty> GetAgentTooltip(Agent agent)
		{
			if (agent == null)
			{
				return new List<TooltipProperty>
				{
					new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator)
				};
			}
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(agent.Name, string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.Title)
			};
			BannerComponent bannerComponent;
			if (agent.FormationBanner != null && (bannerComponent = agent.FormationBanner.ItemComponent as BannerComponent) != null)
			{
				list.Add(new TooltipProperty(this._bannerText.ToString(), agent.FormationBanner.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				GameTexts.SetVariable("RANK", bannerComponent.BannerEffect.Name);
				string text = string.Empty;
				if (bannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.AddFactor)
				{
					GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", ((int)Math.Abs(bannerComponent.GetBannerEffectBonus() * 100f)).ToString());
					object obj;
					text = obj.ToString();
				}
				else if (bannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.Add)
				{
					text = bannerComponent.GetBannerEffectBonus().ToString();
				}
				GameTexts.SetVariable("NUMBER", text);
				list.Add(new TooltipProperty(this._bannerEffectText.ToString(), GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			else
			{
				list.Add(new TooltipProperty(this._noBannerEquippedText.ToString(), string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x000119B8 File Offset: 0x0000FBB8
		private bool HasAnyTroopWithClass(FormationClass formationClass)
		{
			return this._availableTroopTypes.Contains(formationClass);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x000119C8 File Offset: 0x0000FBC8
		private void RefreshWeights()
		{
			if (this._isSaving || !this._isInitialized)
			{
				return;
			}
			List<OrderOfBattleFormationClassVM> allClasses = new List<OrderOfBattleFormationClassVM>();
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM formation)
			{
				for (int i = 0; i < formation.Classes.Count; i++)
				{
					OrderOfBattleFormationClassVM orderOfBattleFormationClassVM2 = formation.Classes[i];
					if (orderOfBattleFormationClassVM2.Class != FormationClass.NumberOfAllFormations)
					{
						allClasses.Add(orderOfBattleFormationClassVM2);
					}
				}
			});
			FormationClass fc;
			allClasses.ForEach(delegate(OrderOfBattleFormationClassVM fc)
			{
				fc.SetWeightAdjustmentLock(true);
				float num2 = (float)OrderOfBattleUIHelper.GetCountOfRealUnitsInClass(fc);
				float num3 = (float)allClasses.Where((OrderOfBattleFormationClassVM c) => c.Class == fc.Class).Sum((OrderOfBattleFormationClassVM c) => OrderOfBattleUIHelper.GetCountOfRealUnitsInClass(c));
				fc.Weight = MathF.Round(num2 / num3 * 100f);
				fc.IsLocked = !this.IsPlayerGeneral;
				fc.SetWeightAdjustmentLock(false);
			});
			FormationClass fc2;
			for (fc = FormationClass.Infantry; fc < FormationClass.NumberOfDefaultFormations; fc = fc2 + 1)
			{
				IEnumerable<OrderOfBattleFormationClassVM> enumerable = allClasses.Where((OrderOfBattleFormationClassVM c) => c.Class == fc);
				OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = enumerable.FirstOrDefault<OrderOfBattleFormationClassVM>();
				if (enumerable.Count<OrderOfBattleFormationClassVM>() > 1)
				{
					for (int num = enumerable.Sum((OrderOfBattleFormationClassVM item) => item.Weight); num != 100; num = enumerable.Sum((OrderOfBattleFormationClassVM item) => item.Weight))
					{
						orderOfBattleFormationClassVM.SetWeightAdjustmentLock(true);
						orderOfBattleFormationClassVM.Weight += ((num > 100) ? (-1) : 1);
						orderOfBattleFormationClassVM.SetWeightAdjustmentLock(false);
					}
				}
				fc2 = fc;
			}
			allClasses.ForEach(delegate(OrderOfBattleFormationClassVM fc)
			{
				fc.UpdateWeightAdjustable();
			});
			allClasses.ForEach(delegate(OrderOfBattleFormationClassVM fc)
			{
				fc.UpdateWeightText();
			});
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00011B54 File Offset: 0x0000FD54
		public void OnAllFormationsAssignedSergeants(Dictionary<int, Agent> preAssignedCommanders)
		{
			foreach (KeyValuePair<int, Agent> keyValuePair in preAssignedCommanders)
			{
				this.AssignCommander(keyValuePair.Value, this._allFormations[keyValuePair.Key]);
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00011BBC File Offset: 0x0000FDBC
		private void OnClassSelectionToggled(OrderOfBattleFormationItemVM formationItem)
		{
			if (formationItem != null && formationItem.IsClassSelectionActive)
			{
				this._lastEnabledClassSelection = formationItem;
				return;
			}
			this._lastEnabledClassSelection = null;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00011BD8 File Offset: 0x0000FDD8
		private void OnFilterSelectionToggled(OrderOfBattleFormationItemVM formationItem)
		{
			if (formationItem != null && formationItem.IsFilterSelectionActive)
			{
				this._lastEnabledFilterSelection = formationItem;
				return;
			}
			this._lastEnabledFilterSelection = null;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00011BF4 File Offset: 0x0000FDF4
		public bool IsAnyClassSelectionEnabled()
		{
			return this._lastEnabledClassSelection != null;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00011BFF File Offset: 0x0000FDFF
		public bool IsAnyFilterSelectionEnabled()
		{
			return this._lastEnabledFilterSelection != null;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00011C0A File Offset: 0x0000FE0A
		public void ExecuteDisableAllClassSelections()
		{
			if (this._lastEnabledClassSelection != null)
			{
				this._lastEnabledClassSelection.IsClassSelectionActive = false;
				this._lastEnabledClassSelection = null;
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00011C27 File Offset: 0x0000FE27
		public void ExecuteDisableAllFilterSelections()
		{
			if (this._lastEnabledFilterSelection != null)
			{
				this._lastEnabledFilterSelection.IsFilterSelectionActive = false;
				this._lastEnabledFilterSelection = null;
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00011C44 File Offset: 0x0000FE44
		private void SelectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			if (!this._selectedHeroes.Contains(heroItem))
			{
				heroItem.IsSelected = true;
				this._selectedHeroes.Add(heroItem);
				this.UpdateHeroItemSelection();
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00011C6D File Offset: 0x0000FE6D
		private void DeselectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			heroItem.IsSelected = false;
			this._selectedHeroes.Remove(heroItem);
			this.UpdateHeroItemSelection();
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00011C89 File Offset: 0x0000FE89
		private void ToggleHeroItemSelection(OrderOfBattleHeroItemVM heroItem)
		{
			if (this._selectedHeroes.Contains(heroItem))
			{
				this.DeselectHeroItem(heroItem);
			}
			else
			{
				this.SelectHeroItem(heroItem);
			}
			this.UpdateHeroItemSelection();
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00011CB0 File Offset: 0x0000FEB0
		private void UpdateHeroItemSelection()
		{
			bool flag = this._selectedHeroes.Count > 0;
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				bool flag2 = orderOfBattleFormationItemVM.HeroTroops.Any((OrderOfBattleHeroItemVM heroTroop) => this._selectedHeroes.Contains(heroTroop));
				orderOfBattleFormationItemVM.OnHeroSelectionUpdated(this._selectedHeroes.Count, flag2);
			}
			bool flag3;
			if (flag)
			{
				flag3 = this._selectedHeroes.All((OrderOfBattleHeroItemVM hero) => hero.IsLeadingAFormation);
			}
			else
			{
				flag3 = false;
			}
			this.IsPoolAcceptingCommander = flag3;
			bool flag4;
			if (flag && !this.IsPoolAcceptingCommander)
			{
				flag4 = this._selectedHeroes.All((OrderOfBattleHeroItemVM hero) => hero.IsAssignedToAFormation);
			}
			else
			{
				flag4 = false;
			}
			this.IsPoolAcceptingHeroTroops = flag4;
			this.SelectedHeroCount = this._selectedHeroes.Count;
			this.HasSelectedHeroes = flag;
			this.LastSelectedHeroItem = ((this._selectedHeroes.Count > 0) ? this._selectedHeroes[this._selectedHeroes.Count - 1] : null);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00011DF0 File Offset: 0x0000FFF0
		private void OnHeroAssignmentBegin(OrderOfBattleHeroItemVM heroItem)
		{
			this.SelectHeroItem(heroItem);
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsShown = false;
			});
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00011E23 File Offset: 0x00010023
		private void OnHeroAssignmentEnd(OrderOfBattleHeroItemVM heroItem)
		{
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsShown = true;
			});
			this.UpdateHeroItemSelection();
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00011E55 File Offset: 0x00010055
		private void ClearAndSelectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			this.ClearHeroItemSelection();
			this.SelectHeroItem(heroItem);
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00011E64 File Offset: 0x00010064
		private void ClearHeroAssignment(OrderOfBattleHeroItemVM heroItem)
		{
			if (heroItem.IsLeadingAFormation)
			{
				heroItem.CurrentAssignedFormationItem.UnassignCommander();
				return;
			}
			if (heroItem.IsAssignedToAFormation)
			{
				heroItem.CurrentAssignedFormationItem.RemoveHeroTroop(heroItem);
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00011E90 File Offset: 0x00010090
		protected void AssignCommander(Agent agent, OrderOfBattleFormationItemVM formationItem)
		{
			OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._allHeroes.FirstOrDefault((OrderOfBattleHeroItemVM h) => h.Agent == agent);
			if (formationItem != null && orderOfBattleHeroItemVM != null)
			{
				if (formationItem.HasCommander)
				{
					formationItem.Commander.IsSelected = false;
					formationItem.UnassignCommander();
				}
				formationItem.Commander = orderOfBattleHeroItemVM;
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00011EE9 File Offset: 0x000100E9
		private void ClearHeroItemSelection()
		{
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsSelected = false;
			});
			this._selectedHeroes.Clear();
			this.UpdateHeroItemSelection();
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00011F28 File Offset: 0x00010128
		public void ExecuteAcceptHeroes()
		{
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in this._selectedHeroes)
			{
				this.ClearHeroAssignment(orderOfBattleHeroItemVM);
				orderOfBattleHeroItemVM.IsShown = true;
			}
			this.ClearHeroItemSelection();
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00011F88 File Offset: 0x00010188
		public void ExecuteSelectAllHeroes()
		{
			this.ClearHeroItemSelection();
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in this.UnassignedHeroes)
			{
				this.SelectHeroItem(orderOfBattleHeroItemVM);
			}
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00011FDC File Offset: 0x000101DC
		public void ExecuteClearHeroSelection()
		{
			this.ClearHeroItemSelection();
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00011FE4 File Offset: 0x000101E4
		private void OnFormationAcceptCommander(OrderOfBattleFormationItemVM formationItem)
		{
			if (this._selectedHeroes.Count != 1)
			{
				this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
				{
					hero.IsShown = true;
				});
				this.ClearHeroItemSelection();
				return;
			}
			OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._selectedHeroes[0];
			this.ClearHeroAssignment(orderOfBattleHeroItemVM);
			this.AssignCommander(orderOfBattleHeroItemVM.Agent, formationItem);
			this.ClearHeroItemSelection();
			orderOfBattleHeroItemVM.IsShown = true;
			if (!this.IsPlayerGeneral)
			{
				this._mission.GetMissionBehavior<AssignPlayerRoleInTeamMissionController>().OnPlayerChoiceMade(formationItem.Formation.Index, false);
			}
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<OrderOfBattleHeroAssignedToFormationEvent>(new OrderOfBattleHeroAssignedToFormationEvent(orderOfBattleHeroItemVM.Agent, formationItem.Formation));
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000120A8 File Offset: 0x000102A8
		private void OnFormationAcceptHeroTroops(OrderOfBattleFormationItemVM formationItem)
		{
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in this._selectedHeroes)
			{
				this.ClearHeroAssignment(orderOfBattleHeroItemVM);
				formationItem.AddHeroTroop(orderOfBattleHeroItemVM);
				orderOfBattleHeroItemVM.IsShown = true;
			}
			this.ClearHeroItemSelection();
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00012110 File Offset: 0x00010310
		private void OnHeroSelection(OrderOfBattleHeroItemVM heroSlotItem)
		{
			if (!this.IsPlayerGeneral)
			{
				this.ToggleHeroItemSelection(heroSlotItem);
				return;
			}
			if (heroSlotItem.IsLeadingAFormation)
			{
				this.ClearAndSelectHeroItem(heroSlotItem);
				return;
			}
			this.ToggleHeroItemSelection(heroSlotItem);
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0001213C File Offset: 0x0001033C
		private void OnFilterUseToggled(OrderOfBattleFormationItemVM formationItem)
		{
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in formationItem.Classes)
			{
				if (orderOfBattleFormationClassVM.Class != FormationClass.NumberOfAllFormations)
				{
					this.DistributeTroops(orderOfBattleFormationClassVM);
				}
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00012194 File Offset: 0x00010394
		public void OnDeploymentFinalized(bool playerDeployed)
		{
			if (playerDeployed)
			{
				this._isSaving = true;
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Commander.Agent == Agent.Main);
				if (orderOfBattleFormationItemVM != null)
				{
					this._mission.GetMissionBehavior<AssignPlayerRoleInTeamMissionController>().OnPlayerChoiceMade(orderOfBattleFormationItemVM.Formation.Index, true);
				}
				this.SaveConfiguration();
				this._isSaving = false;
				this._orderController.OnSelectedFormationsChanged -= this.OnSelectedFormationsChanged;
				this._orderController.OnOrderIssued -= this.OnOrderIssued;
			}
			this.IsEnabled = false;
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0001223C File Offset: 0x0001043C
		private void OnHeroAssignedFormationChanged(OrderOfBattleHeroItemVM heroItem)
		{
			if (heroItem.IsAssignedToAFormation)
			{
				this.UnassignedHeroes.Remove(this.UnassignedHeroes.FirstOrDefault((OrderOfBattleHeroItemVM h) => h.Agent == heroItem.Agent));
			}
			else if (this.IsPlayerGeneral || heroItem.Agent.IsMainAgent)
			{
				this.UnassignedHeroes.Insert(0, heroItem);
			}
			this._isTroopCountsDirty = true;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x000122BB File Offset: 0x000104BB
		private bool CanAdjustWeight(OrderOfBattleFormationClassVM formationClass)
		{
			return this._isInitialized && OrderOfBattleUIHelper.GetMatchingClasses(this._allFormations, formationClass, null).Count > 1;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x000122DC File Offset: 0x000104DC
		private void OnWeightAdjusted(OrderOfBattleFormationClassVM formationClass)
		{
			if (!this._isInitialized)
			{
				return;
			}
			this.DistributeWeights(formationClass);
			this.DistributeTroops(formationClass);
			EventManager eventManager = Game.Current.EventManager;
			OrderOfBattleFormationItemVM belongedFormationItem = formationClass.BelongedFormationItem;
			eventManager.TriggerEvent<OrderOfBattleFormationWeightChangedEvent>(new OrderOfBattleFormationWeightChangedEvent((belongedFormationItem != null) ? belongedFormationItem.Formation : null));
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0001231C File Offset: 0x0001051C
		private void DistributeWeights(OrderOfBattleFormationClassVM formationClass)
		{
			List<OrderOfBattleFormationClassVM> matchingClasses = OrderOfBattleUIHelper.GetMatchingClasses(this._allFormations, formationClass, (OrderOfBattleFormationClassVM fc) => !fc.IsLocked);
			if (matchingClasses.Count == 1)
			{
				formationClass.SetWeightAdjustmentLock(true);
				formationClass.Weight = formationClass.PreviousWeight;
				formationClass.SetWeightAdjustmentLock(false);
				return;
			}
			int num = OrderOfBattleUIHelper.GetMatchingClasses(this._allFormations, formationClass, (OrderOfBattleFormationClassVM fc) => fc.IsLocked).Sum((OrderOfBattleFormationClassVM fc) => fc.Weight);
			int adjustableWeight = 100 - num;
			if (formationClass.Weight > adjustableWeight)
			{
				formationClass.SetWeightAdjustmentLock(true);
				formationClass.Weight = adjustableWeight;
				formationClass.SetWeightAdjustmentLock(false);
				matchingClasses.Remove(formationClass);
				matchingClasses.ForEach(delegate(OrderOfBattleFormationClassVM c)
				{
					c.SetWeightAdjustmentLock(true);
					c.Weight = 0;
					c.SetWeightAdjustmentLock(false);
				});
				return;
			}
			matchingClasses.Remove(formationClass);
			int changePerClass = MathF.Round((float)(formationClass.PreviousWeight - formationClass.Weight) / (float)matchingClasses.Count);
			matchingClasses.ForEach(delegate(OrderOfBattleFormationClassVM formation)
			{
				formation.SetWeightAdjustmentLock(true);
			});
			if (changePerClass != 0)
			{
				matchingClasses.ForEach(delegate(OrderOfBattleFormationClassVM formation)
				{
					int num5 = MBMath.ClampInt(changePerClass, -formation.Weight, adjustableWeight - formation.Weight);
					formation.Weight += num5;
				});
			}
			int num2 = matchingClasses.Sum((OrderOfBattleFormationClassVM c) => c.Weight) + formationClass.Weight;
			while (matchingClasses.Count > 0 && num2 != 100)
			{
				int num3 = num2;
				if (num2 > 100)
				{
					OrderOfBattleFormationClassVM formationClassWithExtremumWeight = OrderOfBattleUIHelper.GetFormationClassWithExtremumWeight(matchingClasses, false);
					if (formationClassWithExtremumWeight != null)
					{
						OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = formationClassWithExtremumWeight;
						int num4 = orderOfBattleFormationClassVM.Weight;
						orderOfBattleFormationClassVM.Weight = num4 - 1;
						num2--;
					}
				}
				else if (num2 < 100)
				{
					OrderOfBattleFormationClassVM formationClassWithExtremumWeight2 = OrderOfBattleUIHelper.GetFormationClassWithExtremumWeight(matchingClasses, true);
					if (formationClassWithExtremumWeight2 != null)
					{
						OrderOfBattleFormationClassVM orderOfBattleFormationClassVM2 = formationClassWithExtremumWeight2;
						int num4 = orderOfBattleFormationClassVM2.Weight;
						orderOfBattleFormationClassVM2.Weight = num4 + 1;
						num2++;
					}
				}
				if (num3 == num2)
				{
					Debug.FailedAssert("Failed to sum up all weights to 100", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleVM.cs", "DistributeWeights", 1149);
					break;
				}
			}
			matchingClasses.ForEach(delegate(OrderOfBattleFormationClassVM formation)
			{
				formation.SetWeightAdjustmentLock(false);
			});
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00012570 File Offset: 0x00010770
		private void DistributeAllTroops()
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> list = new List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>>();
			List<FormationClass> list2 = new List<FormationClass>();
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				for (int j = 0; j < this._allFormations[i].Classes.Count; j++)
				{
					OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = this._allFormations[i].Classes[j];
					if (!orderOfBattleFormationClassVM.IsUnset && !list2.Contains(orderOfBattleFormationClassVM.Class))
					{
						List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> massTransferDataForFormation = this.GetMassTransferDataForFormation(orderOfBattleFormationClassVM);
						list.AddRange(massTransferDataForFormation);
						list2.Add(orderOfBattleFormationClassVM.Class);
					}
				}
				if (list.Count > 0)
				{
					this._orderController.RearrangeFormationsAccordingToFilters(Agent.Main.Team, list);
				}
				list.Clear();
				if (list2.Count == 4)
				{
					break;
				}
			}
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.OnSizeChanged();
			});
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00012670 File Offset: 0x00010870
		private List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> GetMassTransferDataForFormationClass(Formation targetFormation, FormationClass formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> list = new List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>>();
			List<OrderOfBattleFormationItemVM> list2 = new List<OrderOfBattleFormationItemVM>();
			List<int> list3 = new List<int>();
			int num = 0;
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				int totalCountOfUnitsInClass = OrderOfBattleUIHelper.GetTotalCountOfUnitsInClass(this._allFormations[i].Formation, formationClass);
				if (totalCountOfUnitsInClass > 0 || this._allFormations[i].Formation == targetFormation)
				{
					list2.Add(this._allFormations[i]);
					list3.Add(totalCountOfUnitsInClass);
					num += totalCountOfUnitsInClass;
				}
			}
			if (list2.Count == 1)
			{
				return list;
			}
			if (num > 0)
			{
				List<int> list4 = new List<int>();
				for (int j = 0; j < list2.Count; j++)
				{
					int num2 = ((list2[j].Formation == targetFormation) ? num : 0);
					list4.Add(num2);
				}
				int num3;
				while (list4.Count > 0 && (num3 = list4.Sum()) != num)
				{
					int num4 = num3 - num;
					int num5;
					if (num4 <= 0)
					{
						num5 = list4.IndexOfMin((int c) => c);
					}
					else
					{
						num5 = list4.IndexOfMax((int c) => c);
					}
					List<int> list5 = list4;
					int num6 = num5;
					list5[num6] -= Math.Sign(num4);
				}
				for (int k = 0; k < list4.Count; k++)
				{
					OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = list2[k];
					Team.TroopFilter troopFilter = OrderOfBattleUIHelper.GetTroopFilterForClass(new FormationClass[] { formationClass });
					troopFilter |= OrderOfBattleUIHelper.GetTroopFilterForFormationFilter((from f in orderOfBattleFormationItemVM.FilterItems
						where f.IsActive
						select f.FilterType).ToArray<FormationFilterType>());
					if (troopFilter != (Team.TroopFilter)0)
					{
						Tuple<Formation, int, Team.TroopFilter, List<Agent>> tuple = OrderOfBattleUIHelper.CreateMassTransferData(orderOfBattleFormationItemVM, formationClass, troopFilter, list4[k]);
						list.Add(tuple);
					}
				}
			}
			return list;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0001289C File Offset: 0x00010A9C
		private List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> GetMassTransferDataForFormation(OrderOfBattleFormationClassVM formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> list = new List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>>();
			List<OrderOfBattleFormationClassVM> allFormationClassesWith = this.GetAllFormationClassesWith(formationClass.Class);
			if (allFormationClassesWith.Count == 1)
			{
				return list;
			}
			int num = allFormationClassesWith.Sum((OrderOfBattleFormationClassVM c) => OrderOfBattleUIHelper.GetCountOfRealUnitsInClass(c));
			if (num > 0)
			{
				List<int> list2 = new List<int>();
				for (int i = 0; i < allFormationClassesWith.Count; i++)
				{
					int num2 = MathF.Ceiling((float)allFormationClassesWith[i].Weight / 100f * (float)num);
					list2.Add(num2);
				}
				int num3;
				while (list2.Count > 0 && (num3 = list2.Sum()) != num)
				{
					int num4 = num3 - num;
					int num5;
					if (num4 <= 0)
					{
						num5 = list2.IndexOfMin((int c) => c);
					}
					else
					{
						num5 = list2.IndexOfMax((int c) => c);
					}
					List<int> list3 = list2;
					int num6 = num5;
					list3[num6] -= Math.Sign(num4);
				}
				for (int j = 0; j < list2.Count; j++)
				{
					OrderOfBattleFormationItemVM belongedFormationItem = allFormationClassesWith[j].BelongedFormationItem;
					Team.TroopFilter troopFilter = OrderOfBattleUIHelper.GetTroopFilterForClass((from c in belongedFormationItem.Classes
						where !c.IsUnset
						select c.Class).ToArray<FormationClass>());
					troopFilter |= OrderOfBattleUIHelper.GetTroopFilterForFormationFilter((from f in belongedFormationItem.FilterItems
						where f.IsActive
						select f.FilterType).ToArray<FormationFilterType>());
					if (troopFilter != (Team.TroopFilter)0)
					{
						Tuple<Formation, int, Team.TroopFilter, List<Agent>> tuple = OrderOfBattleUIHelper.CreateMassTransferData(allFormationClassesWith[j], allFormationClassesWith[j].Class, troopFilter, list2[j]);
						list.Add(tuple);
					}
				}
			}
			return list;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00012AE0 File Offset: 0x00010CE0
		private List<OrderOfBattleFormationClassVM> GetAllFormationClassesWith(FormationClass formationClass)
		{
			List<OrderOfBattleFormationClassVM> list = new List<OrderOfBattleFormationClassVM>();
			if (formationClass >= FormationClass.NumberOfDefaultFormations)
			{
				return list;
			}
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				for (int j = 0; j < this._allFormations[i].Classes.Count; j++)
				{
					if (this._allFormations[i].Classes[j].Class == formationClass)
					{
						list.Add(this._allFormations[i].Classes[j]);
					}
				}
			}
			return list;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00012B70 File Offset: 0x00010D70
		private void RefreshFormationsWithClass(FormationClass formationClass)
		{
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				for (int j = 0; j < this._allFormations[i].Classes.Count; j++)
				{
					if (this._allFormations[i].Classes[j].Class == formationClass)
					{
						this._allFormations[i].OnSizeChanged();
						break;
					}
				}
			}
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00012BE8 File Offset: 0x00010DE8
		private void DistributeTroops(OrderOfBattleFormationClassVM formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> massTransferDataForFormation = this.GetMassTransferDataForFormation(formationClass);
			if (massTransferDataForFormation.Count > 0)
			{
				this._orderController.RearrangeFormationsAccordingToFilters(Agent.Main.Team, massTransferDataForFormation);
				this.RefreshFormationsWithClass(formationClass.Class);
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00012C28 File Offset: 0x00010E28
		private List<Agent> GetLockedAgents()
		{
			List<Agent> list = new List<Agent>();
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				if (orderOfBattleFormationItemVM.Commander.Agent != null)
				{
					list.Add(orderOfBattleFormationItemVM.Commander.Agent);
				}
				foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in orderOfBattleFormationItemVM.HeroTroops)
				{
					list.Add(orderOfBattleHeroItemVM.Agent);
				}
			}
			return list;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00012CDC File Offset: 0x00010EDC
		private void TransferAllAvailableTroopsToFormation(OrderOfBattleFormationItemVM formation, FormationClass formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> massTransferDataForFormationClass = this.GetMassTransferDataForFormationClass(formation.Formation, formationClass);
			if (massTransferDataForFormationClass.Count > 0)
			{
				this._orderController.RearrangeFormationsAccordingToFilters(Agent.Main.Team, massTransferDataForFormationClass);
				this.RefreshFormationsWithClass(formationClass);
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00012D20 File Offset: 0x00010F20
		private void OnFormationClassChanged(OrderOfBattleFormationClassVM formationClassItem, FormationClass newFormationClass)
		{
			if (!this._isInitialized)
			{
				return;
			}
			List<OrderOfBattleFormationClassVM> previousFormationClasses = new List<OrderOfBattleFormationClassVM>();
			List<OrderOfBattleFormationClassVM> newFormationClasses = new List<OrderOfBattleFormationClassVM>();
			Func<OrderOfBattleFormationClassVM, bool> <>9__5;
			Func<OrderOfBattleFormationClassVM, bool> <>9__6;
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM formation)
			{
				List<OrderOfBattleFormationClassVM> previousFormationClasses2 = previousFormationClasses;
				IEnumerable<OrderOfBattleFormationClassVM> enumerable = formation.Classes.ToList<OrderOfBattleFormationClassVM>();
				Func<OrderOfBattleFormationClassVM, bool> func3;
				if ((func3 = <>9__5) == null)
				{
					func3 = (<>9__5 = (OrderOfBattleFormationClassVM fc) => fc.Class == formationClassItem.Class);
				}
				previousFormationClasses2.AddRange(enumerable.Where(func3));
				List<OrderOfBattleFormationClassVM> newFormationClasses2 = newFormationClasses;
				IEnumerable<OrderOfBattleFormationClassVM> enumerable2 = formation.Classes.ToList<OrderOfBattleFormationClassVM>();
				Func<OrderOfBattleFormationClassVM, bool> func4;
				if ((func4 = <>9__6) == null)
				{
					func4 = (<>9__6 = (OrderOfBattleFormationClassVM fc) => fc.Class == newFormationClass);
				}
				newFormationClasses2.AddRange(enumerable2.Where(func4));
			});
			if (newFormationClasses.Count > 0)
			{
				formationClassItem.Weight = 0;
			}
			else
			{
				this.TransferAllAvailableTroopsToFormation(formationClassItem.BelongedFormationItem, newFormationClass);
				formationClassItem.SetWeightAdjustmentLock(true);
				formationClassItem.Weight = 100;
				formationClassItem.SetWeightAdjustmentLock(false);
			}
			newFormationClasses.Add(formationClassItem);
			previousFormationClasses.ForEach(delegate(OrderOfBattleFormationClassVM fc)
			{
				fc.IsAdjustable = formationClassItem.Class != FormationClass.NumberOfAllFormations && previousFormationClasses.Count > 2;
			});
			newFormationClasses.ForEach(delegate(OrderOfBattleFormationClassVM fc)
			{
				fc.IsAdjustable = newFormationClass != FormationClass.NumberOfAllFormations && newFormationClasses.Count > 1;
			});
			List<OrderOfBattleFormationClassVM> allClasses = new List<OrderOfBattleFormationClassVM>();
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM formation)
			{
				allClasses.AddRange(formation.Classes.Where((OrderOfBattleFormationClassVM fc) => fc.Class != FormationClass.NumberOfAllFormations));
			});
			if (newFormationClass != FormationClass.NumberOfAllFormations || allClasses.Contains(formationClassItem))
			{
				allClasses.Remove(formationClassItem);
				allClasses.Add(new OrderOfBattleFormationClassVM(formationClassItem.BelongedFormationItem, newFormationClass));
			}
			bool flag = this._orderController.BackupAndDisableGesturesEnabled();
			IEnumerable<OrderOfBattleFormationItemVM> allFormations = this._allFormations;
			Func<OrderOfBattleFormationItemVM, bool> <>9__8;
			Func<OrderOfBattleFormationItemVM, bool> func;
			if ((func = <>9__8) == null)
			{
				Func<OrderOfBattleFormationClassVM, bool> <>9__9;
				func = (<>9__8 = delegate(OrderOfBattleFormationItemVM f)
				{
					IEnumerable<OrderOfBattleFormationClassVM> classes2 = f.Classes;
					Func<OrderOfBattleFormationClassVM, bool> func5;
					if ((func5 = <>9__9) == null)
					{
						func5 = (<>9__9 = (OrderOfBattleFormationClassVM c) => c.Class != newFormationClass);
					}
					return classes2.All(func5);
				});
			}
			Func<OrderOfBattleFormationClassVM, bool> <>9__10;
			Action<OrderOfBattleFormationItemVM> <>9__11;
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in allFormations.Where(func))
			{
				IEnumerable<OrderOfBattleFormationClassVM> classes = orderOfBattleFormationItemVM.Classes;
				Func<OrderOfBattleFormationClassVM, bool> func2;
				if ((func2 = <>9__10) == null)
				{
					func2 = (<>9__10 = (OrderOfBattleFormationClassVM c) => c.Class == newFormationClass);
				}
				ValueTuple<int, bool, bool> relevantTroopTransferParameters = OrderOfBattleUIHelper.GetRelevantTroopTransferParameters(classes.FirstOrDefault(func2));
				if (relevantTroopTransferParameters.Item1 > 0)
				{
					List<OrderOfBattleFormationItemVM> allFormations2 = this._allFormations;
					Action<OrderOfBattleFormationItemVM> action;
					if ((action = <>9__11) == null)
					{
						action = (<>9__11 = delegate(OrderOfBattleFormationItemVM f)
						{
							if (this._orderController.SelectedFormations.Contains(f.Formation))
							{
								this._orderController.DeselectFormation(f.Formation);
							}
						});
					}
					allFormations2.ForEach(action);
					this._orderController.SelectFormation(orderOfBattleFormationItemVM.Formation);
					this._orderController.TransferUnitWithPriorityFunction(formationClassItem.BelongedFormationItem.Formation, relevantTroopTransferParameters.Item1, false, false, false, false, relevantTroopTransferParameters.Item2, relevantTroopTransferParameters.Item3, true, this.GetLockedAgents());
					orderOfBattleFormationItemVM.OnSizeChanged();
					formationClassItem.BelongedFormationItem.OnSizeChanged();
				}
			}
			this._isTroopCountsDirty = true;
			this._isHeroSelectionDirty = true;
			this._isMissingFormationsDirty = true;
			this._orderController.RestoreGesturesEnabled(flag);
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.UpdateAdjustable();
			});
			EventManager eventManager = Game.Current.EventManager;
			OrderOfBattleFormationItemVM belongedFormationItem = formationClassItem.BelongedFormationItem;
			eventManager.TriggerEvent<OrderOfBattleFormationClassChangedEvent>(new OrderOfBattleFormationClassChangedEvent((belongedFormationItem != null) ? belongedFormationItem.Formation : null));
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00013040 File Offset: 0x00011240
		private void RefreshMissingFormations()
		{
			if (this.IsPlayerGeneral)
			{
				List<OrderOfBattleFormationClassVM> allClasses = new List<OrderOfBattleFormationClassVM>();
				this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM formation)
				{
					allClasses.AddRange(formation.Classes.Where((OrderOfBattleFormationClassVM fc) => fc.Class != FormationClass.NumberOfAllFormations));
				});
				bool flag = false;
				using (List<FormationClass>.Enumerator enumerator = this._availableTroopTypes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FormationClass availableTroopType = enumerator.Current;
						if (allClasses.All((OrderOfBattleFormationClassVM c) => c.Class != availableTroopType))
						{
							if (Mission.Current.IsSiegeBattle)
							{
								if (availableTroopType != FormationClass.HorseArcher && availableTroopType != FormationClass.Cavalry)
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
							if (flag)
							{
								this.MissingFormationsHint.HintText.SetTextVariable("FORMATION_CLASS", availableTroopType.GetLocalizedName());
								this.CanStartMission = false;
								break;
							}
						}
					}
				}
				this.CanStartMission = !flag;
			}
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00013144 File Offset: 0x00011344
		private OrderOfBattleFormationItemVM GetFormationItemAtIndex(int index)
		{
			if (index < this.TotalFormationCount / 2)
			{
				return this.FormationsFirstHalf.ElementAt(index);
			}
			if (index < this.TotalFormationCount)
			{
				return this.FormationsSecondHalf.ElementAt(index - this.TotalFormationCount / 2);
			}
			return null;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0001317E File Offset: 0x0001137E
		private IEnumerable<OrderOfBattleFormationItemVM> GetFormationItemsWithCondition(Func<OrderOfBattleFormationItemVM, bool> condition)
		{
			return this._allFormations.Where(condition);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0001318C File Offset: 0x0001138C
		private void OnSelectedFormationsChanged()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.IsSelected = false;
			});
			using (List<Formation>.Enumerator enumerator = this._orderController.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation selectedFormation = enumerator.Current;
					this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Formation == selectedFormation).IsSelected = true;
				}
			}
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00013238 File Offset: 0x00011438
		private void SelectFormationItem(OrderOfBattleFormationItemVM formationItem)
		{
			formationItem.IsSelected = true;
			this._selectFormationAtIndex(formationItem.Formation.Index);
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00013258 File Offset: 0x00011458
		private void DeselectFormationItem(OrderOfBattleFormationItemVM formationItem)
		{
			Formation formation = formationItem.Formation;
			if (formation != null && formation.Index >= 0)
			{
				Mission.Current.PlayerTeam.PlayerOrderController.DeselectFormation(formationItem.Formation);
				Action<int> deselectFormationAtIndex = this._deselectFormationAtIndex;
				if (deselectFormationAtIndex == null)
				{
					return;
				}
				deselectFormationAtIndex(formationItem.Formation.Index);
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000132B4 File Offset: 0x000114B4
		public void SelectFormationItemAtIndex(int index)
		{
			this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Formation.Index == index).IsSelected = true;
			this._selectFormationAtIndex(index);
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x000132FC File Offset: 0x000114FC
		public void FocusFormationItemAtIndex(int index)
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.IsBeingFocused = false;
			});
			this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Formation.Index == index).IsBeingFocused = true;
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00013360 File Offset: 0x00011560
		public void DeselectAllFormations()
		{
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				orderOfBattleFormationItemVM.IsSelected = false;
			}
			Action clearFormationSelection = this._clearFormationSelection;
			if (clearFormationSelection == null)
			{
				return;
			}
			clearFormationSelection();
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x000133C4 File Offset: 0x000115C4
		public void OnUnitDeployed()
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				if (f != null)
				{
					f.RefreshMarkerWorldPosition();
				}
			});
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000133F0 File Offset: 0x000115F0
		public bool OnEscape()
		{
			if (this._allFormations.Any((OrderOfBattleFormationItemVM f) => f.IsSelected))
			{
				this.DeselectAllFormations();
				return true;
			}
			return false;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00013428 File Offset: 0x00011628
		private int GetTroopCountWithFilter(DeploymentFormationClass orderOfBattleFormationClass, FormationFilterType filterType)
		{
			int num = 0;
			List<FormationClass> formationClasses = orderOfBattleFormationClass.GetFormationClasses();
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				List<FormationClass> list = (from c in orderOfBattleFormationItemVM.Classes
					select c.Class into c
					where c != FormationClass.NumberOfAllFormations
					select c).ToList<FormationClass>();
				if (formationClasses.Intersect(list).Any<FormationClass>())
				{
					switch (filterType)
					{
					case FormationFilterType.Shield:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.HasShieldCached);
						break;
					case FormationFilterType.Spear:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.HasSpearCached);
						break;
					case FormationFilterType.Thrown:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.HasThrownCached);
						break;
					case FormationFilterType.Heavy:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(a));
						break;
					case FormationFilterType.HighTier:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.Character.GetBattleTier() >= 4);
						break;
					case FormationFilterType.LowTier:
						num += orderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.Character.GetBattleTier() <= 3);
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00013640 File Offset: 0x00011840
		protected void ClearFormationItem(OrderOfBattleFormationItemVM formationItem)
		{
			formationItem.FormationClassSelector.SelectedIndex = 0;
			formationItem.UnassignCommander();
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in formationItem.Classes)
			{
				orderOfBattleFormationClassVM.IsLocked = false;
				orderOfBattleFormationClassVM.Weight = 0;
				orderOfBattleFormationClassVM.Class = FormationClass.NumberOfAllFormations;
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x000136AC File Offset: 0x000118AC
		private int GetVisibleTotalTroopCountOfType(FormationClass formationClass)
		{
			return this._visibleTroopTypeCountLookup[formationClass];
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x000136BA File Offset: 0x000118BA
		private void OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM x)
			{
				x.RefreshMarkerWorldPosition();
			});
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x000136E6 File Offset: 0x000118E6
		private void OnHeroesChanged()
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.OnSizeChanged();
				f.UpdateAdjustable();
			});
			this.RefreshWeights();
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00013718 File Offset: 0x00011918
		public void ExecuteAutoDeploy()
		{
			if (this.IsPlayerGeneral)
			{
				this._onAutoDeploy();
				this.AfterAutoDeploy();
			}
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00013733 File Offset: 0x00011933
		private void AfterAutoDeploy()
		{
			this.RefreshWeights();
			this.OnUnitDeployed();
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.UpdateAdjustable();
			});
			this._isMissingFormationsDirty = true;
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00013774 File Offset: 0x00011974
		public void ExecuteBeginMission()
		{
			List<ValueTuple<int, List<int>>> currentConfiguration = this.CurrentConfiguration;
			if (currentConfiguration != null)
			{
				currentConfiguration.Clear();
			}
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in this._allFormations)
			{
				List<ValueTuple<int, List<int>>> currentConfiguration2 = this.CurrentConfiguration;
				if (currentConfiguration2 != null)
				{
					currentConfiguration2.Add(new ValueTuple<int, List<int>>(orderOfBattleFormationItemVM.Formation.Index, orderOfBattleFormationItemVM.ActiveFilterItems.Select((OrderOfBattleFormationFilterSelectorItemVM f) => (int)f.FilterType).ToList<int>()));
				}
			}
			if (this._bannerBearerLogic != null)
			{
				this._bannerBearerLogic.OnBannerBearersUpdated -= this.OnBannerBearersUpdated;
			}
			Action onBeginMission = this._onBeginMission;
			if (onBeginMission != null)
			{
				onBeginMission();
			}
			MBInformationManager.HideInformations();
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x00013858 File Offset: 0x00011A58
		// (set) Token: 0x06000421 RID: 1057 RVA: 0x00013860 File Offset: 0x00011A60
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x0001387E File Offset: 0x00011A7E
		// (set) Token: 0x06000423 RID: 1059 RVA: 0x00013886 File Offset: 0x00011A86
		[DataSourceProperty]
		public bool IsPlayerGeneral
		{
			get
			{
				return this._isPlayerGeneral;
			}
			set
			{
				if (value != this._isPlayerGeneral)
				{
					this._isPlayerGeneral = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerGeneral");
				}
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x000138A4 File Offset: 0x00011AA4
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x000138AC File Offset: 0x00011AAC
		[DataSourceProperty]
		public bool IsPoolAcceptingCommander
		{
			get
			{
				return this._isPoolAcceptingCommander;
			}
			set
			{
				if (value != this._isPoolAcceptingCommander)
				{
					this._isPoolAcceptingCommander = value;
					base.OnPropertyChangedWithValue(value, "IsPoolAcceptingCommander");
				}
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x000138CA File Offset: 0x00011ACA
		// (set) Token: 0x06000427 RID: 1063 RVA: 0x000138D2 File Offset: 0x00011AD2
		[DataSourceProperty]
		public bool AreCameraControlsEnabled
		{
			get
			{
				return this._areCameraControlsEnabled;
			}
			set
			{
				if (value != this._areCameraControlsEnabled)
				{
					this._areCameraControlsEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreCameraControlsEnabled");
				}
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x000138F0 File Offset: 0x00011AF0
		// (set) Token: 0x06000429 RID: 1065 RVA: 0x000138F8 File Offset: 0x00011AF8
		[DataSourceProperty]
		public bool IsPoolAcceptingHeroTroops
		{
			get
			{
				return this._isPoolAcceptingHeroTroops;
			}
			set
			{
				if (value != this._isPoolAcceptingHeroTroops)
				{
					this._isPoolAcceptingHeroTroops = value;
					base.OnPropertyChangedWithValue(value, "IsPoolAcceptingHeroTroops");
				}
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x00013916 File Offset: 0x00011B16
		// (set) Token: 0x0600042B RID: 1067 RVA: 0x0001391E File Offset: 0x00011B1E
		[DataSourceProperty]
		public bool CanStartMission
		{
			get
			{
				return this._canStartMission;
			}
			set
			{
				if (value != this._canStartMission)
				{
					this._canStartMission = value;
					base.OnPropertyChangedWithValue(value, "CanStartMission");
				}
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x0001393C File Offset: 0x00011B3C
		// (set) Token: 0x0600042D RID: 1069 RVA: 0x00013944 File Offset: 0x00011B44
		[DataSourceProperty]
		public bool HasSelectedHeroes
		{
			get
			{
				return this._hasSelectedHeroes;
			}
			set
			{
				if (value != this._hasSelectedHeroes)
				{
					this._hasSelectedHeroes = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedHeroes");
				}
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x00013962 File Offset: 0x00011B62
		// (set) Token: 0x0600042F RID: 1071 RVA: 0x0001396A File Offset: 0x00011B6A
		[DataSourceProperty]
		public int SelectedHeroCount
		{
			get
			{
				return this._selectedHeroCount;
			}
			set
			{
				if (value != this._selectedHeroCount)
				{
					this._selectedHeroCount = value;
					base.OnPropertyChangedWithValue(value, "SelectedHeroCount");
				}
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x00013988 File Offset: 0x00011B88
		// (set) Token: 0x06000431 RID: 1073 RVA: 0x00013990 File Offset: 0x00011B90
		[DataSourceProperty]
		public string BeginMissionText
		{
			get
			{
				return this._beginMissionText;
			}
			set
			{
				if (value != this._beginMissionText)
				{
					this._beginMissionText = value;
					base.OnPropertyChangedWithValue<string>(value, "BeginMissionText");
				}
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x000139B3 File Offset: 0x00011BB3
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x000139BB File Offset: 0x00011BBB
		[DataSourceProperty]
		public string AutoDeployText
		{
			get
			{
				return this._autoDeployText;
			}
			set
			{
				if (value != this._autoDeployText)
				{
					this._autoDeployText = value;
					base.OnPropertyChangedWithValue<string>(value, "AutoDeployText");
				}
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x000139DE File Offset: 0x00011BDE
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x000139E6 File Offset: 0x00011BE6
		[DataSourceProperty]
		public HintViewModel ClearSelectionHint
		{
			get
			{
				return this._clearSelectionHint;
			}
			set
			{
				if (value != this._clearSelectionHint)
				{
					this._clearSelectionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ClearSelectionHint");
				}
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00013A04 File Offset: 0x00011C04
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x00013A0C File Offset: 0x00011C0C
		[DataSourceProperty]
		public HintViewModel SelectAllHint
		{
			get
			{
				return this._selectAllHint;
			}
			set
			{
				if (value != this._selectAllHint)
				{
					this._selectAllHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SelectAllHint");
				}
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00013A2A File Offset: 0x00011C2A
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x00013A32 File Offset: 0x00011C32
		[DataSourceProperty]
		public HintViewModel MissingFormationsHint
		{
			get
			{
				return this._missingFormationsHint;
			}
			set
			{
				if (value != this._missingFormationsHint)
				{
					this._missingFormationsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "MissingFormationsHint");
				}
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00013A50 File Offset: 0x00011C50
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x00013A58 File Offset: 0x00011C58
		[DataSourceProperty]
		public OrderOfBattleHeroItemVM LastSelectedHeroItem
		{
			get
			{
				return this._lastSelectedHeroItem;
			}
			set
			{
				if (value != this._lastSelectedHeroItem)
				{
					this._lastSelectedHeroItem = value;
					base.OnPropertyChangedWithValue<OrderOfBattleHeroItemVM>(value, "LastSelectedHeroItem");
				}
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00013A76 File Offset: 0x00011C76
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x00013A7E File Offset: 0x00011C7E
		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationItemVM> FormationsFirstHalf
		{
			get
			{
				return this._formationsFirstHalf;
			}
			set
			{
				if (value != this._formationsFirstHalf)
				{
					this._formationsFirstHalf = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationItemVM>>(value, "FormationsFirstHalf");
				}
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00013A9C File Offset: 0x00011C9C
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x00013AA4 File Offset: 0x00011CA4
		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationItemVM> FormationsSecondHalf
		{
			get
			{
				return this._formationsSecondHalf;
			}
			set
			{
				if (value != this._formationsSecondHalf)
				{
					this._formationsSecondHalf = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationItemVM>>(value, "FormationsSecondHalf");
				}
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00013AC2 File Offset: 0x00011CC2
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00013ACA File Offset: 0x00011CCA
		[DataSourceProperty]
		public MBBindingList<OrderOfBattleHeroItemVM> UnassignedHeroes
		{
			get
			{
				return this._unassignedHeroes;
			}
			set
			{
				if (value != this._unassignedHeroes)
				{
					this._unassignedHeroes = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleHeroItemVM>>(value, "UnassignedHeroes");
				}
			}
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x00013AE8 File Offset: 0x00011CE8
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					if (this._isAssignCaptainHighlightApplied)
					{
						this.SetHighlightEmptyCaptainFormations(false);
						this.SetHighlightMainAgentPortait(false);
						this._isAssignCaptainHighlightApplied = false;
					}
					if (this._isCreateFormationHighlightApplied)
					{
						this.SetHighlightFormationTypeSelection(false);
						this.SetHighlightFormationWeights(false);
						this._isCreateFormationHighlightApplied = false;
					}
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					if (this._latestTutorialElementID == "AssignCaptain" && !this._isAssignCaptainHighlightApplied)
					{
						this.SetHighlightEmptyCaptainFormations(true);
						this.SetHighlightMainAgentPortait(true);
						this._isAssignCaptainHighlightApplied = true;
					}
					if (this._latestTutorialElementID == "CreateFormation" && !this._isCreateFormationHighlightApplied)
					{
						this.SetHighlightFormationTypeSelection(true);
						this.SetHighlightFormationWeights(true);
						this._isCreateFormationHighlightApplied = true;
					}
				}
			}
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00013BC0 File Offset: 0x00011DC0
		private void SetHighlightMainAgentPortait(bool state)
		{
			for (int i = 0; i < this._allHeroes.Count; i++)
			{
				OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._allHeroes[i];
				if (orderOfBattleHeroItemVM.Agent.IsMainAgent)
				{
					orderOfBattleHeroItemVM.IsHighlightActive = state;
					return;
				}
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x00013C08 File Offset: 0x00011E08
		private void SetHighlightEmptyCaptainFormations(bool state)
		{
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations[i];
				if (!state || (!orderOfBattleFormationItemVM.HasCommander && orderOfBattleFormationItemVM.HasFormation))
				{
					orderOfBattleFormationItemVM.IsCaptainSlotHighlightActive = state;
				}
			}
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00013C54 File Offset: 0x00011E54
		private void SetHighlightFormationTypeSelection(bool state)
		{
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations[i];
				if (!state || orderOfBattleFormationItemVM.IsAdjustable)
				{
					this._allFormations[i].IsTypeSelectionHighlightActive = state;
				}
			}
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00013CA4 File Offset: 0x00011EA4
		private void SetHighlightFormationWeights(bool state)
		{
			for (int i = 0; i < this._allFormations.Count; i++)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = this._allFormations[i];
				for (int j = 0; j < orderOfBattleFormationItemVM.Classes.Count; j++)
				{
					orderOfBattleFormationItemVM.Classes[j].IsWeightHighlightActive = state;
				}
			}
		}

		// Token: 0x040001F0 RID: 496
		private readonly TextObject _bannerText = new TextObject("{=FvYhaE3z}Banner", null);

		// Token: 0x040001F1 RID: 497
		private readonly TextObject _bannerEffectText = new TextObject("{=zjcZZgUY}Banner Effect", null);

		// Token: 0x040001F2 RID: 498
		private readonly TextObject _noBannerEquippedText = new TextObject("{=suyl7WWa}No banner equipped", null);

		// Token: 0x040001F3 RID: 499
		private readonly TextObject _missingFormationsHintText = new TextObject("{=2AGvFYk9}To start the mission, you need to have at least one formation with {FORMATION_CLASS} class.", null);

		// Token: 0x040001F4 RID: 500
		private readonly TextObject _selectAllHintText = new TextObject("{=YwbymaBc}Select all heroes", null);

		// Token: 0x040001F5 RID: 501
		private readonly TextObject _clearSelectionHintText = new TextObject("{=Sbb8YcJM}Deselect all selected heroes", null);

		// Token: 0x040001F6 RID: 502
		private readonly List<OrderOfBattleHeroItemVM> _selectedHeroes;

		// Token: 0x040001F7 RID: 503
		protected readonly List<OrderOfBattleHeroItemVM> _allHeroes;

		// Token: 0x040001F8 RID: 504
		private bool _isInitialized;

		// Token: 0x040001F9 RID: 505
		private bool _isSaving;

		// Token: 0x040001FC RID: 508
		protected List<OrderOfBattleFormationItemVM> _allFormations;

		// Token: 0x040001FD RID: 509
		private List<FormationClass> _availableTroopTypes;

		// Token: 0x040001FE RID: 510
		private bool _isUnitDeployRefreshed;

		// Token: 0x040001FF RID: 511
		private Dictionary<FormationClass, int> _visibleTroopTypeCountLookup;

		// Token: 0x04000200 RID: 512
		private Action<int> _selectFormationAtIndex;

		// Token: 0x04000201 RID: 513
		private Action<int> _deselectFormationAtIndex;

		// Token: 0x04000202 RID: 514
		private Action _clearFormationSelection;

		// Token: 0x04000203 RID: 515
		private Action _onAutoDeploy;

		// Token: 0x04000204 RID: 516
		private Action _onBeginMission;

		// Token: 0x04000205 RID: 517
		private Mission _mission;

		// Token: 0x04000206 RID: 518
		private Camera _missionCamera;

		// Token: 0x04000207 RID: 519
		private BannerBearerLogic _bannerBearerLogic;

		// Token: 0x04000208 RID: 520
		private OrderController _orderController;

		// Token: 0x04000209 RID: 521
		private bool _isMissingFormationsDirty;

		// Token: 0x0400020A RID: 522
		private bool _isHeroSelectionDirty;

		// Token: 0x0400020B RID: 523
		private bool _isTroopCountsDirty;

		// Token: 0x0400020C RID: 524
		private OrderOfBattleFormationItemVM _lastEnabledClassSelection;

		// Token: 0x0400020D RID: 525
		private OrderOfBattleFormationItemVM _lastEnabledFilterSelection;

		// Token: 0x0400020E RID: 526
		private bool _isEnabled;

		// Token: 0x0400020F RID: 527
		private bool _isPlayerGeneral;

		// Token: 0x04000210 RID: 528
		private bool _areCameraControlsEnabled;

		// Token: 0x04000211 RID: 529
		private bool _canStartMission = true;

		// Token: 0x04000212 RID: 530
		private bool _isPoolAcceptingCommander;

		// Token: 0x04000213 RID: 531
		private bool _isPoolAcceptingHeroTroops;

		// Token: 0x04000214 RID: 532
		private bool _hasSelectedHeroes;

		// Token: 0x04000215 RID: 533
		private int _selectedHeroCount;

		// Token: 0x04000216 RID: 534
		private MBBindingList<OrderOfBattleFormationItemVM> _formationsSecondHalf;

		// Token: 0x04000217 RID: 535
		private string _beginMissionText;

		// Token: 0x04000218 RID: 536
		private string _autoDeployText;

		// Token: 0x04000219 RID: 537
		private HintViewModel _missingFormationsHint;

		// Token: 0x0400021A RID: 538
		private MBBindingList<OrderOfBattleHeroItemVM> _unassignedHeroes;

		// Token: 0x0400021B RID: 539
		private HintViewModel _selectAllHint;

		// Token: 0x0400021C RID: 540
		private HintViewModel _clearSelectionHint;

		// Token: 0x0400021D RID: 541
		private MBBindingList<OrderOfBattleFormationItemVM> _formationsFirstHalf;

		// Token: 0x0400021E RID: 542
		private OrderOfBattleHeroItemVM _lastSelectedHeroItem;

		// Token: 0x0400021F RID: 543
		private string _latestTutorialElementID;

		// Token: 0x04000220 RID: 544
		private const string _assignCaptainHighlightID = "AssignCaptain";

		// Token: 0x04000221 RID: 545
		private const string _createFormationHighlightID = "CreateFormation";

		// Token: 0x04000222 RID: 546
		private bool _isAssignCaptainHighlightApplied;

		// Token: 0x04000223 RID: 547
		private bool _isCreateFormationHighlightApplied;
	}
}

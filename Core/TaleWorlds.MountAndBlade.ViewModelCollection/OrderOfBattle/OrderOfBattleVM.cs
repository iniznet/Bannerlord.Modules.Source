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
	public class OrderOfBattleVM : ViewModel
	{
		protected int TotalFormationCount
		{
			get
			{
				return this._mission.PlayerTeam.FormationsIncludingEmpty.Count;
			}
		}

		public List<ValueTuple<int, List<int>>> CurrentConfiguration { get; private set; }

		public bool IsOrderPreconfigured { get; protected set; }

		public OrderOfBattleVM()
		{
			this._allFormations = new List<OrderOfBattleFormationItemVM>();
			this._allHeroes = new List<OrderOfBattleHeroItemVM>();
			this._selectedHeroes = new List<OrderOfBattleHeroItemVM>();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.FinalizeFormationCallbacks();
		}

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

		[Conditional("DEBUG")]
		private void EnsureAllFormationPercentagesAreValid()
		{
		}

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
						func = (<>9__2 = (OrderOfBattleFormationClassVM fc) => fc.Class == f.Formation.PhysicalClass);
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
				this._bannerBearerLogic.OnBannerBearerAgentUpdated += this.OnBannerAgentUpdated;
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

		private void OnBannerAgentUpdated(Agent agent, bool isBannerBearer)
		{
			if (this._isInitialized && (agent.Team.IsPlayerTeam || agent.Team.IsPlayerAlly) && this._orderController.SelectedFormations.Contains(agent.Formation))
			{
				this._orderController.DeselectFormation(agent.Formation);
				this._orderController.SelectFormation(agent.Formation);
			}
		}

		private OrderOfBattleFormationItemVM GetFirstAvailableFormationWithAnyClass(params FormationClass[] classes)
		{
			OrderOfBattleVM.<>c__DisplayClass51_0 CS$<>8__locals1 = new OrderOfBattleVM.<>c__DisplayClass51_0();
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
						Debug.FailedAssert("Failed to find an initial formation for hero: " + this._allHeroes[i].Agent.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleVM.cs", "SetInitialHeroFormations", 639);
					}
				}
			}
		}

		protected virtual void LoadConfiguration()
		{
		}

		protected virtual void SaveConfiguration()
		{
		}

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

		private bool HasAnyTroopWithClass(FormationClass formationClass)
		{
			return this._availableTroopTypes.Contains(formationClass);
		}

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

		public void OnAllFormationsAssignedSergeants(Dictionary<int, Agent> preAssignedCommanders)
		{
			foreach (KeyValuePair<int, Agent> keyValuePair in preAssignedCommanders)
			{
				this.AssignCommander(keyValuePair.Value, this._allFormations[keyValuePair.Key]);
			}
		}

		private void OnClassSelectionToggled(OrderOfBattleFormationItemVM formationItem)
		{
			if (formationItem != null && formationItem.IsClassSelectionActive)
			{
				this._lastEnabledClassSelection = formationItem;
				return;
			}
			this._lastEnabledClassSelection = null;
		}

		private void OnFilterSelectionToggled(OrderOfBattleFormationItemVM formationItem)
		{
			if (formationItem != null && formationItem.IsFilterSelectionActive)
			{
				this._lastEnabledFilterSelection = formationItem;
				return;
			}
			this._lastEnabledFilterSelection = null;
		}

		public bool IsAnyClassSelectionEnabled()
		{
			return this._lastEnabledClassSelection != null;
		}

		public bool IsAnyFilterSelectionEnabled()
		{
			return this._lastEnabledFilterSelection != null;
		}

		public void ExecuteDisableAllClassSelections()
		{
			if (this._lastEnabledClassSelection != null)
			{
				this._lastEnabledClassSelection.IsClassSelectionActive = false;
				this._lastEnabledClassSelection = null;
			}
		}

		public void ExecuteDisableAllFilterSelections()
		{
			if (this._lastEnabledFilterSelection != null)
			{
				this._lastEnabledFilterSelection.IsFilterSelectionActive = false;
				this._lastEnabledFilterSelection = null;
			}
		}

		private void SelectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			if (!this._selectedHeroes.Contains(heroItem))
			{
				heroItem.IsSelected = true;
				this._selectedHeroes.Add(heroItem);
				this.UpdateHeroItemSelection();
			}
		}

		private void DeselectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			heroItem.IsSelected = false;
			this._selectedHeroes.Remove(heroItem);
			this.UpdateHeroItemSelection();
		}

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

		private void OnHeroAssignmentBegin(OrderOfBattleHeroItemVM heroItem)
		{
			this.SelectHeroItem(heroItem);
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsShown = false;
			});
		}

		private void OnHeroAssignmentEnd(OrderOfBattleHeroItemVM heroItem)
		{
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsShown = true;
			});
			this.UpdateHeroItemSelection();
		}

		private void ClearAndSelectHeroItem(OrderOfBattleHeroItemVM heroItem)
		{
			this.ClearHeroItemSelection();
			this.SelectHeroItem(heroItem);
		}

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

		protected void AssignCommander(Agent agent, OrderOfBattleFormationItemVM formationItem)
		{
			OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._allHeroes.FirstOrDefault((OrderOfBattleHeroItemVM h) => h.Agent == agent);
			if (formationItem != null && orderOfBattleHeroItemVM != null && formationItem.Commander != orderOfBattleHeroItemVM)
			{
				if (formationItem.HasCommander)
				{
					formationItem.Commander.IsSelected = false;
					formationItem.UnassignCommander();
				}
				formationItem.Commander = orderOfBattleHeroItemVM;
			}
		}

		private void ClearHeroItemSelection()
		{
			this._selectedHeroes.ForEach(delegate(OrderOfBattleHeroItemVM hero)
			{
				hero.IsSelected = false;
			});
			this._selectedHeroes.Clear();
			this.UpdateHeroItemSelection();
		}

		public void ExecuteAcceptHeroes()
		{
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in this._selectedHeroes)
			{
				this.ClearHeroAssignment(orderOfBattleHeroItemVM);
				orderOfBattleHeroItemVM.IsShown = true;
			}
			this.ClearHeroItemSelection();
		}

		public void ExecuteSelectAllHeroes()
		{
			this.ClearHeroItemSelection();
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in this.UnassignedHeroes)
			{
				this.SelectHeroItem(orderOfBattleHeroItemVM);
			}
		}

		public void ExecuteClearHeroSelection()
		{
			this.ClearHeroItemSelection();
		}

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

		private bool CanAdjustWeight(OrderOfBattleFormationClassVM formationClass)
		{
			return this._isInitialized && OrderOfBattleUIHelper.GetMatchingClasses(this._allFormations, formationClass, null).Count > 1;
		}

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

		private void DistributeTroops(OrderOfBattleFormationClassVM formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> massTransferDataForFormation = this.GetMassTransferDataForFormation(formationClass);
			if (massTransferDataForFormation.Count > 0)
			{
				this._orderController.RearrangeFormationsAccordingToFilters(Agent.Main.Team, massTransferDataForFormation);
				this.RefreshFormationsWithClass(formationClass.Class);
			}
		}

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
					Debug.FailedAssert("Failed to sum up all weights to 100", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleVM.cs", "DistributeWeights", 1178);
					break;
				}
			}
			matchingClasses.ForEach(delegate(OrderOfBattleFormationClassVM formation)
			{
				formation.SetWeightAdjustmentLock(false);
			});
		}

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

		private void TransferAllAvailableTroopsToFormation(OrderOfBattleFormationItemVM formation, FormationClass formationClass)
		{
			List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> massTransferDataForFormationClass = this.GetMassTransferDataForFormationClass(formation.Formation, formationClass);
			if (massTransferDataForFormationClass.Count > 0)
			{
				this._orderController.RearrangeFormationsAccordingToFilters(Agent.Main.Team, massTransferDataForFormationClass);
				this.RefreshFormationsWithClass(formationClass);
			}
		}

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

		private IEnumerable<OrderOfBattleFormationItemVM> GetFormationItemsWithCondition(Func<OrderOfBattleFormationItemVM, bool> condition)
		{
			return this._allFormations.Where(condition);
		}

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

		private void SelectFormationItem(OrderOfBattleFormationItemVM formationItem)
		{
			formationItem.IsSelected = true;
			this._selectFormationAtIndex(formationItem.Formation.Index);
		}

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

		public void SelectFormationItemAtIndex(int index)
		{
			this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Formation.Index == index).IsSelected = true;
			this._selectFormationAtIndex(index);
		}

		public void FocusFormationItemAtIndex(int index)
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.IsBeingFocused = false;
			});
			this._allFormations.FirstOrDefault((OrderOfBattleFormationItemVM f) => f.Formation.Index == index).IsBeingFocused = true;
		}

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

		public bool OnEscape()
		{
			if (this._allFormations.Any((OrderOfBattleFormationItemVM f) => f.IsSelected))
			{
				this.DeselectAllFormations();
				return true;
			}
			return false;
		}

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

		private int GetVisibleTotalTroopCountOfType(FormationClass formationClass)
		{
			return this._visibleTroopTypeCountLookup[formationClass];
		}

		private void OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM x)
			{
				x.RefreshMarkerWorldPosition();
			});
		}

		private void OnHeroesChanged()
		{
			this._allFormations.ForEach(delegate(OrderOfBattleFormationItemVM f)
			{
				f.OnSizeChanged();
				f.UpdateAdjustable();
			});
			this.RefreshWeights();
		}

		public void ExecuteAutoDeploy()
		{
			if (this.IsPlayerGeneral)
			{
				this._onAutoDeploy();
				this.AfterAutoDeploy();
			}
		}

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
				this._bannerBearerLogic.OnBannerBearerAgentUpdated -= this.OnBannerAgentUpdated;
			}
			Action onBeginMission = this._onBeginMission;
			if (onBeginMission != null)
			{
				onBeginMission();
			}
			MBInformationManager.HideInformations();
		}

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

		private readonly TextObject _bannerText = new TextObject("{=FvYhaE3z}Banner", null);

		private readonly TextObject _bannerEffectText = new TextObject("{=zjcZZgUY}Banner Effect", null);

		private readonly TextObject _noBannerEquippedText = new TextObject("{=suyl7WWa}No banner equipped", null);

		private readonly TextObject _missingFormationsHintText = new TextObject("{=2AGvFYk9}To start the mission, you need to have at least one formation with {FORMATION_CLASS} class.", null);

		private readonly TextObject _selectAllHintText = new TextObject("{=YwbymaBc}Select all heroes", null);

		private bool _isSaving;

		private readonly TextObject _clearSelectionHintText = new TextObject("{=Sbb8YcJM}Deselect all selected heroes", null);

		private Dictionary<FormationClass, int> _visibleTroopTypeCountLookup;

		private bool _isUnitDeployRefreshed;

		private Action<int> _selectFormationAtIndex;

		private readonly List<OrderOfBattleHeroItemVM> _selectedHeroes;

		private Action<int> _deselectFormationAtIndex;

		protected readonly List<OrderOfBattleHeroItemVM> _allHeroes;

		private List<FormationClass> _availableTroopTypes;

		private bool _isInitialized;

		protected List<OrderOfBattleFormationItemVM> _allFormations;

		private Action _clearFormationSelection;

		private Action _onAutoDeploy;

		private Action _onBeginMission;

		private Mission _mission;

		private Camera _missionCamera;

		private BannerBearerLogic _bannerBearerLogic;

		private OrderController _orderController;

		private bool _isMissingFormationsDirty;

		private bool _isHeroSelectionDirty;

		private bool _isTroopCountsDirty;

		private OrderOfBattleFormationItemVM _lastEnabledClassSelection;

		private OrderOfBattleFormationItemVM _lastEnabledFilterSelection;

		private bool _isEnabled;

		private bool _isPlayerGeneral;

		private bool _areCameraControlsEnabled;

		private bool _canStartMission = true;

		private bool _isPoolAcceptingCommander;

		private bool _isPoolAcceptingHeroTroops;

		private string _beginMissionText;

		private bool _hasSelectedHeroes;

		private int _selectedHeroCount;

		private MBBindingList<OrderOfBattleFormationItemVM> _formationsSecondHalf;

		private HintViewModel _missingFormationsHint;

		private HintViewModel _selectAllHint;

		private HintViewModel _clearSelectionHint;

		private string _autoDeployText;

		private MBBindingList<OrderOfBattleHeroItemVM> _unassignedHeroes;

		private OrderOfBattleHeroItemVM _lastSelectedHeroItem;

		private MBBindingList<OrderOfBattleFormationItemVM> _formationsFirstHalf;

		private string _latestTutorialElementID;

		private const string _assignCaptainHighlightID = "AssignCaptain";

		private const string _createFormationHighlightID = "CreateFormation";

		private bool _isAssignCaptainHighlightApplied;

		private bool _isCreateFormationHighlightApplied;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleFormationItemVM : ViewModel
	{
		public Formation Formation { get; private set; }

		public OrderOfBattleFormationItemVM(Camera missionCamera)
		{
			this._missionCamera = missionCamera;
			this.Formation = null;
			this._bannerBearerLogic = Mission.Current.GetMissionBehavior<BannerBearerLogic>();
			this.HasFormation = false;
			this.FilterItems = new MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>();
			this.ActiveFilterItems = new MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>();
			for (FormationFilterType formationFilterType = FormationFilterType.Shield; formationFilterType < FormationFilterType.NumberOfFilterTypes; formationFilterType++)
			{
				this.FilterItems.Add(new OrderOfBattleFormationFilterSelectorItemVM(formationFilterType, new Action<OrderOfBattleFormationFilterSelectorItemVM>(this.OnFilterToggled)));
			}
			this.FormationClassSelector = new SelectorVM<OrderOfBattleFormationClassSelectorItemVM>(0, new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChanged));
			for (DeploymentFormationClass deploymentFormationClass = DeploymentFormationClass.Unset; deploymentFormationClass <= DeploymentFormationClass.CavalryAndHorseArcher; deploymentFormationClass++)
			{
				if (!Mission.Current.IsSiegeBattle || (deploymentFormationClass != DeploymentFormationClass.Cavalry && deploymentFormationClass != DeploymentFormationClass.HorseArcher && deploymentFormationClass != DeploymentFormationClass.CavalryAndHorseArcher))
				{
					this.FormationClassSelector.AddItem(new OrderOfBattleFormationClassSelectorItemVM(deploymentFormationClass));
				}
			}
			this.Classes = new MBBindingList<OrderOfBattleFormationClassVM>
			{
				new OrderOfBattleFormationClassVM(this, FormationClass.NumberOfAllFormations),
				new OrderOfBattleFormationClassVM(this, FormationClass.NumberOfAllFormations)
			};
			this.HeroTroops = new MBBindingList<OrderOfBattleHeroItemVM>();
			this._unassignedCommander = new OrderOfBattleHeroItemVM();
			this.Commander = this._unassignedCommander;
			this.Tooltip = new BasicTooltipViewModel(() => this.GetTooltip());
			this.IsControlledByPlayer = Mission.Current.PlayerTeam.IsPlayerGeneral;
			this._worldPosition = Vec3.Zero;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FormationIsEmptyText = new TextObject("{=P3IWytsr}Formation is currently empty", null).ToString();
			this.CommanderSlotHint = new HintViewModel(this._commanderSlotHintText, null);
			this.HeroTroopSlotHint = new HintViewModel(this._heroTroopSlotHintText, null);
			this.AssignCommanderHint = new HintViewModel(this._assignCommanderHintText, null);
			this.AssignHeroTroopHint = new HintViewModel(this._assignHeroTroopHintText, null);
		}

		public void Tick()
		{
			this.Classes.ApplyActionOnAllItems(delegate(OrderOfBattleFormationClassVM c)
			{
				c.UpdateWeightAdjustable();
			});
			this.UpdateAdjustable();
			bool flag;
			if (this.Formation.CountOfUnits != 0)
			{
				flag = this.Classes.Any((OrderOfBattleFormationClassVM c) => c.Class != FormationClass.NumberOfAllFormations);
			}
			else
			{
				flag = false;
			}
			this.IsMarkerShown = flag;
			if (!this.IsMarkerShown)
			{
				return;
			}
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(this._missionCamera, this._worldPosition, ref this._latestX, ref this._latestY, ref this._latestW);
			this.ScreenPosition = new Vec2(this._latestX, this._latestY);
			this._wPosAfterPositionCalculation = ((this._latestW < 0f) ? (-1f) : 1.1f);
			this.WSign = (int)this._wPosAfterPositionCalculation;
		}

		public void RefreshFormation(Formation formation, DeploymentFormationClass overriddenClass = DeploymentFormationClass.Unset, bool mustExist = false)
		{
			this.Formation = formation;
			if (formation.CountOfUnits != 0 || mustExist)
			{
				DeploymentFormationClass formationTypeToSet = DeploymentFormationClass.Unset;
				if (overriddenClass != DeploymentFormationClass.Unset)
				{
					formationTypeToSet = overriddenClass;
				}
				else
				{
					FormationClass formationClass = formation.SecondaryLogicalClasses.FirstOrDefault<FormationClass>();
					if (formation.GetCountOfUnitsBelongingToLogicalClass(formationClass) == 0)
					{
						formationClass = FormationClass.NumberOfAllFormations;
					}
					switch (formation.LogicalClass)
					{
					case FormationClass.Infantry:
						formationTypeToSet = ((formationClass == FormationClass.Ranged) ? DeploymentFormationClass.InfantryAndRanged : DeploymentFormationClass.Infantry);
						break;
					case FormationClass.Ranged:
						formationTypeToSet = ((formationClass == FormationClass.Infantry) ? DeploymentFormationClass.InfantryAndRanged : DeploymentFormationClass.Ranged);
						break;
					case FormationClass.Cavalry:
						formationTypeToSet = ((formationClass == FormationClass.HorseArcher) ? DeploymentFormationClass.CavalryAndHorseArcher : DeploymentFormationClass.Cavalry);
						break;
					case FormationClass.HorseArcher:
						formationTypeToSet = ((formationClass == FormationClass.Cavalry) ? DeploymentFormationClass.CavalryAndHorseArcher : DeploymentFormationClass.HorseArcher);
						break;
					default:
						Debug.FailedAssert("Formation doesn't have a proper primary class. Value : " + formation.PhysicalClass.GetName(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleFormationItemVM.cs", "RefreshFormation", 171);
						break;
					}
				}
				OrderOfBattleFormationClassSelectorItemVM orderOfBattleFormationClassSelectorItemVM = this.FormationClassSelector.ItemList.SingleOrDefault((OrderOfBattleFormationClassSelectorItemVM i) => i.FormationClass == formationTypeToSet);
				int num = this.FormationClassSelector.ItemList.IndexOf(orderOfBattleFormationClassSelectorItemVM);
				if (num != -1)
				{
					this.FormationClassSelector.SelectedIndex = num;
				}
			}
			else
			{
				this.FormationClassSelector.SelectedIndex = 0;
			}
			this.TitleText = Common.ToRoman(this.Formation.Index + 1);
			this.OnSizeChanged();
		}

		public void RefreshMarkerWorldPosition()
		{
			if (this.Formation == null)
			{
				return;
			}
			Agent medianAgent = this.Formation.GetMedianAgent(false, false, this.Formation.GetAveragePositionOfUnits(false, false));
			if (medianAgent == null)
			{
				return;
			}
			this._worldPosition = medianAgent.GetWorldPosition().GetGroundVec3();
			this._worldPosition += new Vec3(0f, 0f, medianAgent.GetEyeGlobalHeight(), -1f);
		}

		public void OnSizeChanged()
		{
			Formation formation = this.Formation;
			this.TroopCount = ((formation != null) ? formation.CountOfUnits : 0);
			this.RefreshMarkerWorldPosition();
			this.IsSelectable = this.FormationClassSelector.SelectedIndex != 0 && this.IsControlledByPlayer && this.TroopCount > 0;
			if (!this.IsSelectable && this.IsSelected)
			{
				Action<OrderOfBattleFormationItemVM> onDeselection = OrderOfBattleFormationItemVM.OnDeselection;
				if (onDeselection != null)
				{
					onDeselection(this);
				}
			}
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in this.Classes)
			{
				orderOfBattleFormationClassVM.UpdateWeightText();
			}
			this.UpdateAdjustable();
		}

		private void OnClassChanged(SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector)
		{
			if (this.Classes == null)
			{
				return;
			}
			DeploymentFormationClass formationClass = formationClassSelector.SelectedItem.FormationClass;
			this.OrderOfBattleFormationClassInt = (int)formationClass;
			switch (formationClass)
			{
			case DeploymentFormationClass.Unset:
			{
				this.Classes[0].Class = FormationClass.NumberOfAllFormations;
				this.Classes[1].Class = FormationClass.NumberOfAllFormations;
				if (this.Commander != this._unassignedCommander)
				{
					this.UnassignCommander();
				}
				List<OrderOfBattleHeroItemVM> list = this.HeroTroops.ToList<OrderOfBattleHeroItemVM>();
				for (int i = 0; i < list.Count; i++)
				{
					this.RemoveHeroTroop(list[i]);
				}
				for (int j = this.ActiveFilterItems.Count - 1; j >= 0; j--)
				{
					this.ActiveFilterItems[j].IsActive = false;
				}
				break;
			}
			case DeploymentFormationClass.Infantry:
				this.Classes[0].Class = FormationClass.Infantry;
				this.Classes[1].Class = FormationClass.NumberOfAllFormations;
				break;
			case DeploymentFormationClass.Ranged:
				this.Classes[0].Class = FormationClass.Ranged;
				this.Classes[1].Class = FormationClass.NumberOfAllFormations;
				break;
			case DeploymentFormationClass.Cavalry:
				this.Classes[0].Class = FormationClass.Cavalry;
				this.Classes[1].Class = FormationClass.NumberOfAllFormations;
				break;
			case DeploymentFormationClass.HorseArcher:
				this.Classes[0].Class = FormationClass.HorseArcher;
				this.Classes[1].Class = FormationClass.NumberOfAllFormations;
				break;
			case DeploymentFormationClass.InfantryAndRanged:
				this.Classes[0].Class = FormationClass.Infantry;
				this.Classes[1].Class = FormationClass.Ranged;
				break;
			case DeploymentFormationClass.CavalryAndHorseArcher:
				this.Classes[0].Class = FormationClass.Cavalry;
				this.Classes[1].Class = FormationClass.HorseArcher;
				break;
			}
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in this.Classes)
			{
				orderOfBattleFormationClassVM.IsLocked = false;
				orderOfBattleFormationClassVM.Weight = 0;
			}
			this.HasFormation = this.Classes.Any((OrderOfBattleFormationClassVM c) => c.Class != FormationClass.NumberOfAllFormations);
			this.UpdateAdjustable();
		}

		public DeploymentFormationClass GetOrderOfBattleClass()
		{
			if (this.Classes[0].Class == FormationClass.Infantry && this.Classes[1].Class == FormationClass.NumberOfAllFormations)
			{
				return DeploymentFormationClass.Infantry;
			}
			if (this.Classes[0].Class == FormationClass.Ranged && this.Classes[1].Class == FormationClass.NumberOfAllFormations)
			{
				return DeploymentFormationClass.Ranged;
			}
			if (this.Classes[0].Class == FormationClass.Cavalry && this.Classes[1].Class == FormationClass.NumberOfAllFormations)
			{
				return DeploymentFormationClass.Cavalry;
			}
			if (this.Classes[0].Class == FormationClass.HorseArcher && this.Classes[1].Class == FormationClass.NumberOfAllFormations)
			{
				return DeploymentFormationClass.HorseArcher;
			}
			if (this.Classes[0].Class == FormationClass.Infantry && this.Classes[1].Class == FormationClass.Ranged)
			{
				return DeploymentFormationClass.InfantryAndRanged;
			}
			if (this.Classes[0].Class == FormationClass.Cavalry && this.Classes[1].Class == FormationClass.HorseArcher)
			{
				return DeploymentFormationClass.CavalryAndHorseArcher;
			}
			return DeploymentFormationClass.Unset;
		}

		private void OnFilterToggled(OrderOfBattleFormationFilterSelectorItemVM filterItem)
		{
			if (filterItem.IsActive)
			{
				this.ActiveFilterItems.Add(filterItem);
			}
			else
			{
				this.ActiveFilterItems.Remove(filterItem);
			}
			this.IsFiltered = this.ActiveFilterItems.Count > 0;
			Action<OrderOfBattleFormationItemVM> onFilterUseToggled = OrderOfBattleFormationItemVM.OnFilterUseToggled;
			if (onFilterUseToggled != null)
			{
				onFilterUseToggled(this);
			}
			this.ActiveFilterItems.Sort(new OrderOfBattleFormationFilterSelectorItemComparer());
		}

		private bool HasAnyActiveFilter()
		{
			return this.HasFilter(FormationFilterType.Shield) || this.HasFilter(FormationFilterType.Spear) || this.HasFilter(FormationFilterType.Thrown) || this.HasFilter(FormationFilterType.Heavy) || this.HasFilter(FormationFilterType.HighTier) || this.HasFilter(FormationFilterType.LowTier);
		}

		public void UpdateAdjustable()
		{
			bool flag;
			if (this.IsControlledByPlayer)
			{
				flag = this.Classes.All((OrderOfBattleFormationClassVM c) => c.Class == FormationClass.NumberOfAllFormations || c.IsAdjustable || !OrderOfBattleFormationItemVM.HasAnyTroopWithClass(c.Class));
			}
			else
			{
				flag = false;
			}
			this.IsAdjustable = flag;
			if (!this.IsControlledByPlayer)
			{
				this.CantAdjustHint = new HintViewModel(this._cantAdjustNotCommanderText, null);
				return;
			}
			if (!this.Classes.All((OrderOfBattleFormationClassVM c) => c.Class == FormationClass.NumberOfAllFormations || c.IsAdjustable))
			{
				this.CantAdjustHint = new HintViewModel(this._cantAdjustSingledOutText, null);
			}
		}

		public bool HasFilter(FormationFilterType filter)
		{
			return this.FilterItems.Any((OrderOfBattleFormationFilterSelectorItemVM f) => f.IsActive && f.FilterType == filter);
		}

		public bool HasOnlyOneClass()
		{
			int num = 0;
			for (int i = 0; i < this.Classes.Count; i++)
			{
				if (!this.Classes[i].IsUnset)
				{
					num++;
				}
			}
			return num == 1;
		}

		public bool OnlyHasClass(FormationClass formationClass)
		{
			bool flag = false;
			for (int i = 0; i < this.Classes.Count; i++)
			{
				if (!flag && this.Classes[i].Class == formationClass && !this.Classes[i].IsUnset)
				{
					flag = true;
				}
				if (flag && this.Classes[i].Class != FormationClass.NumberOfAllFormations)
				{
					return false;
				}
			}
			return true;
		}

		public bool HasClass(FormationClass formationClass)
		{
			for (int i = 0; i < this.Classes.Count; i++)
			{
				if (this.Classes[i].Class == formationClass && !this.Classes[i].IsUnset)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasClasses(FormationClass[] formationClasses)
		{
			FormationClass[] array = (from c in this.Classes
				select c.Class into c
				where c != FormationClass.NumberOfAllFormations
				select c).ToArray<FormationClass>();
			return formationClasses.OrderBy((FormationClass c) => c).SequenceEqual(array.OrderBy((FormationClass c) => c));
		}

		private List<TooltipProperty> GetTooltip()
		{
			GameTexts.SetVariable("NUMBER", this.TitleText);
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(this._formationTooltipTitleText.ToString(), string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.Title),
				new TooltipProperty(string.Empty, string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.DefaultSeperator)
			};
			if (this.FormationClassSelector.SelectedItem == null)
			{
				return list;
			}
			List<Agent> list2 = new List<Agent>();
			int[] array = new int[4];
			using (List<IFormationUnit>.Enumerator enumerator = this.Formation.Arrangement.GetAllUnits().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Agent agent4;
					if ((agent4 = enumerator.Current as Agent) != null)
					{
						if (agent4.IsHero)
						{
							list2.Add(agent4);
						}
						else if (agent4.Banner == null)
						{
							FormationClass formationClass = agent4.Character.GetFormationClass();
							if (formationClass >= FormationClass.Infantry && formationClass < FormationClass.NumberOfDefaultFormations)
							{
								array[(int)formationClass]++;
							}
						}
					}
				}
			}
			foreach (Agent agent2 in this.Formation.DetachedUnits)
			{
				if (agent2.IsHero)
				{
					list2.Add(agent2);
				}
				else if (agent2.Banner == null)
				{
					FormationClass formationClass2 = agent2.Character.GetFormationClass();
					if (formationClass2 >= FormationClass.Infantry && formationClass2 < FormationClass.NumberOfDefaultFormations)
					{
						array[(int)formationClass2]++;
					}
				}
			}
			for (FormationClass formationClass3 = FormationClass.Infantry; formationClass3 < FormationClass.NumberOfDefaultFormations; formationClass3++)
			{
				int num = array[(int)formationClass3];
				List<Agent> list3 = new List<Agent>();
				for (int i = 0; i < list2.Count; i++)
				{
					Agent agent3 = list2[i];
					if ((formationClass3 == FormationClass.Infantry && QueryLibrary.IsInfantry(agent3)) || (formationClass3 == FormationClass.Ranged && QueryLibrary.IsRanged(agent3)) || (formationClass3 == FormationClass.Cavalry && QueryLibrary.IsCavalry(agent3)) || (formationClass3 == FormationClass.HorseArcher && QueryLibrary.IsRangedCavalry(agent3)))
					{
						list3.Add(agent3);
					}
				}
				if (num > 0 || list3.Count > 0)
				{
					List<TooltipProperty> list4 = list;
					string text = "str_troop_group_name";
					int num2 = (int)formationClass3;
					list4.Add(new TooltipProperty(GameTexts.FindText(text, num2.ToString()).ToString(), num.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
					for (int j = 0; j < list3.Count; j++)
					{
						list.Add(new TooltipProperty(list3[j].Name, " ", 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
					list.Add(new TooltipProperty(string.Empty, string.Empty, -1, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			List<Agent> formationBannerBearers = this._bannerBearerLogic.GetFormationBannerBearers(this.Formation);
			if (formationBannerBearers.Count > 0)
			{
				list.Add(new TooltipProperty(new TextObject("{=scnSXrYC}Banner Bearers", null).ToString(), formationBannerBearers.Count.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasAnyActiveFilter())
			{
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.DefaultSeperator));
			}
			DeploymentFormationClass formationClass4 = this.FormationClassSelector.SelectedItem.FormationClass;
			if (this.HasFilter(FormationFilterType.Shield))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.HasShieldCached));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.Shield));
				list.Add(new TooltipProperty(FormationFilterType.Shield.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasFilter(FormationFilterType.Spear))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.HasSpearCached));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.Spear));
				list.Add(new TooltipProperty(FormationFilterType.Spear.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasFilter(FormationFilterType.Thrown))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.HasThrownCached));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.Thrown));
				list.Add(new TooltipProperty(FormationFilterType.Thrown.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasFilter(FormationFilterType.Heavy))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(agent)));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.Heavy));
				list.Add(new TooltipProperty(FormationFilterType.Heavy.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasFilter(FormationFilterType.HighTier))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.Character.GetBattleTier() >= 4));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.HighTier));
				list.Add(new TooltipProperty(FormationFilterType.HighTier.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			if (this.HasFilter(FormationFilterType.LowTier))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.Character.GetBattleTier() <= 3));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(formationClass4, FormationFilterType.LowTier));
				list.Add(new TooltipProperty(FormationFilterType.LowTier.GetFilterName().ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		public void UnassignCommander()
		{
			if (this.Commander != this._unassignedCommander)
			{
				this.Commander.CurrentAssignedFormationItem = null;
				this.Commander = this._unassignedCommander;
			}
		}

		private void ExecuteSelection()
		{
			Action<OrderOfBattleFormationItemVM> onSelection = OrderOfBattleFormationItemVM.OnSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		private void HandleCommanderAssignment(OrderOfBattleHeroItemVM newCommander)
		{
			this.HasCommander = newCommander != this._unassignedCommander;
			if (this.HasCommander)
			{
				Agent agent = newCommander.Agent;
				agent.Formation = this.Formation;
				this.Formation.Captain = agent;
				newCommander.CurrentAssignedFormationItem = this;
				BannerBearerLogic bannerBearerLogic = this._bannerBearerLogic;
				if (bannerBearerLogic != null)
				{
					bannerBearerLogic.SetFormationBanner(this.Formation, newCommander.BannerOfHero);
				}
				if (agent.IsAIControlled)
				{
					agent.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(agent);
				}
			}
			else if (this.Formation != null)
			{
				this.Formation.Captain = null;
				BannerBearerLogic bannerBearerLogic2 = this._bannerBearerLogic;
				if (bannerBearerLogic2 != null)
				{
					bannerBearerLogic2.SetFormationBanner(this.Formation, null);
				}
			}
			this.RefreshFormation();
			this.OnSizeChanged();
			newCommander.RefreshInformation();
		}

		public void ExecuteAcceptCommander()
		{
			Action<OrderOfBattleFormationItemVM> onAcceptCommander = OrderOfBattleFormationItemVM.OnAcceptCommander;
			if (onAcceptCommander == null)
			{
				return;
			}
			onAcceptCommander(this);
		}

		public void ExecuteAcceptHeroTroops()
		{
			Action<OrderOfBattleFormationItemVM> onAcceptHeroTroops = OrderOfBattleFormationItemVM.OnAcceptHeroTroops;
			if (onAcceptHeroTroops == null)
			{
				return;
			}
			onAcceptHeroTroops(this);
		}

		public void OnHeroSelectionUpdated(int selectedHeroCount, bool hasOwnHeroTroopInSelection)
		{
			if (this.IsControlledByPlayer)
			{
				this.IsAcceptingCommander = selectedHeroCount == 1 && this.HasFormation;
				if (!hasOwnHeroTroopInSelection)
				{
					this.IsAcceptingHeroTroops = selectedHeroCount >= 1 && this.HasFormation;
					return;
				}
			}
			else
			{
				this.IsAcceptingCommander = selectedHeroCount == 1 && this.HasFormation && (this.Commander == this._unassignedCommander || !this.Commander.IsAssignedBeforePlayer);
			}
		}

		public void AddHeroTroop(OrderOfBattleHeroItemVM heroItem)
		{
			if (!this.HeroTroops.Contains(heroItem))
			{
				heroItem.CurrentAssignedFormationItem = this;
				heroItem.Agent.Formation = this.Formation;
				this.HeroTroops.Add(heroItem);
				this.RefreshFormation();
				this.OnSizeChanged();
			}
		}

		public void RemoveHeroTroop(OrderOfBattleHeroItemVM heroItem)
		{
			if (this.HeroTroops.Contains(heroItem))
			{
				heroItem.CurrentAssignedFormationItem = null;
				heroItem.Agent.Formation = heroItem.InitialFormation;
				this.HeroTroops.Remove(heroItem);
				this.RefreshFormation();
				this.OnSizeChanged();
			}
		}

		private void RefreshFormation()
		{
			this.HasHeroTroops = this.HeroTroops.Count > 0;
			this.IsHeroTroopsOverflowing = this.HeroTroops.Count > 8;
			this.OverflowHeroTroopCountText = (this.HeroTroops.Count - 8 + 1).ToString("+#;-#;0");
			this.Formation.Refresh();
			Action onHeroesChanged = OrderOfBattleFormationItemVM.OnHeroesChanged;
			if (onHeroesChanged == null)
			{
				return;
			}
			onHeroesChanged();
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool HasFormation
		{
			get
			{
				return this._hasFormation;
			}
			set
			{
				if (value != this._hasFormation)
				{
					this._hasFormation = value;
					base.OnPropertyChangedWithValue(value, "HasFormation");
				}
			}
		}

		[DataSourceProperty]
		public bool HasCommander
		{
			get
			{
				return this._hasCommander;
			}
			set
			{
				if (value != this._hasCommander)
				{
					this._hasCommander = value;
					base.OnPropertyChangedWithValue(value, "HasCommander");
				}
			}
		}

		[DataSourceProperty]
		public bool HasHeroTroops
		{
			get
			{
				return this._hasHeroTroops;
			}
			set
			{
				if (value != this._hasHeroTroops)
				{
					this._hasHeroTroops = value;
					base.OnPropertyChangedWithValue(value, "HasHeroTroops");
				}
			}
		}

		[DataSourceProperty]
		public bool IsControlledByPlayer
		{
			get
			{
				return this._isControlledByPlayer;
			}
			set
			{
				if (value != this._isControlledByPlayer)
				{
					this._isControlledByPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsControlledByPlayer");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChangedWithValue(value, "IsSelectable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFiltered
		{
			get
			{
				return this._isFiltered;
			}
			set
			{
				if (value != this._isFiltered)
				{
					this._isFiltered = value;
					base.OnPropertyChangedWithValue(value, "IsFiltered");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAdjustable
		{
			get
			{
				return this._isAdjustable;
			}
			set
			{
				if (value != this._isAdjustable)
				{
					this._isAdjustable = value;
					base.OnPropertyChangedWithValue(value, "IsAdjustable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMarkerShown
		{
			get
			{
				return this._isMarkerShown;
			}
			set
			{
				if (value != this._isMarkerShown)
				{
					this._isMarkerShown = value;
					base.OnPropertyChangedWithValue(value, "IsMarkerShown");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBeingFocused
		{
			get
			{
				return this._isBeingFocused;
			}
			set
			{
				if (value != this._isBeingFocused)
				{
					this._isBeingFocused = value;
					base.OnPropertyChangedWithValue(value, "IsBeingFocused");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAcceptingCommander
		{
			get
			{
				return this._isAcceptingCommander;
			}
			set
			{
				if (value != this._isAcceptingCommander)
				{
					this._isAcceptingCommander = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptingCommander");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAcceptingHeroTroops
		{
			get
			{
				return this._isAcceptingHeroTroops;
			}
			set
			{
				if (value != this._isAcceptingHeroTroops)
				{
					this._isAcceptingHeroTroops = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptingHeroTroops");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHeroTroopsOverflowing
		{
			get
			{
				return this._isHeroTroopsOverflowing;
			}
			set
			{
				if (value != this._isHeroTroopsOverflowing)
				{
					this._isHeroTroopsOverflowing = value;
					base.OnPropertyChangedWithValue(value, "IsHeroTroopsOverflowing");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFilterSelectionActive
		{
			get
			{
				return this._isFilterSelectionActive;
			}
			set
			{
				if (value != this._isFilterSelectionActive)
				{
					this._isFilterSelectionActive = value;
					base.OnPropertyChangedWithValue(value, "IsFilterSelectionActive");
					Action<OrderOfBattleFormationItemVM> onFilterSelectionToggled = OrderOfBattleFormationItemVM.OnFilterSelectionToggled;
					if (onFilterSelectionToggled == null)
					{
						return;
					}
					onFilterSelectionToggled(this);
				}
			}
		}

		[DataSourceProperty]
		public bool IsClassSelectionActive
		{
			get
			{
				return this._isClassSelectionActive;
			}
			set
			{
				if (value != this._isClassSelectionActive)
				{
					this._isClassSelectionActive = value;
					base.OnPropertyChangedWithValue(value, "IsClassSelectionActive");
					Action<OrderOfBattleFormationItemVM> onClassSelectionToggled = OrderOfBattleFormationItemVM.OnClassSelectionToggled;
					if (onClassSelectionToggled == null)
					{
						return;
					}
					onClassSelectionToggled(this);
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string FormationIsEmptyText
		{
			get
			{
				return this._formationIsEmptyText;
			}
			set
			{
				if (value != this._formationIsEmptyText)
				{
					this._formationIsEmptyText = value;
					base.OnPropertyChangedWithValue<string>(value, "FormationIsEmptyText");
				}
			}
		}

		[DataSourceProperty]
		public string OverflowHeroTroopCountText
		{
			get
			{
				return this._overflowHeroTroopCountText;
			}
			set
			{
				if (value != this._overflowHeroTroopCountText)
				{
					this._overflowHeroTroopCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "OverflowHeroTroopCountText");
				}
			}
		}

		[DataSourceProperty]
		public int TroopCount
		{
			get
			{
				return this._troopCount;
			}
			set
			{
				if (value != this._troopCount)
				{
					this._troopCount = value;
					base.OnPropertyChangedWithValue(value, "TroopCount");
				}
			}
		}

		[DataSourceProperty]
		public int OrderOfBattleFormationClassInt
		{
			get
			{
				return this._orderOfBattleFormationClassInt;
			}
			set
			{
				if (value != this._orderOfBattleFormationClassInt)
				{
					this._orderOfBattleFormationClassInt = value;
					base.OnPropertyChangedWithValue(value, "OrderOfBattleFormationClassInt");
				}
			}
		}

		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (value != this._wSign)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		[DataSourceProperty]
		public OrderOfBattleHeroItemVM Commander
		{
			get
			{
				return this._commander;
			}
			set
			{
				if (value != this._commander)
				{
					this._commander = value;
					base.OnPropertyChangedWithValue<OrderOfBattleHeroItemVM>(value, "Commander");
					this.HandleCommanderAssignment(value);
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderOfBattleHeroItemVM> HeroTroops
		{
			get
			{
				return this._heroTroops;
			}
			set
			{
				if (value != this._heroTroops)
				{
					this._heroTroops = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleHeroItemVM>>(value, "HeroTroops");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationClassVM> Classes
		{
			get
			{
				return this._classes;
			}
			set
			{
				if (value != this._classes)
				{
					this._classes = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationClassVM>>(value, "Classes");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<OrderOfBattleFormationClassSelectorItemVM> FormationClassSelector
		{
			get
			{
				return this._formationClassSelector;
			}
			set
			{
				if (value != this._formationClassSelector)
				{
					this._formationClassSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(value, "FormationClassSelector");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> FilterItems
		{
			get
			{
				return this._filterItems;
			}
			set
			{
				if (value != this._filterItems)
				{
					this._filterItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>>(value, "FilterItems");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> ActiveFilterItems
		{
			get
			{
				return this._activeFilterItems;
			}
			set
			{
				if (value != this._activeFilterItems)
				{
					this._activeFilterItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>>(value, "ActiveFilterItems");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CantAdjustHint
		{
			get
			{
				return this._cantAdjustHint;
			}
			set
			{
				if (value != this._cantAdjustHint)
				{
					this._cantAdjustHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CantAdjustHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CommanderSlotHint
		{
			get
			{
				return this._commanderSlotHint;
			}
			set
			{
				if (value != this._commanderSlotHint)
				{
					this._commanderSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CommanderSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel HeroTroopSlotHint
		{
			get
			{
				return this._heroTroopSlotHint;
			}
			set
			{
				if (value != this._heroTroopSlotHint)
				{
					this._heroTroopSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HeroTroopSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel AssignCommanderHint
		{
			get
			{
				return this._assignCommanderHint;
			}
			set
			{
				if (value != this._assignCommanderHint)
				{
					this._assignCommanderHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AssignCommanderHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel AssignHeroTroopHint
		{
			get
			{
				return this._assignHeroTroopHint;
			}
			set
			{
				if (value != this._assignHeroTroopHint)
				{
					this._assignHeroTroopHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AssignHeroTroopHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCaptainSlotHighlightActive
		{
			get
			{
				return this._isCaptainSlotHighlightActive;
			}
			set
			{
				if (value != this._isCaptainSlotHighlightActive)
				{
					this._isCaptainSlotHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsCaptainSlotHighlightActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTypeSelectionHighlightActive
		{
			get
			{
				return this._isTypeSelectionHighlightActive;
			}
			set
			{
				if (value != this._isTypeSelectionHighlightActive)
				{
					this._isTypeSelectionHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsTypeSelectionHighlightActive");
				}
			}
		}

		private const int MaxShownHeroTroopCount = 8;

		private readonly Camera _missionCamera;

		private BannerBearerLogic _bannerBearerLogic;

		public static Action OnHeroesChanged;

		public static Action<OrderOfBattleFormationItemVM> OnFilterSelectionToggled;

		public static Action<OrderOfBattleFormationItemVM> OnClassSelectionToggled;

		public static Action<OrderOfBattleFormationItemVM> OnFilterUseToggled;

		public static Action<OrderOfBattleFormationItemVM> OnSelection;

		public static Action<OrderOfBattleFormationItemVM> OnDeselection;

		public static Func<DeploymentFormationClass, FormationFilterType, int> GetTotalTroopCountWithFilter;

		public static Func<Func<OrderOfBattleFormationItemVM, bool>, IEnumerable<OrderOfBattleFormationItemVM>> GetFormationWithCondition;

		public static Func<FormationClass, bool> HasAnyTroopWithClass;

		public static Action<OrderOfBattleFormationItemVM> OnAcceptCommander;

		public static Action<OrderOfBattleFormationItemVM> OnAcceptHeroTroops;

		private OrderOfBattleHeroItemVM _unassignedCommander;

		private readonly TextObject _formationTooltipTitleText = new TextObject("{=cZNA5Z6l}Formation {NUMBER}", null);

		private readonly TextObject _filteredTroopCountInfoText = new TextObject("{=yRIPADWl}{TROOP_COUNT}/{TOTAL_TROOP_COUNT}", null);

		private readonly TextObject _cantAdjustNotCommanderText = new TextObject("{=ZixS1b4u}You're not leading this battle.", null);

		private readonly TextObject _cantAdjustSingledOutText = new TextObject("{=7jhe9cT9}You need to have at least one more formation of this type to change this formation's type.", null);

		private readonly TextObject _commanderSlotHintText = new TextObject("{=RvKwdXWs}Commander", null);

		private readonly TextObject _heroTroopSlotHintText = new TextObject("{=VyMD4iRV}Hero Troops", null);

		private readonly TextObject _assignCommanderHintText = new TextObject("{=MssTzJJb}Assign as Commander", null);

		private readonly TextObject _assignHeroTroopHintText = new TextObject("{=ngyMTaqr}Assign as Hero Troop", null);

		private Vec3 _worldPosition;

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private float _wPosAfterPositionCalculation;

		private bool _isSelected;

		private bool _hasFormation;

		private bool _hasCommander;

		private bool _isControlledByPlayer;

		private bool _hasHeroTroops;

		private bool _isSelectable;

		private bool _isFiltered;

		private bool _isAdjustable;

		private bool _isMarkerShown;

		private bool _isBeingFocused;

		private bool _isAcceptingCommander;

		private bool _isAcceptingHeroTroops;

		private bool _isHeroTroopsOverflowing;

		private bool _isFilterSelectionActive;

		private bool _isClassSelectionActive;

		private string _titleText;

		private string _formationIsEmptyText;

		private string _overflowHeroTroopCountText;

		private int _orderOfBattleFormationClassInt;

		private int _troopCount;

		private int _wSign;

		private Vec2 _screenPosition;

		private OrderOfBattleHeroItemVM _commander;

		private MBBindingList<OrderOfBattleHeroItemVM> _heroTroops;

		private MBBindingList<OrderOfBattleFormationClassVM> _classes;

		private SelectorVM<OrderOfBattleFormationClassSelectorItemVM> _formationClassSelector;

		private MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> _filterItems;

		private MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> _activeFilterItems;

		private BasicTooltipViewModel _tooltip;

		private HintViewModel _cantAdjustHint;

		private HintViewModel _commanderSlotHint;

		private HintViewModel _heroTroopSlotHint;

		private HintViewModel _assignCommanderHint;

		private HintViewModel _assignHeroTroopHint;

		private bool _isCaptainSlotHighlightActive;

		private bool _isTypeSelectionHighlightActive;
	}
}

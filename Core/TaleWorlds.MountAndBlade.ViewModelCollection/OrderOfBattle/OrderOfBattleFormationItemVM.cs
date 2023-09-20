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
	// Token: 0x0200002E RID: 46
	public class OrderOfBattleFormationItemVM : ViewModel
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000E21F File Offset: 0x0000C41F
		// (set) Token: 0x0600032C RID: 812 RVA: 0x0000E227 File Offset: 0x0000C427
		public Formation Formation { get; private set; }

		// Token: 0x0600032D RID: 813 RVA: 0x0000E230 File Offset: 0x0000C430
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

		// Token: 0x0600032E RID: 814 RVA: 0x0000E404 File Offset: 0x0000C604
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FormationIsEmptyText = new TextObject("{=P3IWytsr}Formation is currently empty", null).ToString();
			this.CommanderSlotHint = new HintViewModel(this._commanderSlotHintText, null);
			this.HeroTroopSlotHint = new HintViewModel(this._heroTroopSlotHintText, null);
			this.AssignCommanderHint = new HintViewModel(this._assignCommanderHintText, null);
			this.AssignHeroTroopHint = new HintViewModel(this._assignHeroTroopHintText, null);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000E478 File Offset: 0x0000C678
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

		// Token: 0x06000330 RID: 816 RVA: 0x0000E588 File Offset: 0x0000C788
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
					switch (formation.PrimaryClass)
					{
					case FormationClass.Infantry:
						formationTypeToSet = DeploymentFormationClass.Infantry;
						break;
					case FormationClass.Ranged:
						formationTypeToSet = DeploymentFormationClass.Ranged;
						break;
					case FormationClass.Cavalry:
						formationTypeToSet = DeploymentFormationClass.Cavalry;
						break;
					case FormationClass.HorseArcher:
						formationTypeToSet = DeploymentFormationClass.HorseArcher;
						break;
					default:
						Debug.FailedAssert("Formation doesn't have a proper primary class. Value : " + formation.PrimaryClass.GetName(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleFormationItemVM.cs", "RefreshFormation", 163);
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

		// Token: 0x06000331 RID: 817 RVA: 0x0000E69C File Offset: 0x0000C89C
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

		// Token: 0x06000332 RID: 818 RVA: 0x0000E710 File Offset: 0x0000C910
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

		// Token: 0x06000333 RID: 819 RVA: 0x0000E7C8 File Offset: 0x0000C9C8
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

		// Token: 0x06000334 RID: 820 RVA: 0x0000EA18 File Offset: 0x0000CC18
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

		// Token: 0x06000335 RID: 821 RVA: 0x0000EB24 File Offset: 0x0000CD24
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

		// Token: 0x06000336 RID: 822 RVA: 0x0000EB89 File Offset: 0x0000CD89
		private bool HasAnyActiveFilter()
		{
			return this.HasFilter(FormationFilterType.Shield) || this.HasFilter(FormationFilterType.Spear) || this.HasFilter(FormationFilterType.Thrown) || this.HasFilter(FormationFilterType.Heavy) || this.HasFilter(FormationFilterType.HighTier) || this.HasFilter(FormationFilterType.LowTier);
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000EBC4 File Offset: 0x0000CDC4
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

		// Token: 0x06000338 RID: 824 RVA: 0x0000EC68 File Offset: 0x0000CE68
		public bool HasFilter(FormationFilterType filter)
		{
			return this.FilterItems.Any((OrderOfBattleFormationFilterSelectorItemVM f) => f.IsActive && f.FilterType == filter);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000EC9C File Offset: 0x0000CE9C
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

		// Token: 0x0600033A RID: 826 RVA: 0x0000ECDC File Offset: 0x0000CEDC
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

		// Token: 0x0600033B RID: 827 RVA: 0x0000ED48 File Offset: 0x0000CF48
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

		// Token: 0x0600033C RID: 828 RVA: 0x0000ED98 File Offset: 0x0000CF98
		public bool HasClasses(FormationClass[] formationClasses)
		{
			FormationClass[] array = (from c in this.Classes
				select c.Class into c
				where c != FormationClass.NumberOfAllFormations
				select c).ToArray<FormationClass>();
			return formationClasses.OrderBy((FormationClass c) => c).SequenceEqual(array.OrderBy((FormationClass c) => c));
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000EE48 File Offset: 0x0000D048
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
				list.Add(new TooltipProperty(new TextObject("{=*}Banner Bearers", null).ToString(), formationBannerBearers.Count.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
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

		// Token: 0x0600033E RID: 830 RVA: 0x0000F43C File Offset: 0x0000D63C
		public void UnassignCommander()
		{
			if (this.Commander != this._unassignedCommander)
			{
				this.Commander.CurrentAssignedFormationItem = null;
				this.Commander = this._unassignedCommander;
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000F464 File Offset: 0x0000D664
		private void ExecuteSelection()
		{
			Action<OrderOfBattleFormationItemVM> onSelection = OrderOfBattleFormationItemVM.OnSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000F478 File Offset: 0x0000D678
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

		// Token: 0x06000341 RID: 833 RVA: 0x0000F53B File Offset: 0x0000D73B
		public void ExecuteAcceptCommander()
		{
			Action<OrderOfBattleFormationItemVM> onAcceptCommander = OrderOfBattleFormationItemVM.OnAcceptCommander;
			if (onAcceptCommander == null)
			{
				return;
			}
			onAcceptCommander(this);
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0000F54D File Offset: 0x0000D74D
		public void ExecuteAcceptHeroTroops()
		{
			Action<OrderOfBattleFormationItemVM> onAcceptHeroTroops = OrderOfBattleFormationItemVM.OnAcceptHeroTroops;
			if (onAcceptHeroTroops == null)
			{
				return;
			}
			onAcceptHeroTroops(this);
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0000F560 File Offset: 0x0000D760
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

		// Token: 0x06000344 RID: 836 RVA: 0x0000F5D3 File Offset: 0x0000D7D3
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

		// Token: 0x06000345 RID: 837 RVA: 0x0000F614 File Offset: 0x0000D814
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

		// Token: 0x06000346 RID: 838 RVA: 0x0000F660 File Offset: 0x0000D860
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

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000F6D1 File Offset: 0x0000D8D1
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000F6D9 File Offset: 0x0000D8D9
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

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000F6F7 File Offset: 0x0000D8F7
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0000F6FF File Offset: 0x0000D8FF
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

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000F71D File Offset: 0x0000D91D
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0000F725 File Offset: 0x0000D925
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

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0000F743 File Offset: 0x0000D943
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0000F74B File Offset: 0x0000D94B
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

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600034F RID: 847 RVA: 0x0000F769 File Offset: 0x0000D969
		// (set) Token: 0x06000350 RID: 848 RVA: 0x0000F771 File Offset: 0x0000D971
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

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0000F78F File Offset: 0x0000D98F
		// (set) Token: 0x06000352 RID: 850 RVA: 0x0000F797 File Offset: 0x0000D997
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

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0000F7B5 File Offset: 0x0000D9B5
		// (set) Token: 0x06000354 RID: 852 RVA: 0x0000F7BD File Offset: 0x0000D9BD
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

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000F7DB File Offset: 0x0000D9DB
		// (set) Token: 0x06000356 RID: 854 RVA: 0x0000F7E3 File Offset: 0x0000D9E3
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

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0000F801 File Offset: 0x0000DA01
		// (set) Token: 0x06000358 RID: 856 RVA: 0x0000F809 File Offset: 0x0000DA09
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

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000F827 File Offset: 0x0000DA27
		// (set) Token: 0x0600035A RID: 858 RVA: 0x0000F82F File Offset: 0x0000DA2F
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

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000F84D File Offset: 0x0000DA4D
		// (set) Token: 0x0600035C RID: 860 RVA: 0x0000F855 File Offset: 0x0000DA55
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

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0000F873 File Offset: 0x0000DA73
		// (set) Token: 0x0600035E RID: 862 RVA: 0x0000F87B File Offset: 0x0000DA7B
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

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600035F RID: 863 RVA: 0x0000F899 File Offset: 0x0000DA99
		// (set) Token: 0x06000360 RID: 864 RVA: 0x0000F8A1 File Offset: 0x0000DAA1
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

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0000F8BF File Offset: 0x0000DABF
		// (set) Token: 0x06000362 RID: 866 RVA: 0x0000F8C7 File Offset: 0x0000DAC7
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

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000363 RID: 867 RVA: 0x0000F8F5 File Offset: 0x0000DAF5
		// (set) Token: 0x06000364 RID: 868 RVA: 0x0000F8FD File Offset: 0x0000DAFD
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

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0000F92B File Offset: 0x0000DB2B
		// (set) Token: 0x06000366 RID: 870 RVA: 0x0000F933 File Offset: 0x0000DB33
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

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000367 RID: 871 RVA: 0x0000F956 File Offset: 0x0000DB56
		// (set) Token: 0x06000368 RID: 872 RVA: 0x0000F95E File Offset: 0x0000DB5E
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

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000369 RID: 873 RVA: 0x0000F981 File Offset: 0x0000DB81
		// (set) Token: 0x0600036A RID: 874 RVA: 0x0000F989 File Offset: 0x0000DB89
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

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x0600036B RID: 875 RVA: 0x0000F9AC File Offset: 0x0000DBAC
		// (set) Token: 0x0600036C RID: 876 RVA: 0x0000F9B4 File Offset: 0x0000DBB4
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

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600036D RID: 877 RVA: 0x0000F9D2 File Offset: 0x0000DBD2
		// (set) Token: 0x0600036E RID: 878 RVA: 0x0000F9DA File Offset: 0x0000DBDA
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

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600036F RID: 879 RVA: 0x0000F9F8 File Offset: 0x0000DBF8
		// (set) Token: 0x06000370 RID: 880 RVA: 0x0000FA00 File Offset: 0x0000DC00
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

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0000FA1E File Offset: 0x0000DC1E
		// (set) Token: 0x06000372 RID: 882 RVA: 0x0000FA26 File Offset: 0x0000DC26
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

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0000FA61 File Offset: 0x0000DC61
		// (set) Token: 0x06000374 RID: 884 RVA: 0x0000FA69 File Offset: 0x0000DC69
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

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000375 RID: 885 RVA: 0x0000FA8E File Offset: 0x0000DC8E
		// (set) Token: 0x06000376 RID: 886 RVA: 0x0000FA96 File Offset: 0x0000DC96
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

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0000FAB4 File Offset: 0x0000DCB4
		// (set) Token: 0x06000378 RID: 888 RVA: 0x0000FABC File Offset: 0x0000DCBC
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

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0000FADA File Offset: 0x0000DCDA
		// (set) Token: 0x0600037A RID: 890 RVA: 0x0000FAE2 File Offset: 0x0000DCE2
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

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600037B RID: 891 RVA: 0x0000FB00 File Offset: 0x0000DD00
		// (set) Token: 0x0600037C RID: 892 RVA: 0x0000FB08 File Offset: 0x0000DD08
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

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600037D RID: 893 RVA: 0x0000FB26 File Offset: 0x0000DD26
		// (set) Token: 0x0600037E RID: 894 RVA: 0x0000FB2E File Offset: 0x0000DD2E
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

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600037F RID: 895 RVA: 0x0000FB4C File Offset: 0x0000DD4C
		// (set) Token: 0x06000380 RID: 896 RVA: 0x0000FB54 File Offset: 0x0000DD54
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

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000381 RID: 897 RVA: 0x0000FB72 File Offset: 0x0000DD72
		// (set) Token: 0x06000382 RID: 898 RVA: 0x0000FB7A File Offset: 0x0000DD7A
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

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000383 RID: 899 RVA: 0x0000FB98 File Offset: 0x0000DD98
		// (set) Token: 0x06000384 RID: 900 RVA: 0x0000FBA0 File Offset: 0x0000DDA0
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

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000385 RID: 901 RVA: 0x0000FBBE File Offset: 0x0000DDBE
		// (set) Token: 0x06000386 RID: 902 RVA: 0x0000FBC6 File Offset: 0x0000DDC6
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

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		// (set) Token: 0x06000388 RID: 904 RVA: 0x0000FBEC File Offset: 0x0000DDEC
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

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0000FC0A File Offset: 0x0000DE0A
		// (set) Token: 0x0600038A RID: 906 RVA: 0x0000FC12 File Offset: 0x0000DE12
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

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x0600038B RID: 907 RVA: 0x0000FC30 File Offset: 0x0000DE30
		// (set) Token: 0x0600038C RID: 908 RVA: 0x0000FC38 File Offset: 0x0000DE38
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

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0000FC56 File Offset: 0x0000DE56
		// (set) Token: 0x0600038E RID: 910 RVA: 0x0000FC5E File Offset: 0x0000DE5E
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

		// Token: 0x04000196 RID: 406
		private const int MaxShownHeroTroopCount = 8;

		// Token: 0x04000198 RID: 408
		private readonly Camera _missionCamera;

		// Token: 0x04000199 RID: 409
		private BannerBearerLogic _bannerBearerLogic;

		// Token: 0x0400019A RID: 410
		public static Action OnHeroesChanged;

		// Token: 0x0400019B RID: 411
		public static Action<OrderOfBattleFormationItemVM> OnFilterSelectionToggled;

		// Token: 0x0400019C RID: 412
		public static Action<OrderOfBattleFormationItemVM> OnClassSelectionToggled;

		// Token: 0x0400019D RID: 413
		public static Action<OrderOfBattleFormationItemVM> OnFilterUseToggled;

		// Token: 0x0400019E RID: 414
		public static Action<OrderOfBattleFormationItemVM> OnSelection;

		// Token: 0x0400019F RID: 415
		public static Action<OrderOfBattleFormationItemVM> OnDeselection;

		// Token: 0x040001A0 RID: 416
		public static Func<DeploymentFormationClass, FormationFilterType, int> GetTotalTroopCountWithFilter;

		// Token: 0x040001A1 RID: 417
		public static Func<Func<OrderOfBattleFormationItemVM, bool>, IEnumerable<OrderOfBattleFormationItemVM>> GetFormationWithCondition;

		// Token: 0x040001A2 RID: 418
		public static Func<FormationClass, bool> HasAnyTroopWithClass;

		// Token: 0x040001A3 RID: 419
		public static Action<OrderOfBattleFormationItemVM> OnAcceptCommander;

		// Token: 0x040001A4 RID: 420
		public static Action<OrderOfBattleFormationItemVM> OnAcceptHeroTroops;

		// Token: 0x040001A5 RID: 421
		private OrderOfBattleHeroItemVM _unassignedCommander;

		// Token: 0x040001A6 RID: 422
		private readonly TextObject _formationTooltipTitleText = new TextObject("{=cZNA5Z6l}Formation {NUMBER}", null);

		// Token: 0x040001A7 RID: 423
		private readonly TextObject _filteredTroopCountInfoText = new TextObject("{=yRIPADWl}{TROOP_COUNT}/{TOTAL_TROOP_COUNT}", null);

		// Token: 0x040001A8 RID: 424
		private readonly TextObject _cantAdjustNotCommanderText = new TextObject("{=ZixS1b4u}You're not leading this battle.", null);

		// Token: 0x040001A9 RID: 425
		private readonly TextObject _cantAdjustSingledOutText = new TextObject("{=7jhe9cT9}You need to have at least one more formation of this type to change this formation's type.", null);

		// Token: 0x040001AA RID: 426
		private readonly TextObject _commanderSlotHintText = new TextObject("{=RvKwdXWs}Commander", null);

		// Token: 0x040001AB RID: 427
		private readonly TextObject _heroTroopSlotHintText = new TextObject("{=VyMD4iRV}Hero Troops", null);

		// Token: 0x040001AC RID: 428
		private readonly TextObject _assignCommanderHintText = new TextObject("{=MssTzJJb}Assign as Commander", null);

		// Token: 0x040001AD RID: 429
		private readonly TextObject _assignHeroTroopHintText = new TextObject("{=ngyMTaqr}Assign as Hero Troop", null);

		// Token: 0x040001AE RID: 430
		private Vec3 _worldPosition;

		// Token: 0x040001AF RID: 431
		private float _latestX;

		// Token: 0x040001B0 RID: 432
		private float _latestY;

		// Token: 0x040001B1 RID: 433
		private float _latestW;

		// Token: 0x040001B2 RID: 434
		private float _wPosAfterPositionCalculation;

		// Token: 0x040001B3 RID: 435
		private bool _isSelected;

		// Token: 0x040001B4 RID: 436
		private bool _hasFormation;

		// Token: 0x040001B5 RID: 437
		private bool _hasCommander;

		// Token: 0x040001B6 RID: 438
		private bool _isControlledByPlayer;

		// Token: 0x040001B7 RID: 439
		private bool _hasHeroTroops;

		// Token: 0x040001B8 RID: 440
		private bool _isSelectable;

		// Token: 0x040001B9 RID: 441
		private bool _isFiltered;

		// Token: 0x040001BA RID: 442
		private bool _isAdjustable;

		// Token: 0x040001BB RID: 443
		private bool _isMarkerShown;

		// Token: 0x040001BC RID: 444
		private bool _isBeingFocused;

		// Token: 0x040001BD RID: 445
		private bool _isAcceptingCommander;

		// Token: 0x040001BE RID: 446
		private bool _isAcceptingHeroTroops;

		// Token: 0x040001BF RID: 447
		private bool _isHeroTroopsOverflowing;

		// Token: 0x040001C0 RID: 448
		private bool _isFilterSelectionActive;

		// Token: 0x040001C1 RID: 449
		private bool _isClassSelectionActive;

		// Token: 0x040001C2 RID: 450
		private string _titleText;

		// Token: 0x040001C3 RID: 451
		private string _formationIsEmptyText;

		// Token: 0x040001C4 RID: 452
		private string _overflowHeroTroopCountText;

		// Token: 0x040001C5 RID: 453
		private int _orderOfBattleFormationClassInt;

		// Token: 0x040001C6 RID: 454
		private int _troopCount;

		// Token: 0x040001C7 RID: 455
		private int _wSign;

		// Token: 0x040001C8 RID: 456
		private Vec2 _screenPosition;

		// Token: 0x040001C9 RID: 457
		private OrderOfBattleHeroItemVM _commander;

		// Token: 0x040001CA RID: 458
		private MBBindingList<OrderOfBattleHeroItemVM> _heroTroops;

		// Token: 0x040001CB RID: 459
		private MBBindingList<OrderOfBattleFormationClassVM> _classes;

		// Token: 0x040001CC RID: 460
		private SelectorVM<OrderOfBattleFormationClassSelectorItemVM> _formationClassSelector;

		// Token: 0x040001CD RID: 461
		private MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> _filterItems;

		// Token: 0x040001CE RID: 462
		private MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> _activeFilterItems;

		// Token: 0x040001CF RID: 463
		private BasicTooltipViewModel _tooltip;

		// Token: 0x040001D0 RID: 464
		private HintViewModel _cantAdjustHint;

		// Token: 0x040001D1 RID: 465
		private HintViewModel _commanderSlotHint;

		// Token: 0x040001D2 RID: 466
		private HintViewModel _heroTroopSlotHint;

		// Token: 0x040001D3 RID: 467
		private HintViewModel _assignCommanderHint;

		// Token: 0x040001D4 RID: 468
		private HintViewModel _assignHeroTroopHint;

		// Token: 0x040001D5 RID: 469
		private bool _isCaptainSlotHighlightActive;

		// Token: 0x040001D6 RID: 470
		private bool _isTypeSelectionHighlightActive;
	}
}

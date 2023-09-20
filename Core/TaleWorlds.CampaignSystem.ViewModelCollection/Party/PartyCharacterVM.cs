using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class PartyCharacterVM : ViewModel
	{
		public TroopRoster Troops { get; private set; }

		public string StringId { get; private set; }

		public TroopRosterElement Troop
		{
			get
			{
				return this._troop;
			}
			set
			{
				this._troop = value;
				this.Character = value.Character;
				this.TroopID = this.Character.StringId;
				this.CheckTransferAmountDefaultValue();
				this.TroopXPTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetTroopXPTooltip(value));
				this.TroopConformityTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetTroopConformityTooltip(value));
			}
		}

		public CharacterObject Character
		{
			get
			{
				return this._character;
			}
			set
			{
				if (this._character != value)
				{
					this._character = value;
					CharacterCode characterCode = this.GetCharacterCode(value, this.Type, this.Side);
					this.Code = new ImageIdentifierVM(characterCode);
					if (this._character.UpgradeTargets.Length != 0)
					{
						this.Upgrades = new MBBindingList<UpgradeTargetVM>();
						for (int i = 0; i < this._character.UpgradeTargets.Length; i++)
						{
							CharacterCode characterCode2 = this.GetCharacterCode(this._character.UpgradeTargets[i], this.Type, this.Side);
							this.Upgrades.Add(new UpgradeTargetVM(i, value, characterCode2, new Action<int, int>(this.Upgrade), new Action<UpgradeTargetVM>(this.FocusUpgrade)));
						}
						this.HasMoreThanTwoUpgrades = this.Upgrades.Count > 2;
					}
				}
				this.CheckTransferAmountDefaultValue();
			}
		}

		public PartyCharacterVM(PartyScreenLogic partyScreenLogic, PartyVM partyVm, TroopRoster troops, int index, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, bool isTroopTransferrable)
		{
			this.Upgrades = new MBBindingList<UpgradeTargetVM>();
			this._partyScreenLogic = partyScreenLogic;
			this._partyVm = partyVm;
			this.Troops = troops;
			this.Side = side;
			this.Type = type;
			this.Troop = troops.GetElementCopyAtIndex(index);
			this.Index = index;
			this.IsHero = this.Troop.Character.IsHero;
			this.IsMainHero = Hero.MainHero.CharacterObject == this.Troop.Character;
			this.IsPrisoner = this.Type == PartyScreenLogic.TroopType.Prisoner;
			this.TierIconData = CampaignUIHelper.GetCharacterTierData(this.Troop.Character, true);
			this.TypeIconData = CampaignUIHelper.GetCharacterTypeData(this.Troop.Character, false);
			this.StringId = CampaignUIHelper.GetTroopLockStringID(this.Troop);
			this._initIsTroopTransferable = isTroopTransferrable;
			this.IsTroopTransferrable = this._initIsTroopTransferable;
			this.TradeData = new PartyTradeVM(partyScreenLogic, this.Troop, this.Side, this.IsTroopTransferrable, this.IsPrisoner, new Action<int, bool>(this.OnTradeApplyTransaction));
			this.IsPrisonerOfPlayer = this.IsPrisoner && this.Side == PartyScreenLogic.PartyRosterSide.Right;
			this.IsHeroPrisonerOfPlayer = this.IsPrisonerOfPlayer && this.Character.IsHero;
			this.IsExecutable = this._partyScreenLogic.IsExecutable(this.Type, this.Character, this.Side);
			this.IsUpgradableTroop = this.Side == PartyScreenLogic.PartyRosterSide.Right && !this.IsHero && !this.IsPrisoner && this.Character.UpgradeTargets.Length != 0 && !this._partyScreenLogic.IsTroopUpgradesDisabled;
			this.InitializeUpgrades();
			this.ThrowOnPropertyChanged();
			this.CheckTransferAmountDefaultValue();
			this.UpdateRecruitable();
			this.RefreshValues();
			this.UpdateTransferHint();
			this.SetMoraleCost();
			this.RecruitPrisonerHint = new BasicTooltipViewModel(() => this._partyScreenLogic.GetRecruitableReasonText(this.Troop.Character, this.IsTroopRecruitable, this.Troop.Number, PartyCharacterVM.FiveStackShortcutKeyText, PartyCharacterVM.EntireStackShortcutKeyText));
			this.ExecutePrisonerHint = new BasicTooltipViewModel(() => this._partyScreenLogic.GetExecutableReasonText(this.Troop.Character, this.IsExecutable));
			this.HeroHealthHint = (this.Troop.Character.IsHero ? new BasicTooltipViewModel(() => CampaignUIHelper.GetHeroHealthTooltip(this.Troop.Character.HeroObject)) : null);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Troop.Character.Name.ToString();
			this.LockHint = new HintViewModel(GameTexts.FindText("str_inventory_lock", null), null);
			MBBindingList<UpgradeTargetVM> upgrades = this.Upgrades;
			if (upgrades != null)
			{
				upgrades.ApplyActionOnAllItems(delegate(UpgradeTargetVM x)
				{
					x.RefreshValues();
				});
			}
			PartyTradeVM tradeData = this.TradeData;
			if (tradeData == null)
			{
				return;
			}
			tradeData.RefreshValues();
		}

		private void UpdateTransferHint()
		{
			GameTexts.SetVariable("newline", "\n");
			GameTexts.SetVariable("STR1", "");
			GameTexts.SetVariable("STR2", "");
			if (!string.IsNullOrEmpty(PartyCharacterVM.EntireStackShortcutKeyText))
			{
				GameTexts.SetVariable("KEY_NAME", PartyCharacterVM.EntireStackShortcutKeyText);
				string text = GameTexts.FindText("str_entire_stack_shortcut_transfer_troops", null).ToString();
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", "");
				if (this.Number >= 5 && !string.IsNullOrEmpty(PartyCharacterVM.FiveStackShortcutKeyText))
				{
					GameTexts.SetVariable("KEY_NAME", PartyCharacterVM.FiveStackShortcutKeyText);
					string text2 = GameTexts.FindText("str_five_stack_shortcut_transfer_troops", null).ToString();
					GameTexts.SetVariable("STR2", text2);
				}
			}
			string text3 = GameTexts.FindText("str_string_newline_string", null).ToString();
			TextObject textObject = GameTexts.FindText("str_string_newline_string", null).CopyTextObject();
			textObject.SetTextVariable("STR2", text3);
			textObject.SetTextVariable("STR1", GameTexts.FindText("str_transfer", null).ToString());
			this.TransferHint = new HintViewModel(textObject, null);
		}

		private void CheckTransferAmountDefaultValue()
		{
			if (this.TransferAmount == 0 && this.Troop.Character != null && this.Troop.Number > 0)
			{
				this.TransferAmount = 1;
			}
		}

		public void ExecuteSetSelected()
		{
			if (this.Character != null)
			{
				PartyCharacterVM.SetSelected(this);
			}
		}

		public void ExecuteTalk()
		{
			PartyVM partyVm = this._partyVm;
			if (partyVm == null)
			{
				return;
			}
			partyVm.ExecuteTalk();
		}

		public void UpdateTradeData()
		{
			PartyTradeVM tradeData = this.TradeData;
			if (tradeData != null)
			{
				tradeData.UpdateTroopData(this.Troop, this.Side, true);
			}
			this.UpdateTransferHint();
		}

		public void UpdateRecruitable()
		{
			this.MaxConformity = this.Troop.Character.ConformityNeededToRecruitPrisoner;
			int elementXp = PartyBase.MainParty.PrisonRoster.GetElementXp(this.Troop.Character);
			this.CurrentConformity = ((elementXp >= this.Troop.Number * this.MaxConformity) ? this.MaxConformity : (elementXp % this.MaxConformity));
			this.IsRecruitablePrisoner = !this._character.IsHero && this.Type == PartyScreenLogic.TroopType.Prisoner;
			this.IsTroopRecruitable = this._partyScreenLogic.IsPrisonerRecruitable(this.Type, this.Character, this.Side);
			this.NumOfRecruitablePrisoners = this._partyScreenLogic.GetTroopRecruitableAmount(this.Character);
			GameTexts.SetVariable("LEFT", this.NumOfRecruitablePrisoners);
			GameTexts.SetVariable("RIGHT", this.Troop.Number);
			this.StrNumOfRecruitableTroop = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		private void OnTradeApplyTransaction(int amount, bool isIncreasing)
		{
			this.TransferAmount = amount;
			PartyScreenLogic.PartyRosterSide partyRosterSide = (isIncreasing ? PartyScreenLogic.PartyRosterSide.Left : PartyScreenLogic.PartyRosterSide.Right);
			this.ApplyTransfer(this.TransferAmount, partyRosterSide);
			this.IsExecutable = this._partyScreenLogic.IsExecutable(this.Type, this.Character, this.Side) && this.Troop.Number > 0;
		}

		public void InitializeUpgrades()
		{
			if (this.IsUpgradableTroop)
			{
				for (int i = 0; i < this.Character.UpgradeTargets.Length; i++)
				{
					CharacterObject characterObject = this.Character.UpgradeTargets[i];
					int level = characterObject.Level;
					int upgradeGoldCost = this.Character.GetUpgradeGoldCost(PartyBase.MainParty, i);
					if (!this.Character.Culture.IsBandit)
					{
						int level2 = this.Character.Level;
					}
					else
					{
						int level3 = this.Character.Level;
					}
					PerkObject perkObject;
					bool flag = Campaign.Current.Models.PartyTroopUpgradeModel.DoesPartyHaveRequiredPerksForUpgrade(PartyBase.MainParty, this.Character, characterObject, out perkObject);
					int num = (flag ? this.Troop.Number : 0);
					bool flag2 = true;
					int numOfCategoryItemPartyHas = this.GetNumOfCategoryItemPartyHas(this._partyScreenLogic.RightOwnerParty.ItemRoster, characterObject.UpgradeRequiresItemFromCategory);
					if (characterObject.UpgradeRequiresItemFromCategory != null)
					{
						flag2 = numOfCategoryItemPartyHas > 0;
					}
					bool flag3 = Hero.MainHero.Gold + this._partyScreenLogic.CurrentData.PartyGoldChangeAmount >= upgradeGoldCost;
					bool flag4 = level >= this.Character.Level && this.Troop.Xp >= this.Character.GetUpgradeXpCost(PartyBase.MainParty, i) && !this._partyVm.PartyScreenLogic.IsTroopUpgradesDisabled;
					bool flag5 = !flag2 || !flag3;
					int num2 = this.Troop.Number;
					if (upgradeGoldCost > 0)
					{
						num2 = (int)MathF.Clamp((float)MathF.Floor((float)(Hero.MainHero.Gold + this._partyScreenLogic.CurrentData.PartyGoldChangeAmount) / (float)upgradeGoldCost), 0f, (float)this.Troop.Number);
					}
					int num3 = ((characterObject.UpgradeRequiresItemFromCategory != null) ? numOfCategoryItemPartyHas : this.Troop.Number);
					int num4 = (flag4 ? ((int)MathF.Clamp((float)MathF.Floor((float)this.Troop.Xp / (float)this.Character.GetUpgradeXpCost(PartyBase.MainParty, i)), 0f, (float)this.Troop.Number)) : 0);
					int num5 = MathF.Min(MathF.Min(num2, num3), MathF.Min(num4, num));
					if (this.Character.Culture.IsBandit)
					{
						flag5 = flag5 || !Campaign.Current.Models.PartyTroopUpgradeModel.CanPartyUpgradeTroopToTarget(PartyBase.MainParty, this.Character, characterObject);
						num5 = ((!flag4) ? 0 : num5);
					}
					string upgradeHint = CampaignUIHelper.GetUpgradeHint(i, numOfCategoryItemPartyHas, num5, upgradeGoldCost, flag, perkObject, this.Character, this.Troop, this._partyScreenLogic.CurrentData.PartyGoldChangeAmount, PartyCharacterVM.EntireStackShortcutKeyText, PartyCharacterVM.FiveStackShortcutKeyText);
					this.Upgrades[i].Refresh(num5, upgradeHint, flag4, flag5, flag2, flag);
					if (i == 0)
					{
						this.UpgradeCostText = upgradeGoldCost.ToString();
						this.HasEnoughGold = flag3;
						this.NumOfReadyToUpgradeTroops = num4;
						this.MaxXP = this.Character.GetUpgradeXpCost(PartyBase.MainParty, i);
						this.CurrentXP = ((this.Troop.Xp >= this.Troop.Number * this.MaxXP) ? this.MaxXP : (this.Troop.Xp % this.MaxXP));
					}
				}
				this.AnyUpgradeHasRequirement = this.Upgrades.Any((UpgradeTargetVM x) => x.Requirements.HasItemRequirement || x.Requirements.HasPerkRequirement);
			}
			int num6 = 0;
			foreach (UpgradeTargetVM upgradeTargetVM in this.Upgrades)
			{
				if (upgradeTargetVM.AvailableUpgrades > num6)
				{
					num6 = upgradeTargetVM.AvailableUpgrades;
				}
			}
			this.NumOfUpgradeableTroops = num6;
			this.IsTroopUpgradable = this.NumOfUpgradeableTroops > 0 && !this._partyVm.PartyScreenLogic.IsTroopUpgradesDisabled;
			GameTexts.SetVariable("LEFT", this.NumOfReadyToUpgradeTroops);
			GameTexts.SetVariable("RIGHT", this.Troop.Number);
			this.StrNumOfUpgradableTroop = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			base.OnPropertyChanged("AmountOfUpgrades");
		}

		public void OnTransferred()
		{
			if (this.Side != PartyScreenLogic.PartyRosterSide.Left || this.IsPrisoner)
			{
				this.InitializeUpgrades();
				return;
			}
			PartyCharacterVM partyCharacterVM = this._partyVm.MainPartyTroops.FirstOrDefault((PartyCharacterVM x) => x.Character == this.Character);
			if (partyCharacterVM == null)
			{
				return;
			}
			partyCharacterVM.InitializeUpgrades();
		}

		public void ThrowOnPropertyChanged()
		{
			base.OnPropertyChanged("Name");
			base.OnPropertyChanged("Number");
			base.OnPropertyChanged("WoundedCount");
			base.OnPropertyChanged("IsTroopTransferrable");
			base.OnPropertyChanged("MaxCount");
			base.OnPropertyChanged("AmountOfUpgrades");
			base.OnPropertyChanged("Level");
			base.OnPropertyChanged("PartyIndex");
			base.OnPropertyChanged("Index");
			base.OnPropertyChanged("TroopNum");
			base.OnPropertyChanged("TransferString");
			base.OnPropertyChanged("CanTalk");
		}

		public override bool Equals(object obj)
		{
			PartyCharacterVM partyCharacterVM;
			return obj != null && (partyCharacterVM = obj as PartyCharacterVM) != null && ((partyCharacterVM.Character == null && this.Code == null) || partyCharacterVM.Character == this.Character);
		}

		private void ApplyTransfer(int transferAmount, PartyScreenLogic.PartyRosterSide side)
		{
			PartyCharacterVM.OnTransfer(this, -1, transferAmount, side);
			this.ThrowOnPropertyChanged();
		}

		private void ExecuteTransfer()
		{
			this.ApplyTransfer(this.TransferAmount, this.Side);
		}

		private void ExecuteTransferAll()
		{
			this.ApplyTransfer(this.Troop.Number, this.Side);
		}

		public void ExecuteSetFocused()
		{
			Action<PartyCharacterVM> onFocus = PartyCharacterVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		public void ExecuteSetUnfocused()
		{
			Action<PartyCharacterVM> onFocus = PartyCharacterVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		public void ExecuteTransferSingle()
		{
			int num = 1;
			if (this._partyVm.IsEntireStackModifierActive)
			{
				num = this.Troop.Number;
			}
			else if (this._partyVm.IsFiveStackModifierActive)
			{
				num = MathF.Min(5, this.Troop.Number);
			}
			this.ApplyTransfer(num, this.Side);
			this._partyVm.ExecuteRemoveZeroCounts();
		}

		public void ExecuteResetTrade()
		{
			this.TradeData.ExecuteReset();
		}

		public void Upgrade(int upgradeIndex, int maxUpgradeCount)
		{
			PartyVM partyVm = this._partyVm;
			if (partyVm == null)
			{
				return;
			}
			partyVm.ExecuteUpgrade(this, upgradeIndex, maxUpgradeCount);
		}

		public void FocusUpgrade(UpgradeTargetVM upgrade)
		{
			this._partyVm.CurrentFocusedUpgrade = upgrade;
		}

		public void RecruitAll()
		{
			if (this.IsTroopRecruitable)
			{
				this._partyVm.ExecuteRecruit(this, true);
			}
		}

		public void ExecuteRecruitTroop()
		{
			if (this.IsTroopRecruitable)
			{
				this._partyVm.ExecuteRecruit(this, false);
			}
		}

		public void ExecuteExecuteTroop()
		{
			if (this.IsExecutable)
			{
				if (FaceGen.GetMaturityTypeWithAge(this.Character.HeroObject.BodyProperties.Age) <= BodyMeshMaturityType.Tween)
				{
					return;
				}
				MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(this.Character.HeroObject, delegate
				{
					this._partyVm.ExecuteExecution();
				}, SceneNotificationData.RelevantContextType.Any));
			}
		}

		public void ExecuteOpenTroopEncyclopedia()
		{
			if (!this.Troop.Character.IsHero)
			{
				if (Campaign.Current.EncyclopediaManager.GetPageOf(typeof(CharacterObject)).IsValidEncyclopediaItem(this.Troop.Character))
				{
					Campaign.Current.EncyclopediaManager.GoToLink(this.Troop.Character.EncyclopediaLink);
					return;
				}
			}
			else if (Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero)).IsValidEncyclopediaItem(this.Troop.Character.HeroObject))
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Troop.Character.HeroObject.EncyclopediaLink);
			}
		}

		private CharacterCode GetCharacterCode(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side)
		{
			IFaction faction = null;
			if (type != PartyScreenLogic.TroopType.Prisoner)
			{
				if (side == PartyScreenLogic.PartyRosterSide.Left && this._partyScreenLogic.LeftOwnerParty != null)
				{
					faction = this._partyScreenLogic.LeftOwnerParty.MapFaction;
				}
				else if (this.Side == PartyScreenLogic.PartyRosterSide.Right && this._partyScreenLogic.RightOwnerParty != null)
				{
					faction = this._partyScreenLogic.RightOwnerParty.MapFaction;
				}
			}
			uint num = Color.White.ToUnsignedInteger();
			uint num2 = Color.White.ToUnsignedInteger();
			if (faction != null)
			{
				num = faction.Color;
				num2 = faction.Color2;
			}
			else if (character.Culture != null)
			{
				num = character.Culture.Color;
				num2 = character.Culture.Color2;
			}
			Equipment equipment = character.Equipment;
			string text = ((equipment != null) ? equipment.CalculateEquipmentCode() : null);
			BodyProperties bodyProperties = character.GetBodyProperties(character.Equipment, -1);
			return CharacterCode.CreateFrom(text, bodyProperties, character.IsFemale, character.IsHero, num, num2, character.DefaultFormationClass, character.Race);
		}

		private void SetMoraleCost()
		{
			if (this.IsTroopRecruitable)
			{
				this.RecruitMoraleCostText = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetPrisonerRecruitmentMoraleEffect(this._partyScreenLogic.RightOwnerParty, this.Character, 1).ToString();
			}
		}

		public void SetIsUpgradeButtonHighlighted(bool isHighlighted)
		{
			MBBindingList<UpgradeTargetVM> upgrades = this.Upgrades;
			if (upgrades == null)
			{
				return;
			}
			upgrades.ApplyActionOnAllItems(delegate(UpgradeTargetVM x)
			{
				x.IsHighlighted = isHighlighted;
			});
		}

		public int GetNumOfCategoryItemPartyHas(ItemRoster items, ItemCategory itemCategory)
		{
			int num = 0;
			foreach (ItemRosterElement itemRosterElement in items)
			{
				if (itemRosterElement.EquipmentElement.Item.ItemCategory == itemCategory)
				{
					num += itemRosterElement.Amount;
				}
			}
			return num;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[DataSourceProperty]
		public bool IsFormationEnabled
		{
			get
			{
				return this._isFormationEnabled;
			}
			set
			{
				if (this._isFormationEnabled != value)
				{
					this._isFormationEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsFormationEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string TransferString
		{
			get
			{
				return this.TransferAmount.ToString() + "/" + this.Number.ToString();
			}
		}

		[DataSourceProperty]
		public bool IsTroopUpgradable
		{
			get
			{
				return this._isTroopUpgradable;
			}
			set
			{
				if (value != this._isTroopUpgradable)
				{
					this._isTroopUpgradable = value;
					base.OnPropertyChangedWithValue(value, "IsTroopUpgradable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTroopRecruitable
		{
			get
			{
				return this._isTroopRecruitable;
			}
			set
			{
				if (value != this._isTroopRecruitable)
				{
					this._isTroopRecruitable = value;
					base.OnPropertyChangedWithValue(value, "IsTroopRecruitable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRecruitablePrisoner
		{
			get
			{
				return this._isRecruitablePrisoner;
			}
			set
			{
				if (value != this._isRecruitablePrisoner)
				{
					this._isRecruitablePrisoner = value;
					base.OnPropertyChangedWithValue(value, "IsRecruitablePrisoner");
				}
			}
		}

		[DataSourceProperty]
		public bool IsUpgradableTroop
		{
			get
			{
				return this._isUpgradableTroop;
			}
			set
			{
				if (value != this._isUpgradableTroop)
				{
					this._isUpgradableTroop = value;
					base.OnPropertyChangedWithValue(value, "IsUpgradableTroop");
				}
			}
		}

		[DataSourceProperty]
		public bool IsExecutable
		{
			get
			{
				return this._isExecutable;
			}
			set
			{
				if (value != this._isExecutable)
				{
					this._isExecutable = value;
					base.OnPropertyChangedWithValue(value, "IsExecutable");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfReadyToUpgradeTroops
		{
			get
			{
				return this._numOfReadyToUpgradeTroops;
			}
			set
			{
				if (value != this._numOfReadyToUpgradeTroops)
				{
					this._numOfReadyToUpgradeTroops = value;
					base.OnPropertyChangedWithValue(value, "NumOfReadyToUpgradeTroops");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfUpgradeableTroops
		{
			get
			{
				return this._numOfUpgradeableTroops;
			}
			set
			{
				if (value != this._numOfUpgradeableTroops)
				{
					this._numOfUpgradeableTroops = value;
					base.OnPropertyChangedWithValue(value, "NumOfUpgradeableTroops");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfRecruitablePrisoners
		{
			get
			{
				return this._numOfRecruitablePrisoners;
			}
			set
			{
				if (value != this._numOfRecruitablePrisoners)
				{
					this._numOfRecruitablePrisoners = value;
					base.OnPropertyChangedWithValue(value, "NumOfRecruitablePrisoners");
				}
			}
		}

		[DataSourceProperty]
		public int MaxXP
		{
			get
			{
				return this._maxXP;
			}
			set
			{
				if (value != this._maxXP)
				{
					this._maxXP = value;
					base.OnPropertyChangedWithValue(value, "MaxXP");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentXP
		{
			get
			{
				return this._currentXP;
			}
			set
			{
				if (value != this._currentXP)
				{
					this._currentXP = value;
					base.OnPropertyChangedWithValue(value, "CurrentXP");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentConformity
		{
			get
			{
				return this._currentConformity;
			}
			set
			{
				if (value != this._currentConformity)
				{
					this._currentConformity = value;
					base.OnPropertyChangedWithValue(value, "CurrentConformity");
				}
			}
		}

		[DataSourceProperty]
		public int MaxConformity
		{
			get
			{
				return this._maxConformity;
			}
			set
			{
				if (value != this._maxConformity)
				{
					this._maxConformity = value;
					base.OnPropertyChangedWithValue(value, "MaxConformity");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TroopXPTooltip
		{
			get
			{
				return this._troopXPTooltip;
			}
			set
			{
				if (value != this._troopXPTooltip)
				{
					this._troopXPTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopXPTooltip");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TroopConformityTooltip
		{
			get
			{
				return this._troopConformityTooltip;
			}
			set
			{
				if (value != this._troopConformityTooltip)
				{
					this._troopConformityTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopConformityTooltip");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel TransferHint
		{
			get
			{
				return this._transferHint;
			}
			set
			{
				if (value != this._transferHint)
				{
					this._transferHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TransferHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRecruitButtonsHiglighted
		{
			get
			{
				return this._isRecruitButtonsHiglighted;
			}
			set
			{
				if (value != this._isRecruitButtonsHiglighted)
				{
					this._isRecruitButtonsHiglighted = value;
					base.OnPropertyChangedWithValue(value, "IsRecruitButtonsHiglighted");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTransferButtonHiglighted
		{
			get
			{
				return this._isTransferButtonHiglighted;
			}
			set
			{
				if (value != this._isTransferButtonHiglighted)
				{
					this._isTransferButtonHiglighted = value;
					base.OnPropertyChangedWithValue(value, "IsTransferButtonHiglighted");
				}
			}
		}

		[DataSourceProperty]
		public string StrNumOfUpgradableTroop
		{
			get
			{
				return this._strNumOfUpgradableTroop;
			}
			set
			{
				if (value != this._strNumOfUpgradableTroop)
				{
					this._strNumOfUpgradableTroop = value;
					base.OnPropertyChangedWithValue<string>(value, "StrNumOfUpgradableTroop");
				}
			}
		}

		[DataSourceProperty]
		public string StrNumOfRecruitableTroop
		{
			get
			{
				return this._strNumOfRecruitableTroop;
			}
			set
			{
				if (value != this._strNumOfRecruitableTroop)
				{
					this._strNumOfRecruitableTroop = value;
					base.OnPropertyChangedWithValue<string>(value, "StrNumOfRecruitableTroop");
				}
			}
		}

		[DataSourceProperty]
		public string TroopID
		{
			get
			{
				return this._troopID;
			}
			set
			{
				if (value != this._troopID)
				{
					this._troopID = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopID");
				}
			}
		}

		[DataSourceProperty]
		public string UpgradeCostText
		{
			get
			{
				return this._upgradeCostText;
			}
			set
			{
				if (value != this._upgradeCostText)
				{
					this._upgradeCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "UpgradeCostText");
				}
			}
		}

		[DataSourceProperty]
		public string RecruitMoraleCostText
		{
			get
			{
				return this._recruitMoraleCostText;
			}
			set
			{
				if (value != this._recruitMoraleCostText)
				{
					this._recruitMoraleCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecruitMoraleCostText");
				}
			}
		}

		[DataSourceProperty]
		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (this._index != value)
				{
					this._index = value;
					base.OnPropertyChangedWithValue(value, "Index");
				}
			}
		}

		[DataSourceProperty]
		public int TransferAmount
		{
			get
			{
				return this._transferAmount;
			}
			set
			{
				if (value <= 0)
				{
					value = 1;
				}
				if (this._transferAmount != value)
				{
					this._transferAmount = value;
					base.OnPropertyChangedWithValue(value, "TransferAmount");
					base.OnPropertyChanged("TransferString");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTroopTransferrable
		{
			get
			{
				return this._isTroopTransferrable;
			}
			set
			{
				if (this.Character != CharacterObject.PlayerCharacter)
				{
					this._isTroopTransferrable = value;
					base.OnPropertyChangedWithValue(value, "IsTroopTransferrable");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string TroopNum
		{
			get
			{
				if (this.Character != null && this.Character.IsHero)
				{
					return "1";
				}
				if (this.Troop.Character == null)
				{
					return "-1";
				}
				int num = this.Troop.Number - this.Troop.WoundedNumber;
				string text = GameTexts.FindText("str_party_nameplate_wounded_abbr", null).ToString();
				if (num != this.Troop.Number && this.Type != PartyScreenLogic.TroopType.Prisoner)
				{
					return string.Concat(new object[]
					{
						num,
						"+",
						this.Troop.WoundedNumber,
						text
					});
				}
				return this.Troop.Number.ToString();
			}
		}

		[DataSourceProperty]
		public bool IsHeroWounded
		{
			get
			{
				CharacterObject character = this.Character;
				return character != null && character.IsHero && this.Character.HeroObject.IsWounded;
			}
		}

		[DataSourceProperty]
		public int HeroHealth
		{
			get
			{
				CharacterObject character = this.Character;
				if (character != null && character.IsHero)
				{
					return MathF.Ceiling((float)this.Character.HeroObject.HitPoints * 100f / (float)this.Character.MaxHitPoints());
				}
				return 0;
			}
		}

		[DataSourceProperty]
		public int Number
		{
			get
			{
				this.IsTroopTransferrable = this._initIsTroopTransferable && this.Troop.Number > 0;
				return this.Troop.Number;
			}
		}

		[DataSourceProperty]
		public int WoundedCount
		{
			get
			{
				if (this.Troop.Character == null)
				{
					return 0;
				}
				return this.Troop.WoundedNumber;
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel RecruitPrisonerHint
		{
			get
			{
				return this._recruitPrisonerHint;
			}
			set
			{
				if (value != this._recruitPrisonerHint)
				{
					this._recruitPrisonerHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "RecruitPrisonerHint");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Code
		{
			get
			{
				return this._code;
			}
			set
			{
				if (value != this._code)
				{
					this._code = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Code");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel ExecutePrisonerHint
		{
			get
			{
				return this._executePrisonerHint;
			}
			set
			{
				if (value != this._executePrisonerHint)
				{
					this._executePrisonerHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ExecutePrisonerHint");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<UpgradeTargetVM> Upgrades
		{
			get
			{
				return this._upgrades;
			}
			set
			{
				if (value != this._upgrades)
				{
					this._upgrades = value;
					base.OnPropertyChangedWithValue<MBBindingList<UpgradeTargetVM>>(value, "Upgrades");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel HeroHealthHint
		{
			get
			{
				return this._heroHealthHint;
			}
			set
			{
				if (value != this._heroHealthHint)
				{
					this._heroHealthHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "HeroHealthHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHero
		{
			get
			{
				return this._isHero;
			}
			set
			{
				if (value != this._isHero)
				{
					this._isHero = value;
					base.OnPropertyChangedWithValue(value, "IsHero");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		[DataSourceProperty]
		public bool HasMoreThanTwoUpgrades
		{
			get
			{
				return this._hasMoreThanTwoUpgrades;
			}
			set
			{
				if (value != this._hasMoreThanTwoUpgrades)
				{
					this._hasMoreThanTwoUpgrades = value;
					base.OnPropertyChangedWithValue(value, "HasMoreThanTwoUpgrades");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (value != this._isPrisoner)
				{
					this._isPrisoner = value;
					base.OnPropertyChangedWithValue(value, "IsPrisoner");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPrisonerOfPlayer
		{
			get
			{
				return this._isPrisonerOfPlayer;
			}
			set
			{
				if (value != this._isPrisonerOfPlayer)
				{
					this._isPrisonerOfPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsPrisonerOfPlayer");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHeroPrisonerOfPlayer
		{
			get
			{
				return this._isHeroPrisonerOfPlayer;
			}
			set
			{
				if (value != this._isHeroPrisonerOfPlayer)
				{
					this._isHeroPrisonerOfPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsHeroPrisonerOfPlayer");
				}
			}
		}

		[DataSourceProperty]
		public bool AnyUpgradeHasRequirement
		{
			get
			{
				return this._anyUpgradeHasRequirement;
			}
			set
			{
				if (value != this._anyUpgradeHasRequirement)
				{
					this._anyUpgradeHasRequirement = value;
					base.OnPropertyChangedWithValue(value, "AnyUpgradeHasRequirement");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
				}
			}
		}

		[DataSourceProperty]
		public bool HasEnoughGold
		{
			get
			{
				return this._hasEnoughGold;
			}
			set
			{
				if (value != this._hasEnoughGold)
				{
					this._hasEnoughGold = value;
					base.OnPropertyChangedWithValue(value, "HasEnoughGold");
				}
			}
		}

		[DataSourceProperty]
		public bool CanTalk
		{
			get
			{
				bool flag = this.Side == PartyScreenLogic.PartyRosterSide.Right;
				bool flag2 = this.Troop.Character != CharacterObject.PlayerCharacter;
				bool isHero = this.Troop.Character.IsHero;
				bool flag3 = CampaignMission.Current == null;
				bool flag4 = Settlement.CurrentSettlement == null;
				bool flag5 = MobileParty.MainParty.MapEvent == null;
				return flag2 && flag && isHero && flag3 && flag4 && flag5;
			}
		}

		[DataSourceProperty]
		public PartyTradeVM TradeData
		{
			get
			{
				return this._tradeData;
			}
			set
			{
				if (value != this._tradeData)
				{
					this._tradeData = value;
					base.OnPropertyChangedWithValue<PartyTradeVM>(value, "TradeData");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
					Action<PartyCharacterVM, bool> processCharacterLock = PartyCharacterVM.ProcessCharacterLock;
					if (processCharacterLock == null)
					{
						return;
					}
					processCharacterLock(this, value);
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LockHint
		{
			get
			{
				return this._lockHint;
			}
			set
			{
				if (value != this._lockHint)
				{
					this._lockHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LockHint");
				}
			}
		}

		public static bool IsShiftingDisabled;

		public static Action<PartyCharacterVM, bool> ProcessCharacterLock;

		public static Action<PartyCharacterVM> SetSelected;

		public static Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide> OnTransfer;

		public static Action<PartyCharacterVM> OnShift;

		public static Action<PartyCharacterVM> OnFocus;

		public static string FiveStackShortcutKeyText;

		public static string EntireStackShortcutKeyText;

		public readonly PartyScreenLogic.PartyRosterSide Side;

		public readonly PartyScreenLogic.TroopType Type;

		protected readonly PartyVM _partyVm;

		protected readonly PartyScreenLogic _partyScreenLogic;

		protected readonly bool _initIsTroopTransferable;

		private TroopRosterElement _troop;

		private CharacterObject _character;

		private string _name;

		private string _strNumOfUpgradableTroop;

		private string _strNumOfRecruitableTroop;

		private string _troopID;

		private string _upgradeCostText;

		private string _recruitMoraleCostText;

		private MBBindingList<UpgradeTargetVM> _upgrades;

		private ImageIdentifierVM _code = new ImageIdentifierVM(ImageIdentifierType.Null);

		public HintViewModel _transferHint;

		private BasicTooltipViewModel _recruitPrisonerHint;

		private BasicTooltipViewModel _executePrisonerHint;

		private BasicTooltipViewModel _heroHealthHint;

		private int _transferAmount = 1;

		private int _index = -2;

		private int _numOfReadyToUpgradeTroops;

		private int _numOfUpgradeableTroops;

		private int _numOfRecruitablePrisoners;

		private int _maxXP;

		private int _currentXP;

		private int _maxConformity;

		private int _currentConformity;

		public BasicTooltipViewModel _troopXPTooltip;

		public BasicTooltipViewModel _troopConformityTooltip;

		private bool _isHero;

		private bool _isMainHero;

		private bool _isPrisoner;

		private bool _isPrisonerOfPlayer;

		private bool _isRecruitablePrisoner;

		private bool _isUpgradableTroop;

		private bool _isTroopTransferrable;

		private bool _isHeroPrisonerOfPlayer;

		private bool _isTroopUpgradable;

		private StringItemWithHintVM _tierIconData;

		private bool _hasEnoughGold;

		private bool _anyUpgradeHasRequirement;

		private StringItemWithHintVM _typeIconData;

		private bool _hasMoreThanTwoUpgrades;

		private bool _isRecruitButtonsHiglighted;

		private bool _isTransferButtonHiglighted;

		private bool _isFormationEnabled;

		private PartyTradeVM _tradeData;

		private bool _isTroopRecruitable;

		private bool _isExecutable;

		private bool _isLocked;

		private HintViewModel _lockHint;
	}
}

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
	// Token: 0x02000023 RID: 35
	public class PartyCharacterVM : ViewModel
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600023F RID: 575 RVA: 0x00011145 File Offset: 0x0000F345
		// (set) Token: 0x06000240 RID: 576 RVA: 0x0001114D File Offset: 0x0000F34D
		public TroopRoster Troops { get; private set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00011156 File Offset: 0x0000F356
		// (set) Token: 0x06000242 RID: 578 RVA: 0x0001115E File Offset: 0x0000F35E
		public string StringId { get; private set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00011167 File Offset: 0x0000F367
		// (set) Token: 0x06000244 RID: 580 RVA: 0x00011170 File Offset: 0x0000F370
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

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000245 RID: 581 RVA: 0x000111EC File Offset: 0x0000F3EC
		// (set) Token: 0x06000246 RID: 582 RVA: 0x000111F4 File Offset: 0x0000F3F4
		public CharacterObject Character
		{
			get
			{
				return this._character;
			}
			set
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
				this.CheckTransferAmountDefaultValue();
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000112C4 File Offset: 0x0000F4C4
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
			this.InitializeUpgrades();
			this.ThrowOnPropertyChanged();
			this._initIsTroopTransferable = isTroopTransferrable;
			this.IsTroopTransferrable = this._initIsTroopTransferable;
			this.TradeData = new PartyTradeVM(partyScreenLogic, this.Troop, this.Side, this.IsTroopTransferrable, this.IsPrisoner, new Action<int, bool>(this.OnTradeApplyTransaction));
			this.IsPrisonerOfPlayer = this.IsPrisoner && this.Side == PartyScreenLogic.PartyRosterSide.Right;
			this.IsHeroPrisonerOfPlayer = this.IsPrisonerOfPlayer && this.Character.IsHero;
			this.IsExecutable = this._partyScreenLogic.IsExecutable(this.Type, this.Character, this.Side);
			this.IsUpgradableTroop = this.Side == PartyScreenLogic.PartyRosterSide.Right && this.Type == PartyScreenLogic.TroopType.Member && !this.IsHero && this.Character.UpgradeTargets.Length != 0;
			this.CheckTransferAmountDefaultValue();
			this.UpdateRecruitable();
			this.RefreshValues();
			this.UpdateTransferHint();
			this.SetMoraleCost();
			this.RecruitPrisonerHint = new BasicTooltipViewModel(() => this._partyScreenLogic.GetRecruitableReasonText(this.Troop.Character, this.IsRecruitablePrisoner, this.Troop.Number, PartyCharacterVM.FiveStackShortcutKeyText, PartyCharacterVM.EntireStackShortcutKeyText));
			this.ExecutePrisonerHint = new BasicTooltipViewModel(() => this._partyScreenLogic.GetExecutableReasonText(this.Troop.Character, this.IsExecutable));
			this.HeroHealthHint = (this.Troop.Character.IsHero ? new BasicTooltipViewModel(() => CampaignUIHelper.GetHeroHealthTooltip(this.Troop.Character.HeroObject)) : null);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0001150C File Offset: 0x0000F70C
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

		// Token: 0x06000249 RID: 585 RVA: 0x00011594 File Offset: 0x0000F794
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

		// Token: 0x0600024A RID: 586 RVA: 0x000116B8 File Offset: 0x0000F8B8
		private void CheckTransferAmountDefaultValue()
		{
			if (this.TransferAmount == 0 && this.Troop.Character != null && this.Troop.Number > 0)
			{
				this.TransferAmount = 1;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x000116F2 File Offset: 0x0000F8F2
		public void ExecuteSetSelected()
		{
			if (this.Character != null)
			{
				PartyCharacterVM.SetSelected(this);
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00011707 File Offset: 0x0000F907
		public void ExecuteTalk()
		{
			PartyVM partyVm = this._partyVm;
			if (partyVm == null)
			{
				return;
			}
			partyVm.ExecuteTalk();
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00011719 File Offset: 0x0000F919
		public void UpdateTradeData()
		{
			PartyTradeVM tradeData = this.TradeData;
			if (tradeData != null)
			{
				tradeData.UpdateTroopData(this.Troop, this.Side, true);
			}
			this.UpdateTransferHint();
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00011740 File Offset: 0x0000F940
		public void UpdateRecruitable()
		{
			this.MaxConformity = this.Troop.Character.ConformityNeededToRecruitPrisoner;
			this.CurrentConformity = PartyBase.MainParty.PrisonRoster.GetElementXp(this.Troop.Character) % this.MaxConformity;
			this.IsRecruitablePrisoner = !this._character.IsHero && this.Type == PartyScreenLogic.TroopType.Prisoner;
			this.IsTroopRecruitable = this._partyScreenLogic.IsPrisonerRecruitable(this.Type, this.Character, this.Side);
			this.NumOfRecruitablePrisoners = this._partyScreenLogic.GetTroopRecruitableAmount(this.Character);
			GameTexts.SetVariable("LEFT", this.NumOfRecruitablePrisoners);
			GameTexts.SetVariable("RIGHT", this.Troop.Number);
			this.StrNumOfRecruitableTroop = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00011824 File Offset: 0x0000FA24
		private void OnTradeApplyTransaction(int amount, bool isIncreasing)
		{
			this.TransferAmount = amount;
			PartyScreenLogic.PartyRosterSide partyRosterSide = (isIncreasing ? PartyScreenLogic.PartyRosterSide.Left : PartyScreenLogic.PartyRosterSide.Right);
			this.ApplyTransfer(this.TransferAmount, partyRosterSide);
			this.IsExecutable = this._partyScreenLogic.IsExecutable(this.Type, this.Character, this.Side) && this.Troop.Number > 0;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00011888 File Offset: 0x0000FA88
		public void InitializeUpgrades()
		{
			if (this.Side == PartyScreenLogic.PartyRosterSide.Right && !this.Character.IsHero && this.Character.UpgradeTargets.Length != 0 && !this.IsPrisoner && !this._partyScreenLogic.IsTroopUpgradesDisabled)
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

		// Token: 0x06000251 RID: 593 RVA: 0x00011D28 File Offset: 0x0000FF28
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

		// Token: 0x06000252 RID: 594 RVA: 0x00011D68 File Offset: 0x0000FF68
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

		// Token: 0x06000253 RID: 595 RVA: 0x00011DFC File Offset: 0x0000FFFC
		public override bool Equals(object obj)
		{
			PartyCharacterVM partyCharacterVM;
			return obj != null && (partyCharacterVM = obj as PartyCharacterVM) != null && ((partyCharacterVM.Character == null && this.Code == null) || partyCharacterVM.Character == this.Character);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00011E3A File Offset: 0x0001003A
		private void ApplyTransfer(int transferAmount, PartyScreenLogic.PartyRosterSide side)
		{
			PartyCharacterVM.OnTransfer(this, -1, transferAmount, side);
			this.ThrowOnPropertyChanged();
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00011E50 File Offset: 0x00010050
		private void ExecuteTransfer()
		{
			this.ApplyTransfer(this.TransferAmount, this.Side);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00011E64 File Offset: 0x00010064
		private void ExecuteTransferAll()
		{
			this.ApplyTransfer(this.Troop.Number, this.Side);
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00011E8B File Offset: 0x0001008B
		public void ExecuteSetFocused()
		{
			Action<PartyCharacterVM> onFocus = PartyCharacterVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00011E9D File Offset: 0x0001009D
		public void ExecuteSetUnfocused()
		{
			Action<PartyCharacterVM> onFocus = PartyCharacterVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00011EB0 File Offset: 0x000100B0
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

		// Token: 0x0600025A RID: 602 RVA: 0x00011F17 File Offset: 0x00010117
		public void ExecuteResetTrade()
		{
			this.TradeData.ExecuteReset();
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00011F24 File Offset: 0x00010124
		public void Upgrade(int upgradeIndex, int maxUpgradeCount)
		{
			PartyVM partyVm = this._partyVm;
			if (partyVm == null)
			{
				return;
			}
			partyVm.ExecuteUpgrade(this, upgradeIndex, maxUpgradeCount);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00011F39 File Offset: 0x00010139
		public void FocusUpgrade(UpgradeTargetVM upgrade)
		{
			this._partyVm.CurrentFocusedUpgrade = upgrade;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00011F47 File Offset: 0x00010147
		public void RecruitAll()
		{
			if (this.IsTroopRecruitable)
			{
				this._partyVm.ExecuteRecruit(this, true);
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00011F5E File Offset: 0x0001015E
		public void ExecuteRecruitTroop()
		{
			if (this.IsTroopRecruitable)
			{
				this._partyVm.ExecuteRecruit(this, false);
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00011F78 File Offset: 0x00010178
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

		// Token: 0x06000260 RID: 608 RVA: 0x00011FD0 File Offset: 0x000101D0
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

		// Token: 0x06000261 RID: 609 RVA: 0x00012090 File Offset: 0x00010290
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

		// Token: 0x06000262 RID: 610 RVA: 0x00012180 File Offset: 0x00010380
		private void SetMoraleCost()
		{
			if (this.IsTroopRecruitable)
			{
				this.RecruitMoraleCostText = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetPrisonerRecruitmentMoraleEffect(this._partyScreenLogic.RightOwnerParty, this.Character, 1).ToString();
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000121CC File Offset: 0x000103CC
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

		// Token: 0x06000264 RID: 612 RVA: 0x00012204 File Offset: 0x00010404
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

		// Token: 0x06000265 RID: 613 RVA: 0x0001226C File Offset: 0x0001046C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000266 RID: 614 RVA: 0x00012274 File Offset: 0x00010474
		// (set) Token: 0x06000267 RID: 615 RVA: 0x0001227C File Offset: 0x0001047C
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

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0001229C File Offset: 0x0001049C
		[DataSourceProperty]
		public string TransferString
		{
			get
			{
				return this.TransferAmount.ToString() + "/" + this.Number.ToString();
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000269 RID: 617 RVA: 0x000122CF File Offset: 0x000104CF
		// (set) Token: 0x0600026A RID: 618 RVA: 0x000122D7 File Offset: 0x000104D7
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

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600026B RID: 619 RVA: 0x000122F5 File Offset: 0x000104F5
		// (set) Token: 0x0600026C RID: 620 RVA: 0x000122FD File Offset: 0x000104FD
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

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0001231B File Offset: 0x0001051B
		// (set) Token: 0x0600026E RID: 622 RVA: 0x00012323 File Offset: 0x00010523
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

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600026F RID: 623 RVA: 0x00012341 File Offset: 0x00010541
		// (set) Token: 0x06000270 RID: 624 RVA: 0x00012349 File Offset: 0x00010549
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

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00012367 File Offset: 0x00010567
		// (set) Token: 0x06000272 RID: 626 RVA: 0x0001236F File Offset: 0x0001056F
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

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000273 RID: 627 RVA: 0x0001238D File Offset: 0x0001058D
		// (set) Token: 0x06000274 RID: 628 RVA: 0x00012395 File Offset: 0x00010595
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

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000275 RID: 629 RVA: 0x000123B3 File Offset: 0x000105B3
		// (set) Token: 0x06000276 RID: 630 RVA: 0x000123BB File Offset: 0x000105BB
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

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000277 RID: 631 RVA: 0x000123D9 File Offset: 0x000105D9
		// (set) Token: 0x06000278 RID: 632 RVA: 0x000123E1 File Offset: 0x000105E1
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

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000279 RID: 633 RVA: 0x000123FF File Offset: 0x000105FF
		// (set) Token: 0x0600027A RID: 634 RVA: 0x00012407 File Offset: 0x00010607
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

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600027B RID: 635 RVA: 0x00012425 File Offset: 0x00010625
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0001242D File Offset: 0x0001062D
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

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600027D RID: 637 RVA: 0x0001244B File Offset: 0x0001064B
		// (set) Token: 0x0600027E RID: 638 RVA: 0x00012453 File Offset: 0x00010653
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

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600027F RID: 639 RVA: 0x00012471 File Offset: 0x00010671
		// (set) Token: 0x06000280 RID: 640 RVA: 0x00012479 File Offset: 0x00010679
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

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000281 RID: 641 RVA: 0x00012497 File Offset: 0x00010697
		// (set) Token: 0x06000282 RID: 642 RVA: 0x0001249F File Offset: 0x0001069F
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

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000283 RID: 643 RVA: 0x000124BD File Offset: 0x000106BD
		// (set) Token: 0x06000284 RID: 644 RVA: 0x000124C5 File Offset: 0x000106C5
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

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000285 RID: 645 RVA: 0x000124E3 File Offset: 0x000106E3
		// (set) Token: 0x06000286 RID: 646 RVA: 0x000124EB File Offset: 0x000106EB
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

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00012509 File Offset: 0x00010709
		// (set) Token: 0x06000288 RID: 648 RVA: 0x00012511 File Offset: 0x00010711
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

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0001252F File Offset: 0x0001072F
		// (set) Token: 0x0600028A RID: 650 RVA: 0x00012537 File Offset: 0x00010737
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

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600028B RID: 651 RVA: 0x00012555 File Offset: 0x00010755
		// (set) Token: 0x0600028C RID: 652 RVA: 0x0001255D File Offset: 0x0001075D
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

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600028D RID: 653 RVA: 0x00012580 File Offset: 0x00010780
		// (set) Token: 0x0600028E RID: 654 RVA: 0x00012588 File Offset: 0x00010788
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

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600028F RID: 655 RVA: 0x000125AB File Offset: 0x000107AB
		// (set) Token: 0x06000290 RID: 656 RVA: 0x000125B3 File Offset: 0x000107B3
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

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000291 RID: 657 RVA: 0x000125D6 File Offset: 0x000107D6
		// (set) Token: 0x06000292 RID: 658 RVA: 0x000125DE File Offset: 0x000107DE
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

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000293 RID: 659 RVA: 0x00012601 File Offset: 0x00010801
		// (set) Token: 0x06000294 RID: 660 RVA: 0x00012609 File Offset: 0x00010809
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

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0001262C File Offset: 0x0001082C
		// (set) Token: 0x06000296 RID: 662 RVA: 0x00012634 File Offset: 0x00010834
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

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00012652 File Offset: 0x00010852
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0001265A File Offset: 0x0001085A
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

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0001268A File Offset: 0x0001088A
		// (set) Token: 0x0600029A RID: 666 RVA: 0x00012692 File Offset: 0x00010892
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

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600029B RID: 667 RVA: 0x000126B4 File Offset: 0x000108B4
		// (set) Token: 0x0600029C RID: 668 RVA: 0x000126BC File Offset: 0x000108BC
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

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600029D RID: 669 RVA: 0x000126E0 File Offset: 0x000108E0
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

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600029E RID: 670 RVA: 0x000127B4 File Offset: 0x000109B4
		[DataSourceProperty]
		public bool IsHeroWounded
		{
			get
			{
				CharacterObject character = this.Character;
				return character != null && character.IsHero && this.Character.HeroObject.IsWounded;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600029F RID: 671 RVA: 0x000127DC File Offset: 0x000109DC
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

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x00012828 File Offset: 0x00010A28
		[DataSourceProperty]
		public int Number
		{
			get
			{
				this.IsTroopTransferrable = this._initIsTroopTransferable && this.Troop.Number > 0;
				return this.Troop.Number;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x00012868 File Offset: 0x00010A68
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

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00012892 File Offset: 0x00010A92
		// (set) Token: 0x060002A3 RID: 675 RVA: 0x0001289A File Offset: 0x00010A9A
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

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x000128B8 File Offset: 0x00010AB8
		// (set) Token: 0x060002A5 RID: 677 RVA: 0x000128C0 File Offset: 0x00010AC0
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

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x000128DE File Offset: 0x00010ADE
		// (set) Token: 0x060002A7 RID: 679 RVA: 0x000128E6 File Offset: 0x00010AE6
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

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00012904 File Offset: 0x00010B04
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x0001290C File Offset: 0x00010B0C
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

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0001292A File Offset: 0x00010B2A
		// (set) Token: 0x060002AB RID: 683 RVA: 0x00012932 File Offset: 0x00010B32
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

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00012950 File Offset: 0x00010B50
		// (set) Token: 0x060002AD RID: 685 RVA: 0x00012958 File Offset: 0x00010B58
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

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00012976 File Offset: 0x00010B76
		// (set) Token: 0x060002AF RID: 687 RVA: 0x0001297E File Offset: 0x00010B7E
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

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0001299C File Offset: 0x00010B9C
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x000129A4 File Offset: 0x00010BA4
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x000129C2 File Offset: 0x00010BC2
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x000129CA File Offset: 0x00010BCA
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x000129E8 File Offset: 0x00010BE8
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x000129F0 File Offset: 0x00010BF0
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x00012A0E File Offset: 0x00010C0E
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x00012A16 File Offset: 0x00010C16
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

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00012A34 File Offset: 0x00010C34
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x00012A3C File Offset: 0x00010C3C
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

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00012A5A File Offset: 0x00010C5A
		// (set) Token: 0x060002BB RID: 699 RVA: 0x00012A62 File Offset: 0x00010C62
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

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00012A80 File Offset: 0x00010C80
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00012A88 File Offset: 0x00010C88
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

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00012AA6 File Offset: 0x00010CA6
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00012AAE File Offset: 0x00010CAE
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

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00012ACC File Offset: 0x00010CCC
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

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00012B35 File Offset: 0x00010D35
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00012B3D File Offset: 0x00010D3D
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

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00012B5B File Offset: 0x00010D5B
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x00012B63 File Offset: 0x00010D63
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

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x00012B92 File Offset: 0x00010D92
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x00012B9A File Offset: 0x00010D9A
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

		// Token: 0x04000106 RID: 262
		public static bool IsShiftingDisabled;

		// Token: 0x04000107 RID: 263
		public static Action<PartyCharacterVM, bool> ProcessCharacterLock;

		// Token: 0x04000108 RID: 264
		public static Action<PartyCharacterVM> SetSelected;

		// Token: 0x04000109 RID: 265
		public static Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide> OnTransfer;

		// Token: 0x0400010A RID: 266
		public static Action<PartyCharacterVM> OnShift;

		// Token: 0x0400010B RID: 267
		public static Action<PartyCharacterVM> OnFocus;

		// Token: 0x0400010C RID: 268
		public static string FiveStackShortcutKeyText;

		// Token: 0x0400010D RID: 269
		public static string EntireStackShortcutKeyText;

		// Token: 0x0400010E RID: 270
		protected readonly PartyVM _partyVm;

		// Token: 0x0400010F RID: 271
		protected readonly PartyScreenLogic _partyScreenLogic;

		// Token: 0x04000110 RID: 272
		protected readonly bool _initIsTroopTransferable;

		// Token: 0x04000112 RID: 274
		public readonly PartyScreenLogic.PartyRosterSide Side;

		// Token: 0x04000113 RID: 275
		public readonly PartyScreenLogic.TroopType Type;

		// Token: 0x04000115 RID: 277
		private TroopRosterElement _troop;

		// Token: 0x04000116 RID: 278
		private CharacterObject _character;

		// Token: 0x04000117 RID: 279
		private string _name;

		// Token: 0x04000118 RID: 280
		private string _strNumOfUpgradableTroop;

		// Token: 0x04000119 RID: 281
		private string _strNumOfRecruitableTroop;

		// Token: 0x0400011A RID: 282
		private string _troopID;

		// Token: 0x0400011B RID: 283
		private string _upgradeCostText;

		// Token: 0x0400011C RID: 284
		private string _recruitMoraleCostText;

		// Token: 0x0400011D RID: 285
		private MBBindingList<UpgradeTargetVM> _upgrades;

		// Token: 0x0400011E RID: 286
		private ImageIdentifierVM _code = new ImageIdentifierVM(ImageIdentifierType.Null);

		// Token: 0x0400011F RID: 287
		public HintViewModel _transferHint;

		// Token: 0x04000120 RID: 288
		private BasicTooltipViewModel _recruitPrisonerHint;

		// Token: 0x04000121 RID: 289
		private BasicTooltipViewModel _executePrisonerHint;

		// Token: 0x04000122 RID: 290
		private BasicTooltipViewModel _heroHealthHint;

		// Token: 0x04000123 RID: 291
		private int _transferAmount = 1;

		// Token: 0x04000124 RID: 292
		private int _index = -2;

		// Token: 0x04000125 RID: 293
		private int _numOfReadyToUpgradeTroops;

		// Token: 0x04000126 RID: 294
		private int _numOfUpgradeableTroops;

		// Token: 0x04000127 RID: 295
		private int _numOfRecruitablePrisoners;

		// Token: 0x04000128 RID: 296
		private int _maxXP;

		// Token: 0x04000129 RID: 297
		private int _currentXP;

		// Token: 0x0400012A RID: 298
		private int _maxConformity;

		// Token: 0x0400012B RID: 299
		private int _currentConformity;

		// Token: 0x0400012C RID: 300
		public BasicTooltipViewModel _troopXPTooltip;

		// Token: 0x0400012D RID: 301
		public BasicTooltipViewModel _troopConformityTooltip;

		// Token: 0x0400012E RID: 302
		private bool _isHero;

		// Token: 0x0400012F RID: 303
		private bool _isMainHero;

		// Token: 0x04000130 RID: 304
		private bool _isPrisoner;

		// Token: 0x04000131 RID: 305
		private bool _isPrisonerOfPlayer;

		// Token: 0x04000132 RID: 306
		private bool _isRecruitablePrisoner;

		// Token: 0x04000133 RID: 307
		private bool _isUpgradableTroop;

		// Token: 0x04000134 RID: 308
		private bool _isTroopTransferrable;

		// Token: 0x04000135 RID: 309
		private bool _isHeroPrisonerOfPlayer;

		// Token: 0x04000136 RID: 310
		private bool _isTroopUpgradable;

		// Token: 0x04000137 RID: 311
		private StringItemWithHintVM _tierIconData;

		// Token: 0x04000138 RID: 312
		private bool _hasEnoughGold;

		// Token: 0x04000139 RID: 313
		private bool _anyUpgradeHasRequirement;

		// Token: 0x0400013A RID: 314
		private StringItemWithHintVM _typeIconData;

		// Token: 0x0400013B RID: 315
		private bool _hasMoreThanTwoUpgrades;

		// Token: 0x0400013C RID: 316
		private bool _isRecruitButtonsHiglighted;

		// Token: 0x0400013D RID: 317
		private bool _isTransferButtonHiglighted;

		// Token: 0x0400013E RID: 318
		private bool _isFormationEnabled;

		// Token: 0x0400013F RID: 319
		private PartyTradeVM _tradeData;

		// Token: 0x04000140 RID: 320
		private bool _isTroopRecruitable;

		// Token: 0x04000141 RID: 321
		private bool _isExecutable;

		// Token: 0x04000142 RID: 322
		private bool _isLocked;

		// Token: 0x04000143 RID: 323
		private HintViewModel _lockHint;
	}
}

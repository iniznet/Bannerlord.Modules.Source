using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	// Token: 0x0200009F RID: 159
	[MenuOverlay("EncounterMenuOverlay")]
	public class EncounterMenuOverlayVM : GameMenuOverlay
	{
		// Token: 0x06000FC9 RID: 4041 RVA: 0x0003D784 File Offset: 0x0003B984
		public EncounterMenuOverlayVM()
		{
			this.AttackerPartyList = new MBBindingList<GameMenuPartyItemVM>();
			this.DefenderPartyList = new MBBindingList<GameMenuPartyItemVM>();
			base.CurrentOverlayType = 1;
			this.AttackerMoraleHint = new BasicTooltipViewModel(() => this.GetEncounterSideMoraleTooltip(BattleSideEnum.Attacker));
			this.DefenderMoraleHint = new BasicTooltipViewModel(() => this.GetEncounterSideMoraleTooltip(BattleSideEnum.Defender));
			this.AttackerFoodHint = new BasicTooltipViewModel(() => this.GetEncounterSideFoodTooltip(BattleSideEnum.Attacker));
			this.DefenderFoodHint = new BasicTooltipViewModel(() => this.GetEncounterSideFoodTooltip(BattleSideEnum.Defender));
			this.DefenderWallHint = new BasicTooltipViewModel();
			base.IsInitializationOver = false;
			this.UpdateLists();
			this.UpdateProperties();
			base.IsInitializationOver = true;
			this.RefreshValues();
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0003D83C File Offset: 0x0003BA3C
		private void SetAttackerAndDefenderParties(out bool attackerChanged, out bool defenderChanged)
		{
			attackerChanged = false;
			defenderChanged = false;
			if (MobileParty.MainParty.MapEvent != null)
			{
				PartyBase leaderParty = MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker);
				if (leaderParty.IsSettlement)
				{
					if (this._attackerLeadingParty == null || this._attackerLeadingParty.Party != leaderParty)
					{
						attackerChanged = true;
						this._attackerLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), leaderParty.Settlement);
					}
				}
				else if (this._attackerLeadingParty == null || this._attackerLeadingParty.Party != leaderParty)
				{
					attackerChanged = true;
					this._attackerLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), leaderParty, false);
				}
				PartyBase leaderParty2 = MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender);
				if (leaderParty2.IsSettlement)
				{
					if (this._defenderLeadingParty == null || this._defenderLeadingParty.Party != leaderParty2)
					{
						defenderChanged = true;
						this._defenderLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), leaderParty2.Settlement);
						return;
					}
				}
				else if (this._defenderLeadingParty == null || this._defenderLeadingParty.Party != leaderParty2)
				{
					defenderChanged = true;
					this._defenderLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), leaderParty2, false);
					return;
				}
			}
			else
			{
				Settlement settlement = Settlement.CurrentSettlement ?? PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
				SiegeEvent siegeEvent = settlement.SiegeEvent;
				if (siegeEvent != null)
				{
					if (this._defenderLeadingParty == null || this._defenderLeadingParty.Settlement != settlement)
					{
						defenderChanged = true;
						this._defenderLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), settlement);
					}
					if (this._attackerLeadingParty == null || this._attackerLeadingParty.Party != siegeEvent.BesiegerCamp.BesiegerParty.Party)
					{
						attackerChanged = true;
						this._attackerLeadingParty = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), siegeEvent.BesiegerCamp.BesiegerParty.Party, false);
					}
					this.DefenderWallHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetSiegeWallTooltip(settlement.Town.GetWallLevel(), MathF.Ceiling(settlement.SettlementTotalWallHitPoints)));
					this.IsSiege = true;
					return;
				}
				Debug.FailedAssert("Encounter overlay is open but MapEvent AND SiegeEvent is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\EncounterMenuOverlayVM.cs", "SetAttackerAndDefenderParties", 113);
			}
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0003DA64 File Offset: 0x0003BC64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AttackerBannerHint = new HintViewModel(GameTexts.FindText("str_attacker_banner", null), null);
			this.DefenderBannerHint = new HintViewModel(GameTexts.FindText("str_defender_banner", null), null);
			this.AttackerTroopNumHint = new HintViewModel(GameTexts.FindText("str_number_of_healthy_attacker_soldiers", null), null);
			this.DefenderTroopNumHint = new HintViewModel(GameTexts.FindText("str_number_of_healthy_defender_soldiers", null), null);
			base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", GameMenuOverlay.MenuOverlayContextList.Encyclopedia.ToString()).ToString(), true, GameMenuOverlay.MenuOverlayContextList.Encyclopedia, null));
			this.AttackerPartyList.ApplyActionOnAllItems(delegate(GameMenuPartyItemVM x)
			{
				x.RefreshValues();
			});
			this.DefenderPartyList.ApplyActionOnAllItems(delegate(GameMenuPartyItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x0003DB6C File Offset: 0x0003BD6C
		public override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (MobileParty.MainParty.MapEvent != null && this.AttackerPartyList.Count + this.DefenderPartyList.Count != MobileParty.MainParty.MapEvent.InvolvedParties.Count<PartyBase>())
			{
				this.UpdateLists();
			}
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0003DBBF File Offset: 0x0003BDBF
		public override void Refresh()
		{
			base.IsInitializationOver = false;
			this.UpdateLists();
			this.UpdateProperties();
			base.IsInitializationOver = true;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0003DBDC File Offset: 0x0003BDDC
		private void UpdateProperties()
		{
			if (this.IsSiege)
			{
				GameMenuPartyItemVM defenderLeadingParty = this._defenderLeadingParty;
				bool flag = ((defenderLeadingParty != null) ? defenderLeadingParty.Settlement : null) != null;
				float num = 0f;
				float num2 = 0f;
				if (flag)
				{
					ValueTuple<int, int> townFoodAndMarketStocks = TownHelpers.GetTownFoodAndMarketStocks((flag ? this._defenderLeadingParty : this._attackerLeadingParty).Settlement.Town);
					num2 = (float)(townFoodAndMarketStocks.Item1 + townFoodAndMarketStocks.Item2) / -this._defenderLeadingParty.Settlement.Town.FoodChangeWithoutMarketStocks;
				}
				foreach (GameMenuPartyItemVM gameMenuPartyItemVM in this.DefenderPartyList)
				{
					num += gameMenuPartyItemVM.Party.MobileParty.Morale;
					if (!flag)
					{
						num2 += gameMenuPartyItemVM.Party.MobileParty.Food / -gameMenuPartyItemVM.Party.MobileParty.FoodChange;
					}
				}
				num /= (float)this.DefenderPartyList.Count;
				if (!flag)
				{
					num2 /= (float)this.AttackerPartyList.Count;
				}
				num2 = (float)Math.Max((int)Math.Ceiling((double)num2), 0);
				MBTextManager.SetTextVariable("DAY_NUM", num2.ToString(), false);
				MBTextManager.SetTextVariable("PLURAL", (num2 > 1f) ? 1 : 0);
				this.DefenderPartyFood = GameTexts.FindText("str_party_food_left", null).ToString();
				this.DefenderPartyMorale = num.ToString("0.0");
				num = 0f;
				num2 = 0f;
				if (!flag)
				{
					if (this._attackerLeadingParty.Settlement != null)
					{
						num2 = this._attackerLeadingParty.Settlement.Town.FoodStocks / this._attackerLeadingParty.Settlement.Town.FoodChangeWithoutMarketStocks;
					}
					else if (this._attackerLeadingParty.Party.MobileParty.CurrentSettlement != null)
					{
						num2 = this._attackerLeadingParty.Party.MobileParty.CurrentSettlement.Town.FoodStocks / this._attackerLeadingParty.Party.MobileParty.CurrentSettlement.Town.FoodChangeWithoutMarketStocks;
					}
					else
					{
						Settlement currentSettlement = Settlement.CurrentSettlement;
						if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null)
						{
							num2 = Settlement.CurrentSettlement.Town.FoodStocks / Settlement.CurrentSettlement.Town.FoodChangeWithoutMarketStocks;
						}
						else
						{
							Debug.FailedAssert("There are no settlements involved in the siege", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\EncounterMenuOverlayVM.cs", "UpdateProperties", 207);
						}
					}
				}
				else
				{
					Settlement currentSettlement2 = Settlement.CurrentSettlement;
					if (((currentSettlement2 != null) ? currentSettlement2.SiegeEvent : null) != null)
					{
						num2 = Settlement.CurrentSettlement.Town.FoodStocks / Settlement.CurrentSettlement.Town.FoodChangeWithoutMarketStocks;
					}
				}
				foreach (GameMenuPartyItemVM gameMenuPartyItemVM2 in this.AttackerPartyList)
				{
					num += gameMenuPartyItemVM2.Party.MobileParty.Morale;
					if (flag)
					{
						num2 += gameMenuPartyItemVM2.Party.MobileParty.Food / -gameMenuPartyItemVM2.Party.MobileParty.FoodChange;
					}
				}
				num /= (float)this.AttackerPartyList.Count;
				if (flag)
				{
					num2 /= (float)this.AttackerPartyList.Count;
				}
				num2 = (float)Math.Max((int)Math.Ceiling((double)num2), 0);
				MBTextManager.SetTextVariable("DAY_NUM", num2.ToString(), false);
				MBTextManager.SetTextVariable("PLURAL", (num2 > 1f) ? 1 : 0);
				this.AttackerPartyFood = GameTexts.FindText("str_party_food_left", null).ToString();
				this.AttackerPartyMorale = num.ToString("0.0");
				Settlement settlement;
				if ((settlement = Settlement.CurrentSettlement) == null)
				{
					SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
					settlement = ((playerSiegeEvent != null) ? playerSiegeEvent.BesiegedSettlement : null);
				}
				Settlement settlement2 = settlement;
				if (settlement2 != null)
				{
					this.DefenderWallHitPoints = MathF.Ceiling(settlement2.SettlementTotalWallHitPoints).ToString();
				}
			}
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0003DFC4 File Offset: 0x0003C1C4
		private void UpdateLists()
		{
			if (MobileParty.MainParty.MapEvent == null)
			{
				Settlement settlement;
				if ((settlement = Settlement.CurrentSettlement) == null)
				{
					SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
					settlement = ((playerSiegeEvent != null) ? playerSiegeEvent.BesiegedSettlement : null);
				}
				if (settlement == null)
				{
					return;
				}
			}
			bool flag;
			bool flag2;
			this.SetAttackerAndDefenderParties(out flag, out flag2);
			if (this._defenderLeadingParty != null && flag2)
			{
				this.DefenderPartyList.Insert(0, this._defenderLeadingParty);
			}
			if (this._attackerLeadingParty != null && flag)
			{
				this.AttackerPartyList.Insert(0, this._attackerLeadingParty);
			}
			List<PartyBase> list = new List<PartyBase>();
			List<PartyBase> list2 = new List<PartyBase>();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			List<PartyBase> list5 = new List<PartyBase>();
			if (MobileParty.MainParty.MapEvent != null)
			{
				list5.AddRange(MobileParty.MainParty.MapEvent.InvolvedParties);
				this.IsSiege = false;
			}
			else
			{
				Settlement settlement2 = Settlement.CurrentSettlement ?? PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
				if (settlement2.SiegeEvent == null)
				{
					this.PowerComparer = new PowerLevelComparer(1.0, 1.0);
					return;
				}
				SiegeEvent siegeEvent = settlement2.SiegeEvent;
				list5.AddRange(siegeEvent.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege));
				this.IsSiege = true;
			}
			foreach (PartyBase partyBase in list5)
			{
				bool flag3;
				if (MobileParty.MainParty.MapEvent != null)
				{
					flag3 = partyBase.Side == BattleSideEnum.Defender;
				}
				else
				{
					flag3 = (Settlement.CurrentSettlement ?? PlayerSiege.PlayerSiegeEvent.BesiegedSettlement).SiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender).HasInvolvedPartyForEventType(partyBase, MapEvent.BattleTypes.Siege);
				}
				List<PartyBase> list6 = (flag3 ? list2 : list);
				List<int> list7 = (flag3 ? list4 : list3);
				if (partyBase.IsActive && partyBase.MemberRoster.Count > 0)
				{
					int numberOfHealthyMembers = partyBase.NumberOfHealthyMembers;
					int num = 0;
					while (num < list7.Count && numberOfHealthyMembers <= list7[num])
					{
						num++;
					}
					list7.Add(partyBase.NumberOfHealthyMembers);
					list6.Insert(num, partyBase);
				}
			}
			if (this.PowerComparer == null)
			{
				this.PowerComparer = new PowerLevelComparer((double)list2.Sum((PartyBase party) => party.TotalStrength), (double)list.Sum((PartyBase party) => party.TotalStrength));
			}
			else
			{
				float num2 = list2.Sum((PartyBase party) => party.TotalStrength);
				float num3 = list.Sum((PartyBase party) => party.TotalStrength);
				this.PowerComparer.Update((double)num2, (double)num3, (double)num2, (double)num3);
			}
			List<PartyBase> list8 = list.OrderByDescending((PartyBase p) => p.NumberOfAllMembers).ToList<PartyBase>();
			List<PartyBase> list9 = this.AttackerPartyList.Select((GameMenuPartyItemVM enemy) => enemy.Party).ToList<PartyBase>();
			List<PartyBase> list10 = list8.Except(list9).ToList<PartyBase>();
			list10.Remove(this._attackerLeadingParty.Party);
			foreach (PartyBase partyBase2 in list9.Except(list8).ToList<PartyBase>())
			{
				for (int i = this.AttackerPartyList.Count - 1; i >= 0; i--)
				{
					if (this.AttackerPartyList[i].Party == partyBase2)
					{
						this.AttackerPartyList.RemoveAt(i);
					}
				}
			}
			if (this.IsSiege)
			{
				list10 = list10.Where((PartyBase x) => x.MemberRoster.TotalHealthyCount > 0).ToList<PartyBase>();
			}
			foreach (PartyBase partyBase3 in list10)
			{
				this.AttackerPartyList.Add(new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), partyBase3, false));
			}
			List<PartyBase> list11 = list2.OrderByDescending((PartyBase p) => p.NumberOfAllMembers).ToList<PartyBase>();
			List<PartyBase> list12 = this.DefenderPartyList.Select((GameMenuPartyItemVM ally) => ally.Party).ToList<PartyBase>();
			List<PartyBase> list13 = list11.Except(list12).ToList<PartyBase>();
			list13.Remove(this._defenderLeadingParty.Party);
			foreach (PartyBase partyBase4 in list12.Except(list11).ToList<PartyBase>())
			{
				for (int j = this.DefenderPartyList.Count - 1; j >= 0; j--)
				{
					if (this.DefenderPartyList[j].Party == partyBase4)
					{
						this.DefenderPartyList.RemoveAt(j);
					}
				}
			}
			if (this.IsSiege)
			{
				list13 = list13.Where((PartyBase x) => x.MemberRoster.TotalHealthyCount > 0).ToList<PartyBase>();
			}
			foreach (PartyBase partyBase5 in list13)
			{
				this.DefenderPartyList.Add(new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), partyBase5, false));
			}
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM in this.DefenderPartyList)
			{
				gameMenuPartyItemVM.RefreshProperties();
			}
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM2 in this.AttackerPartyList)
			{
				gameMenuPartyItemVM2.RefreshProperties();
			}
			this.DefenderPartyCount = 0;
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM3 in this.DefenderPartyList)
			{
				if (gameMenuPartyItemVM3.Party != null)
				{
					this.DefenderPartyCount += gameMenuPartyItemVM3.Party.NumberOfHealthyMembers;
				}
			}
			this.AttackerPartyCount = 0;
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM4 in this.AttackerPartyList)
			{
				if (gameMenuPartyItemVM4.Party != null)
				{
					this.AttackerPartyCount += gameMenuPartyItemVM4.Party.NumberOfHealthyMembers;
				}
			}
			this.DefenderPartyCountLbl = this.DefenderPartyCount.ToString();
			this.AttackerPartyCountLbl = this.AttackerPartyCount.ToString();
			if (MobileParty.MainParty.MapEvent != null)
			{
				PartyBase leaderParty = MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker);
				PartyBase leaderParty2 = MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender);
				if (this._attackerLeadingParty.Party == leaderParty2 || this._defenderLeadingParty.Party == leaderParty)
				{
					GameMenuPartyItemVM attackerLeadingParty = this._attackerLeadingParty;
					this._attackerLeadingParty = this._defenderLeadingParty;
					this._defenderLeadingParty = attackerLeadingParty;
				}
			}
			this.TitleText = (this.IsSiege ? GameTexts.FindText("str_siege", null).ToString() : (this.TitleText = GameTexts.FindText("str_battle", null).ToString()));
			IFaction faction = ((this._defenderLeadingParty.Party == null) ? this._defenderLeadingParty.Settlement.MapFaction : this._defenderLeadingParty.Party.MapFaction);
			IFaction faction2 = ((this._attackerLeadingParty.Party == null) ? this._attackerLeadingParty.Settlement.MapFaction : this._attackerLeadingParty.Party.MapFaction);
			Banner banner = ((this._defenderLeadingParty.Party == null) ? this._defenderLeadingParty.Settlement.OwnerClan.Banner : this._defenderLeadingParty.Party.Banner);
			Banner banner2 = ((this._attackerLeadingParty.Party == null) ? this._attackerLeadingParty.Settlement.OwnerClan.Banner : this._attackerLeadingParty.Party.Banner);
			this.DefenderPartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom(banner), true);
			this.AttackerPartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom(banner2), true);
			string text;
			if (faction != null && faction is Kingdom)
			{
				text = Color.FromUint(((Kingdom)faction).PrimaryBannerColor).ToString();
			}
			else
			{
				uint? num4;
				if (faction == null)
				{
					num4 = null;
				}
				else
				{
					Banner banner3 = faction.Banner;
					num4 = ((banner3 != null) ? new uint?(banner3.GetPrimaryColor()) : null);
				}
				text = Color.FromUint(num4 ?? Color.White.ToUnsignedInteger()).ToString();
			}
			string text2;
			if (faction2 != null && faction2 is Kingdom)
			{
				text2 = Color.FromUint(((Kingdom)faction2).PrimaryBannerColor).ToString();
			}
			else
			{
				uint? num5;
				if (faction2 == null)
				{
					num5 = null;
				}
				else
				{
					Banner banner4 = faction2.Banner;
					num5 = ((banner4 != null) ? new uint?(banner4.GetPrimaryColor()) : null);
				}
				text2 = Color.FromUint(num5 ?? Color.White.ToUnsignedInteger()).ToString();
			}
			this.PowerComparer.SetColors(text, text2);
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x0003EA18 File Offset: 0x0003CC18
		// (set) Token: 0x06000FD1 RID: 4049 RVA: 0x0003EA20 File Offset: 0x0003CC20
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

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0003EA43 File Offset: 0x0003CC43
		// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x0003EA4B File Offset: 0x0003CC4B
		[DataSourceProperty]
		public ImageIdentifierVM DefenderPartyBanner
		{
			get
			{
				return this._defenderPartyBanner;
			}
			set
			{
				if (value != this._defenderPartyBanner)
				{
					this._defenderPartyBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "DefenderPartyBanner");
				}
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0003EA69 File Offset: 0x0003CC69
		// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x0003EA71 File Offset: 0x0003CC71
		[DataSourceProperty]
		public ImageIdentifierVM AttackerPartyBanner
		{
			get
			{
				return this._attackerPartyBanner;
			}
			set
			{
				if (value != this._attackerPartyBanner)
				{
					this._attackerPartyBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "AttackerPartyBanner");
				}
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x0003EA8F File Offset: 0x0003CC8F
		// (set) Token: 0x06000FD7 RID: 4055 RVA: 0x0003EA97 File Offset: 0x0003CC97
		[DataSourceProperty]
		public PowerLevelComparer PowerComparer
		{
			get
			{
				return this._powerComparer;
			}
			set
			{
				if (value != this._powerComparer)
				{
					this._powerComparer = value;
					base.OnPropertyChangedWithValue<PowerLevelComparer>(value, "PowerComparer");
				}
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0003EAB5 File Offset: 0x0003CCB5
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x0003EABD File Offset: 0x0003CCBD
		[DataSourceProperty]
		public MBBindingList<GameMenuPartyItemVM> AttackerPartyList
		{
			get
			{
				return this._attackerPartyList;
			}
			set
			{
				if (value != this._attackerPartyList)
				{
					this._attackerPartyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPartyItemVM>>(value, "AttackerPartyList");
				}
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0003EADB File Offset: 0x0003CCDB
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0003EAE3 File Offset: 0x0003CCE3
		[DataSourceProperty]
		public MBBindingList<GameMenuPartyItemVM> DefenderPartyList
		{
			get
			{
				return this._defenderPartyList;
			}
			set
			{
				if (value != this._defenderPartyList)
				{
					this._defenderPartyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPartyItemVM>>(value, "DefenderPartyList");
				}
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0003EB01 File Offset: 0x0003CD01
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x0003EB09 File Offset: 0x0003CD09
		[DataSourceProperty]
		public string DefenderPartyMorale
		{
			get
			{
				return this._defenderPartyMorale;
			}
			set
			{
				if (value != this._defenderPartyMorale)
				{
					this._defenderPartyMorale = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderPartyMorale");
				}
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0003EB2C File Offset: 0x0003CD2C
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x0003EB34 File Offset: 0x0003CD34
		[DataSourceProperty]
		public string AttackerPartyMorale
		{
			get
			{
				return this._attackerPartyMorale;
			}
			set
			{
				if (value != this._attackerPartyMorale)
				{
					this._attackerPartyMorale = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerPartyMorale");
				}
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x0003EB57 File Offset: 0x0003CD57
		// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x0003EB5F File Offset: 0x0003CD5F
		[DataSourceProperty]
		public int DefenderPartyCount
		{
			get
			{
				return this._defenderPartyCount;
			}
			set
			{
				if (value != this._defenderPartyCount)
				{
					this._defenderPartyCount = value;
					base.OnPropertyChangedWithValue(value, "DefenderPartyCount");
				}
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x0003EB7D File Offset: 0x0003CD7D
		// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x0003EB85 File Offset: 0x0003CD85
		[DataSourceProperty]
		public int AttackerPartyCount
		{
			get
			{
				return this._attackerPartyCount;
			}
			set
			{
				if (value != this._attackerPartyCount)
				{
					this._attackerPartyCount = value;
					base.OnPropertyChangedWithValue(value, "AttackerPartyCount");
				}
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0003EBA3 File Offset: 0x0003CDA3
		// (set) Token: 0x06000FE5 RID: 4069 RVA: 0x0003EBAB File Offset: 0x0003CDAB
		[DataSourceProperty]
		public string DefenderPartyFood
		{
			get
			{
				return this._defenderPartyFood;
			}
			set
			{
				if (value != this._defenderPartyFood)
				{
					this._defenderPartyFood = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderPartyFood");
				}
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x0003EBCE File Offset: 0x0003CDCE
		// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x0003EBD6 File Offset: 0x0003CDD6
		[DataSourceProperty]
		public string AttackerPartyFood
		{
			get
			{
				return this._attackerPartyFood;
			}
			set
			{
				if (value != this._attackerPartyFood)
				{
					this._attackerPartyFood = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerPartyFood");
				}
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0003EBF9 File Offset: 0x0003CDF9
		// (set) Token: 0x06000FE9 RID: 4073 RVA: 0x0003EC01 File Offset: 0x0003CE01
		public string DefenderWallHitPoints
		{
			get
			{
				return this._defenderWallHitPoints;
			}
			set
			{
				if (value != this._defenderWallHitPoints)
				{
					this._defenderWallHitPoints = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderWallHitPoints");
				}
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0003EC24 File Offset: 0x0003CE24
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0003EC2C File Offset: 0x0003CE2C
		[DataSourceProperty]
		public bool IsSiege
		{
			get
			{
				return this._isSiege;
			}
			set
			{
				if (value != this._isSiege)
				{
					this._isSiege = value;
					base.OnPropertyChangedWithValue(value, "IsSiege");
				}
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0003EC4A File Offset: 0x0003CE4A
		// (set) Token: 0x06000FED RID: 4077 RVA: 0x0003EC52 File Offset: 0x0003CE52
		[DataSourceProperty]
		public string DefenderPartyCountLbl
		{
			get
			{
				return this._defenderPartyCountLbl;
			}
			set
			{
				if (value != this._defenderPartyCountLbl)
				{
					this._defenderPartyCountLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderPartyCountLbl");
				}
			}
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0003EC75 File Offset: 0x0003CE75
		// (set) Token: 0x06000FEF RID: 4079 RVA: 0x0003EC7D File Offset: 0x0003CE7D
		[DataSourceProperty]
		public string AttackerPartyCountLbl
		{
			get
			{
				return this._attackerPartyCountLbl;
			}
			set
			{
				if (value != this._attackerPartyCountLbl)
				{
					this._attackerPartyCountLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerPartyCountLbl");
				}
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0003ECA0 File Offset: 0x0003CEA0
		// (set) Token: 0x06000FF1 RID: 4081 RVA: 0x0003ECA8 File Offset: 0x0003CEA8
		[DataSourceProperty]
		public HintViewModel AttackerBannerHint
		{
			get
			{
				return this._attackerBannerHint;
			}
			set
			{
				if (value != this._attackerBannerHint)
				{
					this._attackerBannerHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AttackerBannerHint");
				}
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0003ECC6 File Offset: 0x0003CEC6
		// (set) Token: 0x06000FF3 RID: 4083 RVA: 0x0003ECCE File Offset: 0x0003CECE
		[DataSourceProperty]
		public HintViewModel DefenderBannerHint
		{
			get
			{
				return this._defenderBannerHint;
			}
			set
			{
				if (value != this._defenderBannerHint)
				{
					this._defenderBannerHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DefenderBannerHint");
				}
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0003ECEC File Offset: 0x0003CEEC
		// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x0003ECF4 File Offset: 0x0003CEF4
		[DataSourceProperty]
		public HintViewModel AttackerTroopNumHint
		{
			get
			{
				return this._attackerTroopNumHint;
			}
			set
			{
				if (value != this._attackerTroopNumHint)
				{
					this._attackerTroopNumHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AttackerTroopNumHint");
				}
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x0003ED12 File Offset: 0x0003CF12
		// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x0003ED1A File Offset: 0x0003CF1A
		[DataSourceProperty]
		public HintViewModel DefenderTroopNumHint
		{
			get
			{
				return this._defenderTroopNumHint;
			}
			set
			{
				if (value != this._defenderTroopNumHint)
				{
					this._defenderTroopNumHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DefenderTroopNumHint");
				}
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x0003ED38 File Offset: 0x0003CF38
		// (set) Token: 0x06000FF9 RID: 4089 RVA: 0x0003ED40 File Offset: 0x0003CF40
		[DataSourceProperty]
		public BasicTooltipViewModel DefenderWallHint
		{
			get
			{
				return this._defenderWallHint;
			}
			set
			{
				if (value != this._defenderWallHint)
				{
					this._defenderWallHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DefenderWallHint");
				}
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x0003ED5E File Offset: 0x0003CF5E
		// (set) Token: 0x06000FFB RID: 4091 RVA: 0x0003ED66 File Offset: 0x0003CF66
		[DataSourceProperty]
		public BasicTooltipViewModel DefenderFoodHint
		{
			get
			{
				return this._defenderFoodHint;
			}
			set
			{
				if (value != this._defenderFoodHint)
				{
					this._defenderFoodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DefenderFoodHint");
				}
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x0003ED84 File Offset: 0x0003CF84
		// (set) Token: 0x06000FFD RID: 4093 RVA: 0x0003ED8C File Offset: 0x0003CF8C
		[DataSourceProperty]
		public BasicTooltipViewModel AttackerFoodHint
		{
			get
			{
				return this._attackerFoodHint;
			}
			set
			{
				if (value != this._attackerFoodHint)
				{
					this._attackerFoodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "AttackerFoodHint");
				}
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0003EDAA File Offset: 0x0003CFAA
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x0003EDB2 File Offset: 0x0003CFB2
		[DataSourceProperty]
		public BasicTooltipViewModel AttackerMoraleHint
		{
			get
			{
				return this._attackerMoraleHint;
			}
			set
			{
				if (value != this._attackerMoraleHint)
				{
					this._attackerMoraleHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "AttackerMoraleHint");
				}
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0003EDD0 File Offset: 0x0003CFD0
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x0003EDD8 File Offset: 0x0003CFD8
		[DataSourceProperty]
		public BasicTooltipViewModel DefenderMoraleHint
		{
			get
			{
				return this._defenderMoraleHint;
			}
			set
			{
				if (value != this._defenderMoraleHint)
				{
					this._defenderMoraleHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DefenderMoraleHint");
				}
			}
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x0003EDF8 File Offset: 0x0003CFF8
		private List<TooltipProperty> GetEncounterSideFoodTooltip(BattleSideEnum side)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			GameMenuPartyItemVM defenderLeadingParty = this._defenderLeadingParty;
			bool flag = ((defenderLeadingParty != null) ? defenderLeadingParty.Settlement : null) != null;
			bool flag2 = (flag && flag && side == BattleSideEnum.Defender) || (!flag && side == BattleSideEnum.Attacker);
			if (this.IsSiege && flag2)
			{
				list.Add(new TooltipProperty(new TextObject("{=OSsSBHKe}Settlement's Food", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.Title));
				GameMenuPartyItemVM gameMenuPartyItemVM = (flag ? this._defenderLeadingParty : this._attackerLeadingParty);
				Town town;
				if (gameMenuPartyItemVM == null)
				{
					town = null;
				}
				else
				{
					Settlement settlement = gameMenuPartyItemVM.Settlement;
					town = ((settlement != null) ? settlement.Town : null);
				}
				Town town2 = town;
				float num = ((town2 != null) ? town2.FoodChangeWithoutMarketStocks : 0f);
				ValueTuple<int, int> townFoodAndMarketStocks = TownHelpers.GetTownFoodAndMarketStocks(town2);
				list.Add(new TooltipProperty(new TextObject("{=EkFDvG7z}Settlement Food Stocks", null).ToString(), townFoodAndMarketStocks.Item1.ToString("F0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				if (townFoodAndMarketStocks.Item2 != 0)
				{
					list.Add(new TooltipProperty(new TextObject("{=HTtWslIx}Market Food Stocks", null).ToString(), townFoodAndMarketStocks.Item2.ToString("F0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
				list.Add(new TooltipProperty(new TextObject("{=laznt9ZK}Settlement Food Change", null).ToString(), num.ToString("F2"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
				list.Add(new TooltipProperty(new TextObject("{=DNXD37JL}Settlement's Days Until Food Runs Out", null).ToString(), CampaignUIHelper.GetDaysUntilNoFood((float)(townFoodAndMarketStocks.Item1 + townFoodAndMarketStocks.Item2), num), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				if (((town2 != null) ? town2.Settlement : null) != null && SettlementHelper.IsGarrisonStarving(town2.Settlement))
				{
					list.Add(new TooltipProperty(new TextObject("{=0rmpC7jf}The Garrison is Starving", null).ToString(), string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			else
			{
				list.Add(new TooltipProperty(new TextObject("{=Q8dhryRX}Parties' Food", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.Title));
				MBBindingList<GameMenuPartyItemVM> mbbindingList = ((side == BattleSideEnum.Attacker) ? this.AttackerPartyList : this.DefenderPartyList);
				double num2 = 0.0;
				foreach (GameMenuPartyItemVM gameMenuPartyItemVM2 in mbbindingList)
				{
					int num3 = MathF.Ceiling(gameMenuPartyItemVM2.Party.MobileParty.Food / -gameMenuPartyItemVM2.Party.MobileParty.FoodChange);
					num2 += (double)Math.Max(num3, 0);
					string daysUntilNoFood = CampaignUIHelper.GetDaysUntilNoFood(gameMenuPartyItemVM2.Party.MobileParty.Food, gameMenuPartyItemVM2.Party.MobileParty.FoodChange);
					list.Add(new TooltipProperty(gameMenuPartyItemVM2.Party.MobileParty.Name.ToString(), daysUntilNoFood, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
				list.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
				list.Add(new TooltipProperty(new TextObject("{=rwKBR4NE}Average Days Until Food Runs Out", null).ToString(), CampaignUIHelper.GetDaysUntilNoFood((float)num2, (float)mbbindingList.Count), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
			return list;
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0003F134 File Offset: 0x0003D334
		private List<TooltipProperty> GetEncounterSideMoraleTooltip(BattleSideEnum side)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty(new TextObject("{=QBB0KQ2Z}Parties' Average Morale", null).ToString(), "", 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			MBBindingList<GameMenuPartyItemVM> mbbindingList = ((side == BattleSideEnum.Attacker) ? this.AttackerPartyList : this.DefenderPartyList);
			double num = 0.0;
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM in mbbindingList)
			{
				list.Add(new TooltipProperty(gameMenuPartyItemVM.Party.MobileParty.Name.ToString(), gameMenuPartyItemVM.Party.MobileParty.Morale.ToString("0.0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				num += (double)gameMenuPartyItemVM.Party.MobileParty.Morale;
			}
			list.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
			list.Add(new TooltipProperty(new TextObject("{=eoVW9z54}Average Morale", null).ToString(), (num / (double)mbbindingList.Count).ToString("0.0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			return list;
		}

		// Token: 0x04000754 RID: 1876
		private GameMenuPartyItemVM _defenderLeadingParty;

		// Token: 0x04000755 RID: 1877
		private GameMenuPartyItemVM _attackerLeadingParty;

		// Token: 0x04000756 RID: 1878
		private string _titleText;

		// Token: 0x04000757 RID: 1879
		private ImageIdentifierVM _defenderPartyBanner;

		// Token: 0x04000758 RID: 1880
		private ImageIdentifierVM _attackerPartyBanner;

		// Token: 0x04000759 RID: 1881
		private MBBindingList<GameMenuPartyItemVM> _attackerPartyList;

		// Token: 0x0400075A RID: 1882
		private MBBindingList<GameMenuPartyItemVM> _defenderPartyList;

		// Token: 0x0400075B RID: 1883
		private string _attackerPartyMorale;

		// Token: 0x0400075C RID: 1884
		private string _defenderPartyMorale;

		// Token: 0x0400075D RID: 1885
		private int _attackerPartyCount;

		// Token: 0x0400075E RID: 1886
		private int _defenderPartyCount;

		// Token: 0x0400075F RID: 1887
		private string _attackerPartyFood;

		// Token: 0x04000760 RID: 1888
		private string _defenderPartyFood;

		// Token: 0x04000761 RID: 1889
		private string _defenderWallHitPoints;

		// Token: 0x04000762 RID: 1890
		private string _defenderPartyCountLbl;

		// Token: 0x04000763 RID: 1891
		private string _attackerPartyCountLbl;

		// Token: 0x04000764 RID: 1892
		private bool _isSiege;

		// Token: 0x04000765 RID: 1893
		private PowerLevelComparer _powerComparer;

		// Token: 0x04000766 RID: 1894
		private HintViewModel _attackerBannerHint;

		// Token: 0x04000767 RID: 1895
		private HintViewModel _defenderBannerHint;

		// Token: 0x04000768 RID: 1896
		private HintViewModel _attackerTroopNumHint;

		// Token: 0x04000769 RID: 1897
		private HintViewModel _defenderTroopNumHint;

		// Token: 0x0400076A RID: 1898
		private BasicTooltipViewModel _defenderWallHint;

		// Token: 0x0400076B RID: 1899
		private BasicTooltipViewModel _defenderFoodHint;

		// Token: 0x0400076C RID: 1900
		private BasicTooltipViewModel _attackerFoodHint;

		// Token: 0x0400076D RID: 1901
		private BasicTooltipViewModel _attackerMoraleHint;

		// Token: 0x0400076E RID: 1902
		private BasicTooltipViewModel _defenderMoraleHint;
	}
}

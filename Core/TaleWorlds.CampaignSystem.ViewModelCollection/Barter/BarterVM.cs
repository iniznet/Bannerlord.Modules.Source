using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Barter
{
	// Token: 0x02000130 RID: 304
	public class BarterVM : ViewModel
	{
		// Token: 0x06001CEB RID: 7403 RVA: 0x0006754C File Offset: 0x0006574C
		public BarterVM(BarterData args)
		{
			this._barterData = args;
			if (this._barterData.OtherHero == Hero.MainHero)
			{
				this._otherParty = this._barterData.OffererParty;
				this._otherCharacter = this._barterData.OffererHero.CharacterObject ?? CampaignUIHelper.GetVisualPartyLeader(this._otherParty);
			}
			else if (this._barterData.OtherHero != null)
			{
				this._otherCharacter = this._barterData.OtherHero.CharacterObject;
				this.LeftMaxGold = this._otherCharacter.HeroObject.Gold;
			}
			else
			{
				this._otherParty = this._barterData.OtherParty;
				this._otherCharacter = CampaignUIHelper.GetVisualPartyLeader(this._otherParty);
				this.LeftMaxGold = this._otherParty.MobileParty.PartyTradeGold;
			}
			this._barter = Campaign.Current.BarterManager;
			this._isPlayerOfferer = this._barterData.OffererHero == Hero.MainHero;
			this.AutoBalanceHint = new HintViewModel();
			this.LeftFiefList = new MBBindingList<BarterItemVM>();
			this.RightFiefList = new MBBindingList<BarterItemVM>();
			this.LeftPrisonerList = new MBBindingList<BarterItemVM>();
			this.RightPrisonerList = new MBBindingList<BarterItemVM>();
			this.LeftItemList = new MBBindingList<BarterItemVM>();
			this.RightItemList = new MBBindingList<BarterItemVM>();
			this.LeftOtherList = new MBBindingList<BarterItemVM>();
			this.RightOtherList = new MBBindingList<BarterItemVM>();
			this.LeftDiplomaticList = new MBBindingList<BarterItemVM>();
			this.RightDiplomaticList = new MBBindingList<BarterItemVM>();
			this.LeftGoldList = new MBBindingList<BarterItemVM>();
			this.RightGoldList = new MBBindingList<BarterItemVM>();
			this._leftList = new Dictionary<BarterGroup, MBBindingList<BarterItemVM>>();
			this._rightList = new Dictionary<BarterGroup, MBBindingList<BarterItemVM>>();
			this._barterList = new List<Dictionary<BarterGroup, MBBindingList<BarterItemVM>>>();
			this._offerList = new List<MBBindingList<BarterItemVM>>();
			this.LeftOfferList = new MBBindingList<BarterItemVM>();
			this.RightOfferList = new MBBindingList<BarterItemVM>();
			this.InitBarterList(this._barterData);
			this.OnInitialized();
			this.RightMaxGold = Hero.MainHero.Gold;
			this.LeftHero = new HeroVM(this._otherCharacter.HeroObject, false);
			this.RightHero = new HeroVM(Hero.MainHero, false);
			this.SendOffer();
			this.InitializationIsOver = true;
			this.RefreshValues();
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x00067788 File Offset: 0x00065988
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.InitializeStaticContent();
			this.LeftNameLbl = this._otherCharacter.Name.ToString();
			this.RightNameLbl = Hero.MainHero.Name.ToString();
			this.LeftFiefList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightFiefList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.LeftPrisonerList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightPrisonerList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.LeftItemList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightItemList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.LeftOtherList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightOtherList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.LeftDiplomaticList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightDiplomaticList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.LeftGoldList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
			this.RightGoldList.ApplyActionOnAllItems(delegate(BarterItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x000679C4 File Offset: 0x00065BC4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x000679F0 File Offset: 0x00065BF0
		private void InitBarterList(BarterData args)
		{
			this._leftList.Add(args.GetBarterGroup<FiefBarterGroup>(), this.LeftFiefList);
			this._leftList.Add(args.GetBarterGroup<PrisonerBarterGroup>(), this.LeftPrisonerList);
			this._leftList.Add(args.GetBarterGroup<ItemBarterGroup>(), this.LeftItemList);
			this._leftList.Add(args.GetBarterGroup<OtherBarterGroup>(), this.LeftOtherList);
			this._leftList.Add(args.GetBarterGroup<GoldBarterGroup>(), this.LeftGoldList);
			this._rightList.Add(args.GetBarterGroup<FiefBarterGroup>(), this.RightFiefList);
			this._rightList.Add(args.GetBarterGroup<PrisonerBarterGroup>(), this.RightPrisonerList);
			this._rightList.Add(args.GetBarterGroup<ItemBarterGroup>(), this.RightItemList);
			this._rightList.Add(args.GetBarterGroup<OtherBarterGroup>(), this.RightOtherList);
			this._rightList.Add(args.GetBarterGroup<GoldBarterGroup>(), this.RightGoldList);
			this._barterList.Add(this._leftList);
			this._barterList.Add(this._rightList);
			this._offerList.Add(this.LeftOfferList);
			this._offerList.Add(this.RightOfferList);
			if (this._barterData.ContextInitializer != null)
			{
				foreach (Barterable barterable in this._barterData.GetBarterables())
				{
					if (barterable.IsContextDependent && this._barterData.ContextInitializer(barterable, this._barterData, null))
					{
						this.ChangeBarterableIsOffered(barterable, true);
					}
				}
			}
			foreach (Barterable barterable2 in args.GetBarterables())
			{
				if (!barterable2.IsOffered && !barterable2.IsContextDependent)
				{
					this._barterList[(barterable2.OriginalOwner == Hero.MainHero) ? 1 : 0][barterable2.Group].Add(new BarterItemVM(barterable2, new BarterItemVM.BarterTransferEventDelegate(this.TransferItem), new Action(this.OnOfferedAmountChange), false));
				}
				else
				{
					BarterItemVM barterItemVM = new BarterItemVM(barterable2, new BarterItemVM.BarterTransferEventDelegate(this.TransferItem), new Action(this.OnOfferedAmountChange), barterable2.IsContextDependent);
					this._offerList[(barterable2.OriginalOwner == Hero.MainHero) ? 1 : 0].Add(barterItemVM);
					this.RefreshCompatibility(barterItemVM, true);
				}
			}
			this._barterData.GetBarterables().Find((Barterable t) => t.Group.GetType() == typeof(GoldBarterGroup) && t.OriginalOwner == Hero.MainHero);
			this._barterData.GetBarterables().Find((Barterable t) => (t.Group.GetType() == typeof(GoldBarterGroup) && this._barterData.OffererHero == Hero.MainHero && t.OriginalOwner == this._barterData.OtherHero) || (this._barterData.OtherHero == Hero.MainHero && t.OriginalOwner == this._barterData.OffererHero));
			this.RefreshOfferLabel();
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x00067CE4 File Offset: 0x00065EE4
		private void ChangeBarterableIsOffered(Barterable barterable, bool newState)
		{
			if (barterable.IsOffered != newState)
			{
				barterable.SetIsOffered(newState);
				this.OnTransferItem(barterable, true);
				foreach (Barterable barterable2 in barterable.LinkedBarterables)
				{
					this.OnTransferItem(barterable2, true);
				}
			}
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x00067D50 File Offset: 0x00065F50
		public void OnInitialized()
		{
			BarterManager barterManager = Campaign.Current.BarterManager;
			barterManager.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barterManager.Closed, new BarterManager.BarterCloseEventDelegate(this.OnClosed));
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x00067D7D File Offset: 0x00065F7D
		private void OnClosed()
		{
			BarterManager barterManager = Campaign.Current.BarterManager;
			barterManager.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barterManager.Closed, new BarterManager.BarterCloseEventDelegate(this.OnClosed));
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x00067DAA File Offset: 0x00065FAA
		public void ExecuteTransferAllLeftFief()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<FiefBarterGroup>());
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x00067DC3 File Offset: 0x00065FC3
		public void ExecuteAutoBalance()
		{
			this.AutoBalanceAdd();
			this.AutoBalanceRemove();
			this.AutoBalanceAdd();
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x00067DD8 File Offset: 0x00065FD8
		private void AutoBalanceRemove()
		{
			if ((int)Campaign.Current.BarterManager.GetOfferValue(this._otherCharacter.HeroObject, this._otherParty, this._barterData.OffererParty, this._barterData.GetOfferedBarterables()) > 0)
			{
				List<ValueTuple<Barterable, int>> list = BarterHelper.GetAutoBalanceBarterablesToRemove(this._barterData, this.OtherFaction, Clan.PlayerClan.MapFaction, Hero.MainHero).ToList<ValueTuple<Barterable, int>>();
				List<ValueTuple<BarterItemVM, int>> list2 = new List<ValueTuple<BarterItemVM, int>>();
				this.GetBarterItems(this.RightGoldList, list, list2);
				this.GetBarterItems(this.RightItemList, list, list2);
				this.GetBarterItems(this.RightPrisonerList, list, list2);
				this.GetBarterItems(this.RightFiefList, list, list2);
				foreach (ValueTuple<BarterItemVM, int> valueTuple in list2)
				{
					BarterItemVM item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					this.OfferItemRemove(item, item2);
				}
			}
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x00067ED8 File Offset: 0x000660D8
		private void AutoBalanceAdd()
		{
			if ((int)Campaign.Current.BarterManager.GetOfferValue(this._otherCharacter.HeroObject, this._otherParty, this._barterData.OffererParty, this._barterData.GetOfferedBarterables()) < 0)
			{
				List<ValueTuple<Barterable, int>> list = BarterHelper.GetAutoBalanceBarterablesAdd(this._barterData, this.OtherFaction, Clan.PlayerClan.MapFaction, Hero.MainHero, 1f).ToList<ValueTuple<Barterable, int>>();
				List<ValueTuple<BarterItemVM, int>> list2 = new List<ValueTuple<BarterItemVM, int>>();
				this.GetBarterItems(this.RightGoldList, list, list2);
				this.GetBarterItems(this.RightItemList, list, list2);
				this.GetBarterItems(this.RightPrisonerList, list, list2);
				this.GetBarterItems(this.RightFiefList, list, list2);
				foreach (ValueTuple<BarterItemVM, int> valueTuple in list2)
				{
					BarterItemVM item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					if (item2 > 0)
					{
						this.OfferItemAdd(item, item2);
					}
				}
			}
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x00067FE0 File Offset: 0x000661E0
		private void GetBarterItems(MBBindingList<BarterItemVM> itemList, [TupleElementNames(new string[] { "barterable", "count" })] List<ValueTuple<Barterable, int>> newBarterables, List<ValueTuple<BarterItemVM, int>> barterItems)
		{
			foreach (BarterItemVM barterItemVM in itemList)
			{
				foreach (ValueTuple<Barterable, int> valueTuple in newBarterables)
				{
					Barterable item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					if (item == barterItemVM.Barterable)
					{
						barterItems.Add(new ValueTuple<BarterItemVM, int>(barterItemVM, item2));
					}
				}
			}
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0006807C File Offset: 0x0006627C
		public void ExecuteTransferAllLeftItem()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<ItemBarterGroup>());
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x00068095 File Offset: 0x00066295
		public void ExecuteTransferAllLeftPrisoner()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<PrisonerBarterGroup>());
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x000680AE File Offset: 0x000662AE
		public void ExecuteTransferAllLeftOther()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<OtherBarterGroup>());
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x000680C7 File Offset: 0x000662C7
		public void ExecuteTransferAllRightFief()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<FiefBarterGroup>());
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x000680DF File Offset: 0x000662DF
		public void ExecuteTransferAllRightItem()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<ItemBarterGroup>());
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x000680F7 File Offset: 0x000662F7
		public void ExecuteTransferAllRightPrisoner()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<PrisonerBarterGroup>());
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x0006810F File Offset: 0x0006630F
		public void ExecuteTransferAllRightOther()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<OtherBarterGroup>());
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x00068128 File Offset: 0x00066328
		private void ExecuteTransferAll(CharacterObject fromCharacter, BarterGroup barterGroup)
		{
			if (barterGroup != null)
			{
				foreach (BarterItemVM barterItemVM in new List<BarterItemVM>(this._barterList[(fromCharacter == CharacterObject.PlayerCharacter) ? 1 : 0][barterGroup].Where((BarterItemVM barterItem) => !barterItem.Barterable.IsOffered)))
				{
					this.TransferItem(barterItemVM, true);
				}
				foreach (BarterItemVM barterItemVM2 in this._barterList[(fromCharacter == CharacterObject.PlayerCharacter) ? 1 : 0][barterGroup])
				{
					barterItemVM2.CurrentOfferedAmount = barterItemVM2.TotalItemCount;
				}
			}
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x00068218 File Offset: 0x00066418
		private void SendOffer()
		{
			this.IsOfferDisabled = !this.IsCurrentOfferAcceptable() || (this.LeftOfferList.Count == 0 && this.RightOfferList.Count == 0);
			this.RefreshResultBar();
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x0006824F File Offset: 0x0006644F
		private bool IsCurrentOfferAcceptable()
		{
			return Campaign.Current.BarterManager.IsOfferAcceptable(this._barterData, this._otherCharacter.HeroObject, this._otherParty);
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x00068278 File Offset: 0x00066478
		private IFaction OtherFaction
		{
			get
			{
				if (!this._otherCharacter.IsHero)
				{
					return this._otherParty.MapFaction;
				}
				return this._otherCharacter.HeroObject.Clan;
			}
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x000682B0 File Offset: 0x000664B0
		private void RefreshResultBar()
		{
			int num = 0;
			int num2 = 0;
			IFaction otherFaction = this.OtherFaction;
			foreach (BarterItemVM barterItemVM in this.LeftOfferList)
			{
				num2 += barterItemVM.Barterable.GetValueForFaction(otherFaction);
			}
			foreach (BarterItemVM barterItemVM2 in this.RightOfferList)
			{
				num += barterItemVM2.Barterable.GetValueForFaction(otherFaction);
			}
			this.ResultBarOtherPercentage = MathF.Round((float)MathF.Max(0, num) / (float)MathF.Max(1, -num2) * 100f);
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x0006837C File Offset: 0x0006657C
		private void ExecuteTransferAllGoldLeft()
		{
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x0006837E File Offset: 0x0006657E
		private void ExecuteTransferAllGoldRight()
		{
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x00068380 File Offset: 0x00066580
		public void ExecuteOffer()
		{
			Campaign.Current.BarterManager.ApplyAndFinalizePlayerBarter(this._barterData.OffererHero, this._barterData.OtherHero, this._barterData);
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x000683AD File Offset: 0x000665AD
		public void ExecuteCancel()
		{
			Campaign.Current.BarterManager.CancelAndFinalizePlayerBarter(this._barterData.OffererHero, this._barterData.OtherHero, this._barterData);
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x000683DC File Offset: 0x000665DC
		public void ExecuteReset()
		{
			this.LeftFiefList.Clear();
			this.RightFiefList.Clear();
			this.LeftPrisonerList.Clear();
			this.RightPrisonerList.Clear();
			this.LeftItemList.Clear();
			this.RightItemList.Clear();
			this.LeftOtherList.Clear();
			this.RightOtherList.Clear();
			this.LeftDiplomaticList.Clear();
			this.RightDiplomaticList.Clear();
			this.LeftGoldList.Clear();
			this.RightGoldList.Clear();
			this._leftList.Clear();
			this._rightList.Clear();
			this._barterList.Clear();
			this.LeftOfferList.Clear();
			this.RightOfferList.Clear();
			this._offerList.Clear();
			foreach (Barterable barterable in this._barterData.GetBarterables())
			{
				if (barterable.IsOffered)
				{
					this.ChangeBarterableIsOffered(barterable, false);
				}
			}
			this.InitBarterList(this._barterData);
			this.LeftNameLbl = this._otherCharacter.Name.ToString();
			this.RightNameLbl = Hero.MainHero.Name.ToString();
			this.LeftMaxGold = ((this._otherCharacter.HeroObject != null) ? this._otherCharacter.HeroObject.Gold : this._otherParty.MobileParty.PartyTradeGold);
			this.RightMaxGold = Hero.MainHero.Gold;
			this.SendOffer();
			this.InitializationIsOver = true;
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x0006858C File Offset: 0x0006678C
		private void TransferItem(BarterItemVM item, bool offerAll)
		{
			this.ChangeBarterableIsOffered(item.Barterable, !item.IsOffered);
			if (offerAll)
			{
				item.CurrentOfferedAmount = item.TotalItemCount;
			}
			this.SendOffer();
			this.RefreshOfferLabel();
			this.RefreshCompatibility(item, item.IsOffered);
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x000685CC File Offset: 0x000667CC
		private void OfferItemAdd(BarterItemVM barterItemVM, int count)
		{
			this.ChangeBarterableIsOffered(barterItemVM.Barterable, true);
			barterItemVM.CurrentOfferedAmount = (int)MathF.Clamp((float)(barterItemVM.CurrentOfferedAmount + count), 0f, (float)barterItemVM.TotalItemCount);
			this.SendOffer();
			this.RefreshOfferLabel();
			this.RefreshCompatibility(barterItemVM, barterItemVM.IsOffered);
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x00068620 File Offset: 0x00066820
		private void OfferItemRemove(BarterItemVM barterItemVM, int count)
		{
			if (barterItemVM.CurrentOfferedAmount <= count)
			{
				this.ChangeBarterableIsOffered(barterItemVM.Barterable, false);
			}
			else
			{
				barterItemVM.CurrentOfferedAmount = (int)MathF.Clamp((float)(barterItemVM.CurrentOfferedAmount - count), 0f, (float)barterItemVM.TotalItemCount);
			}
			this.SendOffer();
			this.RefreshOfferLabel();
			this.RefreshCompatibility(barterItemVM, barterItemVM.IsOffered);
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x00068680 File Offset: 0x00066880
		public void OnTransferItem(Barterable barter, bool isTransferrable)
		{
			int num = ((barter.OriginalOwner == Hero.MainHero) ? 1 : 0);
			if (!this._barterList.IsEmpty<Dictionary<BarterGroup, MBBindingList<BarterItemVM>>>())
			{
				BarterItemVM barterItemVM = this._barterList[num][barter.Group].FirstOrDefault((BarterItemVM i) => i.Barterable == barter);
				if (barterItemVM == null && !this._offerList.IsEmpty<MBBindingList<BarterItemVM>>())
				{
					barterItemVM = this._offerList[num].FirstOrDefault((BarterItemVM i) => i.Barterable == barter);
				}
				if (barterItemVM != null)
				{
					barterItemVM.IsOffered = barter.IsOffered;
					barterItemVM.IsItemTransferrable = isTransferrable;
					if (barterItemVM.IsOffered)
					{
						this._offerList[num].Add(barterItemVM);
						if (barterItemVM.IsMultiple)
						{
							barterItemVM.CurrentOfferedAmount = 1;
							return;
						}
					}
					else
					{
						this._offerList[num].Remove(barterItemVM);
						if (barterItemVM.IsMultiple)
						{
							barterItemVM.CurrentOfferedAmount = 1;
						}
					}
				}
			}
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x00068784 File Offset: 0x00066984
		private void OnOfferedAmountChange()
		{
			this.SendOffer();
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x0006878C File Offset: 0x0006698C
		private void RefreshOfferLabel()
		{
			this.OfferLbl = ((this.LeftOfferList.Count > 0) ? GameTexts.FindText("str_offer", null).ToString() : GameTexts.FindText("str_gift", null).ToString());
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x000687C4 File Offset: 0x000669C4
		private void RefreshCompatibility(BarterItemVM lastTransferredItem, bool gotOffered)
		{
			Action<BarterItemVM> <>9__0;
			foreach (MBBindingList<BarterItemVM> mbbindingList in this._leftList.Values)
			{
				List<BarterItemVM> list = mbbindingList.ToList<BarterItemVM>();
				Action<BarterItemVM> action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate(BarterItemVM b)
					{
						b.RefreshCompabilityWithItem(lastTransferredItem, gotOffered);
					});
				}
				list.ForEach(action);
			}
			Action<BarterItemVM> <>9__1;
			foreach (MBBindingList<BarterItemVM> mbbindingList2 in this._rightList.Values)
			{
				List<BarterItemVM> list2 = mbbindingList2.ToList<BarterItemVM>();
				Action<BarterItemVM> action2;
				if ((action2 = <>9__1) == null)
				{
					action2 = (<>9__1 = delegate(BarterItemVM b)
					{
						b.RefreshCompabilityWithItem(lastTransferredItem, gotOffered);
					});
				}
				list2.ForEach(action2);
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x000688BC File Offset: 0x00066ABC
		// (set) Token: 0x06001D10 RID: 7440 RVA: 0x000688C4 File Offset: 0x00066AC4
		[DataSourceProperty]
		public string FiefLbl
		{
			get
			{
				return this._fiefLbl;
			}
			set
			{
				if (value != this._fiefLbl)
				{
					this._fiefLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "FiefLbl");
				}
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06001D11 RID: 7441 RVA: 0x000688E7 File Offset: 0x00066AE7
		// (set) Token: 0x06001D12 RID: 7442 RVA: 0x000688EF File Offset: 0x00066AEF
		[DataSourceProperty]
		public string PrisonerLbl
		{
			get
			{
				return this._prisonerLbl;
			}
			set
			{
				if (value != this._prisonerLbl)
				{
					this._prisonerLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "PrisonerLbl");
				}
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x06001D13 RID: 7443 RVA: 0x00068912 File Offset: 0x00066B12
		// (set) Token: 0x06001D14 RID: 7444 RVA: 0x0006891A File Offset: 0x00066B1A
		[DataSourceProperty]
		public string ItemLbl
		{
			get
			{
				return this._itemLbl;
			}
			set
			{
				if (value != this._itemLbl)
				{
					this._itemLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemLbl");
				}
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x06001D15 RID: 7445 RVA: 0x0006893D File Offset: 0x00066B3D
		// (set) Token: 0x06001D16 RID: 7446 RVA: 0x00068945 File Offset: 0x00066B45
		[DataSourceProperty]
		public string OtherLbl
		{
			get
			{
				return this._otherLbl;
			}
			set
			{
				if (value != this._otherLbl)
				{
					this._otherLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherLbl");
				}
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06001D17 RID: 7447 RVA: 0x00068968 File Offset: 0x00066B68
		// (set) Token: 0x06001D18 RID: 7448 RVA: 0x00068970 File Offset: 0x00066B70
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06001D19 RID: 7449 RVA: 0x00068993 File Offset: 0x00066B93
		// (set) Token: 0x06001D1A RID: 7450 RVA: 0x0006899B File Offset: 0x00066B9B
		[DataSourceProperty]
		public string ResetLbl
		{
			get
			{
				return this._resetLbl;
			}
			set
			{
				if (value != this._resetLbl)
				{
					this._resetLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetLbl");
				}
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x000689BE File Offset: 0x00066BBE
		// (set) Token: 0x06001D1C RID: 7452 RVA: 0x000689C6 File Offset: 0x00066BC6
		[DataSourceProperty]
		public string OfferLbl
		{
			get
			{
				return this._offerLbl;
			}
			set
			{
				if (value != this._offerLbl)
				{
					this._offerLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OfferLbl");
				}
			}
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06001D1D RID: 7453 RVA: 0x000689E9 File Offset: 0x00066BE9
		// (set) Token: 0x06001D1E RID: 7454 RVA: 0x000689F1 File Offset: 0x00066BF1
		[DataSourceProperty]
		public string DiplomaticLbl
		{
			get
			{
				return this._diplomaticLbl;
			}
			set
			{
				if (value != this._diplomaticLbl)
				{
					this._diplomaticLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DiplomaticLbl");
				}
			}
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06001D1F RID: 7455 RVA: 0x00068A14 File Offset: 0x00066C14
		// (set) Token: 0x06001D20 RID: 7456 RVA: 0x00068A1C File Offset: 0x00066C1C
		[DataSourceProperty]
		public HintViewModel AutoBalanceHint
		{
			get
			{
				return this._autoBalanceHint;
			}
			set
			{
				if (value != this._autoBalanceHint)
				{
					this._autoBalanceHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AutoBalanceHint");
				}
			}
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x00068A3A File Offset: 0x00066C3A
		// (set) Token: 0x06001D22 RID: 7458 RVA: 0x00068A42 File Offset: 0x00066C42
		[DataSourceProperty]
		public HeroVM LeftHero
		{
			get
			{
				return this._leftHero;
			}
			set
			{
				if (value != this._leftHero)
				{
					this._leftHero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "LeftHero");
				}
			}
		}

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x06001D23 RID: 7459 RVA: 0x00068A60 File Offset: 0x00066C60
		// (set) Token: 0x06001D24 RID: 7460 RVA: 0x00068A68 File Offset: 0x00066C68
		[DataSourceProperty]
		public HeroVM RightHero
		{
			get
			{
				return this._rightHero;
			}
			set
			{
				if (value != this._rightHero)
				{
					this._rightHero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "RightHero");
				}
			}
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x00068A86 File Offset: 0x00066C86
		// (set) Token: 0x06001D26 RID: 7462 RVA: 0x00068A8E File Offset: 0x00066C8E
		[DataSourceProperty]
		public bool IsOfferDisabled
		{
			get
			{
				return this._isOfferDisabled;
			}
			set
			{
				if (value != this._isOfferDisabled)
				{
					this._isOfferDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsOfferDisabled");
				}
			}
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06001D27 RID: 7463 RVA: 0x00068AAC File Offset: 0x00066CAC
		// (set) Token: 0x06001D28 RID: 7464 RVA: 0x00068AB4 File Offset: 0x00066CB4
		[DataSourceProperty]
		public int LeftMaxGold
		{
			get
			{
				return this._leftMaxGold;
			}
			set
			{
				if (value != this._leftMaxGold)
				{
					this._leftMaxGold = value;
					base.OnPropertyChangedWithValue(value, "LeftMaxGold");
				}
			}
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06001D29 RID: 7465 RVA: 0x00068AD2 File Offset: 0x00066CD2
		// (set) Token: 0x06001D2A RID: 7466 RVA: 0x00068ADA File Offset: 0x00066CDA
		[DataSourceProperty]
		public int RightMaxGold
		{
			get
			{
				return this._rightMaxGold;
			}
			set
			{
				if (value != this._rightMaxGold)
				{
					this._rightMaxGold = value;
					base.OnPropertyChangedWithValue(value, "RightMaxGold");
				}
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x06001D2B RID: 7467 RVA: 0x00068AF8 File Offset: 0x00066CF8
		// (set) Token: 0x06001D2C RID: 7468 RVA: 0x00068B00 File Offset: 0x00066D00
		[DataSourceProperty]
		public string LeftNameLbl
		{
			get
			{
				return this._leftNameLbl;
			}
			set
			{
				if (value != this._leftNameLbl)
				{
					this._leftNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "LeftNameLbl");
				}
			}
		}

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x06001D2D RID: 7469 RVA: 0x00068B23 File Offset: 0x00066D23
		// (set) Token: 0x06001D2E RID: 7470 RVA: 0x00068B2B File Offset: 0x00066D2B
		[DataSourceProperty]
		public string RightNameLbl
		{
			get
			{
				return this._rightNameLbl;
			}
			set
			{
				if (value != this._rightNameLbl)
				{
					this._rightNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "RightNameLbl");
				}
			}
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x00068B4E File Offset: 0x00066D4E
		// (set) Token: 0x06001D30 RID: 7472 RVA: 0x00068B56 File Offset: 0x00066D56
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftFiefList
		{
			get
			{
				return this._leftFiefList;
			}
			set
			{
				if (value != this._leftFiefList)
				{
					this._leftFiefList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftFiefList");
				}
			}
		}

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06001D31 RID: 7473 RVA: 0x00068B74 File Offset: 0x00066D74
		// (set) Token: 0x06001D32 RID: 7474 RVA: 0x00068B7C File Offset: 0x00066D7C
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightFiefList
		{
			get
			{
				return this._rightFiefList;
			}
			set
			{
				if (value != this._rightFiefList)
				{
					this._rightFiefList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightFiefList");
				}
			}
		}

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06001D33 RID: 7475 RVA: 0x00068B9A File Offset: 0x00066D9A
		// (set) Token: 0x06001D34 RID: 7476 RVA: 0x00068BA2 File Offset: 0x00066DA2
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftPrisonerList
		{
			get
			{
				return this._leftPrisonerList;
			}
			set
			{
				if (value != this._leftPrisonerList)
				{
					this._leftPrisonerList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftPrisonerList");
				}
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x06001D35 RID: 7477 RVA: 0x00068BC0 File Offset: 0x00066DC0
		// (set) Token: 0x06001D36 RID: 7478 RVA: 0x00068BC8 File Offset: 0x00066DC8
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightPrisonerList
		{
			get
			{
				return this._rightPrisonerList;
			}
			set
			{
				if (value != this._rightPrisonerList)
				{
					this._rightPrisonerList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightPrisonerList");
				}
			}
		}

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x06001D37 RID: 7479 RVA: 0x00068BE6 File Offset: 0x00066DE6
		// (set) Token: 0x06001D38 RID: 7480 RVA: 0x00068BEE File Offset: 0x00066DEE
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftItemList
		{
			get
			{
				return this._leftItemList;
			}
			set
			{
				if (value != this._leftItemList)
				{
					this._leftItemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftItemList");
				}
			}
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x06001D39 RID: 7481 RVA: 0x00068C0C File Offset: 0x00066E0C
		// (set) Token: 0x06001D3A RID: 7482 RVA: 0x00068C14 File Offset: 0x00066E14
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightItemList
		{
			get
			{
				return this._rightItemList;
			}
			set
			{
				if (value != this._rightItemList)
				{
					this._rightItemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightItemList");
				}
			}
		}

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x00068C32 File Offset: 0x00066E32
		// (set) Token: 0x06001D3C RID: 7484 RVA: 0x00068C3A File Offset: 0x00066E3A
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftOtherList
		{
			get
			{
				return this._leftOtherList;
			}
			set
			{
				if (value != this._leftOtherList)
				{
					this._leftOtherList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftOtherList");
				}
			}
		}

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06001D3D RID: 7485 RVA: 0x00068C58 File Offset: 0x00066E58
		// (set) Token: 0x06001D3E RID: 7486 RVA: 0x00068C60 File Offset: 0x00066E60
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightOtherList
		{
			get
			{
				return this._rightOtherList;
			}
			set
			{
				if (value != this._rightOtherList)
				{
					this._rightOtherList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightOtherList");
				}
			}
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06001D3F RID: 7487 RVA: 0x00068C7E File Offset: 0x00066E7E
		// (set) Token: 0x06001D40 RID: 7488 RVA: 0x00068C86 File Offset: 0x00066E86
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftDiplomaticList
		{
			get
			{
				return this._leftDiplomaticList;
			}
			set
			{
				if (value != this._leftDiplomaticList)
				{
					this._leftDiplomaticList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftDiplomaticList");
				}
			}
		}

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06001D41 RID: 7489 RVA: 0x00068CA4 File Offset: 0x00066EA4
		// (set) Token: 0x06001D42 RID: 7490 RVA: 0x00068CAC File Offset: 0x00066EAC
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightDiplomaticList
		{
			get
			{
				return this._rightDiplomaticList;
			}
			set
			{
				if (value != this._rightDiplomaticList)
				{
					this._rightDiplomaticList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightDiplomaticList");
				}
			}
		}

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06001D43 RID: 7491 RVA: 0x00068CCA File Offset: 0x00066ECA
		// (set) Token: 0x06001D44 RID: 7492 RVA: 0x00068CD2 File Offset: 0x00066ED2
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftOfferList
		{
			get
			{
				return this._leftOfferList;
			}
			set
			{
				if (value != this._leftOfferList)
				{
					this._leftOfferList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftOfferList");
				}
			}
		}

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06001D45 RID: 7493 RVA: 0x00068CF0 File Offset: 0x00066EF0
		// (set) Token: 0x06001D46 RID: 7494 RVA: 0x00068CF8 File Offset: 0x00066EF8
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightOfferList
		{
			get
			{
				return this._rightOfferList;
			}
			set
			{
				if (value != this._rightOfferList)
				{
					this._rightOfferList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightOfferList");
				}
			}
		}

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x06001D47 RID: 7495 RVA: 0x00068D16 File Offset: 0x00066F16
		// (set) Token: 0x06001D48 RID: 7496 RVA: 0x00068D1E File Offset: 0x00066F1E
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> RightGoldList
		{
			get
			{
				return this._rightGoldList;
			}
			set
			{
				if (value != this._rightGoldList)
				{
					this._rightGoldList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "RightGoldList");
				}
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x06001D49 RID: 7497 RVA: 0x00068D3C File Offset: 0x00066F3C
		// (set) Token: 0x06001D4A RID: 7498 RVA: 0x00068D44 File Offset: 0x00066F44
		[DataSourceProperty]
		public MBBindingList<BarterItemVM> LeftGoldList
		{
			get
			{
				return this._leftGoldList;
			}
			set
			{
				if (value != this._leftGoldList)
				{
					this._leftGoldList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BarterItemVM>>(value, "LeftGoldList");
				}
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x00068D62 File Offset: 0x00066F62
		// (set) Token: 0x06001D4C RID: 7500 RVA: 0x00068D6A File Offset: 0x00066F6A
		[DataSourceProperty]
		public bool InitializationIsOver
		{
			get
			{
				return this._initializationIsOver;
			}
			set
			{
				this._initializationIsOver = value;
				base.OnPropertyChangedWithValue(value, "InitializationIsOver");
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x00068D7F File Offset: 0x00066F7F
		// (set) Token: 0x06001D4E RID: 7502 RVA: 0x00068D87 File Offset: 0x00066F87
		[DataSourceProperty]
		public int ResultBarOtherPercentage
		{
			get
			{
				return this._resultBarOtherPercentage;
			}
			set
			{
				this._resultBarOtherPercentage = value;
				base.OnPropertyChangedWithValue(value, "ResultBarOtherPercentage");
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06001D4F RID: 7503 RVA: 0x00068D9C File Offset: 0x00066F9C
		// (set) Token: 0x06001D50 RID: 7504 RVA: 0x00068DA4 File Offset: 0x00066FA4
		[DataSourceProperty]
		public int ResultBarOffererPercentage
		{
			get
			{
				return this._resultBarOffererPercentage;
			}
			set
			{
				this._resultBarOffererPercentage = value;
				base.OnPropertyChangedWithValue(value, "ResultBarOffererPercentage");
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00068DB9 File Offset: 0x00066FB9
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x00068DC8 File Offset: 0x00066FC8
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x00068DD7 File Offset: 0x00066FD7
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06001D54 RID: 7508 RVA: 0x00068DE6 File Offset: 0x00066FE6
		// (set) Token: 0x06001D55 RID: 7509 RVA: 0x00068DEE File Offset: 0x00066FEE
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06001D56 RID: 7510 RVA: 0x00068E0C File Offset: 0x0006700C
		// (set) Token: 0x06001D57 RID: 7511 RVA: 0x00068E14 File Offset: 0x00067014
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06001D58 RID: 7512 RVA: 0x00068E32 File Offset: 0x00067032
		// (set) Token: 0x06001D59 RID: 7513 RVA: 0x00068E3A File Offset: 0x0006703A
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00068E58 File Offset: 0x00067058
		public void InitializeStaticContent()
		{
			this.FiefLbl = GameTexts.FindText("str_fiefs", null).ToString();
			this.PrisonerLbl = GameTexts.FindText("str_prisoner_tag_name", null).ToString();
			this.ItemLbl = GameTexts.FindText("str_item_tag_name", null).ToString();
			this.OtherLbl = GameTexts.FindText("str_other", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.ResetLbl = GameTexts.FindText("str_reset", null).ToString();
			this.DiplomaticLbl = GameTexts.FindText("str_diplomatic_group", null).ToString();
			this.AutoBalanceHint.HintText = new TextObject("{=Ve5jkJqf}Auto Offer", null);
		}

		// Token: 0x04000DA3 RID: 3491
		private readonly List<Dictionary<BarterGroup, MBBindingList<BarterItemVM>>> _barterList;

		// Token: 0x04000DA4 RID: 3492
		private readonly List<MBBindingList<BarterItemVM>> _offerList;

		// Token: 0x04000DA5 RID: 3493
		private readonly Dictionary<BarterGroup, MBBindingList<BarterItemVM>> _leftList;

		// Token: 0x04000DA6 RID: 3494
		private readonly Dictionary<BarterGroup, MBBindingList<BarterItemVM>> _rightList;

		// Token: 0x04000DA7 RID: 3495
		private readonly bool _isPlayerOfferer;

		// Token: 0x04000DA8 RID: 3496
		private readonly BarterManager _barter;

		// Token: 0x04000DA9 RID: 3497
		private readonly CharacterObject _otherCharacter;

		// Token: 0x04000DAA RID: 3498
		private readonly PartyBase _otherParty;

		// Token: 0x04000DAB RID: 3499
		private readonly BarterData _barterData;

		// Token: 0x04000DAC RID: 3500
		private string _fiefLbl;

		// Token: 0x04000DAD RID: 3501
		private string _prisonerLbl;

		// Token: 0x04000DAE RID: 3502
		private string _itemLbl;

		// Token: 0x04000DAF RID: 3503
		private string _otherLbl;

		// Token: 0x04000DB0 RID: 3504
		private string _cancelLbl;

		// Token: 0x04000DB1 RID: 3505
		private string _resetLbl;

		// Token: 0x04000DB2 RID: 3506
		private string _offerLbl;

		// Token: 0x04000DB3 RID: 3507
		private string _diplomaticLbl;

		// Token: 0x04000DB4 RID: 3508
		private HintViewModel _autoBalanceHint;

		// Token: 0x04000DB5 RID: 3509
		private HeroVM _leftHero;

		// Token: 0x04000DB6 RID: 3510
		private HeroVM _rightHero;

		// Token: 0x04000DB7 RID: 3511
		private string _leftNameLbl;

		// Token: 0x04000DB8 RID: 3512
		private string _rightNameLbl;

		// Token: 0x04000DB9 RID: 3513
		private MBBindingList<BarterItemVM> _leftFiefList;

		// Token: 0x04000DBA RID: 3514
		private MBBindingList<BarterItemVM> _rightFiefList;

		// Token: 0x04000DBB RID: 3515
		private MBBindingList<BarterItemVM> _leftPrisonerList;

		// Token: 0x04000DBC RID: 3516
		private MBBindingList<BarterItemVM> _rightPrisonerList;

		// Token: 0x04000DBD RID: 3517
		private MBBindingList<BarterItemVM> _leftItemList;

		// Token: 0x04000DBE RID: 3518
		private MBBindingList<BarterItemVM> _rightItemList;

		// Token: 0x04000DBF RID: 3519
		private MBBindingList<BarterItemVM> _leftOtherList;

		// Token: 0x04000DC0 RID: 3520
		private MBBindingList<BarterItemVM> _rightOtherList;

		// Token: 0x04000DC1 RID: 3521
		private MBBindingList<BarterItemVM> _leftDiplomaticList;

		// Token: 0x04000DC2 RID: 3522
		private MBBindingList<BarterItemVM> _rightDiplomaticList;

		// Token: 0x04000DC3 RID: 3523
		private MBBindingList<BarterItemVM> _leftGoldList;

		// Token: 0x04000DC4 RID: 3524
		private MBBindingList<BarterItemVM> _rightGoldList;

		// Token: 0x04000DC5 RID: 3525
		private MBBindingList<BarterItemVM> _leftOfferList;

		// Token: 0x04000DC6 RID: 3526
		private MBBindingList<BarterItemVM> _rightOfferList;

		// Token: 0x04000DC7 RID: 3527
		private int _leftMaxGold;

		// Token: 0x04000DC8 RID: 3528
		private int _rightMaxGold;

		// Token: 0x04000DC9 RID: 3529
		private bool _initializationIsOver;

		// Token: 0x04000DCA RID: 3530
		private bool _isOfferDisabled;

		// Token: 0x04000DCB RID: 3531
		private int _resultBarOffererPercentage = -1;

		// Token: 0x04000DCC RID: 3532
		private int _resultBarOtherPercentage = -1;

		// Token: 0x04000DCD RID: 3533
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000DCE RID: 3534
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000DCF RID: 3535
		private InputKeyItemVM _cancelInputKey;
	}
}

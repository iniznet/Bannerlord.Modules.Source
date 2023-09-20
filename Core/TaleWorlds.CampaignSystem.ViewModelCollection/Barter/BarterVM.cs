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
	public class BarterVM : ViewModel
	{
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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
		}

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

		public void OnInitialized()
		{
			BarterManager barterManager = Campaign.Current.BarterManager;
			barterManager.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barterManager.Closed, new BarterManager.BarterCloseEventDelegate(this.OnClosed));
		}

		private void OnClosed()
		{
			BarterManager barterManager = Campaign.Current.BarterManager;
			barterManager.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barterManager.Closed, new BarterManager.BarterCloseEventDelegate(this.OnClosed));
		}

		public void ExecuteTransferAllLeftFief()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<FiefBarterGroup>());
		}

		public void ExecuteAutoBalance()
		{
			this.AutoBalanceAdd();
			this.AutoBalanceRemove();
			this.AutoBalanceAdd();
		}

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

		public void ExecuteTransferAllLeftItem()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<ItemBarterGroup>());
		}

		public void ExecuteTransferAllLeftPrisoner()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<PrisonerBarterGroup>());
		}

		public void ExecuteTransferAllLeftOther()
		{
			this.ExecuteTransferAll(this._otherCharacter, this._barterData.GetBarterGroup<OtherBarterGroup>());
		}

		public void ExecuteTransferAllRightFief()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<FiefBarterGroup>());
		}

		public void ExecuteTransferAllRightItem()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<ItemBarterGroup>());
		}

		public void ExecuteTransferAllRightPrisoner()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<PrisonerBarterGroup>());
		}

		public void ExecuteTransferAllRightOther()
		{
			this.ExecuteTransferAll(CharacterObject.PlayerCharacter, this._barterData.GetBarterGroup<OtherBarterGroup>());
		}

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

		private void SendOffer()
		{
			this.IsOfferDisabled = !this.IsCurrentOfferAcceptable() || (this.LeftOfferList.Count == 0 && this.RightOfferList.Count == 0);
			this.RefreshResultBar();
		}

		private bool IsCurrentOfferAcceptable()
		{
			return Campaign.Current.BarterManager.IsOfferAcceptable(this._barterData, this._otherCharacter.HeroObject, this._otherParty);
		}

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

		private void ExecuteTransferAllGoldLeft()
		{
		}

		private void ExecuteTransferAllGoldRight()
		{
		}

		public void ExecuteOffer()
		{
			Campaign.Current.BarterManager.ApplyAndFinalizePlayerBarter(this._barterData.OffererHero, this._barterData.OtherHero, this._barterData);
		}

		public void ExecuteCancel()
		{
			Campaign.Current.BarterManager.CancelAndFinalizePlayerBarter(this._barterData.OffererHero, this._barterData.OtherHero, this._barterData);
		}

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

		private void OfferItemAdd(BarterItemVM barterItemVM, int count)
		{
			this.ChangeBarterableIsOffered(barterItemVM.Barterable, true);
			barterItemVM.CurrentOfferedAmount = (int)MathF.Clamp((float)(barterItemVM.CurrentOfferedAmount + count), 0f, (float)barterItemVM.TotalItemCount);
			this.SendOffer();
			this.RefreshOfferLabel();
			this.RefreshCompatibility(barterItemVM, barterItemVM.IsOffered);
		}

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

		private void OnOfferedAmountChange()
		{
			this.SendOffer();
		}

		private void RefreshOfferLabel()
		{
			this.OfferLbl = ((this.LeftOfferList.Count > 0) ? GameTexts.FindText("str_offer", null).ToString() : GameTexts.FindText("str_gift", null).ToString());
		}

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

		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

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

		private readonly List<Dictionary<BarterGroup, MBBindingList<BarterItemVM>>> _barterList;

		private readonly List<MBBindingList<BarterItemVM>> _offerList;

		private readonly Dictionary<BarterGroup, MBBindingList<BarterItemVM>> _leftList;

		private readonly Dictionary<BarterGroup, MBBindingList<BarterItemVM>> _rightList;

		private readonly bool _isPlayerOfferer;

		private readonly BarterManager _barter;

		private readonly CharacterObject _otherCharacter;

		private readonly PartyBase _otherParty;

		private readonly BarterData _barterData;

		private string _fiefLbl;

		private string _prisonerLbl;

		private string _itemLbl;

		private string _otherLbl;

		private string _cancelLbl;

		private string _resetLbl;

		private string _offerLbl;

		private string _diplomaticLbl;

		private HintViewModel _autoBalanceHint;

		private HeroVM _leftHero;

		private HeroVM _rightHero;

		private string _leftNameLbl;

		private string _rightNameLbl;

		private MBBindingList<BarterItemVM> _leftFiefList;

		private MBBindingList<BarterItemVM> _rightFiefList;

		private MBBindingList<BarterItemVM> _leftPrisonerList;

		private MBBindingList<BarterItemVM> _rightPrisonerList;

		private MBBindingList<BarterItemVM> _leftItemList;

		private MBBindingList<BarterItemVM> _rightItemList;

		private MBBindingList<BarterItemVM> _leftOtherList;

		private MBBindingList<BarterItemVM> _rightOtherList;

		private MBBindingList<BarterItemVM> _leftDiplomaticList;

		private MBBindingList<BarterItemVM> _rightDiplomaticList;

		private MBBindingList<BarterItemVM> _leftGoldList;

		private MBBindingList<BarterItemVM> _rightGoldList;

		private MBBindingList<BarterItemVM> _leftOfferList;

		private MBBindingList<BarterItemVM> _rightOfferList;

		private int _leftMaxGold;

		private int _rightMaxGold;

		private bool _initializationIsOver;

		private bool _isOfferDisabled;

		private int _resultBarOffererPercentage = -1;

		private int _resultBarOtherPercentage = -1;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;
	}
}

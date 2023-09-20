using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Barter
{
	// Token: 0x0200012F RID: 303
	public class BarterItemVM : EncyclopediaLinkVM
	{
		// Token: 0x06001CCB RID: 7371 RVA: 0x000670E0 File Offset: 0x000652E0
		public BarterItemVM(Barterable barterable, BarterItemVM.BarterTransferEventDelegate OnTransfer, Action onAmountChange, bool isFixed = false)
		{
			this.Barterable = barterable;
			base.ActiveLink = barterable.GetEncyclopediaLink();
			this._onTransfer = OnTransfer;
			this._onAmountChange = onAmountChange;
			this._isFixed = isFixed;
			this.IsItemTransferrable = !isFixed;
			this.BarterableType = this.Barterable.StringID;
			ImageIdentifier visualIdentifier = this.Barterable.GetVisualIdentifier();
			this.HasVisualIdentifier = visualIdentifier != null;
			if (visualIdentifier != null)
			{
				this.VisualIdentifier = new ImageIdentifierVM(visualIdentifier);
			}
			else
			{
				this.VisualIdentifier = null;
				FiefBarterable fiefBarterable;
				if ((fiefBarterable = this.Barterable as FiefBarterable) != null)
				{
					this.FiefFileName = fiefBarterable.TargetSettlement.SettlementComponent.BackgroundMeshName;
				}
			}
			this.TotalItemCount = this.Barterable.MaxAmount;
			this.CurrentOfferedAmount = 1;
			this.IsMultiple = this.TotalItemCount > 1;
			this.IsOffered = this.Barterable.IsOffered;
			this.RefreshValues();
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x000671E6 File Offset: 0x000653E6
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ItemLbl = this.Barterable.Name.ToString();
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x00067204 File Offset: 0x00065404
		public void RefreshCompabilityWithItem(BarterItemVM item, bool isItemGotOffered)
		{
			if (isItemGotOffered && !item.Barterable.IsCompatible(this.Barterable))
			{
				this._incompatibleItems.Add(item.Barterable);
			}
			else if (!isItemGotOffered && this._incompatibleItems.Contains(item.Barterable))
			{
				this._incompatibleItems.Remove(item.Barterable);
			}
			this.IsItemTransferrable = this._incompatibleItems.Count <= 0;
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x0006727C File Offset: 0x0006547C
		public void ExecuteAddOffered()
		{
			int num = (BarterItemVM.IsEntireStackModifierActive ? this.TotalItemCount : (this.CurrentOfferedAmount + (BarterItemVM.IsFiveStackModifierActive ? 5 : 1)));
			this.CurrentOfferedAmount = ((num < this.TotalItemCount) ? num : this.TotalItemCount);
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000672C4 File Offset: 0x000654C4
		public void ExecuteRemoveOffered()
		{
			int num = (BarterItemVM.IsEntireStackModifierActive ? 1 : (this.CurrentOfferedAmount - (BarterItemVM.IsFiveStackModifierActive ? 5 : 1)));
			this.CurrentOfferedAmount = ((num > 1) ? num : 1);
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000672FC File Offset: 0x000654FC
		public void ExecuteAction()
		{
			if (this.IsItemTransferrable)
			{
				this._onTransfer(this, false);
			}
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06001CD1 RID: 7377 RVA: 0x00067313 File Offset: 0x00065513
		// (set) Token: 0x06001CD2 RID: 7378 RVA: 0x0006731B File Offset: 0x0006551B
		[DataSourceProperty]
		public int TotalItemCount
		{
			get
			{
				return this._totalItemCount;
			}
			set
			{
				if (this._totalItemCount != value)
				{
					this._totalItemCount = value;
					base.OnPropertyChangedWithValue(value, "TotalItemCount");
					this.TotalItemCountText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(value);
				}
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06001CD3 RID: 7379 RVA: 0x00067345 File Offset: 0x00065545
		// (set) Token: 0x06001CD4 RID: 7380 RVA: 0x0006734D File Offset: 0x0006554D
		[DataSourceProperty]
		public string TotalItemCountText
		{
			get
			{
				return this._totalItemCountText;
			}
			set
			{
				if (this._totalItemCountText != value)
				{
					this._totalItemCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalItemCountText");
				}
			}
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x06001CD5 RID: 7381 RVA: 0x00067370 File Offset: 0x00065570
		// (set) Token: 0x06001CD6 RID: 7382 RVA: 0x00067378 File Offset: 0x00065578
		[DataSourceProperty]
		public int CurrentOfferedAmount
		{
			get
			{
				return this._currentOfferedAmount;
			}
			set
			{
				if (this._currentOfferedAmount != value)
				{
					this.Barterable.CurrentAmount = value;
					Action onAmountChange = this._onAmountChange;
					if (onAmountChange != null)
					{
						onAmountChange();
					}
					this._currentOfferedAmount = value;
					base.OnPropertyChangedWithValue(value, "CurrentOfferedAmount");
					this.CurrentOfferedAmountText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(value);
				}
			}
		}

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x06001CD7 RID: 7383 RVA: 0x000673CA File Offset: 0x000655CA
		// (set) Token: 0x06001CD8 RID: 7384 RVA: 0x000673D2 File Offset: 0x000655D2
		[DataSourceProperty]
		public string CurrentOfferedAmountText
		{
			get
			{
				return this._currentOfferedAmountText;
			}
			set
			{
				if (this._currentOfferedAmountText != value)
				{
					this._currentOfferedAmountText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentOfferedAmountText");
				}
			}
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x000673F5 File Offset: 0x000655F5
		// (set) Token: 0x06001CDA RID: 7386 RVA: 0x000673FD File Offset: 0x000655FD
		[DataSourceProperty]
		public string BarterableType
		{
			get
			{
				return this._barterableType;
			}
			set
			{
				if (this._barterableType != value)
				{
					this._barterableType = value;
					base.OnPropertyChangedWithValue<string>(value, "BarterableType");
				}
			}
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06001CDB RID: 7387 RVA: 0x00067420 File Offset: 0x00065620
		// (set) Token: 0x06001CDC RID: 7388 RVA: 0x00067428 File Offset: 0x00065628
		[DataSourceProperty]
		public bool HasVisualIdentifier
		{
			get
			{
				return this._hasVisualIdentifier;
			}
			set
			{
				if (this._hasVisualIdentifier != value)
				{
					this._hasVisualIdentifier = value;
					base.OnPropertyChangedWithValue(value, "HasVisualIdentifier");
				}
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06001CDD RID: 7389 RVA: 0x00067446 File Offset: 0x00065646
		// (set) Token: 0x06001CDE RID: 7390 RVA: 0x0006744E File Offset: 0x0006564E
		[DataSourceProperty]
		public bool IsMultiple
		{
			get
			{
				return this._isMultiple;
			}
			set
			{
				if (this._isMultiple != value)
				{
					this._isMultiple = value;
					base.OnPropertyChangedWithValue(value, "IsMultiple");
				}
			}
		}

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06001CDF RID: 7391 RVA: 0x0006746C File Offset: 0x0006566C
		// (set) Token: 0x06001CE0 RID: 7392 RVA: 0x00067474 File Offset: 0x00065674
		[DataSourceProperty]
		public bool IsSelectorActive
		{
			get
			{
				return this._isSelectorActive;
			}
			set
			{
				if (this._isSelectorActive != value)
				{
					this._isSelectorActive = value;
					base.OnPropertyChangedWithValue(value, "IsSelectorActive");
				}
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06001CE1 RID: 7393 RVA: 0x00067492 File Offset: 0x00065692
		// (set) Token: 0x06001CE2 RID: 7394 RVA: 0x0006749A File Offset: 0x0006569A
		[DataSourceProperty]
		public ImageIdentifierVM VisualIdentifier
		{
			get
			{
				return this._visualIdentifier;
			}
			set
			{
				if (this._visualIdentifier != value)
				{
					this._visualIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "VisualIdentifier");
				}
			}
		}

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06001CE3 RID: 7395 RVA: 0x000674B8 File Offset: 0x000656B8
		// (set) Token: 0x06001CE4 RID: 7396 RVA: 0x000674C0 File Offset: 0x000656C0
		[DataSourceProperty]
		public string ItemLbl
		{
			get
			{
				return this._itemLbl;
			}
			set
			{
				this._itemLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "ItemLbl");
			}
		}

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06001CE5 RID: 7397 RVA: 0x000674D5 File Offset: 0x000656D5
		// (set) Token: 0x06001CE6 RID: 7398 RVA: 0x000674DD File Offset: 0x000656DD
		[DataSourceProperty]
		public string FiefFileName
		{
			get
			{
				return this._fiefFileName;
			}
			set
			{
				this._fiefFileName = value;
				base.OnPropertyChangedWithValue<string>(value, "FiefFileName");
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06001CE7 RID: 7399 RVA: 0x000674F2 File Offset: 0x000656F2
		// (set) Token: 0x06001CE8 RID: 7400 RVA: 0x000674FA File Offset: 0x000656FA
		[DataSourceProperty]
		public bool IsItemTransferrable
		{
			get
			{
				return this._isItemTransferrable;
			}
			set
			{
				if (this._isFixed)
				{
					value = false;
				}
				if (this._isItemTransferrable != value)
				{
					this._isItemTransferrable = value;
					base.OnPropertyChangedWithValue(value, "IsItemTransferrable");
				}
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06001CE9 RID: 7401 RVA: 0x00067523 File Offset: 0x00065723
		// (set) Token: 0x06001CEA RID: 7402 RVA: 0x0006752B File Offset: 0x0006572B
		[DataSourceProperty]
		public bool IsOffered
		{
			get
			{
				return this._isOffered;
			}
			set
			{
				if (value != this._isOffered)
				{
					this._isOffered = value;
					base.OnPropertyChangedWithValue(value, "IsOffered");
				}
			}
		}

		// Token: 0x04000D8F RID: 3471
		public static bool IsEntireStackModifierActive;

		// Token: 0x04000D90 RID: 3472
		public static bool IsFiveStackModifierActive;

		// Token: 0x04000D91 RID: 3473
		private readonly BarterItemVM.BarterTransferEventDelegate _onTransfer;

		// Token: 0x04000D92 RID: 3474
		private readonly Action _onAmountChange;

		// Token: 0x04000D93 RID: 3475
		private bool _isFixed;

		// Token: 0x04000D94 RID: 3476
		private List<Barterable> _incompatibleItems = new List<Barterable>();

		// Token: 0x04000D95 RID: 3477
		public Barterable Barterable;

		// Token: 0x04000D96 RID: 3478
		public bool _isOffered;

		// Token: 0x04000D97 RID: 3479
		private bool _isItemTransferrable = true;

		// Token: 0x04000D98 RID: 3480
		private string _itemLbl;

		// Token: 0x04000D99 RID: 3481
		private string _fiefFileName;

		// Token: 0x04000D9A RID: 3482
		private string _barterableType = "NULL";

		// Token: 0x04000D9B RID: 3483
		private string _currentOfferedAmountText;

		// Token: 0x04000D9C RID: 3484
		private ImageIdentifierVM _visualIdentifier;

		// Token: 0x04000D9D RID: 3485
		private bool _isSelectorActive;

		// Token: 0x04000D9E RID: 3486
		private bool _hasVisualIdentifier;

		// Token: 0x04000D9F RID: 3487
		private bool _isMultiple;

		// Token: 0x04000DA0 RID: 3488
		private int _totalItemCount;

		// Token: 0x04000DA1 RID: 3489
		private string _totalItemCountText;

		// Token: 0x04000DA2 RID: 3490
		private int _currentOfferedAmount;

		// Token: 0x0200027A RID: 634
		// (Invoke) Token: 0x0600226C RID: 8812
		public delegate void BarterTransferEventDelegate(BarterItemVM itemVM, bool transferAll);
	}
}

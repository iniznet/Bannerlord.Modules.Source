using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryMode.ViewModelCollection.Tutorial
{
	// Token: 0x02000003 RID: 3
	public class TutorialVM : ViewModel
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000023B6 File Offset: 0x000005B6
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000023BD File Offset: 0x000005BD
		public static TutorialVM Instance { get; set; }

		// Token: 0x06000023 RID: 35 RVA: 0x000023C8 File Offset: 0x000005C8
		public TutorialVM(Action onTutorialDisabled)
		{
			TutorialVM.Instance = this;
			this._onTutorialDisabled = onTutorialDisabled;
			this.LeftItem = new TutorialItemVM();
			this.RightItem = new TutorialItemVM();
			this.BottomItem = new TutorialItemVM();
			this.TopItem = new TutorialItemVM();
			this.LeftBottomItem = new TutorialItemVM();
			this.LeftTopItem = new TutorialItemVM();
			this.RightBottomItem = new TutorialItemVM();
			this.RightTopItem = new TutorialItemVM();
			this.CenterItem = new TutorialItemVM();
			GameTexts.SetVariable("newline", "\n");
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000245C File Offset: 0x0000065C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LeftItem.RefreshValues();
			this.RightItem.RefreshValues();
			this.BottomItem.RefreshValues();
			this.TopItem.RefreshValues();
			this.LeftBottomItem.RefreshValues();
			this.LeftTopItem.RefreshValues();
			this.RightBottomItem.RefreshValues();
			this.RightTopItem.RefreshValues();
			this.CenterItem.RefreshValues();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000024D4 File Offset: 0x000006D4
		public void SetCurrentTutorial(TutorialItemVM.ItemPlacements placement, string tutorialTypeId, bool requiresMouse)
		{
			if (this._currentTutorialItem != null)
			{
				TutorialItemVM currentTutorialItem = this._currentTutorialItem;
				if (currentTutorialItem != null)
				{
					currentTutorialItem.CloseTutorialPanel();
				}
				this._currentTutorialItem = null;
			}
			TutorialItemVM item = this.GetItem(placement);
			if (!item.IsEnabled)
			{
				this._currentTutorialItem = item;
				this._currentTutorialItem.Init(tutorialTypeId, requiresMouse, new Action(this.FinalizeTutorial));
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002531 File Offset: 0x00000731
		private void ResetCurrentTutorial()
		{
			this._currentTutorialItem.CloseTutorialPanel();
			this._currentTutorialItem = null;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002548 File Offset: 0x00000748
		private TutorialItemVM GetItem(TutorialItemVM.ItemPlacements placement)
		{
			switch (placement)
			{
			case TutorialItemVM.ItemPlacements.Left:
				return this.LeftItem;
			case TutorialItemVM.ItemPlacements.Right:
				return this.RightItem;
			case TutorialItemVM.ItemPlacements.Top:
				return this.TopItem;
			case TutorialItemVM.ItemPlacements.Bottom:
				return this.BottomItem;
			case TutorialItemVM.ItemPlacements.TopLeft:
				return this.LeftTopItem;
			case TutorialItemVM.ItemPlacements.TopRight:
				return this.RightTopItem;
			case TutorialItemVM.ItemPlacements.BottomLeft:
				return this.LeftBottomItem;
			case TutorialItemVM.ItemPlacements.BottomRight:
				return this.RightBottomItem;
			case TutorialItemVM.ItemPlacements.Center:
				return this.CenterItem;
			default:
				return null;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000025C1 File Offset: 0x000007C1
		public void Tick(float dt)
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000025C3 File Offset: 0x000007C3
		public void CloseTutorialStep(bool finalizeAllSteps = false)
		{
			TutorialItemVM currentTutorialItem = this._currentTutorialItem;
			if (currentTutorialItem != null)
			{
				currentTutorialItem.CloseTutorialPanel();
			}
			this._currentTutorialItem = null;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000025DD File Offset: 0x000007DD
		public void FinalizeTutorial()
		{
			this._onTutorialDisabled();
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000025EA File Offset: 0x000007EA
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000025F2 File Offset: 0x000007F2
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002610 File Offset: 0x00000810
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002618 File Offset: 0x00000818
		[DataSourceProperty]
		public TutorialItemVM LeftItem
		{
			get
			{
				return this._leftItem;
			}
			set
			{
				if (value != this._leftItem)
				{
					this._leftItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "LeftItem");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002636 File Offset: 0x00000836
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000263E File Offset: 0x0000083E
		[DataSourceProperty]
		public TutorialItemVM RightItem
		{
			get
			{
				return this._rightItem;
			}
			set
			{
				if (value != this._rightItem)
				{
					this._rightItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "RightItem");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000031 RID: 49 RVA: 0x0000265C File Offset: 0x0000085C
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002664 File Offset: 0x00000864
		[DataSourceProperty]
		public TutorialItemVM BottomItem
		{
			get
			{
				return this._bottomItem;
			}
			set
			{
				if (value != this._bottomItem)
				{
					this._bottomItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "BottomItem");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002682 File Offset: 0x00000882
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000268A File Offset: 0x0000088A
		[DataSourceProperty]
		public TutorialItemVM TopItem
		{
			get
			{
				return this._topItem;
			}
			set
			{
				if (value != this._topItem)
				{
					this._topItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "TopItem");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000026A8 File Offset: 0x000008A8
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000026B0 File Offset: 0x000008B0
		[DataSourceProperty]
		public TutorialItemVM LeftBottomItem
		{
			get
			{
				return this._leftBottomItem;
			}
			set
			{
				if (value != this._leftBottomItem)
				{
					this._leftBottomItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "LeftBottomItem");
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000026CE File Offset: 0x000008CE
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000026D6 File Offset: 0x000008D6
		[DataSourceProperty]
		public TutorialItemVM LeftTopItem
		{
			get
			{
				return this._leftTopItem;
			}
			set
			{
				if (value != this._leftTopItem)
				{
					this._leftTopItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "LeftTopItem");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000026F4 File Offset: 0x000008F4
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000026FC File Offset: 0x000008FC
		[DataSourceProperty]
		public TutorialItemVM RightBottomItem
		{
			get
			{
				return this._rightBottomItem;
			}
			set
			{
				if (value != this._rightBottomItem)
				{
					this._rightBottomItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "RightBottomItem");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000271A File Offset: 0x0000091A
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002722 File Offset: 0x00000922
		[DataSourceProperty]
		public TutorialItemVM RightTopItem
		{
			get
			{
				return this._rightTopItem;
			}
			set
			{
				if (value != this._rightTopItem)
				{
					this._rightTopItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "RightTopItem");
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002740 File Offset: 0x00000940
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002748 File Offset: 0x00000948
		[DataSourceProperty]
		public TutorialItemVM CenterItem
		{
			get
			{
				return this._centerItem;
			}
			set
			{
				if (value != this._centerItem)
				{
					this._centerItem = value;
					base.OnPropertyChangedWithValue<TutorialItemVM>(value, "CenterItem");
				}
			}
		}

		// Token: 0x04000012 RID: 18
		private const float TutorialDelayInSeconds = 0f;

		// Token: 0x04000013 RID: 19
		private TutorialItemVM _currentTutorialItem;

		// Token: 0x04000014 RID: 20
		private Action _onTutorialDisabled;

		// Token: 0x04000015 RID: 21
		private bool _isVisible;

		// Token: 0x04000016 RID: 22
		private TutorialItemVM _leftItem;

		// Token: 0x04000017 RID: 23
		private TutorialItemVM _rightItem;

		// Token: 0x04000018 RID: 24
		private TutorialItemVM _bottomItem;

		// Token: 0x04000019 RID: 25
		private TutorialItemVM _topItem;

		// Token: 0x0400001A RID: 26
		private TutorialItemVM _leftBottomItem;

		// Token: 0x0400001B RID: 27
		private TutorialItemVM _leftTopItem;

		// Token: 0x0400001C RID: 28
		private TutorialItemVM _rightBottomItem;

		// Token: 0x0400001D RID: 29
		private TutorialItemVM _rightTopItem;

		// Token: 0x0400001E RID: 30
		private TutorialItemVM _centerItem;
	}
}

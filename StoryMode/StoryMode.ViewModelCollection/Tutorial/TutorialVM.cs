using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryMode.ViewModelCollection.Tutorial
{
	public class TutorialVM : ViewModel
	{
		public static TutorialVM Instance { get; set; }

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

		private void ResetCurrentTutorial()
		{
			this._currentTutorialItem.CloseTutorialPanel();
			this._currentTutorialItem = null;
		}

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

		public void Tick(float dt)
		{
		}

		public void CloseTutorialStep(bool finalizeAllSteps = false)
		{
			TutorialItemVM currentTutorialItem = this._currentTutorialItem;
			if (currentTutorialItem != null)
			{
				currentTutorialItem.CloseTutorialPanel();
			}
			this._currentTutorialItem = null;
		}

		public void FinalizeTutorial()
		{
			this._onTutorialDisabled();
		}

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

		private const float TutorialDelayInSeconds = 0f;

		private TutorialItemVM _currentTutorialItem;

		private Action _onTutorialDisabled;

		private bool _isVisible;

		private TutorialItemVM _leftItem;

		private TutorialItemVM _rightItem;

		private TutorialItemVM _bottomItem;

		private TutorialItemVM _topItem;

		private TutorialItemVM _leftBottomItem;

		private TutorialItemVM _leftTopItem;

		private TutorialItemVM _rightBottomItem;

		private TutorialItemVM _rightTopItem;

		private TutorialItemVM _centerItem;
	}
}

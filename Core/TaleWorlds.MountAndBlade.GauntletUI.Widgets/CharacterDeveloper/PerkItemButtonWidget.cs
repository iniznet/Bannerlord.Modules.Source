using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class PerkItemButtonWidget : ButtonWidget
	{
		public Brush NotEarnedPerkBrush { get; set; }

		public Brush EarnedNotSelectedPerkBrush { get; set; }

		public Brush InSelectionPerkBrush { get; set; }

		public Brush EarnedActivePerkBrush { get; set; }

		public Brush EarnedNotActivePerkBrush { get; set; }

		public Brush EarnedPreviousPerkNotSelectedPerkBrush { get; set; }

		public BrushWidget PerkVisualWidgetParent { get; set; }

		public PerkItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.PerkVisualWidget != null && ((this.PerkVisualWidget.Sprite != null && base.Context.SpriteData.GetSprite(this.PerkVisualWidget.Sprite.Name) == null) || this.PerkVisualWidget.Sprite == null))
			{
				this.PerkVisualWidget.Sprite = base.Context.SpriteData.GetSprite("SPPerks\\locked_fallback");
			}
			if (this._animState == PerkItemButtonWidget.AnimState.Start)
			{
				this._tickCount++;
				if (this._tickCount > 20)
				{
					this._animState = PerkItemButtonWidget.AnimState.Starting;
					return;
				}
			}
			else if (this._animState == PerkItemButtonWidget.AnimState.Starting)
			{
				this.PerkVisualWidgetParent.BrushRenderer.RestartAnimation();
				this._animState = PerkItemButtonWidget.AnimState.Playing;
			}
		}

		private void SetColorState(bool isActive)
		{
			if (this.PerkVisualWidget != null)
			{
				float num = (isActive ? 1f : 1f);
				float num2 = (isActive ? 1.3f : 0.75f);
				using (IEnumerator<Widget> enumerator = base.AllChildrenAndThis.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BrushWidget brushWidget;
						if ((brushWidget = enumerator.Current as BrushWidget) != null)
						{
							foreach (Style style in brushWidget.Brush.Styles)
							{
								for (int i = 0; i < style.LayerCount; i++)
								{
									StyleLayer layer = style.GetLayer(i);
									layer.AlphaFactor = num;
									layer.ColorFactor = num2;
								}
							}
						}
					}
				}
			}
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (this._isSelectable)
			{
				base.Context.TwoDimensionContext.PlaySound("popup");
			}
		}

		private void UpdatePerkStateVisual(int perkState)
		{
			switch (perkState)
			{
			case 0:
				this.PerkVisualWidgetParent.Brush = this.NotEarnedPerkBrush;
				this._isSelectable = false;
				return;
			case 1:
				this.PerkVisualWidgetParent.Brush = this.EarnedNotSelectedPerkBrush;
				this._animState = PerkItemButtonWidget.AnimState.Start;
				this._isSelectable = true;
				return;
			case 2:
				this.PerkVisualWidgetParent.Brush = this.InSelectionPerkBrush;
				this._isSelectable = false;
				return;
			case 3:
				this.PerkVisualWidgetParent.Brush = this.EarnedActivePerkBrush;
				this._isSelectable = false;
				return;
			case 4:
				this.PerkVisualWidgetParent.Brush = this.EarnedNotActivePerkBrush;
				this._isSelectable = false;
				return;
			case 5:
				this.PerkVisualWidgetParent.Brush = this.EarnedPreviousPerkNotSelectedPerkBrush;
				this._isSelectable = false;
				return;
			default:
				Debug.FailedAssert("Perk visual state is not defined", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\CharacterDeveloper\\PerkItemButtonWidget.cs", "UpdatePerkStateVisual", 134);
				return;
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (this._level != value)
				{
					this._level = value;
					base.OnPropertyChanged(value, "Level");
				}
			}
		}

		public Widget PerkVisualWidget
		{
			get
			{
				return this._perkVisualWidget;
			}
			set
			{
				if (this._perkVisualWidget != value)
				{
					this._perkVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "PerkVisualWidget");
				}
			}
		}

		public int PerkState
		{
			get
			{
				return this._perkState;
			}
			set
			{
				if (this._perkState != value)
				{
					this._perkState = value;
					base.OnPropertyChanged(value, "PerkState");
					this.UpdatePerkStateVisual(this.PerkState);
				}
			}
		}

		public int AlternativeType
		{
			get
			{
				return this._alternativeType;
			}
			set
			{
				if (this._alternativeType != value)
				{
					this._alternativeType = value;
					base.OnPropertyChanged(value, "AlternativeType");
				}
			}
		}

		private PerkItemButtonWidget.AnimState _animState;

		private int _tickCount;

		private bool _isSelectable;

		private int _level;

		private int _alternativeType;

		private int _perkState = -1;

		private Widget _perkVisualWidget;

		public enum AnimState
		{
			Idle,
			Start,
			Starting,
			Playing
		}
	}
}

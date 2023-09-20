using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	public class TooltipWidget : ListPanel
	{
		private float TooltipOffset
		{
			get
			{
				return 30f;
			}
		}

		public Color AllyColor { get; set; }

		public Color EnemyColor { get; set; }

		public Color NeutralColor { get; set; }

		public Widget PropertyListBackground { get; set; }

		public ListPanel PropertyList { get; set; }

		public bool DoNotUpdatePosition { get; set; }

		public TooltipWidget(UIContext context)
			: base(context)
		{
			base.IsVisible = false;
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (this._isActive != base.IsVisible)
			{
				this._isActive = base.IsVisible;
				if (this._isActive)
				{
					this._frame = 0;
					this._currentTooltipAnimState = TooltipWidget.TooltipAnimStates.Start;
					return;
				}
				this._currentTooltipAnimState = TooltipWidget.TooltipAnimStates.Finished;
			}
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this.DoNotUpdatePosition && this._isActive)
			{
				this.UpdateAnimationProperties(dt);
				this.UpdateBattleScopes();
				Vector2 vector;
				if (this._frame < 3)
				{
					this._tooltipPosition = base.EventManager.MousePosition;
					vector..ctor(-2000f, -2000f);
				}
				else
				{
					vector = this._tooltipPosition;
				}
				this._frame++;
				float num = 0f;
				HorizontalAlignment horizontalAlignment;
				if ((double)vector.X < (double)base.EventManager.PageSize.X * 0.5)
				{
					horizontalAlignment = HorizontalAlignment.Left;
					num = this.TooltipOffset;
				}
				else
				{
					horizontalAlignment = HorizontalAlignment.Right;
					num -= 0f;
					vector..ctor(-(base.EventManager.PageSize.X - vector.X), vector.Y);
				}
				VerticalAlignment verticalAlignment;
				float num2;
				if ((double)vector.Y < (double)base.EventManager.PageSize.Y * 0.5)
				{
					verticalAlignment = VerticalAlignment.Top;
					num2 = this.TooltipOffset;
				}
				else
				{
					verticalAlignment = VerticalAlignment.Bottom;
					num2 = 0f;
					vector..ctor(vector.X, -(base.EventManager.PageSize.Y - vector.Y));
				}
				vector += new Vector2(num, num2);
				if (this._frame > 3)
				{
					if (base.Size.Y > base.EventManager.PageSize.Y)
					{
						verticalAlignment = VerticalAlignment.Center;
						vector..ctor(vector.X, 0f);
					}
					else
					{
						if (verticalAlignment == VerticalAlignment.Top && vector.Y + base.Size.Y > base.EventManager.PageSize.Y)
						{
							vector += new Vector2(0f, -(vector.Y + base.Size.Y - base.EventManager.PageSize.Y));
						}
						if (verticalAlignment == VerticalAlignment.Bottom && vector.Y - base.Size.Y + base.EventManager.PageSize.Y < 0f)
						{
							vector += new Vector2(0f, -(vector.Y - base.Size.Y + base.EventManager.PageSize.Y));
						}
					}
				}
				base.HorizontalAlignment = horizontalAlignment;
				base.VerticalAlignment = verticalAlignment;
				base.ScaledPositionXOffset = vector.X + this._currentTooltipAnimXOffset * base._scaleToUse - base.EventManager.LeftUsableAreaStart;
				base.ScaledPositionYOffset = vector.Y - base.EventManager.TopUsableAreaStart;
			}
		}

		private void UpdateBattleScopes()
		{
			bool flag = false;
			foreach (TooltipPropertyWidget tooltipPropertyWidget in this.PropertyWidgets)
			{
				if (tooltipPropertyWidget.IsBattleMode)
				{
					flag = true;
				}
				else if (tooltipPropertyWidget.IsBattleModeOver)
				{
					flag = false;
				}
				tooltipPropertyWidget.SetBattleScope(flag);
			}
		}

		private float GetBattleScopeSize()
		{
			bool flag = false;
			float num = 0f;
			if (this.PropertyList != null)
			{
				for (int i = 0; i < this.PropertyList.ChildCount; i++)
				{
					TooltipPropertyWidget tooltipPropertyWidget;
					if ((tooltipPropertyWidget = this.PropertyList.GetChild(i) as TooltipPropertyWidget) != null)
					{
						if (tooltipPropertyWidget.IsBattleMode)
						{
							flag = true;
						}
						else if (tooltipPropertyWidget.IsBattleModeOver)
						{
							flag = false;
						}
						if (flag)
						{
							float num2 = ((tooltipPropertyWidget.ValueLabel.Size.X > tooltipPropertyWidget.DefinitionLabel.Size.X) ? tooltipPropertyWidget.ValueLabel.Size.X : tooltipPropertyWidget.DefinitionLabel.Size.X);
							if (num2 > num)
							{
								num = num2;
							}
						}
					}
				}
			}
			return num;
		}

		private void UpdateAnimationProperties(float dt)
		{
			this.TooltipAnimXOffset = 0f;
			this.TooltipAnimTime = 0.35f;
			if (this._currentTooltipAnimState == TooltipWidget.TooltipAnimStates.Start)
			{
				this._currentTooltipAnimTime = 0f;
				this._currentTooltipAnimXOffset = this.TooltipAnimXOffset;
				this._currentTooltipAnimAlpha = 0f;
				this._currentTooltipAnimState = TooltipWidget.TooltipAnimStates.Animating;
			}
			else if (this._currentTooltipAnimState == TooltipWidget.TooltipAnimStates.Animating)
			{
				this._currentTooltipAnimTime += dt;
				if (this._currentTooltipAnimTime <= this.TooltipAnimTime)
				{
					this._currentTooltipAnimXOffset = MathF.Lerp(this._currentTooltipAnimXOffset, 0f, this._currentTooltipAnimTime / this.TooltipAnimTime, 1E-05f);
					this._currentTooltipAnimAlpha = MathF.Lerp(this._currentTooltipAnimAlpha, 1f, this._currentTooltipAnimTime / this.TooltipAnimTime, 1E-05f);
				}
				else
				{
					this._currentTooltipAnimState = TooltipWidget.TooltipAnimStates.Finished;
				}
			}
			else
			{
				this._currentTooltipAnimXOffset = 0f;
				this._currentTooltipAnimAlpha = 1f;
			}
			foreach (Widget widget in base.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(this._currentTooltipAnimAlpha);
			}
		}

		private void UpdateRelationBrushes()
		{
			TooltipPropertyWidget tooltipPropertyWidget = this.PropertyWidgets.SingleOrDefault((TooltipPropertyWidget p) => p.IsRelation);
			if (tooltipPropertyWidget != null)
			{
				if ((tooltipPropertyWidget.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarFirstAlly) == TooltipPropertyWidget.TooltipPropertyFlags.WarFirstAlly)
				{
					this._definitionRelationBrush = this.AllyTroopsTextBrush;
				}
				else if ((tooltipPropertyWidget.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarFirstEnemy) == TooltipPropertyWidget.TooltipPropertyFlags.WarFirstEnemy)
				{
					this._definitionRelationBrush = this.EnemyTroopsTextBrush;
				}
				else
				{
					this._definitionRelationBrush = this.NeutralTroopsTextBrush;
				}
				if ((tooltipPropertyWidget.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarSecondAlly) == TooltipPropertyWidget.TooltipPropertyFlags.WarSecondAlly)
				{
					this._valueRelationBrush = this.AllyTroopsTextBrush;
					return;
				}
				if ((tooltipPropertyWidget.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarSecondEnemy) == TooltipPropertyWidget.TooltipPropertyFlags.WarSecondEnemy)
				{
					this._valueRelationBrush = this.EnemyTroopsTextBrush;
					return;
				}
				this._valueRelationBrush = this.NeutralTroopsTextBrush;
			}
		}

		private IEnumerable<TooltipPropertyWidget> PropertyWidgets
		{
			get
			{
				if (this.PropertyList != null)
				{
					int num;
					for (int i = 0; i < this.PropertyList.ChildCount; i = num + 1)
					{
						TooltipPropertyWidget tooltipPropertyWidget;
						if ((tooltipPropertyWidget = this.PropertyList.GetChild(i) as TooltipPropertyWidget) != null)
						{
							yield return tooltipPropertyWidget;
						}
						num = i;
					}
				}
				yield break;
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool flag = false;
			float battleScopeSize = this.GetBattleScopeSize();
			float num = -1f;
			float num2 = -1f;
			this._definitionRelationBrush = null;
			this._valueRelationBrush = null;
			if (this.PropertyList != null)
			{
				for (int i = 0; i < this.PropertyList.ChildCount; i++)
				{
					TooltipPropertyWidget tooltipPropertyWidget;
					if ((tooltipPropertyWidget = this.PropertyList.GetChild(i) as TooltipPropertyWidget) != null && tooltipPropertyWidget.IsTwoColumn && !tooltipPropertyWidget.IsMultiLine)
					{
						if (num < tooltipPropertyWidget.ValueLabelContainer.Size.X)
						{
							num = tooltipPropertyWidget.ValueLabelContainer.Size.X;
						}
						if (num2 < tooltipPropertyWidget.DefinitionLabelContainer.Size.X)
						{
							num2 = tooltipPropertyWidget.DefinitionLabelContainer.Size.X;
						}
					}
				}
				for (int j = 0; j < this.PropertyList.ChildCount; j++)
				{
					TooltipPropertyWidget tooltipPropertyWidget2;
					if ((tooltipPropertyWidget2 = this.PropertyList.GetChild(j) as TooltipPropertyWidget) != null)
					{
						if (tooltipPropertyWidget2.IsBattleMode)
						{
							flag = true;
						}
						else if (tooltipPropertyWidget2.IsBattleModeOver)
						{
							flag = false;
						}
						if (flag && (this._definitionRelationBrush == null || this._valueRelationBrush == null))
						{
							this.UpdateRelationBrushes();
						}
						tooltipPropertyWidget2.RefreshSize(flag, battleScopeSize, num, num2, this._definitionRelationBrush, this._valueRelationBrush);
					}
				}
			}
			if (this.PropertyListBackground != null)
			{
				if (this.Mode == 2)
				{
					this.PropertyListBackground.Color = this.AllyColor;
					return;
				}
				if (this.Mode == 3)
				{
					this.PropertyListBackground.Color = this.EnemyColor;
					return;
				}
				this.PropertyListBackground.Color = this.NeutralColor;
			}
		}

		[Editor(false)]
		public float TooltipAnimTime
		{
			get
			{
				return this._tooltipAnimTime;
			}
			set
			{
				if (this._tooltipAnimTime != value)
				{
					this._tooltipAnimTime = value;
					base.OnPropertyChanged(value, "TooltipAnimTime");
				}
			}
		}

		[Editor(false)]
		public int Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				if (this._mode != value)
				{
					this._mode = value;
				}
			}
		}

		[Editor(false)]
		public float TooltipAnimXOffset
		{
			get
			{
				return this._tooltipAnimXOffset;
			}
			set
			{
				if (this._tooltipAnimXOffset != value)
				{
					this._tooltipAnimXOffset = value;
					base.OnPropertyChanged(value, "TooltipAnimXOffset");
				}
			}
		}

		[Editor(false)]
		public Brush NeutralTroopsTextBrush
		{
			get
			{
				return this._neutralTroopsTextBrush;
			}
			set
			{
				if (this._neutralTroopsTextBrush != value)
				{
					this._neutralTroopsTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "NeutralTroopsTextBrush");
				}
			}
		}

		[Editor(false)]
		public Brush EnemyTroopsTextBrush
		{
			get
			{
				return this._enemyTroopsTextBrush;
			}
			set
			{
				if (this._enemyTroopsTextBrush != value)
				{
					this._enemyTroopsTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "EnemyTroopsTextBrush");
				}
			}
		}

		[Editor(false)]
		public Brush AllyTroopsTextBrush
		{
			get
			{
				return this._allyTroopsTextBrush;
			}
			set
			{
				if (this._allyTroopsTextBrush != value)
				{
					this._allyTroopsTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "AllyTroopsTextBrush");
				}
			}
		}

		private bool _isActive;

		private int _frame;

		private Vector2 _tooltipPosition;

		private TooltipWidget.TooltipAnimStates _currentTooltipAnimState;

		private float _currentTooltipAnimTime;

		private float _currentTooltipAnimAlpha;

		private float _currentTooltipAnimXOffset;

		private Brush _definitionRelationBrush;

		private Brush _valueRelationBrush;

		private float _tooltipAnimXOffset = 8f;

		private float _tooltipAnimTime = 0.5f;

		private int _mode;

		private Brush _neutralTroopsTextBrush;

		private Brush _allyTroopsTextBrush;

		private Brush _enemyTroopsTextBrush;

		public enum TooltipAnimStates
		{
			Start,
			Animating,
			Finished
		}
	}
}

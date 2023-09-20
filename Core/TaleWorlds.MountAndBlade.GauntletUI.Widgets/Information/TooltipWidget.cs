using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	// Token: 0x0200012B RID: 299
	public class TooltipWidget : ListPanel
	{
		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0002C7AA File Offset: 0x0002A9AA
		private float TooltipOffset
		{
			get
			{
				return 30f;
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x0002C7B1 File Offset: 0x0002A9B1
		// (set) Token: 0x06000FC2 RID: 4034 RVA: 0x0002C7B9 File Offset: 0x0002A9B9
		public Color AllyColor { get; set; }

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x0002C7C2 File Offset: 0x0002A9C2
		// (set) Token: 0x06000FC4 RID: 4036 RVA: 0x0002C7CA File Offset: 0x0002A9CA
		public Color EnemyColor { get; set; }

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x0002C7D3 File Offset: 0x0002A9D3
		// (set) Token: 0x06000FC6 RID: 4038 RVA: 0x0002C7DB File Offset: 0x0002A9DB
		public Color NeutralColor { get; set; }

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x0002C7E4 File Offset: 0x0002A9E4
		// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x0002C7EC File Offset: 0x0002A9EC
		public Widget PropertyListBackground { get; set; }

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x0002C7F5 File Offset: 0x0002A9F5
		// (set) Token: 0x06000FCA RID: 4042 RVA: 0x0002C7FD File Offset: 0x0002A9FD
		public ListPanel PropertyList { get; set; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0002C806 File Offset: 0x0002AA06
		// (set) Token: 0x06000FCC RID: 4044 RVA: 0x0002C80E File Offset: 0x0002AA0E
		public bool DoNotUpdatePosition { get; set; }

		// Token: 0x06000FCD RID: 4045 RVA: 0x0002C817 File Offset: 0x0002AA17
		public TooltipWidget(UIContext context)
			: base(context)
		{
			base.IsVisible = false;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0002C83D File Offset: 0x0002AA3D
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

		// Token: 0x06000FCF RID: 4047 RVA: 0x0002C880 File Offset: 0x0002AA80
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
					vector = new Vector2(-2000f, -2000f);
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
					vector = new Vector2(-(base.EventManager.PageSize.X - vector.X), vector.Y);
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
					vector = new Vector2(vector.X, -(base.EventManager.PageSize.Y - vector.Y));
				}
				vector += new Vector2(num, num2);
				if (this._frame > 3)
				{
					if (base.Size.Y > base.EventManager.PageSize.Y)
					{
						verticalAlignment = VerticalAlignment.Center;
						vector = new Vector2(vector.X, 0f);
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

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0002CB24 File Offset: 0x0002AD24
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

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0002CB8C File Offset: 0x0002AD8C
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

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0002CC48 File Offset: 0x0002AE48
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

		// Token: 0x06000FD3 RID: 4051 RVA: 0x0002CD80 File Offset: 0x0002AF80
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

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0002CE44 File Offset: 0x0002B044
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

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0002CE54 File Offset: 0x0002B054
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

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x0002CFF0 File Offset: 0x0002B1F0
		// (set) Token: 0x06000FD7 RID: 4055 RVA: 0x0002CFF8 File Offset: 0x0002B1F8
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

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0002D016 File Offset: 0x0002B216
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x0002D01E File Offset: 0x0002B21E
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

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0002D030 File Offset: 0x0002B230
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0002D038 File Offset: 0x0002B238
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

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0002D056 File Offset: 0x0002B256
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x0002D05E File Offset: 0x0002B25E
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

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0002D07C File Offset: 0x0002B27C
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x0002D084 File Offset: 0x0002B284
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

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x0002D0A2 File Offset: 0x0002B2A2
		// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x0002D0AA File Offset: 0x0002B2AA
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

		// Token: 0x04000744 RID: 1860
		private bool _isActive;

		// Token: 0x04000745 RID: 1861
		private int _frame;

		// Token: 0x04000746 RID: 1862
		private Vector2 _tooltipPosition;

		// Token: 0x0400074D RID: 1869
		private TooltipWidget.TooltipAnimStates _currentTooltipAnimState;

		// Token: 0x0400074E RID: 1870
		private float _currentTooltipAnimTime;

		// Token: 0x0400074F RID: 1871
		private float _currentTooltipAnimAlpha;

		// Token: 0x04000750 RID: 1872
		private float _currentTooltipAnimXOffset;

		// Token: 0x04000751 RID: 1873
		private Brush _definitionRelationBrush;

		// Token: 0x04000752 RID: 1874
		private Brush _valueRelationBrush;

		// Token: 0x04000753 RID: 1875
		private float _tooltipAnimXOffset = 8f;

		// Token: 0x04000754 RID: 1876
		private float _tooltipAnimTime = 0.5f;

		// Token: 0x04000755 RID: 1877
		private int _mode;

		// Token: 0x04000756 RID: 1878
		private Brush _neutralTroopsTextBrush;

		// Token: 0x04000757 RID: 1879
		private Brush _allyTroopsTextBrush;

		// Token: 0x04000758 RID: 1880
		private Brush _enemyTroopsTextBrush;

		// Token: 0x0200019D RID: 413
		public enum TooltipAnimStates
		{
			// Token: 0x04000930 RID: 2352
			Start,
			// Token: 0x04000931 RID: 2353
			Animating,
			// Token: 0x04000932 RID: 2354
			Finished
		}
	}
}

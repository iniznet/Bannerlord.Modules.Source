using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	public class PropertyBasedTooltipWidget : TooltipWidget
	{
		public Color AllyColor { get; set; }

		public Color EnemyColor { get; set; }

		public Color NeutralColor { get; set; }

		public Widget PropertyListBackground { get; set; }

		public ListPanel PropertyList { get; set; }

		public PropertyBasedTooltipWidget(UIContext context)
			: base(context)
		{
			this._animationDelayInFrames = 2;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateBattleScopes();
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

		private Brush _definitionRelationBrush;

		private Brush _valueRelationBrush;

		private int _mode;

		private Brush _neutralTroopsTextBrush;

		private Brush _allyTroopsTextBrush;

		private Brush _enemyTroopsTextBrush;
	}
}

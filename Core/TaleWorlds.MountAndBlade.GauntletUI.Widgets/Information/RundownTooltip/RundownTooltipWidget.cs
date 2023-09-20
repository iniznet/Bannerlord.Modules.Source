using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information.RundownTooltip
{
	public class RundownTooltipWidget : TooltipWidget
	{
		public RundownTooltipWidget(UIContext context)
			: base(context)
		{
			this.RefreshOnNextLateUpdate();
			this._animationDelayInFrames = 2;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.LineContainerWidget != null)
			{
				GridLayout gridLayout = this.LineContainerWidget.GridLayout;
				bool flag = this._lastCheckedColumnWidths.Count != gridLayout.ColumnWidths.Count;
				bool flag2 = false;
				for (int i = 0; i < this._lastCheckedColumnWidths.Count; i++)
				{
					float num = this._lastCheckedColumnWidths[i];
					float num2 = ((i < gridLayout.ColumnWidths.Count) ? gridLayout.ColumnWidths[i] : (-1f));
					if (MathF.Abs(num - num2) > 1E-05f)
					{
						flag2 = true;
						break;
					}
				}
				if (flag || flag2)
				{
					this._lastCheckedColumnWidths = gridLayout.ColumnWidths;
					RundownColumnDividerCollectionWidget dividerCollectionWidget = this.DividerCollectionWidget;
					if (dividerCollectionWidget == null)
					{
						return;
					}
					dividerCollectionWidget.Refresh(gridLayout.ColumnWidths);
				}
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			GridLayout gridLayout = this.LineContainerWidget.GridLayout;
			for (int i = 0; i < this.LineContainerWidget.ChildCount; i++)
			{
				RundownLineWidget rundownLineWidget = this.LineContainerWidget.GetChild(i) as RundownLineWidget;
				int num = i / this.LineContainerWidget.RowCount;
				rundownLineWidget.RefreshValueOffset((num < gridLayout.ColumnWidths.Count) ? gridLayout.ColumnWidths[num] : (-1f));
			}
		}

		private void Refresh()
		{
			RundownTooltipWidget.ValueCategorization valueCategorizationAsInt = (RundownTooltipWidget.ValueCategorization)this.ValueCategorizationAsInt;
			if (this.LineContainerWidget != null)
			{
				List<RundownLineWidget> list = new List<RundownLineWidget>();
				float num = 0f;
				float num2 = 0f;
				using (List<Widget>.Enumerator enumerator = this.LineContainerWidget.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RundownLineWidget rundownLineWidget;
						if ((rundownLineWidget = enumerator.Current as RundownLineWidget) != null)
						{
							list.Add(rundownLineWidget);
							float value = rundownLineWidget.Value;
							if (value < num)
							{
								num = value;
							}
							if (value > num2)
							{
								num2 = value;
							}
						}
					}
				}
				foreach (RundownLineWidget rundownLineWidget2 in list)
				{
					float value2 = rundownLineWidget2.Value;
					Brush brush = rundownLineWidget2.ValueTextWidget.Brush;
					Color color = this._defaultValueColor;
					if (valueCategorizationAsInt != RundownTooltipWidget.ValueCategorization.None)
					{
						float num3 = ((value2 < 0f) ? num : num2);
						float num4 = MathF.Abs(value2 / num3);
						float num5 = (float)((valueCategorizationAsInt == RundownTooltipWidget.ValueCategorization.LargeIsBetter) ? 1 : (-1)) * value2;
						color = Color.Lerp(this._defaultValueColor, (num5 < 0f) ? this._negativeValueColor : this._positiveValueColor, num4);
					}
					brush.FontColor = color;
				}
			}
			this._willRefreshThisFrame = false;
		}

		private void RefreshOnNextLateUpdate()
		{
			if (!this._willRefreshThisFrame)
			{
				this._willRefreshThisFrame = true;
				base.EventManager.AddLateUpdateAction(this, delegate(float _)
				{
					this.Refresh();
				}, 1);
			}
		}

		private void OnLineContainerEventFire(Widget widget, string eventName, object[] args)
		{
			if (eventName == "ItemAdd" || eventName == "ItemRemove")
			{
				this.RefreshOnNextLateUpdate();
			}
		}

		[Editor(false)]
		public GridWidget LineContainerWidget
		{
			get
			{
				return this._lineContainerWidget;
			}
			set
			{
				if (value != this._lineContainerWidget)
				{
					if (this._lineContainerWidget != null)
					{
						this._lineContainerWidget.EventFire -= this.OnLineContainerEventFire;
					}
					this._lineContainerWidget = value;
					base.OnPropertyChanged<GridWidget>(value, "LineContainerWidget");
					this.RefreshOnNextLateUpdate();
					if (this._lineContainerWidget != null)
					{
						this._lineContainerWidget.EventFire += this.OnLineContainerEventFire;
					}
				}
			}
		}

		[Editor(false)]
		public RundownColumnDividerCollectionWidget DividerCollectionWidget
		{
			get
			{
				return this._dividerCollectionWidget;
			}
			set
			{
				if (value != this._dividerCollectionWidget)
				{
					this._dividerCollectionWidget = value;
					base.OnPropertyChanged<RundownColumnDividerCollectionWidget>(value, "DividerCollectionWidget");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		[Editor(false)]
		public int ValueCategorizationAsInt
		{
			get
			{
				return this._valueCategorizationAsInt;
			}
			set
			{
				if (value != this._valueCategorizationAsInt)
				{
					this._valueCategorizationAsInt = value;
					base.OnPropertyChanged(value, "ValueCategorizationAsInt");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		private readonly Color _defaultValueColor = new Color(1f, 1f, 1f, 1f);

		private readonly Color _negativeValueColor = new Color(0.8352941f, 0.12941177f, 0.12941177f, 1f);

		private readonly Color _positiveValueColor = new Color(0.38039216f, 0.7490196f, 0.33333334f, 1f);

		private bool _willRefreshThisFrame;

		private IReadOnlyList<float> _lastCheckedColumnWidths = new List<float>();

		private GridWidget _lineContainerWidget;

		private RundownColumnDividerCollectionWidget _dividerCollectionWidget;

		private int _valueCategorizationAsInt;

		private enum ValueCategorization
		{
			None,
			LargeIsBetter,
			SmallIsBetter
		}
	}
}

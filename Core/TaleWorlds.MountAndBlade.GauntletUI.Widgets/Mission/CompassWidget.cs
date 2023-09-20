using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class CompassWidget : Widget
	{
		public CompassWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.HandleHorizontalPositioning();
			this.HandleMarkerPositioning();
		}

		private void HandleHorizontalPositioning()
		{
			if (this.ItemContainerPanel.ChildCount <= 0)
			{
				return;
			}
			List<List<Widget>> list = new List<List<Widget>>();
			float num = 0f;
			float num2 = this.ItemContainerPanel.ParentWidget.MeasuredSize.X * base._inverseScaleToUse - 50f;
			for (int i = 0; i < this.ItemContainerPanel.ChildCount; i++)
			{
				CompassElementWidget compassElementWidget = this.ItemContainerPanel.GetChild(i) as CompassElementWidget;
				if (!compassElementWidget.IsHidden)
				{
					float num3 = (compassElementWidget.Position + 1f) * 0.5f;
					compassElementWidget.MarginLeft = MBMath.Lerp(num, num2, num3, 1E-05f);
					bool flag = false;
					if (list.Count > 0)
					{
						List<Widget> list2 = list[list.Count - 1];
						int j = list2.Count - 1;
						while (j >= 0)
						{
							if (Math.Abs(list2[j].MarginLeft - compassElementWidget.MarginLeft) < 10f)
							{
								flag = true;
								compassElementWidget.MarginLeft = list[list.Count - 1][list[list.Count - 1].Count - 1].MarginLeft + 10f;
								list[list.Count - 1].Add(compassElementWidget);
								if (compassElementWidget.MarginLeft > num2)
								{
									float marginLeft = compassElementWidget.MarginLeft;
									compassElementWidget.MarginLeft = num2;
									float num4 = marginLeft - compassElementWidget.MarginLeft;
									for (int k = 1; k < list2.Count; k++)
									{
										int num5 = list2.Count - 1 - k;
										list2[num5].MarginLeft -= num4;
									}
									break;
								}
								break;
							}
							else
							{
								j--;
							}
						}
					}
					if (!flag)
					{
						list.Add(new List<Widget>());
						list[list.Count - 1].Add(compassElementWidget);
					}
				}
			}
		}

		private void HandleMarkerPositioning()
		{
			if (this.MarkerContainerPanel.ChildCount <= 0)
			{
				return;
			}
			float num = 0f;
			float num2 = this.MarkerContainerPanel.ParentWidget.MeasuredSize.X * base._inverseScaleToUse;
			for (int i = 0; i < this.MarkerContainerPanel.ChildCount; i++)
			{
				CompassMarkerTextWidget compassMarkerTextWidget = this.MarkerContainerPanel.GetChild(i) as CompassMarkerTextWidget;
				float num3 = (compassMarkerTextWidget.Position + 1f) * 0.5f;
				compassMarkerTextWidget.MarginLeft = MBMath.Lerp(num, num2, num3, 1E-05f) - compassMarkerTextWidget.Size.X * 0.5f;
				compassMarkerTextWidget.IsHidden = MBMath.ApproximatelyEquals(num3, 0f, 1E-05f) || MBMath.ApproximatelyEquals(num3, 1f, 1E-05f);
			}
		}

		[DataSourceProperty]
		public Widget ItemContainerPanel
		{
			get
			{
				return this._itemContainerPanel;
			}
			set
			{
				if (this._itemContainerPanel != value)
				{
					this._itemContainerPanel = value;
					base.OnPropertyChanged<Widget>(value, "ItemContainerPanel");
				}
			}
		}

		[DataSourceProperty]
		public Widget MarkerContainerPanel
		{
			get
			{
				return this._markerContainerPanel;
			}
			set
			{
				if (this._markerContainerPanel != value)
				{
					this._markerContainerPanel = value;
					base.OnPropertyChanged<Widget>(value, "MarkerContainerPanel");
				}
			}
		}

		private Widget _itemContainerPanel;

		private Widget _markerContainerPanel;
	}
}

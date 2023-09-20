using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C7 RID: 199
	public class CompassWidget : Widget
	{
		// Token: 0x060009FC RID: 2556 RVA: 0x0001C6D9 File Offset: 0x0001A8D9
		public CompassWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x0001C6E2 File Offset: 0x0001A8E2
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.HandleHorizontalPositioning();
			this.HandleMarkerPositioning();
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0001C6F8 File Offset: 0x0001A8F8
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

		// Token: 0x060009FF RID: 2559 RVA: 0x0001C8E0 File Offset: 0x0001AAE0
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

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000A00 RID: 2560 RVA: 0x0001C9B1 File Offset: 0x0001ABB1
		// (set) Token: 0x06000A01 RID: 2561 RVA: 0x0001C9B9 File Offset: 0x0001ABB9
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

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000A02 RID: 2562 RVA: 0x0001C9D7 File Offset: 0x0001ABD7
		// (set) Token: 0x06000A03 RID: 2563 RVA: 0x0001C9DF File Offset: 0x0001ABDF
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

		// Token: 0x0400048F RID: 1167
		private Widget _itemContainerPanel;

		// Token: 0x04000490 RID: 1168
		private Widget _markerContainerPanel;
	}
}

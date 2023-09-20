using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	// Token: 0x020000DD RID: 221
	public class NameMarkerScreenWidget : Widget
	{
		// Token: 0x06000B6B RID: 2923 RVA: 0x0001F9F7 File Offset: 0x0001DBF7
		public NameMarkerScreenWidget(UIContext context)
			: base(context)
		{
			this._markers = new List<NameMarkerListPanel>();
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0001FA0C File Offset: 0x0001DC0C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num = (this.IsMarkersEnabled ? this.TargetAlphaValue : 0f);
			base.AlphaFactor = Mathf.Lerp(base.AlphaFactor, num, dt * 10f);
			bool flag = this._markers.Count > 0;
			for (int i = 0; i < this._markers.Count; i++)
			{
				this._markers[i].Update(dt);
				flag &= this._markers[i].TypeVisualWidget.AlphaFactor > 0f;
			}
			if (flag)
			{
				this._markers.Sort((NameMarkerListPanel m1, NameMarkerListPanel m2) => m1.Rect.Left.CompareTo(m2.Rect.Left));
				for (int j = 0; j < this._markers.Count; j++)
				{
					int num2 = j + 1;
					while (num2 < this._markers.Count && this._markers[num2].Rect.Left - this._markers[j].Rect.Left <= this._markers[j].Rect.Width)
					{
						if (this._markers[j].Rect.IsOverlapping(this._markers[num2].Rect))
						{
							this._markers[num2].ScaledPositionXOffset += this._markers[j].Rect.Right - this._markers[num2].Rect.Left;
							this._markers[num2].UpdateRectangle();
						}
						num2++;
					}
				}
				NameMarkerListPanel nameMarkerListPanel = null;
				float num3 = 3600f;
				for (int k = 0; k < this._markers.Count; k++)
				{
					if (this._markers[k].IsInScreenBoundaries)
					{
						NameMarkerListPanel nameMarkerListPanel2 = this._markers[k];
						float num4 = base.EventManager.PageSize.X / 2f;
						float num5 = base.EventManager.PageSize.Y / 2f;
						float num6 = Mathf.Abs(num4 - nameMarkerListPanel2.Rect.CenterX);
						float num7 = Mathf.Abs(num5 - nameMarkerListPanel2.Rect.CenterY);
						float num8 = num6 * num6 + num7 * num7;
						if (num8 < num3)
						{
							num3 = num8;
							nameMarkerListPanel = nameMarkerListPanel2;
						}
					}
				}
				if (nameMarkerListPanel != this._lastFocusedWidget)
				{
					if (this._lastFocusedWidget != null)
					{
						this._lastFocusedWidget.IsFocused = false;
					}
					this._lastFocusedWidget = nameMarkerListPanel;
					if (this._lastFocusedWidget != null)
					{
						this._lastFocusedWidget.IsFocused = true;
					}
				}
			}
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0001FCDC File Offset: 0x0001DEDC
		private void OnMarkersChanged(Widget widget, string eventName, object[] args)
		{
			NameMarkerListPanel nameMarkerListPanel;
			if (args.Length == 1 && (nameMarkerListPanel = args[0] as NameMarkerListPanel) != null)
			{
				if (eventName == "ItemAdd")
				{
					this._markers.Add(nameMarkerListPanel);
					return;
				}
				if (eventName == "ItemRemove")
				{
					this._markers.Remove(nameMarkerListPanel);
				}
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x0001FD2F File Offset: 0x0001DF2F
		// (set) Token: 0x06000B6F RID: 2927 RVA: 0x0001FD37 File Offset: 0x0001DF37
		public bool IsMarkersEnabled
		{
			get
			{
				return this._isMarkersEnabled;
			}
			set
			{
				if (this._isMarkersEnabled != value)
				{
					this._isMarkersEnabled = value;
					base.OnPropertyChanged(value, "IsMarkersEnabled");
				}
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000B70 RID: 2928 RVA: 0x0001FD55 File Offset: 0x0001DF55
		// (set) Token: 0x06000B71 RID: 2929 RVA: 0x0001FD5D File Offset: 0x0001DF5D
		public float TargetAlphaValue
		{
			get
			{
				return this._targetAlphaValue;
			}
			set
			{
				if (this._targetAlphaValue != value)
				{
					this._targetAlphaValue = value;
					base.OnPropertyChanged(value, "TargetAlphaValue");
				}
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x0001FD7B File Offset: 0x0001DF7B
		// (set) Token: 0x06000B73 RID: 2931 RVA: 0x0001FD84 File Offset: 0x0001DF84
		[Editor(false)]
		public Widget MarkersContainer
		{
			get
			{
				return this._markersContainer;
			}
			set
			{
				if (value != this._markersContainer)
				{
					if (this._markersContainer != null)
					{
						this._markersContainer.EventFire += this.OnMarkersChanged;
					}
					this._markersContainer = value;
					if (this._markersContainer != null)
					{
						this._markersContainer.EventFire += this.OnMarkersChanged;
					}
					base.OnPropertyChanged<Widget>(value, "MarkersContainer");
				}
			}
		}

		// Token: 0x04000538 RID: 1336
		private const float MinDistanceToFocusSquared = 3600f;

		// Token: 0x04000539 RID: 1337
		private List<NameMarkerListPanel> _markers;

		// Token: 0x0400053A RID: 1338
		private NameMarkerListPanel _lastFocusedWidget;

		// Token: 0x0400053B RID: 1339
		private bool _isMarkersEnabled;

		// Token: 0x0400053C RID: 1340
		private float _targetAlphaValue;

		// Token: 0x0400053D RID: 1341
		private Widget _markersContainer;
	}
}

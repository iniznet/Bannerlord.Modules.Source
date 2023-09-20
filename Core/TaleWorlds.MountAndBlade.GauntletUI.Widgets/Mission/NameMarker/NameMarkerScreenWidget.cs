using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	public class NameMarkerScreenWidget : Widget
	{
		public NameMarkerScreenWidget(UIContext context)
			: base(context)
		{
			this._markers = new List<NameMarkerListPanel>();
		}

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

		private const float MinDistanceToFocusSquared = 3600f;

		private List<NameMarkerListPanel> _markers;

		private NameMarkerListPanel _lastFocusedWidget;

		private bool _isMarkersEnabled;

		private float _targetAlphaValue;

		private Widget _markersContainer;
	}
}

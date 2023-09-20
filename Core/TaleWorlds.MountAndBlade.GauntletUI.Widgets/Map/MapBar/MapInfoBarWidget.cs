using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x0200010D RID: 269
	public class MapInfoBarWidget : Widget
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000DC1 RID: 3521 RVA: 0x000268B4 File Offset: 0x00024AB4
		// (remove) Token: 0x06000DC2 RID: 3522 RVA: 0x000268EC File Offset: 0x00024AEC
		public event MapInfoBarWidget.MapBarExtendStateChangeEvent OnMapInfoBarExtendStateChange;

		// Token: 0x06000DC3 RID: 3523 RVA: 0x00026921 File Offset: 0x00024B21
		public MapInfoBarWidget(UIContext context)
			: base(context)
		{
			base.AddState("Disabled");
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x00026935 File Offset: 0x00024B35
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshBarExtendState();
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00026944 File Offset: 0x00024B44
		private void OnExtendButtonClick(Widget widget)
		{
			this.IsInfoBarExtended = !this.IsInfoBarExtended;
			this.RefreshBarExtendState();
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0002695C File Offset: 0x00024B5C
		private void RefreshBarExtendState()
		{
			if (this.IsInfoBarExtended && base.CurrentState != "Extended")
			{
				this.SetState("Extended");
				this.RefreshVerticalVisual();
				return;
			}
			if (!this.IsInfoBarExtended && base.CurrentState != "Default")
			{
				this.SetState("Default");
				this.RefreshVerticalVisual();
			}
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x000269C0 File Offset: 0x00024BC0
		private void RefreshVerticalVisual()
		{
			foreach (Style style in this.ExtendButtonWidget.Brush.Styles)
			{
				for (int i = 0; i < style.LayerCount; i++)
				{
					style.GetLayer(i).VerticalFlip = this.IsInfoBarExtended;
				}
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x00026A3C File Offset: 0x00024C3C
		// (set) Token: 0x06000DC9 RID: 3529 RVA: 0x00026A44 File Offset: 0x00024C44
		[Editor(false)]
		public ButtonWidget ExtendButtonWidget
		{
			get
			{
				return this._extendButtonWidget;
			}
			set
			{
				if (this._extendButtonWidget != value)
				{
					this._extendButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ExtendButtonWidget");
					if (!this._extendButtonWidget.ClickEventHandlers.Contains(new Action<Widget>(this.OnExtendButtonClick)))
					{
						this._extendButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnExtendButtonClick));
					}
				}
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x00026AA7 File Offset: 0x00024CA7
		// (set) Token: 0x06000DCB RID: 3531 RVA: 0x00026AAF File Offset: 0x00024CAF
		[Editor(false)]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				if (this._isInfoBarExtended != value)
				{
					this._isInfoBarExtended = value;
					base.OnPropertyChanged(value, "IsInfoBarExtended");
					MapInfoBarWidget.MapBarExtendStateChangeEvent onMapInfoBarExtendStateChange = this.OnMapInfoBarExtendStateChange;
					if (onMapInfoBarExtendStateChange == null)
					{
						return;
					}
					onMapInfoBarExtendStateChange(this.IsInfoBarExtended);
				}
			}
		}

		// Token: 0x04000658 RID: 1624
		private ButtonWidget _extendButtonWidget;

		// Token: 0x04000659 RID: 1625
		private bool _isInfoBarExtended;

		// Token: 0x02000198 RID: 408
		// (Invoke) Token: 0x06001319 RID: 4889
		public delegate void MapBarExtendStateChangeEvent(bool newState);
	}
}

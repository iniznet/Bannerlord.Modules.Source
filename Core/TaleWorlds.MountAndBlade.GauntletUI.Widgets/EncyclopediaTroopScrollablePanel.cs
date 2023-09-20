using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000018 RID: 24
	public class EncyclopediaTroopScrollablePanel : ScrollablePanel
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000506C File Offset: 0x0000326C
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00005074 File Offset: 0x00003274
		public bool PanWithMouseEnabled { get; set; }

		// Token: 0x0600011E RID: 286 RVA: 0x0000507D File Offset: 0x0000327D
		public EncyclopediaTroopScrollablePanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00005088 File Offset: 0x00003288
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.PanWithMouseEnabled)
			{
				bool flag = this.IsMouseOverWidget(this);
				if (flag)
				{
					foreach (Widget widget in base.AllChildrenAndThis)
					{
						if (this.IsMouseOverWidget(widget) && widget is ButtonWidget)
						{
							flag = false;
						}
					}
				}
				if (flag && base.HorizontalScrollbar != null && this._canScrollHorizontal)
				{
					base.SetActiveCursor(UIContext.MouseCursors.Move);
					if (Input.IsKeyPressed(InputKey.LeftMouseButton))
					{
						this._isDragging = true;
					}
				}
			}
			if (Input.IsKeyReleased(InputKey.LeftMouseButton))
			{
				this._isDragging = false;
			}
			if (this._isDragging)
			{
				base.HorizontalScrollbar.ValueFloat -= Input.MouseMoveX;
				base.VerticalScrollbar.ValueFloat -= Input.MouseMoveY;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005174 File Offset: 0x00003374
		private bool IsMouseOverWidget(Widget widget)
		{
			return widget.GlobalPosition.X <= Input.MousePositionPixel.X && Input.MousePositionPixel.X <= widget.GlobalPosition.X + widget.Size.X && widget.GlobalPosition.Y <= Input.MousePositionPixel.Y && Input.MousePositionPixel.Y <= widget.GlobalPosition.Y + widget.Size.Y;
		}

		// Token: 0x04000088 RID: 136
		private bool _isDragging;
	}
}

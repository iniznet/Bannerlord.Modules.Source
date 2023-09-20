using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class EncyclopediaTroopScrollablePanel : ScrollablePanel
	{
		public bool PanWithMouseEnabled { get; set; }

		public EncyclopediaTroopScrollablePanel(UIContext context)
			: base(context)
		{
		}

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

		private bool IsMouseOverWidget(Widget widget)
		{
			return widget.GlobalPosition.X <= Input.MousePositionPixel.X && Input.MousePositionPixel.X <= widget.GlobalPosition.X + widget.Size.X && widget.GlobalPosition.Y <= Input.MousePositionPixel.Y && Input.MousePositionPixel.Y <= widget.GlobalPosition.Y + widget.Size.Y;
		}

		private bool _isDragging;
	}
}

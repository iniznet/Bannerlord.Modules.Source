using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class MouseWidget : Widget
	{
		public MouseWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			if (base.IsVisible)
			{
				this.UpdatePressedKeys();
			}
		}

		public void UpdatePressedKeys()
		{
			Color color = new Color(1f, 0f, 0f, 1f);
			this.LeftMouseButton.Color = Color.White;
			this.RightMouseButton.Color = Color.White;
			this.MiddleMouseButton.Color = Color.White;
			this.MouseX1Button.Color = Color.White;
			this.MouseX2Button.Color = Color.White;
			this.MouseScrollUp.IsVisible = false;
			this.MouseScrollDown.IsVisible = false;
			this.KeyboardKeys.Text = "";
			if (Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				this.LeftMouseButton.Color = color;
			}
			if (Input.IsKeyDown(InputKey.RightMouseButton))
			{
				this.RightMouseButton.Color = color;
			}
			if (Input.IsKeyDown(InputKey.MiddleMouseButton))
			{
				this.MiddleMouseButton.Color = color;
			}
			if (Input.IsKeyDown(InputKey.X1MouseButton))
			{
				this.MouseX1Button.Color = color;
			}
			if (Input.IsKeyDown(InputKey.X2MouseButton))
			{
				this.MouseX2Button.Color = color;
			}
			if (Input.IsKeyDown(InputKey.MouseScrollUp))
			{
				this.MouseScrollUp.IsVisible = true;
			}
			if (Input.IsKeyDown(InputKey.MouseScrollDown))
			{
				this.MouseScrollDown.IsVisible = true;
			}
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "UpdatePressedKeys");
			for (int i = 0; i < 256; i++)
			{
				if (Key.GetInputType((InputKey)i) == Key.InputType.Keyboard && Input.IsKeyDown((InputKey)i))
				{
					InputKey inputKey = (InputKey)i;
					mbstringBuilder.Append<string>(inputKey.ToString());
					mbstringBuilder.Append<string>(", ");
				}
			}
			this.KeyboardKeys.Text = mbstringBuilder.ToStringAndRelease().TrimEnd(MouseWidget._trimChars);
		}

		public Widget LeftMouseButton { get; set; }

		public Widget RightMouseButton { get; set; }

		public Widget MiddleMouseButton { get; set; }

		public Widget MouseX1Button { get; set; }

		public Widget MouseX2Button { get; set; }

		public Widget MouseScrollUp { get; set; }

		public Widget MouseScrollDown { get; set; }

		public TextWidget KeyboardKeys { get; set; }

		private static readonly char[] _trimChars = new char[] { ' ', ',' };
	}
}

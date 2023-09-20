using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000C RID: 12
	public class MouseWidget : Widget
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00005252 File Offset: 0x00003452
		public MouseWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000525B File Offset: 0x0000345B
		protected override void OnUpdate(float dt)
		{
			if (base.IsVisible)
			{
				this.UpdatePressedKeys();
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000526C File Offset: 0x0000346C
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000542A File Offset: 0x0000362A
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00005432 File Offset: 0x00003632
		public Widget LeftMouseButton { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000543B File Offset: 0x0000363B
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00005443 File Offset: 0x00003643
		public Widget RightMouseButton { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000544C File Offset: 0x0000364C
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00005454 File Offset: 0x00003654
		public Widget MiddleMouseButton { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000545D File Offset: 0x0000365D
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00005465 File Offset: 0x00003665
		public Widget MouseX1Button { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000546E File Offset: 0x0000366E
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00005476 File Offset: 0x00003676
		public Widget MouseX2Button { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000547F File Offset: 0x0000367F
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00005487 File Offset: 0x00003687
		public Widget MouseScrollUp { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00005490 File Offset: 0x00003690
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00005498 File Offset: 0x00003698
		public Widget MouseScrollDown { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000054A1 File Offset: 0x000036A1
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x000054A9 File Offset: 0x000036A9
		public TextWidget KeyboardKeys { get; set; }

		// Token: 0x0400004B RID: 75
		private static readonly char[] _trimChars = new char[] { ' ', ',' };
	}
}

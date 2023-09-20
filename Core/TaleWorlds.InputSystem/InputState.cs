using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x0200000F RID: 15
	public class InputState
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600013A RID: 314 RVA: 0x000057B8 File Offset: 0x000039B8
		public Vec2 NativeResolution
		{
			get
			{
				return Input.Resolution;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600013B RID: 315 RVA: 0x000057BF File Offset: 0x000039BF
		// (set) Token: 0x0600013C RID: 316 RVA: 0x000057C8 File Offset: 0x000039C8
		public Vec2 MousePositionRanged
		{
			get
			{
				return this._mousePositionRanged;
			}
			set
			{
				this._mousePositionRanged = value;
				this._mousePositionPixel = new Vec2(this._mousePositionRanged.x * this.NativeResolution.x, this._mousePositionRanged.y * this.NativeResolution.y);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00005815 File Offset: 0x00003A15
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000581D File Offset: 0x00003A1D
		public Vec2 OldMousePositionRanged { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00005826 File Offset: 0x00003A26
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000582E File Offset: 0x00003A2E
		public bool MousePositionChanged { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00005837 File Offset: 0x00003A37
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00005840 File Offset: 0x00003A40
		public Vec2 MousePositionPixel
		{
			get
			{
				return this._mousePositionPixel;
			}
			set
			{
				this._mousePositionPixel = value;
				this._mousePositionRanged = new Vec2(this._mousePositionPixel.x / Input.Resolution.x, this._mousePositionPixel.y / this.NativeResolution.y);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000588C File Offset: 0x00003A8C
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00005894 File Offset: 0x00003A94
		public Vec2 OldMousePositionPixel { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000589D File Offset: 0x00003A9D
		// (set) Token: 0x06000146 RID: 326 RVA: 0x000058A5 File Offset: 0x00003AA5
		public float MouseScrollValue { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000147 RID: 327 RVA: 0x000058AE File Offset: 0x00003AAE
		// (set) Token: 0x06000148 RID: 328 RVA: 0x000058B6 File Offset: 0x00003AB6
		public bool MouseScrollChanged { get; private set; }

		// Token: 0x06000149 RID: 329 RVA: 0x000058C0 File Offset: 0x00003AC0
		public InputState()
		{
			this.MousePositionRanged = default(Vec2);
			this.OldMousePositionRanged = default(Vec2);
			this.MousePositionPixel = default(Vec2);
			this.OldMousePositionPixel = default(Vec2);
			this._mousePositionRanged = new Vec2(0f, 0f);
			this._mousePositionPixel = new Vec2(0f, 0f);
			this._mousePositionPixelDevice = new Vec2(0f, 0f);
			this._mousePositionRangedDevice = new Vec2(0f, 0f);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00005964 File Offset: 0x00003B64
		public bool UpdateMousePosition(float mousePositionX, float mousePositionY)
		{
			this.OldMousePositionRanged = new Vec2(this._mousePositionRangedDevice.x, this._mousePositionRangedDevice.y);
			this._mousePositionRangedDevice = new Vec2(mousePositionX, mousePositionY);
			this.OldMousePositionPixel = new Vec2(this._mousePositionPixelDevice.x, this._mousePositionPixelDevice.y);
			this._mousePositionPixelDevice = new Vec2(this._mousePositionRangedDevice.x * this.NativeResolution.x, this._mousePositionRangedDevice.y * this.NativeResolution.y);
			if (this._mousePositionRangedDevice.x == this.OldMousePositionRanged.x && this._mousePositionRangedDevice.y == this.OldMousePositionRanged.y)
			{
				this.MousePositionChanged = false;
			}
			else
			{
				this.MousePositionChanged = true;
				this.MousePositionPixel = this._mousePositionPixelDevice;
				this.MousePositionRanged = this._mousePositionRangedDevice;
			}
			return this.MousePositionChanged;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005A58 File Offset: 0x00003C58
		public bool UpdateMouseScroll(float mouseScrollValue)
		{
			if (!this.MouseScrollValue.Equals(mouseScrollValue))
			{
				this.MouseScrollValue = mouseScrollValue;
				this.MouseScrollChanged = true;
			}
			else
			{
				this.MouseScrollChanged = false;
			}
			return this.MouseScrollChanged;
		}

		// Token: 0x0400013F RID: 319
		private Vec2 _mousePositionRanged;

		// Token: 0x04000141 RID: 321
		private Vec2 _mousePositionRangedDevice;

		// Token: 0x04000143 RID: 323
		private Vec2 _mousePositionPixel;

		// Token: 0x04000144 RID: 324
		private Vec2 _mousePositionPixelDevice;
	}
}

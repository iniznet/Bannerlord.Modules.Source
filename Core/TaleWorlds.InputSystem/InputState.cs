using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public class InputState
	{
		public Vec2 NativeResolution
		{
			get
			{
				return Input.Resolution;
			}
		}

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

		public Vec2 OldMousePositionRanged { get; private set; }

		public bool MousePositionChanged { get; private set; }

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

		public Vec2 OldMousePositionPixel { get; private set; }

		public float MouseScrollValue { get; private set; }

		public bool MouseScrollChanged { get; private set; }

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

		private Vec2 _mousePositionRanged;

		private Vec2 _mousePositionRangedDevice;

		private Vec2 _mousePositionPixel;

		private Vec2 _mousePositionPixelDevice;
	}
}

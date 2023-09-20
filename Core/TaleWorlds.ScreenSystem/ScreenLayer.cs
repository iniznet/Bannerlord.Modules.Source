using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	public abstract class ScreenLayer : IComparable
	{
		public float Scale
		{
			get
			{
				return ScreenManager.Scale;
			}
		}

		public Vec2 UsableArea
		{
			get
			{
				return ScreenManager.UsableArea;
			}
		}

		public InputContext Input { get; private set; }

		public InputRestrictions InputRestrictions { get; private set; }

		public string Name { get; set; }

		public bool LastActiveState { get; set; }

		public bool Finalized { get; private set; }

		public bool IsActive { get; private set; }

		public bool MouseEnabled { get; protected internal set; }

		public bool KeyboardEnabled { get; protected internal set; }

		public bool GamepadEnabled { get; protected internal set; }

		public bool IsHitThisFrame { get; internal set; }

		public bool IsFocusLayer { get; set; }

		public CursorType ActiveCursor { get; set; }

		protected InputType _usedInputs { get; set; }

		protected bool? _isMousePressedByThisLayer { get; set; }

		public int ScreenOrderInLastFrame { get; internal set; }

		protected ScreenLayer(int localOrder, string categoryId)
		{
			this.InputRestrictions = new InputRestrictions(localOrder);
			this.Input = new InputContext();
			this._categoryId = categoryId;
			this.Name = "ScreenLayer";
			this.LastActiveState = true;
			this.Finalized = false;
			this.IsActive = false;
			this.IsFocusLayer = false;
			this._usedInputs = InputType.None;
			this._isMousePressedByThisLayer = null;
			this.ActiveCursor = CursorType.Default;
		}

		protected internal virtual void Tick(float dt)
		{
		}

		protected internal virtual void LateTick(float dt)
		{
		}

		protected internal virtual void OnLateUpdate(float dt)
		{
		}

		protected internal virtual void Update(IReadOnlyList<int> lastKeysPressed)
		{
		}

		internal void HandleFinalize()
		{
			this.OnFinalize();
			this.Finalized = true;
		}

		protected virtual void OnActivate()
		{
			this.Finalized = false;
		}

		protected virtual void OnDeactivate()
		{
		}

		protected internal virtual void OnLoseFocus()
		{
		}

		internal void HandleActivate()
		{
			this.IsActive = true;
			this.OnActivate();
		}

		internal void HandleDeactivate()
		{
			this.OnDeactivate();
			this.IsActive = false;
			ScreenManager.TryLoseFocus(this);
		}

		protected virtual void OnFinalize()
		{
		}

		protected internal virtual void RefreshGlobalOrder(ref int currentOrder)
		{
		}

		public virtual void DrawDebugInfo()
		{
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Order: {0}", this.InputRestrictions.Order));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Keys Allowed: {0}", this.Input.IsKeysAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Controller Allowed: {0}", this.Input.IsControllerAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Mouse Button Allowed: {0}", this.Input.IsMouseButtonAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Mouse Wheel Allowed: {0}", this.Input.IsMouseWheelAllowed));
		}

		public virtual void EarlyProcessEvents(InputType handledInputs, bool? isMousePressed)
		{
			this._usedInputs = handledInputs;
			this._isMousePressedByThisLayer = isMousePressed;
			bool? flag = isMousePressed;
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				this.Input.MouseOnMe = true;
			}
			if (this.Input.MouseOnMe)
			{
				this._usedInputs |= InputType.MouseButton;
			}
		}

		public virtual void ProcessEvents()
		{
			this.Input.IsKeysAllowed = this._usedInputs.HasAnyFlag(InputType.Key);
			this.Input.IsMouseButtonAllowed = this._usedInputs.HasAnyFlag(InputType.MouseButton);
			this.Input.IsMouseWheelAllowed = this._usedInputs.HasAnyFlag(InputType.MouseWheel);
		}

		public virtual void LateProcessEvents()
		{
			bool? isMousePressedByThisLayer = this._isMousePressedByThisLayer;
			bool flag = false;
			if ((isMousePressedByThisLayer.GetValueOrDefault() == flag) & (isMousePressedByThisLayer != null))
			{
				this.Input.MouseOnMe = false;
			}
		}

		public virtual bool HitTest(Vector2 position)
		{
			return false;
		}

		public virtual bool HitTest()
		{
			return false;
		}

		public virtual bool FocusTest()
		{
			return false;
		}

		public InputUsageMask InputUsageMask
		{
			get
			{
				return this.InputRestrictions.InputUsageMask;
			}
		}

		public virtual bool IsFocusedOnInput()
		{
			return false;
		}

		public virtual void OnOnScreenKeyboardDone(string inputText)
		{
		}

		public virtual void OnOnScreenKeyboardCanceled()
		{
		}

		public int CompareTo(object obj)
		{
			ScreenLayer screenLayer = obj as ScreenLayer;
			if (screenLayer == null)
			{
				return 1;
			}
			if (screenLayer == this)
			{
				return 0;
			}
			if (this.InputRestrictions.Order == screenLayer.InputRestrictions.Order)
			{
				return this.InputRestrictions.Id.CompareTo(screenLayer.InputRestrictions.Id);
			}
			return this.InputRestrictions.Order.CompareTo(screenLayer.InputRestrictions.Order);
		}

		public virtual void UpdateLayout()
		{
		}

		public readonly string _categoryId;
	}
}

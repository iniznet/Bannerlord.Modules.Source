using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	// Token: 0x02000008 RID: 8
	public abstract class ScreenLayer : IComparable
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00002BEC File Offset: 0x00000DEC
		public float Scale
		{
			get
			{
				return ScreenManager.Scale;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002BF3 File Offset: 0x00000DF3
		public Vec2 UsableArea
		{
			get
			{
				return ScreenManager.UsableArea;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002BFA File Offset: 0x00000DFA
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002C02 File Offset: 0x00000E02
		public InputContext Input { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002C0B File Offset: 0x00000E0B
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002C13 File Offset: 0x00000E13
		public InputRestrictions InputRestrictions { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002C1C File Offset: 0x00000E1C
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002C24 File Offset: 0x00000E24
		public string Name { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002C2D File Offset: 0x00000E2D
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002C35 File Offset: 0x00000E35
		public bool LastActiveState { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002C3E File Offset: 0x00000E3E
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002C46 File Offset: 0x00000E46
		public bool Finalized { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002C4F File Offset: 0x00000E4F
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00002C57 File Offset: 0x00000E57
		public bool IsActive { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002C60 File Offset: 0x00000E60
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002C68 File Offset: 0x00000E68
		public bool MouseEnabled { get; protected internal set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002C71 File Offset: 0x00000E71
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00002C79 File Offset: 0x00000E79
		public bool KeyboardEnabled { get; protected internal set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00002C82 File Offset: 0x00000E82
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00002C8A File Offset: 0x00000E8A
		public bool GamepadEnabled { get; protected internal set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00002C93 File Offset: 0x00000E93
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00002C9B File Offset: 0x00000E9B
		public bool IsHitThisFrame { get; internal set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00002CA4 File Offset: 0x00000EA4
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002CAC File Offset: 0x00000EAC
		public bool IsFocusLayer { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00002CB5 File Offset: 0x00000EB5
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002CBD File Offset: 0x00000EBD
		public CursorType ActiveCursor { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00002CC6 File Offset: 0x00000EC6
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00002CCE File Offset: 0x00000ECE
		protected InputType _usedInputs { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002CD7 File Offset: 0x00000ED7
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002CDF File Offset: 0x00000EDF
		protected bool? _isMousePressedByThisLayer { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00002CE8 File Offset: 0x00000EE8
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00002CF0 File Offset: 0x00000EF0
		public int ScreenOrderInLastFrame { get; internal set; }

		// Token: 0x06000078 RID: 120 RVA: 0x00002CFC File Offset: 0x00000EFC
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

		// Token: 0x06000079 RID: 121 RVA: 0x00002D71 File Offset: 0x00000F71
		protected internal virtual void Tick(float dt)
		{
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00002D73 File Offset: 0x00000F73
		protected internal virtual void LateTick(float dt)
		{
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002D75 File Offset: 0x00000F75
		protected internal virtual void OnLateUpdate(float dt)
		{
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00002D77 File Offset: 0x00000F77
		protected internal virtual void Update(IReadOnlyList<int> lastKeysPressed)
		{
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002D79 File Offset: 0x00000F79
		internal void HandleFinalize()
		{
			this.OnFinalize();
			this.Finalized = true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002D88 File Offset: 0x00000F88
		protected virtual void OnActivate()
		{
			this.Finalized = false;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002D91 File Offset: 0x00000F91
		protected virtual void OnDeactivate()
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002D93 File Offset: 0x00000F93
		protected internal virtual void OnLoseFocus()
		{
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002D95 File Offset: 0x00000F95
		internal void HandleActivate()
		{
			this.IsActive = true;
			this.OnActivate();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002DA4 File Offset: 0x00000FA4
		internal void HandleDeactivate()
		{
			this.OnDeactivate();
			this.IsActive = false;
			ScreenManager.TryLoseFocus(this);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002DB9 File Offset: 0x00000FB9
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002DBB File Offset: 0x00000FBB
		protected internal virtual void RefreshGlobalOrder(ref int currentOrder)
		{
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002DC0 File Offset: 0x00000FC0
		public virtual void DrawDebugInfo()
		{
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Order: {0}", this.InputRestrictions.Order));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Keys Allowed: {0}", this.Input.IsKeysAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Controller Allowed: {0}", this.Input.IsControllerAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Mouse Button Allowed: {0}", this.Input.IsMouseButtonAllowed));
			ScreenManager.EngineInterface.DrawDebugText(string.Format("Mouse Wheel Allowed: {0}", this.Input.IsMouseWheelAllowed));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002E84 File Offset: 0x00001084
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

		// Token: 0x06000087 RID: 135 RVA: 0x00002EE0 File Offset: 0x000010E0
		public virtual void ProcessEvents()
		{
			this.Input.IsKeysAllowed = this._usedInputs.HasAnyFlag(InputType.Key);
			this.Input.IsMouseButtonAllowed = this._usedInputs.HasAnyFlag(InputType.MouseButton);
			this.Input.IsMouseWheelAllowed = this._usedInputs.HasAnyFlag(InputType.MouseWheel);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00002F34 File Offset: 0x00001134
		public virtual void LateProcessEvents()
		{
			bool? isMousePressedByThisLayer = this._isMousePressedByThisLayer;
			bool flag = false;
			if ((isMousePressedByThisLayer.GetValueOrDefault() == flag) & (isMousePressedByThisLayer != null))
			{
				this.Input.MouseOnMe = false;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00002F6A File Offset: 0x0000116A
		public virtual bool HitTest(Vector2 position)
		{
			return false;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00002F6D File Offset: 0x0000116D
		public virtual bool HitTest()
		{
			return false;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002F70 File Offset: 0x00001170
		public virtual bool FocusTest()
		{
			return false;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00002F73 File Offset: 0x00001173
		public InputUsageMask InputUsageMask
		{
			get
			{
				return this.InputRestrictions.InputUsageMask;
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00002F80 File Offset: 0x00001180
		public virtual bool IsFocusedOnInput()
		{
			return false;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00002F83 File Offset: 0x00001183
		public virtual void OnOnScreenKeyboardDone(string inputText)
		{
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002F85 File Offset: 0x00001185
		public virtual void OnOnScreenKeyboardCanceled()
		{
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002F88 File Offset: 0x00001188
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

		// Token: 0x06000091 RID: 145 RVA: 0x00002FFC File Offset: 0x000011FC
		public virtual void UpdateLayout()
		{
		}

		// Token: 0x0400002C RID: 44
		public readonly string _categoryId;
	}
}

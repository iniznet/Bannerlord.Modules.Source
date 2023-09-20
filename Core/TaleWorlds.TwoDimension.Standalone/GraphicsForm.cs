using System;
using System.Drawing;
using System.Numerics;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000004 RID: 4
	public class GraphicsForm : IMessageCommunicator
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002C31 File Offset: 0x00000E31
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002C39 File Offset: 0x00000E39
		public GraphicsContext GraphicsContext { get; private set; }

		// Token: 0x06000026 RID: 38 RVA: 0x00002C44 File Offset: 0x00000E44
		public GraphicsForm(int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, bool layeredWindow = false, string name = null)
		{
			Rectangle rectangle;
			User32.GetClientRect(User32.GetDesktopWindow(), out rectangle);
			int num = (rectangle.Width - width) / 2;
			int num2 = (rectangle.Height - height) / 2;
			this._windowsForm = new WindowsForm(num, num2, width, height, resourceDepot, borderlessWindow, enableWindowBlur, name);
			this.Initalize(layeredWindow);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CAC File Offset: 0x00000EAC
		public GraphicsForm(int x, int y, int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, bool layeredWindow = false, string name = null)
		{
			this._windowsForm = new WindowsForm(x, y, width, height, resourceDepot, borderlessWindow, enableWindowBlur, name);
			this.Initalize(layeredWindow);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002CF1 File Offset: 0x00000EF1
		public GraphicsForm(WindowsForm windowsForm)
		{
			this._windowsForm = windowsForm;
			this.Initalize(false);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002D1C File Offset: 0x00000F1C
		private void Initalize(bool layeredWindow)
		{
			this._currentInputData = new InputData();
			this._oldInputData = new InputData();
			this._messageLoopInputData = new InputData();
			this._windowsForm.AddMessageHandler(new WindowsFormMessageHandler(this.MessageHandler));
			this._windowsForm.Show();
			this.GraphicsContext = new GraphicsContext();
			if (layeredWindow)
			{
				this._layeredWindowController = new LayeredWindowController(this._windowsForm.Handle, this._windowsForm.Width, this._windowsForm.Height);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002DA6 File Offset: 0x00000FA6
		public void Destroy()
		{
			this.GraphicsContext.DestroyContext();
			this._windowsForm.Destroy();
			LayeredWindowController layeredWindowController = this._layeredWindowController;
			if (layeredWindowController == null)
			{
				return;
			}
			layeredWindowController.OnFinalize();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002DCE File Offset: 0x00000FCE
		public void MinimizeWindow()
		{
			User32.ShowWindow(this._windowsForm.Handle, WindowShowStyle.Minimize);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public void InitializeGraphicsContext(ResourceDepot resourceDepot)
		{
			this.GraphicsContext.Control = this._windowsForm;
			this.GraphicsContext.CreateContext(resourceDepot);
			this.GraphicsContext.ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, (float)this._windowsForm.Width, (float)this._windowsForm.Height, 0f, 0f, 1f);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002E4C File Offset: 0x0000104C
		public void BeginFrame()
		{
			if (this.GraphicsContext != null)
			{
				this.GraphicsContext.BeginFrame(this._windowsForm.Width, this._windowsForm.Height);
				this.GraphicsContext.Resize(this._windowsForm.Width, this._windowsForm.Height);
			}
			this.GraphicsContext.ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, (float)this._windowsForm.Width, (float)this._windowsForm.Height, 0f, 0f, 1f);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002EE0 File Offset: 0x000010E0
		public void Update()
		{
			if (!this._isDragging && this._mouseOverDragArea && this._currentInputData.LeftMouse && !this._oldInputData.LeftMouse)
			{
				this._isDragging = true;
				this.MessageHandler(WindowMessage.LeftButtonUp, 0L, 0L);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002F30 File Offset: 0x00001130
		public void MessageLoop()
		{
			if (this._isDragging)
			{
				User32.ReleaseCapture();
				User32.SendMessage(this._windowsForm.Handle, 161U, new IntPtr(2), IntPtr.Zero);
				this._isDragging = false;
				User32.SetCapture(this._windowsForm.Handle);
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002F84 File Offset: 0x00001184
		public void UpdateInput(bool mouseOverDragArea = false)
		{
			this._mouseOverDragArea = mouseOverDragArea;
			InputData oldInputData = this._oldInputData;
			this._oldInputData = this._currentInputData;
			this._currentInputData = oldInputData;
			object inputDataLocker = this._inputDataLocker;
			lock (inputDataLocker)
			{
				this._currentInputData.FillFrom(this._messageLoopInputData);
				this._messageLoopInputData.Reset();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002FFC File Offset: 0x000011FC
		public void PostRender()
		{
			if (this._layeredWindowController != null)
			{
				this._layeredWindowController.PostRender();
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003014 File Offset: 0x00001214
		public bool GetKeyDown(InputKey keyCode)
		{
			if (keyCode == InputKey.LeftMouseButton)
			{
				return this.LeftMouseDown();
			}
			if (keyCode == InputKey.RightMouseButton)
			{
				return this.RightMouseDown();
			}
			return this._currentInputData.KeyData[(int)keyCode] && !this._oldInputData.KeyData[(int)keyCode];
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003060 File Offset: 0x00001260
		public bool GetKey(InputKey keyCode)
		{
			if (keyCode == InputKey.LeftMouseButton)
			{
				return this.LeftMouse();
			}
			if (keyCode == InputKey.RightMouseButton)
			{
				return this.RightMouse();
			}
			return this._currentInputData.KeyData[(int)keyCode];
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000308D File Offset: 0x0000128D
		public bool GetKeyUp(InputKey keyCode)
		{
			if (keyCode == InputKey.LeftMouseButton)
			{
				return this.LeftMouseUp();
			}
			if (keyCode == InputKey.RightMouseButton)
			{
				return this.RightMouseUp();
			}
			return !this._currentInputData.KeyData[(int)keyCode] && this._oldInputData.KeyData[(int)keyCode];
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000030CB File Offset: 0x000012CB
		public float GetMouseDeltaZ()
		{
			return this._currentInputData.MouseScrollDelta;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000030D8 File Offset: 0x000012D8
		public bool LeftMouse()
		{
			return this._currentInputData.LeftMouse;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000030E5 File Offset: 0x000012E5
		public bool LeftMouseDown()
		{
			return this._currentInputData.LeftMouse && !this._oldInputData.LeftMouse;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003104 File Offset: 0x00001304
		public bool LeftMouseUp()
		{
			return !this._currentInputData.LeftMouse && this._oldInputData.LeftMouse;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003120 File Offset: 0x00001320
		public bool RightMouse()
		{
			return this._currentInputData.RightMouse;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000312D File Offset: 0x0000132D
		public bool RightMouseDown()
		{
			return this._currentInputData.RightMouse && !this._oldInputData.RightMouse;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000314C File Offset: 0x0000134C
		public bool RightMouseUp()
		{
			return !this._currentInputData.RightMouse && this._oldInputData.RightMouse;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003168 File Offset: 0x00001368
		public Vector2 MousePosition()
		{
			return new Vector2((float)this._currentInputData.CursorX, (float)this._currentInputData.CursorY);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003187 File Offset: 0x00001387
		public bool MouseMove()
		{
			return this._currentInputData.MouseMove;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003194 File Offset: 0x00001394
		public void FillInputDataFromCurrent(InputData inputData)
		{
			inputData.FillFrom(this._currentInputData);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000031A4 File Offset: 0x000013A4
		private void MessageHandler(WindowMessage message, long wParam, long lParam)
		{
			object obj;
			if (message <= WindowMessage.Close)
			{
				switch (message)
				{
				case WindowMessage.Size:
				case (WindowMessage)6U:
					return;
				case WindowMessage.SetFocus:
					goto IL_324;
				case WindowMessage.KillFocus:
					break;
				default:
					if (message != WindowMessage.Close)
					{
						return;
					}
					this.Destroy();
					Environment.Exit(0);
					return;
				}
			}
			else
			{
				checked
				{
					if (message != WindowMessage.KeyDown)
					{
						if (message != WindowMessage.KeyUp)
						{
							switch (message)
							{
							case WindowMessage.MouseMove:
								goto IL_23F;
							case WindowMessage.LeftButtonDown:
								goto IL_1E4;
							case WindowMessage.LeftButtonUp:
								goto IL_189;
							case (WindowMessage)515U:
							case (WindowMessage)518U:
							case (WindowMessage)519U:
							case (WindowMessage)520U:
							case (WindowMessage)521U:
								return;
							case WindowMessage.RightButtonDown:
								goto IL_12E;
							case WindowMessage.RightButtonUp:
								goto IL_D7;
							case WindowMessage.MouseWheel:
								goto IL_29A;
							default:
								return;
							}
						}
					}
					else
					{
						obj = this._inputDataLocker;
						lock (obj)
						{
							this._messageLoopInputData.KeyData[(int)((IntPtr)wParam)] = true;
							return;
						}
					}
					obj = this._inputDataLocker;
					lock (obj)
					{
						this._messageLoopInputData.KeyData[(int)((IntPtr)wParam)] = false;
						return;
					}
				}
				IL_D7:
				obj = this._inputDataLocker;
				lock (obj)
				{
					this._messageLoopInputData.RightMouse = false;
					int num = (int)lParam % 65536;
					int num2 = (int)(lParam / 65536L);
					this._messageLoopInputData.CursorX = num;
					this._messageLoopInputData.CursorY = num2;
					return;
				}
				IL_12E:
				obj = this._inputDataLocker;
				lock (obj)
				{
					this._messageLoopInputData.RightMouse = true;
					int num3 = (int)lParam % 65536;
					int num4 = (int)(lParam / 65536L);
					this._messageLoopInputData.CursorX = num3;
					this._messageLoopInputData.CursorY = num4;
					return;
				}
				IL_189:
				obj = this._inputDataLocker;
				lock (obj)
				{
					this._messageLoopInputData.LeftMouse = false;
					int num5 = (int)lParam % 65536;
					int num6 = (int)(lParam / 65536L);
					this._messageLoopInputData.CursorX = num5;
					this._messageLoopInputData.CursorY = num6;
					return;
				}
				IL_1E4:
				obj = this._inputDataLocker;
				lock (obj)
				{
					this._messageLoopInputData.LeftMouse = true;
					int num7 = (int)lParam % 65536;
					int num8 = (int)(lParam / 65536L);
					this._messageLoopInputData.CursorX = num7;
					this._messageLoopInputData.CursorY = num8;
					return;
				}
				IL_23F:
				obj = this._inputDataLocker;
				lock (obj)
				{
					this._messageLoopInputData.MouseMove = true;
					int num9 = (int)lParam % 65536;
					int num10 = (int)(lParam / 65536L);
					this._messageLoopInputData.CursorX = num9;
					this._messageLoopInputData.CursorY = num10;
					return;
				}
				IL_29A:
				obj = this._inputDataLocker;
				lock (obj)
				{
					short num11 = (short)(wParam >> 16);
					this._messageLoopInputData.MouseScrollDelta = (float)num11;
					return;
				}
			}
			obj = this._inputDataLocker;
			lock (obj)
			{
				for (int i = 0; i < 256; i++)
				{
					this._messageLoopInputData.KeyData[i] = false;
					this._messageLoopInputData.RightMouse = false;
					this._messageLoopInputData.LeftMouse = false;
				}
				return;
			}
			IL_324:
			obj = this._inputDataLocker;
			lock (obj)
			{
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00003570 File Offset: 0x00001770
		public int Width
		{
			get
			{
				return this._windowsForm.Width;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000357D File Offset: 0x0000177D
		public int Height
		{
			get
			{
				return this._windowsForm.Height;
			}
		}

		// Token: 0x04000017 RID: 23
		public const int WM_NCLBUTTONDOWN = 161;

		// Token: 0x04000018 RID: 24
		public const int HT_CAPTION = 2;

		// Token: 0x04000019 RID: 25
		private WindowsForm _windowsForm;

		// Token: 0x0400001B RID: 27
		private InputData _currentInputData;

		// Token: 0x0400001C RID: 28
		private InputData _oldInputData;

		// Token: 0x0400001D RID: 29
		private InputData _messageLoopInputData;

		// Token: 0x0400001E RID: 30
		private object _inputDataLocker = new object();

		// Token: 0x0400001F RID: 31
		private bool _mouseOverDragArea = true;

		// Token: 0x04000020 RID: 32
		private bool _isDragging;

		// Token: 0x04000021 RID: 33
		private LayeredWindowController _layeredWindowController;
	}
}

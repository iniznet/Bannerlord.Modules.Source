using System;
using System.Drawing;
using System.Numerics;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class GraphicsForm : IMessageCommunicator
	{
		public GraphicsContext GraphicsContext { get; private set; }

		public GraphicsForm(int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, bool layeredWindow = false, string name = null)
		{
			Rectangle rectangle;
			User32.GetClientRect(User32.GetDesktopWindow(), out rectangle);
			int num = (rectangle.Width - width) / 2;
			int num2 = (rectangle.Height - height) / 2;
			this._windowsForm = new WindowsForm(num, num2, width, height, resourceDepot, borderlessWindow, enableWindowBlur, name);
			this.Initalize(layeredWindow);
		}

		public GraphicsForm(int x, int y, int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, bool layeredWindow = false, string name = null)
		{
			this._windowsForm = new WindowsForm(x, y, width, height, resourceDepot, borderlessWindow, enableWindowBlur, name);
			this.Initalize(layeredWindow);
		}

		public GraphicsForm(WindowsForm windowsForm)
		{
			this._windowsForm = windowsForm;
			this.Initalize(false);
		}

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

		public void MinimizeWindow()
		{
			User32.ShowWindow(this._windowsForm.Handle, WindowShowStyle.Minimize);
		}

		public void InitializeGraphicsContext(ResourceDepot resourceDepot)
		{
			this.GraphicsContext.Control = this._windowsForm;
			this.GraphicsContext.CreateContext(resourceDepot);
			this.GraphicsContext.ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, (float)this._windowsForm.Width, (float)this._windowsForm.Height, 0f, 0f, 1f);
		}

		public void BeginFrame()
		{
			if (this.GraphicsContext != null)
			{
				this.GraphicsContext.BeginFrame(this._windowsForm.Width, this._windowsForm.Height);
				this.GraphicsContext.Resize(this._windowsForm.Width, this._windowsForm.Height);
			}
			this.GraphicsContext.ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, (float)this._windowsForm.Width, (float)this._windowsForm.Height, 0f, 0f, 1f);
		}

		public void Update()
		{
			if (!this._isDragging && this._mouseOverDragArea && this._currentInputData.LeftMouse && !this._oldInputData.LeftMouse)
			{
				this._isDragging = true;
				this.MessageHandler(WindowMessage.LeftButtonUp, 0L, 0L);
			}
		}

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

		public void PostRender()
		{
			if (this._layeredWindowController != null)
			{
				this._layeredWindowController.PostRender();
			}
		}

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

		public float GetMouseDeltaZ()
		{
			return this._currentInputData.MouseScrollDelta;
		}

		public bool LeftMouse()
		{
			return this._currentInputData.LeftMouse;
		}

		public bool LeftMouseDown()
		{
			return this._currentInputData.LeftMouse && !this._oldInputData.LeftMouse;
		}

		public bool LeftMouseUp()
		{
			return !this._currentInputData.LeftMouse && this._oldInputData.LeftMouse;
		}

		public bool RightMouse()
		{
			return this._currentInputData.RightMouse;
		}

		public bool RightMouseDown()
		{
			return this._currentInputData.RightMouse && !this._oldInputData.RightMouse;
		}

		public bool RightMouseUp()
		{
			return !this._currentInputData.RightMouse && this._oldInputData.RightMouse;
		}

		public Vector2 MousePosition()
		{
			return new Vector2((float)this._currentInputData.CursorX, (float)this._currentInputData.CursorY);
		}

		public bool MouseMove()
		{
			return this._currentInputData.MouseMove;
		}

		public void FillInputDataFromCurrent(InputData inputData)
		{
			inputData.FillFrom(this._currentInputData);
		}

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

		public int Width
		{
			get
			{
				return this._windowsForm.Width;
			}
		}

		public int Height
		{
			get
			{
				return this._windowsForm.Height;
			}
		}

		public const int WM_NCLBUTTONDOWN = 161;

		public const int HT_CAPTION = 2;

		private WindowsForm _windowsForm;

		private InputData _currentInputData;

		private InputData _oldInputData;

		private InputData _messageLoopInputData;

		private object _inputDataLocker = new object();

		private bool _mouseOverDragArea = true;

		private bool _isDragging;

		private LayeredWindowController _layeredWindowController;
	}
}

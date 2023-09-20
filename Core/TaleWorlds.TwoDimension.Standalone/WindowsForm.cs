using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class WindowsForm
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public string Text { get; set; }

		public IntPtr Handle { get; set; }

		public WindowsForm(int x, int y, int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, string name = null)
			: this(x, y, width, height, resourceDepot, IntPtr.Zero, borderlessWindow, enableWindowBlur, name)
		{
		}

		public WindowsForm(int x, int y, int width, int height, ResourceDepot resourceDepot, IntPtr parent, bool borderlessWindow = false, bool enableWindowBlur = false, string name = null)
		{
			this.Handle = IntPtr.Zero;
			WindowsForm.classNameCount++;
			this.Width = width;
			this.Height = height;
			this.Text = "Form";
			this.windowClassName = "Form" + WindowsForm.classNameCount;
			this.wc = default(WindowClass);
			this._windowProcedure = new WndProc(this.WndProc);
			this.wc.style = 0U;
			this.wc.lpfnWndProc = this._windowProcedure;
			this.wc.cbClsExtra = 0;
			this.wc.cbWndExtra = 0;
			this.wc.hCursor = User32.LoadCursorFromFile(resourceDepot.GetFilePath("mb_cursor.cur"));
			this.wc.hInstance = Kernel32.GetModuleHandle(null);
			this.wc.lpszMenuName = null;
			this.wc.lpszClassName = this.windowClassName;
			this.wc.hbrBackground = Gdi32.CreateSolidBrush(IntPtr.Zero);
			User32.RegisterClass(ref this.wc);
			if (string.IsNullOrEmpty(name))
			{
				name = "Gauntlet UI: " + Process.GetCurrentProcess().Id;
			}
			WindowStyle windowStyle;
			if (parent != IntPtr.Zero)
			{
				windowStyle = WindowStyle.WS_CHILD | WindowStyle.WS_VISIBLE;
			}
			else if (!borderlessWindow)
			{
				windowStyle = WindowStyle.OverlappedWindow;
			}
			else
			{
				windowStyle = (WindowStyle)2416443392U;
			}
			this.Handle = User32.CreateWindowEx(0, this.windowClassName, name, windowStyle, x, y, width, height, parent, IntPtr.Zero, Kernel32.GetModuleHandle(null), IntPtr.Zero);
			if (enableWindowBlur)
			{
				DwmBlurBehind dwmBlurBehind = default(DwmBlurBehind);
				dwmBlurBehind.dwFlags = BlurBehindConstraints.Enable | BlurBehindConstraints.BlurRegion;
				dwmBlurBehind.hRgnBlur = Gdi32.CreateRectRgn(0, 0, -1, -1);
				dwmBlurBehind.fEnable = true;
				Dwmapi.DwmEnableBlurBehindWindow(this.Handle, ref dwmBlurBehind);
			}
		}

		public WindowsForm(int width, int height, ResourceDepot resourceDepot)
			: this(100, 100, width, height, resourceDepot, false, false, null)
		{
		}

		public void SetParent(IntPtr parentHandle)
		{
			User32.SetParent(this.Handle, parentHandle);
		}

		public void Show()
		{
			User32.ShowWindow(this.Handle, WindowShowStyle.Show);
		}

		public void Hide()
		{
			User32.ShowWindow(this.Handle, WindowShowStyle.Hide);
		}

		public void Destroy()
		{
			this.Hide();
			User32.DestroyWindow(this.Handle);
			User32.UnregisterClass(this.windowClassName, IntPtr.Zero);
		}

		public void AddMessageHandler(WindowsFormMessageHandler messageHandler)
		{
			this._messageHandlers.Add(messageHandler);
		}

		private IntPtr WndProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
		{
			long num = wParam.ToInt64();
			long num2 = lParam.ToInt64();
			if (message == 5U)
			{
				int num3 = (int)num2 % 65536;
				int num4 = (int)(num2 / 65536L);
				this.Width = num3;
				this.Height = num4;
			}
			foreach (WindowsFormMessageHandler windowsFormMessageHandler in this._messageHandlers)
			{
				windowsFormMessageHandler((WindowMessage)message, num, num2);
			}
			return User32.DefWindowProc(hWnd, message, wParam, lParam);
		}

		private static int classNameCount;

		private WindowClass wc;

		private string windowClassName;

		private WndProc _windowProcedure;

		private List<WindowsFormMessageHandler> _messageHandlers = new List<WindowsFormMessageHandler>();
	}
}

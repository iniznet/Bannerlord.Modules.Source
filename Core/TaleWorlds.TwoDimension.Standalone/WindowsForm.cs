using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x0200000D RID: 13
	public class WindowsForm
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00004650 File Offset: 0x00002850
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00004658 File Offset: 0x00002858
		public int Width { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004661 File Offset: 0x00002861
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00004669 File Offset: 0x00002869
		public int Height { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004672 File Offset: 0x00002872
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x0000467A File Offset: 0x0000287A
		public string Text { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004683 File Offset: 0x00002883
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x0000468B File Offset: 0x0000288B
		public IntPtr Handle { get; set; }

		// Token: 0x060000C6 RID: 198 RVA: 0x00004694 File Offset: 0x00002894
		public WindowsForm(int x, int y, int width, int height, ResourceDepot resourceDepot, bool borderlessWindow = false, bool enableWindowBlur = false, string name = null)
			: this(x, y, width, height, resourceDepot, IntPtr.Zero, borderlessWindow, enableWindowBlur, name)
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000046BC File Offset: 0x000028BC
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

		// Token: 0x060000C8 RID: 200 RVA: 0x00004898 File Offset: 0x00002A98
		public WindowsForm(int width, int height, ResourceDepot resourceDepot)
			: this(100, 100, width, height, resourceDepot, false, false, null)
		{
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000048B5 File Offset: 0x00002AB5
		public void SetParent(IntPtr parentHandle)
		{
			User32.SetParent(this.Handle, parentHandle);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000048C4 File Offset: 0x00002AC4
		public void Show()
		{
			User32.ShowWindow(this.Handle, WindowShowStyle.Show);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000048D3 File Offset: 0x00002AD3
		public void Hide()
		{
			User32.ShowWindow(this.Handle, WindowShowStyle.Hide);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000048E2 File Offset: 0x00002AE2
		public void Destroy()
		{
			this.Hide();
			User32.DestroyWindow(this.Handle);
			User32.UnregisterClass(this.windowClassName, IntPtr.Zero);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004907 File Offset: 0x00002B07
		public void AddMessageHandler(WindowsFormMessageHandler messageHandler)
		{
			this._messageHandlers.Add(messageHandler);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004918 File Offset: 0x00002B18
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

		// Token: 0x04000044 RID: 68
		private static int classNameCount;

		// Token: 0x04000045 RID: 69
		private WindowClass wc;

		// Token: 0x04000046 RID: 70
		private string windowClassName;

		// Token: 0x04000047 RID: 71
		private WndProc _windowProcedure;

		// Token: 0x0400004B RID: 75
		private List<WindowsFormMessageHandler> _messageHandlers = new List<WindowsFormMessageHandler>();
	}
}

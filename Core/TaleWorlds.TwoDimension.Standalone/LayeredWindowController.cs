using System;
using System.Drawing;
using System.Runtime.InteropServices;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000007 RID: 7
	public class LayeredWindowController
	{
		// Token: 0x06000054 RID: 84 RVA: 0x000036F4 File Offset: 0x000018F4
		public LayeredWindowController(IntPtr windowHandle, int width, int height)
		{
			this._windowHandle = windowHandle;
			User32.SetWindowLong(this._windowHandle, -20, 524288U);
			this._screenDC = User32.GetDC(IntPtr.Zero);
			this._memoryDC = Gdi32.CreateCompatibleDC(this._screenDC);
			this.SetSize(width, height);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003764 File Offset: 0x00001964
		private void CreateBitmapInfo()
		{
			BitmapInfoHeader bitmapInfoHeader;
			bitmapInfoHeader.biWidth = this._windowSize.Width;
			bitmapInfoHeader.biHeight = this._windowSize.Height;
			bitmapInfoHeader.biPlanes = 1;
			bitmapInfoHeader.biBitCount = 32;
			bitmapInfoHeader.biCompression = 0U;
			bitmapInfoHeader.biSizeImage = 0U;
			bitmapInfoHeader.biXPelsPerMeter = 0;
			bitmapInfoHeader.biYPelsPerMeter = 0;
			bitmapInfoHeader.biClrUsed = 0U;
			bitmapInfoHeader.biClrImportant = 0U;
			bitmapInfoHeader.biSize = (uint)Marshal.SizeOf(typeof(BitmapInfoHeader));
			this._bitmapInfo.bmiHeader = bitmapInfoHeader;
			this._bitmapInfo.r = 0;
			this._bitmapInfo.g = 0;
			this._bitmapInfo.b = 0;
			this._bitmapInfo.a = 0;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003828 File Offset: 0x00001A28
		public void SetSize(int width, int height)
		{
			this._windowSize = new Size(width, height);
			if (this._windowSize.Width > 0 && this._windowSize.Height > 0)
			{
				this._pixelData = new byte[this._windowSize.Width * this._windowSize.Height * 4];
			}
			this.CreateBitmapInfo();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003888 File Offset: 0x00001A88
		public void PostRender()
		{
			if (this._windowSize.Width <= 0 || this._windowSize.Height <= 0)
			{
				return;
			}
			Opengl32.PixelStore(Target.PACK_ALIGNMENT, 1);
			Opengl32.ReadPixels(0, 0, this._windowSize.Width, this._windowSize.Height, PixelFormat.BGRA, DataType.UnsignedByte, this._pixelData);
			IntPtr intPtr = Gdi32.CreateCompatibleBitmap(this._screenDC, this._windowSize.Width, this._windowSize.Height);
			IntPtr intPtr2 = Gdi32.SelectObject(this._memoryDC, intPtr);
			Gdi32.StretchDIBits(this._memoryDC, 0, 0, this._windowSize.Width, this._windowSize.Height, 0, 0, this._windowSize.Width, this._windowSize.Height, this._pixelData, ref this._bitmapInfo, 0U, 13369376);
			Rectangle rectangle;
			User32.GetWindowRect(this._windowHandle, out rectangle);
			Point point = new Point(rectangle.Left, rectangle.Top);
			User32.UpdateLayeredWindow(this._windowHandle, this._screenDC, ref point, ref this._windowSize, this._memoryDC, ref this._localOriginPoint, 0, ref this._blendFunction, 2);
			if (intPtr != IntPtr.Zero)
			{
				Gdi32.SelectObject(this._memoryDC, intPtr2);
				Gdi32.DeleteObject(intPtr);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000039D9 File Offset: 0x00001BD9
		public void OnFinalize()
		{
			User32.ReleaseDC(IntPtr.Zero, this._screenDC);
			Gdi32.DeleteDC(this._memoryDC);
		}

		// Token: 0x04000029 RID: 41
		private const int GwlExStyle = -20;

		// Token: 0x0400002A RID: 42
		private const uint WsExLayered = 524288U;

		// Token: 0x0400002B RID: 43
		private readonly IntPtr _windowHandle;

		// Token: 0x0400002C RID: 44
		private readonly IntPtr _screenDC;

		// Token: 0x0400002D RID: 45
		private readonly IntPtr _memoryDC;

		// Token: 0x0400002E RID: 46
		private Size _windowSize;

		// Token: 0x0400002F RID: 47
		private byte[] _pixelData;

		// Token: 0x04000030 RID: 48
		private BlendFunction _blendFunction = BlendFunction.Default;

		// Token: 0x04000031 RID: 49
		private Point _localOriginPoint = new Point(0, 0);

		// Token: 0x04000032 RID: 50
		private BitmapInfo _bitmapInfo;
	}
}

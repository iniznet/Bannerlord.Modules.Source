using System;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class InputData
	{
		public bool[] KeyData { get; set; }

		public bool LeftMouse { get; set; }

		public bool RightMouse { get; set; }

		public int CursorX { get; set; }

		public int CursorY { get; set; }

		public bool MouseMove { get; set; }

		public float MouseScrollDelta { get; set; }

		public InputData()
		{
			this.KeyData = new bool[256];
			this.CursorX = 0;
			this.CursorY = 0;
			this.LeftMouse = false;
			this.RightMouse = false;
			this.MouseMove = false;
			this.MouseScrollDelta = 0f;
			for (int i = 0; i < 256; i++)
			{
				this.KeyData[i] = false;
			}
		}

		public void Reset()
		{
			this.MouseScrollDelta = 0f;
		}

		public void FillFrom(InputData inputData)
		{
			this.CursorX = inputData.CursorX;
			this.CursorY = inputData.CursorY;
			this.LeftMouse = inputData.LeftMouse;
			this.RightMouse = inputData.RightMouse;
			this.MouseMove = inputData.MouseMove;
			this.MouseScrollDelta = inputData.MouseScrollDelta;
			for (int i = 0; i < 256; i++)
			{
				this.KeyData[i] = inputData.KeyData[i];
			}
		}
	}
}

using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	internal class TwoDimensionDrawLayer
	{
		public TwoDimensionDrawLayer()
		{
			this._drawData = new List<TwoDimensionDrawData>(2);
		}

		public void Reset()
		{
			this._drawData.Clear();
		}

		public void AddDrawData(TwoDimensionDrawData drawData)
		{
			this._drawData.Add(drawData);
		}

		public void DrawTo(TwoDimensionContext twoDimensionContext, int layer)
		{
			for (int i = 0; i < this._drawData.Count; i++)
			{
				this._drawData[i].DrawTo(twoDimensionContext, layer);
			}
		}

		public bool IsIntersects(Rectangle rectangle)
		{
			for (int i = 0; i < this._drawData.Count; i++)
			{
				if (this._drawData[i].IsIntersects(rectangle))
				{
					return true;
				}
			}
			return false;
		}

		private List<TwoDimensionDrawData> _drawData;
	}
}

using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000039 RID: 57
	internal class TwoDimensionDrawLayer
	{
		// Token: 0x06000286 RID: 646 RVA: 0x00009E00 File Offset: 0x00008000
		public TwoDimensionDrawLayer()
		{
			this._drawData = new List<TwoDimensionDrawData>(2);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00009E14 File Offset: 0x00008014
		public void Reset()
		{
			this._drawData.Clear();
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00009E21 File Offset: 0x00008021
		public void AddDrawData(TwoDimensionDrawData drawData)
		{
			this._drawData.Add(drawData);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00009E30 File Offset: 0x00008030
		public void DrawTo(TwoDimensionContext twoDimensionContext, int layer)
		{
			for (int i = 0; i < this._drawData.Count; i++)
			{
				this._drawData[i].DrawTo(twoDimensionContext, layer);
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00009E6C File Offset: 0x0000806C
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

		// Token: 0x04000148 RID: 328
		private List<TwoDimensionDrawData> _drawData;
	}
}

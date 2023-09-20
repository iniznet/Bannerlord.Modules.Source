using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200000F RID: 15
	public class BrushLayerAnimation
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00006970 File Offset: 0x00004B70
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00006978 File Offset: 0x00004B78
		public string LayerName { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006981 File Offset: 0x00004B81
		public MBReadOnlyList<BrushAnimationProperty> Collections
		{
			get
			{
				return this._collections;
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00006989 File Offset: 0x00004B89
		public BrushLayerAnimation()
		{
			this.LayerName = null;
			this._collections = new MBList<BrushAnimationProperty>();
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000069A3 File Offset: 0x00004BA3
		internal void RemoveAnimationProperty(BrushAnimationProperty property)
		{
			this._collections.Remove(property);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000069B2 File Offset: 0x00004BB2
		public void AddAnimationProperty(BrushAnimationProperty property)
		{
			this._collections.Add(property);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000069C0 File Offset: 0x00004BC0
		private void FillFrom(BrushLayerAnimation brushLayerAnimation)
		{
			this.LayerName = brushLayerAnimation.LayerName;
			this._collections = new MBList<BrushAnimationProperty>();
			foreach (BrushAnimationProperty brushAnimationProperty in brushLayerAnimation._collections)
			{
				BrushAnimationProperty brushAnimationProperty2 = brushAnimationProperty.Clone();
				this._collections.Add(brushAnimationProperty2);
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006A34 File Offset: 0x00004C34
		public BrushLayerAnimation Clone()
		{
			BrushLayerAnimation brushLayerAnimation = new BrushLayerAnimation();
			brushLayerAnimation.FillFrom(this);
			return brushLayerAnimation;
		}

		// Token: 0x04000065 RID: 101
		private MBList<BrushAnimationProperty> _collections;
	}
}

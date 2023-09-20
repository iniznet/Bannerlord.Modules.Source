using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class BrushLayerAnimation
	{
		public string LayerName { get; set; }

		public MBReadOnlyList<BrushAnimationProperty> Collections
		{
			get
			{
				return this._collections;
			}
		}

		public BrushLayerAnimation()
		{
			this.LayerName = null;
			this._collections = new MBList<BrushAnimationProperty>();
		}

		internal void RemoveAnimationProperty(BrushAnimationProperty property)
		{
			this._collections.Remove(property);
		}

		public void AddAnimationProperty(BrushAnimationProperty property)
		{
			this._collections.Add(property);
		}

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

		public BrushLayerAnimation Clone()
		{
			BrushLayerAnimation brushLayerAnimation = new BrushLayerAnimation();
			brushLayerAnimation.FillFrom(this);
			return brushLayerAnimation;
		}

		private MBList<BrushAnimationProperty> _collections;
	}
}

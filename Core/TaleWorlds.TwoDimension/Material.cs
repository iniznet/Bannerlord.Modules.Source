using System;

namespace TaleWorlds.TwoDimension
{
	public abstract class Material
	{
		public bool Blending { get; private set; }

		public int RenderOrder { get; private set; }

		protected Material(bool blending, int renderOrder)
		{
			this.Blending = blending;
			this.RenderOrder = renderOrder;
		}
	}
}

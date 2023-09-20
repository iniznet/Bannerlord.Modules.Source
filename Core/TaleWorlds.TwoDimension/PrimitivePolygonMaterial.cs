﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class PrimitivePolygonMaterial : Material
	{
		public Color Color { get; private set; }

		public PrimitivePolygonMaterial(Color color)
			: this(color, 0)
		{
		}

		public PrimitivePolygonMaterial(Color color, int renderOrder)
			: this(color, renderOrder, true)
		{
		}

		public PrimitivePolygonMaterial(Color color, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Color = color;
		}
	}
}

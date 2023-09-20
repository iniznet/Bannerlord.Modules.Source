using System;

namespace TaleWorlds.TwoDimension
{
	public class TwoDimensionContextObject
	{
		public TwoDimensionContext Context { get; private set; }

		protected TwoDimensionContextObject(TwoDimensionContext context)
		{
			this.Context = context;
		}
	}
}

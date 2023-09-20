using System;

namespace psai.net
{
	public class Weighting
	{
		internal Weighting()
		{
			this.intensityVsVariety = 0.5f;
			this.lowPlaycountVsRandom = 0.9f;
			this.switchGroups = 0.5f;
		}

		public float switchGroups;

		public float intensityVsVariety;

		public float lowPlaycountVsRandom;
	}
}

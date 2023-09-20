using System;

namespace psai.net
{
	public enum SegmentSuitability
	{
		none,
		start,
		middle,
		end = 4,
		bridge = 8,
		whatever = 15
	}
}

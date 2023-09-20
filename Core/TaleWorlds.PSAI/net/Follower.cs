using System;

namespace psai.net
{
	public struct Follower
	{
		public float compatibility { get; private set; }

		public int snippetId { get; private set; }

		public Follower(int id, float compatibility)
		{
			this.snippetId = id;
			this.compatibility = compatibility;
		}
	}
}

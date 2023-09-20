using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class ThumbnailCacheNode
	{
		public ThumbnailCacheNode()
		{
		}

		public ThumbnailCacheNode(string key, Texture value, int frameNo)
		{
			this.Key = key;
			this.Value = value;
			this.FrameNo = frameNo;
			this.ReferenceCount = 0;
		}

		public string Key;

		public Texture Value;

		public int FrameNo;

		public int ReferenceCount;
	}
}

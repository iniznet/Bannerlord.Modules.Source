using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class ThumbnailCache
	{
		public int Count { get; private set; }

		public ThumbnailCache(int capacity, ThumbnailCreatorView thumbnailCreatorView)
		{
			this._capacity = capacity;
			this._map = new Dictionary<string, ThumbnailCacheNode>(capacity);
			this._thumbnailCreatorView = thumbnailCreatorView;
			this.Count = 0;
		}

		public bool GetValue(string key, out Texture texture)
		{
			texture = null;
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.FrameNo = Utilities.EngineFrameNo;
				texture = thumbnailCacheNode.Value;
				return true;
			}
			return false;
		}

		public bool AddReference(string key)
		{
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.ReferenceCount++;
				return true;
			}
			return false;
		}

		public bool MarkForDeletion(string key)
		{
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.ReferenceCount--;
				return true;
			}
			return false;
		}

		public void ForceDelete(string key)
		{
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.ReferenceCount = 0;
			}
		}

		public void Tick()
		{
			if (this.Count > this._capacity)
			{
				List<ThumbnailCacheNode> list = new List<ThumbnailCacheNode>();
				foreach (KeyValuePair<string, ThumbnailCacheNode> keyValuePair in this._map)
				{
					if (keyValuePair.Value.ReferenceCount == 0)
					{
						list.Add(keyValuePair.Value);
					}
				}
				list.Sort(this._nodeComparer);
				int num = 0;
				while (this.Count > this._capacity && num < list.Count)
				{
					this._map.Remove(list[num].Key);
					this._thumbnailCreatorView.CancelRequest(list[num].Key);
					if (list[num].Value != null)
					{
						list[num].Value.Release();
					}
					num++;
					int count = this.Count;
					this.Count = count - 1;
				}
				list.RemoveRange(0, num);
			}
		}

		public void Add(string key, Texture value)
		{
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.Value = value;
				thumbnailCacheNode.FrameNo = Utilities.EngineFrameNo;
				return;
			}
			ThumbnailCacheNode thumbnailCacheNode2 = new ThumbnailCacheNode(key, value, Utilities.EngineFrameNo);
			this._map[key] = thumbnailCacheNode2;
			int count = this.Count;
			this.Count = count + 1;
		}

		public int GetTotalMemorySize()
		{
			int num = 0;
			foreach (ThumbnailCacheNode thumbnailCacheNode in this._map.Values)
			{
				num += thumbnailCacheNode.Value.MemorySize;
			}
			return num;
		}

		private int _capacity;

		private ThumbnailCreatorView _thumbnailCreatorView;

		private Dictionary<string, ThumbnailCacheNode> _map;

		private NodeComparer _nodeComparer = new NodeComparer();
	}
}

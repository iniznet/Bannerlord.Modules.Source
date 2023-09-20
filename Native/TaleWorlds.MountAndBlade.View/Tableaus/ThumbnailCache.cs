using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000027 RID: 39
	public class ThumbnailCache
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000EB2D File Offset: 0x0000CD2D
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x0000EB35 File Offset: 0x0000CD35
		public int Count { get; private set; }

		// Token: 0x060001A4 RID: 420 RVA: 0x0000EB3E File Offset: 0x0000CD3E
		public ThumbnailCache(int capacity, ThumbnailCreatorView thumbnailCreatorView)
		{
			this._capacity = capacity;
			this._map = new Dictionary<string, ThumbnailCacheNode>(capacity);
			this._thumbnailCreatorView = thumbnailCreatorView;
			this.Count = 0;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000EB74 File Offset: 0x0000CD74
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

		// Token: 0x060001A6 RID: 422 RVA: 0x0000EBAC File Offset: 0x0000CDAC
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

		// Token: 0x060001A7 RID: 423 RVA: 0x0000EBDC File Offset: 0x0000CDDC
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

		// Token: 0x060001A8 RID: 424 RVA: 0x0000EC0C File Offset: 0x0000CE0C
		public void ForceDelete(string key)
		{
			ThumbnailCacheNode thumbnailCacheNode;
			if (this._map.TryGetValue(key, out thumbnailCacheNode))
			{
				thumbnailCacheNode.ReferenceCount = 0;
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000EC30 File Offset: 0x0000CE30
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

		// Token: 0x060001AA RID: 426 RVA: 0x0000ED4C File Offset: 0x0000CF4C
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

		// Token: 0x060001AB RID: 427 RVA: 0x0000EDA8 File Offset: 0x0000CFA8
		public int GetTotalMemorySize()
		{
			int num = 0;
			foreach (ThumbnailCacheNode thumbnailCacheNode in this._map.Values)
			{
				num += thumbnailCacheNode.Value.MemorySize;
			}
			return num;
		}

		// Token: 0x0400010E RID: 270
		private int _capacity;

		// Token: 0x0400010F RID: 271
		private ThumbnailCreatorView _thumbnailCreatorView;

		// Token: 0x04000110 RID: 272
		private Dictionary<string, ThumbnailCacheNode> _map;

		// Token: 0x04000111 RID: 273
		private NodeComparer _nodeComparer = new NodeComparer();
	}
}

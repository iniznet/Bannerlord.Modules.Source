using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000008 RID: 8
	public class BrushAnimation
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003544 File Offset: 0x00001744
		// (set) Token: 0x0600007E RID: 126 RVA: 0x0000354C File Offset: 0x0000174C
		public string Name { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00003555 File Offset: 0x00001755
		// (set) Token: 0x06000080 RID: 128 RVA: 0x0000355D File Offset: 0x0000175D
		public float Duration { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00003566 File Offset: 0x00001766
		// (set) Token: 0x06000082 RID: 130 RVA: 0x0000356E File Offset: 0x0000176E
		public bool Loop { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00003577 File Offset: 0x00001777
		// (set) Token: 0x06000084 RID: 132 RVA: 0x0000357F File Offset: 0x0000177F
		public BrushLayerAnimation StyleAnimation { get; set; }

		// Token: 0x06000085 RID: 133 RVA: 0x00003588 File Offset: 0x00001788
		public BrushAnimation()
		{
			this._data = new Dictionary<string, BrushLayerAnimation>();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000359C File Offset: 0x0000179C
		public void AddAnimationProperty(BrushAnimationProperty property)
		{
			BrushLayerAnimation brushLayerAnimation = null;
			if (string.IsNullOrEmpty(property.LayerName))
			{
				if (this.StyleAnimation == null)
				{
					this.StyleAnimation = new BrushLayerAnimation();
				}
				brushLayerAnimation = this.StyleAnimation;
			}
			else if (!this._data.TryGetValue(property.LayerName, out brushLayerAnimation))
			{
				brushLayerAnimation = new BrushLayerAnimation();
				brushLayerAnimation.LayerName = property.LayerName;
				this._data.Add(property.LayerName, brushLayerAnimation);
			}
			brushLayerAnimation.AddAnimationProperty(property);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003614 File Offset: 0x00001814
		public void RemoveAnimationProperty(BrushAnimationProperty property)
		{
			BrushLayerAnimation brushLayerAnimation;
			if (string.IsNullOrEmpty(property.LayerName))
			{
				if (this.StyleAnimation == null)
				{
					this.StyleAnimation = new BrushLayerAnimation();
				}
				brushLayerAnimation = this.StyleAnimation;
			}
			else
			{
				if (!this._data.ContainsKey(property.LayerName))
				{
					return;
				}
				brushLayerAnimation = this._data[property.LayerName];
			}
			brushLayerAnimation.RemoveAnimationProperty(property);
			if (brushLayerAnimation.Collections.Count == 0)
			{
				this._data.Remove(property.LayerName);
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003698 File Offset: 0x00001898
		public void FillFrom(BrushAnimation animation)
		{
			this.Name = animation.Name;
			this.Duration = animation.Duration;
			this.Loop = animation.Loop;
			if (animation.StyleAnimation != null)
			{
				this.StyleAnimation = animation.StyleAnimation.Clone();
			}
			this._data = new Dictionary<string, BrushLayerAnimation>();
			foreach (KeyValuePair<string, BrushLayerAnimation> keyValuePair in animation._data)
			{
				string key = keyValuePair.Key;
				BrushLayerAnimation brushLayerAnimation = keyValuePair.Value.Clone();
				this._data.Add(key, brushLayerAnimation);
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003750 File Offset: 0x00001950
		public BrushLayerAnimation GetLayerAnimation(string name)
		{
			if (this._data.ContainsKey(name))
			{
				return this._data[name];
			}
			return null;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000376E File Offset: 0x0000196E
		public IEnumerable<BrushLayerAnimation> GetLayerAnimations()
		{
			return this._data.Values;
		}

		// Token: 0x0400002D RID: 45
		private Dictionary<string, BrushLayerAnimation> _data;
	}
}

using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	public class BrushAnimation
	{
		public string Name { get; set; }

		public float Duration { get; set; }

		public bool Loop { get; set; }

		public BrushLayerAnimation StyleAnimation { get; set; }

		public BrushAnimation()
		{
			this._data = new Dictionary<string, BrushLayerAnimation>();
		}

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

		public BrushLayerAnimation GetLayerAnimation(string name)
		{
			if (this._data.ContainsKey(name))
			{
				return this._data[name];
			}
			return null;
		}

		public IEnumerable<BrushLayerAnimation> GetLayerAnimations()
		{
			return this._data.Values;
		}

		private Dictionary<string, BrushLayerAnimation> _data;
	}
}

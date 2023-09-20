using System;
using System.Collections.Generic;

namespace TaleWorlds.ScreenSystem
{
	public class GlobalLayer : IComparable
	{
		public ScreenLayer Layer { get; protected set; }

		internal void EarlyTick(float dt)
		{
			this.OnEarlyTick(dt);
		}

		internal void Tick(float dt)
		{
			this.OnTick(dt);
			this.Layer.Tick(dt);
		}

		internal void LateTick(float dt)
		{
			this.OnLateTick(dt);
			this.Layer.LateTick(dt);
		}

		protected virtual void OnEarlyTick(float dt)
		{
		}

		protected virtual void OnTick(float dt)
		{
		}

		protected virtual void OnLateTick(float dt)
		{
		}

		internal void Update(IReadOnlyList<int> lastKeysPressed)
		{
			this.Layer.Update(lastKeysPressed);
		}

		public int CompareTo(object obj)
		{
			GlobalLayer globalLayer = obj as GlobalLayer;
			if (globalLayer == null)
			{
				return -1;
			}
			return this.Layer.CompareTo(globalLayer.Layer);
		}

		public virtual void UpdateLayout()
		{
			this.Layer.UpdateLayout();
		}
	}
}

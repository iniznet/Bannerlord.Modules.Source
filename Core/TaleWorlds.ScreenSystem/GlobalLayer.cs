using System;
using System.Collections.Generic;

namespace TaleWorlds.ScreenSystem
{
	// Token: 0x02000003 RID: 3
	public class GlobalLayer : IComparable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public ScreenLayer Layer { get; protected set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		internal void EarlyTick(float dt)
		{
			this.OnEarlyTick(dt);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002062 File Offset: 0x00000262
		internal void Tick(float dt)
		{
			this.OnTick(dt);
			this.Layer.Tick(dt);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002077 File Offset: 0x00000277
		internal void LateTick(float dt)
		{
			this.OnLateTick(dt);
			this.Layer.LateTick(dt);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000208C File Offset: 0x0000028C
		protected virtual void OnEarlyTick(float dt)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000208E File Offset: 0x0000028E
		protected virtual void OnTick(float dt)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002090 File Offset: 0x00000290
		protected virtual void OnLateTick(float dt)
		{
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002092 File Offset: 0x00000292
		internal void Update(IReadOnlyList<int> lastKeysPressed)
		{
			this.Layer.Update(lastKeysPressed);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020A0 File Offset: 0x000002A0
		public int CompareTo(object obj)
		{
			GlobalLayer globalLayer = obj as GlobalLayer;
			if (globalLayer == null)
			{
				return -1;
			}
			return this.Layer.CompareTo(globalLayer.Layer);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020CA File Offset: 0x000002CA
		public virtual void UpdateLayout()
		{
			this.Layer.UpdateLayout();
		}
	}
}

using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000188 RID: 392
	public struct ActionIndexValueCache : IEquatable<ActionIndexCache>, IEquatable<ActionIndexValueCache>
	{
		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001486 RID: 5254 RVA: 0x0004EF76 File Offset: 0x0004D176
		// (set) Token: 0x06001487 RID: 5255 RVA: 0x0004EF7D File Offset: 0x0004D17D
		public static ActionIndexValueCache act_none { get; private set; } = new ActionIndexValueCache(-1, "act_none");

		// Token: 0x06001488 RID: 5256 RVA: 0x0004EF85 File Offset: 0x0004D185
		public static ActionIndexValueCache Create(string actName)
		{
			if (string.IsNullOrWhiteSpace(actName))
			{
				return ActionIndexValueCache.act_none;
			}
			return new ActionIndexValueCache(actName);
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0004EF9B File Offset: 0x0004D19B
		public static ActionIndexValueCache Create(ActionIndexCache actCache)
		{
			return new ActionIndexValueCache(actCache.Index);
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0004EFA8 File Offset: 0x0004D1A8
		private ActionIndexValueCache(string actName)
		{
			this._name = actName;
			this._actionIndex = -2;
			this._isValidAction = true;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0004EFC0 File Offset: 0x0004D1C0
		private ActionIndexValueCache(int actionIndex, string actName)
		{
			this._name = actName;
			this._actionIndex = actionIndex;
			this._isValidAction = false;
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0004EFD7 File Offset: 0x0004D1D7
		internal ActionIndexValueCache(int actionIndex)
		{
			this._name = null;
			this._actionIndex = actionIndex;
			this._isValidAction = actionIndex >= 0;
		}

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x0600148D RID: 5261 RVA: 0x0004EFF4 File Offset: 0x0004D1F4
		public int Index
		{
			get
			{
				if (!this._isValidAction)
				{
					return ActionIndexValueCache.act_none._actionIndex;
				}
				if (this._actionIndex == -2)
				{
					this.ResolveIndex();
				}
				return this._actionIndex;
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x0600148E RID: 5262 RVA: 0x0004F01F File Offset: 0x0004D21F
		public string Name
		{
			get
			{
				if (!this._isValidAction)
				{
					return ActionIndexValueCache.act_none._name;
				}
				if (this._name == null)
				{
					this.ResolveName();
				}
				return this._name;
			}
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x0004F048 File Offset: 0x0004D248
		private void ResolveIndex()
		{
			this._actionIndex = MBAnimation.GetActionCodeWithName(this._name);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0004F05B File Offset: 0x0004D25B
		private void ResolveName()
		{
			this._name = MBAnimation.GetActionNameWithCode(this._actionIndex);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x0004F06E File Offset: 0x0004D26E
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is ActionIndexCache)
			{
				return this.Equals((ActionIndexCache)obj);
			}
			return this.Equals((ActionIndexValueCache)obj);
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x0004F096 File Offset: 0x0004D296
		public bool Equals(ActionIndexCache other)
		{
			return other != null && this.Index == other.Index;
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x0004F0AB File Offset: 0x0004D2AB
		public bool Equals(ActionIndexValueCache other)
		{
			return this.Index == other.Index;
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x0004F0BC File Offset: 0x0004D2BC
		public static bool operator ==(ActionIndexCache action0, ActionIndexValueCache action1)
		{
			return action0.Equals(action1);
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0004F0C5 File Offset: 0x0004D2C5
		public static bool operator !=(ActionIndexCache action0, ActionIndexValueCache action1)
		{
			return !(action0 == action1);
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x0004F0D1 File Offset: 0x0004D2D1
		public static bool operator ==(ActionIndexValueCache action0, ActionIndexValueCache action1)
		{
			return action0.Equals(action1);
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x0004F0DB File Offset: 0x0004D2DB
		public static bool operator !=(ActionIndexValueCache action0, ActionIndexValueCache action1)
		{
			return !(action0 == action1);
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x0004F0E8 File Offset: 0x0004D2E8
		public override int GetHashCode()
		{
			return this.Index.GetHashCode();
		}

		// Token: 0x04000720 RID: 1824
		private const int UnresolvedActionIndex = -2;

		// Token: 0x04000721 RID: 1825
		private string _name;

		// Token: 0x04000722 RID: 1826
		private int _actionIndex;

		// Token: 0x04000723 RID: 1827
		private bool _isValidAction;
	}
}

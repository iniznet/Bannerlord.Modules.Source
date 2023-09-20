using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000189 RID: 393
	public class ActionIndexCache : IEquatable<ActionIndexCache>, IEquatable<ActionIndexValueCache>
	{
		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x0600149A RID: 5274 RVA: 0x0004F115 File Offset: 0x0004D315
		// (set) Token: 0x0600149B RID: 5275 RVA: 0x0004F11C File Offset: 0x0004D31C
		public static ActionIndexCache act_none { get; private set; } = new ActionIndexCache(-1, "act_none");

		// Token: 0x0600149C RID: 5276 RVA: 0x0004F124 File Offset: 0x0004D324
		public static ActionIndexCache Create(string actName)
		{
			if (string.IsNullOrWhiteSpace(actName))
			{
				return ActionIndexCache.act_none;
			}
			return new ActionIndexCache(actName);
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x0004F13A File Offset: 0x0004D33A
		private ActionIndexCache(string actName)
		{
			this._name = actName;
			this._actionIndex = -2;
			this._isValidAction = true;
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0004F158 File Offset: 0x0004D358
		private ActionIndexCache(int actionIndex, string actName)
		{
			this._name = actName;
			this._actionIndex = actionIndex;
			this._isValidAction = false;
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0004F175 File Offset: 0x0004D375
		internal ActionIndexCache(int actionIndex)
		{
			this._name = null;
			this._actionIndex = actionIndex;
			this._isValidAction = actionIndex >= 0;
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x0004F198 File Offset: 0x0004D398
		public int Index
		{
			get
			{
				if (!this._isValidAction)
				{
					return ActionIndexCache.act_none._actionIndex;
				}
				if (this._actionIndex == -2)
				{
					this.ResolveIndex();
				}
				return this._actionIndex;
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x0004F1C3 File Offset: 0x0004D3C3
		public string Name
		{
			get
			{
				if (!this._isValidAction)
				{
					return ActionIndexCache.act_none._name;
				}
				if (this._name == null)
				{
					this.ResolveName();
				}
				return this._name;
			}
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x0004F1EC File Offset: 0x0004D3EC
		private void ResolveIndex()
		{
			this._actionIndex = MBAnimation.GetActionCodeWithName(this._name);
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x0004F1FF File Offset: 0x0004D3FF
		private void ResolveName()
		{
			this._name = MBAnimation.GetActionNameWithCode(this._actionIndex);
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x0004F212 File Offset: 0x0004D412
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

		// Token: 0x060014A5 RID: 5285 RVA: 0x0004F23A File Offset: 0x0004D43A
		public bool Equals(ActionIndexCache other)
		{
			return other != null && this.Index == other.Index;
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x0004F24F File Offset: 0x0004D44F
		public bool Equals(ActionIndexValueCache other)
		{
			return this.Index == other.Index;
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x0004F260 File Offset: 0x0004D460
		public static bool operator ==(ActionIndexValueCache action0, ActionIndexCache action1)
		{
			return action0.Equals(action1);
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x0004F26A File Offset: 0x0004D46A
		public static bool operator !=(ActionIndexValueCache action0, ActionIndexCache action1)
		{
			return !(action0 == action1);
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x0004F276 File Offset: 0x0004D476
		public static bool operator ==(ActionIndexCache action0, ActionIndexCache action1)
		{
			return (action0 != null || action1 == null) && (action1 != null || action0 == null) && (action0 == action1 || action0.Equals(action1));
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x0004F295 File Offset: 0x0004D495
		public static bool operator !=(ActionIndexCache action0, ActionIndexCache action1)
		{
			return !(action0 == action1);
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0004F2A4 File Offset: 0x0004D4A4
		public override int GetHashCode()
		{
			return this.Index.GetHashCode();
		}

		// Token: 0x04000725 RID: 1829
		private const int UnresolvedActionIndex = -2;

		// Token: 0x04000726 RID: 1830
		private string _name;

		// Token: 0x04000727 RID: 1831
		private int _actionIndex;

		// Token: 0x04000728 RID: 1832
		private bool _isValidAction;
	}
}

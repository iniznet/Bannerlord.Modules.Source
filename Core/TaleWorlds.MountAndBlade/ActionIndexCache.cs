using System;

namespace TaleWorlds.MountAndBlade
{
	public class ActionIndexCache : IEquatable<ActionIndexCache>, IEquatable<ActionIndexValueCache>
	{
		public static ActionIndexCache act_none { get; private set; } = new ActionIndexCache(-1, "act_none");

		public static ActionIndexCache Create(string actName)
		{
			if (string.IsNullOrWhiteSpace(actName))
			{
				return ActionIndexCache.act_none;
			}
			return new ActionIndexCache(actName);
		}

		private ActionIndexCache(string actName)
		{
			this._name = actName;
			this._actionIndex = -2;
			this._isValidAction = true;
		}

		private ActionIndexCache(int actionIndex, string actName)
		{
			this._name = actName;
			this._actionIndex = actionIndex;
			this._isValidAction = false;
		}

		internal ActionIndexCache(int actionIndex)
		{
			this._name = null;
			this._actionIndex = actionIndex;
			this._isValidAction = actionIndex >= 0;
		}

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

		private void ResolveIndex()
		{
			this._actionIndex = MBAnimation.GetActionCodeWithName(this._name);
		}

		private void ResolveName()
		{
			this._name = MBAnimation.GetActionNameWithCode(this._actionIndex);
		}

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

		public bool Equals(ActionIndexCache other)
		{
			return other != null && this.Index == other.Index;
		}

		public bool Equals(ActionIndexValueCache other)
		{
			return this.Index == other.Index;
		}

		public static bool operator ==(ActionIndexValueCache action0, ActionIndexCache action1)
		{
			return action0.Equals(action1);
		}

		public static bool operator !=(ActionIndexValueCache action0, ActionIndexCache action1)
		{
			return !(action0 == action1);
		}

		public static bool operator ==(ActionIndexCache action0, ActionIndexCache action1)
		{
			return (action0 != null || action1 == null) && (action1 != null || action0 == null) && (action0 == action1 || action0.Equals(action1));
		}

		public static bool operator !=(ActionIndexCache action0, ActionIndexCache action1)
		{
			return !(action0 == action1);
		}

		public override int GetHashCode()
		{
			return this.Index.GetHashCode();
		}

		private const int UnresolvedActionIndex = -2;

		private string _name;

		private int _actionIndex;

		private bool _isValidAction;
	}
}

using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class InitialStateOption
	{
		public int OrderIndex { get; private set; }

		public TextObject Name { get; private set; }

		public string Id { get; private set; }

		public Func<ValueTuple<bool, TextObject>> IsDisabledAndReason { get; private set; }

		public InitialStateOption(string id, TextObject name, int orderIndex, Action action, Func<ValueTuple<bool, TextObject>> isDisabledAndReason)
		{
			this.Name = name;
			this.Id = id;
			this.OrderIndex = orderIndex;
			this._action = action;
			this.IsDisabledAndReason = isDisabledAndReason;
			TextObject item = this.IsDisabledAndReason().Item2;
			string.IsNullOrEmpty((item != null) ? item.ToString() : null);
		}

		public void DoAction()
		{
			Action action = this._action;
			if (action == null)
			{
				return;
			}
			action();
		}

		private Action _action;
	}
}

using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000228 RID: 552
	public class InitialStateOption
	{
		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06001E31 RID: 7729 RVA: 0x0006CCDC File Offset: 0x0006AEDC
		// (set) Token: 0x06001E32 RID: 7730 RVA: 0x0006CCE4 File Offset: 0x0006AEE4
		public int OrderIndex { get; private set; }

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001E33 RID: 7731 RVA: 0x0006CCED File Offset: 0x0006AEED
		// (set) Token: 0x06001E34 RID: 7732 RVA: 0x0006CCF5 File Offset: 0x0006AEF5
		public TextObject Name { get; private set; }

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001E35 RID: 7733 RVA: 0x0006CCFE File Offset: 0x0006AEFE
		// (set) Token: 0x06001E36 RID: 7734 RVA: 0x0006CD06 File Offset: 0x0006AF06
		public string Id { get; private set; }

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06001E37 RID: 7735 RVA: 0x0006CD0F File Offset: 0x0006AF0F
		// (set) Token: 0x06001E38 RID: 7736 RVA: 0x0006CD17 File Offset: 0x0006AF17
		public Func<ValueTuple<bool, TextObject>> IsDisabledAndReason { get; private set; }

		// Token: 0x06001E39 RID: 7737 RVA: 0x0006CD20 File Offset: 0x0006AF20
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

		// Token: 0x06001E3A RID: 7738 RVA: 0x0006CD7A File Offset: 0x0006AF7A
		public void DoAction()
		{
			Action action = this._action;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x04000B28 RID: 2856
		private Action _action;
	}
}

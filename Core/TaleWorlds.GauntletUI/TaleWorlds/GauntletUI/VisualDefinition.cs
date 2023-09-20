using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200002F RID: 47
	public class VisualDefinition
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0000E78E File Offset: 0x0000C98E
		// (set) Token: 0x06000329 RID: 809 RVA: 0x0000E796 File Offset: 0x0000C996
		public string Name { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600032A RID: 810 RVA: 0x0000E79F File Offset: 0x0000C99F
		// (set) Token: 0x0600032B RID: 811 RVA: 0x0000E7A7 File Offset: 0x0000C9A7
		public float TransitionDuration { get; private set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600032C RID: 812 RVA: 0x0000E7B0 File Offset: 0x0000C9B0
		// (set) Token: 0x0600032D RID: 813 RVA: 0x0000E7B8 File Offset: 0x0000C9B8
		public float DelayOnBegin { get; private set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600032E RID: 814 RVA: 0x0000E7C1 File Offset: 0x0000C9C1
		// (set) Token: 0x0600032F RID: 815 RVA: 0x0000E7C9 File Offset: 0x0000C9C9
		public bool EaseIn { get; private set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000330 RID: 816 RVA: 0x0000E7D2 File Offset: 0x0000C9D2
		// (set) Token: 0x06000331 RID: 817 RVA: 0x0000E7DA File Offset: 0x0000C9DA
		public Dictionary<string, VisualState> VisualStates { get; private set; }

		// Token: 0x06000332 RID: 818 RVA: 0x0000E7E3 File Offset: 0x0000C9E3
		public VisualDefinition(string name, float transitionDuration, float delayOnBegin, bool easeIn)
		{
			this.Name = name;
			this.TransitionDuration = transitionDuration;
			this.DelayOnBegin = delayOnBegin;
			this.EaseIn = easeIn;
			this.VisualStates = new Dictionary<string, VisualState>();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000E813 File Offset: 0x0000CA13
		public void AddVisualState(VisualState visualState)
		{
			this.VisualStates.Add(visualState.State, visualState);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000E827 File Offset: 0x0000CA27
		public VisualState GetVisualState(string state)
		{
			if (this.VisualStates.ContainsKey(state))
			{
				return this.VisualStates[state];
			}
			return null;
		}
	}
}

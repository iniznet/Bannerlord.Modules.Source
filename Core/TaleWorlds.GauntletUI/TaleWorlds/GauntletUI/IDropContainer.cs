using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000025 RID: 37
	public interface IDropContainer
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060002D8 RID: 728
		// (set) Token: 0x060002D9 RID: 729
		Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x060002DA RID: 730
		Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition);
	}
}

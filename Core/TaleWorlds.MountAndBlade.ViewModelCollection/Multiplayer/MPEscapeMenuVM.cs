using System;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003D RID: 61
	public class MPEscapeMenuVM : EscapeMenuVM
	{
		// Token: 0x06000545 RID: 1349 RVA: 0x00016CB2 File Offset: 0x00014EB2
		public MPEscapeMenuVM(IEnumerable<EscapeMenuItemVM> items, TextObject title = null)
			: base(items, title)
		{
		}
	}
}

using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000381 RID: 897
	public class UsableGameObjectGroup : ScriptComponentBehavior, IVisible
	{
		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060030B4 RID: 12468 RVA: 0x000CA159 File Offset: 0x000C8359
		// (set) Token: 0x060030B5 RID: 12469 RVA: 0x000CA166 File Offset: 0x000C8366
		public bool IsVisible
		{
			get
			{
				return base.GameEntity.IsVisibleIncludeParents();
			}
			set
			{
				base.GameEntity.SetVisibilityExcludeParents(value);
			}
		}
	}
}

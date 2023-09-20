using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x0200040C RID: 1036
	public abstract class FormationArrangementModel : GameModel
	{
		// Token: 0x0600358B RID: 13707
		public abstract List<FormationArrangementModel.ArrangementPosition> GetBannerBearerPositions(Formation formation, int maxCount);

		// Token: 0x020006E2 RID: 1762
		public struct ArrangementPosition
		{
			// Token: 0x17000A1A RID: 2586
			// (get) Token: 0x06004026 RID: 16422 RVA: 0x000F99A3 File Offset: 0x000F7BA3
			public bool IsValid
			{
				get
				{
					return this.FileIndex > -1 && this.RankIndex > -1;
				}
			}

			// Token: 0x17000A1B RID: 2587
			// (get) Token: 0x06004027 RID: 16423 RVA: 0x000F99BC File Offset: 0x000F7BBC
			public static FormationArrangementModel.ArrangementPosition Invalid
			{
				get
				{
					return default(FormationArrangementModel.ArrangementPosition);
				}
			}

			// Token: 0x06004028 RID: 16424 RVA: 0x000F99D2 File Offset: 0x000F7BD2
			public ArrangementPosition(int fileIndex = -1, int rankIndex = -1)
			{
				this.FileIndex = fileIndex;
				this.RankIndex = rankIndex;
			}

			// Token: 0x040022F4 RID: 8948
			public readonly int FileIndex;

			// Token: 0x040022F5 RID: 8949
			public readonly int RankIndex;
		}
	}
}

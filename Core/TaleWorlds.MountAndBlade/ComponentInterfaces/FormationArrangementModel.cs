using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class FormationArrangementModel : GameModel
	{
		public abstract List<FormationArrangementModel.ArrangementPosition> GetBannerBearerPositions(Formation formation, int maxCount);

		public struct ArrangementPosition
		{
			public bool IsValid
			{
				get
				{
					return this.FileIndex > -1 && this.RankIndex > -1;
				}
			}

			public static FormationArrangementModel.ArrangementPosition Invalid
			{
				get
				{
					return default(FormationArrangementModel.ArrangementPosition);
				}
			}

			public ArrangementPosition(int fileIndex = -1, int rankIndex = -1)
			{
				this.FileIndex = fileIndex;
				this.RankIndex = rankIndex;
			}

			public readonly int FileIndex;

			public readonly int RankIndex;
		}
	}
}

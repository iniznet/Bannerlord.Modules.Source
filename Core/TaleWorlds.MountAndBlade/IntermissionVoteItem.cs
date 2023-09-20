using System;

namespace TaleWorlds.MountAndBlade
{
	public class IntermissionVoteItem
	{
		public int VoteCount { get; private set; }

		public IntermissionVoteItem(string id, int index)
		{
			this.Id = id;
			this.Index = index;
			this.VoteCount = 0;
		}

		public void SetVoteCount(int voteCount)
		{
			this.VoteCount = voteCount;
		}

		public void IncreaseVoteCount(int incrementAmount)
		{
			this.VoteCount += incrementAmount;
		}

		public readonly string Id;

		public readonly int Index;
	}
}

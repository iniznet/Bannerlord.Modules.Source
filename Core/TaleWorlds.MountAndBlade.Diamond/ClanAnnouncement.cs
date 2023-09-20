using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanAnnouncement
	{
		public int Id { get; private set; }

		public string Announcement { get; private set; }

		public PlayerId AuthorId { get; private set; }

		public DateTime CreationTime { get; private set; }

		public ClanAnnouncement(int id, string announcement, PlayerId authorId, DateTime creationTime)
		{
			this.Id = id;
			this.Announcement = announcement;
			this.AuthorId = authorId;
			this.CreationTime = creationTime;
		}
	}
}

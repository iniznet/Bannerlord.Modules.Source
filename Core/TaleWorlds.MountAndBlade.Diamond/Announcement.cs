using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E0 RID: 224
	[Serializable]
	public class Announcement
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600035C RID: 860 RVA: 0x00004630 File Offset: 0x00002830
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00004638 File Offset: 0x00002838
		public int Id { get; private set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00004641 File Offset: 0x00002841
		// (set) Token: 0x0600035F RID: 863 RVA: 0x00004649 File Offset: 0x00002849
		public Guid BattleId { get; private set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000360 RID: 864 RVA: 0x00004652 File Offset: 0x00002852
		// (set) Token: 0x06000361 RID: 865 RVA: 0x0000465A File Offset: 0x0000285A
		public AnnouncementType Type { get; private set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00004663 File Offset: 0x00002863
		// (set) Token: 0x06000363 RID: 867 RVA: 0x0000466B File Offset: 0x0000286B
		public TextObject Text { get; private set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000364 RID: 868 RVA: 0x00004674 File Offset: 0x00002874
		// (set) Token: 0x06000365 RID: 869 RVA: 0x0000467C File Offset: 0x0000287C
		public bool IsEnabled { get; private set; }

		// Token: 0x06000366 RID: 870 RVA: 0x00004685 File Offset: 0x00002885
		public Announcement(int id, Guid battleId, AnnouncementType type, TextObject text, bool isEnabled)
		{
			this.Id = id;
			this.BattleId = battleId;
			this.Type = type;
			this.Text = text;
			this.IsEnabled = isEnabled;
		}
	}
}

using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032A RID: 810
	[AttributeUsage(AttributeTargets.Field)]
	public class NotificationProperty : Attribute
	{
		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06002BE1 RID: 11233 RVA: 0x000AA790 File Offset: 0x000A8990
		// (set) Token: 0x06002BE2 RID: 11234 RVA: 0x000AA798 File Offset: 0x000A8998
		public string StringId { get; private set; }

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06002BE3 RID: 11235 RVA: 0x000AA7A1 File Offset: 0x000A89A1
		// (set) Token: 0x06002BE4 RID: 11236 RVA: 0x000AA7A9 File Offset: 0x000A89A9
		public string SoundIdOne { get; private set; }

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06002BE5 RID: 11237 RVA: 0x000AA7B2 File Offset: 0x000A89B2
		// (set) Token: 0x06002BE6 RID: 11238 RVA: 0x000AA7BA File Offset: 0x000A89BA
		public string SoundIdTwo { get; private set; }

		// Token: 0x06002BE7 RID: 11239 RVA: 0x000AA7C3 File Offset: 0x000A89C3
		public NotificationProperty(string stringId, string soundIdOne, string soundIdTwo = "")
		{
			this.StringId = stringId;
			this.SoundIdOne = soundIdOne;
			this.SoundIdTwo = soundIdTwo;
		}
	}
}

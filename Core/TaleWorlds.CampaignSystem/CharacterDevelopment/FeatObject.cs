using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000352 RID: 850
	public sealed class FeatObject : PropertyObject
	{
		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x06002FCD RID: 12237 RVA: 0x000CB34E File Offset: 0x000C954E
		public static MBReadOnlyList<FeatObject> All
		{
			get
			{
				return Campaign.Current.AllFeats;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x06002FCE RID: 12238 RVA: 0x000CB35A File Offset: 0x000C955A
		// (set) Token: 0x06002FCF RID: 12239 RVA: 0x000CB362 File Offset: 0x000C9562
		public float EffectBonus { get; private set; }

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x06002FD0 RID: 12240 RVA: 0x000CB36B File Offset: 0x000C956B
		// (set) Token: 0x06002FD1 RID: 12241 RVA: 0x000CB373 File Offset: 0x000C9573
		public FeatObject.AdditionType IncrementType { get; private set; }

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x06002FD2 RID: 12242 RVA: 0x000CB37C File Offset: 0x000C957C
		// (set) Token: 0x06002FD3 RID: 12243 RVA: 0x000CB384 File Offset: 0x000C9584
		public bool IsPositive { get; private set; }

		// Token: 0x06002FD4 RID: 12244 RVA: 0x000CB38D File Offset: 0x000C958D
		public FeatObject(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06002FD5 RID: 12245 RVA: 0x000CB396 File Offset: 0x000C9596
		public void Initialize(string name, string description, float effectBonus, bool isPositiveEffect, FeatObject.AdditionType incrementType)
		{
			base.Initialize(new TextObject(name, null), new TextObject(description, null));
			this.EffectBonus = effectBonus;
			this.IncrementType = incrementType;
			this.IsPositive = isPositiveEffect;
			base.AfterInitialized();
		}

		// Token: 0x0200068D RID: 1677
		public enum AdditionType
		{
			// Token: 0x04001B37 RID: 6967
			Add,
			// Token: 0x04001B38 RID: 6968
			AddFactor
		}
	}
}

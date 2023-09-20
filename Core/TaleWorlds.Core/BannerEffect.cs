using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000012 RID: 18
	public sealed class BannerEffect : PropertyObject
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x000042AD File Offset: 0x000024AD
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x000042B5 File Offset: 0x000024B5
		public BannerEffect.EffectIncrementType IncrementType { get; private set; }

		// Token: 0x060000D2 RID: 210 RVA: 0x000042BE File Offset: 0x000024BE
		public BannerEffect(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000042D4 File Offset: 0x000024D4
		public void Initialize(string name, string description, float level1Bonus, float level2Bonus, float level3Bonus, BannerEffect.EffectIncrementType incrementType)
		{
			TextObject textObject = new TextObject(description, null);
			this._levelBonuses[0] = level1Bonus;
			this._levelBonuses[1] = level2Bonus;
			this._levelBonuses[2] = level3Bonus;
			this.IncrementType = incrementType;
			base.Initialize(new TextObject(name, null), textObject);
			base.AfterInitialized();
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00004324 File Offset: 0x00002524
		public float GetBonusAtLevel(int bannerLevel)
		{
			int num = bannerLevel - 1;
			num = MBMath.ClampIndex(num, 0, this._levelBonuses.Length);
			return this._levelBonuses[num];
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00004350 File Offset: 0x00002550
		public string GetBonusStringAtLevel(int bannerLevel)
		{
			float bonusAtLevel = this.GetBonusAtLevel(bannerLevel);
			return string.Format("{0:P2}", bonusAtLevel);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00004378 File Offset: 0x00002578
		public TextObject GetDescription(int bannerLevel)
		{
			float bonusAtLevel = this.GetBonusAtLevel(bannerLevel);
			if (bonusAtLevel > 0f)
			{
				TextObject textObject = new TextObject("{=Ffwgecvr}{PLUS_OR_MINUS}{BONUSEFFECT}", null);
				textObject.SetTextVariable("BONUSEFFECT", bonusAtLevel);
				textObject.SetTextVariable("PLUS_OR_MINUS", "{=eTw2aNV5}+");
				return base.Description.SetTextVariable("BONUS_AMOUNT", textObject);
			}
			return base.Description.SetTextVariable("BONUS_AMOUNT", bonusAtLevel);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000043E2 File Offset: 0x000025E2
		public override string ToString()
		{
			return base.Name.ToString();
		}

		// Token: 0x040000EE RID: 238
		private readonly float[] _levelBonuses = new float[3];

		// Token: 0x020000D4 RID: 212
		public enum EffectIncrementType
		{
			// Token: 0x0400060A RID: 1546
			Invalid = -1,
			// Token: 0x0400060B RID: 1547
			Add,
			// Token: 0x0400060C RID: 1548
			AddFactor
		}
	}
}

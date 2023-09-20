using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public sealed class BannerEffect : PropertyObject
	{
		public BannerEffect.EffectIncrementType IncrementType { get; private set; }

		public BannerEffect(string stringId)
			: base(stringId)
		{
		}

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

		public float GetBonusAtLevel(int bannerLevel)
		{
			int num = bannerLevel - 1;
			num = MBMath.ClampIndex(num, 0, this._levelBonuses.Length);
			return this._levelBonuses[num];
		}

		public string GetBonusStringAtLevel(int bannerLevel)
		{
			float bonusAtLevel = this.GetBonusAtLevel(bannerLevel);
			return string.Format("{0:P2}", bonusAtLevel);
		}

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

		public override string ToString()
		{
			return base.Name.ToString();
		}

		private readonly float[] _levelBonuses = new float[3];

		public enum EffectIncrementType
		{
			Invalid = -1,
			Add,
			AddFactor
		}
	}
}

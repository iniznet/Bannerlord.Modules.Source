using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public struct CraftingStatData
	{
		public bool IsValid
		{
			get
			{
				return this.MaxValue >= 0f;
			}
		}

		public CraftingStatData(TextObject descriptionText, float curValue, float maxValue, CraftingTemplate.CraftingStatTypes type, DamageTypes damageType = DamageTypes.Invalid)
		{
			this.DescriptionText = descriptionText;
			this.CurValue = curValue;
			this.MaxValue = maxValue;
			this.Type = type;
			this.DamageType = damageType;
		}

		public readonly TextObject DescriptionText;

		public readonly float CurValue;

		public readonly float MaxValue;

		public readonly CraftingTemplate.CraftingStatTypes Type;

		public readonly DamageTypes DamageType;
	}
}

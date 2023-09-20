using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public sealed class FeatObject : PropertyObject
	{
		public static MBReadOnlyList<FeatObject> All
		{
			get
			{
				return Campaign.Current.AllFeats;
			}
		}

		public float EffectBonus { get; private set; }

		public FeatObject.AdditionType IncrementType { get; private set; }

		public bool IsPositive { get; private set; }

		public FeatObject(string stringId)
			: base(stringId)
		{
		}

		public void Initialize(string name, string description, float effectBonus, bool isPositiveEffect, FeatObject.AdditionType incrementType)
		{
			base.Initialize(new TextObject(name, null), new TextObject(description, null));
			this.EffectBonus = effectBonus;
			this.IncrementType = incrementType;
			this.IsPositive = isPositiveEffect;
			base.AfterInitialized();
		}

		public enum AdditionType
		{
			Add,
			AddFactor
		}
	}
}

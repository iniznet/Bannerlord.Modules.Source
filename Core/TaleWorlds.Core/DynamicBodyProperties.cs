using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	[Serializable]
	public struct DynamicBodyProperties
	{
		public DynamicBodyProperties(float age, float weight, float build)
		{
			this.Age = age;
			this.Weight = weight;
			this.Build = build;
		}

		public static bool operator ==(DynamicBodyProperties a, DynamicBodyProperties b)
		{
			return a == b || (a != null && b != null && (a.Age == b.Age && a.Weight == b.Weight) && a.Build == b.Build);
		}

		public static bool operator !=(DynamicBodyProperties a, DynamicBodyProperties b)
		{
			return !(a == b);
		}

		public bool Equals(DynamicBodyProperties other)
		{
			return this.Age.Equals(other.Age) && this.Weight.Equals(other.Weight) && this.Build.Equals(other.Build);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is DynamicBodyProperties && this.Equals((DynamicBodyProperties)obj);
		}

		public override int GetHashCode()
		{
			return (((this.Age.GetHashCode() * 397) ^ this.Weight.GetHashCode()) * 397) ^ this.Build.GetHashCode();
		}

		public override string ToString()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(150, "ToString");
			mbstringBuilder.Append<string>("age=\"");
			mbstringBuilder.Append<string>(this.Age.ToString("0.##"));
			mbstringBuilder.Append<string>("\" weight=\"");
			mbstringBuilder.Append<string>(this.Weight.ToString("0.####"));
			mbstringBuilder.Append<string>("\" build=\"");
			mbstringBuilder.Append<string>(this.Build.ToString("0.####"));
			mbstringBuilder.Append<string>("\" ");
			return mbstringBuilder.ToStringAndRelease();
		}

		public const float MaxAge = 128f;

		public const float MaxAgeTeenager = 21f;

		public float Age;

		public float Weight;

		public float Build;

		public static readonly DynamicBodyProperties Invalid;

		public static readonly DynamicBodyProperties Default = new DynamicBodyProperties(20f, 0.5f, 0.5f);
	}
}

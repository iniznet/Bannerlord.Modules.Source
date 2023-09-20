using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000054 RID: 84
	[Serializable]
	public struct DynamicBodyProperties
	{
		// Token: 0x06000623 RID: 1571 RVA: 0x00016672 File Offset: 0x00014872
		public DynamicBodyProperties(float age, float weight, float build)
		{
			this.Age = age;
			this.Weight = weight;
			this.Build = build;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0001668C File Offset: 0x0001488C
		public static bool operator ==(DynamicBodyProperties a, DynamicBodyProperties b)
		{
			return a == b || (a != null && b != null && (a.Age == b.Age && a.Weight == b.Weight) && a.Build == b.Build);
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x000166E7 File Offset: 0x000148E7
		public static bool operator !=(DynamicBodyProperties a, DynamicBodyProperties b)
		{
			return !(a == b);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000166F3 File Offset: 0x000148F3
		public bool Equals(DynamicBodyProperties other)
		{
			return this.Age.Equals(other.Age) && this.Weight.Equals(other.Weight) && this.Build.Equals(other.Build);
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0001672E File Offset: 0x0001492E
		public override bool Equals(object obj)
		{
			return obj != null && obj is DynamicBodyProperties && this.Equals((DynamicBodyProperties)obj);
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x0001674B File Offset: 0x0001494B
		public override int GetHashCode()
		{
			return (((this.Age.GetHashCode() * 397) ^ this.Weight.GetHashCode()) * 397) ^ this.Build.GetHashCode();
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0001677C File Offset: 0x0001497C
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

		// Token: 0x04000315 RID: 789
		public const float MaxAge = 128f;

		// Token: 0x04000316 RID: 790
		public const float MaxAgeTeenager = 21f;

		// Token: 0x04000317 RID: 791
		public float Age;

		// Token: 0x04000318 RID: 792
		public float Weight;

		// Token: 0x04000319 RID: 793
		public float Build;

		// Token: 0x0400031A RID: 794
		public static readonly DynamicBodyProperties Invalid;

		// Token: 0x0400031B RID: 795
		public static readonly DynamicBodyProperties Default = new DynamicBodyProperties(20f, 0.5f, 0.5f);
	}
}

using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000D8 RID: 216
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MeansImplicitUseAttribute : Attribute
	{
		// Token: 0x06000876 RID: 2166 RVA: 0x0000F2CA File Offset: 0x0000D4CA
		[UsedImplicitly]
		public MeansImplicitUseAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0000F2D4 File Offset: 0x0000D4D4
		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0000F2EA File Offset: 0x0000D4EA
		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0000F2F4 File Offset: 0x0000D4F4
		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x0000F2FE File Offset: 0x0000D4FE
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x0000F306 File Offset: 0x0000D506
		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x0000F30F File Offset: 0x0000D50F
		// (set) Token: 0x0600087D RID: 2173 RVA: 0x0000F317 File Offset: 0x0000D517
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}
}

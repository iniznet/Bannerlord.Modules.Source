using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000D7 RID: 215
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class UsedImplicitlyAttribute : Attribute
	{
		// Token: 0x0600086E RID: 2158 RVA: 0x0000F274 File Offset: 0x0000D474
		[UsedImplicitly]
		public UsedImplicitlyAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0000F27E File Offset: 0x0000D47E
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0000F294 File Offset: 0x0000D494
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0000F29E File Offset: 0x0000D49E
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000872 RID: 2162 RVA: 0x0000F2A8 File Offset: 0x0000D4A8
		// (set) Token: 0x06000873 RID: 2163 RVA: 0x0000F2B0 File Offset: 0x0000D4B0
		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000874 RID: 2164 RVA: 0x0000F2B9 File Offset: 0x0000D4B9
		// (set) Token: 0x06000875 RID: 2165 RVA: 0x0000F2C1 File Offset: 0x0000D4C1
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}
}

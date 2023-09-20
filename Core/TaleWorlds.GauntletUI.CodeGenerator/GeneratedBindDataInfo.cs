using System;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	// Token: 0x02000005 RID: 5
	public class GeneratedBindDataInfo
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000021B4 File Offset: 0x000003B4
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000021BC File Offset: 0x000003BC
		public string Property { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000021C5 File Offset: 0x000003C5
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000021CD File Offset: 0x000003CD
		public string Path { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000021D6 File Offset: 0x000003D6
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000021DE File Offset: 0x000003DE
		public Type WidgetPropertyType { get; internal set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000021E7 File Offset: 0x000003E7
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000021EF File Offset: 0x000003EF
		public Type ViewModelPropertType { get; internal set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000021F8 File Offset: 0x000003F8
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002200 File Offset: 0x00000400
		public bool RequiresConversion { get; internal set; }

		// Token: 0x0600002A RID: 42 RVA: 0x00002209 File Offset: 0x00000409
		internal GeneratedBindDataInfo(string property, string path)
		{
			this.Property = property;
			this.Path = path;
		}
	}
}

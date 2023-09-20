using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000011 RID: 17
	public class SaveableObjectSystemTypeDefiner : SaveableTypeDefiner
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00004442 File Offset: 0x00002642
		public SaveableObjectSystemTypeDefiner()
			: base(10000)
		{
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000444F File Offset: 0x0000264F
		protected override void DefineBasicTypes()
		{
			base.DefineBasicTypes();
			base.AddBasicTypeDefinition(typeof(MBGUID), 1005, new MBGUIDBasicTypeSerializer());
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004471 File Offset: 0x00002671
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(MBObjectBase), 34, null);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004486 File Offset: 0x00002686
		protected override void DefineStructTypes()
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004488 File Offset: 0x00002688
		protected override void DefineEnumTypes()
		{
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000448A File Offset: 0x0000268A
		protected override void DefineInterfaceTypes()
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000448C File Offset: 0x0000268C
		protected override void DefineRootClassTypes()
		{
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000448E File Offset: 0x0000268E
		protected override void DefineGenericClassDefinitions()
		{
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004490 File Offset: 0x00002690
		protected override void DefineGenericStructDefinitions()
		{
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004492 File Offset: 0x00002692
		protected override void DefineContainerDefinitions()
		{
		}
	}
}

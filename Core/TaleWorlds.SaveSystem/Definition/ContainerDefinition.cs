using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000056 RID: 86
	public class ContainerDefinition : TypeDefinitionBase
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000A934 File Offset: 0x00008B34
		// (set) Token: 0x06000289 RID: 649 RVA: 0x0000A93C File Offset: 0x00008B3C
		public Assembly DefinedAssembly { get; private set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000A945 File Offset: 0x00008B45
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000A94D File Offset: 0x00008B4D
		public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000A956 File Offset: 0x00008B56
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000A95E File Offset: 0x00008B5E
		public bool HasNoChildObject { get; private set; }

		// Token: 0x0600028E RID: 654 RVA: 0x0000A967 File Offset: 0x00008B67
		public ContainerDefinition(Type type, ContainerSaveId saveId, Assembly definedAssembly)
			: base(type, saveId)
		{
			this.DefinedAssembly = definedAssembly;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000A978 File Offset: 0x00008B78
		public void InitializeForAutoGeneration(CollectObjectsDelegate collectObjectsDelegate, bool hasNoChildObject)
		{
			this.CollectObjectsMethod = collectObjectsDelegate;
			this.HasNoChildObject = hasNoChildObject;
		}
	}
}

using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000032 RID: 50
	public class ContainerHeaderLoadData
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00008606 File Offset: 0x00006806
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000860E File Offset: 0x0000680E
		public int Id { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x00008617 File Offset: 0x00006817
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000861F File Offset: 0x0000681F
		public object Target { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00008628 File Offset: 0x00006828
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x00008630 File Offset: 0x00006830
		public LoadContext Context { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00008639 File Offset: 0x00006839
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00008641 File Offset: 0x00006841
		public ContainerDefinition TypeDefinition { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000864A File Offset: 0x0000684A
		// (set) Token: 0x060001CD RID: 461 RVA: 0x00008652 File Offset: 0x00006852
		public SaveId SaveId { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000865B File Offset: 0x0000685B
		// (set) Token: 0x060001CF RID: 463 RVA: 0x00008663 File Offset: 0x00006863
		public int ElementCount { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000866C File Offset: 0x0000686C
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x00008674 File Offset: 0x00006874
		public ContainerType ContainerType { get; private set; }

		// Token: 0x060001D2 RID: 466 RVA: 0x0000867D File Offset: 0x0000687D
		public ContainerHeaderLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00008693 File Offset: 0x00006893
		public bool GetObjectTypeDefinition()
		{
			this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this.SaveId) as ContainerDefinition;
			return this.TypeDefinition != null;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x000086C0 File Offset: 0x000068C0
		public void CreateObject()
		{
			Type type = this.TypeDefinition.Type;
			if (this.ContainerType == ContainerType.Array)
			{
				this.Target = Activator.CreateInstance(type, new object[] { this.ElementCount });
				return;
			}
			if (this.ContainerType == ContainerType.List)
			{
				this.Target = Activator.CreateInstance(typeof(MBList<>).MakeGenericType(type.GetGenericArguments()));
				return;
			}
			this.Target = Activator.CreateInstance(type);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000873C File Offset: 0x0000693C
		public void InitialieReaders(SaveEntryFolder saveEntryFolder)
		{
			BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Object)).GetBinaryReader();
			this.SaveId = SaveId.ReadSaveIdFrom(binaryReader);
			this.ContainerType = (ContainerType)binaryReader.ReadByte();
			this.ElementCount = binaryReader.ReadInt();
		}
	}
}

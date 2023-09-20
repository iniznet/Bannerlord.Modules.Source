using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	public class ContainerHeaderLoadData
	{
		public int Id { get; private set; }

		public object Target { get; private set; }

		public LoadContext Context { get; private set; }

		public ContainerDefinition TypeDefinition { get; private set; }

		public SaveId SaveId { get; private set; }

		public int ElementCount { get; private set; }

		public ContainerType ContainerType { get; private set; }

		public ContainerHeaderLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
		}

		public bool GetObjectTypeDefinition()
		{
			this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this.SaveId) as ContainerDefinition;
			return this.TypeDefinition != null;
		}

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

		public void InitialieReaders(SaveEntryFolder saveEntryFolder)
		{
			BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Object)).GetBinaryReader();
			this.SaveId = SaveId.ReadSaveIdFrom(binaryReader);
			this.ContainerType = (ContainerType)binaryReader.ReadByte();
			this.ElementCount = binaryReader.ReadInt();
		}
	}
}

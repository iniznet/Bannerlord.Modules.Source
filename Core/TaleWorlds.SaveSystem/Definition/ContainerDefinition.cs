using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
	public class ContainerDefinition : TypeDefinitionBase
	{
		public Assembly DefinedAssembly { get; private set; }

		public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

		public bool HasNoChildObject { get; private set; }

		public ContainerDefinition(Type type, ContainerSaveId saveId, Assembly definedAssembly)
			: base(type, saveId)
		{
			this.DefinedAssembly = definedAssembly;
		}

		public void InitializeForAutoGeneration(CollectObjectsDelegate collectObjectsDelegate, bool hasNoChildObject)
		{
			this.CollectObjectsMethod = collectObjectsDelegate;
			this.HasNoChildObject = hasNoChildObject;
		}
	}
}

using System;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class EnumDefinition : TypeDefinitionBase
	{
		public EnumDefinition(Type type, SaveId saveId, IEnumResolver resolver)
			: base(type, saveId)
		{
			this.Resolver = resolver;
		}

		public EnumDefinition(Type type, int saveId, IEnumResolver resolver)
			: this(type, new TypeSaveId(saveId), resolver)
		{
		}

		public readonly IEnumResolver Resolver;
	}
}

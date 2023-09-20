using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
	public class SaveableObjectSystemTypeDefiner : SaveableTypeDefiner
	{
		public SaveableObjectSystemTypeDefiner()
			: base(10000)
		{
		}

		protected override void DefineBasicTypes()
		{
			base.DefineBasicTypes();
			base.AddBasicTypeDefinition(typeof(MBGUID), 1005, new MBGUIDBasicTypeSerializer());
		}

		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(MBObjectBase), 34, null);
		}

		protected override void DefineStructTypes()
		{
		}

		protected override void DefineEnumTypes()
		{
		}

		protected override void DefineInterfaceTypes()
		{
		}

		protected override void DefineRootClassTypes()
		{
		}

		protected override void DefineGenericClassDefinitions()
		{
		}

		protected override void DefineGenericStructDefinitions()
		{
		}

		protected override void DefineContainerDefinitions()
		{
		}
	}
}

using System;

namespace TaleWorlds.DotNet
{
	[EngineStruct("ftlNative_object_pointer")]
	internal struct NativeObjectPointer
	{
		public UIntPtr Pointer;

		public int TypeId;
	}
}

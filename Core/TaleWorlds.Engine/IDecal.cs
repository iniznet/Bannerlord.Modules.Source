using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001C RID: 28
	[ApplicationInterfaceBase]
	internal interface IDecal
	{
		// Token: 0x0600017A RID: 378
		[EngineMethod("get_material", false)]
		Material GetMaterial(UIntPtr decalPointer);

		// Token: 0x0600017B RID: 379
		[EngineMethod("set_material", false)]
		void SetMaterial(UIntPtr decalPointer, UIntPtr materialPointer);

		// Token: 0x0600017C RID: 380
		[EngineMethod("create_decal", false)]
		Decal CreateDecal(string name);

		// Token: 0x0600017D RID: 381
		[EngineMethod("get_factor_1", false)]
		uint GetFactor1(UIntPtr decalPointer);

		// Token: 0x0600017E RID: 382
		[EngineMethod("set_factor_1_linear", false)]
		void SetFactor1Linear(UIntPtr decalPointer, uint linearFactorColor1);

		// Token: 0x0600017F RID: 383
		[EngineMethod("set_factor_1", false)]
		void SetFactor1(UIntPtr decalPointer, uint factorColor1);

		// Token: 0x06000180 RID: 384
		[EngineMethod("set_vector_argument", false)]
		void SetVectorArgument(UIntPtr decalPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x06000181 RID: 385
		[EngineMethod("set_vector_argument_2", false)]
		void SetVectorArgument2(UIntPtr decalPointer, float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3);

		// Token: 0x06000182 RID: 386
		[EngineMethod("get_global_frame", false)]
		void GetFrame(UIntPtr decalPointer, ref MatrixFrame outFrame);

		// Token: 0x06000183 RID: 387
		[EngineMethod("set_global_frame", false)]
		void SetFrame(UIntPtr decalPointer, ref MatrixFrame decalFrame);

		// Token: 0x06000184 RID: 388
		[EngineMethod("create_copy", false)]
		Decal CreateCopy(UIntPtr pointer);
	}
}

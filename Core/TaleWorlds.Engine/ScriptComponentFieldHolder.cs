using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000054 RID: 84
	[EngineStruct("rglScript_component_field_holder")]
	internal struct ScriptComponentFieldHolder
	{
		// Token: 0x040000B1 RID: 177
		public MatrixFrame matrixFrame;

		// Token: 0x040000B2 RID: 178
		public Vec3 color;

		// Token: 0x040000B3 RID: 179
		public Vec3 v3;

		// Token: 0x040000B4 RID: 180
		public UIntPtr entityPointer;

		// Token: 0x040000B5 RID: 181
		public UIntPtr texturePointer;

		// Token: 0x040000B6 RID: 182
		public UIntPtr meshPointer;

		// Token: 0x040000B7 RID: 183
		public UIntPtr materialPointer;

		// Token: 0x040000B8 RID: 184
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string s;

		// Token: 0x040000B9 RID: 185
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string enumValue;

		// Token: 0x040000BA RID: 186
		public double d;

		// Token: 0x040000BB RID: 187
		public float f;

		// Token: 0x040000BC RID: 188
		public int b;

		// Token: 0x040000BD RID: 189
		public int i;
	}
}

﻿using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x0200001C RID: 28
	internal struct PixelFormatDescriptor
	{
		// Token: 0x04000080 RID: 128
		public ushort nSize;

		// Token: 0x04000081 RID: 129
		public ushort nVersion;

		// Token: 0x04000082 RID: 130
		public uint dwFlags;

		// Token: 0x04000083 RID: 131
		public byte iPixelType;

		// Token: 0x04000084 RID: 132
		public byte cColorBits;

		// Token: 0x04000085 RID: 133
		public byte cRedBits;

		// Token: 0x04000086 RID: 134
		public byte cRedShift;

		// Token: 0x04000087 RID: 135
		public byte cGreenBits;

		// Token: 0x04000088 RID: 136
		public byte cGreenShift;

		// Token: 0x04000089 RID: 137
		public byte cBlueBits;

		// Token: 0x0400008A RID: 138
		public byte cBlueShift;

		// Token: 0x0400008B RID: 139
		public byte cAlphaBits;

		// Token: 0x0400008C RID: 140
		public byte cAlphaShift;

		// Token: 0x0400008D RID: 141
		public byte cAccumBits;

		// Token: 0x0400008E RID: 142
		public byte cAccumRedBits;

		// Token: 0x0400008F RID: 143
		public byte cAccumGreenBits;

		// Token: 0x04000090 RID: 144
		public byte cAccumBlueBits;

		// Token: 0x04000091 RID: 145
		public byte cAccumAlphaBits;

		// Token: 0x04000092 RID: 146
		public byte cDepthBits;

		// Token: 0x04000093 RID: 147
		public byte cStencilBits;

		// Token: 0x04000094 RID: 148
		public byte cAuxBuffers;

		// Token: 0x04000095 RID: 149
		public byte iLayerType;

		// Token: 0x04000096 RID: 150
		public byte bReserved;

		// Token: 0x04000097 RID: 151
		public uint dwLayerMask;

		// Token: 0x04000098 RID: 152
		public uint dwVisibleMask;

		// Token: 0x04000099 RID: 153
		public uint dwDamageMask;
	}
}

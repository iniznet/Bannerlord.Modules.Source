using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000010 RID: 16
	[EngineClass("rglDecal")]
	public sealed class Decal : GameEntityComponent
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00002DE4 File Offset: 0x00000FE4
		internal Decal(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002DED File Offset: 0x00000FED
		public static Decal CreateDecal(string name = null)
		{
			return EngineApplicationInterface.IDecal.CreateDecal(name);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002DFA File Offset: 0x00000FFA
		public Decal CreateCopy()
		{
			return EngineApplicationInterface.IDecal.CreateCopy(base.Pointer);
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00002E0C File Offset: 0x0000100C
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002E1E File Offset: 0x0000101E
		public uint GetFactor1()
		{
			return EngineApplicationInterface.IDecal.GetFactor1(base.Pointer);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002E30 File Offset: 0x00001030
		public void SetFactor1Linear(uint linearFactorColor1)
		{
			EngineApplicationInterface.IDecal.SetFactor1Linear(base.Pointer, linearFactorColor1);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002E43 File Offset: 0x00001043
		public void SetFactor1(uint factorColor1)
		{
			EngineApplicationInterface.IDecal.SetFactor1(base.Pointer, factorColor1);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002E56 File Offset: 0x00001056
		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IDecal.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002E6D File Offset: 0x0000106D
		public void SetVectorArgument2(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IDecal.SetVectorArgument2(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002E84 File Offset: 0x00001084
		public Material GetMaterial()
		{
			return EngineApplicationInterface.IDecal.GetMaterial(base.Pointer);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002E96 File Offset: 0x00001096
		public void SetMaterial(Material material)
		{
			EngineApplicationInterface.IDecal.SetMaterial(base.Pointer, material.Pointer);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002EAE File Offset: 0x000010AE
		public void SetFrame(MatrixFrame Frame)
		{
			this.Frame = Frame;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002EB8 File Offset: 0x000010B8
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002EE0 File Offset: 0x000010E0
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.IDecal.GetFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
			set
			{
				EngineApplicationInterface.IDecal.SetFrame(base.Pointer, ref value);
			}
		}
	}
}

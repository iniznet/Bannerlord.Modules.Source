using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000E RID: 14
	[EngineClass("rglComposite_component")]
	public sealed class CompositeComponent : GameEntityComponent
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000029AD File Offset: 0x00000BAD
		internal CompositeComponent(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000029B6 File Offset: 0x00000BB6
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000029C8 File Offset: 0x00000BC8
		public static bool IsNull(CompositeComponent component)
		{
			return component == null || component.Pointer == UIntPtr.Zero;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000029E5 File Offset: 0x00000BE5
		public static CompositeComponent CreateCompositeComponent()
		{
			return EngineApplicationInterface.ICompositeComponent.CreateCompositeComponent();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000029F1 File Offset: 0x00000BF1
		public CompositeComponent CreateCopy()
		{
			return EngineApplicationInterface.ICompositeComponent.CreateCopy(base.Pointer);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002A03 File Offset: 0x00000C03
		public void AddComponent(GameEntityComponent component)
		{
			EngineApplicationInterface.ICompositeComponent.AddComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002A1B File Offset: 0x00000C1B
		public void AddPrefabEntity(string prefabName, Scene scene)
		{
			EngineApplicationInterface.ICompositeComponent.AddPrefabEntity(base.Pointer, scene.Pointer, prefabName);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002A34 File Offset: 0x00000C34
		public void Dispose()
		{
			if (this.IsValid)
			{
				this.Release();
				GC.SuppressFinalize(this);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002A4A File Offset: 0x00000C4A
		private void Release()
		{
			EngineApplicationInterface.ICompositeComponent.Release(base.Pointer);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002A5C File Offset: 0x00000C5C
		~CompositeComponent()
		{
			this.Dispose();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002A88 File Offset: 0x00000C88
		public uint GetFactor1()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFactor1(base.Pointer);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002A9A File Offset: 0x00000C9A
		public uint GetFactor2()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFactor2(base.Pointer);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002AAC File Offset: 0x00000CAC
		public void SetFactor1(uint factorColor1)
		{
			EngineApplicationInterface.ICompositeComponent.SetFactor1(base.Pointer, factorColor1);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002ABF File Offset: 0x00000CBF
		public void SetFactor2(uint factorColor2)
		{
			EngineApplicationInterface.ICompositeComponent.SetFactor2(base.Pointer, factorColor2);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002AD2 File Offset: 0x00000CD2
		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.ICompositeComponent.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002AE9 File Offset: 0x00000CE9
		public void SetMaterial(Material material)
		{
			EngineApplicationInterface.ICompositeComponent.SetMaterial(base.Pointer, material.Pointer);
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002B04 File Offset: 0x00000D04
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002B2C File Offset: 0x00000D2C
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.ICompositeComponent.GetFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
			set
			{
				EngineApplicationInterface.ICompositeComponent.SetFrame(base.Pointer, ref value);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002B40 File Offset: 0x00000D40
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002B52 File Offset: 0x00000D52
		public Vec3 VectorUserData
		{
			get
			{
				return EngineApplicationInterface.ICompositeComponent.GetVectorUserData(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ICompositeComponent.SetVectorUserData(base.Pointer, ref value);
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002B66 File Offset: 0x00000D66
		public void SetVisibilityMask(VisibilityMaskFlags visibilityMask)
		{
			EngineApplicationInterface.ICompositeComponent.SetVisibilityMask(base.Pointer, visibilityMask);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002B79 File Offset: 0x00000D79
		public override MetaMesh GetFirstMetaMesh()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFirstMetaMesh(base.Pointer);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002B8B File Offset: 0x00000D8B
		public void AddMultiMesh(string MultiMeshName)
		{
			EngineApplicationInterface.ICompositeComponent.AddMultiMesh(base.Pointer, MultiMeshName);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002B9E File Offset: 0x00000D9E
		public void SetVisible(bool visible)
		{
			EngineApplicationInterface.ICompositeComponent.SetVisible(base.Pointer, visible);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00002BB1 File Offset: 0x00000DB1
		public bool GetVisible()
		{
			return EngineApplicationInterface.ICompositeComponent.IsVisible(base.Pointer);
		}
	}
}

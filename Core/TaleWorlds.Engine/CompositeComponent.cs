using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineClass("rglComposite_component")]
	public sealed class CompositeComponent : GameEntityComponent
	{
		internal CompositeComponent(UIntPtr pointer)
			: base(pointer)
		{
		}

		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		public static bool IsNull(CompositeComponent component)
		{
			return component == null || component.Pointer == UIntPtr.Zero;
		}

		public static CompositeComponent CreateCompositeComponent()
		{
			return EngineApplicationInterface.ICompositeComponent.CreateCompositeComponent();
		}

		public CompositeComponent CreateCopy()
		{
			return EngineApplicationInterface.ICompositeComponent.CreateCopy(base.Pointer);
		}

		public void AddComponent(GameEntityComponent component)
		{
			EngineApplicationInterface.ICompositeComponent.AddComponent(base.Pointer, component.Pointer);
		}

		public void AddPrefabEntity(string prefabName, Scene scene)
		{
			EngineApplicationInterface.ICompositeComponent.AddPrefabEntity(base.Pointer, scene.Pointer, prefabName);
		}

		public void Dispose()
		{
			if (this.IsValid)
			{
				this.Release();
				GC.SuppressFinalize(this);
			}
		}

		private void Release()
		{
			EngineApplicationInterface.ICompositeComponent.Release(base.Pointer);
		}

		~CompositeComponent()
		{
			this.Dispose();
		}

		public uint GetFactor1()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFactor1(base.Pointer);
		}

		public uint GetFactor2()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFactor2(base.Pointer);
		}

		public void SetFactor1(uint factorColor1)
		{
			EngineApplicationInterface.ICompositeComponent.SetFactor1(base.Pointer, factorColor1);
		}

		public void SetFactor2(uint factorColor2)
		{
			EngineApplicationInterface.ICompositeComponent.SetFactor2(base.Pointer, factorColor2);
		}

		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.ICompositeComponent.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		public void SetMaterial(Material material)
		{
			EngineApplicationInterface.ICompositeComponent.SetMaterial(base.Pointer, material.Pointer);
		}

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

		public void SetVisibilityMask(VisibilityMaskFlags visibilityMask)
		{
			EngineApplicationInterface.ICompositeComponent.SetVisibilityMask(base.Pointer, visibilityMask);
		}

		public override MetaMesh GetFirstMetaMesh()
		{
			return EngineApplicationInterface.ICompositeComponent.GetFirstMetaMesh(base.Pointer);
		}

		public void AddMultiMesh(string MultiMeshName)
		{
			EngineApplicationInterface.ICompositeComponent.AddMultiMesh(base.Pointer, MultiMeshName);
		}

		public void SetVisible(bool visible)
		{
			EngineApplicationInterface.ICompositeComponent.SetVisible(base.Pointer, visible);
		}

		public bool GetVisible()
		{
			return EngineApplicationInterface.ICompositeComponent.IsVisible(base.Pointer);
		}
	}
}

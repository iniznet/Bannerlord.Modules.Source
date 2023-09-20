using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("int")]
	public struct PhysicsMaterial
	{
		internal PhysicsMaterial(int index)
		{
			this = default(PhysicsMaterial);
			this.Index = index;
		}

		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		public PhysicsMaterialFlags GetFlags()
		{
			return PhysicsMaterial.GetFlagsAtIndex(this.Index);
		}

		public float GetDynamicFriction()
		{
			return PhysicsMaterial.GetDynamicFrictionAtIndex(this.Index);
		}

		public float GetStaticFriction()
		{
			return PhysicsMaterial.GetStaticFrictionAtIndex(this.Index);
		}

		public float GetSoftness()
		{
			return PhysicsMaterial.GetSoftnessAtIndex(this.Index);
		}

		public float GetRestitution()
		{
			return PhysicsMaterial.GetRestitutionAtIndex(this.Index);
		}

		public string Name
		{
			get
			{
				return PhysicsMaterial.GetNameAtIndex(this.Index);
			}
		}

		public bool Equals(PhysicsMaterial m)
		{
			return this.Index == m.Index;
		}

		public static int GetMaterialCount()
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetMaterialCount();
		}

		public static PhysicsMaterial GetFromName(string id)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetIndexWithName(id);
		}

		public static string GetNameAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetMaterialNameAtIndex(index);
		}

		public static PhysicsMaterialFlags GetFlagsAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetFlagsAtIndex(index);
		}

		public static float GetRestitutionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetRestitutionAtIndex(index);
		}

		public static float GetSoftnessAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetSoftnessAtIndex(index);
		}

		public static float GetDynamicFrictionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetDynamicFrictionAtIndex(index);
		}

		public static float GetStaticFrictionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetStaticFrictionAtIndex(index);
		}

		public static PhysicsMaterial GetFromIndex(int index)
		{
			return new PhysicsMaterial(index);
		}

		public readonly int Index;

		public static readonly PhysicsMaterial InvalidPhysicsMaterial = new PhysicsMaterial(-1);
	}
}

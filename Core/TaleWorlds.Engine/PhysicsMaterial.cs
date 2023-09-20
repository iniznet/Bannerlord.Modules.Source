using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000072 RID: 114
	[EngineStruct("int")]
	public struct PhysicsMaterial
	{
		// Token: 0x060008A4 RID: 2212 RVA: 0x00008B1C File Offset: 0x00006D1C
		internal PhysicsMaterial(int index)
		{
			this = default(PhysicsMaterial);
			this.Index = index;
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00008B2C File Offset: 0x00006D2C
		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00008B3A File Offset: 0x00006D3A
		public PhysicsMaterialFlags GetFlags()
		{
			return PhysicsMaterial.GetFlagsAtIndex(this.Index);
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00008B47 File Offset: 0x00006D47
		public float GetDynamicFriction()
		{
			return PhysicsMaterial.GetDynamicFrictionAtIndex(this.Index);
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00008B54 File Offset: 0x00006D54
		public float GetStaticFriction()
		{
			return PhysicsMaterial.GetStaticFrictionAtIndex(this.Index);
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x00008B61 File Offset: 0x00006D61
		public float GetSoftness()
		{
			return PhysicsMaterial.GetSoftnessAtIndex(this.Index);
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00008B6E File Offset: 0x00006D6E
		public float GetRestitution()
		{
			return PhysicsMaterial.GetRestitutionAtIndex(this.Index);
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00008B7B File Offset: 0x00006D7B
		public string Name
		{
			get
			{
				return PhysicsMaterial.GetNameAtIndex(this.Index);
			}
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00008B88 File Offset: 0x00006D88
		public bool Equals(PhysicsMaterial m)
		{
			return this.Index == m.Index;
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x00008B98 File Offset: 0x00006D98
		public static int GetMaterialCount()
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetMaterialCount();
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x00008BA4 File Offset: 0x00006DA4
		public static PhysicsMaterial GetFromName(string id)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetIndexWithName(id);
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x00008BB1 File Offset: 0x00006DB1
		public static string GetNameAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetMaterialNameAtIndex(index);
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00008BBE File Offset: 0x00006DBE
		public static PhysicsMaterialFlags GetFlagsAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetFlagsAtIndex(index);
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x00008BCB File Offset: 0x00006DCB
		public static float GetRestitutionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetRestitutionAtIndex(index);
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x00008BD8 File Offset: 0x00006DD8
		public static float GetSoftnessAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetSoftnessAtIndex(index);
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00008BE5 File Offset: 0x00006DE5
		public static float GetDynamicFrictionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetDynamicFrictionAtIndex(index);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00008BF2 File Offset: 0x00006DF2
		public static float GetStaticFrictionAtIndex(int index)
		{
			return EngineApplicationInterface.IPhysicsMaterial.GetStaticFrictionAtIndex(index);
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x00008BFF File Offset: 0x00006DFF
		public static PhysicsMaterial GetFromIndex(int index)
		{
			return new PhysicsMaterial(index);
		}

		// Token: 0x04000150 RID: 336
		public readonly int Index;

		// Token: 0x04000151 RID: 337
		public static readonly PhysicsMaterial InvalidPhysicsMaterial = new PhysicsMaterial(-1);
	}
}

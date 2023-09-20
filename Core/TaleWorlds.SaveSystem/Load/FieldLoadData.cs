using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000035 RID: 53
	internal class FieldLoadData : MemberLoadData
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x00008DC3 File Offset: 0x00006FC3
		public FieldLoadData(ObjectLoadData objectLoadData, IReader reader)
			: base(objectLoadData, reader)
		{
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00008DD0 File Offset: 0x00006FD0
		public void FillObject()
		{
			FieldDefinition fieldDefinitionWithId;
			if (base.ObjectLoadData.TypeDefinition == null || (fieldDefinitionWithId = base.ObjectLoadData.TypeDefinition.GetFieldDefinitionWithId(base.MemberSaveId)) == null)
			{
				return;
			}
			FieldInfo fieldInfo = fieldDefinitionWithId.FieldInfo;
			object target = base.ObjectLoadData.Target;
			object dataToUse = base.GetDataToUse();
			if (dataToUse != null && !fieldInfo.FieldType.IsInstanceOfType(dataToUse) && !LoadContext.TryConvertType(dataToUse.GetType(), fieldInfo.FieldType, ref dataToUse))
			{
				return;
			}
			fieldInfo.SetValue(target, dataToUse);
		}
	}
}

using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	internal class FieldLoadData : MemberLoadData
	{
		public FieldLoadData(ObjectLoadData objectLoadData, IReader reader)
			: base(objectLoadData, reader)
		{
		}

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

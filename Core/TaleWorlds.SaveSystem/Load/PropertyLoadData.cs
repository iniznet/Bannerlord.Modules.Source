using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x0200003D RID: 61
	internal class PropertyLoadData : MemberLoadData
	{
		// Token: 0x06000236 RID: 566 RVA: 0x0000A0B0 File Offset: 0x000082B0
		public PropertyLoadData(ObjectLoadData objectLoadData, IReader reader)
			: base(objectLoadData, reader)
		{
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000A0BC File Offset: 0x000082BC
		public void FillObject()
		{
			PropertyDefinition propertyDefinitionWithId;
			if (base.ObjectLoadData.TypeDefinition == null || (propertyDefinitionWithId = base.ObjectLoadData.TypeDefinition.GetPropertyDefinitionWithId(base.MemberSaveId)) == null)
			{
				return;
			}
			MethodInfo setMethod = propertyDefinitionWithId.SetMethod;
			object target = base.ObjectLoadData.Target;
			object dataToUse = base.GetDataToUse();
			if (dataToUse != null && !propertyDefinitionWithId.PropertyInfo.PropertyType.IsInstanceOfType(dataToUse) && !LoadContext.TryConvertType(dataToUse.GetType(), propertyDefinitionWithId.PropertyInfo.PropertyType, ref dataToUse))
			{
				return;
			}
			setMethod.Invoke(target, new object[] { dataToUse });
		}
	}
}

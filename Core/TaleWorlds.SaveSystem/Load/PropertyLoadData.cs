using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	internal class PropertyLoadData : MemberLoadData
	{
		public PropertyLoadData(ObjectLoadData objectLoadData, IReader reader)
			: base(objectLoadData, reader)
		{
		}

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

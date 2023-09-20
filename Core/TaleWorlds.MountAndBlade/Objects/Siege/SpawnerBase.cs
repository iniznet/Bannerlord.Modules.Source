using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class SpawnerBase : ScriptComponentBehavior
	{
		protected internal override bool OnCheckForProblems()
		{
			return !this._spawnerEditorHelper.IsValid;
		}

		public virtual void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			Debug.FailedAssert("Please override 'AssignParameters' function in the derived class.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SpawnerBase.cs", "AssignParameters", 40);
		}

		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideName = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideNameForFireVersion = "";

		protected SpawnerEntityEditorHelper _spawnerEditorHelper;

		protected SpawnerEntityMissionHelper _spawnerMissionHelper;

		protected SpawnerEntityMissionHelper _spawnerMissionHelperFire;

		public class SpawnerPermissionField : EditorVisibleScriptComponentVariable
		{
			public SpawnerPermissionField()
				: base(false)
			{
			}
		}
	}
}

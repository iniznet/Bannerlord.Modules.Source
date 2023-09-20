using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Save
{
	public class SaveOutput
	{
		public GameData Data { get; private set; }

		public SaveResult Result { get; private set; }

		public SaveError[] Errors { get; private set; }

		public bool Successful
		{
			get
			{
				return this.Result == SaveResult.Success;
			}
		}

		public bool IsContinuing
		{
			get
			{
				Task<SaveResultWithMessage> continuingTask = this._continuingTask;
				return continuingTask != null && !continuingTask.IsCompleted;
			}
		}

		private SaveOutput()
		{
		}

		internal static SaveOutput CreateSuccessful(GameData data)
		{
			return new SaveOutput
			{
				Data = data,
				Result = SaveResult.Success
			};
		}

		internal static SaveOutput CreateFailed(IEnumerable<SaveError> errors, SaveResult result)
		{
			return new SaveOutput
			{
				Result = result,
				Errors = errors.ToArray<SaveError>()
			};
		}

		internal static SaveOutput CreateContinuing(Task<SaveResultWithMessage> continuingTask)
		{
			SaveOutput saveOutput = new SaveOutput();
			saveOutput._continuingTask = continuingTask;
			saveOutput._continuingTask.ContinueWith(delegate(Task<SaveResultWithMessage> t)
			{
				saveOutput.Result = t.Result.SaveResult;
			});
			return saveOutput;
		}

		public void PrintStatus()
		{
			Task<SaveResultWithMessage> continuingTask = this._continuingTask;
			if (continuingTask != null && continuingTask.IsCompleted)
			{
				this.Result = this._continuingTask.Result.SaveResult;
				this.Errors = new SaveError[0];
			}
			if (this.Result == SaveResult.Success)
			{
				Debug.Print("Successfully saved", 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			Debug.Print("Couldn't save because of errors listed below.", 0, Debug.DebugColor.White, 17592186044416UL);
			for (int i = 0; i < this.Errors.Length; i++)
			{
				SaveError saveError = this.Errors[i];
				Debug.Print(string.Concat(new object[] { "[", i, "]", saveError.Message }), 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.FailedAssert(string.Concat(new object[] { "SAVE FAILED: [", i, "]", saveError.Message, "\n" }), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Save\\SaveOutput.cs", "PrintStatus", 74);
			}
			Debug.Print("--------------------", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private Task<SaveResultWithMessage> _continuingTask;
	}
}

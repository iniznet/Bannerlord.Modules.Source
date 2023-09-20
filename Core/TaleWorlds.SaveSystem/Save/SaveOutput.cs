using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x0200002E RID: 46
	public class SaveOutput
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x000080E4 File Offset: 0x000062E4
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x000080EC File Offset: 0x000062EC
		public GameData Data { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x000080F5 File Offset: 0x000062F5
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x000080FD File Offset: 0x000062FD
		public SaveResult Result { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00008106 File Offset: 0x00006306
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000810E File Offset: 0x0000630E
		public SaveError[] Errors { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00008117 File Offset: 0x00006317
		public bool Successful
		{
			get
			{
				return this.Result == SaveResult.Success;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001AD RID: 429 RVA: 0x00008122 File Offset: 0x00006322
		public bool IsContinuing
		{
			get
			{
				Task<SaveResultWithMessage> continuingTask = this._continuingTask;
				return continuingTask != null && !continuingTask.IsCompleted;
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00008138 File Offset: 0x00006338
		private SaveOutput()
		{
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00008140 File Offset: 0x00006340
		internal static SaveOutput CreateSuccessful(GameData data)
		{
			return new SaveOutput
			{
				Data = data,
				Result = SaveResult.Success
			};
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00008155 File Offset: 0x00006355
		internal static SaveOutput CreateFailed(IEnumerable<SaveError> errors, SaveResult result)
		{
			return new SaveOutput
			{
				Result = result,
				Errors = errors.ToArray<SaveError>()
			};
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00008170 File Offset: 0x00006370
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

		// Token: 0x060001B2 RID: 434 RVA: 0x000081C0 File Offset: 0x000063C0
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

		// Token: 0x0400007F RID: 127
		private Task<SaveResultWithMessage> _continuingTask;
	}
}

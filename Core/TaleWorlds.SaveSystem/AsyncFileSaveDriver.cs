using System;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000002 RID: 2
	public class AsyncFileSaveDriver : ISaveDriver
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public AsyncFileSaveDriver()
		{
			this._saveDriver = new FileDriver();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		private void WaitPreviousTask()
		{
			Task currentNonSaveTask = this._currentNonSaveTask;
			Task<SaveResultWithMessage> currentSaveTask = this._currentSaveTask;
			if (currentNonSaveTask != null && !currentNonSaveTask.IsCompleted)
			{
				using (new PerformanceTestBlock("AsyncFileSaveDriver::Save - waiting previous save"))
				{
					currentNonSaveTask.Wait();
					return;
				}
			}
			if (currentSaveTask != null && !currentSaveTask.IsCompleted)
			{
				using (new PerformanceTestBlock("MBAsyncSaveDriver::Save - waiting previous save"))
				{
					currentSaveTask.Wait();
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
		Task<SaveResultWithMessage> ISaveDriver.Save(string saveName, int version, MetaData metaData, GameData gameData)
		{
			this.WaitPreviousTask();
			Task<SaveResultWithMessage> currentSaveTask;
			using (new PerformanceTestBlock("AsyncFileSaveDriver::Save"))
			{
				this._currentSaveTask = Task.Run<SaveResultWithMessage>(delegate
				{
					Task<SaveResultWithMessage> task2;
					using (new PerformanceTestBlock("AsyncFileSaveDriver::Save - Task itself"))
					{
						Task<SaveResultWithMessage> task = this._saveDriver.Save(saveName, version, metaData, gameData);
						this._currentNonSaveTask = null;
						task2 = task;
					}
					return task2;
				});
				currentSaveTask = this._currentSaveTask;
			}
			return currentSaveTask;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002168 File Offset: 0x00000368
		SaveGameFileInfo[] ISaveDriver.GetSaveGameFileInfos()
		{
			this.WaitPreviousTask();
			return this._saveDriver.GetSaveGameFileInfos();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000217B File Offset: 0x0000037B
		string[] ISaveDriver.GetSaveGameFileNames()
		{
			this.WaitPreviousTask();
			return this._saveDriver.GetSaveGameFileNames();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000218E File Offset: 0x0000038E
		MetaData ISaveDriver.LoadMetaData(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.LoadMetaData(saveName);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021A2 File Offset: 0x000003A2
		LoadData ISaveDriver.Load(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.Load(saveName);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021B6 File Offset: 0x000003B6
		bool ISaveDriver.Delete(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.Delete(saveName);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021CA File Offset: 0x000003CA
		bool ISaveDriver.IsSaveGameFileExists(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.IsSaveGameFileExists(saveName);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021DE File Offset: 0x000003DE
		bool ISaveDriver.IsWorkingAsync()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		private FileDriver _saveDriver;

		// Token: 0x04000002 RID: 2
		private Task _currentNonSaveTask;

		// Token: 0x04000003 RID: 3
		private Task<SaveResultWithMessage> _currentSaveTask;
	}
}

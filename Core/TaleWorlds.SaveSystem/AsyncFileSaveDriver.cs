using System;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	public class AsyncFileSaveDriver : ISaveDriver
	{
		public AsyncFileSaveDriver()
		{
			this._saveDriver = new FileDriver();
		}

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

		SaveGameFileInfo[] ISaveDriver.GetSaveGameFileInfos()
		{
			this.WaitPreviousTask();
			return this._saveDriver.GetSaveGameFileInfos();
		}

		string[] ISaveDriver.GetSaveGameFileNames()
		{
			this.WaitPreviousTask();
			return this._saveDriver.GetSaveGameFileNames();
		}

		MetaData ISaveDriver.LoadMetaData(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.LoadMetaData(saveName);
		}

		LoadData ISaveDriver.Load(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.Load(saveName);
		}

		bool ISaveDriver.Delete(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.Delete(saveName);
		}

		bool ISaveDriver.IsSaveGameFileExists(string saveName)
		{
			this.WaitPreviousTask();
			return this._saveDriver.IsSaveGameFileExists(saveName);
		}

		bool ISaveDriver.IsWorkingAsync()
		{
			return true;
		}

		private FileDriver _saveDriver;

		private Task _currentNonSaveTask;

		private Task<SaveResultWithMessage> _currentSaveTask;
	}
}

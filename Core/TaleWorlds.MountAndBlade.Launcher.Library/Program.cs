using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.Starter.Library;
using TaleWorlds.TwoDimension.Standalone;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000008 RID: 8
	public class Program
	{
		// Token: 0x0600003D RID: 61 RVA: 0x00002934 File Offset: 0x00000B34
		static Program()
		{
			Common.PlatformFileHelper = new PlatformFileHelperPC("Mount and Blade II Bannerlord");
			Debug.DebugManager = new LauncherDebugManager();
			AppDomain.CurrentDomain.AssemblyResolve += Program.OnAssemblyResolve;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000296F File Offset: 0x00000B6F
		public static void NativeMain(string commandLine)
		{
			Program._isTestMode = commandLine.ToLower().Contains("/runtest");
			Program.Main(commandLine.Split(Array.Empty<char>()).ToArray<string>());
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000299C File Offset: 0x00000B9C
		public static void Main(string[] args)
		{
			Program._args = args.ToList<string>();
			if (!Program._isTestMode)
			{
				try
				{
					Common.PlatformFileHelper = new PlatformFileHelperPC("Mount and Blade II Bannerlord");
					Common.SetInvariantCulture();
					LauncherPlatform.Initialize();
					ResourceDepot resourceDepot = new ResourceDepot();
					resourceDepot.AddLocation(BasePath.Name, "Modules/Native/LauncherGUI/");
					resourceDepot.CollectResources();
					resourceDepot.StartWatchingChangesInDepot();
					string text = "M&B II: Bannerlord";
					Program._graphicsForm = new GraphicsForm(1154, 701, resourceDepot, true, true, true, text);
					Program._windowsFramework = new WindowsFramework();
					Program._windowsFramework.ThreadConfig = WindowsFrameworkThreadConfig.NoThread;
					Program._standaloneUIDomain = new StandaloneUIDomain(Program._graphicsForm, resourceDepot);
					Program._windowsFramework.Initialize(new FrameworkDomain[] { Program._standaloneUIDomain });
					Program._windowsFramework.RegisterMessageCommunicator(Program._graphicsForm);
					Program._windowsFramework.Start();
					LauncherPlatform.Destroy();
					goto IL_101;
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
					throw;
				}
			}
			Program._gameStarted = true;
			IL_101:
			if (Program._gameStarted)
			{
				Program.Main(Program._args.ToArray());
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002AD4 File Offset: 0x00000CD4
		public static void StartGame()
		{
			Program._additionalArgs = Program._standaloneUIDomain.AdditionalArgs;
			Program._args.Add(Program._additionalArgs);
			Program._hasUnofficialModulesSelected = Program._standaloneUIDomain.HasUnofficialModulesSelected;
			Program._gameStarted = true;
			Program.AuxFinalize();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002B0E File Offset: 0x00000D0E
		public static void StartDigitalCompanion()
		{
			Program.AuxFinalize();
			Process.Start(new ProcessStartInfo("..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe"));
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002B28 File Offset: 0x00000D28
		private static void AuxFinalize()
		{
			Program._windowsFramework.UnRegisterMessageCommunicator(Program._graphicsForm);
			Program._graphicsForm.Destroy();
			Program._windowsFramework.Stop();
			Program._windowsFramework = null;
			Program._graphicsForm = null;
			LauncherDebugManager launcherDebugManager = Debug.DebugManager as LauncherDebugManager;
			if (launcherDebugManager != null)
			{
				launcherDebugManager.OnFinalize();
			}
			User32.SetForegroundWindow(Kernel32.GetConsoleWindow());
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002B84 File Offset: 0x00000D84
		private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			Debug.Print("Resolving: " + args.Name, 0, Debug.DebugColor.White, 17592186044416UL);
			if (args.Name.Contains("ManagedStarter"))
			{
				return Assembly.LoadFrom(Program.StarterExecutable);
			}
			return null;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002BD0 File Offset: 0x00000DD0
		public static bool IsDigitalCompanionAvailable()
		{
			return File.Exists("..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe");
		}

		// Token: 0x04000021 RID: 33
		private static string StarterExecutable = "Bannerlord.exe";

		// Token: 0x04000022 RID: 34
		private const string _pathToDigitalCompanionExe = "..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe";

		// Token: 0x04000023 RID: 35
		private static WindowsFramework _windowsFramework;

		// Token: 0x04000024 RID: 36
		private static GraphicsForm _graphicsForm;

		// Token: 0x04000025 RID: 37
		private static List<string> _args;

		// Token: 0x04000026 RID: 38
		private static string _additionalArgs;

		// Token: 0x04000027 RID: 39
		private static bool _hasUnofficialModulesSelected;

		// Token: 0x04000028 RID: 40
		private static StandaloneUIDomain _standaloneUIDomain;

		// Token: 0x04000029 RID: 41
		private static bool _isTestMode;

		// Token: 0x0400002A RID: 42
		private static bool _gameStarted;
	}
}

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
	public class Program
	{
		static Program()
		{
			Common.PlatformFileHelper = new PlatformFileHelperPC("Mount and Blade II Bannerlord");
			Debug.DebugManager = new LauncherDebugManager();
			AppDomain.CurrentDomain.AssemblyResolve += Program.OnAssemblyResolve;
		}

		public static void NativeMain(string commandLine)
		{
			Program._isTestMode = commandLine.ToLower().Contains("/runtest");
			Program.Main(commandLine.Split(Array.Empty<char>()).ToArray<string>());
		}

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
					LauncherPlatform.SetLauncherMode(true);
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
					LauncherPlatform.SetLauncherMode(false);
					LauncherPlatform.Destroy();
					goto IL_10D;
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
					throw;
				}
			}
			Program._gameStarted = true;
			IL_10D:
			if (Program._gameStarted)
			{
				LauncherPlatform.SetLauncherMode(false);
				Program.Main(Program._args.ToArray());
			}
		}

		public static void StartGame()
		{
			Program._additionalArgs = Program._standaloneUIDomain.AdditionalArgs;
			Program._args.Add(Program._additionalArgs);
			Program._hasUnofficialModulesSelected = Program._standaloneUIDomain.HasUnofficialModulesSelected;
			Program._gameStarted = true;
			Program.AuxFinalize();
		}

		public static void StartDigitalCompanion()
		{
			Program.AuxFinalize();
			Process.Start(new ProcessStartInfo("..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe"));
		}

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

		private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			Debug.Print("Resolving: " + args.Name, 0, Debug.DebugColor.White, 17592186044416UL);
			if (args.Name.Contains("ManagedStarter"))
			{
				return Assembly.LoadFrom(Program.StarterExecutable);
			}
			return null;
		}

		public static bool IsDigitalCompanionAvailable()
		{
			return File.Exists("..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe");
		}

		private static string StarterExecutable = "Bannerlord.exe";

		private const string _pathToDigitalCompanionExe = "..\\..\\DigitalCompanion\\Mount & Blade II Bannerlord - Digital Companion.exe";

		private static WindowsFramework _windowsFramework;

		private static GraphicsForm _graphicsForm;

		private static List<string> _args;

		private static string _additionalArgs;

		private static bool _hasUnofficialModulesSelected;

		private static StandaloneUIDomain _standaloneUIDomain;

		private static bool _isTestMode;

		private static bool _gameStarted;
	}
}

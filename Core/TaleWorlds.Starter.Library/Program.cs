using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace TaleWorlds.Starter.Library
{
	// Token: 0x02000002 RID: 2
	public class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		private static int Starter()
		{
			try
			{
				Assembly.LoadFrom("TaleWorlds.DotNet.dll").GetType("TaleWorlds.DotNet.Controller").GetMethod("SetEngineMethodsAsDotNet")
					.Invoke(null, new object[]
					{
						new Program.ControllerDelegate(MBDotNet.PassControllerMethods),
						new Program.InitializerDelegate(MBDotNet.PassManagedInitializeMethodPointerDotNet),
						new Program.InitializerDelegate(MBDotNet.PassManagedEngineCallbackMethodPointersDotNet)
					});
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("Exception: " + ex);
				Console.WriteLine("Fusion Log: " + ex.FusionLog);
				Console.WriteLine("Exception detailed: " + ex.ToString());
				if (ex.InnerException != null)
				{
					Console.WriteLine("Inner Exception: " + ex.InnerException);
				}
				Console.WriteLine("Press a key to continue...");
				Console.ReadKey();
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Exception: " + ex2);
				if (ex2.InnerException != null)
				{
					Console.WriteLine("Inner Exception: " + ex2.InnerException);
				}
				Console.WriteLine("Press a key to continue...");
				Console.ReadKey();
			}
			string text = "";
			for (int i = 0; i < Program._args.Length; i++)
			{
				string text2 = Program._args[i];
				text += text2;
				if (i + 1 < Program._args.Length)
				{
					text += " ";
				}
			}
			MBDotNet.SetCurrentDirectory(Directory.GetCurrentDirectory());
			return MBDotNet.WotsMainDotNet(text);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021CC File Offset: 0x000003CC
		[STAThread]
		public static int Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			Program._args = args;
			return Program.Starter();
		}

		// Token: 0x04000001 RID: 1
		private static string[] _args;

		// Token: 0x02000004 RID: 4
		// (Invoke) Token: 0x0600000A RID: 10
		private delegate void ControllerDelegate(Delegate currentDomainInitializer);

		// Token: 0x02000005 RID: 5
		// (Invoke) Token: 0x0600000E RID: 14
		private delegate void InitializerDelegate(Delegate argument);

		// Token: 0x02000006 RID: 6
		// (Invoke) Token: 0x06000012 RID: 18
		private delegate void StartMethodDelegate(string args);
	}
}

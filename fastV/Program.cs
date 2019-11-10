using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace fastV {
	class Program {
		// enum LogLevel{
		// 	verbose,
		// 	debug,
		// 	Info,
		// 	Error,
		// 	Fatal
		// }
		static string cc = "msvc";
		static string git = "";
		static string datadir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
		static string vdir = Path.Combine(datadir, "v");

		static int Measure(string binPath, string cmd) {
			// returns milliseconds
			P("Warming up...");
			foreach (int i in Enumerable.Range(0, 4)) {
				Exec(binPath, cmd);
			}

			P("Building...");
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Exec(binPath, cmd);
			sw.Stop();
			if (sw.ElapsedMilliseconds > int.MaxValue) { return int.MaxValue; }
			return Convert.ToInt32(sw.ElapsedMilliseconds);
		}
		static string Exec(string binPath, string cmd) {
			/* if (!File.Exists(binPath) && !File.Exists(binPath + ".exe")) {
				P($"can´t find: \"{binPath}\"", ConsoleColor.Red, "ERROR");
				return "";
			} */
			using(Process p = new Process()) {
				ProcessStartInfo psi = new ProcessStartInfo() {
					WindowStyle = ProcessWindowStyle.Hidden,
						RedirectStandardError = true,
						RedirectStandardOutput = true,
						CreateNoWindow = true,

						FileName = binPath,
						Arguments = cmd
				};

				P($"bin: \"{binPath}\" - cmd: \"{psi.Arguments}\"");
				p.StartInfo = psi;
				try {
					p.Start();
					p.WaitForExit();
					string tmp = (p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd()).Trim();
					if (p.ExitCode != 0) {
						P($"bin: \"{binPath}\" - bad exitcode: \"{psi.Arguments}\"", ConsoleColor.Red, "ERROR");
						P($"bin: \"{binPath}\" - bad exitcode: \"{tmp}\"", ConsoleColor.Red, "ERROR");
						return tmp;
					}
					P($"bin: \"{binPath}\" - output: \"{tmp}\" - Exitcode: {p.ExitCode}");
					return tmp;
				} catch (Exception ex) {
					P($"bin: \"{binPath}\" - \"{ex.Message}\": \"{psi.Arguments}\"", ConsoleColor.Red, "ERROR");
					throw ex;
					//return ex.Message;
				}
			}
		}
		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		static void Application_Start() {
			string p1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string p2 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string p3 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			string p4 = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
			string p5 = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
			string p6 = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).DirectoryName;
			string p7 = AppDomain.CurrentDomain.BaseDirectory;
			P("p1 = " + p1);
			P("p2 = " + p2);
			P("p3 = " + p3);
			P("p4 = " + p4);
			P("p5 = " + p5);
			P("p6 = " + p6);
			P("p7 = " + p7);
		}
		static void PrintInfo() {

			Console.WriteLine($"{nameof(RuntimeInformation.FrameworkDescription)}: {RuntimeInformation.FrameworkDescription}");
			Console.WriteLine($"{nameof(RuntimeInformation.OSArchitecture)}: {RuntimeInformation.OSArchitecture}");
			Console.WriteLine($"{nameof(RuntimeInformation.OSDescription)}: {RuntimeInformation.OSDescription}");
			Console.WriteLine($"{nameof(RuntimeInformation.ProcessArchitecture)}: {RuntimeInformation.ProcessArchitecture}");
			// Show the path where the CLR was loaded from.
			Console.WriteLine("Runtime directory: {0}", RuntimeEnvironment.GetRuntimeDirectory());

			// Show the CLR's version number.
			Console.WriteLine("System version: {0}", RuntimeEnvironment.GetSystemVersion());

			Console.WriteLine(Environment.CurrentDirectory);
			Application_Start();
			Console.WriteLine(String.Join("\n", Environment.GetCommandLineArgs()));
			Console.WriteLine(Environment.MachineName);
			Console.WriteLine(Environment.UserName);
			Console.WriteLine(Environment.UserDomainName);
			Console.WriteLine(Environment.ProcessorCount);

		}
		static void setupFreshV() {

		}

		static void P(string msg, ConsoleColor c = ConsoleColor.Blue, string typ = "Debug") {
			Console.Write($"[{DateTime.Now.ToString("dd.MM.yyyy - H:mm:ss")}][");
			Console.ForegroundColor = c;
			Console.Write(typ);
			Console.ResetColor();
			Console.Write($"] {msg}\n");
		}

		// bin pathToDataDir pathToV_Dir 
		// example:
		// dotnet "run" "--project" "fastV.csproj" -- c:/dev/fastV c:/dev/v clang
		// or
		// fastV_bin ./data /home/user/v gcc
		/* 
		dotnet run --project "c:\dev\dotnet\ae_git\fastV\fastV\fastV.csproj" -- "c:/dev/dotnet/ae_git/fastV/fast" "c:/dev/v_stuff/v" clang
 
 		dotnet run --project "c:\dev\dotnet\ae_git\fastV\fastV\fastV.csproj" -- "c:/dev/dotnet/ae_git/fastV/fast" "c:/dev/v_stuff/v" gcc
 
 		dotnet run --project "c:\dev\dotnet\ae_git\fastV\fastV\fastV.csproj" -- "c:/dev/dotnet/ae_git/fastV/fast" "c:/dev/v_stuff/v" msvc
		 */


		//clang -dumpversion
		//clang --version
		//gcc -dumpfullversion
		//gcc -dumpversion
		static void Main(string[] args) {
			string[] tmp = Where.Find("git");
			foreach (string item in tmp) { Console.WriteLine(Exec(item, "--version")); }
			P(string.Join(", ", tmp));
			tmp = Where.Find("v");
			P(string.Join(", ", tmp));
			P(tmp.Length.ToString());
			tmp = Where.Find(cc);
			P(string.Join(", ", tmp));
			P(tmp.Length.ToString());
			if (tmp.Length > 0) { P(Exec(tmp[0], "-dumpversion")); }
			Console.WriteLine(String.Join("\n", Environment.GetCommandLineArgs()));
			// Console.WriteLine(String.Join("\n", args));
			args = Environment.GetCommandLineArgs();
			// P(args.Length.ToString());
			if (args.Length > 1) {
				if (Directory.Exists(args[1])) { datadir = args[1]; }
			}
			if (args.Length > 2) {
				P(vdir);
				P(String.Join("\n", args));
				string tmp_vlib = Path.Combine(args[2], "vlib");
				if (Directory.Exists(args[2]) &&
					Directory.Exists(tmp_vlib) &&
					Directory.Exists(Path.Combine(tmp_vlib, "compiler"))) {
					vdir = args[2];
				}

				P(vdir);
			}
			if (args.Length > 3) {
				tmp = Where.Find(args[2]);
				P(string.Join(", ", tmp));
				P(tmp.Length.ToString());
				if (tmp.Length > 0) { P(Exec(tmp[0], "-dumpversion")); }
				cc = args[3];
			}
			if (!Directory.Exists(datadir)) { Directory.CreateDirectory(datadir); }
			if (!Directory.Exists(vdir)) { Directory.CreateDirectory(vdir); }
			// P(Directory.GetFiles(vdir).Length.ToString());

			P(DateTime.Now.ToString("HH:mm:ss")+" start ... ");
			//PrintInfo();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				// C:/Windows/System32/where.exe
				// C:/Windows/System32/bash.exe
				// C:/Windows/System32/wsl.exe
				// C:/Program Files/Git/cmd/git.exe
				// C:\WINDOWS\system32
				// Console.WriteLine(Exec("C:/Windows/system32/cmd.exe", "/c which git && where git"));
				// Console.WriteLine(Exec("C:/WINDOWS/system32/cmd.exe", "/c which git && where git"));
				// Console.WriteLine(Exec("cmd.exe", "/c which git && where git"));
				// Console.WriteLine(Exec("C:/Program Files/Git/cmd/git.exe", "--version"));
				git = "C:/Program Files/Git/cmd/git.exe";
			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				//
				// Console.WriteLine(Exec("/bin/bash", "which git"));
				// Console.WriteLine(Exec("/usr/bin/git", "--version"));
				git = "/usr/bin/git";
			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {

			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {

			}

			// Console.WriteLine();
			// Console.WriteLine("Press a key to close...");
			// Console.ReadKey();

			P("fast.html generator\n");
			// Fetch the last commit"s hash
			P("Fetching updates...");
			Exec(git, $"-C {vdir} pull --rebase");
			string commit_hash = Exec(git, $"-C {vdir} rev-parse HEAD");
			commit_hash = commit_hash[..7];
			if (!File.Exists($"{datadir}/table.html")) {
				File.WriteAllText($"{datadir}/table.html", "");
			}
			string table = File.ReadAllText($"{datadir}/table.html");
			// Do nothing if it"s already been processed.
			if (table.Contains($"{commit_hash}_{cc}")) {
				Console.WriteLine($"Commit {commit_hash}_{cc} has already been processed");
				return;
			}
			// Build an debug V
			P("Building vdebug...");
			Exec($"{vdir}/v", $"-o {vdir}/vdebug -cc {cc} -debug -show_c_cmd {vdir}/v.v");
			P($"{vdir}/v -o {vdir}/vdebug -cc {cc} -debug -show_c_cmd {vdir}/v.v");
			// Build an optimized V
			P("Building vprod...");
			Exec($"{vdir}/v", $"-o {vdir}/vprod -cc {cc} -prod {vdir}/v.v");
			P("Measuring...");
			int diff1 = Measure($"{vdir}/vprod", $"-cc {cc} -o v.c {vdir}/v.v");
			Console.WriteLine($"Building V took {diff1}ms");
			int diff2 = Measure($"{vdir}/vprod", $"-cc {cc} -o v2 {vdir}/v.v");
			Console.WriteLine($"Building V took {diff2}ms");
			int diff3 = Measure($"{vdir}/vprod", $"-o v2 -fast {vdir}/v.v");
			Console.WriteLine($"Building V took {diff3}ms");
			int diff4 = Measure($"{vdir}/vprod", $"-cc {cc} {vdir}/examples/hello_world.v");
			Console.WriteLine($"Building V took {diff4}ms");
			string commit_date = Exec(git, $"-C {vdir} log -n1 --pretty=\"format:%at\"");
			string message = Exec(git, $"-C {vdir} log -n1 --pretty=\"format:%s\"");
			DateTime date = UnixTimeStampToDateTime(Convert.ToInt32(commit_date));
			//FileStream outt = File.Create("table.html");
			// Place the new row on top
			table = $"<tr>\n<td>{date.ToString("dd.MM.yyyy - H:mm:ss")}</td>\n<td><a target=_blank href=\"https://github.com/vlang/v/commit/{commit_hash}\">{commit_hash}_{cc}</a></td>\n<td>{message}</td>\n<td>{diff1}ms</td>\n<td>{diff2}ms</td>\n<td>{diff3}ms</td>\n<td>{diff4}ms</td>\n</tr>\n" + table.Trim();
			File.WriteAllText($"{datadir}/table.html", table);
			// Regenerate index.html
			//string header = File.ReadAllText($"{datadir}/header.html");
			File.WriteAllText($"{datadir}/index.html", Header.raw + table);
		}
	}
}
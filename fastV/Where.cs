using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace fastV {
	class Where {
		static public string[] Find(string bin) {
			List<string> l = new List<string>();
			string p = Environment.GetEnvironmentVariable("PATH");
			string[] pArr;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				pArr = p.Split(';');
			} else {
				pArr = p.Split(':');
			}
			//Console.WriteLine(string.Join(", ", pArr));
			foreach (string s in pArr) {
				if (!Directory.Exists(s)) {
					// Console.WriteLine($"bad path entry: \"{s}\"");
					continue;
				}
				foreach (string item in Directory.GetFiles(s)) {
					string fp = Path.GetFullPath(item);
					if (l.Contains(fp)) { continue; }
					string n = Path.GetFileNameWithoutExtension(item);
					if (n == bin || n + Path.GetExtension(item) == bin) {
						l.Add(fp);
					}
				}
			}
			return l.ToArray();
		}
	}
}
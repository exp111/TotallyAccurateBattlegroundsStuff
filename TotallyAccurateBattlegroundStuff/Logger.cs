﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotallyAccurateBattlegroundsStuff
{
	public static class Logger
	{
		public static readonly string _fileCurrent = Directory.GetCurrentDirectory() + "/TABGStuff.log";

		public static readonly string _fileOld = Directory.GetCurrentDirectory() + "/TABGStuff.old.log";

		static Logger()
		{
			bool flag = File.Exists(_fileOld);
			if (flag)
			{
				File.Delete(_fileOld);
			}
			bool flag2 = File.Exists(_fileCurrent);
			if (flag2)
			{
				File.Move(_fileCurrent, _fileOld);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000026C7 File Offset: 0x000008C7
		private static void Log(object log, ConsoleColor color = ConsoleColor.White, string prefix = "[LOG]")
		{
			log = prefix + " TABGStuff  >> " + log;
			File.AppendAllText(_fileCurrent, log + Environment.NewLine);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000059DC File Offset: 0x00003BDC
		public static void Log(object log)
		{
			Log(log);
		}
	}
}
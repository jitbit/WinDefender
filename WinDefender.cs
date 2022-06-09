using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jitbit.Utils
{
	public class WinDefender
	{
		private static bool _isDefenderAvailable;
		private static string _defenderPath;
		private static SemaphoreSlim _lock = new SemaphoreSlim(5); //limit to 5 concurrent checks at a time

		//static ctor
		static WinDefender()
		{
			if (OperatingSystem.IsWindows())
			{
				_defenderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Defender", "MpCmdRun.exe");
				_isDefenderAvailable = File.Exists(_defenderPath);
			}
			else
				_isDefenderAvailable = false;
		}

		public static async Task<bool> IsVirus(byte[] file)
		{
			if (!_isDefenderAvailable) return false;

			string path = Path.GetTempFileName();
			await File.WriteAllBytesAsync(path, file); //save temp file

			await _lock.WaitAsync();

			try
			{
				using (var process = Process.Start(_defenderPath, $"-Scan -ScanType 3 -File \"{path}\" -DisableRemediation"))
				{
					if (process == null)
					{
						_isDefenderAvailable = false; //disable future attempts
						throw new InvalidOperationException("Failed to start MpCmdRun.exe");
					}

					try
					{
						await process.WaitForExitAsync().WaitAsync(TimeSpan.FromMilliseconds(2500));
					}
					catch (TimeoutException ex) //timeout
					{
						process.Kill();
						throw new TimeoutException("Timeout waiting for MpCmdRun.exe to return", ex);
					}

					return process.ExitCode == 2;
				}
			}
			finally
			{
				_lock.Release();
				File.Delete(path); //cleanup temp file
			}
		}
	}
}

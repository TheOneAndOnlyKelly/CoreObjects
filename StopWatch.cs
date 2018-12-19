using System;
using System.Runtime.InteropServices;

namespace CoreObjects
{
	public class StopWatch
	{

		//Code-timing class by Alastair Dallas 11/25/02
		//Based on Microsoft KB Article 172338 and 306978
		//
		//Use Kernel32 functions for near-microsecond timing of processes
		//
		//Usage:
		//
		//       Dim swatch as New Stopwatch("My process")
		//       ...perform My process...
		//       swatch.ReportToConsole()
		//
		//       Output: "My process (0.332131039000767 seconds)"
		//
		//   Alternative:
		//
		//       Dim firstResult, secondResult as Double
		//       Dim swatch as New Stopwatch()
		//       ...perform process...
		//       swatch.Done()
		//       firstResult = swatch.ElapsedTime()
		//       swatch.Start()
		//       ...more process work...
		//       swatch.Done()
		//       secondResult = swatch.ElapsedTime()
		//       Console.WriteLine("First took {0} seconds and Second took {1} seconds.",
		//           firstResult, secondResult)
		//
		//       Output: "First took 0.0131530683368976 seconds and Second took 8.8372527793337 seconds."

		[DllImport("Kernel32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern short QueryPerformanceCounter(ref long X);
		[DllImport("Kernel32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern short QueryPerformanceFrequency(ref long X);

		private long _start;
		private long _end;
		private long _freq;
		private long _overhead;
		private string _desc;

		public StopWatch()
			: this(string.Empty)
		{
		}

		public StopWatch(string description)
		{
			if (QueryPerformanceCounter(ref _start) == 0)
			{
				throw (new StopwatchException());
			}
			if (QueryPerformanceCounter(ref _end) == 0)
			{
				throw (new StopwatchException());
			}
			// calculate the time just to call start and end
			_overhead = _end - _start;
			this.Start(description);
		}

		public void Start(string description)
		{
			_desc = description;
			QueryPerformanceFrequency(ref _freq);
			_end = 0;
			if (QueryPerformanceCounter(ref _start) == 0)
			{
				throw (new StopwatchException());
			}
		}

		public double Done()
		{
			if (QueryPerformanceCounter(ref _end) == 0)
			{
				throw (new StopwatchException());
			}
			return this.ElapsedTime();
		}

		public double ElapsedTime()
		{
			if (_end == 0)
			{
				long RightNow = 0;
				if (QueryPerformanceCounter(ref RightNow) == 0)
				{
					throw (new StopwatchException());
				}
				return (((RightNow - _start) - _overhead) / _freq);
			}
			return (((_end - _start) - _overhead) / _freq);
		}

		public void ReportToConsole()
		{
			if (_end == 0)
			{
				this.Done();
			}
			Console.WriteLine(_desc + " ({0} seconds)", this.ElapsedTime());
		}

	}

	public class StopwatchException : ApplicationException
	{
		override public string Message
		{
			get
			{
				return "Stopwatch: QueryPerformanceCounter[Kernel32] returned 0";
			}
		}
	}
}

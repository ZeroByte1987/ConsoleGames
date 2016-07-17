using System;
using System.Diagnostics;
using ZFC.Strings;



namespace ZFC
{
	/// <summary>
	/// This class defines a set methods for checking different statistic data, like time and memory consuming.
	/// </summary>
	public class ZStat
	{
		//	Fields & Properties
		#region
		private bool			_timerOnly;
		private PerformanceCounter	Cnt;
		private long			Init_Ticks;
		private long			Init_Mem;
		private long			Excess;
		private bool			LastOpTime;

		/// <summary>
		/// Total amount of memory used by this process in MegaBytes.
		/// </summary>
		public int				Memory_UsedMB	{	get	{	if (_timerOnly)	return -1;	return (int)(Environment.WorkingSet / 1048576);	}}
		/// <summary>
		/// Total amount of free system memory in MegaBytes.
		/// </summary>
		public int				Memory_FreeMB	{	get {	if (_timerOnly)	return -1;	return (int)(Cnt.RawValue / 1048576);	}}

		/// <summary>
		/// List of time states.
		/// </summary>
		public TimeStateList	TimeStates;
		/// <summary>
		/// List of memory states.
		/// </summary>
		public MemStateList		MemStates;
		/// <summary>
		/// List of timers.
		/// </summary>
		public TimeStateList	Timers;
		#endregion

		//	Methods for statistics checking
		#region
		/// <summary>
		/// Check current statistics and update data.
		/// </summary>
		public void			CheckPoint()
		{
			CheckPoint("");
		}
		/// <summary>
		/// Check current statistics and update data.
		/// </summary>
		/// <param name="Descr">Description of current state.</param>
		public void			CheckPoint(string Descr)
		{
			long CT	= DateTime.Now.Ticks;
			var A = new TimeStat();
			A.T_Ticks = CT;
			if (TimeStates.Count == 0)	A.Ticks	= CT-Init_Ticks;	
			else	A.Ticks	= CT - TimeStates.Last.T_Ticks - Excess;
			A.Descr	= Descr;
			TimeStates.Add(A);
			LastOpTime = true;
		}

		/// <summary>
		///	Start a timer with given name.
		/// </summary>
		/// <param name="TimerName">Timer name.</param>
		public void			StartTimer(string TimerName)
		{
			var A = Timers.GetElement(TimerName);
			if (A == null)	A = new TimeStat();
			if (A.Stopped)	A.Stopped = false;
			A.Descr = TimerName;
			A.T_Ticks	= DateTime.Now.Ticks;
			if (!Timers.ContainsKey(TimerName))		Timers.Add(A);
		}
		/// <summary>
		/// Stop started timer. If there was no timer started, this will do nothing.
		/// </summary>
		/// <param name="TimerName">Name of the timer to stop.</param>
		public void			StopTimer(string TimerName)
		{
			if (!Timers.ContainsKey(TimerName))	return;
			long CT	= DateTime.Now.Ticks;
			var A = Timers.GetElement(TimerName);
			if (A.Stopped)	return;
			A.Ticks += CT - A.T_Ticks;
			A.Stopped = true;
		}

		/// <summary>
		/// Check current memory allocation.
		/// </summary>
		public void			CheckMemory()
		{
			CheckMemory("");
		}
		/// <summary>
		/// Check current memory allocation.
		/// </summary>
		/// <param name="Descr">Description of current state.</param>
		public void			CheckMemory(string Descr)
		{
			if (_timerOnly)		return;
			long T	= DateTime.Now.Ticks;
			long CM = Environment.WorkingSet;
			var A = new MemStat();
			A.T_Mem	= CM;
			if (MemStates.Count == 0)	A.Memory = CM - Init_Mem;
			else	A.Memory = CM - MemStates.Last.T_Mem;
			A.Descr	= Descr;
			MemStates.Add(A);
			Excess	+= DateTime.Now.Ticks - T;
			LastOpTime = false;
		}
		#endregion

		//	Methods for statistics output
		#region
		/// <summary>
		/// Prints current memory state to console.
		/// </summary>
		/// <param name="Header">Text string which should be used as a header.</param>
		public void			PrintCurrentMemory(string Header)
		{
			Console.WriteLine(Header);
            Console.WriteLine("Time: " + DateTime.Now.ToString("hh:mm:ss"));
            Console.WriteLine("Free memory: " + this.Memory_FreeMB + " mb");
            Console.WriteLine("Used memory: " + this.Memory_UsedMB + " mb");
			Console.WriteLine("\r\n----------------------------------------\r\n");
		}

		/// <summary>
		/// Delegate for MeasureMethod
		/// </summary>
		public delegate void VoidDT();
		/// <summary>
		/// Measure performance of the method.
		/// </summary>
		/// <param name="MethodName">Name of the method (for readability).</param>
		/// <param name="Method">The delegate of method in the next form: delegate(){ SomeMethod(Parameters); }
		/// <para>For example, to measure performance of method SomeMethod with 2 int arguments, use next string:</para>
		/// <para>YourStatInstance.MeasureMethod("SomeMethod",  delegate(){ SomeMethod(N, M); });</para></param>
		/// <returns>Returns the time of performance in seconds.</returns>
		public float		MeasureMethod(string MethodName, VoidDT Method)
		{
			this.StartTimer(MethodName);
			Method();
			this.StopTimer(MethodName);
			return this.Timers.Last.Secs;
		}
		#endregion

		//	Constructors & ToString method
		#region
		/// <summary>
		/// Constructor of ZStat class.
		/// </summary>
		/// <param name="TimerOnly">Gets whether this instance of ZStat will be used for timing only.</param>
		public ZStat(bool TimerOnly)
		{
			_timerOnly = TimerOnly;
			if (!TimerOnly)
				Cnt = new PerformanceCounter("Memory", "Available Bytes");
			Init_Ticks	= DateTime.Now.Ticks;
			Init_Mem	= Environment.WorkingSet;
			TimeStates	= new TimeStateList();
			Timers		= new TimeStateList();
			MemStates	= new MemStateList();
		}
		/// <summary>
		/// Constructor of ZStat class.
		/// </summary>
		public ZStat()	: this(false)
		{
		}
		
		/// <summary>
		/// Returns a string representation of this ZStat instance.
		/// </summary>
		/// <returns>A string representation of this ZStat instance.</returns>
		public new string	ToString()
		{
			if (LastOpTime)	return TimeStates.Last.ToString();
			else return MemStates.Last.ToString();
		}
		#endregion

		//	Definition of internal classes  &  service methods
		#region
		/// <summary>
		/// This class defines a single state of statistics.
		/// </summary>
		public class TimeStat
		{
			/// <summary>
			/// Description of this state.
			/// </summary>
			public string	Descr;
			/// <summary>
			/// Count of ticks passed from certain point.
			/// </summary>
			public long		Ticks;
			/// <summary>
			/// Count of seconds passed from certain point.
			/// </summary>
			public float	Secs		{	get	{	return (float)Math.Round(Ticks / 10000000f, 3);	}}
			/// <summary>
			/// Count of micro-seconds (one 1'000'000th of second) passed from certain point.
			/// </summary>
			public long		MicroSecs	{	get	{	return Ticks / 10;	}}

			internal long	T_Ticks;
			/// <summary>
			/// Defines if this timer is stopped.
			/// </summary>
			public bool		Stopped;

			/// <summary>
			/// Returns a string representation of this TimeStat instance.
			/// </summary>
			/// <returns>A string representation of this TimeStat instance.</returns>
			public new string ToString()
			{
				return Descr + "  |  " + Math.Round(Secs, 3) + " secs";
			}
		}

		/// <summary>
		/// This class defines a single state of statistics.
		/// </summary>
		public class MemStat
		{
			/// <summary>
			/// Description of this state.
			/// </summary>
			public string	Descr;
			/// <summary>
			/// Memory consuming (in bytes).
			/// </summary>
			public long		Memory;

			internal long	T_Mem;

			/// <summary>
			/// Returns a string representation of this MemStat instance.
			/// </summary>
			/// <returns>A string representation of this MemStat instance.</returns>
			public new string ToString()
			{
				return ToString(true);
			}

			/// <summary>
			/// Returns a string representation of this MemStat instance.
			/// </summary>
			/// <param name="IncludeDescription">Include description or not.</param>
			/// <returns>A string representation of this MemStat instance.</returns>
			public string ToString(bool IncludeDescription)
			{
				string S =  ZString.GetKB(Memory);
				if (Memory / 1048576 > 30)	S = ZString.GetMB(Memory);
				if (IncludeDescription)	return Descr + "  |  " + S;
				else return S;
			}
		}

		
		/// <summary>
		/// This class defines list of time states.
		/// </summary>
		public class TimeStateList	:	BaseList<TimeStat>
		{
			/// <summary>
			/// Total time.
			/// </summary>
			public float	TotalTimeSecs
			{
				get
				{
					float N = 0;
					for (int i = 0; i < this.Count; i++)	N += this[i].MicroSecs;
					return (float)Math.Round(N / 1000000f, 3);
				}
			}

			/// <summary>
			/// Check if this TimeStatList contains a record with given name.
			/// </summary>
			/// <param name="Name">Name of the record to search for.</param>
			/// <returns>Returns true if this TimeStatList contains a record with given name, otherwise returs false.</returns>
			public bool		ContainsKey(string Name)
			{
				for (int i = 0; i < this.Count; i++)
					if (this[i].Descr == Name)	return true;
				return false;
			}

			/// <summary>
			/// Get the element with given name.
			/// </summary>
			/// <param name="Name">Name of the element to get.</param>
			/// <returns>Returns element with given name if it exist, otherwise return null.</returns>
			public TimeStat GetElement(string Name)
			{
				for (int i = 0; i < this.Count; i++)
					if (this[i].Descr == Name)	return this[i];
				return null;
			}
		}
		
		/// <summary>
		/// This class defines list of memory states.
		/// </summary>
		public class MemStateList	:	BaseList<MemStat>
		{
			/// <summary>
			/// Total amount of memory used by these check-states.
			/// </summary>
			public int TotalMemoryMB
			{
				get
				{
					long N = 0;
					for (int i = 0; i < this.Count; i++)	N += this[i].Memory;
					return (int)(N / 1048576);
				}
			}

			/// <summary>
			/// Check if this MemStateList contains a record with given name.
			/// </summary>
			/// <param name="Name">Name of the record to search for.</param>
			/// <returns>Returns true if this MemStateList contains a record with given name, otherwise returs false.</returns>
			public bool		ContainsKey(string Name)
			{
				for (int i = 0; i < this.Count; i++)
					if (this[i].Descr == Name)	return true;
				return false;
			}

			/// <summary>
			/// Get the element with given name.
			/// </summary>
			/// <param name="Name">Name of the element to get.</param>
			/// <returns>Returns element with given name if it exist, otherwise return null.</returns>
			public MemStat	GetElement(string Name)
			{
				for (int i = 0; i < this.Count; i++)
					if (this[i].Descr == Name)	return this[i];
				return null;
			}
		}
		#endregion
	}
}

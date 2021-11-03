using System;
using System.Threading;

namespace Utilities
{
    public class TaskEvent
	{
		private readonly Lazy<ManualResetEvent> _start = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _stop = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _abort = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _idle = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _stopped = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _setupDone = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));
		private readonly Lazy<ManualResetEvent> _error = new Lazy<ManualResetEvent>(() => new ManualResetEvent(false));

		public ManualResetEvent Start => _start.Value;
		public ManualResetEvent Stop => _stop.Value;
		public ManualResetEvent Abort => _abort.Value;
		public ManualResetEvent Idle => _idle.Value;
		public ManualResetEvent Stopped => _stopped.Value;
		public ManualResetEvent SetupDone => _setupDone.Value;
		public ManualResetEvent Error => _error.Value;

	}
}
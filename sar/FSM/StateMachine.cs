using System;

namespace sar.FSM
{
	/// <summary>
	/// Description of StateMachine.
	/// </summary>
	public abstract class StateMachine
	{
		public CommandQueue CommandQueue;

		protected bool loopStopRequested;
		protected bool loopStopped;
		
		protected StateMachine()
		{
			CommandQueue = new CommandQueue();
			loopStopped = false;
			loopStopRequested = false;
		}
		
		protected abstract void Start();
		protected abstract void Stop();		
	}
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Proto;
using Utilities;

namespace ActorDemo.Actors
{
    public class ProcedureActor:IActor
    {
        public ProcedureActor(TaskEvent _taskEvent)
        {
            TaskEvent =_taskEvent;
        }
        private PID ProcessSetPid;
        private TaskEvent TaskEvent { get; }
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    string currentName = context.Self.Id.Split('/').Reverse().First();
                    ProcessSetPid = context.SpawnNamed(Props.FromProducer(() => new ProcedureSetActor()), $"{currentName}Set");
                    break;
                case StationRunningMessage _:
                    bool isStop = false;
                    context.Send(ProcessSetPid, new InitRunMessage());
                    while (!isStop)
                    {
                        var result = WaitHandle.WaitAny(new WaitHandle[] { TaskEvent.Start, TaskEvent.Stop, TaskEvent.Abort });
                        {
                            switch (result)
                            {
                                case 0:
                                    var stateStart = context.RequestAsync<bool>(ProcessSetPid, new StartMessage()).Result;
                                    break;
                                case 1:
                                    var stateStop = context.RequestAsync<bool>(ProcessSetPid, new StopMessage()).Result;
                                    isStop = true;
                                    break;
                                case 2:
                                    var stateAbort = context.RequestAsync<bool>(ProcessSetPid, new AbortMessage()).Result;
                                    isStop = true;
                                    break;
                            }
                        }
                        Thread.Sleep(50);
                    }
                    break;

                case Stopping msg:
                    Console.WriteLine($"{context.Self.Id} is Stopping");
                    break;
                case Stopped msg:
                    Console.WriteLine($"{context.Self.Id} is Stopped");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
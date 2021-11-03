using System;
using System.Threading.Tasks;
using Proto;
using Utilities;

namespace ActorDemo.Actors
{
    class StationActor:IActor
    {
        private readonly TaskEvent taskEvent = new TaskEvent();
        private PID ProcedurePID;
        private readonly Behavior _behavior;
        public StationActor()
        {
            _behavior = new Behavior();
            _behavior.Become(NullAsync);
        }
        public Task ReceiveAsync(IContext ctx) => _behavior.ReceiveAsync(ctx);
        public Task NullAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case Started:
                    Console.WriteLine(ctx.Parent.Id+" Create the "+ ctx.Self.Id.ToString()+":is Started");
                    _behavior.Become(IdleAsync);
                    //Tell Parent(Station Hub) that self is Started;
                    ctx.Request(ctx.Parent,new StationStartedMessage());
                break;

            }
         return Task.CompletedTask;
        }
        ///Procedure 只有在完成事件後，才會被清掉，
        ///

        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case string msg when msg == "Start":  
                    taskEvent.Stop.Reset();
                    taskEvent.Start.Set();
                    ProcedurePID =(ProcedurePID is null)?  IncarnateProcedure(ctx,taskEvent):ProcedurePID;
                    ctx.Send(ProcedurePID,new StationRunningMessage());
                break;  
                case string msg when msg == "Stop":
                    taskEvent.Start.Reset();
                    taskEvent.Stop.Set();
                    Console.WriteLine($"Stop!!!");
                break;
            }
         return Task.CompletedTask;
        }
        PID IncarnateProcedure(IContext ctx,TaskEvent taskEvent)
        {
            PID pid = ctx.Spawn(Props.FromProducer(()=>new ProcedureActor(taskEvent)));
            Console.WriteLine(ctx.Self.Id+" Create "+pid.Id );
            return pid;
        }
        void switchProcedureSet (IContext ctx,int type)
        {
            ctx.Send(ProcedurePID,type);
        }
    }
}
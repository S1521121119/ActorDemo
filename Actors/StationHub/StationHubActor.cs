using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Proto;

namespace ActorDemo.Actors
{
    class StationHubActor:IActor
    {
        List<PID> StationsPID;
        Lazy<ManualResetEvent> _stop = new Lazy<ManualResetEvent>(()=>new ManualResetEvent(false));
        private readonly Behavior _behavior;
        public StationHubActor()
        {
            StationsPID = new List<PID>();
            _behavior = new Behavior();
            _behavior.Become(NullAsync);
        }
        public Task ReceiveAsync(IContext ctx) => _behavior.ReceiveAsync(ctx);
        public Task NullAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case Started:
                    _behavior.Become(IdleAsync);
                    StationsPID.Clear();
                break;

            }
         return Task.CompletedTask;
        }
        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case string msg when msg == "Create":
                    ctx.Send(ctx.Self,new IncarnateStationMessage());
                break;
                
                case  IncarnateStationMessage:
                    var name = "ST_"+StationsPID.Count.ToString("00");
                    StationsPID.Add(IncarnateStation(ctx,name));
                break;

                case string msg:
                    Console.WriteLine($"Station hub msg : {ctx.Message}");
                    foreach (var st in StationsPID)
                    {
                        ctx.Forward(st);
                    }
                break;            
                
            }
         return Task.CompletedTask;
        }
        private PID IncarnateStation(IContext ctx,string name)
        {
            return ctx.SpawnNamed(Props.FromProducer(()=>new StationActor()),name);
        }
    }


}
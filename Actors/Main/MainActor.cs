using System;
using System.Threading.Tasks;
using Proto;

namespace ActorDemo.Actors
{
    class MainActor:IActor
    {
        PID stationHubPID;
        PID deviceHubPID;
        private readonly Behavior _behavior;
        public MainActor()
        {
            _behavior = new Behavior();
            _behavior.Become(NullAsync);
        }
        public Task ReceiveAsync(IContext ctx) => _behavior.ReceiveAsync(ctx);
        public Task NullAsync(IContext ctx)
        {
            if (stationHubPID == null)
            {    
                stationHubPID =ctx.SpawnNamed(Props.FromProducer(()=>new StationHubActor()),"st_hub") ;
            }
            if (deviceHubPID == null)
            {    
                deviceHubPID =ctx.SpawnNamed(Props.FromProducer(()=>new DeviceHubActor()),"dv_hub") ;
            }
            switch (ctx.Message)
            {
                case Started:
                    _behavior.Become(IdleAsync);
                break;
                
            }
         return Task.CompletedTask;
        }
        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case string msg:
                    ctx.Send(stationHubPID,ctx.Message);
                    ctx.Send(deviceHubPID,ctx.Message);
                break;
                
            }
         return Task.CompletedTask;
        }
    }
}
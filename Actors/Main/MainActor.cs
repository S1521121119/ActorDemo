using System;
using System.Threading.Tasks;
using Proto;

namespace ActorDemo.Actors
{
    class MainActor:IActor
    {
        PID st_01;
        PID deviceHub;
        private readonly Behavior _behavior;
        public MainActor()
        {
            _behavior = new Behavior();
            _behavior.Become(NullAsync);
        }
        public Task ReceiveAsync(IContext ctx) => _behavior.ReceiveAsync(ctx);
        public Task NullAsync(IContext ctx)
        {
            if (st_01 == null)
            {
                
                st_01 =ctx.SpawnNamed(Props.FromProducer(()=>new StationActor()),"st01") ;
                deviceHub =ctx.SpawnNamed(Props.FromProducer(()=>new DeviceHub()),"dvhub") ;
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
                    ctx.Send(st_01,ctx.Message);
                    ctx.Send(deviceHub,ctx.Message);
                break;
                
            }
         return Task.CompletedTask;
        }
    }
}
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
                break;

            }
         return Task.CompletedTask;
        }
        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case string msg when msg == "Start":
                    _stop.Value.Reset();
                    Task.Run(()=>
                    {
                        for (int i = 0 ; i<10;i++)
                        {                   
                            Console.WriteLine(i);
                            Thread.Sleep(1000);
                            
                            var value = WaitHandle.WaitAny(new WaitHandle[] {_stop.Value},300);
                            if (value != 258)
                            {
                                Console.WriteLine(value);
                                break;
                            }
                        }
                    } );
                     
                break;  
                case string msg when msg == "Stop":
                Console.WriteLine($"Stop!!!");
                _stop.Value.Set();
                break;
                case string msg when msg == "Start":
                    Console.WriteLine($"msg : {msg}");
                break;
            }
         return Task.CompletedTask;
        }
    }


}
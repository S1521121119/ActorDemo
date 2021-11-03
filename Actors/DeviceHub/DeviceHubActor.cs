using System;
using System.Threading.Tasks;
using Proto;
using ModbusTCPActor;


namespace ActorDemo.Actors
{
    class DeviceHubActor:IActor
    {
        static ushort count =100;
        PID modbusPID;
        private readonly Behavior _behavior;
        public DeviceHubActor()
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
                    modbusPID = ctx.SpawnNamed(Props.FromProducer(()=>new ModbusTCPActor.ModbusTCPSlaveActor(502)),"Modbus");
                    _behavior.Become(IdleAsync);
                    //ctx.Send(ctx.Parent,new DeviceHubStartedMessage());
                break;

            }
         return Task.CompletedTask;
        }
        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case string msg when msg == "AO":
                    ctx.Send(modbusPID,new WriteAO(1,count++));
                break;
                default:
                    Console.WriteLine($"device hub msg : {ctx.Message}");
                break;
            }
         return Task.CompletedTask;
        }
    }
}
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
                    //ctx.Send(ctx.Parent,new StationHubStartedMessage());
                break;

            }
         return Task.CompletedTask;
        }
        public Task IdleAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                //From Main
                case string msg when msg == "Create":
                    ctx.Send(ctx.Self,new IncarnateStationMessage());
                break;
                
                case IncarnateStationMessage msg:
                    var name = "ST_"+StationsPID.Count.ToString("00");
                    StationsPID.Add(IncarnateStation(ctx,name));
                break;
                
                //from Station
                case StationStartedMessage msg:
                    ShowMessage($"StationStartedMessage : {ctx.Sender.Id}");
                    ctx.Send(ctx.Sender,"Start");
                break;

                case string msg:
                    ShowMessage($"Station hub msg : {ctx.Message}");
                    foreach (var st in StationsPID)
                    {
                        ctx.Forward(st);
                    }
                break;            
                
            }
         return Task.CompletedTask;
        }
        static bool ShowMessageSwitch = true;
        public void ShowMessage(string msg)
        {
            if (ShowMessageSwitch)
                Console.WriteLine(msg);
        }
        private PID IncarnateStation(IContext ctx,string name)
        {
            return ctx.SpawnNamed(Props.FromProducer(()=>new StationActor()).WithChildSupervisorStrategy(new OneForOneStrategy(CommonStrategy.Decider.Decide,1,null)),name);
            //return ctx.SpawnNamed(Props.FromProducer(()=>new StationActor()),name);
        }
    }
    public static class CommonStrategy{
        public static class Decider
        {
            public static SupervisorDirective Decide(PID pid, Exception reason)
                => reason switch
                {
                    RecoverableException _ => SupervisorDirective.Restart,
                    FatalException _       => SupervisorDirective.Stop,
                    _                      => SupervisorDirective.Escalate
                };
        }
        public class RecoverableException : Exception
        {
            
        }

        public class FatalException : Exception
        {
        }
    }

}
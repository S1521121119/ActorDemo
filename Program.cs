using System;
using System.Threading.Tasks;
using Proto;
using ActorDemo.Actors;

namespace ActorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var system = new ActorSystem();
            var root = system.Root;
            var props = Props.FromProducer(()=>new MainActor());
            var MainPid = root.SpawnNamed(props,"Main");
            string msg = "null";

            root.Send(MainPid,new IncarnateStationMessage());

            while (msg!="exit")
            {   
                msg = Console.ReadLine();

                if (msg == "clear")
                    Console.Clear();

                root.Send(MainPid,msg);
            }
        }
    }
    
}

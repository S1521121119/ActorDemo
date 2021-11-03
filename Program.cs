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
            var system = new ActorSystem();
            var root = system.Root;
            var props = Props.FromProducer(()=>new MainActor());
            var MainPid = root.SpawnNamed(props,"Main");
            string msg = "null";
            
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

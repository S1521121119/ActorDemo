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
            var MainPid = root.Spawn(props);
            string msg = "null";
            while (msg!="exit")
            {
                msg = Console.ReadLine();
                root.Send(MainPid,msg);
            }
        }
    }
    
}

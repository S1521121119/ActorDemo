using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Proto;

namespace ActorDemo.Actors
{
    public class ProcedureSetActor:IActor
    {
     public Task ReceiveAsync(IContext context)
        {
          switch (context.Message)
            {  
              case Started _:      

              break;   
              case InitRunMessage _:

              break;
              case StartMessage _:
                for (int i =0;i<10;i++)
                {
                  Console.WriteLine(i);

                }
              break;
              case StopMessage _:
              
              break;
            }
            return Task.CompletedTask;
        }   
    }
}
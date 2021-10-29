using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTCPActor
{
    internal class WriteAO
    {
        public WriteAO(ushort startingAddress, ushort value)
        {
            StartingAddress = startingAddress;
            Value = value;
        }

        public ushort StartingAddress { get; }
        public ushort Value { get; }
    }
}

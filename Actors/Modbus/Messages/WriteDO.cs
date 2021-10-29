using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTCPActor
{
    internal class WriteDO
    {
        public WriteDO(ushort startingAddress, bool value)
        {
            StartingAddress = startingAddress;
            Value = value;
        }

        public ushort StartingAddress { get; }
        public bool Value { get; }
    }
}

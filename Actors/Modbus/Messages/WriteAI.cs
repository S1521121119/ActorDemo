namespace ModbusTCPActor
{
    internal class WriteAI
    {
        public WriteAI(ushort startingAddress, ushort value)
        {
            StartingAddress = startingAddress;
            Value = value;
        }

        public ushort StartingAddress { get; }
        public ushort Value { get; }
    }
}

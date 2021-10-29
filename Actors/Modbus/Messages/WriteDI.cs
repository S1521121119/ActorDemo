namespace ModbusTCPActor
{
    internal class WriteDI
    {
        public WriteDI(ushort startingAddress, bool value)
        {
            StartingAddress = startingAddress;
            Value = value;
        }

        public ushort StartingAddress { get; }
        public bool Value { get; }
    }
}

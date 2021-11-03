using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modbus.Data;
using Modbus.Device;
using Proto;

namespace ModbusTCPActor
{
    public class ModbusTCPSlaveActor : IActor
    {
        public int Port { get; }
        public IPHostEntry IpEntry { get; }
        IPAddress[] Address => IpEntry.AddressList;
        public int TcpPort { get; }

        TcpListener slaveTcpListener;
        ModbusSlave slave;

        bool[] Coils = new bool[65536];

        private byte slaveID = 1;

        public ModbusTCPSlaveActor(int port)
        {
            //Get host IP
            IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            TcpPort = port;
            Console.WriteLine("Host IP=" + Address[1].ToString());
            //ModbusThread = new Thread(ScanListen);
            //string ServerName = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        }

        public Task ReceiveAsync(IContext context)

        {
            switch (context.Message)
            {
                case Started _:
                    // create and start the TCP slave
                    slaveTcpListener = new TcpListener(Address[1], TcpPort);
                    slaveTcpListener.Start();

                    slave = Modbus.Device.ModbusTcpSlave.CreateTcp(slaveID, slaveTcpListener);
                    slave.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(Modbus_Request_Event);
                    slave.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore();
                    slave.DataStore.DataStoreWrittenTo += new EventHandler<DataStoreEventArgs>(Modbus_DataStoreWriteTo);
                    slave.ListenAsync();
                  //Console.WriteLine("ModbusTCPSlaveActor Started Up");
                    break;
                case WriteAI msg:
                    WriteHoldingRegisters(msg.StartingAddress, msg.Value);
                    break;
                case WriteDI msg:
                    SetCoil(msg.StartingAddress, msg.Value);
                    break;
                case WriteAO msg:
                    WriteInputRegisters(msg.StartingAddress, msg.Value);
                    break;
                case WriteDO msg:
                    SetDiscretes(msg.StartingAddress, msg.Value);
                    break;
                
                case Stopping msg:
                    slaveTcpListener.Stop();
                    slaveTcpListener = null;
                    //slave.Dispose();            
                    if (slave != null)
                        slave.Dispose();
                    break;

            }
            return Task.CompletedTask;
        }
        private void Modbus_Request_Event(object sender, Modbus.Device.ModbusSlaveRequestEventArgs e)
        {
            //request from master//disassemble packet from master
            byte fc = e.Message.FunctionCode;
            byte[] data = e.Message.MessageFrame;
            byte[] byteStartAddress = new byte[] { data[3], data[2] };
            byte[] byteNum = new byte[] { data[5], data[4] };
            Int16 StartAddress = BitConverter.ToInt16(byteStartAddress, 0);
            Int16 NumOfPoint = BitConverter.ToInt16(byteNum, 0);

            //Console.WriteLine(fc.ToString() + "," + StartAddress.ToString() + "," + NumOfPoint.ToString());
        }
        private void Modbus_DataStoreWriteTo(object sender, Modbus.Data.DataStoreEventArgs e)
        {
            //this.Text = "DataType=" + e.ModbusDataType.ToString() + "  StartAdress=" + e.StartAddress;
            int iAddress = e.StartAddress;//e.StartAddress;

            switch (e.ModbusDataType)
            {
                case ModbusDataType.HoldingRegister:

                    for (int i = 0; i < e.Data.B.Count; i++)
                    {
                        //Set AO                 

                        //e.Data.B[i] already write to slave.DataStore.HoldingRegisters[e.StartAddress + i + 1]
                        //e.StartAddress starts from 0
                        //You can set AO value to hardware here
                        DoAOUpdate(iAddress, e.Data.B[i]);
                        iAddress++;
                    }
                    break;

                case ModbusDataType.Coil:
                    for (int i = 0; i < e.Data.A.Count; i++)
                    {
                        //Set DO
                        //e.Data.A[i] already write to slave.DataStore.CoilDiscretes[e.StartAddress + i + 1]
                        //e.StartAddress starts from 0
                        //You can set DO value to hardware here
                        DoDOUpdate(iAddress, e.Data.A[i]);
                        iAddress++;
                        if (e.Data.A.Count == 1)
                        {
                            break;
                        }
                    }
                    break;
            }
        }
        #region "Set AO"
        private void WriteHoldingRegisters(int index, ushort message)
        {
            slave.DataStore.HoldingRegisters[index] = message;
        }
        private void WriteInputRegisters(int index, ushort message)
        {
            slave.DataStore.InputRegisters[index] = message;
        }
        private delegate void UpdateAOStatusDelegate(int index, String message);
        private void DoAOUpdate(int index, ushort value)
        {
            /*
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new UpdateAOStatusDelegate(DoAOUpdate), new object[] { index, message });
                return;
            }*/

            // this code can only be reached
            // by the user interface thread
            /*
            switch (index)
            {
                case 0:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_00 = value;
                    break;
                case 1:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_01 = value;
                    break;
                case 2:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_02 = value;
                    break;
                case 3:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_03 = value;
                    break;
                case 4:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_04 = value;
                    break;
                case 5:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_05 = value;
                    break;
                case 6:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_06 = value;
                    break;
                case 7:
                    UserDataBinding.GlobalVariable.ModbusAIData.Nud_AI_07 = value;
                    break;
                default:
                  //Console.WriteLine(index.ToString() + ":" + value);
                    break;
            }
          //Console.WriteLine(index.ToString() + ":" + value);
            */
        }
        #endregion

        #region "Set DO"
        private void SetCoil(int index, bool value)
        {
            slave.DataStore.CoilDiscretes[index] = value;
        }
        private void SetDiscretes(ushort index, bool value)
        {
            slave.DataStore.InputDiscretes[index] = value;
        }
        private delegate void UpdateDOStatusDelegate(int index, bool value);
        private void DoDOUpdate(int index, bool value)
        {
            /*
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new UpdateDOStatusDelegate(DoDOUpdate),
                            new object[] { index, value });
                return;
            }*/

            // this code can only be reached
            // by the user interface thread

            Coils[index] = value;
            /*
            switch (index)
            {
                case 0:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_00 = value;
                    break;
                case 1:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_01 = value;
                    break;
                case 2:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_02 = value;
                    break;
                case 3:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_03 = value;
                    break;
                case 4:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_04 = value;
                    break;
                case 5:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_05 = value;
                    break;
                case 6:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_06 = value;
                    break;
                case 7:
                    UserDataBinding.GlobalVariable.ModbusDIData.Chk_DI_07 = value;
                    break;
                default:
                  //Console.WriteLine(index.ToString() + ":" + value);
                    break;
            }
            */
        }
        #endregion
    }

}

using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using Modbus.Serial;

namespace Model
{
    public class ModbusCommunication
    {
        IModbusMaster master;

        public ModbusCommunication()
        {
            SerialPort serialPort = new SerialPort()
            {
                PortName = "COM1", //the port is system dependant. Needs a way to pick the right one
                BaudRate = 57600,
                DataBits = 8,
                Parity = Parity.Even,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            serialPort.Open();
        
		    var adapter = new SerialPortAdapter(serialPort);
            // create modbus master
            master = ModbusSerialMaster.CreateRtu(adapter);

		}

        public void RunModbus(ushort startAddress, ushort [] data)
        {
            byte slaveAddress = 0x1;
            
            master.WriteMultipleRegisters(slaveAddress, startAddress, data);

        }

        public void ReadModbus()
        {

        }

        public void EndModbus()
        {
            master.Dispose();
        }
    }
}

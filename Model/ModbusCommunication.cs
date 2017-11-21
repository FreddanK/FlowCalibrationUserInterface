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
        public ModbusCommunication()
        {

            using (SerialPort port = new SerialPort("/dev/ttyUSB0"))
			{
				// configure serial port
				port.BaudRate = 57600;
				port.DataBits = 8;
				port.Parity = Parity.Even;
				port.StopBits = StopBits.One;
				port.Open();

				Console.WriteLine("port open");

				var adapter = new SerialPortAdapter(port);
        // create modbus master
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(adapter);
				Console.WriteLine("modbus master created");
			}
		}
    }
}

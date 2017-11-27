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
        public IModbusMaster Master { get; set; }

        const int MAXBYTE = 65536;

        public ModbusCommunication()
        {
            SerialPort serialPort = new SerialPort()
            {
                PortName = "/dev/ttyUSB0", //the port is system dependant. Needs a way to pick the right one
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
            Master = ModbusSerialMaster.CreateRtu(adapter);

			// REGISTER: 204: Max torque allowed
			const ushort MotorTorqueMax = 204;
			if ((ReadModbus(MotorTorqueMax, 1, false)) > 100)
			{
                Master.Dispose();
				throw new Exception("Maximum torque set too high for safe operation");
			}
		}

        public void RunModbus(ushort registerStartAddress, Int32 data)
        {
            // For Int32 data (Double registers)

            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }

            byte slaveAddress = 0x1;

            ushort startAddress = (ushort)(registerStartAddress - 1);

            ushort[] dataUShort = new ushort[2];

            if (data >= MAXBYTE)
            {
                dataUShort[0] = ((ushort)(data / MAXBYTE));
                dataUShort[1] = ((ushort)(data - ((ushort)(data / MAXBYTE)) * MAXBYTE));
            }
            else if ( data < 0)
            {
                dataUShort[0] = (ushort)(MAXBYTE - 1 - ((ushort)((-data - 1) / MAXBYTE)));
                dataUShort[1] = (ushort)(MAXBYTE + data + ((ushort)((-data - 1) / MAXBYTE)) * MAXBYTE);
            }
            else
            {
                dataUShort[0] = (ushort)0;
                dataUShort[1] = (ushort)data;
            }
            
            Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
        }
        public void RunModbus(ushort registerStartAddress, Int16 data)
        {
            // For Int16 data (Single register)
            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }
            
            byte slaveAddress = 0x1;
            ushort startAddress = (ushort)(registerStartAddress - 1);

            ushort[] dataUShort = new ushort[] { (ushort)data };
            Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
        }

		public void RunModbus(ushort registerStartAddress, ushort data)
		{
			// For Int16 data (Single register)
			if (registerStartAddress == 0)
			{
				throw new Exception("Illegal register address 0");
			}

			byte slaveAddress = 0x1;
			ushort startAddress = (ushort)(registerStartAddress - 1);

			ushort[] dataUShort = new ushort[] { data };
			Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
		}
  
        public int ReadModbus(ushort registerStartAddress, ushort nrOfRegisters, Boolean signedValue)
        {
            // Reads nrOfRegisters amount of registers and returns thier combined data as an int
            int returnData;

            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }

            byte slaveAddress = 0x1;
            ushort startAddress = (ushort)(registerStartAddress - 1);

            ushort[] dataValue = new ushort[nrOfRegisters];
            dataValue = Master.ReadHoldingRegisters(slaveAddress, startAddress, nrOfRegisters);
            
            // Convert dataValue (ushort[]) to int
            if (signedValue && dataValue[0] >= 32768)
            {
                returnData = -(MAXBYTE -1 - dataValue[0]) * MAXBYTE - (MAXBYTE -1 - dataValue[1]) - 1;
            }
            else if (nrOfRegisters == 1)
            {
                returnData = dataValue[0];
            }
            else {
                returnData = dataValue[0] * MAXBYTE + dataValue[1];
            }
            return returnData;
        }

        public void EndModbus()
        {
            Master.Dispose();
        }
    }
}

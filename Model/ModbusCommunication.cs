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
        static IModbusMaster master;

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

        public void RunModbus(Int32 registerStartAddress, int data)
        {
            // For Int32 data (Double registers)

            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }

            int MAXBYTE = 65536;
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
            
            master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
        }
        public void RunModbus(Int16 registerStartAddress, int data)
        {
            // For Int16 data (Single register)
            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }
            
            byte slaveAddress = 0x1;
            ushort startAddress = ((ushort)(registerStartAddress - 1));

            ushort[] dataUShort = new ushort[] { (ushort)data };
            master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
        }

        public int ReadModbus(Int32 registerStartAddress, Boolean signedValue)
        {
            // For Int32 data (Double registers)
            int returnData;

            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }

            int MAXBYTE = 65536;
            byte slaveAddress = 0x1;
            ushort startAddress = (ushort)(registerStartAddress - 1);

            ushort[] dataValue = new ushort[2];
            dataValue = master.ReadHoldingRegisters(slaveAddress, startAddress, 2);
            
            // Convert dataValue (ushort[]) to int
            if (signedValue && dataValue[0] >= 32768)
            {
                returnData = -(MAXBYTE -1 - dataValue[0]) * MAXBYTE - (MAXBYTE -1 - dataValue[1]) - 1;
                return returnData;
            }
            returnData = dataValue[0] * MAXBYTE + dataValue[1];
            return returnData;
        }
        public int ReadModbus(Int16 registerStartAddress, Boolean signedValue)
        {
            // For Int16 data (Single register)
            int returnData;

            if (registerStartAddress == 0)
            {
                throw new Exception("Illegal register address 0");
            }

            byte slaveAddress = 0x1;
            ushort startAddress = (ushort)(registerStartAddress - 1);

            ushort[] dataValue = new ushort[1];
            dataValue = master.ReadHoldingRegisters(slaveAddress, startAddress, 1);

            // Convert dataValue (ushort[]) to int
            returnData = dataValue[0];
            return returnData;
        }

        public ushort[] ReadModbus(ushort startAdress, ushort registerLength)
        {
            byte slaveAddress = 0x1;
            ushort[] data = master.ReadHoldingRegisters(slaveAddress,startAdress, registerLength);

            return data;

        }

        public void EndModbus()
        {
            master.Dispose();
        }
    }
}

using System;
using System.IO.Ports;
using Modbus.Device;
using Modbus.Serial;
using System.Management;
using System.Collections.Generic;

namespace Model
{
    public class ModbusCommunication
    {
        public IModbusMaster Master { get; set; }

        const int MAXBYTE = 65536;

        public ModbusCommunication(String portName)
        {
            SerialPort serialPort = new SerialPort()
            {
                PortName = portName,
                BaudRate = 57600,
                DataBits = 8,
                Parity = Parity.Even,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 20,
                WriteTimeout = 20
            };

            serialPort.Open();
            
        
		    var adapter = new SerialPortAdapter(serialPort);
            // create modbus master
            Master = ModbusSerialMaster.CreateRtu(adapter);
        }

        public static string GetSerialPortName()
        {
            return "COM3";
            //return "/dev/ttyUSB0"; // For Linux
            /*string NameOfDevice = "Moxa USB Serial Port";
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
                if (comPort.Name.Equals(NameOfDevice, StringComparison.Ordinal))
                {
                    return comPort.Port;
                }

            }
            throw new System.ArgumentException("No device found");*/
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
            
            try
            {
                Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
            }
            catch (System.TimeoutException) { }
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
            try
            {
                Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
            }
            catch (System.TimeoutException) { }
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
            try
            {
                Master.WriteMultipleRegisters(slaveAddress, startAddress, dataUShort);
            }
            catch (System.TimeoutException) { }
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
            try
            {
                dataValue = Master.ReadHoldingRegisters(slaveAddress, startAddress, nrOfRegisters);
            }
            catch(System.TimeoutException)
            {
                if (nrOfRegisters == 1)
                {
                    dataValue[0] = 0;
                }
                else if (nrOfRegisters == 2)
                {
                    dataValue[0] = 0;
                    dataValue[1] = 0;
                }
            }
            
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
    internal class ProcessConnection
    {

        public static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }

        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + path);
            connectScope.Options = options;
            connectScope.Connect();
            return connectScope;
        }
    }
    public class COMPortInfo
    {
        public string Name { get; set; }
        public string Port { get; set; }

        public COMPortInfo() { }

        public static List<COMPortInfo> GetCOMPortsInfo()
        {
            List<COMPortInfo> comPortInfoList = new List<COMPortInfo>();

            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");

            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);
            using (comPortSearcher)
            {
                string caption = null;
                foreach (ManagementObject obj in comPortSearcher.Get())
                {
                    if (obj != null)
                    {
                        object captionObj = obj["Caption"];
                        if (captionObj != null)
                        {
                            caption = captionObj.ToString();
                            if (caption.Contains("(COM"))
                            {
                                COMPortInfo comPortInfo = new COMPortInfo();
                                comPortInfo.Port = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")",
                                                                     string.Empty);
                                comPortInfo.Name = caption.Substring(0, caption.LastIndexOf("(COM") - 1); // -1 to remove space;
                                comPortInfoList.Add(comPortInfo);
                            }
                        }
                    }
                }
            }
            return comPortInfoList;
        }
    }
}

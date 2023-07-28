using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
using static UI_QuickChangeAOE;

public class SerialPortUnityGateMono : MonoBehaviour
{

    public string[] m_channelOpened;

    public void Update()
    {
        SerialPortUnityGateStatic.GetListChannelOpen(out m_channelOpened);
    }

    [ContextMenu("Restart All Port")]
    public void RestartAllPort()
    {
        SerialPortUnityGateStatic.RestartAllPorts();
    }

    private void OnDestroy()
    {
        SerialPortUnityGateStatic.CloseAllOpen();
    }
    private void OnApplicationQuit()
    {

        SerialPortUnityGateStatic.CloseAllOpen();
       
    }
    public void CloseAndReopenPortIfRegistered(params string[] portToReopen)
    {
        SerialPortUnityGateStatic.GetInstance().CloseAndReopenPortIfRegistered(portToReopen);
    }
 }


public class SerialPortUnityGateStatic {

    static SerialPortUnityGate m_instance = new SerialPortUnityGate();
    public static SerialPortUnityGate GetInstance() { return m_instance; }
    public static void GetInstance(out SerialPortUnityGate gateInstance) { 
        gateInstance= m_instance; 
    }

    public  static void GetListChannelOpen(out string[] channelOpened)
    {
        channelOpened = m_instance.m_openedPort.Select(k=>k.Key).ToArray();
    }



    public static void CloseAllOpen()
    {
        m_instance.CloseAllOpen();
    }

    internal static void RestartAllPorts()
    {
        m_instance.RestartAllPortsRegistered();
    }
}

public class SerialPortUnityGate  { 


public Dictionary<string, SerialPort> m_openedPort = new Dictionary<string, SerialPort>();

public void OpenSerialPort(int portId,
   in int baudRate = 9600,
   in int dataBits = 8,
   in Parity parity = Parity.None,
   in StopBits stopBits = StopBits.One)
=> OpenSerialPort("COM" + portId, baudRate, dataBits, parity, stopBits);

public void OpenSerialPort(in string portName = "COM30",
in int baudRate = 9600,
in int dataBits = 8,
in Parity parity = Parity.None,
in StopBits stopBits = StopBits.One)
{


    if (m_openedPort.ContainsKey(portName))
    {
        CloseSerialPort(portName);
    }
    if (!m_openedPort.ContainsKey(portName))
    {
        try
        {

            SerialPort serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            serialPort.Open();
            m_openedPort.Add(portName.ToString(), serialPort);
        }
        catch (Exception e)
        {
            Debug.Log("Fail to open serial port:"+ portName+"\n"+ e.StackTrace);

        }
    }
}

public void SentMessagetoSerialPort(string portName = "COM1", params string[] texts)
{
    try
    {
        for (int i = 0; i < texts.Length; i++)
        {
            m_openedPort[portName].WriteLine(" "+texts[i]+"\n");
                //Debug.Log("YY|"+portName+"|"+texts[i]);
        }
    }
    catch (Exception e)
    {
        Debug.Log("Fail to sent text to serial port:" + e.StackTrace);

    }
}

public void CloseSerialPort(string portName = "COM1")
{
    if (m_openedPort.ContainsKey(portName))
    {
        SerialPort p = m_openedPort[portName];
        try
        {
            if (p != null)
            {
                p.Close();
                p.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fail to close port: " + ex.Message);
        }
        m_openedPort.Remove(portName);

    }
}
public void CloseAllOpen()
{

    string[] keys = m_openedPort.Keys.ToArray();
    foreach (var item in keys)
    {
        SerialPort p = m_openedPort[item];
        try
        {
            if (p != null)
            {
                p.Close();
                p.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fail to close port: " + ex.Message);
        }
        m_openedPort.Remove(item);
    }
}


    public void SendTextTo(int serialId, string text)
    {
        SendTextTo("COM" + serialId, text);
    }
    public void SendTextTo(string serialId, string text)
    {
        if (m_openedPort.ContainsKey(serialId)) {
            m_openedPort[serialId].WriteLine(text);
        }
    }

    public void GetExistingPort(string serialPort, out bool found, out SerialPort port)
    {
        found = m_openedPort.ContainsKey(serialPort);
        if (found)
            port = m_openedPort[serialPort];
        else port = null;
    }


    public void CloseAndReopenPortIfRegistered(params string[] portToReopen)
    {

        foreach (var port in portToReopen)
        {
            GetExistingPort(port, out bool found, out SerialPort sp);
            if (found)
            {
                CloseSerialPort(sp.PortName);
                OpenSerialPort(sp.PortName, sp.BaudRate, sp.DataBits, sp.Parity, sp.StopBits);
            }

        }
    }

    public void RestartAllPortsRegistered()
    {
        string [] keys = m_openedPort.Keys.ToArray();
        foreach (var port in keys)
        {
            GetExistingPort(port, out bool found, out SerialPort sp);
            if (found)
            {
                CloseSerialPort(sp.PortName);
                OpenSerialPort(sp.PortName, sp.BaudRate, sp.DataBits, sp.Parity, sp.StopBits);
            }

        }
    }

    public void IsRegistered(string targetSerialPort, out bool found)
    {
        found = m_openedPort.ContainsKey(targetSerialPort);
    }

    public bool IsRegistered(string targetSerialPort)
    {
        return m_openedPort.ContainsKey(targetSerialPort);
    }
}



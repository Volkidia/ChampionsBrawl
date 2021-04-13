using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSend {
    string IP;
    int port;
    UdpClient host;
    IPEndPoint groupEP;
    string Message = "";
	// Use this for initialization
	public void Init (string _IP, int _port) {
        IP = _IP;
        port = _port;
        host = new UdpClient();
        groupEP = new IPEndPoint(IPAddress.Parse("192.168.1.255"), port);
        host.Connect(groupEP);
        Message = IP.ToString();
	}
	
	// Update is called once per frame
	public void sendData () {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(Message);
            host.Send(data, data.Length);
        }

        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
        Debug.Log("J'envoie les donnée Ip:" + IP + "port:" + port);
	}
}

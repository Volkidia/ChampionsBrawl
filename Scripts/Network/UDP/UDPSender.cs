using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class UDPSender : MonoBehaviour
{
	static void Main(string[] args) 
	{
		Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
		                      ProtocolType.Udp);
		
		IPAddress broadcast = IPAddress.Parse("192.168.1.255");
		
		byte[] sendbuf = Encoding.ASCII.GetBytes(args[0]);
		IPEndPoint ep = new IPEndPoint(broadcast, 11000);
		
		s.SendTo(sendbuf, ep);
		
		Debug.Log("Message sent to the broadcast address");
	}
}
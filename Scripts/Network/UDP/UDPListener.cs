using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
	private const int listenPort = 7777;
	
	private static void StartListener() 
	{
		bool done = false;
		
		UdpClient listener = new UdpClient(listenPort);
		IPEndPoint groupEP = new IPEndPoint(IPAddress.Any,listenPort);
		
		try 
		{
			while (!done) 
			{
				Debug.Log("Waiting for broadcast");
				byte[] bytes = listener.Receive( ref groupEP);
				
				Debug.Log("Received broadcast from {0} :\n {1}\n"+
				                  groupEP.ToString()+
				                  Encoding.ASCII.GetString(bytes,0,bytes.Length));
			}
			
		} 
		catch (Exception e) 
		{
			Console.WriteLine(e.ToString());
		}
		finally
		{
			listener.Close();
		}
	}
	
	public static int Main() 
	{
		StartListener();
		
		return 0;
	}
}

using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class UDPListen {
	private UdpClient Client;
	private IPEndPoint groupeEp;
	private string strReceiveUDP = "";
    private int port;
	// Use this for initialization

	public void Init(int ListenPort){
        port = ListenPort;
        /*Client = new UdpClient (ListenPort);
		groupeEp = new IPEndPoint (IPAddress.Any, ListenPort);
		ReceiveData ();*/
        try
        {
            if (Client == null)
            {
                Client = new UdpClient(ListenPort);
                Client.BeginReceive(new AsyncCallback(ReceiveData), null);
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }

	private void ReceiveData (IAsyncResult result){
        /*while (true)
		{
			try
			{
				byte[] data = Client.Receive(ref groupeEp);
				string text = Encoding.ASCII.GetString(data);
				strReceiveUDP = text;
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}*/
       groupeEp = new IPEndPoint(IPAddress.Any, port );
        byte[] received;
        if (Client != null)
        {
            received = Client.EndReceive(result, ref groupeEp);
        }
        else {
            return;
        }
        Client.BeginReceive(new AsyncCallback(ReceiveData), null);
        strReceiveUDP = Encoding.ASCII.GetString(received);
    }

	public string GetUDPinfo (){
		return strReceiveUDP;
	}
}

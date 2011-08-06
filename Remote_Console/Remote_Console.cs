using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.IO;

namespace MCForge
{
	public class Remote_Console : Plugin
	{
		private  Socket listen;

		// An ArrayList is used to keep track of worker sockets that are designed
		// to communicate with each connected client. Make it a synchronized ArrayList
		// For thread safety
		public AsyncCallback pfnWorkerCallBack; 
		private System.Collections.ArrayList clients =  ArrayList.Synchronized(new System.Collections.ArrayList());

		// The following variable will keep track of the cumulative 
		// total number of clients connected at any time. Since multiple threads
		// can access this variable, modifying this variable should be done
		// in a thread safe manner
		private int clientCount = 0;
		public int port = 1337;
		public override int build { get { return 100; } }
		public override string creator { get { return "GamezGalaxy"; } }
		public override bool LoadAtStartup { get { return true; } }
		public override string MCForge_Version { get { return ""; } }
		public override string name { get { return "Remote Console"; } }
		public override string website { get { return "www.gamezgalaxy.com"; } }
		public override string welcome { get { 
				if (listen.Connected)
					return ConsoleMessage("Remote Console is now ONLINE", false); 
				else
					return ConsoleMessage("Remote Console is now OFFLINE", false);
			} 
		}
		public override void Load (bool startup)
		{
			Server.s.Log("[Remote Console] Loading Config Files..", false);
			if (!Directory.Exists("plugins/Remote Console"))
				Directory.CreateDirectory("plugins/Remote Console");
			if (!File.Exists("plugins/Remote Console/config.properties"))
			{
				Server.s.Log("[Remote Console] Config file not found...let me make you one!", false);
				string[] lines = new string[2];
				lines[0] = "# Remote Console Config File";
				lines[1] = "# Please make sure to PORT FOWARD THE REMOTE PORT!";
				lines[2] = "Remote-Port=1337";
				File.WriteAllLines("plugins/Remote Console/config.properties", lines);
			}
			string[] config = File.ReadAllLines("plugins/Remote Console/config.properties");
			foreach (string line in config)
			{
				if (!line.StartsWith("#"))
				{
					if (line.Split('=')[0] == "Remote-Port")
						port = int.Parse(line.Split('=')[1]);
				}
			}
			if (port == Server.port)
			{
				ConsoleMessage("You are using the same port as the Server port", true);
				ConsoleMessage("Using defualt 1337", true);
			}
			try
			{
				// Create the listening socket...
				listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream,  ProtocolType.Tcp);
				// Bind to local IP Address...
				listen.Bind(new IPEndPoint(IPAddress.Any, Server.RemotePort));
				// Start listening...
				listen.Listen(4);
				// Create the call back for any client connections...
				listen.BeginAccept(new AsyncCallback (OnClientConnect), null);
				ConsoleMessage("Remote Console now listening on port " + port, false);
				
			}
			catch(SocketException se)
			{
				Server.ErrorLog(se);
				ConsoleMessage("Could not bind port", true);
				ConsoleMessage("Is something else using it?", false);
			}
			Player.PlayerChat += delegate(Player from, string message) {
				SendToAll("SERVER_CHAT " + from.prefix + from.name + ": " + message);
				return false;
			};
			
		}
		public void ConsoleMessage(string message, bool error)
		{
			if (!error)
				Server.s.Log("[Remote Console] " + message);
			else
				Server.s.Log("!![Remote Console] " + message + "!!");
		}
		public void OnClientConnect(IAsyncResult asyn)
		{
			try
			{
				// Here we complete/end the BeginAccept() asynchronous call
				// by calling EndAccept() - which returns the reference to
				// a new Socket object
				Socket workerSocket = listen.EndAccept (asyn);
				// Now increment the client count for this client 
				// in a thread safe manner
				Interlocked.Increment(ref clientCount);
				Remote_Clients temp = new Remote_Clients(workerSocket, clientCount);
				
				// Add the workerSocket reference to our ArrayList
				m_workerSocketList.Add(temp);
				ConsoleMessage("Client " + clientCount + " connected", false);
				// Send a welcome message to client
				SendMsgToClient("name", clientCount);

				// Update the list box showing the list of clients (thread safe call)

				// Let the worker Socket do the further processing for the 
				// just connected client
				WaitForData(workerSocket, clientCount);
							
				// Since the main Socket is now free, it can go back and wait for
				// other clients who are attempting to connect
				m_mainSocket.BeginAccept(new AsyncCallback ( OnClientConnect ),null);
			}
			catch(ObjectDisposedException)
			{
				ConsoleMessage("OnClientConnection: Socket has been closed", true);
			}
			catch(SocketException se)
			{
				Server.ErrorLog(se);
				ConsoleMessage("Error, please check the error log for more info!", true);
			}
			
		}
		// Start waiting for data from the client
		public void WaitForData(System.Net.Sockets.Socket soc, int clientNumber)
		{
			try
			{
				if  (pfnWorkerCallBack == null)
				{		
					// Specify the call back function which is to be 
					// invoked when there is any write activity by the 
					// connected client
					pfnWorkerCallBack = new AsyncCallback (OnDataReceived);
				}
				if (getclient(clientNumber) != null)
				{
					Remote_Clients remote = getclient(clientNumber);
					soc.BeginReceive(remote.dataBuffer, 0, remote.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, r);
				}
			}
			catch(SocketException se)
			{
				Server.ErrorLog(se);
				ConsoleMessage("Error, please check the error log for more info!", true);
			}
		}
		public  void OnDataReceived(IAsyncResult asyn)
		{
			Remote_Clients socketData = (Remote_Clients)asyn.AsyncState ;
			try
			{
				// Complete the BeginReceive() asynchronous call by EndReceive() method
				// which will return the number of characters written to the stream 
				// by the client
				int iRx  = socketData.m_currentSocket.EndReceive (asyn);
				char[] chars = new char[iRx +  1];
				// Extract the characters as a buffer
				System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
				int charLen = d.GetChars(socketData.dataBuffer, 
					0, iRx, chars, 0);
				/**TODO
				 * Add Commands and such..
				 */
				System.String szData = new System.String(chars);
				string msg = "" + socketData.m_clientNumber + ":";
				//AppendToRichEditControl(msg + szData);

				// Send back the reply to the client
				//string replyMsg = "Server Reply:" + szData.ToUpper(); 
				// Convert the reply to byte array
				//byte[] byData = System.Text.Encoding.ASCII.GetBytes(replyMsg);

				//Socket workerSocket = (Socket)socketData.m_currentSocket;
				//workerSocket.Send(byData);
	
				// Continue the waiting for data on the Socket
				WaitForData(socketData.m_currentSocket, socketData.m_clientNumber );

			}
			catch (ObjectDisposedException )
			{
				System.Diagnostics.Debugger.Log(0,"1","\nOnDataReceived: Socket has been closed\n");
			}
			catch(SocketException se)
			{
				if(se.ErrorCode == 10054) // Error code for Connection reset by peer
				{	
					ConsoleMessage("Client " + socketData.id + " Disconnected", false);
					// Remove the reference to the worker socket of the closed client
					// so that this object will get garbage collected
					m_workerSocketList[socketData.m_clientNumber - 1] = null;
				}
				else
				{
					Server.ErrorLog(se);
					ConsoleMessage("Error, please check the error log for more info!", true);
				}
			}
		}
		public void SendToAll(string message)
		{
			try 
			{
				byte[] byData = System.Text.Encoding.ASCII.GetBytes(message);
				Remote_Clients r = null;
				for (int i = 0; i < clients.Count; i++)
				{
					r = clients[i];
					if (r.s != null)
					{
						if (r.s.Connected)
							r.s.Send(byData);
					}
				}
			}
			catch (SocketException se)
			{
				Server.ErrorLog(se);
				ConsoleMessage("Error, please check the error log for more info!", true);
			}
		}
		String GetIP()
		{	   
			String strHostName = Dns.GetHostName();
		
			// Find host by name
			IPHostEntry iphostentry = Dns.GetHostByName(strHostName);
		
			// Grab the first IP addresses
			String IPStr = "";
			foreach(IPAddress ipaddress in iphostentry.AddressList)
			{
				IPStr = ipaddress.ToString();
				return IPStr;
			}
			return IPStr;
		}
		public override void Unload (bool shutdown)
		{
			ConsoleMessage("Closing Sockets");
			CloseSockets();
			ConsoleMessage("Sockets have been closed!");
			ConsoleMessage(welcome);
		}
		void CloseSockets()
		{
			if(m_mainSocket != null)
				m_mainSocket.Close();
			Socket workerSocket = null;
			for(int i = 0; i < m_workerSocketList.Count; i++)
			{
				workerSocket = (Socket)m_workerSocketList[i];
				if(workerSocket != null)
				{
					workerSocket.Close();
					workerSocket = null;
				}
			}	
		}
		void SendMsgToClient(string msg, int clientNumber)
		{
			// Convert the reply to byte array
			byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);

			Socket workerSocket = (Socket)m_workerSocketList[clientNumber - 1];
			workerSocket.Send(byData);
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			richTextBoxReceivedMsg.Clear();
		}
		public Remote_Clients getclient(int id)
		{
			foreach (Remote_Clients r in clients)
			{
				if (r.id == id)
					return r;
			}
			return null;
		}
	}
	public class Remote_Clients
	{
		public Socket s;
		public string name;
		public int id;
		// Buffer to store the data sent by the client
		public byte[] dataBuffer = new byte[1024];
		public Remote_Clients (Socket socket, int id_number)
		{
			id = id_number;
			s = socket;
		}
	}
}


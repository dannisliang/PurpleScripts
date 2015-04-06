using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using PurpleNetwork.Server;
using Entities.PurpleNetwork.Server;
using Entities.Database;
using PurpleDatabase;


// TODO: Class for monitoring - split it out later on


namespace PurpleNetwork
{
	public class PurpleNetworkServerSanityTester : MonoBehaviour
	{
		private static PurpleNetworkServerSanityTester instance;

		private static string formerIP;
		private static int formerPort;

		private static bool testDone;
		private static ConnectionTesterStatus testResult;

		private static pingData pingObject;

		private static PurpleCountdown countdown;

		private static ServerReference currentServerReference;

		private static PurpleCountdown sanityCountdown;
		private static PurpleCountdown sanityCountdownDone;

		private static bool server_sanity_database;
		private static bool server_sanity_network_ip;
		private static bool server_sanity_network_reachable;

		private class pingData
		{
			public Ping ping;
			public int lastPing;
			public IPAddress IP;
			public string host;
			public bool done;

			public pingData()
			{
				ping = null;
				lastPing = -1;
				IP = null;
				host = String.Empty;
				done = true;
			}
		}

		// START UP /////////////////////////
		protected PurpleNetworkServerSanityTester ()
		{
			formerIP = Network.connectionTesterIP;
			formerPort = Network.connectionTesterPort;

			testDone = true;
			testResult = ConnectionTesterStatus.Undetermined;

			currentServerReference = null;
			pingObject = new pingData ();
		}


		// SINGLETON /////////////////////////
		private static PurpleNetworkServerSanityTester Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject 	= new GameObject ("PurpleNetworkTester");
					instance     			= gameObject.AddComponent<PurpleNetworkServerSanityTester> ();
				}
				return instance;
			}
		}


		// PUBLIC FUNCTIONS /////////////////////////
		// SERVER SANITY CHECK /////////////////////////
		public static void ServerSanityCheck()
		{
			Instance.server_sanity_check ();
		}


		// SIMPLE PING /////////////////////////
		public static int Ping(IPAddress ipAddress)
		{
			return Instance.ping(ipAddress);
		}

		public static int Ping(string host)
		{
			return Instance.ping(host);
		}


		// ADVANCED FUNCTIONS /////////////////////////
		public static string Run()
		{
			return Instance.run_test (30).ToString();
		}

		public static string Run(string ipAddress, int port)
		{
			return Instance.run_test (ipAddress, port, 30).ToString();
		}

		public static string Run(ServerReference reference)
		{
			return Instance.run_test (reference).ToString();
		}

		public static string Run(string ipAddress, int port, int timeout)
		{
			return Instance.run_test (ipAddress, port, timeout).ToString();
		}

		public static bool IsTestDone
		{
			get
			{
				return Instance.get_test_state();
			}
		}

		public static string TestResult
		{
			get
			{
				return Instance.get_test_result().ToString();
			}
		}

		public static bool TestDatabase(string ip, string externalIp)
		{
			return Instance.test_current_database_connection (ip, externalIp);
		}



		// PRIVATE FUNCTIONS /////////////////////////
		private int ping(IPAddress ipAddress)
		{
			if(pingObject.IP != ipAddress)
			{
				pingObject.IP = ipAddress;
			}
			return ping ();
		}

		private int ping(string host)
		{
			if(pingObject.host != host)
			{
				pingObject.host = host;
				pingObject.IP = Dns.GetHostEntry(host).AddressList.First();
			}
			return ping ();
		}

		private int ping()
		{
			if(pingObject.ping != null && pingObject.ping.isDone)
				pingObject.lastPing = pingObject.ping.time;

			if(pingObject.ping == null || pingObject.ping.isDone)
				pingObject.ping = new Ping (pingObject.IP.ToString());

			return pingObject.lastPing;
		}


		// ADVANCED FUNCTIONS /////////////////////////
		private ConnectionTesterStatus run_test(ServerReference reference)
		{
			if(testDone)
			{
				currentServerReference = reference;
			}
			return run_test (currentServerReference.ServerHost, currentServerReference.ServerPort, currentServerReference.TesterTimeout);
		}

		private ConnectionTesterStatus run_test(string ipAddress, int port, int timeout)
		{
			if(testDone)
			{
				testDone = false;
				init_connection (ipAddress, port);
				countdown = PurpleCountdown.NewInstance ("ConnectionTesterStatus");
				countdown.CountdownRunEvent += test_connection;
				countdown.CountdownDoneEvent += reset_connection;
				countdown.CountDown (timeout);
			}
			return testResult;
		}

		private ConnectionTesterStatus run_test(int timeout)
		{
			if(testDone)
			{
				testDone = false;
				countdown = PurpleCountdown.NewInstance ("ConnectionTesterStatus");
				countdown.CountdownRunEvent += test_connection;
				countdown.CountdownDoneEvent += reset_connection;
				countdown.CountDown (timeout);
			}
			return testResult;
		}


		private bool get_test_state()
		{
			return testDone;
		}

		private ConnectionTesterStatus get_test_result()
		{
			return testResult;
		}

		// PRIVATE HELPER /////////////////////////
		private void init_connection(string ipAddress, int port)
		{
			formerIP = Network.connectionTesterIP;
			Network.connectionTesterIP = ipAddress;

			formerPort = Network.connectionTesterPort;
			Network.connectionTesterPort = port;
		}

		private void test_connection()
		{
			testResult = Network.TestConnection();
			if(testResult != ConnectionTesterStatus.Undetermined)
			{
				countdown.CancelCountDown();
				reset_connection();
			}
		}

		private void reset_connection()
		{
			countdown.CountdownRunEvent -= test_connection;
			countdown.CountdownDoneEvent -= reset_connection;
			countdown.DestroyInstance ();

			testDone = true;

			Network.connectionTesterIP = formerIP;
			Network.connectionTesterPort = formerPort;
		}



		// UNTESTED FUNCTIONS /////////////////////////
		private bool test_client_server_connecion(ConnectionTesterStatus type1, ConnectionTesterStatus type2)
		{
			if (type1 == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted &&
			    type2 == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric)
				return false;
			else if (type1 == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric &&
			         type2 == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted)
				return false;
			else if (type1 == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric &&
			         type2 == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric)
				return false;
			return true;
		}


		// TESTER FUNCTIONS /////////////////////////
		// TODO: MOVE
		/*
		public bool Test(List <ServerReference> serverList)
		{
			// check all server availabilities
			bool returnValue = true;
			foreach(ServerReference sr in serverList)
			{
				ServerReference newSR = new ServerReference();
				bool pingReturn = Test(sr, out newSR);
				sr.ReferencePingNote = newSR.ReferencePingNote;
				sr.ServerState = newSR.ServerState;
				sr.ReferenceLastSeen = newSR.ReferenceLastSeen;

				if(returnValue)
					returnValue = pingReturn;
			}
			return returnValue;
		}
		*/
		/*
		public static ServerReference Test(ServerReference reference)
		{
			return Ping (reference.ServerHost);
		}

		public static ServerReference Test(ServerReference reference)
		{
			string pingMessage = String.Empty;
			ServerReference newRefernece = reference;
			bool pingReturn = Ping(reference.ServerHost, out pingMessage);

			newRefernece.ReferencePingNote = pingMessage;
			if(pingReturn)
			{
				newRefernece.ServerState = ServerStates.Online;
				newRefernece.ReferenceLastSeen = DateTime.Now;
			}
			else
			{
				newRefernece.ServerState = ServerStates.Offline;
			}
			return newRefernece;
		}

		public bool Test(ServerReference reference, out string pingMessage)
		{
			return Ping (reference.ServerHost, out pingMessage);
		}
		*/


		private void server_sanity_check()
		{
			server_sanity_database = false;
			server_sanity_network_ip = false;
			server_sanity_network_reachable = false;

			int testTime = 30;

			Debug.Log ("ServerSanityCheck: Start...");
			Debug.Log ("ServerSanityCheck: Initialize calls...");
			run_test (testTime-2);
			string ipAddress = Network.player.ipAddress;
			string externalIP = Network.player.externalIP;

			Debug.Log ("ServerSanityCheck: Start Database check...");
			if(test_current_database_connection (ipAddress, externalIP))
			{
				Debug.Log ("ServerSanityCheck: Database OK");
				server_sanity_database = true;
			}
			else
			{
				Debug.LogError("ServerSanityCheck: Database ERROR");
			}

			Debug.Log ("ServerSanityCheck: Start network check...");
			Debug.Log ("ServerSanityCheck: Local IP Address " + ipAddress);
			Debug.Log ("ServerSanityCheck: External IP Address " + externalIP);
			if(!string.IsNullOrEmpty(externalIP)
			   && !externalIP.StartsWith("0.")
			   && !externalIP.StartsWith("127.")
			   && !externalIP.StartsWith("192.")
			   && !externalIP.Contains("UNASSIGNED_SYSTEM_ADDRESS"))
			{
				server_sanity_network_ip = true;
			}

			sanityCountdown = PurpleCountdown.NewInstance ("SanityCheck");
			sanityCountdown.TriggerEvent += server_sanity_check_periodical;
			sanityCountdown.Trigger (5, testTime/10);

			sanityCountdownDone = PurpleCountdown.NewInstance ("SanityCheckKill");
			sanityCountdownDone.CountdownDoneEvent += server_sanity_check_done;
			sanityCountdownDone.CountDown (testTime);
		}

		private void server_sanity_check_periodical()
		{
			if(testDone)
			{
				Debug.Log ("ServerSanityCheck: Network Testresult " + testResult.ToString());
				if(testResult != ConnectionTesterStatus.Error &&
				   testResult != ConnectionTesterStatus.Undetermined)
				{
					server_sanity_network_reachable = true;
				}
				server_sanity_check_done();
			}
			else
			{
				Debug.Log ("ServerSanityCheck: Network Testresult still running...");
			}
		}

		private void server_sanity_check_done()
		{
			sanityCountdown.TriggerEvent -= server_sanity_check_periodical;
			sanityCountdown.CancelTrigger();
			sanityCountdown.DestroyInstance();

			sanityCountdownDone.CountdownDoneEvent -= server_sanity_check_done;
			sanityCountdownDone.CancelCountDown();
			sanityCountdownDone.DestroyInstance();

			if(server_sanity_database && server_sanity_network_ip && server_sanity_network_reachable)
			{
				Debug.Log("ServerSanityCheck: Success!");
			}
			else
			{
				Debug.LogError("ServerSanityCheck: ERROR!");
				if(!server_sanity_database)
					Debug.LogError("ServerSanityCheck: Database unreachable!");

				if(!server_sanity_network_ip)
					Debug.LogError("ServerSanityCheck: No external IP!");

				if(!server_sanity_network_reachable)
					Debug.LogError("ServerSanityCheck: Network Error!");

				Debug.LogError("ServerSanityCheck: Shut down server...");
				if(PurpleServer.CurrentConfig.SanityTest)
				{
					switch (PurpleServer.CurrentConfig.SanityAction.ToLower())
					{
					case "shutdown":
						PurpleServer.StopServer(5);
						break;

					case "restart":
						PurpleServer.RestartServer(5);
						break;

					case "warn":
						Debug.LogError("ServerSanityCheck: Shutdown/Restart recommended!");
						break;
					}
				}
			}
		}

		private bool test_current_database_connection(string ip, string externalIp)
		{
			Entities.Database.PurpleServerLog psl = new Entities.Database.PurpleServerLog ();
			psl.name = PurpleServer.CurrentConfig.ServerName;
			psl.port = PurpleServer.CurrentConfig.ServerPort;
			psl.max_player = PurpleServer.CurrentConfig.ServerMaxClients;
			psl.host = PurpleServer.CurrentConfig.ServerHost;
			psl.comment = String.Empty;
			psl.type = PurpleServer.CurrentConfig.ServerType.ToString();
			psl.local_ip = ip;
			psl.global_ip = externalIp;

			int insertResult = psl.ToSQLInsert ().Execute ();
			if (insertResult == 1)
			{
				if(PurpleDatabase.PurpleDatabase.LastInsertedId() > 0)
					return true;
			}

			return false;
		}
	}
}

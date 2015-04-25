using UnityEngine;
		Debug.Log("Server Manager Started!");
		PurpleLog.AddListener("start", launch_server_command);
		PurpleLog.AddListener("stop", stop_server_command);
		PurpleLog.AddListener("restart", restart_server_command);
		PurpleLog.AddListener("debug", debug_server_command);

			GUI.Label(new Rect(10, 10, 200, 20), "Status: Disconnected");
				launch_server();
				GUI.Label(new Rect(450, 30, 200, 20), "IP: " + Network.player.ipAddress + ":" + Network.player.port);
				GUI.Label(new Rect(450, 50, 200, 20), "Uptime: " + PurpleNetwork.Server.PurpleServer.ServerUptime());

					debug_data();
		}
		GUI.Label(new Rect(10, Screen.height - 30, 200, 20), "Press '^' for console window.");

	private void debug_data()
	{
		Debug.Log("Max Player: " + PurpleConfig.Network.MaxPlayer);
		Debug.Log("Port: " + PurpleConfig.Network.Port);
		Debug.Log("Password: " + PurpleConfig.Network.Password);
		Debug.Log("Connections: " + Network.connections.Length);
		Debug.Log("UserList: " + PurpleSerializer.ObjectToStringConverter(PurpleNetwork.Server.PurpleServer.UserList));
	}

	private void launch_server()
	{
		Debug.Log("Start Server!");
		Entities.PurpleNetwork.Server.ServerConfig psc = new Entities.PurpleNetwork.Server.ServerConfig();
		psc.ServerMaxClients = numberOfPlayers;
		psc.ServerPassword = connectionPassword;
		psc.ServerPort = connectionPort;
		PurpleNetwork.Server.PurpleServer.LaunchServer(psc);

		Debug.Log(numberOfPlayers + " - - " + connectionPassword + " - - " + connectionPort);
	}


	string launch_server_command(string[] args)
	{
		if (args.Length == 2 && PurpleLog.IsHelpRequired(args[1]))
		{
			Debug.Log("Launch the server.");
			return string.Empty;
		}

		launch_server();
		return string.Empty;
	}

	string stop_server_command(string[] args)
	{
		if (args.Length == 2 && PurpleLog.IsHelpRequired(args[1]))
		{
			Debug.Log("Stop the server.");
			return string.Empty;
		}

		PurpleNetwork.Server.PurpleServer.StopServer(5);
		return string.Empty;
	}

	string restart_server_command(string[] args)
	{
		if (args.Length == 2 && PurpleLog.IsHelpRequired(args[1]))
		{
			Debug.Log("Restart the server.");
			return string.Empty;
		}

		PurpleNetwork.Server.PurpleServer.RestartServer(5);
		return string.Empty;
	}

	string debug_server_command(string[] args)
	{
		debug_data();
		return string.Empty;
	}
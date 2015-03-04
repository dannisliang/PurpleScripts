using System;
namespace PurpleConfig
{
	// Networking /////////////////////////
	public static class Network {
		public static string IP { get { return ItemIds.getConfigString ("Network.IP"); } }
		public static string Host { get { return ItemIds.getConfigString ("Network.Host"); } }
		public static int Port { get { return ItemIds.getConfigInt ("Network.Port"); } }
		public static int MaxPlayer { get { return ItemIds.getConfigInt ("Network.MaxPlayer"); } }
		public static string Password { get { return ItemIds.getConfigString ("Network.Password"); } }
		public static int Pause { get { return ItemIds.getConfigInt ("Network.Pause"); } }

		public static class Server {
			public static string Name { get { return ItemIds.getConfigString ("Network.Server.Name"); } }
			public static string Type { get { return ItemIds.getConfigString ("Network.Server.Type"); } }
			public static string IPScript { get { return ItemIds.getConfigString ("Network.Server.IPScript"); } }
			public static int Delay { get { return ItemIds.getConfigInt ("Network.Server.ActionDelay"); } }
		}

		public static class Message {
			public static string Placeholder { get { return ItemIds.getConfigString ("Network.Message.Placeholder"); } }
		}
	}
	
	// Globalization /////////////////////////
	public static class Globalization {
		public static string Culture { get { return ItemIds.getConfigString ("Globalization.Culture"); } }
	}

	// Storage /////////////////////////
	public static class Storage {
		public static bool ForcePlayerPrefs { get { return ItemIds.getConfigBoolean ("Storage.ForcePlayerPrefs"); } }

		public static class File {
			public static string Extension { get { return ItemIds.getConfigString ("Storage.File.Extension"); } }
			public static string AlternativePath { get { return ItemIds.getConfigString ("Storage.File.AlternativePath"); } }
			public static string MetaName { get { return ItemIds.getConfigString ("Storage.File.MetaName"); } }
		}
	}

	// Console Log /////////////////////////
	public static class ConsoleLog {
		public static bool Enabled { get { return ItemIds.getConfigBoolean ("ConsoleLog.Enabled"); } }
		public static int History { get { return ItemIds.getConfigInt ("ConsoleLog.History"); } }

		public static class Color {
			public static string Log { get { return ItemIds.getConfigString ("ConsoleLog.Color.Log"); } }
			public static string Error { get { return ItemIds.getConfigString ("ConsoleLog.Color.Error"); } }
			public static string Warning { get { return ItemIds.getConfigString ("ConsoleLog.Color.Warning"); } }
			public static string User { get { return ItemIds.getConfigString ("ConsoleLog.Color.User"); } }
		}
	}

	// Database /////////////////////////
	public static class Database {
		public static string IP { get { return ItemIds.getConfigString ("Database.IP"); } }
		public static string Name { get { return ItemIds.getConfigString ("Database.Name"); } }
		public static string User { get { return ItemIds.getConfigString ("Database.User"); } }
		public static string Password { get { return ItemIds.getConfigString ("Database.Password"); } }
		public static int Port { get { return ItemIds.getConfigInt ("Database.Port"); } }
	}

	// Password generation/validation /////////////////////////
	public static class Password {
		public static int SaltByteSize { get { return ItemIds.getConfigInt ("Password.SaltByteSize"); } }
		public static int HashByteSize { get { return ItemIds.getConfigInt ("Password.HashByteSize"); } }
		public static int IterationIndex { get { return ItemIds.getConfigInt ("Password.IterationIndex"); } }
		public static int SaltIndex { get { return ItemIds.getConfigInt ("Password.SaltIndex"); } }

		public static class PBKDF2 {
			public static int Iterations { get { return ItemIds.getConfigInt ("Password.PBKDF2.Iterations"); } }
			public static int Index { get { return ItemIds.getConfigInt ("Password.PBKDF2.Index"); } }
		}
	}

	// License /////////////////////////
	public static class License {
		public static int KeySize { get { return ItemIds.getConfigInt ("License.KeySize"); } }
		public static string CryptoConfig { get { return ItemIds.getConfigString ("License.CryptoConfig"); } }

		public static class XMLKey {
			public static string Private { get { return ItemIds.getConfigString ("License.XMLKey.Private"); } }
			public static string Public { get { return ItemIds.getConfigString ("License.XMLKey.Public"); } }
		}
	}

	// Mail /////////////////////////
	public static class Mail {
		public static class Server {
			public static string User { get { return ItemIds.getConfigString ("Mail.Server.User"); } }
			public static string Password { get { return ItemIds.getConfigString ("Mail.Server.Password"); } }
			public static string Host { get { return ItemIds.getConfigString ("Mail.Server.Host"); } }
			public static int Port { get { return ItemIds.getConfigInt ("Mail.Server.Port"); } }
			public static bool SSL { get { return ItemIds.getConfigBoolean ("Mail.Server.SSL"); } }
		}

		public static class Sender {
			public static string Address { get { return ItemIds.getConfigString ("Mail.Sender.Address"); } }
			public static string Name { get { return ItemIds.getConfigString ("Mail.Sender.Name"); } }
		}
	}
}
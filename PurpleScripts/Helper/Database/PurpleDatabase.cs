﻿/**
 * If you get an error like:
 * 		Internal compiler error. ... Could not load file or assembly 'System.Drawing,
 * 		Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies.
 * change the API compatibility Level from ".NET 2.0 Subset" to ".NET 2.0"!
 *
 * Also see: http://answers.unity3d.com/questions/379212/how-to-solve-the-error-type-or-namespace-systemdat.html
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace PurpleDatabase
{
	public class PurpleDatabase : MonoBehaviour {

		private static string serverIP;
		private static string serverDatabase;
		private static string serverUser;
		private static string serverPassword;
		private static int serverPort;

		private static bool versionValidate;
		private static string versionRequired;

		private static string connectionString;
		private static MySqlConnection connection;

		private static PurpleDatabase instance;

		private static long lastInsertedId;

		protected PurpleDatabase () {
			lastInsertedId = -1;
			try
			{
				serverIP = PurpleConfig.Database.IP;
				serverDatabase = PurpleConfig.Database.Name;
				serverUser = PurpleConfig.Database.User;
				serverPassword = PurpleConfig.Database.Password;
				serverPort = PurpleConfig.Database.Port;

				versionValidate = PurpleConfig.Database.Version.Validate;
				versionRequired = PurpleConfig.Database.Version.Required;
			}
			catch(Exception e)
			{
				PurpleDebug.LogError("Can not read Purple Config! " + e.ToString(), 1);
			}
		}


		// SINGLETON /////////////////////////
		private static PurpleDatabase Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject 	= new GameObject ("PurpleDatabase");
					instance     			= gameObject.AddComponent<PurpleDatabase> ();
					instance.initialize();
				}
				return instance;
			}
		}


		// SETUP ////////////////////////////
		public static void Setup (string host, string database, string user, string password)
		{
			Instance.purple_setup (host, database, user, password);
		}

		public static void Setup (string host, string database, string user, string password, int port)
		{
			Instance.purple_setup (host, database, user, password, port);
		}

		public static void SwitchDatabase(string database)
		{
			Instance.switch_database (database);
		}

		public static void Initialize()
		{
			Instance.open_connection ();
			Instance.close_connection ();
		}


		// ExecuteQuery - INSERT, UPDATE, and DELETE
		public static int ExecuteQuery(string query)
		{
			return Instance.sql_write_statement (query);
		}


		// SELECT
		public static DataTable SelectQuery(string query)
		{
			int rowCount = 0, colCount = 0;
			return Instance.sql_read_statement (query, out rowCount, out colCount);
		}

		public static DataTable SelectQuery(string query, out int rowCount)
		{
			int colCount = 0;
			return Instance.sql_read_statement (query, out rowCount, out colCount);
		}

		public static DataTable SelectQuery(string query, out int rowCount, out int colCount)
		{
			return Instance.sql_read_statement (query, out rowCount, out colCount);
		}


		// SELECT One Result
		public static DataRow SelectQuerySingle(string query)
		{
			int colCount;
			DataColumnCollection colums;
			return SelectQuerySingle (query, out colCount, out colums);
		}

		public static DataRow SelectQuerySingle(string query, out DataColumnCollection colums)
		{
			int colCount;
			return SelectQuerySingle (query, out colCount, out colums);
		}

		public static DataRow SelectQuerySingle(string query, out int colCount)
		{
			DataColumnCollection colums;
			return SelectQuerySingle (query, out colCount, out colums);
		}

		public static DataRow SelectQuerySingle(string query, out int colCount, out DataColumnCollection colums)
		{
			int rowCount;
			DataTable dt = Instance.sql_read_statement (query, out rowCount, out colCount);

			colums = dt.Columns;
			if(rowCount > 0)
				return dt.Rows[0];
			return null;
		}

		public static string MySQLEscapeString(string str)
		{
			return Instance.my_sql_escape (str);
		}

		public static bool IsSQLValid(string query, bool writeStatement)
		{
			return Instance.is_sql_valid (query, writeStatement);
		}

		public static long LastInsertedId()
		{
			return Instance.sql_last_id ();
		}


		// PRIVATE ////////////////////////////
		// SETUP ////////////////////////////
		private void purple_setup(string host, string database, string user, string password, int port)
		{
			serverPort = port;
			purple_setup(host, database, user, password);
		}

		private void purple_setup(string host, string database, string user, string password)
		{
			serverIP = host;
			serverDatabase = database;
			serverUser = user;
			serverPassword = password;
			initialize (true);
		}

		private void switch_database(string database)
		{
			serverDatabase = database;
			initialize (true);
		}


		// SQL FUNCTIONS ////////////////////////////
		// CREATE, INSERT, UPDATE, and DELETE
		private int sql_write_statement(string query)
		{
			if (!is_sql_valid (query, true))
				return 0;

			int affectedRows = 0;
			if (open_connection () == true)
			{
				try
				{
					MySqlCommand cmd = new MySqlCommand (query, connection);
					affectedRows = cmd.ExecuteNonQuery ();
					lastInsertedId = cmd.LastInsertedId;
				}
				catch (Exception ex)
				{
					PurpleDebug.LogError("Can not execute query. " + ex.ToString(), 1);
				}
				close_connection ();
			}
			return affectedRows;
		}

		// SELECT
		private DataTable sql_read_statement(string query, out int rowCount, out int colCount)
		{
			colCount = 0;
			rowCount = 0;
			DataTable dt = new DataTable();
			dt.Clear();

			if (!is_sql_valid (query, false))
				return dt;

			if (open_connection () == true)
			{
				MySqlDataReader reader = null;
				try
				{
					MySqlCommand cmd = new MySqlCommand (query, connection);
					reader = cmd.ExecuteReader ();

					if(reader.HasRows)
					{
						colCount = reader.FieldCount;
						bool first_run = true;

						while (reader.Read())
						{
							rowCount++;
							DataRow dt_row = dt.NewRow();

							for(int i=0;i<reader.FieldCount;i++)
							{
								if(first_run)
								{
									dt.Columns.Add(reader.GetName(i), reader.GetType());
								}
								dt_row[i] = reader[i];
							}

							dt.Rows.Add(dt_row);
							first_run = false;
						}
					}
				}
				catch (Exception ex)
				{
					PurpleDebug.LogError("Can not execute query. " + ex.ToString(), 1);
				}
				finally
				{
					if (reader != null)
					{
						reader.Close();
					}
				}
				close_connection ();
			}
			return dt;
		}

		private long sql_last_id()
		{
			return lastInsertedId;
		}


		// HELPER ////////////////////////////
		private string my_sql_escape(string str)
		{
			return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
				delegate(Match match)
				{
					string v = match.Value;
					switch (v)
					{
					case "\x00":	 	// ASCII NUL (0x00) character
						return "\\0";
					case "\b":		 	// BACKSPACE character
						return "\\b";
					case "\n":		 	// NEWLINE (linefeed) character
						return "\\n";
					case "\r":		 	// CARRIAGE RETURN character
						return "\\r";
					case "\t":			// TAB
						return "\\t";
					case "\u001A":	 	// Ctrl-Z
						return "\\Z";
					}
					return "\\" + v;
				}
			);
		}

		private bool is_sql_valid(string query)
		{
			return is_sql_valid (query, true);
		}

		private bool is_sql_valid(string query, bool isWrite)
		{
			string[] invalidSymbols = new string[] { "--", ";--", "/*", "*/", "@@", "''" };
			string[] invalidWords = {};

			if(isWrite)
			{
				// WRITE
				invalidWords = new string[] { "char", "nchar", "varchar", "nvarchar", "alter",
					"begin", "cast", "create", "cursor", "declare", "drop", "end", "exec",
					"execute", "fetch", "kill", "sys", "sysobjects", "syscolumns", "table" };
			}
			else
			{
				// READ
				invalidWords = new string[] { "char", "nchar", "varchar", "nvarchar", "alter",
					"begin", "cast", "create", "cursor", "declare", "delete", "drop", "end",
					"exec", "execute", "fetch", "insert", "kill", "sys", "sysobjects", "syscolumns",
					"table", "update", "''" };
			}

			string PatternTemplate = @"\b({0})(s?)\b";

			IEnumerable<Regex> badWordMatchers = invalidWords.
				Select(x => new Regex(string.Format(PatternTemplate, x), RegexOptions.IgnoreCase));

			string output = badWordMatchers.
				Aggregate(query, (current, matcher) => matcher.Replace(current, String.Empty));

			if (!query.Equals (output))
				return false;

			if (invalidSymbols.Any (query.ToLowerInvariant().Contains))
				return false;

			if (Regex.Matches (query, ";").Count > 1)
				return false;

			Regex outerRegex = new Regex(@"\S*\s?=");
			foreach (Match m in outerRegex.Matches(query))
			{
				string leftSide = m.Value.Trim(new char[] { '=', ' ' });
				Regex innerRegex = new Regex(leftSide+@"*\s?=\s?"+leftSide);

				if (innerRegex.Matches (query).Count > 0)
					return false;
			}

			return true;
		}


		// DATABASE BASICS /////////////////////////
		// initialize database
		private void initialize()
		{
			initialize (false);
		}

		private void initialize(bool force)
		{
			if(connection == null || force)
			{
				try
				{
					connectionString = "Server="+serverIP+";" +
						"Database="+serverDatabase+";" +
						"User ID="+serverUser+";" +
						"Pooling=false;" +
						"Password="+serverPassword+";" +
						"Port="+serverPort+";";
					connection = new MySqlConnection (connectionString);

					if (versionValidate)
					{
						PurpleVersion versionRequired = new PurpleVersion(versionRequired);
						PurpleVersion databaseVerion = new PurpleVersion(Entities.Database.PurpleDatabase.CurrentVersion());
						if(!versionRequired.AreEqual(databaseVerion))
						{
							PurpleDebug.LogError ("PurpleDatabase: Database version does not match!", 1);
						}
					}
				}
				catch (Exception ex)
				{
					connection = null;
					PurpleDebug.Log(ex.ToString());
				}
			}
		}

		// open DB connection
		private bool open_connection()
		{
			try
			{
				if(connection != null)
				{
					connection.Open();
					return true;
				}
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
				{
					case 0:
						PurpleDebug.Log("Cannot connect to server.", 1);
						PurpleDebug.LogWarning(ex);
						break;

					case 1042:
						PurpleDebug.Log("Unable to connect to any of the specified MySQL hosts.", 1);
						PurpleDebug.LogWarning(ex);
						break;

					case 1045:
						PurpleDebug.Log("Invalid username/password.", 1);
						PurpleDebug.LogWarning(ex);
						break;

					default:
						PurpleDebug.Log(ex.Number + " - " + ex.Message, 1);
						PurpleDebug.LogWarning(ex);
						break;
				}
				connection = null;
			}
			return false;
		}

		// close DB connection
		private bool close_connection()
		{
			try
			{
				if(connection != null)
				{
					connection.Close();
					return true;
				}
			}
			catch (MySqlException ex)
			{
				PurpleDebug.Log(ex.Message);
				connection = null;
			}
			return false;
		}
	}
}

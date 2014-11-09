using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;


// PlayerPrefs - Work in WebPlayer
//http://docs.unity3d.com/ScriptReference/PlayerPrefs.html 

// as well as File saving - does not work in web player!?

// perhaps own object for storage???

// how to format data

namespace PurpleStorage
{
	public class PurpleStorage : MonoBehaviour
	{
		private static PurpleStorage instance;

		private static string fileEnding;
		private static bool binaryFormat;
		private static string alternativePath;
		private static bool forcePlayerPrefs;


		// START UP /////////////////////////
		protected PurpleStorage ()
		{
			try{
				fileEnding = "."+PurpleConfig.Storage.File.Extension.TrimStart('.');
				forcePlayerPrefs = PurpleConfig.Storage.ForcePlayerPrefs;		// ???
				alternativePath = PurpleConfig.Storage.File.AlternativePath;	// alt store path
				binaryFormat = PurpleConfig.Storage.File.Binary;				// binary format or raw
			} catch(Exception e){
				fileEnding = ".data";
				forcePlayerPrefs = false;
				alternativePath = String.Empty;
				binaryFormat = true;
				Debug.LogError("Can not read Purple Config! " + e.ToString());
			}
		}


		// SINGLETON /////////////////////////
		public static PurpleStorage Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject 	= new GameObject ("PurpleStorageManager");
					instance     			= gameObject.AddComponent<PurpleStorage> ();
				}
				return instance;
			}
		}

		
		// PUBLIC FUNCTIONS /////////////////////////
		public static void Test()
		{
			Instance.tester ();
		}

		
		// PRIVATE FUNCTIONS /////////////////////////
		private void tester()
		{
			Debug.Log (Application.persistentDataPath + "/" + "-- dummy --" + fileEnding);
			Debug.LogWarning (PurpleConfig.Storage.File.Extension);
		}

// TODO: web player compatibility check
// TODO: JSON?
		public static void Save(string filename, object data) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create (Application.persistentDataPath + "/" + filename + fileEnding);
			bf.Serialize(file, data);
			file.Close();
		}   

// TODO: web player compatibility check
// TODO: JSON?
		public static object Load(string filename) {
			if(File.Exists(Application.persistentDataPath + "/" + filename + fileEnding)) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/" + filename + fileEnding, FileMode.Open);
				object data = bf.Deserialize(file);
				file.Close();
				return data;
			}
			return null;
		}
	}


	
	[Serializable]
	public class PurpleFileObject
	{
		public Guid guid;
		public string name;

		public DateTime created;
		public DateTime updated;

		public string dataString;
		public object dataObject;
		
		// CONSTRUCTOR
		public PurpleFileObject()
		{
			guid = System.Guid.NewGuid ();
			created = DateTime.Now;
		}
	}
}

using UnityEngine;
using PurpleDatabase;
using System.Data;
using NUnit.Framework;

namespace PurpleDatabaseWrapper
{
	[TestFixture]
	[Category("SELECT Tests")]
	public class SELECT_Test
	{
		[Test]
		[Category("SELECT Test")]
		public void Select_01 ()
		{
			string expected = "SELECT `test` FROM `one`;";
			string generated = SQLGenerator.Select (select: "test", from: "one");
			Assert.AreEqual (expected, generated);

			SQLGenerator.Reset ();

			generated = SQLGenerator.Select ("test");
			generated = SQLGenerator.From ("one");
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_02 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` LIMIT 5 OFFSET 10;";
			string generated = SQLGenerator.Select (select: "test, test2", from: "one", limit:5, offset:10 );
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test, test2", "one");
			generated = SQLGenerator.Limit (5, 10);
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_03 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` ORDER BY `test` ASC;";
			string generated = SQLGenerator.Select (select: "test, test2", from: "one", sorting:"test ASC" );
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test, test2", "one");
			generated = SQLGenerator.OrderBy ("test", "ASC");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test, test2", "one");
			generated = SQLGenerator.OrderBy ("test ASC");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("SELECT Test")]
		public void Select_04 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `test` = 'variable';";
			string generated = SQLGenerator.Select (select: "test, test2", from: "one", where:"test='variable'" );
			Assert.AreEqual (expected, generated);

			SQLGenerator.Reset ();

			generated = SQLGenerator.Select (select: "test, test2", from: "one", where:"test=variable" );
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_05 ()
		{
			string expected = "SELECT `test` FROM `one` WHERE `test` = 'Test';";
			string generated = SQLGenerator.Select ("test", "one", "test = 'Test'");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT `test`, `test2` FROM `one` WHERE `test` = 'Test';";
			generated = SQLGenerator.Select ("test2");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT `test`, `test2`, `test3` FROM `one` WHERE `test` = 'Test';";
			generated = SQLGenerator.Select ("test3");
			Assert.AreEqual (expected, generated);

			expected = "SELECT `test`, `test2`, `test3` FROM `one` WHERE `test` = 'Test' AND `test2` = 'Test12';";
			generated = SQLGenerator.Where ("test2='Test12'");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("SELECT Test")]
		public void Select_05_NoEscape ()
		{
			SQLGenerator.DisableEscape ();
			string expected = "SELECT test FROM one WHERE test = 'Test';";
			string generated = SQLGenerator.Select ("test", "one", "test = 'Test'");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT test, test2 FROM one WHERE test = 'Test';";
			generated = SQLGenerator.Select ("test2");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT test, test2, test3 FROM one WHERE test = 'Test';";
			generated = SQLGenerator.Select ("test3");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT test, test2, test3 FROM one WHERE test = 'Test' AND test2 = 'Test12';";
			generated = SQLGenerator.Where ("test2='Test12'");
			Assert.AreEqual (expected, generated);
			SQLGenerator.EnableEscape ();
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_05_NoStringEscape ()
		{
			string expected = "SELECT `test` FROM `one` WHERE `test` = 'Test';";
			string generated = SQLGenerator.Select ("test", "one", "test = Test");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT `test`, `test2` FROM `one` WHERE `test` = 'Test';";
			generated = SQLGenerator.Select ("test2");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT `test`, `test2`, `test3` FROM `one` WHERE `test` = 'Test';";
			generated = SQLGenerator.Select ("test3");
			Assert.AreEqual (expected, generated);
			
			expected = "SELECT `test`, `test2`, `test3` FROM `one` WHERE `test` = 'Test' AND `test2` = 'Test12';";
			generated = SQLGenerator.Where ("test2=Test12");
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_06 ()
		{
			string expected = "SELECT * FROM `one`;";
			string generated = SQLGenerator.Select (from: "one");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();

			generated = SQLGenerator.Select (select: "*", from: "one");
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_07 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one`;";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_08 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `numbervalue` >= 10;";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue >= 10");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test");
			generated = SQLGenerator.From ("one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue >= 10");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("SELECT Test")]
		public void Select_09 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `numbervalue` = 10 AND `test` = 'test';";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("test = 'test'");
			Assert.AreEqual (expected, generated);

			SQLGenerator.Reset ();

			generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("AND test = 'test'");
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_10 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `numbervalue` = 10 OR `test` = 'test';";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("OR test = 'test'");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("test = 'test'", "OR");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test");
			generated = SQLGenerator.Select ("test2");
			generated = SQLGenerator.From ("one");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("test = 'test'", "OR");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("SELECT Test")]
		public void Select_11 ()
		{
			string expected = "SELECT `test` FROM `one` WHERE `test` LIKE '%est%';";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Where ("test LIKE '%est%'");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Like ("test", "%est%");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Select ("test");
			generated = SQLGenerator.From ("one");
			generated = SQLGenerator.Like ("test", "'%est%'");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("SELECT Test")]
		public void Select_12 ()
		{
			string expected = "SELECT `test` FROM `one` WHERE `numbervalue` = 10 OR `test` LIKE '%est%';";
			string generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Where ("test LIKE '%est%'", "OR");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();

			generated = SQLGenerator.Select ("test", "one");
			generated = SQLGenerator.Where ("numbervalue = 10");
			generated = SQLGenerator.Like ("test", "%est%", "OR");
			Assert.AreEqual (expected, generated);
		}	

		[Test]
		[Category("SELECT Test")]
		public void Select_13 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `test` IN (SELECT `dummy` FROM `two` WHERE `dummy` = 'one');";
			string generated = "";
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_14 ()
		{
			string expected = "SELECT `test`, `test2` FROM `one` WHERE `test` IN ('value1', 'value2', 'value3');";
			string generated = "";
			Assert.AreEqual (expected, generated);
		}

		[Test]
		[Category("SELECT Test")]
		public void Select_15 ()
		{
			string expected = "SELECT `one`.`test`, `two`.`dummy` FROM `one`, `two` WHERE `one`.`test2` = `two`.`dummy2`;";
			string generated = "";
			Assert.AreEqual (expected, generated);
		}
	}


	[TestFixture]
	[Category("UPDATE Tests")]
	public class UPDATE_Test
	{		
		[Test]
		[Category("UPDATE Test")]
		public void Update_1 ()
		{
			string expected = "UPDATE `tabletwo` SET `one` = NULL WHERE `two` = 'Test';";
			string generated = SQLGenerator.Update ("one=null", "tabletwo",  "two = 'Test'");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();

			generated = SQLGenerator.Table ("tabletwo");
			generated = SQLGenerator.Update ("one=null");
			generated = SQLGenerator.Where ("two = 'Test'");
			Assert.AreEqual (expected, generated);
		}
		
		[Test]
		[Category("UPDATE Test")]
		public void Update_2 ()
		{
			string expected = "UPDATE `tabletwo` SET `one` = NULL, `three` = 'dummy2' WHERE `two` = 'Test2';";
			string generated = SQLGenerator.Update ("one=null", "tabletwo",  "two = 'Test2'");
			generated = SQLGenerator.Update ("three=dummy2");
			Assert.AreEqual (expected, generated);
			
			SQLGenerator.Reset ();
			
			generated = SQLGenerator.Table ("tabletwo");
			generated = SQLGenerator.Update ("one=null");
			generated = SQLGenerator.Update ("three=dummy2");
			generated = SQLGenerator.Where ("two = 'Test2'");
			Assert.AreEqual (expected, generated);

		}
	}
}

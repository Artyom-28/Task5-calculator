using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Calculator.Tests
{
	[TestClass()]
	public class CalculatorTests
	{
		[TestMethod()]
		public void ExecuteTest()
		{
			string actual = new Calculator("2+2*3-1").Execute();
			string expected = "7";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_1()
		{
			string actual = new Calculator("((1+1)*2+3)/(8-2*(3-1)+1)-1").Execute();
			string expected = "Invalid data in entered expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_2()
		{
			string actual = new Calculator("+").Execute();
			string expected = "Invalid expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_3()
		{
			string actual = new Calculator("+3").Execute();
			string expected = "Invalid operation!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_5()
		{
			string actual = new Calculator("++5").Execute();
			string expected = "Invalid operation!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_6()
		{
			string actual = new Calculator("3+").Execute();
			string expected = "Invalid expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_7()
		{
			string actual = new Calculator("(((1+1)/(2-3)+1)*2+1)*(((2+1)-2*(1+1)))").Execute();
			string expected = "Invalid data in entered expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_DivZero()
		{
			string actual = new Calculator("1/0").Execute();
			string expected = "Division by zero!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_InValidData_X()
		{
			string actual = new Calculator("1/(3-1*2-X)").Execute();
			string expected = "Invalid data in entered expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_InValidData_Bracket ()
		{
			string actual = new Calculator("1/3)").Execute();
			string expected = "Invalid data in entered expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_InValidData_Empty()
		{
			string actual = new Calculator("").Execute();
			string expected = "Entered empty expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_InValidData_Spacebar()
		{
			string actual = new Calculator(" ").Execute();
			string expected = "Entered empty expression!";
			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_FileCalc()
		{
			var calc = new Calculator("C:\\FoxMinded\\task5-calculator\\TestToCalc.txt").Execute();
			string actual = ""; 
			string expected = $@"(1+2)*3 = 9{Environment.NewLine}";
			try
			{
				using (StreamReader sr = new StreamReader("C:\\FoxMinded\\task5-calculator\\TestToCalcResult.txt"))
				{
					actual = sr.ReadToEnd();
				}
			}
			catch (Exception e)
			{
				throw e;
			}

			Assert.AreEqual(actual: actual, expected: expected);
		}


		[TestMethod()]
		public void ExecuteTest_FileCalc_onlyFileName()
		{
			string fileNameIn = "CL.txt";
			string actual = new Calculator(fileNameIn).Execute(); 
			string expected = $"{Environment.NewLine}File '{fileNameIn}' not found! Enter full path to file!{Environment.NewLine}";

			Assert.AreEqual(actual: actual, expected: expected);
		}
	}
}
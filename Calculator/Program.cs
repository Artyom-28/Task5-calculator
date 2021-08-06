using System;
using System.Collections.Generic;

namespace Calculator
{
	class Program
	{
		static void Main(string[] args)
		{
			bool quitForceFlag = false;
			Console.WriteLine($"Hello! This is a Calculator program!{Environment.NewLine}You can enter expression for calculation or full path to the text file with expressions to calculation");
			do
			{
				Console.Write($@"Enter: ");
				string expression = Console.ReadLine();
				var calc = new Calculator(expression); 
				{
					Console.WriteLine($"Entered:{Environment.NewLine}{expression}{Environment.NewLine}{Environment.NewLine}Result:{Environment.NewLine}{calc.Execute()}{Environment.NewLine}");
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine($"Continue calculation?");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"YES - press any kye!");
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"No - press 'n' or 'N' key{Environment.NewLine}");
					Console.ForegroundColor = ConsoleColor.Gray;
					quitForceFlag = new List<char>() { 'n', 'N' }.Contains(Console.ReadKey().KeyChar);
					Console.WriteLine();
				}
			}
			while (!quitForceFlag);
		}
	}
}

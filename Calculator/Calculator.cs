using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Calculator
{
	public class Calculator
	{
		private static readonly Dictionary<char, int> _operationsPriority = new Dictionary<char, int>() { {'+', 1}, { '-', 1 }, { '*', 2 }, { '/', 2 }, { '(', 0 }, { ')', 0 } };
		private string _errorMessage { get; set; }
		private string _expression { get; set; }
		private bool _isExecutedCloseBracketFlag { get; set; }
		private Stack<decimal> _operandsStack { get; set; }
		private Stack<char> _operationsStack { get; set; }
		private string _pathFileIn { get; set; }

		public Calculator(string enteredDataStr)
		{
			if(enteredDataStr.Contains(":\\") || enteredDataStr.Split(".txt").Count() == 2)
			{
				_pathFileIn = enteredDataStr;
			}
			else
			{
				SetExpression(expression: enteredDataStr);
			}

			_isExecutedCloseBracketFlag = true;
			_operandsStack = new Stack<decimal>();
			_operationsStack = new Stack<char>();
		}


		public string Execute()
		{
			string result = string.IsNullOrEmpty(_pathFileIn) ? ExecSingleExpression() : ExecExpresionsInFile();
			return result;
		}


		private string ExecExpresionsInFile()
		{
			string resultDataFileOut = "";
			string pathFileOut = "";
			FileInfo fileInfo = new FileInfo(_pathFileIn);

			if (!fileInfo.Exists)
			{
				return	$"{Environment.NewLine}File '{_pathFileIn}' not found! Enter full path to file!{Environment.NewLine}";
			}

			try
			{
				using (StreamReader sr = new StreamReader(_pathFileIn))
				{
					string fileDataStr = sr.ReadToEnd();
					List<string> expressionsList = fileDataStr.Split(Environment.NewLine).ToList();
					resultDataFileOut = string.Join("", expressionsList.Select(e => GetSingleResultFileMode(e))).Trim();

					string fileInName = _pathFileIn.Split('\\').LastOrDefault();
					string fileOutName = fileInName.Split('.').FirstOrDefault() + "Result.txt";
					pathFileOut = $"{string.Join('\\', _pathFileIn.Split('\\').Where(e => !e.Equals(fileInName)))}\\{fileOutName}";

					using (StreamWriter sw = new StreamWriter(path: pathFileOut, append: false, encoding: System.Text.Encoding.Default))  
					{
						sw.WriteLineAsync(resultDataFileOut);
					}
				}

			}
			catch (Exception e)
			{
				throw e;
			}
			return $"{resultDataFileOut}{Environment.NewLine}{Environment.NewLine}Written to the file  {pathFileOut}{Environment.NewLine}";
		}


		private string ExecSingleExpression()
		{
			if (string.IsNullOrEmpty(_expression))
			{
				TerminateCalculation(errorMessage: "Entered empty expression!");
			}

			var expresionChars = _expression.ToCharArray();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < expresionChars.Length; i++)
			{
				char currentChar = expresionChars[i];

				if (!IsValidData(currentChar))
				{
					TerminateCalculation(errorMessage: $"Invalid data in entered expression!");	
				}

				if (!string.IsNullOrEmpty(_errorMessage))
				{
					break;
				}
				
				if (Char.IsDigit(currentChar))
				{
					sb.Append(currentChar);
					if (i == expresionChars.Length - 1)
					{
						PushOperand(sb);

						while (_operationsStack.Count > 0)
						{
							ExecOperation(operation: _operationsStack.Pop());
						}
					}
				}
				else
				{
					PushOperand(sb);

					_isExecutedCloseBracketFlag = currentChar == ')' ? false : true ;

					if (_operationsStack.Count == 0 || GetPriority(_operationsStack.Peek()) <= GetPriority(currentChar) && GetPriority(currentChar) > 0 || currentChar == '(')
					{
						_operationsStack.Push(currentChar);
					}
					else if (_operationsStack.Count > 0)
					{
						i = IsCurrentOperationPriorityLessPrevious(currentOperation: currentChar) ? --i : i;

						while ( _operationsStack.Count > 0 && GetPriority(currentChar) <= GetPriority(_operationsStack.Peek()) )
						{
							ExecOperation(operation: _operationsStack.Pop());
							
							if (IsForceBreakFlag(index: i, expresionChars: expresionChars, currentChar: currentChar))
							{
								break;
							}
						}
					}
				}
			}

			string result = GetResult();
			return result;
		}


		private void ExecOperation(char operation)
		{
			if (operation.Equals('('))
			{
				_isExecutedCloseBracketFlag = true;
				return;
			}

			if(_operandsStack.Count() < 2)
			{
				TerminateCalculation(errorMessage: "Invalid operation!");
				return;
			}

			var operandRight = _operandsStack.Pop();
			var operandLeft = _operandsStack.Pop();
			var result = Decimal.MinValue;

			if (operation.Equals('+'))
			{
				result = operandLeft + operandRight;
			}
			else if (operation.Equals('-'))
			{
				result = operandLeft - operandRight;
			}
			else if (operation.Equals('*'))
			{
				result = operandLeft * operandRight;
			}
			else if (operation.Equals('/'))
			{
				if (operandRight == 0m)
				{
					TerminateCalculation(errorMessage: "Division by zero!");
					return;
				}

				result = operandLeft / operandRight;
			}

			_operandsStack.Push(result);
		}


		private bool IsCurrentOperationPriorityLessPrevious(char currentOperation)
		{
			bool isCurrentOperationPriorityLessPrevious = GetPriority(_operationsStack.Peek()) > GetPriority(currentOperation) 
														  && GetPriority(currentOperation) > 0 
														  && GetPriority(_operationsStack.Peek()) > 0;

			return isCurrentOperationPriorityLessPrevious;
		}


		private bool IsForceBreakFlag(int index, char[] expresionChars, char currentChar)
		{
			if (_operationsStack.Count > 0 && _operationsStack.Peek() == '(' && _isExecutedCloseBracketFlag)
			{
				return true;
			}

			bool forceBreak = (index + 1) < expresionChars.Length ? _operationsStack.Count > 0 && GetPriority(expresionChars[index + 1]) > GetPriority(_operationsStack.Peek()) && new char[] { '+', '-' }.Contains(_operationsStack.Peek()) : false;

			if (currentChar == ')' && forceBreak)
			{
				return true;
			}

			return false;
		}


		private bool IsValidData(char dataChar)
		{
			bool res = Char.IsDigit(dataChar) || _operationsPriority.Select(e => e.Key).Contains(dataChar);
			res &= _expression.ToCharArray().Where(e => e.Equals('(')).Count() == _expression.ToCharArray().Where(e => e.Equals(')')).Count();
			res = string.IsNullOrEmpty(_pathFileIn) ? res && !_expression.Contains("(") && !_expression.Contains(")") : res;
			return res;
		}


		private string GetResult()
		{
			string result = "";
			string defaultErrorMessage = "Invalid expression!";

			if(_operandsStack.Count > 0)
			{
				while (_operationsStack.Count > 0)
				{
					ExecOperation(operation: _operationsStack.Pop());
				}

				result = string.IsNullOrEmpty(_errorMessage) ? _operandsStack.Pop().ToString().Replace(',', '.') : defaultErrorMessage;
				
			}
			else
			{
				result = string.IsNullOrEmpty(_errorMessage) ? defaultErrorMessage : _errorMessage;
			}
			
			_errorMessage = String.Empty;
			_operandsStack.Clear();
			_operationsStack.Clear();

			return result;
		}


		private int GetPriority(char operation)
		{
			int priority = _operationsPriority.FirstOrDefault(e => e.Key == operation).Value;
			return priority;
		}


		private string GetSingleResultFileMode(string expression)
		{
			SetExpression(expression: expression);
			string result = $"{expression} = {ExecSingleExpression()}{Environment.NewLine}";
			return result;
		}


		private void PushOperand(StringBuilder sb)
		{
			decimal number = 0;
			if(Decimal.TryParse(s: sb.ToString(), result: out number))
			{
				_operandsStack.Push(number);
				sb.Clear();
			}
		}


		private void SetExpression(string expression)
		{
			_expression = expression.Replace(" ", "");
		}


		private void TerminateCalculation(string errorMessage)
		{
			_operandsStack.Clear();
			_operationsStack.Clear();
			_errorMessage = errorMessage;
		}
	}
}

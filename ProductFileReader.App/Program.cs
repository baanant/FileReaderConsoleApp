using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProductFileReader.Common.Commands;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Constants.General.ApplicationName;
            Init();
        }

        private static void Init()
        {
            while (true)
            {
                var inputCommandText = Read();
                if (string.IsNullOrWhiteSpace(inputCommandText)) continue;

                try
                {
                    //Parse command class data. 
                    var cmdClassData    = CommandParser.ParseClassData(Constants.Commands.CommandNamespace);
                    //Parse input command data.
                    var cmdInputData    = CommandParser.ParseInputData(inputCommandText, Constants.Commands.CommandClassName);
                    //Validate input command data against existing class command data.
                    CommandValidator.Validate(cmdClassData, cmdInputData);
                    var classData       = cmdClassData.FirstOrDefault(c => c.Name == cmdInputData.ClassName);
                    //Execute command method based on input.
                    var result          = CommandExecuter.ExecuteMethod(classData, cmdInputData, typeof(Commands).Assembly);
                    //Write command result.
                    Write(result);
                }
                catch (InputException inputException)
                {
                    Write(inputException.Message);
                }
                catch (FatalException fatalException)
                {
                    Write($"{fatalException.Message} \n {fatalException.InnerExceptionMessages}");
                }
            }
        }

        /// <summary>
        /// Read input command.
        /// </summary>
        /// <returns>Command input.</returns>
        private static string Read()
        {
            Console.Write(Constants.General.ConsolePrompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// Write text to console.
        /// </summary>
        /// <param name="text">Text to be written.</param>
        private static void Write(string text)
        {
            if(!string.IsNullOrEmpty(text)) Console.WriteLine(text);
        }

     
    }
}

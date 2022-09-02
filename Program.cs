using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Interview_Question
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Moffat Omuya Starkey 2022");
            string startupPath = Directory.GetCurrentDirectory() + "\\input.json";
            string inputData = File.ReadAllText(startupPath); //(@"C:\Users\Moff - it\Desktop\Interview\input.json"); // Convert JSON file to string
            Data[] inputDataObj = JsonConvert.DeserializeObject<Data[]>(inputData); // Convert string to object


            List<string> distinctUser = new List<string>(); //List for distinct users
            List<string> allUser = new List<string>(); // List for all users 

            var inputDataSize = inputDataObj.Length; // Get how many data sets inside JSON file

            for (int i = 0; i < inputDataSize; i++) // Find Unique Users
            {
                bool isDuplicate = false;
                for (int j = 0; j < i; j++)
                {
                    if (inputDataObj[i].user == inputDataObj[j].user)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    distinctUser.Add(inputDataObj[i].user); // Add unique users to a List
                }

                allUser.Add(inputDataObj[i].user); // store all usernames in a list
            }
            
            var numberOfDistinctUser = distinctUser.Count; // Get number of distinct users

            var mostCommonUser = (allUser.GroupBy(x => x) // Get the user that has appeared the most times 
            .Select(x => new { num = x, cnt = x.Count() })
            .OrderByDescending(g => g.cnt)
            .Select(g => g.num).First());

            var mostCommonUserCount = mostCommonUser.Count(); // get how many times the most popular user hit a command

            string[,] commandArray = new string[numberOfDistinctUser, mostCommonUserCount];   // Create a 2d arrray to store commands for each user
            string[,] timestampArray = new string[numberOfDistinctUser, mostCommonUserCount]; // Create a 2d arrray to store timestamp for each user

            for (int i = 0; i < numberOfDistinctUser; i++) 
            {
                var Index = 0;
                for (int j = 0; j < inputDataSize; j++)
                {
                    if(inputDataObj[j].user == distinctUser[i])
                    {
                        commandArray[i, Index] = inputDataObj[j].command; //store command in column corresponding to the user
                        timestampArray[i, Index] = inputDataObj[j].timestamp; //store timestamp in column corresponding to the user
                        Index++;
                    }
                }
            }

            List<string> alreadyFound = new List<string>(); //List of commands that have been checked for specific user
            int placeholder = 0; // Holds the element position for the element where a command was called for the first time for specific user

            for (int i = 0; i < numberOfDistinctUser; i++) // For every distinct user 
            {
                alreadyFound.Clear(); //Clear alreadyFound list for every new distinct user 
                JTokenWriter outputWriter = new JTokenWriter();
                outputWriter.WriteStartObject();
                outputWriter.WritePropertyName("<" + distinctUser[i] + ">");
                outputWriter.WriteStartObject();
                for (int j = 0; j < mostCommonUserCount; j++) // For every command for the distinct user
                {
                    string currentCommand; // current command we are checking
                    if ((commandArray[i, j] != null) && (!alreadyFound.Contains(commandArray[i,j]))) // Found user command 
                    {
                        placeholder = j; // save this element position for next time around
                        outputWriter.WritePropertyName("<" + commandArray[i, j] + ">");
                        outputWriter.WriteStartArray();
                        currentCommand = commandArray[i, j]; // save the current command

                        for (int k = j; k < mostCommonUserCount; k++) // Look for all timestamps for that user command
                        {
                            if (commandArray[i, k] == currentCommand) // if command equals command we are looking for the specific user
                            {
                                outputWriter.WriteRaw("<" + timestampArray[i, k] + ">");
                            }
                        }
                        outputWriter.WriteEndArray();
                        j = placeholder; // start loop back at place holder to make sure every command is checked for specific user
                        alreadyFound.Add(commandArray[i, j]); // add the command that has already been checked to exclusion list
                    }
                }

                outputWriter.WriteEndObject();
                outputWriter.WriteEndObject();
                JObject o = (JObject)outputWriter.Token;
                // Manipulate string to get formating from document
                string newString = o.ToString().Replace("\"", "");
                newString = newString.Replace("[\r\n      ", "[");
                newString = newString.Replace(">\r\n    ", ">");
                newString = newString.Replace(">,\r\n", ">,");
                newString = newString.Replace("     <", "<");

                File.AppendAllText("output.json", "\r\n" + newString);
            }

            var lines = File.ReadAllLines("output.json");
            File.WriteAllLines("output.json", lines.Skip(1).ToArray()); // Get rid of that first line of the output file (\r\n) from fist call of File.AppendAllText
        }
    }

    public class Data
    {
        public string user { get; set; }
        public string command { get; set; }
        public string timestamp { get; set; }
    }

}

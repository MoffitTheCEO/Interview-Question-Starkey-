using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interview_Question
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Moffat Omuya Starkey 2022");
            string inputData = File.ReadAllText(@"C:\Users\Moff - it\Desktop\Interview\input.json"); // Convert JSON file to string
            Data[] inputDataObj = JsonConvert.DeserializeObject<Data[]>(inputData); // Convert string to object

            Console.WriteLine("Unique User Names: ");

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
                    Console.WriteLine(inputDataObj[i].user);
                    distinctUser.Add(inputDataObj[i].user); // Add unique users a List
                }

                allUser.Add(inputDataObj[i].user); // store all usernames in a list
            }
            
            var numberOfDistinctUser = distinctUser.Count;

            var mostCommonUser = (allUser.GroupBy(x => x) // Get the user that has appeared the most times 
            .Select(x => new { num = x, cnt = x.Count() })
            .OrderByDescending(g => g.cnt)
            .Select(g => g.num).First());

            Console.WriteLine("The most frequent number in this array is {0} has been repeated {1} times.", mostCommonUser.Key, mostCommonUser.Count());

            var mostCommonUserCount = mostCommonUser.Count();

            string[,] commandArray = new string[numberOfDistinctUser, mostCommonUserCount];   // Create a 2d arrray to store commands for each user
            string[,] timestampArray = new string[numberOfDistinctUser, mostCommonUserCount]; // Create a 2d arrray to store timestamp for each user

            List<string> alreadyFound = new List<string>(); //List for distinct users

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
            int placeholder = 0;
            for (int i = 0; i < numberOfDistinctUser; i++) // For every distinct user 
            {
                alreadyFound.Clear();
                JTokenWriter outputWriter = new JTokenWriter();
                outputWriter.WriteStartObject();
                outputWriter.WritePropertyName("<" + inputDataObj[i].user + ">");
                outputWriter.WriteStartObject();
                for (int j = 0; j < mostCommonUserCount; j++) // For every command or timestamp for the distinct user
                {
                    string currentCommand;
                    if ((commandArray[i, j] != null) && (!alreadyFound.Contains(commandArray[i,j]))) // Found user command 
                    {
                        placeholder = j;
                        outputWriter.WritePropertyName("<" + commandArray[i, j] + ">");
                        outputWriter.WriteStartArray();
                        currentCommand = commandArray[i, j];

                        for (int k = j; k < mostCommonUserCount; k++) // Look for all timestamps for that user command
                        {
                            if (commandArray[i, k] == currentCommand)
                            {
                                outputWriter.WriteRaw("<" + timestampArray[i, k] + ">");
                                //j = k;
                            }
                        }
                        outputWriter.WriteEndArray();
                        j = placeholder;
                        alreadyFound.Add(commandArray[i, j]);
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

                File.AppendAllText(@"C:\Users\Moff - it\Desktop\Interview\output.json", "\r\n" + newString);
            }

            var lines = File.ReadAllLines(@"C:\Users\Moff - it\Desktop\Interview\output.json");
            File.WriteAllLines(@"C:\Users\Moff - it\Desktop\Interview\output.json", lines.Skip(1).ToArray()); // Get rid of that first line of the output file
        }
    }

    public class Data
    {
        public string user { get; set; }
        public string command { get; set; }
        public string timestamp { get; set; }
    }

}

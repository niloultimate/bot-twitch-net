using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace bot_twitch_net
{
    class Program
    {
        static void Main(string[] args)
        {
            Start().Wait();
        }

        private static async Task Start()
        {
            string ip = "irc.chat.twitch.tv";
            int port = 6667;
            string password = "oauth:0800000000000000000000";
            string botUsername = "bot-net";
            Dictionary<string, string> channel = new Dictionary<string, string>(){
                //{ "Apelido", "Canal" }
                { "Alan", "alanzoka" }
            };

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            var streamReader = new StreamReader(tcpClient.GetStream());
            var streamWriter = new StreamWriter(tcpClient.GetStream()) { NewLine = "\r\n", AutoFlush = true };

            await streamWriter.WriteLineAsync($"PASS {password}");
            await streamWriter.WriteLineAsync($"NICK {botUsername}");

            foreach (var item in channel)
            {
                Console.WriteLine($"Conectando no canal {item.Key}.");
                await streamWriter.WriteLineAsync($"JOIN #{item.Value}");
            }

            while (true)
            {
                string line = await streamReader.ReadLineAsync();


                string[] auxSplit = line.Split(" :");

                switch (auxSplit[0])
                {
                    case "PING":
                        await streamWriter.WriteLineAsync($"PONG :{auxSplit[1]}");
                        break;
                    default:
                        Console.WriteLine(line);
                        break;
                }
            }

            Console.WriteLine("End");
        }
    }
}

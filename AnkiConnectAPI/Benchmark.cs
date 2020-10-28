using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnkiConnectAPI
{
    public class Benchmark
    {
        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var final = new String(stringChars);
            return final;
        }
        private static void SendRequest(string json)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:8765");
            request.ContentType = "text/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }


        }
        private static void CreateDeck(string deckName)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteString("action", "createDeck");
                    writer.WriteNumber("version", 6);

                    writer.WriteStartObject("params");
                    writer.WriteString("deck", deckName);
                    writer.WriteEndObject();

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    SendRequest(json);
                    Console.WriteLine("a");
                }
            }
        }
        private static void DeleteDeck(string deckName)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteString("action", "deleteDecks");
                    writer.WriteNumber("version", 6);

                    writer.WriteStartObject("params");
                    writer.WriteStartArray("decks");
                    writer.WriteStringValue(deckName);
                    writer.WriteEndArray();
                    writer.WriteString("cardsToo", "true");
                    writer.WriteEndObject();

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    SendRequest(json);
                }
            }
        }
        private static void AddNote(string deckName, string modelName, List<string> fields, List<string> content, List<string> tags)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteString("action", "addNote");
                    writer.WriteNumber("version", 6);

                    writer.WriteStartObject("params");

                    writer.WriteStartObject("note");
                    writer.WriteString("deckName", deckName);
                    writer.WriteString("modelName", modelName);

                    writer.WriteStartObject("fields");
                    for (int i = 0; i < fields.Count; i++)
                    {
                        writer.WriteString(fields[i], content[i]);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartArray("tags");
                    if (!(tags == null || tags.Any()))
                    {
                        foreach (string i in tags)
                        {
                            writer.WriteStringValue(i);
                        }
                    }
                    writer.WriteEndArray();

                    writer.WriteEndObject();

                    writer.WriteEndObject(); 

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    SendRequest(json);
                }
            }
        }
        public static void SendBenchmark(int rep)
        {
            string deckName = "benchmark." + RandomString(16);
            var fields = new List<string>() { "Front", "Back" };
            var content = new List<string>() { "", "" };
            var tags = new List<string>() { "benchmark" };
            CreateDeck(deckName);
            Console.WriteLine(deckName);
            for (int i = 0; i < rep; i++)
            {
                content[0] = RandomString(32);
                content[1] = RandomString(32);
                AddNote(deckName, "Basic", fields, content, tags);
            }

            //DeleteDeck(deckName);
        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers; 

namespace AnkiConnectAPI
{
    public static class SendToAnki
    {
        internal static string SendRequest(string json)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:8765");
            request.ContentType = "text/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            { 
                streamWriter.Write(json); 
            }
            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                return responseText;
            }
        }
        public static string Browse()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteString("action", "guiBrowse");
                    writer.WriteNumber("version", 6);

                    writer.WriteStartObject("params");
                    writer.WriteString("query", "deck:current");
                    writer.WriteEndObject();

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    var response = SendRequest(json);
                    return response;
                }
            }
        }
        public static string CreateDeck(string deckName)
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
                    var response = SendRequest(json);
                    return response;
                }
            }
        }
        public static string DeleteDeck(string deckName, bool deleteCards)
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
                    writer.WriteString("cardsToo", Convert.ToString(deleteCards));
                    writer.WriteEndObject();

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    var response = SendRequest(json);
                    return response;
                }
            }   
        }
        public static string DeleteDeck(List<string> deckName, bool deleteCards)
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
                    foreach (string i in deckName)
                    {
                        writer.WriteStringValue(i);
                    }
                    writer.WriteEndArray();
                    writer.WriteString("cardsToo", Convert.ToString(deleteCards));
                    writer.WriteEndObject();

                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    var response = SendRequest(json);
                    return response;
                }
            }
        }
        public static string AddNote(string deckName, string modelName, List<string> fields, List<string> content, List<string> tags)
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
                    if (tags != null)
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
                    var response = SendRequest(json);
                    return response;
                }
            }
        }
        public static string DeckNames()
        { 
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteString("action", "deckNames");
                    writer.WriteNumber("version", 6);
                    writer.WriteEndObject();

                    writer.Flush();
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    var response = SendRequest(json);
                    return response;
                }
            }
        }
    }
    
    public class Benchmark
    {
        public static string SendBenchmark(int rep)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
                {
                    //var random = new RandomString();
                    var random = RandomizerFactory.GetRandomizer(new FieldOptionsText
                        {UseSpecial = false,Min = 32, Max = 32, ValueAsString = true});
                    var deckName = "benchmark." + random.Generate();
                    SendToAnki.CreateDeck(deckName);
                    var fields = new List<string>() { "Front", "Back" };
                    var tags = new List<string>() { "benchmark" };
                    
                    writer.WriteStartObject();
                    writer.WriteString("action", "addNotes");
                    writer.WriteNumber("version", 6);

                    writer.WriteStartObject("params");
                    
                    writer.WriteStartArray("notes");
                    for (var r = 0; r < rep; r++)
                    {

                        writer.WriteStartObject();
                        writer.WriteString("deckName", deckName);
                        writer.WriteString("modelName", "Basic");

                        writer.WriteStartObject("fields");
                        for (var i = 0; i < fields.Count; i++)
                        {
                            writer.WriteString(fields[i], random.Generate()); //Pretty sure it generates the same thing
                        }

                        writer.WriteEndObject(); //Fields
                        
                        writer.WriteStartObject("options");
                        writer.WriteBoolean("allowDuplicate" , true);
                        writer.WriteString("duplicateScope", "deck");
                        writer.WriteEndObject(); //Options

                        writer.WriteStartArray("tags");
                        foreach (var i in tags)
                        { 
                            writer.WriteStringValue(i);
                        }

                        writer.WriteEndArray(); //Tags

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray(); //Notes
                    writer.WriteEndObject(); //Params
                    writer.WriteEndObject(); //Main

                    writer.Flush();
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    var response = SendToAnki.SendRequest(json);
                    SendToAnki.DeleteDeck(deckName, true);
                    return response;
                }
            }
        }
    }
}

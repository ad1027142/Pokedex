using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Pokedex
{
    class API
    {
        public static async Task PokeAPI(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            // Normalize input for the API URL
            string normalized = input.Trim().ToLowerInvariant();
            string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{normalized}";
            try
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                // Parse JSON and populate variables
                JObject j = JObject.Parse(content);
                string rawName = j["name"]?.ToString() + "";
                string Name = FormatName(rawName);
                int Id = j["id"]?.ToObject<int>() ?? 0;
                int Height = j["height"]?.ToObject<int>() ?? 0;
                int Weight = j["weight"]?.ToObject<int>() ?? 0;
                Pokemon pokemon = new Pokemon
                {
                    Name = Name,
                    Id = Id,
                    Height = Height,
                    Weight = Weight
                };
                var typesArray = j["types"] as JArray;
                string Types = string.Empty;
                if (typesArray != null)
                {
                    var typeNames = new List<string>();
                    foreach (JToken t in typesArray)
                    {
                        var typeNameToken = t["type"]?["name"];
                        if (typeNameToken != null)
                        {
                            string typeNameRaw = typeNameToken.ToString();
                            if (!string.IsNullOrEmpty(typeNameRaw))
                            {
                                typeNames.Add(FormatName(typeNameRaw));
                            }
                        }
                    }

                    if (typeNames.Count > 0)
                    {
                        Types = string.Join(", ", typeNames);
                    }
                }

                // TODO: return or surface 'pokemon' / 'Types' to the UI as needed.
            }
            catch (Exception)
            {
                // Consider logging or rethrowing for UI feedback.
            }
        }

        private static string FormatName(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return "";
            }

            if (raw.Length == 1)
            {
                return char.ToUpper(raw[0]) + raw.Substring(1);
            }

            return char.ToUpper(raw[0]) + raw.Substring(1).ToLower();
        }
    }
}

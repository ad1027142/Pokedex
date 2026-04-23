using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Plugin.Maui.Audio;

namespace Pokedex
{
    class API
    {
        public static async Task<Pokemon?> PokeAPI(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string normalized = NormalizeInputForApi(input);
            if (string.IsNullOrEmpty(normalized))
                return null;

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

                // API-provided sprite (may be null/empty)
                string apiSprite = j["sprites"]?["front_default"]?.ToString() ?? string.Empty;

                // Prefer constructing the sprite URL from the Pokemon ID (guaranteed stable),
                // but verify it exists. Fallback to apiSprite or local placeholder if not.
                string imageUrl = string.Empty;
                if (Id > 0)
                {
                    string constructed = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";
                    try
                    {
                        // Use HEAD to check existence without downloading the image
                        var headReq = new HttpRequestMessage(HttpMethod.Head, constructed);
                        using var headResp = await client.SendAsync(headReq);
                        if (headResp.IsSuccessStatusCode)
                        {
                            imageUrl = constructed;
                        }
                        else if (!string.IsNullOrEmpty(apiSprite))
                        {
                            imageUrl = apiSprite;
                        }
                        else
                        {
                            // local fallback — add a placeholder image to Resources/Images named placeholder.png
                            imageUrl = "placeholder.png";
                        }
                    }
                    catch
                    {
                        imageUrl = !string.IsNullOrEmpty(apiSprite) ? apiSprite : "placeholder.png";
                    }
                }
                else
                {
                    imageUrl = !string.IsNullOrEmpty(apiSprite) ? apiSprite : "placeholder.png";
                }

                Pokemon pokemon = new Pokemon
                {
                    Name = Name,
                    Id = Id,
                    Height = Height,
                    Weight = Weight,
                    Types = Types,
                    ImageUrl = imageUrl
                };
                return pokemon;
            }
            catch (HttpRequestException)
            {
                // Network / non-success HTTP responses
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Normalizes the text input so the API gets either a clean numeric id or a valid name string.
        private static string NormalizeInputForApi(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                return string.Empty;

            // Trim and remove surrounding whitespace
            string candidate = rawInput.Trim();

            // Allow inputs like "#25" -> "25"
            if (candidate.StartsWith("#"))
                candidate = candidate.Substring(1).Trim();

            // If input is purely numeric (possibly with leading zeros), keep as number
            // Remove any grouping characters (commas, spaces) before testing
            string digitsOnly = new string(candidate.Where(char.IsDigit).ToArray());
            bool allDigits = digitsOnly.Length > 0 && digitsOnly.Length == candidate.Count(c => char.IsDigit(c));

            if (allDigits)
            {
                // Remove leading zeros (API accepts plain integer ids)
                string noLeadingZeros = digitsOnly.TrimStart('0');
                return string.IsNullOrEmpty(noLeadingZeros) ? "0" : noLeadingZeros;
            }

            // Otherwise treat as a name: keep letters, digits and hyphens (some Pokemon use hyphens),
            // remove other punctuation and whitespace, then lowercase.
            var allowed = candidate
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .ToArray();

            string cleaned = new string(allowed).ToLowerInvariant();
            return cleaned;
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

        public static async Task<Pokemon?> GetPokemonById(int id)
        {
            if (id <= 0)
                return null;

            return await PokeAPI(id.ToString());
        }

        public static async Task<Pokemon?> GetNextPokemon(int currentId)
        {
            if (currentId == 1025)
            {
                return await GetPokemonById(10001);
            }else if(currentId == 10325)
            {
                return await GetPokemonById(1);
            }
            else
            {
                return await GetPokemonById(currentId + 1);
            }
        }

        public static async Task<Pokemon?> GetPreviousPokemon(int currentId)
        {
            if (currentId <= 0)
            {
                return null;
            }
            else if (currentId == 10001)
            {
                return await GetPokemonById(1025);
            } else if (currentId == 1)
            {
                return await GetPokemonById(10325);
            }
            else
            {
                return await GetPokemonById(currentId - 1);
            }
        }
    }
}
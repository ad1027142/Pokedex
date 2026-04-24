using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace Pokedex
{
    public class MainViewModel : INotifyPropertyChanged
    {
        string name = "Name";
        string id = "ID";
        string height = "Height";
        string weight = "Weight";
        string types = "Type";
        string imageUrl = string.Empty;
        int currentId = 0;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        public string Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        public string Types
        {
            get => types;
            set => SetProperty(ref types, value);
        }

        public string ImageUrl
        {
            get => imageUrl;
            set => SetProperty(ref imageUrl, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;
            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task PlayRandomAudioAsync(int pokemonId)
        {
            try
            {
                Random random = new Random();
                int chance = random.Next(1, 5); // 1-4

                // If Pikachu (id 25) and within 1% chance, play Pikachu sound
                if (!(pokemonId == 25) && chance == 1)               {
                    await PlayAudioAsync("pikachu.mp3");
                }
                else
                {
                    // Otherwise, play "Who's that Pokemon"
                    await PlayAudioAsync("whosthatpokemon.wav");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio playback error: {ex.Message}");
            }
        }

        private async Task PlayAudioAsync(string fileName)
        {
            try
            {
                var audioManager = AudioManager.Current;
                var audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync($"{fileName}"));
                audioPlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to play {fileName}: {ex.Message}");
            }
        }

        public async Task LoadPokemonAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            var p = await API.PokeAPI(input);
            if (p is null)
                return;

            currentId = p.Id;
            Name = p.Name;
            Id = p.Id.ToString();
            Height = p.Height.ToString();
            Weight = p.Weight.ToString();
            Types = p.Types;
            ImageUrl = p.ImageUrl;

            await PlayRandomAudioAsync(p.Id);
        }

        public async Task NextPokemonAsync()
        {
            var p = await API.GetNextPokemon(currentId);
            if (p is null)
                return;

            currentId = p.Id;
            Name = p.Name;
            Id = p.Id.ToString();
            Height = p.Height.ToString();
            Weight = p.Weight.ToString();
            Types = p.Types;
            ImageUrl = p.ImageUrl;

            await PlayRandomAudioAsync(p.Id);
        }

        public async Task PreviousPokemonAsync()
        {
            var p = await API.GetPreviousPokemon(currentId);
            if (p is null)
                return;

            currentId = p.Id;
            Name = p.Name;
            Id = p.Id.ToString();
            Height = p.Height.ToString();
            Weight = p.Weight.ToString();
            Types = p.Types;
            ImageUrl = p.ImageUrl;

            await PlayRandomAudioAsync(p.Id);
        }
    }
}
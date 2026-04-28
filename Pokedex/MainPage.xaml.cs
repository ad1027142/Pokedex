using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Pokedex;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel = new MainViewModel();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    public void Volume(object sender, EventArgs e)
    {
        viewModel.VolumeToggle();
    }
    async void OnSubmitClicked(object sender, EventArgs e)
    {
        var text = InputEntry?.Text ?? string.Empty;
        await viewModel.LoadPokemonAsync(text);
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        var text = InputEntry?.Text ?? string.Empty;
        // Optional: keep the existing behavior or remove if not needed
        DisplayAlert("Input submitted", text, "OK");
    }

    async void Right(object sender, EventArgs e)
    {
        await viewModel.NextPokemonAsync();
    }

    async void Left(object sender, EventArgs e)
    {
        await viewModel.PreviousPokemonAsync();
    }
}
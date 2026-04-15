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

    async Task Right(object sender, EventArgs e)
    {
        // Try to get the current numeric ID from the view model (or the input entry as fallback).
        var current = viewModel.Id ?? InputEntry?.Text ?? string.Empty;
        if (!int.TryParse(current, out int id))
        {
            // If parsing fails, start from 1
            id = 1;
        }

        id++; // move to next pokemon
        var nextId = id.ToString();
        // Update the input field (optional) so user sees the new id
        if (InputEntry is not null) InputEntry.Text = nextId;

        await viewModel.LoadPokemonAsync(nextId);
    }

    async Task Left(object sender, EventArgs e)
    {
        // Try to get the current numeric ID from the view model (or the input entry as fallback).
        var current = viewModel.Id ?? InputEntry?.Text ?? string.Empty;
        if (!int.TryParse(current, out int id))
        {
            // If parsing fails, start from 1
            id = 1;
        }

        // don't go below 1
        if (id > 1) id--;

        var prevId = id.ToString();
        // Update the input field (optional) so user sees the new id
        if (InputEntry is not null) InputEntry.Text = prevId;

        await viewModel.LoadPokemonAsync(prevId);
    }
}
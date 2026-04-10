using System;
using Microsoft.Maui.Controls;

namespace Pokedex;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Await the Task returned by PokeAPI, and pass the entry text (or empty string) as required.
        await API.PokeAPI(InputEntry?.Text ?? string.Empty);
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        var text = InputEntry?.Text ?? string.Empty;
        // Handle submitted text (search, filter, navigate, etc.)
        DisplayAlert("Input submitted", text, "OK");
    }
}

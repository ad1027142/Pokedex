using System;
using Microsoft.Maui.Controls;

namespace Pokedex;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    void OnLocationClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            // Replace with your navigation / logic for that map location
            DisplayAlert("Location tapped", $"You tapped: {btn.Text}", "OK");
        }
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        var text = InputEntry?.Text ?? string.Empty;
        // Handle submitted text (search, filter, navigate, etc.)
        DisplayAlert("Input submitted", text, "OK");
    }
}

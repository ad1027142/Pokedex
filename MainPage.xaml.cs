using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;

namespace Pokedex
{
    public partial class MainPage : ContentPage
    {
        // Thresholds (device-independent pixels)
        const double CompactHeightThresholdDp = 700.0;

        public MainPage()
        {
            InitializeComponent();

            // Initial apply based on current display info
            ApplyLayoutForScreen(DeviceDisplay.Current.MainDisplayInfo);

            // React to changes (orientation, windowing)
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        }

        void OnMainDisplayInfoChanged(object? sender, MainDisplayInfoChangedEventArgs e)
        {
            // Run on UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyLayoutForScreen(e.DisplayInfo);
            });
        }

        void ApplyLayoutForScreen(MainDisplayInfo info)
        {
            // Convert pixels to device-independent units (dp)
            double heightDp = info.Height / info.Density;

            bool compact = heightDp < CompactHeightThresholdDp;
            if (compact)
                ApplyCompactLayout();
            else
                ApplyNormalLayout();
        }

        void ApplyCompactLayout()
        {
            // Image
            PokemonImage.HeightRequest = 180;
            PokemonImage.WidthRequest = 180;

            // Frame / border
            InfoFrame.Padding = new Thickness(10);
            InfoFrame.CornerRadius = 8;
            InfoFrame.Margin = new Thickness(0, 8, 0, 0);

            // Labels
            NameLabel.FontSize = 18;
            TypesLabel.FontSize = 14;

            // If you have more labels / controls, reduce their sizes/paddings here
        }

        void ApplyNormalLayout()
        {
            // Image
            PokemonImage.HeightRequest = 250;
            PokemonImage.WidthRequest = 220;

            // Frame / border
            InfoFrame.Padding = new Thickness(20);
            InfoFrame.CornerRadius = 12;
            InfoFrame.Margin = new Thickness(0, 12, 0, 0);

            // Labels
            NameLabel.FontSize = 22;
            TypesLabel.FontSize = 16;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace Curby
{
    public partial class MainPage : ContentPage
    {
        private Plugin.Geolocator.Abstractions.Position currentUserPosition;
        private IGeolocator locator = CrossGeolocator.Current;
        private Pin currentUserLocationPin;

        public MainPage()
        {
            InitializeComponent();
            VariableSetup();
        }

        async void VariableSetup()
        {
            currentUserPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(100));
            await StartLocationListening();
            var map = new Map((MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentUserPosition.Latitude, currentUserPosition.Longitude), Distance.FromMiles(0.5))));
            stack.Children.Add(map);
            currentUserLocationPin = new Pin
            {
                Type = PinType.Place,
                Position = new Xamarin.Forms.Maps.Position(currentUserPosition.Latitude, currentUserPosition.Longitude),
                Label = "Custom Pin",
                Address = "You dingus"
            };
            map.Pins.Add(currentUserLocationPin);
        }

        private void PositionChanged(object sender, PositionEventArgs e)
        {
            currentUserPosition = e.Position;
            currentUserLocationPin.Position = new Xamarin.Forms.Maps.Position(currentUserPosition.Latitude, currentUserPosition.Longitude);
        }

        async Task StartLocationListening()
        {
            await locator.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true, new ListenerSettings
            {
                ActivityType = ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = true,
                DeferralDistanceMeters = 1,
                DeferralTime = TimeSpan.FromSeconds(1),
                ListenForSignificantChanges = true,
                PauseLocationUpdatesAutomatically = false
            });
            locator.PositionChanged += PositionChanged;
        }
    }
}

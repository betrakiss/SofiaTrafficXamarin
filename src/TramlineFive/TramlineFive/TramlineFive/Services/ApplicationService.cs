﻿using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TramlineFive.Common.Models;
using TramlineFive.Common.Services;
using Xamarin.Forms;

namespace TramlineFive.Services
{
    public class ApplicationService : IApplicationService
    {
        public async Task<Position> GetCurrentPositionAsync()
        {
            Plugin.Geolocator.Abstractions.Position position = await CrossGeolocator.Current.GetPositionAsync(timeout: TimeSpan.FromSeconds(5));
            return new Position
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            };
        }

        public string GetVersion()
        {
            return Version.Plugin.CrossVersion.Current.Version.Substring(0, 5);
        }

        public void OpenUri(string uri)
        {
            Device.OpenUri(new Uri(uri));
        }

        public bool HasLocationPermissions()
        {
            return DependencyService.Get<IPermissionService>().HasLocationPermissions();
        }

        public void RunOnUIThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}

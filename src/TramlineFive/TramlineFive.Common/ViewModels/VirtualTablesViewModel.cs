﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SkgtService;
using SkgtService.Exceptions;
using SkgtService.Models;
using SkgtService.Parsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TramlineFive.Common.Messages;
using TramlineFive.Common.Services;
using TramlineFive.DataAccess.Domain;

namespace TramlineFive.Common.ViewModels
{
    public class VirtualTablesViewModel : BaseViewModel
    {
        public ICommand FavouriteCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }

        public VirtualTablesViewModel()
        {
            FavouriteCommand = new RelayCommand(async () => await AddFavouriteAsync());
            SearchCommand = new RelayCommand(() => MessengerInstance.Send(new StopSelectedMessage(StopCode, true)));

            MessengerInstance.Register<StopSelectedMessage>(this, async (sc) => await OnStopSelected(sc.Selected));
        }

        private async Task OnStopSelected(string stopCode)
        {
            await CheckStopAsync(stopCode);
        }

        private async Task AddFavouriteAsync()
        {
            FavouriteDomain added = await FavouriteDomain.AddAsync(stopInfo.Name, stopInfo.Code);
            if (added != null)
            {
                MessengerInstance.Send(new FavouriteAddedMessage(added));
                InteractionService.DisplayToast($"Спирка {added.Name} е добавена към любими");
            }
            else
            {
                InteractionService.DisplayToast($"Спирката вече съществува в любими");
            }
        }

        public async Task CheckStopAsync(string selected)
        {
            StopCode = selected;
            await SearchByStopCodeAsync();
        }

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                Version = await VersionService.CheckForUpdates();
            }
            catch (Exception ex)
            {
                await InteractionService.DisplayAlertAsync("exception", ex.InnerException.Message, "ok");
            }
        }

        public async Task SearchByStopCodeAsync()
        {
            IsLoading = true;

            try
            {
                StopInfo info = await new ArrivalsService().GetByStopCodeAsync(stopCode);

                IsLoading = false;

                StopInfo = info;
                Direction = stopInfo.Lines.FirstOrDefault(l => !String.IsNullOrEmpty(l.Direction))?.Direction;
               
                MessengerInstance.Send(new ShowMapMessage(true, StopInfo.Lines.Count));
                await HistoryDomain.AddAsync(stopCode, stopInfo.Name);
            }
            catch (StopNotFoundException)
            {
                await InteractionService.DisplayAlertAsync("Няма данни", $"Няма данни за спирка {stopCode}.", "OK");
            }
            catch (Exception e)
            {
                await InteractionService.DisplayAlertAsync("Грешка", e.Message, "OK");
            }
        }

        private Line selected;
        public Line Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = null;
                RaisePropertyChanged();
            }
        }

        private string direction;
        public string Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                RaisePropertyChanged();
            }
        }

        private StopInfo stopInfo;
        public StopInfo StopInfo
        {
            get
            {
                return stopInfo;
            }
            set
            {
                stopInfo = value;
                RaisePropertyChanged();
            }
        }

        private NewVersion version;
        public NewVersion Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                RaisePropertyChanged();
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                RaisePropertyChanged();
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                RaisePropertyChanged();
            }
        }

        private string stopCode;
        public string StopCode
        {
            get
            {
                return stopCode;
            }
            set
            {
                stopCode = value;
                RaisePropertyChanged();
            }
        }
    }
}

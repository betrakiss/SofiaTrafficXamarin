﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TramlineFive.Common.Messages;
using TramlineFive.DataAccess.Domain;

namespace TramlineFive.Common.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<HistoryDomain> History { get; private set; }

        public HistoryViewModel()
        {
            MessengerInstance.Register<HistoryClearedMessage>(this, (h) => History.Clear());
        }

        private HistoryDomain selected;
        public HistoryDomain Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                RaisePropertyChanged();

                if (value != null)
                {
                    InteractionService.ChangeTab(0);
                    MessengerInstance.Send(new HistorySelectedMessage(selected));

                    selected = null;
                    RaisePropertyChanged();
                }
            }
        }

        public async Task LoadHistoryAsync()
        {
            History = new ObservableCollection<HistoryDomain>((await HistoryDomain.TakeAsync()).Reverse());
            RaisePropertyChanged("History");

            HistoryDomain.HistoryAdded += OnHistoryAdded;
        }

        private void OnHistoryAdded(object sender, EventArgs e)
        {
            History.Insert(0, sender as HistoryDomain);
        }
    }
}

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
            MessengerInstance.Register<HistoryClearedMessage>(this, (h) => OnHistoryCleared());
        }

        public bool HasHistory => (History == null || History.Count == 0);


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
                    MessengerInstance.Send(new StopSelectedMessage(selected.StopCode));

                    selected = null;
                    RaisePropertyChanged();
                }
            }
        }

        public async Task LoadHistoryAsync()
        {
            History = new ObservableCollection<HistoryDomain>((await HistoryDomain.TakeAsync()).Reverse());
            RaisePropertyChanged("History");
            RaisePropertyChanged("HasHistory");

            HistoryDomain.HistoryAdded += OnHistoryAdded;
        }

        private void OnHistoryAdded(object sender, EventArgs e)
        {
            History.Insert(0, sender as HistoryDomain);
            RaisePropertyChanged("HasHistory");
        }

        private void OnHistoryCleared()
        {
            History.Clear();
            RaisePropertyChanged("HasHistory");
        }
    }
}

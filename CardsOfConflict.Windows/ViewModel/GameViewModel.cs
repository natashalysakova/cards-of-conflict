using CardsOfConflict.Windows.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public GameViewModel()
        {
            openPage = new StartupPage();
        }
        private Page openPage;

        public Page OpenPage
        {
            get => openPage;
            set
            {
                openPage = value;
                OnPropertyChanged(nameof(OpenPage));
            }
        }


        #region INotifyPropertyChanged implemenation
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

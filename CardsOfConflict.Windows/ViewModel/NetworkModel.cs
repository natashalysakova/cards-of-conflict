using CardsOfConflict.Library.Helpers;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace CardsOfConflict.Windows.ViewModel
{

    public class NetworkModel
    {
        int port;
        string connectIp;
        public IPAddress ExternalIp
        {
            get => NetworkHelper.GetPublicIpAddress();
        }
        public IPAddress LocalIp
        {
            get => NetworkHelper.GetLocalIPAddress();
        }
        public int Port
        {
            get
            {
                if (port == 0)
                {
                    Port = Settings.Default.port;
                }
                return port;
            }
            set
            {
                port = value;
                Settings.Default.port = port;
                Settings.Default.Save();
                OnPropertyChanged(nameof(Port));
            }
        }
        public string ConnectIp
        {
            get
            {
                return Settings.Default.lastConnected;
            }
            set
            {
                Settings.Default.lastConnected = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(Port));
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

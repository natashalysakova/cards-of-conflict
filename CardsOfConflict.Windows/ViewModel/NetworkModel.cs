using CardsOfConflict.Library.Helpers;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace CardsOfConflict.Windows.ViewModel
{

    public class NetworkModel
    {
        int port;
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
                    Port = int.Parse(ConfigurationManager.AppSettings["defaultPort"]);
                }
                return port;
            }
            set
            {
                port = value;
                ConfigurationManager.AppSettings["defaultPort"] = port.ToString();
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

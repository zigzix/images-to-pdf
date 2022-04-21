using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToPdf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string OutputFolder 
        { 
            get { return _outputFolder; } 
            set 
            {
                if (_outputFolder != value)
                {
                    _outputFolder = value;
                    OnPropertyChange("OutputFolder");
                }
            } 
        } 

        public ObservableCollection<SourceFolder> SourceFolders { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            SourceFolders = new ObservableCollection<SourceFolder>();
        }

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

using FileSearcher.Commands;
using FileSearcher.Stores;
using FileSearcher.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileSearcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string illegalWordsPath = "illegal_words.txt";
        private readonly NavigationStore _navigationStore;
        private ObservableCollection<string> _illegalWords;

        public App()
        {
            _navigationStore = new NavigationStore();
            _illegalWords = new ObservableCollection<string>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new FileSearchSettingsViewModel(_navigationStore, illegalWordsPath);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore, illegalWordsPath)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}

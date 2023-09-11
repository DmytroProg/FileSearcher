﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSearcher.Views
{
    /// <summary>
    /// Interaction logic for FileSearchProcessingView.xaml
    /// </summary>
    public partial class FileSearchProcessingView : UserControl
    {
        public FileSearchProcessingView()
        {
            InitializeComponent();
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseBtn.Visibility = Visibility.Hidden;
            resumeBtn.Visibility = Visibility.Visible;
        }

        private void resumeBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseBtn.Visibility = Visibility.Visible;
            resumeBtn.Visibility = Visibility.Hidden;
        }
    }
}

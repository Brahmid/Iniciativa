﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Iniciativa
{
    /// <summary>
    /// Interakční logika pro Forplayers.xaml
    /// </summary>
    public partial class Forplayers : Window
    {
        public ObservableCollection<CharacterItem> Items { get; set; }
        public ObservableCollection<CharacterItem> ItemsEdited { get; set; }


        public Forplayers(ObservableCollection<CharacterItem> items)
        {
           
            DataContext = this;
            Items = items;
            Items.CollectionChanged += Items_CollectionChanged;
            Topmost = true; // Vždy nahoře           
            ItemsEdited = new ObservableCollection<CharacterItem>();           
            InitializeComponent();
            Update();
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            Update();
        }

        public void Update()
        {            
            if (Items.Count > 0)
            {
                CharacterItem newChar = Items[0];
                FirstImage.DataContext = newChar;
                FirstName.DataContext = newChar;
            }
        }
    }
}

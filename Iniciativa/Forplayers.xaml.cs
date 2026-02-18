using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Iniciativa
{
    /// <summary>
    /// Interakční logika pro Forplayers.xaml
    /// </summary>
    public partial class Forplayers : Window
    {
        public InitiativeManager Manager { get; set; }
        public float rotation = 0;
        //public ObservableCollection<CharacterItem> ItemsEdited { get; set; }


        public Forplayers(InitiativeManager manager)
        {
           
            DataContext = manager;
            Manager = manager;
            //Items.CollectionChanged += Items_CollectionChanged;
            Topmost = true; // Vždy nahoře           
            //ItemsEdited = new ObservableCollection<CharacterItem>();           
            InitializeComponent();
            
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            Update();
        }

        public void Update()
        {
            List.Items.Refresh();
        }

        public void Rotate()
        {
            rotation += 180;
            Rotate(rotation);
            
        }

        public void Rotate(float rotation)
        {
            this.rotation = rotation;
            List.RenderTransform = new RotateTransform(rotation);
            List.RenderTransformOrigin = new Point(0.5, 0.5);
        }
    }
}

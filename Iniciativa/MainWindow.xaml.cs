using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Iniciativa
{   
    public enum NameDisplay
    {
        Name,
        IdAndName,
        None
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public NameDisplay NameDisplayOpt { get { return _nameDisplayOpt; } }
        static NameDisplay _nameDisplayOpt = NameDisplay.IdAndName;

        public ObservableCollection<CharacterItem> Items { get; set; }

        private Forplayers secondWindow;

        private CharacterItem OnTurn = null;

        private bool OpenedForPlayers = false;

        public MainWindow()
        {
            InitializeComponent();
            //Items = new ObservableCollection<CharacterItem>();
            Items = new ObservableCollection<CharacterItem>
            {
            new CharacterItem { Name = "Aegir", AvatarName = "p_Aegir"},
            new CharacterItem { Name = "Joogurt", AvatarName = "p_Joogurt" },
            new CharacterItem { Name = "Vélin", AvatarName = "p_Velin"},
            new CharacterItem { Name = "Šakal", AvatarName = "p_Sakal"},
            new CharacterItem { Name = "Mantus",AvatarName = "p_Mantus"},
            new CharacterItem { Name = "Jorge", AvatarName = "p_Jorge" },
            new CharacterItem { Name = "Viki", AvatarName = "p_Victorie" },
            new CharacterItem { Name = "Merric",AvatarName = "p_Merric"}
            };
            Details.UpdateVisibilityEvent += UpdateVisibility;
            DataContext = this;            
        }

        private void OpenSecondWindow_Click(object sender, RoutedEventArgs e)
        {
            if (OpenedForPlayers)
            {
                
                if (secondWindow != null)
                {
                    secondWindow.Hide();
                }
                OpenPlayerWindow.Content = "Otevři hráčské okno";
                OpenedForPlayers = false;
            }
            else
            {
                if (secondWindow == null || !secondWindow.IsLoaded)
                {
                    secondWindow = new Forplayers(Items);
                    secondWindow.Closed += OnPlayerWindowClosed;
                }                
                secondWindow.Show();
                OpenPlayerWindow.Content = "Zavři hráčské okno";
                OpenedForPlayers = true;
            }
            
            
        }

        private void OnPlayerWindowClosed(object? sender, EventArgs e)
        {
            if (OpenedForPlayers)
            {
                OpenedForPlayers = false;
                OpenPlayerWindow.Content = "Otevři hráčské okno";
                OpenedForPlayers = false;
            }
        }

        private void CloseSecondWindow_Click(object sender, RoutedEventArgs e)
        {
            switch (_nameDisplayOpt)
            {
                case NameDisplay.Name:
                    NameShow.Content = "Nic";
                    _nameDisplayOpt = NameDisplay.None;
                    break;
                case NameDisplay.IdAndName:
                    NameShow.Content = "Jméno";
                    _nameDisplayOpt = NameDisplay.Name;
                    break;
                case NameDisplay.None:
                    NameShow.Content = "Jméno(id)";
                    _nameDisplayOpt = NameDisplay.IdAndName;

                    break;
                default:
                    break;
            }

            if (!(secondWindow == null || !secondWindow.IsLoaded))
            {
                secondWindow.Update();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (secondWindow != null)
            {
                secondWindow.Close();
            }
        }

        private void OnItemClicked(object sender, MouseButtonEventArgs e)
        {
            // Získání aktuální položky (DataContext je aktuální prvek seznamu)
            var item = (sender as FrameworkElement)?.DataContext as CharacterItem;
            if (item != null)
            {
                Details.SetupDetail(item);
            }
        }      

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            SetNextOnTurn();
            Details.SetupDetail(OnTurn);
        }

        private void SetNextOnTurn()
        {
            Sort();
            if (OnTurn == null && Items.Count > 0)
            {
                OnTurn = Items[0];
            }
            else if (OnTurn != null && Items.Count > 2)
            {
                int it = Items.IndexOf(OnTurn);
                int next = it;
                do
                {
                    next = (next + 1) % Items.Count;
                }
                while (!Items[next].IsReady());
                OnTurn = Items[next];
                while (Items[0] != OnTurn)
                {
                    CharacterItem temp = Items[0];
                    Items.RemoveAt(0);
                    Items.Add(temp);
                }
            }
        }

        private void Sort()
        {
            for (int i = 0; i < Items.Count - 1; i++)
            {
                for (int j = 0; j < Items.Count - 1 - i; j++)
                {
                    if (Items[j].Initiative < Items[j + 1].Initiative)
                    {
                        // Výměna položek
                        var temp = Items[j];
                        Items[j] = Items[j + 1];
                        Items[j + 1] = temp;
                    }
                    else if (Items[j].Initiative == Items[j + 1].Initiative)
                    {
                        if (Items[j].InitiativeSecond < Items[j + 1].InitiativeSecond)
                        {
                            // Výměna položek
                            var temp = Items[j];
                            Items[j] = Items[j + 1];
                            Items[j + 1] = temp;
                        }
                        else if(String.Compare(Items[j].AvatarName, Items[j + 1].AvatarName, true) > 0)
                        {
                            var temp = Items[j];
                            Items[j] = Items[j + 1];
                            Items[j + 1] = temp;
                        }
                    }
                }
            }
        }

        private void UpdateVisibility(object sender, CharacterItem character)
        {
            if (character == OnTurn)
            {
                SetNextOnTurn();
            }
            if (secondWindow != null)
            {
                secondWindow.Update();
            }
        }

        private void RemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            CharacterItem charSelected = Details.Character;
            if(charSelected!= null)
            {
                Items.Remove(charSelected);
                if (charSelected == OnTurn)
                {
                    SetNextOnTurn();
                   
                }
                Details.SetupDetail(OnTurn);

            }
        }

        private void AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            CharacterItem newChar = new CharacterItem { Hide = true};
            Items.Add(newChar);
            Details.SetupDetail(newChar);
        }

        private void Opacity_Click(object sender, RoutedEventArgs e)
        {
            if (secondWindow != null)
            {
                if (secondWindow.AllowsTransparency)
                {
                    Forplayers thirdWindow = new Forplayers(Items);                  

                    // Zkopírování velikosti
                    thirdWindow.Width = secondWindow.Width;
                    thirdWindow.Height = secondWindow.Height;

                    // Zkopírování pozice
                    thirdWindow.Left = secondWindow.Left;
                    thirdWindow.Top = secondWindow.Top;
                    secondWindow.Close();

                    secondWindow = thirdWindow;
                    // Zobrazení druhého okna
                    thirdWindow.Show();
                }
                else
                {
                    Forplayers thirdWindow = new Forplayers(Items);

                    thirdWindow.WindowStyle = WindowStyle.None; // Skryje rámeček
                    thirdWindow.Background = Brushes.Transparent; // Plně průhledné pozadí                   
                    thirdWindow.AllowsTransparency = true;

                    // Zkopírování velikosti
                    thirdWindow.Width = secondWindow.Width;
                    thirdWindow.Height = secondWindow.Height;

                    // Zkopírování pozice
                    thirdWindow.Left = secondWindow.Left;
                    thirdWindow.Top = secondWindow.Top;
                    thirdWindow.Show();
                    secondWindow.Close();

                    secondWindow = thirdWindow;
                    // Zobrazení druhého okna
                    
                }
            }
        }
    }

    
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public class InitiativeManager : INotifyPropertyChanged
    {
        private CharacterItem _currentTurnId;
        public CharacterItem CurrentTurnId
        {
            get => _currentTurnId;
            set
            {
                _currentTurnId = value;
                OnPropertyChanged(nameof(SortedCharacters));
            }
        }

        public InitiativeManager(ObservableCollection<CharacterItem> Items)
        {
            Characters = Items;
        }

        public ObservableCollection<CharacterItem> Characters { get; set; } = new();

        public IEnumerable<CharacterItem> SortedCharacters =>
        Characters.OrderByDescending(c => c == CurrentTurnId) // Aktuální tah první
         .ThenByDescending(c => CurrentTurnId != null ? (CurrentTurnId.IsLowerInitiative(c) ? c.Initiative * 100 : c.Initiative) : c.Initiative)        // Seřazení podle iniciativy (sestupně)
         .ThenByDescending(c => c.InitiativeSecond); // Pokud je stejná iniciativa, použije druhou hodnotu

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void SetNextOnTurn()
        {
           
            if (CurrentTurnId == null)
            {
                CurrentTurnId = GetCharacterWithBiggestInitiative();
                return;
            }

            CharacterItem next = null;
            foreach (var item in Characters)
            {
                if (item.Hide || item == CurrentTurnId)
                    continue;
                if (CurrentTurnId.IsLowerInitiative(item))
                {
                    if (next == null)
                        next = item;
                    else if (item.IsLowerInitiative(next))
                    {
                        next = item;
                    }
                }
            }

            if (next != null)
                CurrentTurnId = next;
            else
            {
                CurrentTurnId = GetCharacterWithBiggestInitiative();
            }
        }

        private CharacterItem GetCharacterWithBiggestInitiative()
        {
            CharacterItem next = null;
            foreach (var item in Characters)
            {
                if (item.Hide || CurrentTurnId == item)
                    continue;
                if (next == null)
                {
                    next = item;
                    continue;
                }

                if (item.IsLowerInitiative(next))
                {
                    next = item;
                }
            }
            if (next != null)
                CurrentTurnId = next;
            return next;
        }

        internal void Remove(CharacterItem charSelected)
        {
            if (charSelected == CurrentTurnId)
            {
                SetNextOnTurn();
            }
            Characters.Remove(charSelected);
            OnPropertyChanged(nameof(SortedCharacters));
        }

        internal void Add(CharacterItem newChar)
        {
            Characters.Add(newChar);
            OnPropertyChanged(nameof(SortedCharacters));
        }
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

        InitiativeManager InitiativeManager;

        public MainWindow()
        {
            InitializeComponent();
            //Items = new ObservableCollection<CharacterItem>();
            Items = new ObservableCollection<CharacterItem>
            {
            new CharacterItem { Name = "Aegir", AvatarName = "aeg", Initiative = 5},
            new CharacterItem { Name = "Joogurt", AvatarName = "jog", Initiative = 15 },
            new CharacterItem { Name = "Vélin", AvatarName = "vel", Initiative = 10},
            new CharacterItem { Name = "Mantus",AvatarName = "man", Initiative = 19, InitiativeSecond = 10},
            new CharacterItem { Name = "Jorge", AvatarName = "jor" , Initiative = 19, InitiativeSecond = 9},
            new CharacterItem { Name = "Viki", AvatarName = "vic", Initiative = 3 },
            new CharacterItem { Name = "Merric",AvatarName = "mer", Initiative = 19, InitiativeSecond = 18}
            };
            Details.UpdateVisibilityEvent += UpdateVisibility;
            InitiativeManager = new InitiativeManager(Items);
            DataContext = InitiativeManager;            
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
                    secondWindow = new Forplayers(InitiativeManager);
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
            InitiativeManager.SetNextOnTurn();
            Details.SetupDetail(InitiativeManager.CurrentTurnId);            
        }

        private void UpdateVisibility(object sender, CharacterItem character)
        {
            if (character == OnTurn)
            {
                InitiativeManager.SetNextOnTurn();
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
                InitiativeManager.Remove(charSelected);
            }
        }

        private void AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            CharacterItem newChar = new CharacterItem { Hide = true};
            InitiativeManager.Add(newChar);
            Details.SetupDetail(newChar);
        }

        private void Opacity_Click(object sender, RoutedEventArgs e)
        {
            if (secondWindow != null)
            {
                if (secondWindow.AllowsTransparency)
                {
                    Forplayers thirdWindow = new Forplayers(InitiativeManager);                  

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
                    Forplayers thirdWindow = new Forplayers(InitiativeManager);

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
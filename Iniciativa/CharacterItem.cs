using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Iniciativa
{
    public class CharacterItem : INotifyPropertyChanged
    {
        private string _name;
        private string _notes;
        private int _initiative;
        private int _initiativeSecond;
        private int _hp;
        private bool _hide;
        private string _avatarName;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetField(ref _notes, value);
        }

        public int HP
        {
            get => _hp;
            set => SetField(ref _hp, value);
        }

        public int Initiative
        {
            get => _initiative;
            set => SetField(ref _initiative, value);
        }

        public int InitiativeSecond
        {
            get => _initiativeSecond;
            set => SetField(ref _initiativeSecond, value);
        }

        public string AvatarName
        {
            get => _avatarName;
            set
            {
                if (SetField(ref _avatarName, value))
                {
                    // Změna AvatarName ovlivní AvatarPath, takže oznámíme změnu také pro AvatarPath
                    OnPropertyChanged(nameof(AvatarPath));
                }
            }
        }

        public bool Hide
        {
            get => _hide;
            set => SetField(ref _hide, value);
        }

        public string AvatarPath => GetAvatarPath();

        private string GetAvatarPath()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatars");
            string file = $"{AvatarName}.png";
            if (File.Exists(Path.Combine(path, file)))
            {
                return Path.Combine(path, file);
            }
            else
            {
                return Path.Combine(path, $"{AvatarName}.jpg");
            }
        }

        // Implementace INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool IsLowerInitiative(CharacterItem lower)
        {
            if(Initiative == lower.Initiative)
            {
                return lower.InitiativeSecond < InitiativeSecond;
            }
            return lower.Initiative < Initiative;
        }

        public bool IsReady()
        {
           return !_hide;
        }
    }
}

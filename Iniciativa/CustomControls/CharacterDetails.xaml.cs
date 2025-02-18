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

namespace Iniciativa.CustomControls
{
    /// <summary>
    /// Interakční logika pro CharacterDetails.xaml
    /// </summary>
    public partial class CharacterDetails : UserControl
    {
        public delegate void UpdateVisibilityHandler(object sender, CharacterItem character);
        public event UpdateVisibilityHandler UpdateVisibilityEvent;


        public CharacterItem Character { get; set; }
        public CharacterDetails()
        {
            Character = new CharacterItem { Name = "", AvatarName = "" };
            DataContext = Character;
            InitializeComponent();
        }

        public void SetupDetail(CharacterItem character)
        {
            Character = character;
            DataContext = Character;
            modifiHP.Text = "0";
        }

        private void Dmg_Click(object sender, RoutedEventArgs e)
        {
            Character.HP = Character.HP - Convert.ToInt32(modifiHP.Text);
        }

        private void Heal_Click(object sender, RoutedEventArgs e)
        {
            Character.HP = Character.HP + Convert.ToInt32(modifiHP.Text);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateVisibilityEvent?.Invoke(this,Character);
        }
    }
}

using System;
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
using System.Windows.Shapes;

namespace Iniciativa.CustomControls
{

    public class MultiAddData
    {
        public List<int> Ids { get; set; } = new();
        public int Modifier { get; set; }
        public int HP { get; set; }
        public bool GenerateInitiative { get; set; }
    }

    /// <summary>
    /// Interaction logic for MultiAdd.xaml
    /// </summary>
    public partial class MultiAdd : Window
    {
        public MultiAddData Result { get; private set; }

        public MultiAdd()
        {
            InitializeComponent();
            Result = new MultiAddData();
        }

        private List<int> ParseIds(string input)
        {
            var result = new List<int>();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            input = input.Replace(".", ","); // nahradi tecku carkou pro pohodlnejsi zadavani rozsahu, kvuli ruzne prezentaci carky u num klavesnice v ruznych lokalizacich

            var parts = input.Split(',');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();

                if (trimmed.Contains('-'))
                {
                    var rangeParts = trimmed.Split('-');

                    if (rangeParts.Length == 2 &&
                        int.TryParse(rangeParts[0].Trim(), out int start) &&
                        int.TryParse(rangeParts[1].Trim(), out int end))
                    {
                        if (start <= end)
                        {
                            for (int i = start; i <= end; i++)
                                result.Add(i);
                        }
                    }
                }
                else
                {
                    if (int.TryParse(trimmed, out int number))
                        result.Add(number);
                }
            }

            return result.Distinct().OrderBy(x => x).ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Result = new MultiAddData
            {
                Ids = ParseIds(ID.Text),
                Modifier = int.TryParse(Mod.Text, out int mod) ? mod : 0,
                HP = int.TryParse(HP.Text, out int hp) ? hp : 0,
                GenerateInitiative = Gen.IsChecked == true
            };

            DialogResult = true;   // důležité!
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }       
    }
}

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

namespace DesktopPetVS
{
    public partial class ControlWindow : Window
    {
        private MainWindow main;
        public ControlWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            main = mainWindow;

            UpdatePetCount();
        }

        private void SpawnButton_Click(object sender, RoutedEventArgs e)
        {
            main.SpawnPet(0);
            UpdatePetCount();
        }

        private void ControlWindow_Closed(object sender, EventArgs e)
        {
            // Získať hlavné okno (MainWindow)
            if (Application.Current.MainWindow is MainWindow main)
            {
                // Zavrieť hlavné okno (peti)
                main.Close();
            }

            // Ukončí celý proces (voliteľné, istota)
            Application.Current.Shutdown();
        }

        private void UpdatePetCount()
        {
            PetCountText.Text = $"Pets: {main.PetCount}";
        }

        private void SpawnButton1_Click(object sender, RoutedEventArgs e)
        {
            main.SpawnPet(1);
            UpdatePetCount();
        }

        private void SpawnButton2_Click(object sender, RoutedEventArgs e)
        {
            main.SpawnPet(2);
            UpdatePetCount();
        }

        private void SpawnButton3_Click(object sender, RoutedEventArgs e)
        {
            main.SpawnPet(3);
            UpdatePetCount();
        }


    }
}

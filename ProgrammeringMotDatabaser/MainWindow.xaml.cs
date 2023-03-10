using ProgrammeringMotDatabaser.DAL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProgrammeringMotDatabaser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();

            string charachterName = txtcharactername.Text;

            var animal = await db.GetAnimalByName(charachterName);

            lblid.Content = animal.AnimalId;
            lblname.Content = animal.CharacterName;

            //MessageBox.Show($"There is no animal called {characterName}");



            MessageBox.Show(animal.CharacterName);







        }

       

        private async void btnload_Click(object sender, RoutedEventArgs e) 
        {
            DbRepository db = new();

            var animalspecies = await db.AddAnimalSpecieToCombox();
            cbospecie.ItemsSource = animalspecies;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";

        }
    }
}

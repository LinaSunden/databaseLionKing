using ProgrammeringMotDatabaser.DAL;
using ProgrammeringMotDatabaser.Models;
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

            var asd = GetAnimalSpecieId();

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

      
        /// <summary>
        /// Button to convert animal specie ID and connect to class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnloadclass_Click(object sender, RoutedEventArgs e)
        {
            if (cbospecie.SelectedItem is Animalspecie select)
            {

                var qwe = select.AnimalSpecieId.ToString();
                if (qwe == "7" || qwe == "8")
                    
                {
                    lblanimalclass.Content = $"Your animal belongs to the animalclass Mammals";

                }
                else if (qwe == "10")
                {
                    lblanimalclass.Content = $"Your animal belongs to the animalclass Reptiles";
                }
                else if (qwe == "11")
                {
                    lblanimalclass.Content = $"Your animal belongs to the animalclass Invertebrates";
                }
                else if (qwe == "9")
                {
                    lblanimalclass.Content = $"Your animal belongs to the animalclass Birds";
                }
                else if (qwe == "12")
                {
                    lblanimalclass.Content = $"Your animal belongs to the animalclass Fish";
                }


            }
        }

        private async void btncreateanimal_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();

            var asd = GetAnimalSpecieId();

            var animal = new Animal()
            {
                CharacterName = txtinput.Text,
                AnimalSpecieid = int.Parse(asd),

            };

            await db.AddAnimal(animal);


        }

        public string GetAnimalSpecieId()
        {

            DbRepository db = new();

            

            if (cbospecie.SelectedItem is Animalspecie select)
            {
                var qwe = select.AnimalSpecieId.ToString();
                return qwe;
            }

            return null;

        }

       
    }
}

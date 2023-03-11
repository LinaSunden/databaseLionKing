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

            string charachterName = txtcharactername.Text;

            var animal = await db.GetAnimalByName(charachterName);

            lblid.Content = animal.AnimalId;
            lblname.Content = animal.CharacterName;

            //MessageBox.Show($"There is no animal called {characterName}");



            //MessageBox.Show(animal.CharacterName);

        }

       

        private async void btnload_Click(object sender, RoutedEventArgs e) 
        {
            DbRepository db = new();

            var animalspecies = await db.AddAnimalSpecieToCombox();
            cbospecie.ItemsSource = animalspecies;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";
          
        }

      
        /// <summary>
        /// Button to convert animal specie ID and connect to class (jag har ingen aning hur det funkar tbh)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnloadclass_Click(object sender, RoutedEventArgs e)
        {
            if (cbospecie.SelectedItem is Animalspecie select)
            {

                var animaSpecieId = select.AnimalSpecieId.ToString();
                if (animaSpecieId == "7" || animaSpecieId == "8")
                {                    
                    txtblock.Text = "your animal belongs in the animal class Mammal";                      
                }
                else if (animaSpecieId == "10")
                {
                    txtblock.Text = "Your animal belongs in the animal class Reptile";
                }
                else if (animaSpecieId == "11")
                {
                    txtblock.Text = "Your animal belongs in the animal class Invertebrate";
                }
                else if (animaSpecieId == "9")
                {
                    txtblock.Text = "Your animal belongs in the animal class Bird";
                }
                else if (animaSpecieId == "12")
                {
                    txtblock.Text = "Your animal belongs in the animal class Fish";
                }


            }
        }
        /// <summary>
        /// Button that creates animal and uses specieID from class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Method to retrieve the animalspecieID
        /// </summary>
        /// <returns></returns>
        public string GetAnimalSpecieId()
        {

            DbRepository db = new();
                        
            if (cbospecie.SelectedItem is Animalspecie select)
            {
                var animalSpecieId = select.AnimalSpecieId.ToString();
                return animalSpecieId;
            }

            return null;

        }

       
    }
}

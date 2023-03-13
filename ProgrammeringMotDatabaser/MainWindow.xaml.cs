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

        DbRepository db = new();
        private async void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var animalspecie = await db.GetAnimalBySpeficClass();
            


            //string characterName = txtcharactername.Text;
            //var animal = await db.GetAnimalByName(characterName);


            //if (animal.AnimalId == 0)//Kanske finns en mer korrekt lösning på detta. Men den fungerar.
            //{
            //    MessageBox.Show($"There is no animal called {characterName}");

            //}
            //else
            //{
            //    lblanimalId.Content = animal.AnimalId;
            //    lblcharacterName.Content = animal.CharacterName;
            //}

        }



      


        /// <summary>
        /// Button that creates animal and uses specieID from class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btncreateanimal_Click(object sender, RoutedEventArgs e)
        {

            var asd = GetAnimalSpecieId();

            var animal = new Animal()
            {
                CharacterName = txtinput.Text,
                Animalspecie = new()
                { 
                    AnimalSpecieId = int.Parse(asd),
                                        
                    
                }
            };

            await db.AddAnimal(animal);

        }


        private void btncreatespecie_Click(object sender, RoutedEventArgs e)
        {



        }

        private async void btncreateclass_Click(object sender, RoutedEventArgs e)
        {
            var newClassName = txtinputclassname.Text;
            var animalClass = new Animalclass()
            {

                AnimalClassName = newClassName,

            };

            await db.AddAnimalClass(animalClass);

        }

        private async void btngetanimalclass_Click(object sender, RoutedEventArgs e)
        {

            var animalClass = await db.GetAnimalClass();
            cboclass.ItemsSource = animalClass;
            cboclass.DisplayMemberPath = "AnimalClassName";


        }

        private async void btnShowSpecie_Click(object sender, RoutedEventArgs e)
        {
            var animalSpecies = await db.GetAnimalSortedBySpecie();
            lstBox.ItemsSource = animalSpecies;
            //lstBox.DisplayMemberPath = "AnimalSpecieName";
        }


        /// <summary>
        /// loads the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnload_Click(object sender, RoutedEventArgs e)
        {

            var animalSpecies = await db.GetAnimalSortedBySpecie();
            cbospecie.ItemsSource = animalSpecies;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";

        }


        /// <summary>
        /// Button to convert animal specie ID and connect to class 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnloadclass_Click(object sender, RoutedEventArgs e)
        {


            var trying = GetAnimalSpecieId();


            if (cbospecie.SelectedItem is Animalspecie select)
            {

                var animaSpecieId = select.AnimalSpecieId.ToString();
                if (animaSpecieId == "7" || animaSpecieId == "8")
                {
                    txtoutputclass.Text = "your animal belongs in the animal class Mammal";
                }
                else if (animaSpecieId == "10")
                {
                    txtoutputclass.Text = "Your animal belongs in the animal class Reptile";
                }
                else if (animaSpecieId == "11")
                {
                    txtoutputclass.Text = "Your animal belongs in the animal class Invertebrate";
                }
                else if (animaSpecieId == "9")
                {
                    txtoutputclass.Text = "Your animal belongs in the animal class Bird";
                }
                else if (animaSpecieId == "12")
                {
                    txtoutputclass.Text = "Your animal belongs in the animal class Fish";
                }


            }
        }

        /// <summary>
        /// Method to retrieve the animalspecieID
        /// </summary>
        /// <returns></returns>
        public string GetAnimalSpecieId()
        {

            if (cbospecie.SelectedItem is Animalspecie select)
            {
                var animalSpecieId = select.AnimalSpecieId.ToString();
                return animalSpecieId;
            }

            return null;

        }

       
    }
}

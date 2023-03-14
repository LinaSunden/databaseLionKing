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
            DisplayCBO();           

        }
        
        DbRepository db = new();
        private async void btnsearch_Click(object sender, RoutedEventArgs e)
        {
        
            string characterName = txtcharactername.Text;
            var animal = await db.GetAnimalByName(characterName);


            if (animal.AnimalId == 0)//Kanske finns en mer korrekt lösning på detta. Men den fungerar.
            {
                MessageBox.Show($"There is no animal called {characterName}");

            }
            else
            {
                lblanimalId.Content = animal.AnimalId;
                lblcharacterName.Content = animal.CharacterName;
                lblanimalspeciename.Content = animal.Animalspecie.AnimalSpecieName;
            }

        }


        private async void btnshowclass_Click(object sender, RoutedEventArgs e)
        {
            var animalClasses = await db.GetAnimalClass();
            lstBox.ItemsSource = animalClasses;
            lstBox.DisplayMemberPath = "AnimalClassName";
        }

        private async void btnShowSpecie_Click(object sender, RoutedEventArgs e)
        {
            //var animalSpecies = await db.GetAnimalSpecie();
            //lstBox.ItemsSource = animalSpecies;

           var allAnimals = await db.GetAllAnimalsSortedBySpecie();
            lstBox.ItemsSource = allAnimals;




        }
       
        private async void btnsortlist_Click(object sender, RoutedEventArgs e)
        {
            Animalclass animalclass = (Animalclass)cbolistofclasses.SelectedItem;

            var listOfClasses= await db.GetAnimalBySpeficClass(animalclass);

            lstBox.ItemsSource = listOfClasses;           
        }

       

        

        /// <summary>
        /// Button that creates animal and uses specieID from class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btncreateanimal_Click(object sender, RoutedEventArgs e)
        {

            var specieId = GetAnimalSpecieId();

            var animal = new Animal()
            {
                CharacterName = txtinput.Text,
                Animalspecie = new()
                { 
                    AnimalSpecieId = int.Parse(specieId),  //vi kan nu skapa en koppling till animal class genom att vi väljer klassen från cbo 
                                       
                    
                }
            };

            await db.AddAnimal(animal);

        }


        private async void btncreatespecie_Click(object sender, RoutedEventArgs e)
        {
            var classId = GetAnimalClassId();

            var animalspecie = new Animalspecie()
            {
                AnimalSpecieName = txtinputspeciename.Text,
                //AnimalClassId = int.Parse(classId)               


                Animalclass = new()
                {
                    AnimalClassId = int.Parse(classId)

                }

            };

            await db.AddAnimalSpecie(animalspecie);
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

       


        /// <summary>
        /// loads the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnload_Click(object sender, RoutedEventArgs e)
        {

            var animalSpecies = await db.GetAnimalSpecie();
            cbospecie.ItemsSource = animalSpecies;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";

        }


        /// <summary>
        /// Button to convert animal specie ID and connect to class 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnloadclass_Click(object sender, RoutedEventArgs e)
        {
            var animaldsa = await db.GetAnimalClass();
            cboclasses2.ItemsSource = animaldsa;
            cboclasses2.DisplayMemberPath = "AnimalClassName";


            //var findAnimalClassId = db.GetAnimalSortedBySpecie();
            //var findAnimalClassName = 

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
        public string GetAnimalClassId()
        {
            
            if (cboclass.SelectedItem is Animalclass select)
            {
                var animalClassId = select.AnimalClassId.ToString();
                return animalClassId;
            }

            return null;

        }

        public string GetClassId()
        {

            if (cbospecie.SelectedItem is Animalspecie select)
            {
                var animalClassId = select.AnimalClassId.ToString();
                return animalClassId;
            }

            return null;

        }

        public async Task DisplayCBO()
        {
            var animalClass = await db.GetAnimalClass();
            cbolistofclasses.ItemsSource = animalClass;
            cbolistofclasses.DisplayMemberPath = "AnimalClassName";
        }

       
    }
}

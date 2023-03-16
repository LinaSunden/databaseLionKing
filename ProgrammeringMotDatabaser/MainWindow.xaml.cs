using ProgrammeringMotDatabaser.DAL;
using ProgrammeringMotDatabaser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            WelcomeMessage();
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




     


      
     
    
      


        /// <summary>
        /// Button that creates animal and uses specieID from class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btncreateanimal_Click(object sender, RoutedEventArgs e)
        {

            var specieId = GetAnimalSpecieId();
            string animalName = txtinput.Text;
            var checkIfAnimalExists = await db.AddAnimalAndGetValue(animalName, int.Parse(specieId));

            MessageBox.Show($"{checkIfAnimalExists.Display1}");

        }


        private async void btncreatespecie_Click(object sender, RoutedEventArgs e)
        {
            var classId = GetAnimalClassId();

            var animalspecie = new Animalspecie()
            {
                AnimalSpecieName = txtinputspeciename.Text,
                
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

            try
            {
                var newAnimalClass = await db.AddAnimalClass(animalClass);
                MessageBox.Show($"You have successfully added a new class {newAnimalClass.AnimalClassName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                       
            cboclasses2.ItemsSource = animalClass;
            cboclasses2.DisplayMemberPath = "AnimalClassName";

            cboclass.ItemsSource = animalClass;
            cboclass.DisplayMemberPath = "AnimalClassName";

            var animalSpecie = await db.GetAnimalSpecie();
            cbospecie.ItemsSource = animalSpecie;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";
        }

        private async void cboclasses2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

                if (cboclasses2.SelectedItem is Animalclass select)
                {

                   var returnAnimalSpecies = await db.GetAnimalClassesForQOne(select);
                   cbospecie.ItemsSource = returnAnimalSpecies;
                   cbospecie.DisplayMemberPath = "AnimalSpecieName";
                }


        }


       


        public async void WelcomeMessage()
        {
            var showTotalSpecies = await db.CountSpecie();
            txtBlockWelcome.Text = $"Welcome Mufasa \n Currently you have {showTotalSpecies.AnimalSpecieId} species in your kingdom";
        }

        private async void cbolistofclasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Animalclass animalclass = (Animalclass)cbolistofclasses.SelectedItem;

            var listOfClasses = await db.GetAnimalBySpeficClass(animalclass);

            lstBox.ItemsSource = listOfClasses;
        }

        private async void cbospecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Animalspecie animalspecie = (Animalspecie)cbospecie.SelectedItem;

            var selectedAnimalspecie = await db.FindClass(animalspecie);


            lblShowAnimalClassForSpecie.Content = $"Animal class: {selectedAnimalspecie.Animalclass.AnimalClassName}";
            
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
           
            var allAnimals = await db.GetAllAnimalsSortedBySpecie();

            lstBox.ItemsSource = allAnimals;
        }

        private async void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var allCharactersWithNames = await db.GetAnimalWithCharacterName();

            lstBox.ItemsSource = allCharactersWithNames;
        }


        private async void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            var numberOfSpecieInClass = await db.NumberOfSpecieInClass();
            lstBox.ItemsSource = numberOfSpecieInClass;
            lstBox.DisplayMemberPath = "CountSpeciesInClass";
        }

        private async void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            var showNumberOfAnimalsBySpecie = await db.CountAnimalInEachSpecie();

            lstBox.ItemsSource = showNumberOfAnimalsBySpecie;

            lstBox.DisplayMemberPath = "Display";
        }


    

    }
}

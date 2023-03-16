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
        
            string characterName = txtCharacterName.Text;
            var animal = await db.GetAnimalByName(characterName);


            if (animal.AnimalId == 0)//Kanske finns en mer korrekt lösning på detta. Men den fungerar.
            {
                MessageBox.Show($"There is no animal called {characterName}");
                ClearTextboxes();
            }
            else
            {
                lblCharacterName.Content = $"Character name: {animal.CharacterName}";
                lblAnimalSpecie.Content = $"Animal specie: {animal.AnimalSpecie.AnimalSpecieName}";
                lblLatinName.Content = $"Latin name: {animal.AnimalSpecie.LatinName}";
                lblAnimalClass.Content = $"Animal class: {animal.AnimalSpecie.AnimalClass.AnimalClassName}";
                ClearTextboxes();
                txtCharacterName.Focus();
   
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
            try
            {
                var checkIfAnimalExists = await db.AddAnimalAndGetValue(animalName, int.Parse(specieId));
                MessageBox.Show($"{checkIfAnimalExists.Display1}");
                await DisplayCBO();
              
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }         

        }


        private async void btncreatespecie_Click(object sender, RoutedEventArgs e)
        {
            var classId = GetAnimalClassId();
            string animalSpecieName = txtinputspeciename.Text;

            if (animalSpecieName == "")
            {
                MessageBox.Show("Please fill in the name of the animal specie you wish to create");

            }
            else
            {
                try
                {
                    var animalspecie = await db.AddAnimalSpecie(animalSpecieName, int.Parse(classId));

                    MessageBox.Show($"You have successfully added a new specie {animalspecie.AnimalSpecieName} from the {animalspecie.AnimalClass.AnimalClassName} class");
                   await DisplayCBO();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void btncreateclass_Click(object sender, RoutedEventArgs e)
        {
           
            var newClassName = txtinputclassname.Text;
            if (newClassName == "")
            {
                MessageBox.Show("Please fill in the name of the animal class you wish to create");
            }

            else
            {
                var animalClass = new AnimalClass()
                {

                    AnimalClassName = newClassName,

                };

                try
                {
                    var newAnimalClass = await db.AddAnimalClass(animalClass);
                    MessageBox.Show($"You have successfully added a new class {newAnimalClass.AnimalClassName}");
                    await DisplayCBO();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

  

       





        /// <summary>
        /// Method to retrieve the animalspecieID
        /// </summary>
        /// <returns></returns>
        public string GetAnimalSpecieId()
        {

            if (cbospecie.SelectedItem is AnimalSpecie select)
            {
                var animalSpecieId = select.AnimalSpecieId.ToString();
                return animalSpecieId;
            }

            return null;

        }
        public string GetAnimalClassId()
        {

            if (cboclass.SelectedItem is AnimalClass select)
            {
                var animalClassId = select.AnimalClassId.ToString();
                return animalClassId;
            }

            return null;

        }

        //public string GetClassId()
        //{

        //    if (cbospecie.SelectedItem is AnimalSpecie select)
        //    {
        //        var animalClassId = select.AnimalClass.AnimalClassId.ToString();
        //        return animalClassId;
        //    }

        //    return null;

        //}

        public async Task DisplayCBO()
        {
            var animalClass = await db.GetAnimalClass();

            cbolistofclasses.ItemsSource = animalClass;
            cbolistofclasses.DisplayMemberPath = "AnimalClassName";
                          
            cboclass.ItemsSource = animalClass;
            cboclass.DisplayMemberPath = "AnimalClassName";

            var animalSpecie = await db.GetAnimalSpecie();
            cbospecie.ItemsSource = animalSpecie;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";
        }

      

       
        public async void WelcomeMessage()
        {
            var showTotalSpecies = await db.CountSpecie();
            txtBlockWelcome.Text = $"Welcome Mufasa \n Currently you have {showTotalSpecies.AnimalSpecieId} species in your kingdom";
        }

        private async void cbolistofclasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalClass animalclass = (AnimalClass)cbolistofclasses.SelectedItem;

            var listOfClasses = await db.GetAnimalBySpeficClass(animalclass);

            lstBox.ItemsSource = listOfClasses;
        }

        private async void cbospecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cbospecie.SelectedItem;

            var selectedAnimalspecie = await db.FindClass(animalspecie);


            lblShowAnimalClassForSpecie.Content = $"Animal class: {selectedAnimalspecie.AnimalClass.AnimalClassName}";
            
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
           
            var allAnimals = await db.MainMethodRetrieveAllInfoAboutAnimal();

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
          var animal = await db.MainMethodRetrieveAllInfoAboutAnimal();
           


        }

        private void ClearTextboxes()
        {
            txtinputspeciename.Clear();
            txtinputclassname.Clear();
            txtinputlatinname.Clear();
            txtinput.Clear();
            txtCharacterName.Clear();
        }

   
    }
}

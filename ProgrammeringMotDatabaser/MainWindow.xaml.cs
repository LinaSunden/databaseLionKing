using ProgrammeringMotDatabaser.DAL;
using ProgrammeringMotDatabaser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        Animal animal = new();

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
            var animalClass = GetAnimalClass();
            string animalSpecieName = txtinputspeciename.Text;
            string latinName = txtinputlatinname.Text;

            if (animalSpecieName == string.Empty)
            {
                MessageBox.Show("Please fill in the name of the animal specie you wish to create");

            }
          
            else
            {
                if (latinName == string.Empty)
                {
                    latinName = null;
                }
                try
                {
                    var animalspecie = await db.AddAnimalSpecie(animalSpecieName, latinName, (int)animalClass.AnimalClassId);
                  

                    MessageBox.Show($"You have successfully added a new specie {animalspecie.AnimalSpecieName} from the {animalClass.AnimalClassName} class {animalspecie.LatinName}");
                    await DisplayCBO();
                    ClearTextboxes();
                    txtinputspeciename.Focus();
                    WelcomeMessage();
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


        private async void btnupdateanimal_Click(object sender, RoutedEventArgs e)
        {                   
            string newCharacterName = txtupdatecharacternameinput.Text;
            string newLatinName = txtupdatelatinname.Text;
            var animalid = lblupdateanimalid.Content;

            if (newCharacterName == string.Empty)
            {
                newCharacterName = null;          
            }

            var newAnimalName = await db.UpdateCharacterName(newCharacterName, DisplaySelectedAnimalInTextBox());


            if (newLatinName == string.Empty)
            {
                newLatinName = null;     
            }

            var updatedLatinName = await db.UpdateLatinName(newLatinName, DisplaySelectedAnimalInTextBox());



            var updatedAnimalSpecie = await db.UpdateAnimalSpecie(updatedLatinName.AnimalSpecie.AnimalSpecieId, (int)animalid);



            if (newAnimalName.CharacterName != newCharacterName && updatedLatinName.AnimalSpecie.LatinName != newLatinName) //dubbelkolla vrf inte meddelandena dyker upp 
            {
                MessageBox.Show($"Your animal with animal id {newAnimalName.AnimalId} has updated name and class latin name.");

            }
            else if (newAnimalName.CharacterName != newCharacterName)
            {
                MessageBox.Show($"Animal with id: {newAnimalName.AnimalId} has the new name: {newAnimalName.CharacterName} ");
            }
            //else if (updatedLatinName.AnimalSpecie.LatinName != newLatinName)
            //{
            //    MessageBox.Show($"{animalClass} latin name is now updated to: {updatedLatinName.AnimalSpecie.LatinName}");
            //}
           
            ClearTextboxes();
            var updateListBox = await db.MainMethodRetrieveAllInfoAboutAnimal();
            lstBox.ItemsSource= updateListBox;
            
        }
        private async void cboupdateanimalspecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cboupdateanimalspecie.SelectedItem;

            var selectedAnimalspecie = await db.FindClass(animalspecie);

            lblupdateanimalclass.Content = $"{selectedAnimalspecie.AnimalClass.AnimalClassName}";
            txtupdateanimalclass.Text = $"{selectedAnimalspecie.AnimalClass.AnimalClassName}";

            txtupdatelatinname.Text = $"{selectedAnimalspecie.LatinName}";
            txtupdateanimalspecie.Text = $"{selectedAnimalspecie.AnimalSpecieName}";

            btnupdateanimal.IsEnabled= true;
          
        }


        private void lstBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {           
            DisplaySelectedAnimalInTextBox();
        }

        internal Animal DisplaySelectedAnimalInTextBox()
        {

            Animal selected = lstBox.SelectedItem as Animal;

            lblupdateanimalclass.Content = selected.AnimalSpecie.AnimalClass.AnimalClassName;
            lblupdateanimalid.Content = $"Animal id: {selected.AnimalId}";
            txtupdatecharacternameinput.Text = selected.CharacterName;
            txtupdateanimalspecie.Text = selected.AnimalSpecie.AnimalSpecieName;
            txtupdatelatinname.Text = selected.AnimalSpecie.LatinName;
           
            lblupdateanimalclass.Content = selected.AnimalSpecie.AnimalClass.AnimalClassName;
            btnupdateanimal.IsEnabled = false;
            
            return selected;
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
     

        internal AnimalClass GetAnimalClass()
        {

            if (cboclass.SelectedItem is AnimalClass selectClass)
            {              
                return selectClass;
            }

            return null;

        }



        public async Task DisplayCBO()
        {
            var animalClass = await db.GetAnimalClass();

            cbolistofclasses.ItemsSource = animalClass;
            cbolistofclasses.DisplayMemberPath = "AnimalClassName";
                          
            cboclass.ItemsSource = animalClass;
            cboclass.DisplayMemberPath = "AnimalClassName";

            cboupdateanimalclass.ItemsSource = animalClass;
            cboupdateanimalclass.DisplayMemberPath = "AnimalClassName";

            var animalSpecie = await db.GetAnimalSpecie();
            cbospecie.ItemsSource = animalSpecie;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";

            cboupdateanimalspecie.ItemsSource = animalSpecie;
            cboupdateanimalspecie.DisplayMemberPath = "AnimalSpecieName";

        }

      

       
        public async void WelcomeMessage()
        {
            var showTotalSpecies = await db.CountSpecie();
            txtBlockWelcome.Text = $"Welcome Mufasa \n Currently you have {showTotalSpecies.AnimalSpecieId} species in your kingdom";

            btnupdateanimal.IsEnabled= false ;
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

   

        private void ClearTextboxes()
        {
            txtinputspeciename.Clear();
            txtinputclassname.Clear();
            txtinputlatinname.Clear();
            txtinput.Clear();
            txtCharacterName.Clear();
            txtupdateanimalclass.Clear();
            txtupdateanimalspecie.Clear();
            txtupdatecharacternameinput.Clear();
            txtupdatelatinname.Clear();
            lblupdateanimalid.Content = string.Empty;
        }

        private void txtupdatecharacternameinput_KeyDown(object sender, KeyEventArgs e) //dubbelkolla om vi kan lösa detta med delete keyn också
        {
            if (e.Key == Key.Delete || e.Key != Key.Enter)
            {
                btnupdateanimal.IsEnabled= true;

            }
        }

        private void txtupdatelatinname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                btnupdateanimal.IsEnabled= true;

            }
        }

        private async void btntestlist_Click(object sender, RoutedEventArgs e)
        {

            var testing = await db.MainMethodRetrieveAllInfoAboutAnimal();

              var qwe =  animal.AnimalSpecie.AnimalClass.AnimalClassName;

           

            //    var idk = await ac.GetAnimalClassTest();

            //lstBox.ItemsSource = testing;
            //lstBox.DisplayMemberPath = "AnimalClassName";

        }
}
}

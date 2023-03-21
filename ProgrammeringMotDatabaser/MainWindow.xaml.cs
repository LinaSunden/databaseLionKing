﻿using ProgrammeringMotDatabaser.DAL;
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


        #region Create 
 


        /// <summary>
        /// Button that creates a animal class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    //await DisplayCBO();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            DisplayCBO();

            UpdateListBoxes();
            ClearCbo();
            ClearTextboxes();
        }

        /// <summary>
        /// Button that create a animal specie 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btncreatespecie_Click(object sender, RoutedEventArgs e)
        {
            var animalClass = GetAnimalClass();
            string animalSpecieName = txtinputspeciename.Text;
            string latinName = txtinputlatinname.Text;

            if (animalSpecieName == string.Empty)
            {
                MessageBox.Show("Please fill in the name of the animal specie you wish to create");

            }
            else if (animalClass == null)
            {
                MessageBox.Show("Please select an animalclass from the dropdown box");
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

                    DisplayCBO();

                    UpdateListBoxes();
                    ClearCbo();
                    ClearTextboxes();
                    rdbtnAllAnimals.IsChecked = true;
                    txtinputspeciename.Focus();
                    WelcomeMessage();


                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
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

            var specieId = GetAnimalSpecieId();
            string animalName = txtinput.Text;

            if (specieId == null)
            {
                MessageBox.Show("To create an animal you need to declare it's specie from the combobox below");

            }
            else
            {
               

                try
                {
                    if (animalName == string.Empty)
                    {
                        animalName = null;
                    }

                    var animal = new Animal()
                    {

                        CharacterName = animalName,

                        AnimalSpecie = new()
                        {
                            AnimalSpecieId = int.Parse(specieId)

                        }

                    };

                    var createdAnimal = await db.AddAnimal(animal);
                    
                    var animalWithSpecieName = await db.GetAnimalById(createdAnimal.AnimalId);

                    MessageBox.Show($"You have successfully created {createdAnimal.CharacterName} an animal who is a {animalWithSpecieName.AnimalSpecie.AnimalSpecieName} with animal id: {createdAnimal.AnimalId}");

                    txtinput.Focus();
                    DisplayCBO();
                    UpdateListBoxes();
                    ClearCbo();
                    ClearTextboxes();
                    rdbtnAllAnimals.IsChecked = true;
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }

                
            

             



          

        }

        #endregion



        #region Read

        /// <summary>
        /// Search after an animal with a specific charatername 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnsearch_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string characterName = txtCharacterName.Text;
                var animal = await db.GetAnimalByCharacterName(characterName);


                if (animal.CharacterName == null)//Kanske finns en mer korrekt lösning på detta. Men den fungerar.
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



        #endregion





        #region Update

        /// <summary>
        /// Update an animal, possible to select newCharacterName, newLatinName and new animal specie 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnupdateanimal_Click(object sender, RoutedEventArgs e)
        {
            string newCharacterName = txtupdatecharacternameinput.Text;
            string newLatinName = txtupdatelatinname.Text;
            var animalspecie = txtupdateanimalspecie.Text;

            if (newCharacterName == string.Empty || newCharacterName == null)
            {
                newCharacterName = null;
            }

            try
            {
                var newAnimalName = await db.UpdateCharacterName(newCharacterName, DisplaySelectedAnimalInTextBox());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


            if (newLatinName == string.Empty || newLatinName == null)
            {
                newLatinName = null;
            }

            try
            {
                var updatedLatinName = await db.UpdateLatinName(newLatinName, animalspecie);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            try
            {
                if (cboupdateanimalspecie.SelectedItem is AnimalSpecie select)
                {
                    int animaSpecieId = select.AnimalSpecieId;

                    var updatedAnimalSpecie = await db.UpdateAnimalSpecie(DisplaySelectedAnimalInTextBox(), animaSpecieId);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            ClearCbo();
            ClearTextboxes();
            UpdateListBoxes();
            DisplayCBO();
        }

        #endregion



        #region Delete
        /// <summary>
        /// Deletes animal from database 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                await db.DeleteAnimal(DisplayAnimalFromDeleteListBox());
                ClearCbo();
                ClearTextboxes();
                UpdateListBoxes();
                DisplayCBO();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

       /// <summary>
       /// Deletes animal specie if there are no animals with that specie, otherwise recieve option to delete them first
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private async void btnDeleteAnimalSpecie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboDeleteAimalSpecie.SelectedItem is AnimalSpecie select)
                {
             
                    await db.DeleteAnimalSpecie(select);
                    MessageBox.Show($"The animal specie {select.AnimalSpecieName} is successfully deleted");
                    ClearCbo();
                    ClearTextboxes();
                    UpdateListBoxes();
                    DisplayCBO();
                    WelcomeMessage();
                }
                
            }
               catch (Exception ex) 
            {
               MessageBoxResult messageBoxResult = MessageBox.Show(ex.Message, "Message", MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (cboDeleteAimalSpecie.SelectedItem is AnimalSpecie select)
                    {
                        await db.DeleteAnimalInSpecie(select); // Lägga i en transaktion? Båda metoderna..
                        await db.DeleteAnimalSpecie(select);
                        MessageBox.Show($"All animals in the {select.AnimalSpecieName} specie, and the specie itself, is now deleted");
                        ClearCbo();
                        ClearTextboxes();
                        UpdateListBoxes();
                        DisplayCBO();
                        WelcomeMessage();

                    }

                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    ClearCbo();

                }
            }

        }

        /// <summary>
        /// Delete a animal class 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDeleteAnimalClass_Click(object sender, RoutedEventArgs e) //återkom till denna när vi har tid, se till att användaren kan se vilka arter som ska tas bort elr erbjud användaren att ta bort arterna som krävs
        {
            try
            {
                if (cboDeleteAimalClass.SelectedItem is AnimalClass select)
                {

                    await db.DeleteAnimalClass(select);
                    MessageBox.Show($"The {select.AnimalClassName} class is now deleted");
                    ClearCbo();
                    ClearTextboxes();
                    UpdateListBoxes();
                    DisplayCBO();
                    WelcomeMessage();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     
   
        /// <summary>
        /// View the specific animal that the user double-click on in the listbox
        /// </summary>
        /// <returns></returns>
        internal Animal DisplayAnimalFromDeleteListBox()
        {
            Animal selected = lstBoxDelete.SelectedItem as Animal;

            lblDeleteAnimalid.Content = $"AnimalId: {selected.AnimalId}";
            lblCharacterNameDelete.Content = $"Name: {selected.CharacterName}";
            lblAnimalSpecieDelete.Content = $"Specie: {selected.AnimalSpecie.AnimalSpecieName}";

            return selected;
        }

        #endregion



      

        /// <summary>
        /// Uses the main List to display a selected animal 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplaySelectedAnimalInTextBox();
        }
      
        /// <summary>
        /// Displays animaldetails in labels from the delete listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBoxDelete_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DisplayAnimalFromDeleteListBox();
        }

        /// <summary>
        /// Method to display the selected animal from main list in labels
        /// </summary>
        /// <returns></returns>
        internal Animal DisplaySelectedAnimalInTextBox()
        {

            Animal selected = lstBox.SelectedItem as Animal;

            lblupdateanimalclass.Content = selected.AnimalSpecie.AnimalClass.AnimalClassName;
            lblupdateanimalid.Content = $"Animal id: {selected.AnimalId}";
            txtupdatecharacternameinput.Text = selected.CharacterName;
            txtupdateanimalspecie.Text = selected.AnimalSpecie.AnimalSpecieName;
            txtupdatelatinname.Text = selected.AnimalSpecie.LatinName;
            var animalspecieid = selected.AnimalSpecie.AnimalSpecieId;

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

        /// <summary>
        /// Method to retrieve chosen item as animalclass
        /// </summary>
        /// <returns></returns>
        internal AnimalClass GetAnimalClass()
        {

            if (cboclass.SelectedItem is AnimalClass selectClass)
            {
                return selectClass;
            }

            return null;

        }



      



        /// <summary>
        /// Welcome message when the program starts. Contains info about how many species there is in the animalregristry
        /// </summary>
        public async void WelcomeMessage()
        {
            try
            {
                var showTotalSpecies = await db.CountSpecie();
                txtBlockWelcome.Text = $"Welcome Mufasa \n Currently you have {showTotalSpecies.AnimalSpecieId} species in your kingdom";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                var displayList = await db.AllInfoAboutAllAnimals();
                lstBoxDelete.ItemsSource = displayList;
                lstBoxDelete.DisplayMemberPath = "DeleteAnimals";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

          

            btnupdateanimal.IsEnabled = false;
        }

        private async void cboupdateanimalspecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cboupdateanimalspecie.SelectedItem;
            if (animalspecie == null)
            {
                return;
            }
            try
            {

                var selectedAnimalspecie = await db.FindClass(animalspecie);

                lblupdateanimalclass.Content = $"{selectedAnimalspecie.AnimalClass.AnimalClassName}";

                txtupdatelatinname.Text = $"{selectedAnimalspecie.LatinName}";
                txtupdateanimalspecie.Text = $"{selectedAnimalspecie.AnimalSpecieName}";

                btnupdateanimal.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void cbolistofclasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalClass animalclass = (AnimalClass)cbolistofclasses.SelectedItem;
            if (animalclass == null)
            {
                return;
            }
            try
            {              
                var listOfClasses = await db.GetAnimalBySpeficClass(animalclass);

                lstBox.ItemsSource = listOfClasses;
                lstBox.DisplayMemberPath = "AnimalsInEachClass";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void cbospecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cbospecie.SelectedItem;

            if (animalspecie == null)
            {
                return;
            }

            try
            {
                var selectedAnimalspecie = await db.FindClass(animalspecie);
                lblShowAnimalClassForSpecie.Content = $"Animal class: {selectedAnimalspecie.AnimalClass.AnimalClassName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }
        private void txtCharacterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnsearch.IsEnabled = true;

        }




        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var allAnimals = await db.AllInfoAboutAllAnimals();
                lstBox.ItemsSource = null;
                lstBox.ItemsSource = allAnimals;
                lstBox.DisplayMemberPath = "AllInfoAboutAnimals";
                UpdateListBoxes();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           
        }

        private async void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {

                var allCharactersWithNames = await db.GetAnimalWithCharacterName();

                lstBox.ItemsSource = null;
                lstBox.ItemsSource = allCharactersWithNames;
                lstBox.DisplayMemberPath = "AllAnimalsWithAName";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// when checked displays how many species is in a class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var numberOfAnimalsInClass = await db.NumberOfAnimalsInClass();
                lstBox.ItemsSource = null;
                lstBox.ItemsSource = numberOfAnimalsInClass;
                lstBox.DisplayMemberPath = "CountAnimalsInClass";
                
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);          
            }
              
        }
        /// <summary>
        /// when checked displays how many animals is in each specie 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var showNumberOfAnimalsBySpecie = await db.CountAnimalInEachSpecie();
                lstBox.ItemsSource = null;
                lstBox.ItemsSource = showNumberOfAnimalsBySpecie;
                lstBox.DisplayMemberPath = "CountAnimalInEachSpecie";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }
        /// <summary>
        /// Makes sure that the comboboxes displays animalclass and speice lists
        /// </summary>
        /// <returns></returns>
        public async Task DisplayCBO()
        {
            var animalClass = await db.GetAnimalClass();

            cbolistofclasses.ItemsSource = animalClass;
            cbolistofclasses.DisplayMemberPath = "AnimalClassName";

            cboclass.ItemsSource = animalClass;
            cboclass.DisplayMemberPath = "AnimalClassName";

            cboDeleteAimalClass.ItemsSource = animalClass;
            cboDeleteAimalClass.DisplayMemberPath = "AnimalClassName";


            var animalSpecie = await db.GetAnimalSpecie();
            cbospecie.ItemsSource = animalSpecie;
            cbospecie.DisplayMemberPath = "AnimalSpecieName";

            cboupdateanimalspecie.ItemsSource = animalSpecie;
            cboupdateanimalspecie.DisplayMemberPath = "AnimalSpecieName";

            cboDeleteAimalSpecie.ItemsSource = animalSpecie;
            cboDeleteAimalSpecie.DisplayMemberPath = "AnimalSpecieName";



        }
        /// <summary>
        /// Updates main list and delete list
        /// </summary>
        private async void UpdateListBoxes()
        {
            var displayList = await db.AllInfoAboutAllAnimals();

            lstBoxDelete.ItemsSource = displayList;
            lstBoxDelete.DisplayMemberPath = "DeleteAnimals";

            lstBox.ItemsSource = displayList;
            lstBox.DisplayMemberPath = "AllInfoAboutAnimals";

        }

        /// <summary>
        /// Clears all textboxes
        /// </summary>
        private void ClearTextboxes()
        {
            txtinputspeciename.Clear();
            txtinputclassname.Clear();
            txtinputlatinname.Clear();
            txtinput.Clear();
            txtCharacterName.Clear();           
            txtupdateanimalspecie.Clear();
            txtupdatecharacternameinput.Clear();
            txtupdatelatinname.Clear();
            lblupdateanimalid.Content = string.Empty;
            lblupdateanimalclass.Content =  string.Empty;
            lblShowAnimalClassForSpecie.Content = string.Empty;
        }
        /// <summary>
        /// Resets all comboboxes
        /// </summary>
        private void ClearCbo()
        {
            cbospecie.SelectedItem = null;
            cboclass.SelectedItem = null;
            cbolistofclasses.SelectedItem = null;
            cboDeleteAimalClass.SelectedItem= null;
            cboDeleteAimalSpecie.SelectedItem= null;
            cboupdateanimalspecie.SelectedItem= null;

        }
        /// <summary>
        /// Enables the "update" button when a key that is not enter is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtupdatecharacternameinput_KeyDown(object sender, KeyEventArgs e) //dubbelkolla om vi kan lösa detta med delete keyn också
        {
            if (e.Key == Key.Delete || e.Key != Key.Enter)
            {
                btnupdateanimal.IsEnabled= true;

            }
        }
        /// <summary>
        /// Enables the "update" button when a key that is not enter is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtupdatelatinname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                btnupdateanimal.IsEnabled= true;

            }
        }

        
      

    }
}

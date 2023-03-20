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


        private async void btnsearch_Click(object sender, RoutedEventArgs e)
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


        /// <summary>
        /// Button that creates animal and uses specieID from class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btncreateanimal_Click(object sender, RoutedEventArgs e)
        {

            var specieId = GetAnimalSpecieId();
            string animalName = txtinput.Text;
          
            if ( specieId == null ) 
            {
                MessageBox.Show("To create an animal you need to declare it's specie from the combobox below");
                
            }
            else
                    try
                    {
                        var checkIfAnimalExists = await db.AddAnimal(animalName, int.Parse(specieId));
                        MessageBox.Show($"{checkIfAnimalExists.CreateAnimalSuccess}");
                        

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }


            DisplayCBO();

            UpdateListBoxes();
            ClearCbo();
            ClearTextboxes();

            rdbtnAllAnimals.IsChecked = true;

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
            var animalspecie = txtupdateanimalspecie.Text;

            if (newCharacterName == string.Empty || newCharacterName == null)
            {
                newCharacterName = null;
            }

            var newAnimalName = await db.UpdateCharacterName(newCharacterName, DisplaySelectedAnimalInTextBox());


            if (newLatinName == string.Empty || newLatinName == null)
            {
                newLatinName = null;
            }

            var updatedLatinName = await db.UpdateLatinName(newLatinName, animalspecie);

            if (cboupdateanimalspecie.SelectedItem is AnimalSpecie select)
            {
                int animaSpecieId = select.AnimalSpecieId;

                var updatedAnimalSpecie = await db.UpdateAnimalSpecie(DisplaySelectedAnimalInTextBox(), animaSpecieId);
            }

            ClearTextboxes();
            var updateListBox = await db.AllInfoAboutAllAnimals();
            lstBox.ItemsSource = updateListBox;

        }

        private async void btnDeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            await db.DeleteAnimal(DisplayAnimalsDeleteListBox());
        }
       
        private async void btnDeleteAnimalSpecie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboDeleteAimalSpecie.SelectedItem is AnimalSpecie select)
                {
             
                    await db.DeleteAnimalSpecie(select);
                }
            }
               catch (Exception ex) 
            {
               MessageBoxResult messageBoxResult = MessageBox.Show(ex.Message, "Message", MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (cboDeleteAimalSpecie.SelectedItem is AnimalSpecie select)
                    {
                        await db.DeleteAnimalInSpecie(select);
                        await db.DeleteAnimalSpecie(select);
                        MessageBox.Show($"All animals in the {select.AnimalSpecieName} specie, and the specie itself, is now deleted");
                    }
                   
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {

                    //Kan lägga till att comboboxen hamnar på ursprungsläget igen att ingenting är valt.
                }
            }

        }
        private async void btnDeleteAnimalClass_Click(object sender, RoutedEventArgs e) //återkom till denna när vi har tid, se till att användaren kan se vilka arter som ska tas bort elr erbjud användaren att ta bort arterna som krävs
        {
            try
            {
                if (cboDeleteAimalClass.SelectedItem is AnimalClass select)
                {

                    await db.DeleteAnimalClass(select);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void lstBoxDelete_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayAnimalsDeleteListBox();
        }






        internal Animal DisplayAnimalsDeleteListBox()
        {
            Animal selected = lstBoxDelete.SelectedItem as Animal;

            lblDeleteAnimalid.Content = $"AnimalId: {selected.AnimalId}";
            lblCharacterNameDelete.Content = $"Name: {selected.CharacterName}";
            lblAnimalSpecieDelete.Content = $"Specie: {selected.AnimalSpecie.AnimalSpecieName}";

            return selected;
        }





        private async void cboupdateanimalspecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cboupdateanimalspecie.SelectedItem;
            if (animalspecie == null)
            {
                return;
            }

            var selectedAnimalspecie = await db.FindClass(animalspecie);

            lblupdateanimalclass.Content = $"{selectedAnimalspecie.AnimalClass.AnimalClassName}";

            txtupdatelatinname.Text = $"{selectedAnimalspecie.LatinName}";
            txtupdateanimalspecie.Text = $"{selectedAnimalspecie.AnimalSpecieName}";

            btnupdateanimal.IsEnabled = true;

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
        private async void UpdateListBoxes()
        {
            var displayList = await db.AllInfoAboutAllAnimals();

            lstBoxDelete.ItemsSource = displayList;
            lstBoxDelete.DisplayMemberPath = "DeleteAnimals";

            lstBox.ItemsSource = displayList;
            lstBox.DisplayMemberPath = "AllInfoAboutAnimals";

        }




        public async void WelcomeMessage()
        {
            var showTotalSpecies = await db.CountSpecie();
            txtBlockWelcome.Text = $"Welcome Mufasa \n Currently you have {showTotalSpecies.AnimalSpecieId} species in your kingdom";

            var displayList = await db.AllInfoAboutAllAnimals();

            lstBoxDelete.ItemsSource = displayList;
            lstBoxDelete.DisplayMemberPath = "DeleteAnimals";

            btnupdateanimal.IsEnabled = false;
        }

        private async void cbolistofclasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalClass animalclass = (AnimalClass)cbolistofclasses.SelectedItem;

            var listOfClasses = await db.GetAnimalBySpeficClass(animalclass);

            lstBox.ItemsSource = listOfClasses;
            lstBox.DisplayMemberPath = "AnimalsInEachClass";

        }

        private async void cbospecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnimalSpecie animalspecie = (AnimalSpecie)cbospecie.SelectedItem;

            if (animalspecie == null)
            {
                return;
            }

            var selectedAnimalspecie = await db.FindClass(animalspecie);


            lblShowAnimalClassForSpecie.Content = $"Animal class: {selectedAnimalspecie.AnimalClass.AnimalClassName}";

        }





        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

            var allAnimals = await db.AllInfoAboutAllAnimals();

            lstBox.ItemsSource = null;
            lstBox.ItemsSource = allAnimals;
            lstBox.DisplayMemberPath = "AllInfoAboutAnimals";
        }

        private async void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

            var allCharactersWithNames = await db.GetAnimalWithCharacterName();

            lstBox.ItemsSource = null;
            lstBox.ItemsSource = allCharactersWithNames;
            lstBox.DisplayMemberPath = "AllAnimalsWithAName";

        }


        private async void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            var numberOfSpecieInClass = await db.NumberOfSpecieInClass();
            lstBox.ItemsSource = null;
            lstBox.ItemsSource = numberOfSpecieInClass;
            lstBox.DisplayMemberPath = "CountSpeciesInClass";
              
        }

        private async void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            var showNumberOfAnimalsBySpecie = await db.CountAnimalInEachSpecie();

            lstBox.ItemsSource = null;
            lstBox.ItemsSource = showNumberOfAnimalsBySpecie;
            lstBox.DisplayMemberPath = "CountAnimalInEachSpecie";
        }

      


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

        private void ClearCbo()
        {
            cbospecie.SelectedItem = null;
            cboclass.SelectedItem = null;
            cbolistofclasses.SelectedItem = null;
            cboDeleteAimalClass.SelectedItem= null;
            cboDeleteAimalSpecie.SelectedItem= null;
            cboupdateanimalspecie.SelectedItem= null;

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

        private void txtCharacterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnsearch.IsEnabled= true;

        }

        //private void cboDeleteAimalSpecie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}


    }
}

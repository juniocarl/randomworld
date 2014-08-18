using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace RandomWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            PasswordLengthSlider.ValueChanged += PasswordLengthSlider_ValueChanged;
            InicializarMoneda();
            ReadFile();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }


        #region UTIL 

        private string prevInput;

        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            prevInput = (sender as TextBox).Text;
        }

        private void ShowMessage(string str)
        {
            MessageDialog ms = new MessageDialog(str);
            ms.ShowAsync();
        }

        async void CreateFile(string password)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("password.dat", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, password);
                ReadFile();
            }
            catch(Exception ex)
            {
                ShowMessage(ex.Message);
            }

        }

        async void ReadFile()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await localFolder.GetFileAsync("password.dat");
                SavedPassword.Text = await FileIO.ReadTextAsync(file);                
            }
            catch(FileNotFoundException)
            {

            }
            catch (Exception ex){
                ShowMessage(ex.Message);
            }
        }

        #endregion 

        #region NUMBER

        private void GetNumber_Click(object sender, RoutedEventArgs e)
        {
            System.Random random = new System.Random();
            string result = result = (random.Next(Int32.Parse(NumberMinimum.Text), Int32.Parse(NumberMaximum.Text) + 1)).ToString();
            DisplayedNumber.Text = result;
        }        

        private void Number_LostFocus(object sender, RoutedEventArgs e)
        {
            string value = (sender as TextBox).Text;
            // Si es numero, validar que sea número
            try { Int32.Parse(value); }
            catch (Exception)
            {
                ShowMessage("El valor ingresado no es numérico.");
                (sender as TextBox).Text = prevInput;
                return;
            }

            if (Int32.Parse(NumberMinimum.Text) > Int32.Parse(NumberMaximum.Text))
            {
                ShowMessage("El valor mínimo debe ser menor o igual al valor máximo.");
                (sender as TextBox).Text = prevInput;
                return;
            }
        }

        #endregion

        #region LETTER

        private void GetLetter_Click(object sender, RoutedEventArgs e)
        {
            System.Random random = new System.Random();
            string result = null;
            int min = LetterMinimum.Text.ToUpper()[0] - 'A';
            int max = LetterMaximum.Text.ToUpper()[0] - 'A';
            char c = ((char)(random.Next(min, max + 1) + 'A'));
            result = c.ToString();
            DisplayedLetter.Text = result;
        }

        private void Letter_LostFocus(object sender, RoutedEventArgs e)
        {
            string value = (sender as TextBox).Text;
            if (value.ToUpper()[0] < 'A' || value.ToUpper()[0] > 'Z')
            {
                ShowMessage("El valor ingresado no es una letra.");
                (sender as TextBox).Text = prevInput;
                return;
            }
            if (LetterMinimum.Text.ToUpper()[0] > LetterMaximum.Text.ToUpper()[0])
            {
                ShowMessage("El valor mínimo debe ser menor o igual al valor máximo.");
                (sender as TextBox).Text = prevInput;
                return;
            }
        }

        #endregion

        #region VOLADO
        private void InicializarMoneda()
        {
            ImageMoneda.Source = new BitmapImage(new Uri(@"ms-appx:/Images/aguila.png", UriKind.Absolute));
        }

        private async void Button_Lanzar_Volado_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            System.Random random = new System.Random();
            var iteraciones = random.Next(20, 26);
            string[] nombres = { @"ms-appx:/Images/aguila.png", @"ms-appx:/Images/sol.png" };
            var sol = new BitmapImage(
                    new Uri(nombres[0], UriKind.Absolute)
                );
            var aguila = new BitmapImage(
                    new Uri(nombres[1], UriKind.Absolute)
                );

            for (int i = 0; i < iteraciones; i++)
            {
                ImageMoneda.Source = i % 2 == 0 ? aguila : sol;
                await Task.Delay(TimeSpan.FromSeconds(i / 25.0));
            }

            (sender as Button).IsEnabled = true;
        }

        #endregion

        #region PASSWORD
        private void PasswordLengthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           try
           {
                PasswordLength.Text = PasswordLengthSlider.Value.ToString();
           }
           catch (Exception)
           {
               PasswordLength.Text = "3";
           }
        }
        
        private void PasswordButton_Click(object sender, RoutedEventArgs e)
        {
            bool Upper = (bool)CheckUpper.IsChecked;
            bool Lower = (bool)CheckLower.IsChecked;
            bool Number = (bool)CheckNumber.IsChecked;
            bool Special = (bool)CheckSpecial.IsChecked;

            if(!Upper && !Lower && !Number && !Special)
            {
                ShowMessage("Debe seleccionar al menos una opción");
                return;
            }

            System.Random random = new System.Random();

            char[] specialChars = new char[] { '?', '#', '_', '!', '.', '$', '*' };

            string strOptions = "";
            PasswordGenerated.Text = "";

            if (Upper) strOptions += "U";
            if (Lower) strOptions += "L";
            if (Number) strOptions += "N";
            if (Special) strOptions += "S";            

            for(int i = 0; i < PasswordLengthSlider.Value; i++)
            {
                var position = random.Next(0, strOptions.Length);

                if (strOptions[position] == 'U')
                {
                    int min = 'A';
                    int max = 'Z';
                    char c = ((char)(random.Next(min, max + 1)));
                    PasswordGenerated.Text += c;
                }
                else if (strOptions[position] == 'L')
                {
                    int min = 'a';
                    int max = 'z';
                    char c = ((char)(random.Next(min, max + 1)));
                    PasswordGenerated.Text += c;
                }
                else if (strOptions[position] == 'N')
                {
                    int min = '0';
                    int max = '9';
                    char c = ((char)(random.Next(min, max + 1)));
                    PasswordGenerated.Text += c;
                }
                else
                {
                    int index = random.Next(0, specialChars.Length);
                    PasswordGenerated.Text += specialChars[index];
                }
            }
        }

        private async void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordGenerated.Text.Length > 0)
            {
                bool? result = null;
                MessageDialog m = new MessageDialog("El password anterior se sobreescribirá. ¿Desea continuar?");                
                m.Commands.Add(new UICommand("Si", new UICommandInvokedHandler((cmd) => result = true)));
                m.Commands.Add(new UICommand("No", new UICommandInvokedHandler((cmd) => result = false)));                 
                await m.ShowAsync();
                if (result == true)
                {
                    CreateFile(PasswordGenerated.Text);
                }
            }
            else
            {                
                ShowMessage("Debe generar un password antes.");
            }
        }

        #endregion
       
    }
}

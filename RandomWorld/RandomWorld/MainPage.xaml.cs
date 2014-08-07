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
            RadioNumber.IsChecked = true;
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

        private string prevInput;

        private void RadioNumber_Checked(object sender, RoutedEventArgs e)
        {
            TextMinimum.Text = "1";
            TextMaximum.Text = "10";

            TextMinimum.MaxLength = 9;
            TextMaximum.MaxLength = 9;
        }

        private void RadioLetter_Checked(object sender, RoutedEventArgs e)
        {
            TextMinimum.Text = "A";
            TextMaximum.Text = "Z";

            TextMinimum.MaxLength = 1;
            TextMaximum.MaxLength = 1;
        }

        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            prevInput = (sender as TextBox).Text;
        }

        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            string value = (sender as TextBox).Text;

            if(RadioNumber.IsChecked == true)
            {
                // Si es numero, validar que sea número
                try { Int32.Parse(value);  }
                catch(Exception)
                {
                    ShowMessage("El valor ingresado no es numérico.");
                    (sender as TextBox).Text = prevInput;
                    return;
                }

                if (Int32.Parse(TextMinimum.Text) > Int32.Parse(TextMaximum.Text))
                {
                    ShowMessage("El valor mínimo debe ser menor o igual al valor máximo.");
                    (sender as TextBox).Text = prevInput;
                    return;
                }
            }
            else
            {
                if(value.ToUpper()[0] < 'A' || value.ToUpper()[0] > 'Z')
                {
                    ShowMessage("El valor ingresado no es una letra.");
                    (sender as TextBox).Text = prevInput;
                    return;
                }
                if (TextMinimum.Text.ToUpper()[0] > TextMaximum.Text.ToUpper()[0])
                {
                    ShowMessage("El valor mínimo debe ser menor o igual al valor máximo.");
                    (sender as TextBox).Text = prevInput;
                    return;
                }
            }

        }

        private void ShowMessage(string str)
        {
            MessageDialog ms = new MessageDialog(str);
            ms.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Random random = new System.Random();
            string result = null;
            
            if(RadioNumber.IsChecked == true)
            {
                result = (random.Next(Int32.Parse(TextMinimum.Text), Int32.Parse(TextMaximum.Text))).ToString();
            }
            else
            {
                int min = TextMinimum.Text.ToUpper()[0] - 'A';
                int max = TextMaximum.Text.ToUpper()[0] - 'A';
                char c = ((char)(random.Next(min, max) + 'A'));
                result = c.ToString();
            }

            DisplayedValue.Text = result;
        }
    }
}

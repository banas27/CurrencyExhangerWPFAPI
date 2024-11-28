using System.Data;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Threading.Tasks;



namespace CurrencyExhangerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root val = new Root(); // an empty Root object
        public static async Task<Root> GetData<T>(string url) // this method runs asynchronise Task, we get a result of (Root Object)
        {
            var myRoot = new Root(); // I create a new Root object to return Root object
            try
            {
                using (var client = new HttpClient()) // HttpClient class provides a base class for sending/receiving the HTTP requests/responses from a URL
                {
                    client.Timeout = TimeSpan.FromMinutes(1); // The timespan to wait before the request times out
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponseMessage is a way of returning a message/data from your action
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // Check API response status code OK
                    {
                        var ResponceString = await response.Content.ReadAsStringAsync(); // Serialize the HTTP content to a string as an asynchronous operation.
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString); // JsonConvert.DeserializedObject to deserialize Json to a C#

                        //MessageBox.Show("License: " + ResponceObject.license, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponceObject; // return API responce
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }
        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=7d2ca284adff45d4bc344a280e64c647");
            BindCurrency();
        }

        public MainWindow() // Codes starts here
        {
            InitializeComponent(); // Displays the xaml file 
            // ClearControls method is used to clear all control values
            ClearControls();
            // BindCurrency is used to bind currency name with the value in the Combobox 
            //BindCurrency();
            GetValue();
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create a variable as ConvertedValue with double data type to store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show the below message box   
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if the currency from is not selected or it is default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }
            // Check if From and To Combobox selected values are same
            if(cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amoubnt textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String to double
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);
                //Show the label converted currency and converted currency name and ToString("N3") is used to place 3 000 after the .
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                // Calculation for currency converter is From Currency value multiply (*)
                // With the amount textbox value and then that total divided (/) with To Currency value
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                // Show the label converted currency and converted currency name
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

            
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Regex code to only allow numbers 0-9 inclusive
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BindCurrency()
        {
            DataTable dt = new DataTable(); // I create a new object called DataTable 
            dt.Columns.Add("Text"); // Adding a column to the DataTable which name will be text
            dt.Columns.Add("Value"); // Adding another column of name Value to the DataTable
            // dtCurrency.Columns.Add("Banknote"); // I could add more columns to it, but I would need to add a 3rd column in Rows.Add("Text", "Value", "Banknote")
            // Adding rows in the DataTable with text and value
            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR); // First column is text the 2nd is value 
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("CZK", val.rates.CZK);

            cmbFromCurrency.ItemsSource = dt.DefaultView; // I assign dtCurrency DataTable to the drop box with the defaultview
            cmbFromCurrency.DisplayMemberPath = "Text"; // I select what should display, so the text should be displayed
            cmbFromCurrency.SelectedValuePath = "Value"; // So when the text is displayed the value path will be selected with this Line of code
            cmbFromCurrency.SelectedIndex = 0; // It determines the default displayed item and the index of it is 0

            cmbToCurrency.ItemsSource = dt.DefaultView; // I add the DataTable to my other drop box
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath= "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        private void ClearControls() // Clear the selected fields from any strings
        {
            txtCurrency.Text = string.Empty;
            if(cmbFromCurrency.Items.Count > 0 )
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if(cmbToCurrency.Items.Count > 0 )
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = string.Empty;
            txtCurrency.Focus();
        }
       
        
    }
}

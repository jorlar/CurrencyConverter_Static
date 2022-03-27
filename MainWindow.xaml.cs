using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System;

// Json Conversion
using Newtonsoft.Json;
 
 
//This library is used for DataTable
using System.Data;


namespace CurrencyConverter_Static
{
    
    public partial class MainWindow : Window
    {
        Root val = new Root();

        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        public class Rate // Make sure API return Value Names and where you want to store that name are same. 
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double EUR { get; set; }
            public double NOK { get; set; }
            public double SEK { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }

        }

        public MainWindow()
        {
            InitializeComponent();

            //ClearControls method is used to clear all control values
            ClearControls();
            GetValue();

            
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=940abe073c674ab58dd6195cd4cc1ef0"); // API ID
            //BindCurrency is used to bind currency name with the value in the Combobox
            BindCurrency();
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient()) // Provides a base class for sending/reciving requests from a URL
                {
                    client.Timeout = TimeSpan.FromMinutes(1); // Timeout await
                    HttpResponseMessage response = await client.GetAsync(url); // Responsemessage for returning a message
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // API Responded ok
                    {
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);

                        MessageBox.Show("TimeStamp: "+ResponceObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponceObject; // Return API Responce
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        #region Bind Currency From and To Combobox
        private void BindCurrency()

        {
            //Create a Datatable Object
            DataTable dtCurrency = new DataTable();

            //Add the text column in the DataTable
            dtCurrency.Columns.Add("Text");

            //Add the value column in the DataTable
            dtCurrency.Columns.Add("Value");

            //Add rows in the Datatable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("SEK", val.rates.SEK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("NOK", val.rates.NOK);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            

            //Datatable data assigned from the currency combobox
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath property is used to display data in the combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in the combobox
            cmbFromCurrency.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind the combobox to its default selected item 
            cmbFromCurrency.SelectedIndex = 0;

            //All properties are set to To Currency combobox as it is in the From Currency combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        #endregion

        #region Button Click Event

        //Convert the button click event
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create the variable as ConvertedValue with double datatype to store currency converted value
            double ConvertedValue;

            //Check if the amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After clicking on messagebox OK set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //Check if From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String To Double.
                //Textbox text have string and ConvertedValue is double Datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency converter is From Currency value multiply(*) 
                //With the amount textbox value and then that total divided(/) with To Currency value

                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        //Clear Button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method is used to clear all controls value
            ClearControls();
        }
        #endregion

        #region Extra Events

        //ClearControls method is used to clear all controls value
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
            // Add Library using System.Text.RegularExpressions;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
    }
}
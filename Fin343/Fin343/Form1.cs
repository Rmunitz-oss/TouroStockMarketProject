using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.AccessControl;

namespace Fin343
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = null;

            try
            {
                //Change this to your local machine
                //Jenn
                //sqlConnection = new SqlConnection("Server=LAPTOP-S1SI6Q9Q\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                //rochel
                sqlConnection =
                    new SqlConnection(
                        "Server=LAPTOP-GOS64JAP\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                sqlConnection.Open();

                //get user's input
                String ticker = tbTicker.Text;
                DateTime dateFrom = dtpFrom.Value;
                DateTime dateTo = dtpTo.Value;

                //call SQL stored procedure
                SqlCommand command = new SqlCommand("GetData", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@givenTicker", ticker));
                command.Parameters.Add(new SqlParameter("@dateFrom", dateFrom));
                command.Parameters.Add(new SqlParameter("@dateTo", dateTo));

                //fill dataset with results
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "DailyPnL");
                //not sure yet the best way to organize the date, leaving this here to test
                //potential idea: put in 2D array?
                DataColumn dates = dataSet.Tables[0].Columns[0];
                DataColumn quantities = dataSet.Tables[0].Columns[1];
                DataColumn transactionPrices = dataSet.Tables[0].Columns[2];
                DataColumn closingPrices = dataSet.Tables[0].Columns[3];
                DataColumn tradingPnLs = dataSet.Tables[0].Columns[4];

                DataRowCollection marketData = dataSet.Tables[0].Rows;
                foreach (DataRow row in marketData)
                {
                    Console.WriteLine(row[0]);
                    Console.WriteLine(row[1]);
                    Console.WriteLine(row[2]);
                    Console.WriteLine(row[3]);
                    Console.WriteLine(row[4]);
                }
                
                //display data to user
                
            }
            catch (Exception error)
            {
                MessageBox.Show(" " + DateTime.Now.ToLongTimeString() + error.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            finally
            {
                if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
        
                    sqlConnection.Close();
            }
        }
    }
}

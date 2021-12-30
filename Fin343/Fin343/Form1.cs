using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

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
                //sqlCon = new SqlConnection("Server=LAPTOP-S1SI6Q9Q\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                //Rochel
                sqlConnection = new SqlConnection("Server=LAPTOP-GOS64JAP\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                sqlConnnection.Open(); // open a connection to the data base specified by sqlConnection
                
               
                SqlCommand sqlCommand = new SqlCommand("write SQL queries directly here", sqlConnection);  
                //sqlCommand.CommandType = CommandType.StoredProcedure; //?

                //add parameter to query
                //example
                //get ticker user input in GUI
                String ticker = tbTicker.Text;
                SqlCommand command = new SqLCommand ("SELECT * FROM MARKETDATA WHERE Ticker = @Ticker", sqlConnection);
                command.Parameters.Add(new SqlParamater("Ticker", ticker));

                //using a sql data reader to read the data results from the command
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read()){
                    //read data into variables etc
                }
                
                /*
                sqlCommand.Parameters.Add("@Symbol", System.Data.SqlDbType.VarChar).Value = symbol;
                sqlCommand.Parameters.Add("@MinPrc", System.Data.SqlDbType.Float).Value = 0.0;
                //this does not work:
                sqlCmd.Parameters.Add("@MinDat", System.Data.SqlDbType.VarChar).Value = dtpFrom.Value.ToString();
                //sqlCmd.Parameters.Add("@MinDat", System.Data.SqlDbType.SmallDateTime).Value = dtpFrom.Value; //.ToString();
                sqlCmd.ExecuteNonQuery();
                */

                //rather than reading line by line: fill a DataSet
                //example:
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataset = new DataSet();
                String srcTable = "MarketData";
                dataAdapter.Fill(dataset, srcTable);

                //fill chart on gui based on dataset 
                chrtPrices.DataSource = dataset.Tables[0];
                var table = dataset.Tables[0];
                int nrPoints = table.Rows.Count;
                for (int ii = 0; ii < nrPoints; ++ii)
                {
                    chrtPrices.Series[0].Points.AddXY(table.Rows[ii][0], table.Rows[ii][1]);
                }
                Form1.ActiveForm.Text = symbol + " Closing Prices";
                dgvDump.DataSource = table;
            }

            catch (Exception ex)

            {
                MessageBox.Show(" " + DateTime.Now.ToLongTimeString() + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally

            {

                if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)

                    sqlConnection.Close();

            }

        }
    }
}

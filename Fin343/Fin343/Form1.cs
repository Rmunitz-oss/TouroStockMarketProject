using System;
using System.Collections;
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
                sqlConnection = new SqlConnection("Server=LAPTOP-S1SI6Q9Q\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                //rochel
                /*sqlConnection =
                    new SqlConnection(
                        "Server=LAPTOP-GOS64JAP\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");*/
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
               
                //create a table?
                //DataTable marketDataTable = new DataTable();
                //marketDataTable.Columns.Add("Date", typeof(DateTime));
                //marketDataTable.Columns.Add("TradeQuantities");
                
                
                DataColumn dates = dataSet.Tables[0].Columns[0];
                DataColumn tradeQuantities = dataSet.Tables[0].Columns[1];
                DataColumn transactionPrices = dataSet.Tables[0].Columns[2];
                DataColumn closingPrices = dataSet.Tables[0].Columns[3];
                DataColumn tradingPnLs = dataSet.Tables[0].Columns[4];
                DataColumn previousColsingPrice = dataSet.Tables[0].Columns[5];
              
                
                DataRowCollection marketDataRows = dataSet.Tables[0].Rows;
                foreach(var item in marketDataRows[0].ItemArray)
                {
                    Console.Write(item + ", ");
                }
                Console.WriteLine();

                //calculations
                int nrRows = marketDataRows.Count;
                Console.WriteLine(nrRows);
                Console.WriteLine("-------------");
                
                //b. total quantity = trade quantity + prior total quantity
                decimal totalQuantity = 0;
                decimal [] totalQuantities = new decimal [nrRows];
                //first row, totalQuantity = trade quantity

                //DataRow firstRow = marketDataRows[0];
                //totalQuantity += (int) firstRow[1]; //change decimal to int
                //totalQuantities[0] = totalQuantity;

                int index = 0;
                foreach (DataRow row in marketDataRows)
                {
                    totalQuantity += (int)row[1]; //change decimal to int
                    totalQuantities[index] = totalQuantity;
                    index++; 
                }
                foreach(var item in totalQuantities)
                {
                    Console.Write(item + ", ");
                }
                Console.WriteLine("-------------");

                //c. position pnl = quantity (current closing price - prior closing price)
                decimal[] positionPnLs = new decimal[nrRows];
                //first row (has no previous row) always zero (?)
                //positionPnLs[0] = 0;
                for (int ix = 0; ix < nrRows; ix++)
                {
                    DataRow row = marketDataRows[ix];
                    //DataRow previousRow = marketDataRows[ix - 1];
                    decimal quantity = (int) row[1];
                    if (quantity < 0)
                    {
                        quantity = totalQuantities[ix]; 
                    }
                    decimal closingPrice = (decimal) row[3];
                    decimal previousClosingPrice = row.IsNull("PreviousClosingPrice") ? (decimal) row[3] : (decimal) row[5];
                    decimal positionPnL = quantity * (closingPrice - previousClosingPrice);
                    positionPnLs[ix] = positionPnL;
                }
                foreach (var item in positionPnLs)
                {
                    Console.Write(item + ", ");
                }
                Console.WriteLine("-------------");

                //d. total daily pnl = trading pnl + position pnl
                decimal[] totalDailyPnLs = new decimal[nrRows];
                for (int ix = 0; ix < nrRows; ix++)
                {
                    DataRow row = marketDataRows[ix];
                    decimal tradingPnl = (decimal) row[4];
                    totalDailyPnLs[ix] = tradingPnl + positionPnLs[ix];
                }
                foreach (var item in totalDailyPnLs)
                {
                    Console.Write(item + ", ");
                }
                Console.WriteLine("-------------");

                //e. cumulative pnl = prior cumulative + total daily pnl
                //TODO ADD CHECKER IF ONLY ONE ROW
                decimal[] cumulativePnLs = new decimal[nrRows];
                cumulativePnLs[0] = totalDailyPnLs[0];
                for (int ix = 1; ix < nrRows; ix++)
                {
                    cumulativePnLs[ix] = cumulativePnLs[ix - 1] + totalDailyPnLs[ix];
                }
                foreach (var item in cumulativePnLs)
                {
                    Console.Write(item + ", ");
                }
                Console.WriteLine("-------------");



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

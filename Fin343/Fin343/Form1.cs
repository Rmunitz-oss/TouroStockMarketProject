using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.AccessControl;
using System.Windows.Forms.DataVisualization.Charting;

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
                
                DataColumn dates = dataSet.Tables[0].Columns[0];
                DataColumn tradeQuantities = dataSet.Tables[0].Columns[1];
                DataColumn transactionPrices = dataSet.Tables[0].Columns[2];
                DataColumn closingPrices = dataSet.Tables[0].Columns[3];
                DataColumn tradingPnLs = dataSet.Tables[0].Columns[4];
                DataColumn previousClosingPrices = dataSet.Tables[0].Columns[5];
                
                DataRowCollection marketDataRows = dataSet.Tables[0].Rows;
               

                //calculations
                int nrRows = marketDataRows.Count;
                
                //b. total quantity = trade quantity + prior total quantity
                Int32 totalQuantity = 0;
                Int32 [] totalQuantities = new Int32 [nrRows];
                //first row, totalQuantity = trade quantity
                
                /* This makes the first total quantity is doubled
                DataRow firstRow = marketDataRows[0];
                totalQuantity += Convert.ToInt32(firstRow[1]);
                totalQuantities[0] = totalQuantity;*/

                int index = 0;
                foreach (DataRow row in marketDataRows)
                {
                    totalQuantity += Convert.ToInt32(row[1]); 
                    totalQuantities[index] = totalQuantity;
                    index++; 
                }
             

                //c. position pnl = quantity (current closing price - prior closing price)
                decimal [] positionPnLs = new decimal[nrRows];
                for (int ix = 0; ix < nrRows; ix++)
                {
                    DataRow row = marketDataRows[ix];
                    decimal quantity = Convert.ToDecimal(row[1]);
                    if (quantity < 0)
                    {
                        quantity = totalQuantities[ix]; 
                    }
                    decimal closingPrice = Convert.ToDecimal(row[3]);
                    decimal previousClosingPrice = row.IsNull("PreviousClosingPrice") ? Convert.ToDecimal(row[3]) :  Convert.ToDecimal(row[5]);
                    decimal positionPnL = quantity * (closingPrice - previousClosingPrice);
                    positionPnLs[ix] = positionPnL;
                }
              

                //d. total daily pnl = trading pnl + position pnl
                decimal [] totalDailyPnLs = new decimal[nrRows];
                for (int ix = 0; ix < nrRows; ix++)
                {
                    DataRow row = marketDataRows[ix];
                    decimal tradingPnl = Convert.ToDecimal(row[4]);
                    totalDailyPnLs[ix] = tradingPnl + positionPnLs[ix];
                }

                //e. cumulative pnl = prior cumulative + total daily pnl
                decimal [] cumulativePnLs = new decimal[nrRows];
                cumulativePnLs[0] = totalDailyPnLs[0];
                if (nrRows > 1)
                {
                    for (int ix = 1; ix < nrRows; ix++)
                    {
                        cumulativePnLs[ix] = cumulativePnLs[ix - 1] + totalDailyPnLs[ix];
                    }
                    
                }
              

                //display data to user
                /*
                Series series = chart.Series.Add("PnL");
                DateTime[] datesChart =
                {
                    Convert.ToDateTime(dates)
                };
                series.ChartType = SeriesChartType.Spline;
                for (int ix = 0; ix < totalDailyPnLs.Length; ix++)
                {
                    series.Points.AddXY(datesChart[ix], totalDailyPnLs[ix]);
                }
                */
                

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

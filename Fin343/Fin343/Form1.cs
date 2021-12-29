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
            SqlConnection sqlCon = null;

            try
            {
                //Change this to your local machine
                sqlCon = new SqlConnection("Server=LAPTOP-S1SI6Q9Q\\SQLEXPRESS;Database=StockMarketProject;Trusted_Connection=True;");
                sqlCon.Open(); // open a connection to the data base specified by sqlCon
                String symbol = tbTicker.Text;

                SqlCommand sqlCmd = new SqlCommand("spGetPrcForSymbol", sqlCon);  
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@Symbol", System.Data.SqlDbType.VarChar).Value = symbol;
                sqlCmd.Parameters.Add("@MinPrc", System.Data.SqlDbType.Float).Value = 0.0;
                //this does not work:
                sqlCmd.Parameters.Add("@MinDat", System.Data.SqlDbType.VarChar).Value = dtpFrom.Value.ToString();
                //sqlCmd.Parameters.Add("@MinDat", System.Data.SqlDbType.SmallDateTime).Value = dtpFrom.Value; //.ToString();
                sqlCmd.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                DataSet dataset = new DataSet();
                da.Fill(dataset, "prices");
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

                if (sqlCon != null && sqlCon.State == System.Data.ConnectionState.Open)

                    sqlCon.Close();

            }

        }
    }
}

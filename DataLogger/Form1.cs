using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.UaFx.Client;
using Microsoft.Data.SqlClient;

namespace DataLogger
{
    public partial class Form1 : Form
    {
        double Rtag1 = 0;
        double Rtag2 = 0;
        int id = 1;
        string connectionString = "Data Source=DESKTOP-970G76V\\SQLEXPRESS;Initial Catalog=ScadaLAB;Integrated Security=True; TrustServerCertificate=Yes;";

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadOPC();
            WriteToDatabase(1, Math.Round(Rtag1,2));  //writing control variable u to tag1
            WriteToDatabase(2, Math.Round(Rtag2,2));  //writing process value x to tag2
        }

        private void ReadOPC()
        {
            string opcUrl = "opc.tcp://localhost:62640/";
            var tagName1 = "ns=2;s=Tag7";   //Read u
            var tagName2 = "ns=2;s=Tag8";   //read x

            var client = new OpcClient(opcUrl);
            client.Connect();
            var tag1 = client.ReadNode(tagName1);
           var tag2 = client.ReadNode(tagName2);
            Rtag1 = Convert.ToDouble(tag1.Value);
            Rtag2 = Convert.ToDouble(tag2.Value);
            client.Disconnect();
        }

        public void WriteToDatabase(int id, double var1)
        {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("SaveTagData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TagId", id));
                cmd.Parameters.Add(new SqlParameter("@TagValue", var1));
                cmd.ExecuteNonQuery();
                con.Close();
        }
        
    }
}
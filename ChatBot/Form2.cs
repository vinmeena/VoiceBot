using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhatsAppApi;

namespace ChatBot
{
    public partial class Form2 : Form
    {
       

        public Form2()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string From = "918824518051";
            string To = textBox1.Text;
            string msg = textBox2.Text;
            //Whatsapp Class
            WhatsApp wa = new WhatsApp(From, "", "Vin", false, false);
            wa.OnConnectSuccess += () =>
             {
                 MessageBox.Show("Connected to WhatsApp..........");
         
                     wa.OnLoginSuccess += (phoneNumber,data) =>
                      {
                          MessageBox.Show("Login SuccessFully!!");
                          wa.SendMessage(To, msg);
                          MessageBox.Show("Message Sent!");
                      };
                     wa.OnLoginFailed += (data) =>
                       {
                           MessageBox.Show("Login Failed :{0}", data);
                       };
                     wa.Login();
             };
            wa.OnConnectFailed += (ex) =>
              {
                  MessageBox.Show("Connection Failed!");
              };
            wa.Connect();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

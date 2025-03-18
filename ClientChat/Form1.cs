using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientChat
{
    public partial class Form1 : Form
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string Username { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIP.Text) || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter both IP address and username.");
                return;
            }
            ServerIP = txtIP.Text;
            Username = txtUsername.Text;
            ServerPort = int.TryParse(txtPort.Text.Trim(), out int port) ? port : 5000;
            ChatForm chatForm = new ChatForm(ServerIP, ServerPort, Username);
            chatForm.Show();
            this.Hide();
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers; 

namespace ClientChat
{
    public partial class ChatForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private string username;
        private string serverIP;
        private int serverPort;
        private bool isTyping = false;
        private System.Timers.Timer typingTimer;
        private bool isRunning = true;

        public ChatForm(string _serverIP, int _serverPort, string _username)
        {
            InitializeComponent();
            this.serverIP = _serverIP;
            this.serverPort = _serverPort;
            this.username = _username;
            MovesForm.Attach(panel1, this);
            MovesForm.Attach(panel4, this);
            ConnectToServer();

            label2.Text = $"{username}";

            // Initialize typingTimer here
            typingTimer = new System.Timers.Timer(1000); 
            typingTimer.Elapsed += OnTypingTimerElapsed;
            typingTimer.AutoReset = false;
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();

               
                byte[] nameBuffer = Encoding.UTF8.GetBytes(username);
                stream.Write(nameBuffer, 0, nameBuffer.Length);

                // Start receiving messages in the background
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect: " + ex.Message);
                this.Close(); 
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (isRunning)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    // Ensure handle is created before invoking
                    if (this.IsHandleCreated)
                    {
                        this.Invoke(new Action(() => ProcessMessage(message)));
                    }
                }
                catch (Exception ex)
                {
                    if(!isRunning) return; 
                    break;
                }
            }
        }



        private void ProcessMessage(string message)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;

            if (message.StartsWith("/users"))
            {
                lstUsers.Items.Clear();
                string[] users = message.Substring(7).Split(',');
                foreach (string user in users)
                {
                    lstUsers.Items.Add(user);
                }
            }
            else if (message.StartsWith("/typing"))
            {
                lblTyping.Text = $"{message.Substring(8)} is typing...";
                typingTimer.Stop();
                typingTimer.Start();
            }
            else
            {
                richTextBox1.AppendText(message + Environment.NewLine);
            }
        }


        private void SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
            stream.Write(msgBuffer, 0, msgBuffer.Length);
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*try
            {
                if (client != null)
                {
                    client.Close();
                }
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    receiveThread.Abort(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during cleanup: " + ex.Message);
            }*/

            isRunning = false;
            client?.Close();
            stream?.Close();
        }
        


        // Event handler for when typing stops (after 1 second of inactivity)
        private void OnTypingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Handle logic when the user stops typing
            isTyping = false;
            lblTyping.Text = ""; // Clear the typing label on timeout
        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            SendMessage(txtMessage.Text);
            txtMessage.Clear();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            if (!isTyping)
            {
                isTyping = true;
                byte[] typingBuffer = Encoding.UTF8.GetBytes("/typing");
                stream.Write(typingBuffer, 0, typingBuffer.Length);
            }

            // Reset and start the typing timer only if it's not already running
            if (!typingTimer.Enabled)
            {
                typingTimer.Start(); // Start the timer when typing starts
            }
            else
            {
                typingTimer.Stop(); // Stop the timer if the user types again before timeout
                typingTimer.Start(); // Restart the timer
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the current form
            Application.Exit();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Check if Enter key is pressed
            {
                e.SuppressKeyPress = true; 
                btnSend.PerformClick(); // Simulates a button click
            }
        }
    }
}

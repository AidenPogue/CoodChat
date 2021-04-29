using System;
using System.Windows.Forms;

namespace CoodChat
{
    public partial class NetworkPopupForm : Form
    {
        public NetworkPopupForm()
        {
            InitializeComponent();
        }

        public static void ShowAsPopup()
        {
            new NetworkPopupForm().ShowDialog();
        }

        private void hostButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hostPortTextBox.Text, out int port))
            {
                NetworkManager.Host(port);
                Close();
            }

        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            var splitAddress = joinIPTextBox.Text.Split(':');
            if (splitAddress.Length == 2 && int.TryParse(splitAddress[1], out int port))
            {
                if (NetworkManager.TryConnect(splitAddress[0], port, usernameBox.Text)) Close();
            }
            else
            {
                MessageBox.Show("Inavlid IP or port.", "Error");
            }
        }
    }
}

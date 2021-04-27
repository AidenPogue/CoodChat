using System;
using System.Windows.Forms;

namespace CoodChat
{
    public partial class InputWindow : Form
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        private void InputWindow_Load(object sender, EventArgs e)
        {
            Console.Title = "CoodChat Console";
            new NetworkPopupForm().ShowDialog();
        }

        private void SendButton(object sender, EventArgs e)
        {
            SendCode();   
        }

        private void SendCode()
        {
            string code = codeInputBox.Text;
            string entryPoint = entryPointTextBox.Text;
            if (AssemblyManager.TryBuildAndExecuteAssembly(code, entryPoint))
            {
                NetworkManager.SendSource(code, entryPoint);
            }
            else
            {
                Console.WriteLine("Source will not be sent due to errors.");
            }
        }

        private void InputWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

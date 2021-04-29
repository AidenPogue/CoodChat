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
            NetworkPopupForm.ShowAsPopup();
        }

        private void SendCode(object sender, EventArgs e)
        {
            string code = codeInputBox.Text;
            string entryPoint = entryPointTextBox.Text;
            if (AssemblyManager.TryBuildAndExecuteAssembly(code, entryPoint))
            {
                NetworkManager.SendToServer($"{entryPoint}|{code}", "c");
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

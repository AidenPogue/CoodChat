using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTest
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
            if (AssemblyManager.TryBuildAssembly(code, out byte[] bytes, out EmitResult result))
            {
                var asm = Assembly.Load(bytes);
                AssemblyManager.ExecuteAssembly(asm, entryPointTextBox.Text);
                NetworkManager.TrySendAssembly(bytes);
            }
            else
            {
                Console.WriteLine("Compiler Error:\n");
                foreach(Diagnostic diagnostic in result.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void codeInputBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

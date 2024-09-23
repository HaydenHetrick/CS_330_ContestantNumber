using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace TelephoneKeypad
{
    public partial class KeypadForm : Form
    {
        string[] values;
        List<string> valueList = new List<string>();
        Random random = new Random();
        Label initialClick = null;
        string passwordAutoFill = "";
        string passwordCheck = "";
        int tempCounter = 0;

        // Begins program
        public KeypadForm()
        {
            InitializeComponent();
            ListReader();
            AssignKeysToButtons();
        }

        // Reads comma separated text file and imports as array, converts to List
        public void ListReader()
        {
            using (StreamReader myReader = new StreamReader(@"C:\Users\Ryan Patton\OneDrive\FISD\BPA\C Sharp RWP\letter.txt"))
            {
                string line = myReader.ReadToEnd();
                values = line.Split(',');
            }

            foreach (string a in values)
            {
                valueList.Add(a);
            }
        }

        // Resets the entire contents. Note: we keep it simple by just reading from the text file again
        private void ResetVariables()
        {
            ListReader();
            passwordAutoFill = "";
            passwordCheck = "";
            tempCounter = 0;
            textBox1.Text = "";
            textBox2.Text = "";
        }

        // Assigns the key values a random value from imported list and removes it
        // Only one value can appear, no duplicates
        private async void AssignKeysToButtons()
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label keyLabel = control as Label;
                if (keyLabel != null)
                {
                    int randomNumber = random.Next(valueList.Count);
                    keyLabel.Text = valueList[randomNumber];
                    tempCounter++;

                    // This is how we get our random password
                    if (tempCounter % 2 == 0)
                        passwordAutoFill += keyLabel.Text;

                    await Task.Delay(100);
                    keyLabel.ForeColor = Color.Red;
                    valueList.RemoveAt(randomNumber);
                }
            }

            textBox1.Text = passwordAutoFill;
        }

        // Controls the clicking of the keypad
        private void KeypadClick(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                return;

            System.Media.SystemSounds.Exclamation.Play();
            Label clickedKey = sender as Label;

            passwordCheck += clickedKey.Text;
            textBox2.Text = passwordCheck;

            if (clickedKey != null)
            {
                if (clickedKey.ForeColor == Color.Black)
                    return;

                if (initialClick == null)
                {
                    initialClick = clickedKey;
                    initialClick.ForeColor = Color.Black;
                    timer1.Start();
                    return;
                }
            }
        }

        // Timer is used to control the display
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            initialClick.ForeColor = Color.Red;
            initialClick = null;
        }

        // Checks the password. WARNING: the password is only checked based upon the label click events.
        // Manual entry of the password is not handled with this program.
        private void CheckPassword(object sender, EventArgs e)
        {
            if (passwordCheck.CompareTo(passwordAutoFill) == 0)
            {
                MessageBox.Show("Your passwords match!", "Congratulations on the security keypad prototype!");
                DialogResult responseYN = MessageBox.Show("Do you want to try again?", "Yes or No?", MessageBoxButtons.YesNo);

                if (responseYN == DialogResult.Yes)
                {
                    ResetVariables();
                    AssignKeysToButtons();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("INCORRECT", "Passwords do not match.");
                DialogResult responseYN = MessageBox.Show("Do you want to try again?", "Yes or No?", MessageBoxButtons.YesNo);

                if (responseYN == DialogResult.Yes)
                {
                    ResetVariables();
                    AssignKeysToButtons();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}

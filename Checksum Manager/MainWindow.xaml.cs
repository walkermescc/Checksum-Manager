using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checksum_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                txtBrowse.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void btnChecksum_Click(object sender, RoutedEventArgs e)
        {
            // Get the folder path from the text field and output a message if folder doesn't return a result.
            string folderPath = txtBrowse.Text;
            if (!Directory.Exists(folderPath))
            {
                System.Windows.Forms.MessageBox.Show("Folder path does not exist!");
                return;
            }
            // Get the total number of files in the directory
            int fileCount = Directory.GetFiles(folderPath).Length;

            // Set the maximum value
            barLoading.Maximum = fileCount;

            // Reset the value of the progress bar
            barLoading.Value = 0;

            StringBuilder sb = new StringBuilder();
            using (var sha128 = chkSHA128.IsChecked.GetValueOrDefault() ? SHA1.Create() : null)
            using (var sha256 = chkSHA256.IsChecked.GetValueOrDefault() ? SHA256.Create() : null)
            using (var sha384 = chkSHA384.IsChecked.GetValueOrDefault() ? SHA384.Create() : null)
            using (var sha512 = chkSHA512.IsChecked.GetValueOrDefault() ? SHA512.Create() : null)
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        if (sha128 != null)
                        {
                            byte[] hashBytes = sha128.ComputeHash(fs);
                            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                            sb.AppendLine($"SHA-128: {hashString}  {filePath}");
                            txtSHABox.Text = sb.ToString();

                            // Update the progress bar
                            barLoading.Value++;
                            await Task.Delay(100); // Wait for a short delay to see the progress
                        }
                        if (sha256 != null)
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            byte[] hashBytes = sha256.ComputeHash(fs);
                            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                            sb.AppendLine($"SHA-256: {hashString}  {filePath}");
                            txtSHABox.Text = sb.ToString();

                            // Update the progress bar
                            barLoading.Value++;
                            await Task.Delay(100); // Wait for a short delay to see the progress
                        }
                        if (sha384 != null)
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            byte[] hashBytes = sha384.ComputeHash(fs);
                            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                            sb.AppendLine($"SHA-384: {hashString}  {filePath}");
                            txtSHABox.Text = sb.ToString();

                            // Update the progress bar
                            barLoading.Value++;
                            await Task.Delay(100); // Wait for a short delay to see the progress
                        }
                        if (sha512 != null)
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            byte[] hashBytes = sha512.ComputeHash(fs);
                            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                            sb.AppendLine($"SHA-512: {hashString}  {filePath}");
                            txtSHABox.Text = sb.ToString();

                            // Update the progress bar
                            barLoading.Value++;
                            await Task.Delay(100); // Wait for a short delay to see the progress
                        }
                    }
                }
            }
        }

        private void btnSaveChecksum_Click(object sender, RoutedEventArgs e)
        {
            {
                // Get the output string and check if it's empty
                string output = txtSHABox.Text;
                if (string.IsNullOrWhiteSpace(output))
                {
                    System.Windows.Forms.MessageBox.Show("Output is empty!");
                    return;
                }

                // Create the file name with the specified format
                string fileName = "ChecksumReport-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt";

                // Create the file path by appending the file name to the selected folder path
                string filePath = System.IO.Path.Combine(txtBrowse.Text, fileName);

                // Create the file and write the output to it
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the title and date/time
                    writer.WriteLine("Checksum Report");
                    writer.WriteLine(DateTime.Now.ToString("dddd, MMMM d, yyyy"));
                    writer.WriteLine(DateTime.Now.ToString("h:mm tt"));

                    // Write the folder path and user
                    writer.WriteLine("Folder Path: " + txtBrowse.Text);
                    writer.WriteLine("User: " + Environment.UserName);

                    // Write a line divider
                    writer.WriteLine(new string('-', 50));

                    // Write the output
                    writer.WriteLine(output);
                }

                System.Windows.Forms.MessageBox.Show("Report saved successfully!");
            }

        }
    }
}
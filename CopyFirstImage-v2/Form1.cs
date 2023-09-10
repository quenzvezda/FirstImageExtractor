using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using System.IO;

namespace CopyFirstImage_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser())
            {
                // Load the last folder from the settings
                string lastFolder = Properties.Settings.Default.LastFolder;
                betterFolderBrowser.RootFolder = string.IsNullOrEmpty(lastFolder) ? "D:\\" : lastFolder;

                betterFolderBrowser.Title = "Select Folders";
                betterFolderBrowser.Multiselect = true;

                if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string[] selectedFolders = betterFolderBrowser.SelectedFolders;
                    listBox1.Items.Clear();
                    foreach (string folder in selectedFolders)
                    {
                        listBox1.Items.Add(folder); // Simpan path lengkap dari folder
                    }
                    label1.Text = $"Selected folders: {listBox1.Items.Count}";
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Asumsi listBoxFolders berisi path lengkap dari setiap folder yang dipilih
            foreach (var item in listBox1.Items)
            {
                string fullFolderPath = item.ToString(); // Path lengkap folder
                string parentPath = Directory.GetParent(fullFolderPath).FullName; // Mendapatkan direktori induk

                try
                {
                    string[] files = Directory.GetFiles(fullFolderPath, "*.jpg"); // Mengambil semua file jpg di folder

                    if (files.Length > 0)
                    {
                        Array.Sort(files); // Mengurutkan array agar file pertama adalah yang akan dikopi
                        string firstFile = files[0];
                        string destFile = Path.Combine(parentPath, Path.GetFileName(firstFile));

                        File.Copy(firstFile, destFile, true); // Mengkopi file
                    }
                }
                catch (Exception ex)
                {
                    // Menangani error, misalnya dengan menampilkan pesan
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            // Set label text to "Extract complete" and clear the listbox
            label1.Text = "Extract complete";

            if (listBox1.Items.Count > 0)
            {
                string lastSelectedFolder = listBox1.Items[0].ToString();
                string parentPath = Directory.GetParent(lastSelectedFolder).FullName;
                Properties.Settings.Default.LastFolder = parentPath;
                Properties.Settings.Default.Save();
            }

            listBox1.Items.Clear();
        }
    }
}

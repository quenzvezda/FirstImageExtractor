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
                string lastFolder = FirstImageExtractor.Properties.Settings.Default.LastFolder;
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

                    // Menyimpan folder terakhir yang diakses
                    FirstImageExtractor.Properties.Settings.Default.LastFolder = betterFolderBrowser.RootFolder;
                    FirstImageExtractor.Properties.Settings.Default.Save();
                }
            }
            listBox1.ForeColor = Color.Black;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> failedFolders = new List<string>();
            // Asumsi listBoxFolders berisi path lengkap dari setiap folder yang dipilih
            foreach (var item in listBox1.Items)
            {
                string fullFolderPath = item.ToString(); // Path lengkap folder
                string parentPath = Directory.GetParent(fullFolderPath).FullName; // Mendapatkan direktori induk

                try
                {
                    // string[] files = Directory.GetFiles(fullFolderPath, "*.jpg"); // Mengambil semua file jpg di folder
                    string[] imageFiles = Directory.GetFiles(fullFolderPath, "*.*")
                               .Where(file => file.ToLower().EndsWith("jpg") ||
                                              file.ToLower().EndsWith("png") ||
                                              file.ToLower().EndsWith("webp") ||
                                              file.ToLower().EndsWith("gif"))
                               .ToArray();

                    string[] videoFiles = Directory.GetFiles(fullFolderPath, "*.*")
                                                   .Where(file => file.ToLower().EndsWith("mp4") ||
                                                                  file.ToLower().EndsWith("mkv"))
                                                   .ToArray();

                    if (imageFiles.Length > 0)
                    {
                        Array.Sort(imageFiles);
                        string firstFile = imageFiles[0];
                        string destFile = Path.Combine(parentPath, Path.GetFileName(firstFile));
                        File.Copy(firstFile, destFile, true);
                    }
                    else if (videoFiles.Length > 0)
                    {
                        Array.Sort(videoFiles);
                        string firstFile = videoFiles[0];
                        string destFile = Path.Combine(parentPath, Path.GetFileName(firstFile));
                        File.Copy(firstFile, destFile, true);
                    }
                    else
                    {
                        // Tidak ada media di folder ini, tambahkan ke list gagal
                        failedFolders.Add(fullFolderPath);
                    }
                }
                catch (Exception ex)
                {
                    // Menangani error, misalnya dengan menampilkan pesan
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

            if (failedFolders.Count > 0)
            {
                // Update ListBox dan Label
                listBox1.Items.Clear();
                listBox1.ForeColor = Color.Red;
                foreach (var folder in failedFolders)
                {
                    listBox1.Items.Add($"{Path.GetFileName(folder)} (no image found in this folder)");
                }
                label1.Text = $"{failedFolders.Count} folders failed to extract";
            }
            else
            {
                // Semua folder berhasil, kosongkan ListBox
                listBox1.Items.Clear();
                label1.Text = "Extract complete";
            }
        }
    }
}

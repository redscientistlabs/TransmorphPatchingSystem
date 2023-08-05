using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransmorphPatchingSystem
{
    public partial class Generator : Form
    {
        public Generator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string input;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Original file";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    input = openFileDialog.FileName;
                }
                else
                    return;
            }

            Operator.LoadInput(input);
            Operator.ProbeInput();

            string output;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Destination File";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    output = openFileDialog.FileName;
                }
                else
                    return;
            }

            Operator.LoadOutput(output);
            Operator.ProbeOutput();


            string tesseract;
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                saveFileDialog1.Title = "Save Tesseract File (TPS)";
                saveFileDialog1.Filter = "TPS files (*.tps)|*.tps";
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    tesseract = saveFileDialog1.FileName;
                }
                else
                {
                    return;
                }
            }

            if (File.Exists(tesseract))
                File.Delete(tesseract);

            Tesseract TPS = Operator.GenerateTesseract();
            Tesseract.SaveTesseract(TPS, tesseract);

            MessageBox.Show("Tesseract Patch has been successfully created");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string tesseract = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Tesseract File";
                openFileDialog.Filter = "TPS files (*.tps)|*.tps";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    tesseract = openFileDialog.FileName;
                }
                else
                    return;
            }

            string input;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Original File";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    input = openFileDialog.FileName;
                }
                else
                    return;
            }

            Operator.LoadInput(input);
            Operator.ProbeInput();

            Tesseract TPS = Tesseract.LoadTesseract(tesseract);

            string output = null;

            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                saveFileDialog1.Title = "Save Patched File";
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = TPS.targetFilename;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    output = saveFileDialog1.FileName;
                }
                else
                {
                    return;
                }
            }

            Operator.ProcessTesseract(TPS, output);

            MessageBox.Show("File was patched and saved successfully");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                @"About this program
---------------------------------------------------
Tesseract Patching System is an universal patching system for Windows X64.

While the program is clearly inspired by Lunar IPS, it fundamentally functions much much differently.

This program generates a Tesseract (TPS File) from an Input file and an Output file. The resulting object contains 2 tables of pointers, one of which points to Raw data from the input file and the other which also points to Raw data but introduces a differential value.
The resulting Tesseract will most likely be bigger in size than the expected Output file. This is normal expected behavior as the transformation operations are bigger than the Output file itself.

Because of the nature of the data contained inside the tesseract, ABSOLUTELY NO DATA FROM THE ORIGINAL FILE IS CONTAINED IN THE PATCH. THE TRANSFORMATION 100% DEPENDS ON THE INPUT FILE'S INTEGRITY.

This program is freeware and is provided AS-IS with no warranty. The author cannot be held for damages of any kind arising from its use or presence.

Version 1.00

Written by Phil Girard
2022 - Redscientist Labs
");
        }
    }
}

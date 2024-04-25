using Microsoft.VisualBasic;
using System;
using System.DirectoryServices;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.IO.Ports;

namespace ArtNet
{
    public partial class Form1 : Form
    {
        private Button[] uniButtonArray = new Button[32];
        private Button[] presetButtonArray = new Button[6];
        private int prevUni = 0;
        private int prevPreset = 0;
        NumericUpDown[] addressArray;
        private int activeUniverse = 1;
        private int activePreset = 1;
        private bool universeinPresets = false;
        private int[] validUni;
        private String mbedDrive = "E";
        private SerialPort serialPort;
        private String comPort = "COM4";
        public Form1()
        {
            InitializeComponent();
            const int numControls = 512;
            addressArray = new NumericUpDown[numControls];
            System.Windows.Forms.Label[] labelArray = new System.Windows.Forms.Label[numControls];

            int startX = 0;
            int startY = 100;
            int labelWidth = 50;
            int controlWidth = 60;
            int spacing = 2;
            int verticalGap = 30;

            for (int i = 0; i < 6; ++i)
            {
                presetButtonArray[i] = new Button();
                presetButtonArray[i].Width = 42;
                presetButtonArray[i].Height = 30;
                presetButtonArray[i].Text = "" + (i + 1);
                presetButtonArray[i].BackColor = Color.Red;
                presetButtonArray[i].Location = new Point(242 + (i * 55), 14);
                presetButtonArray[i].Click += presetButton_Click;
                Controls.Add(presetButtonArray[i]);
            }
            presetButtonArray[activePreset - 1].BackColor = Color.LawnGreen;

            for (int i = 0; i < uniButtonArray.Length; i++)
            {
                uniButtonArray[i] = new Button();
                uniButtonArray[i].Width = 42;
                uniButtonArray[i].Height = 30;
                uniButtonArray[i].Text = "" + (i + 1);
                uniButtonArray[i].BackColor = Color.Gray;
                uniButtonArray[i].Location = new Point(20 + (i * 55), 50);
                uniButtonArray[i].Click += uniButton_Click;
                Controls.Add(uniButtonArray[i]);
            }

            for (int i = 0; i < numControls; i++)
            {
                labelArray[i] = new System.Windows.Forms.Label();
                labelArray[i].Text = "" + (i + 1);
                labelArray[i].Location = new Point(startX + (i / 32) * (labelWidth + controlWidth + spacing), startY + (i % 32) * verticalGap);
                labelArray[i].Width = labelWidth;
                labelArray[i].TextAlign = System.Drawing.ContentAlignment.MiddleRight;


                addressArray[i] = new NumericUpDown();
                addressArray[i].Minimum = 0;
                addressArray[i].Maximum = 255; // You can change the maximum value as needed
                addressArray[i].Width = controlWidth;
                addressArray[i].Location = new Point(startX + labelWidth + spacing + (i / 32) * (labelWidth + controlWidth + spacing), startY + (i % 32) * verticalGap);
                addressArray[i].Hexadecimal = true;
                Controls.Add(labelArray[i]);
                Controls.Add(addressArray[i]);


            }

            FileStream fs = new FileStream(mbedDrive + ":\\art0.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader sr = new BinaryReader(fs);
            byte[] buffer = new byte[530];
            while (true)
            {
                if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                {
                    int tempUniverse = ((int)buffer[15] << 8) | (int)buffer[14];
                    if (validUni == null)
                    {
                        validUni = new int[1] { tempUniverse };
                        uniButtonArray[tempUniverse - 1].BackColor = Color.LawnGreen;
                        activeUniverse = tempUniverse;
                        prevUni = activeUniverse - 1;
                    }
                    else
                    {
                        Array.Resize(ref validUni, validUni.Length + 1);
                        validUni[validUni.Length - 1] = tempUniverse;
                        uniButtonArray[tempUniverse - 1].BackColor = Color.Red;
                    }
                }
                else
                {
                    break;
                }
            }
            if (validUni == null)
            {
                universeinPresets = false;
                button3.Text = "Add Universe to Presets";
                uniButtonArray[0].BackColor = Color.LawnGreen;
            }
            sr.Close();
        }

        private void presetButton_Click(object sender, EventArgs e)
        {
            if ((prevPreset + 1) != Int32.Parse((sender as Button).Text))
            {
                activePreset = Int32.Parse((sender as Button).Text);
                (sender as Button).BackColor = Color.LawnGreen;
                presetButtonArray[prevPreset].BackColor = Color.Red;
                for (int i = 0; i < 6; ++i)
                {
                    if ((sender as Button).Text == presetButtonArray[i].Text)
                    {
                        prevPreset = i;
                        break;
                    }
                }

            }
        }
        private void uniButton_Click(object sender, EventArgs e)
        {
            if ((prevUni + 1) != Int32.Parse((sender as Button).Text))
            {
                activeUniverse = Int32.Parse((sender as Button).Text);
                (sender as Button).BackColor = Color.LawnGreen;
                if (validUni != null && validUni.Contains(prevUni + 1))
                {
                    uniButtonArray[prevUni].BackColor = Color.Red;
                }
                else
                {
                    uniButtonArray[prevUni].BackColor = Color.Gray;
                }
                for (int i = 0; i < 32; ++i)
                {
                    if ((sender as Button).Text == uniButtonArray[i].Text)
                    {
                        prevUni = i;
                        break;
                    }
                }

            }
            if (validUni != null && validUni.Contains(activeUniverse))
            {
                universeinPresets = true;
                button3.Text = "Remove Universe from Presets";
            }
            else
            {
                universeinPresets = false;
                button3.Text = "Add Universe to Presets";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (validUni != null && validUni.Contains(activeUniverse))
            {
                button3.BackColor = Color.White;
                FileStream temp = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                FileStream fs = new FileStream(mbedDrive + ":\\art" + activePreset + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                temp.SetLength(0);
                BinaryWriter sw = new BinaryWriter(temp);
                BinaryReader sr = new BinaryReader(fs);
                byte[] buffer = new byte[530];
                while (true)
                {
                    if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                    {
                        int tempUniverse = ((int)buffer[15] << 8) | (int)buffer[14];
                        if (tempUniverse == activeUniverse)
                        {
                            for (int i = 0; i < 512; ++i)
                            {
                                buffer[i + 18] = (byte)(Convert.ToInt32(Math.Round(addressArray[i].Value, 0)));
                            }
                        }
                        sw.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        break;
                    }
                }
                sw.Flush();
                sw.Close();
                sr.Close();

                temp = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs = new FileStream(mbedDrive + ":\\art" + activePreset + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.SetLength(0);
                sw = new BinaryWriter(fs);
                sr = new BinaryReader(temp);

                while (true)
                {
                    if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                    {
                        sw.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        break;
                    }
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            } else
            {
                button3.BackColor = Color.HotPink;
            }

            }

            private void button2_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(mbedDrive + ":\\art" + (activePreset) + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader sr = new BinaryReader(fs);
            byte[] buffer = new byte[530];
            bool inFile = false;
            while (true)
            {
                if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                {
                    int tempUniverse = ((int)buffer[15] << 8) | (int)buffer[14];
                    if (tempUniverse == activeUniverse)
                    {
                        inFile = true;
                        for (int i = 18; i < 530; ++i)
                        {
                            addressArray[i - 18].Value = ((int)buffer[i]) & 0xFF;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (!inFile)
            {
                for (int i = 0; i < 512; ++i)
                {
                    addressArray[i].Value = 0;
                }
            }
            sr.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.White;
            if (!universeinPresets)
            {
                for (int i = 0; i <= 6; ++i)
                {
                    FileStream temp = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    FileStream fs = new FileStream(mbedDrive + ":\\art" + i + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    temp.SetLength(0);
                    BinaryWriter sw = new BinaryWriter(temp);
                    BinaryReader sr = new BinaryReader(fs);
                    byte[] buffer = new byte[530];
                    bool inFile = false;
                    while (true)
                    {
                        if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                        {
                            sw.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            break;
                        }
                    }
                    byte[] artNet = [(byte)0x41, (byte)0x72, (byte)0x74, (byte)0x2d, (byte)0x4e, (byte)0x65, (byte)0x74, (byte)0x00, (byte)0x00, (byte)0x50, (byte)0x00, (byte)0x0e, (byte)0x00, (byte)0x00, (byte)0x05, (byte)0x00, (byte)0x02, (byte)0x00];
                    artNet[15] = (byte)((activeUniverse >> 8) & 0xFF);
                    artNet[14] = (byte)(activeUniverse & 0xFF);
                    sw.Write(artNet, 0, artNet.Length);
                    for (int j = 0; j < 512; ++j)
                    {
                        sw.Write((byte)addressArray[j].Value);
                    }
                    sw.Flush();
                    sw.Close();
                    sr.Close();

                    temp = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs = new FileStream(mbedDrive + ":\\art" + i + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.SetLength(0);
                    sw = new BinaryWriter(fs);
                    sr = new BinaryReader(temp);

                    while (true)
                    {
                        if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                        {
                            sw.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            break;
                        }
                    }

                    sw.Flush();
                    sw.Close();
                    sr.Close();
                }
            }
            else
            {
                for (int i = 0; i <= 6; ++i)
                {
                    FileStream temp = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    FileStream fs = new FileStream(mbedDrive + ":\\art" + i + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    temp.SetLength(0);
                    BinaryWriter sw = new BinaryWriter(temp);
                    BinaryReader sr = new BinaryReader(fs);
                    byte[] buffer = new byte[530];
                    while (true)
                    {
                        if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                        {
                            int tempUniverse = ((int)buffer[15] << 8) | (int)buffer[14];
                            if (tempUniverse != activeUniverse)
                            {
                                sw.Write(buffer, 0, buffer.Length);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                    fs = new FileStream("temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    temp = new FileStream(mbedDrive + ":\\art" + i + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    temp.SetLength(0);
                    sw = new BinaryWriter(temp);
                    sr = new BinaryReader(fs);
                    while (true)
                    {
                        if (sr.Read(buffer, 0, buffer.Length) == 530 && buffer[0] == 0x41)
                        {
                            sw.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            break;
                        }
                    }
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                }
            }
            universeinPresets = !universeinPresets;
            if (universeinPresets)
            {
                universeinPresets = true;
                button3.Text = "Remove Universe from Presets";
                if (validUni != null)
                {
                    Array.Resize(ref validUni, validUni.Length + 1);
                    validUni[validUni.Length - 1] = activeUniverse;
                }
                else
                {
                    validUni = new int[1] { activeUniverse };
                }

            }
            else
            {
                for (int i = 0; i < validUni.Length; ++i)
                {
                    bool replace = false;
                    if (validUni != null && validUni[i] == activeUniverse)
                    {
                        replace = true;
                    }
                    if (replace)
                    {
                        if (validUni != null && i == (validUni.Length - 1))
                        {
                            Array.Resize(ref validUni, validUni.Length - 1);
                            break;
                        }
                        else
                        {
                            validUni[i] = validUni[i + 1];
                        }
                    }
                }
                universeinPresets = false;
                button3.Text = "Add Universe to Presets";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 6; ++i)
            {
                FileStream fs = new FileStream(mbedDrive + ":\\art" + i + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.SetLength(0);
                fs.Close();
            }
            validUni = null;
            for (int i = 0; i < uniButtonArray.Length; ++i)
            {
                if (uniButtonArray[i].BackColor == Color.Red)
                {
                    uniButtonArray[i].BackColor = Color.Gray;
                }
            }
            button3.Text = "Add Universe to Preset";
            universeinPresets = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Length > 1)
            {
                mbedDrive = (sender as TextBox).Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            comPort = (sender as TextBox).Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort = new SerialPort(comPort);
            serialPort.BaudRate = 9600;
            serialPort.Open();
            serialPort.Write("Reset");
            serialPort.Close();


        }
    }
}

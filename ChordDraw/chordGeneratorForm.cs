/*
 * From Eddie Sullivan
 * The Chorderator Chord Generator
 * https://sourceforge.net/projects/chorderator/
 *
 * 
 * 
 * 
 */

using System;
using System.Windows.Forms;
using ChordLib;

namespace ChordDraw
{
    public partial class chordGeneratorForm : Form
    {
        private string[] tuningList = new string[] {"Guitar: Standard", "EADGBE",
                                                       "Guitar: Dropped D", "DADGBE",
                                                       "Guitar: DADGAD", "DADGAD",
                                                       "Guitar: Open G", "DGDGBD",
                                                       "Guitar: Open D", "DADF#AD",
                                                       "Guitar: Open Em", "EBEGBE",
                                                       "Guitar: Detuned 1/2-step", "EbAbDbGbBbEb",                                                       
                                                       "Ukulele (soprano)", "GCEA",
                                                       "Mandolin", "EADG",
                                                       "<Custom>", ""
                                                   };
       
        private int chordPicWidth, chordPicBorder;
                      

        public chordGeneratorForm()
        {
            InitializeComponent();

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);


            

            // Size the chord pictures just big enough to fit 3 across.
            chordPicBorder = panelResults.ClientRectangle.Width / 40;
            chordPicWidth = (panelResults.ClientRectangle.Width - chordPicBorder * 4) / 3 - 1;

            for (int i = 0; i < tuningList.Length; i += 2)
            {
                comboBoxTuning.Items.Add(tuningList[i]);
            }
            comboBoxTuning.SelectedIndex = 0;
            textBoxTuning.Text = tuningList[1];

        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            ChordParser.SetTuning(textBoxTuning.Text, (int)upDownCapo.Value);
            ChordParser cp = new ChordParser();

            try
            {
                cp.parse(this.inputTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            this.outputTextBox.Text = cp.GetListing();
            panelResults.Controls.Clear();
            foreach (Fingering fing in cp.mergedChords)
            {
                ChordPicture cpic = new ChordPicture();
                cpic.ChordToShow = fing;
                //				cpic.Visible = true;
                //				cpic.Enabled = true;
                panelResults.Controls.Add(cpic);
            }
            inputTextBox.SelectAll();
            textBoxChordInEnglish.Text = cp.ToString();
            buttonGenerate.Enabled = false;
        }

        /// <summary>
        /// TO ADD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelResults_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            // If we were using a later version of .NET, we could have it
            // do the layout for us.  Unfortunately, we can't do that now.

            int border = chordPicBorder;
            if (panelResults.ClientSize.Width < chordPicWidth + border * 2)
            {
                // Shrink the border proportionally.
                border = border * panelResults.ClientSize.Width / (chordPicWidth + border * 2);
            }
            int x = border, y = border;
            foreach (ChordPicture cpic in panelResults.Controls)
            {
                int width = Math.Min(chordPicWidth, panelResults.ClientSize.Width - border * 2);
                cpic.SetBounds(x, y, width, width);
                x += width + border;
                if (x > panelResults.ClientSize.Width - width)
                {
                    y += width + border;
                    x = border;
                }
            }
        }

        private void comboBoxTuning_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTuning.Focused)
            {
                if (comboBoxTuning.SelectedIndex == tuningList.Length / 2 - 1)
                {
                    // User selected Custom.
                    textBoxTuning.SelectAll();
                    textBoxTuning.Focus();
                }
                else
                {
                    textBoxTuning.Text = tuningList[comboBoxTuning.SelectedIndex * 2 + 1];
                }
            }
        }

        /// <summary>
        /// TO ADD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTuning_TextChanged(object sender, System.EventArgs e)
        {
            if (textBoxTuning.Focused)
            {
                string text = textBoxTuning.Text;
                for (int i = 1; i < tuningList.Length; i += 2)
                {
                    if (tuningList[i] == text)
                    {
                        comboBoxTuning.SelectedIndex = i / 2; // Will automatically round down.
                        return;
                    }
                }
                // If we get here, we didn't find it.  Call it custom.
                comboBoxTuning.SelectedIndex = tuningList.Length / 2 - 1;
            }
            buttonGenerate.Enabled = true;
        }

        private void upDownCapo_ValueChanged(object sender, EventArgs e)
        {
            buttonGenerate.Enabled = true;
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            buttonGenerate.Enabled = true;
        }

        private void upDownCapo_KeyPress(object sender, KeyPressEventArgs e)
        {
            buttonGenerate.Enabled = true;
        }

       

       

       



    }
}

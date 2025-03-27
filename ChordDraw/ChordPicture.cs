using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChordLib
{
    public partial class ChordPicture : UserControl
    {
       
        private Fingering chordToShow = null;

        public ChordPicture()
        {
            InitializeComponent();

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.Resize += new EventHandler(ChordPicture_Resize);
            this.MouseDown += new MouseEventHandler(ChordPicture_MouseDown);
        }

        public Fingering ChordToShow
        {
            set
            {
                chordToShow = value;
                this.Invalidate();
            }
            get
            {
                return chordToShow;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // ES: Much of this code is the same as the GTK# version.  See about abstracting it.

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int numStrings = this.chordToShow.NumStrings;
            int numFrets = 6; // ES: Make this adjustable

            base.OnPaint(e);

            System.Drawing.Graphics g = e.Graphics;

            float xSpace = (this.ClientRectangle.Right - this.ClientRectangle.Left) / (float)(numStrings + 1);
            float ySpace = (this.ClientRectangle.Bottom - this.ClientRectangle.Top) / (float)(numFrets + 1);

            g.Clear(System.Drawing.SystemColors.Window);
            // Strings, drawn vertically.
            for (int i = 1; i < numStrings + 1; i++)
            {
                g.DrawLine(SystemPens.ControlText, i * xSpace, ySpace, i * xSpace, ySpace * (float)numFrets);
            }
            // Frets, drawn horizontally.
            for (int i = 1; i < numFrets + 1; i++)
            {
                g.DrawLine(SystemPens.ControlText, xSpace, i * ySpace, xSpace * (float)numStrings, i * ySpace);
            }

            if (chordToShow != null)
            {
                // Find the fret range.
                int highestFret = 0;
                for (int i = 0; i < numStrings; i++)
                {
                    if (chordToShow.IsPlayed(i))
                    {
                        highestFret = Math.Max(highestFret, chordToShow[i] + 1);
                    }
                }
                int lowestFret = 0;
                if (highestFret > numFrets) lowestFret = highestFret - numFrets;
                // Fret numbers.
                for (int i = 0; i < numStrings; i++)
                {
                    string fretText = (i + lowestFret).ToString();
                    SizeF stringSize = g.MeasureString(fretText, Font, (int)xSpace);
                    g.DrawString(fretText, Font, SystemBrushes.ControlText,
                        new PointF(xSpace - stringSize.Width - Font.Size / 2, (i + 1) * ySpace - stringSize.Height / 2));
                }
                // Dots.
                for (int i = 0; i < numStrings; i++)
                {
                    if (chordToShow.IsPlayed(i))
                    {
                        string noteText = chordToShow.GetNote(i).ToString();
                        SizeF noteTextSize = g.MeasureString(noteText, Font, (int)xSpace);

                        if (chordToShow[i] == 0)
                        {
                            Pen pen = SystemPens.WindowText;
                            Brush textBrush = SystemBrushes.WindowText;

                            if (chordToShow.GetNote(i).NoteName == chordToShow.PlayedChord.RootNote.NoteName)
                            {
                                pen = SystemPens.Highlight;
                            }

                            if (!chordToShow.IsRequired(i))
                            {
                                Brush brush = new System.Drawing.Drawing2D.HatchBrush(
                                    System.Drawing.Drawing2D.HatchStyle.Percent20,
                                    SystemColors.WindowText, SystemColors.Window);
                                g.FillEllipse(brush, ((i + 1) * xSpace) - xSpace / 2,
                                    0, xSpace, ySpace);
                                brush.Dispose();
                            }

                            g.DrawEllipse(pen, ((i + 1) * xSpace) - xSpace / 2,
                                0, xSpace, ySpace);

                            g.DrawString(noteText, Font,
                                textBrush, (i + 1) * xSpace - noteTextSize.Width / 2,
                                ySpace / 2 - noteTextSize.Height / 2);
                        }
                        else
                        {
                            Brush brush = SystemBrushes.WindowText;
                            Brush textBrush = SystemBrushes.Window;

                            bool needToDisposeBrush = false;
                            if (!chordToShow.IsRequired(i))
                            {
                                brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent75,
                                    SystemColors.WindowText, SystemColors.Window);
                                needToDisposeBrush = true;
                            }
                            else if (chordToShow.GetNote(i).NoteName == chordToShow.PlayedChord.RootNote.NoteName)
                            {
                                brush = SystemBrushes.Highlight;
                                textBrush = SystemBrushes.HighlightText;
                            }
                            g.FillEllipse(brush, ((i + 1) * xSpace) - xSpace / 2,
                                (chordToShow[i] - lowestFret) * ySpace, xSpace, ySpace);

                            g.DrawString(noteText, Font,
                                SystemBrushes.ControlLight, (i + 1) * xSpace - noteTextSize.Width / 2,
                                (chordToShow[i] - lowestFret + 0.5F) * ySpace - noteTextSize.Height / 2);

                            if (needToDisposeBrush)
                            {
                                brush.Dispose();
                            }
                        }
                    }
                    else
                    {
                        // Draw an X.
                        SizeF stringSize = g.MeasureString("X", Font, (int)xSpace);
                        g.DrawString("X", Font, SystemBrushes.ControlText,
                            new PointF((i + 1) * xSpace - stringSize.Width / 2,
                            ySpace - stringSize.Height - Font.Size / 2));
                    }
                }
            }
        }

        

        private void ChordPicture_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ChordPicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (Parent != null)
            {
                this.Parent.Focus();
            }
        }

    }
}

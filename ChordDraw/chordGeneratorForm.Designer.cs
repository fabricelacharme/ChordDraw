using System;
using System.Windows.Forms;

namespace ChordDraw
{
    partial class chordGeneratorForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxChordInEnglish = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.textBoxTuning = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxTuning = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.upDownCapo = new System.Windows.Forms.NumericUpDown();
            this.panelResults = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.upDownCapo)).BeginInit();
            this.SuspendLayout();            
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(0, 0);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(112, 8);
            this.outputTextBox.TabIndex = 1;
            this.outputTextBox.Text = "Please type a chord name";
            this.outputTextBox.Visible = false;
            // 
            // panelResults
            // 
            this.panelResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelResults.AutoScroll = true;
            this.panelResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelResults.Location = new System.Drawing.Point(40, 128);
            this.panelResults.Name = "panelResults";
            this.panelResults.Size = new System.Drawing.Size(576, 464);
            this.panelResults.TabIndex = 5;
            this.panelResults.Layout += new System.Windows.Forms.LayoutEventHandler(this.panelResults_Layout);

            // 
            // textBoxChordInEnglish
            // 
            this.textBoxChordInEnglish.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxChordInEnglish.Location = new System.Drawing.Point(32, 104);
            this.textBoxChordInEnglish.Name = "textBoxChordInEnglish";
            this.textBoxChordInEnglish.ReadOnly = true;
            this.textBoxChordInEnglish.Size = new System.Drawing.Size(576, 13);
            this.textBoxChordInEnglish.TabIndex = 8;
            this.textBoxChordInEnglish.Text = "";
            this.textBoxChordInEnglish.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxTuning
            // 
            this.textBoxTuning.Location = new System.Drawing.Point(440, 56);
            this.textBoxTuning.Name = "textBoxTuning";
            this.textBoxTuning.TabIndex = 3;
            this.textBoxTuning.Text = "EADGBE";
            this.textBoxTuning.TextChanged += new System.EventHandler(this.textBoxTuning_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(352, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 23);
            this.label2.TabIndex = 14;
            this.label2.Text = "or customize:";
            // 
            // comboBoxTuning
            // 
            this.comboBoxTuning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTuning.Location = new System.Drawing.Point(208, 56);
            this.comboBoxTuning.Name = "comboBoxTuning";
            this.comboBoxTuning.Size = new System.Drawing.Size(136, 21);
            this.comboBoxTuning.TabIndex = 2;
            this.comboBoxTuning.SelectedIndexChanged += new System.EventHandler(this.comboBoxTuning_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 23);
            this.label1.TabIndex = 12;
            this.label1.Text = "How is your instrument tuned?";
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonGenerate.Enabled = false;
            this.buttonGenerate.Location = new System.Drawing.Point(352, 23);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(128, 24);
            this.buttonGenerate.TabIndex = 1;
            this.buttonGenerate.Text = "Generate fingerings";
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(208, 25);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(136, 20);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.Text = "";
            this.inputTextBox.TextChanged += new System.EventHandler(this.inputTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 23);
            this.label3.TabIndex = 16;
            this.label3.Text = "What chord do you want to play?";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(32, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 23);
            this.label4.TabIndex = 17;
            this.label4.Text = "What fret is your capo on (0 means no capo)?";
            // 
            // upDownCapo
            // 
            this.upDownCapo.CausesValidation = false;
            this.upDownCapo.Location = new System.Drawing.Point(272, 80);
            this.upDownCapo.Maximum = new System.Decimal(new int[] {
                                                                       12,
                                                                       0,
                                                                       0,
                                                                       0});
            this.upDownCapo.Name = "upDownCapo";
            this.upDownCapo.Size = new System.Drawing.Size(48, 20);
            this.upDownCapo.TabIndex = 4;
            this.upDownCapo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.upDownCapo_KeyPress);
            this.upDownCapo.ValueChanged += new System.EventHandler(this.upDownCapo_ValueChanged);

            // 
            // chordGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 606);
            this.Controls.Add(this.upDownCapo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxChordInEnglish);
            this.Controls.Add(this.panelResults);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTuning);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTuning);
            this.Controls.Add(this.inputTextBox);
            this.Name = "chordGeneratorForm";
            this.Text = "Chord generator";
            ((System.ComponentModel.ISupportInitialize)(this.upDownCapo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       

        #endregion

        private System.Windows.Forms.TextBox textBoxChordInEnglish;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox textBoxTuning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxTuning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown upDownCapo;
        private System.Windows.Forms.Panel panelResults;
    }
}


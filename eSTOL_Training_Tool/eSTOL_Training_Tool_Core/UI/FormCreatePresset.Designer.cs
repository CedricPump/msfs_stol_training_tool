namespace STOL_Training_Tool_Core.UI
{
    partial class FormCreatePresset
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonSave = new System.Windows.Forms.Button();
            buttonAbort = new System.Windows.Forms.Button();
            labelPresetName = new System.Windows.Forms.Label();
            textBoxPresetName = new System.Windows.Forms.TextBox();
            labelData = new System.Windows.Forms.Label();
            textBoxPresetData = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSave.Location = new System.Drawing.Point(427, 191);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonAbort
            // 
            buttonAbort.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonAbort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonAbort.Location = new System.Drawing.Point(346, 191);
            buttonAbort.Name = "buttonAbort";
            buttonAbort.Size = new System.Drawing.Size(75, 23);
            buttonAbort.TabIndex = 3;
            buttonAbort.Text = "Abort";
            buttonAbort.UseVisualStyleBackColor = true;
            buttonAbort.Click += buttonAbort_Click;
            // 
            // labelPresetName
            // 
            labelPresetName.AutoSize = true;
            labelPresetName.Location = new System.Drawing.Point(12, 9);
            labelPresetName.Name = "labelPresetName";
            labelPresetName.Size = new System.Drawing.Size(74, 15);
            labelPresetName.TabIndex = 98;
            labelPresetName.Text = "Preset Name";
            // 
            // textBoxPresetName
            // 
            textBoxPresetName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxPresetName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxPresetName.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBoxPresetName.Location = new System.Drawing.Point(12, 27);
            textBoxPresetName.Name = "textBoxPresetName";
            textBoxPresetName.Size = new System.Drawing.Size(490, 19);
            textBoxPresetName.TabIndex = 1;
            textBoxPresetName.TextChanged += textBoxPresetName_TextChanged;
            // 
            // labelData
            // 
            labelData.AutoSize = true;
            labelData.Location = new System.Drawing.Point(12, 53);
            labelData.Name = "labelData";
            labelData.Size = new System.Drawing.Size(66, 15);
            labelData.TabIndex = 99;
            labelData.Text = "Preset Data";
            // 
            // textBoxPresetData
            // 
            textBoxPresetData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxPresetData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxPresetData.Enabled = false;
            textBoxPresetData.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBoxPresetData.Location = new System.Drawing.Point(12, 71);
            textBoxPresetData.Multiline = true;
            textBoxPresetData.Name = "textBoxPresetData";
            textBoxPresetData.Size = new System.Drawing.Size(490, 106);
            textBoxPresetData.TabIndex = 4;
            // 
            // FormCreatePresset
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(514, 226);
            Controls.Add(textBoxPresetData);
            Controls.Add(labelData);
            Controls.Add(textBoxPresetName);
            Controls.Add(labelPresetName);
            Controls.Add(buttonAbort);
            Controls.Add(buttonSave);
            Name = "FormCreatePresset";
            Text = "FormCreatePresset";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Label labelPresetName;
        private System.Windows.Forms.TextBox textBoxPresetName;
        private System.Windows.Forms.Label labelData;
        private System.Windows.Forms.TextBox textBoxPresetData;
    }
}
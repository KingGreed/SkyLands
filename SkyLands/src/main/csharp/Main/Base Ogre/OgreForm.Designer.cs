using Awesomium.Core;

namespace Game.BaseApp {
    partial class OgreForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            webView = new Awesomium.Windows.Forms.WebControl(this.components);
            SelectBar = new Awesomium.Windows.Forms.WebControl(this.components);
            this.SuspendLayout();
            // 
            // webView
            // 
            webView.Dock = System.Windows.Forms.DockStyle.Fill;
            webView.Location = new System.Drawing.Point(0, 0);
            webView.Margin = new System.Windows.Forms.Padding(4);
            webView.Size = new System.Drawing.Size(959, 428);
            webView.TabIndex = 0;
            // 
            // SelectBar
            // 
            SelectBar.Location = new System.Drawing.Point(158, 45);
            SelectBar.Size = new System.Drawing.Size(75, 23);
            SelectBar.TabIndex = 1;
            // 
            // OgreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 428);
            this.Controls.Add(SelectBar);
            this.Controls.Add(webView);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OgreForm";
            this.Text = "OgreForm";
            this.ResumeLayout(false);

        }

        #endregion

        public static Awesomium.Windows.Forms.WebControl SelectBar;
        public static Awesomium.Windows.Forms.WebControl webView;




    }
}
using System.ComponentModel;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OgreForm));
            webView = new Awesomium.Windows.Forms.WebControl(this.components);
            SelectBar = new Awesomium.Windows.Forms.WebControl(this.components);
            Hud = new Awesomium.Windows.Forms.WebControl(this.components);
            this.SuspendLayout();
            // 
            // webView
            // 
            webView.Dock = System.Windows.Forms.DockStyle.Fill;
            webView.Location = new System.Drawing.Point(0, 0);
            webView.Size = new System.Drawing.Size(884, 353);
            webView.TabIndex = 0;
            // 
            // SelectBar
            // 
            SelectBar.Location = new System.Drawing.Point(118, 37);
            SelectBar.Margin = new System.Windows.Forms.Padding(2);
            SelectBar.Size = new System.Drawing.Size(56, 19);
            SelectBar.TabIndex = 1;
            // 
            // Hud
            // 
            Hud.Location = new System.Drawing.Point(0, 0);
            Hud.Size = new System.Drawing.Size(800, 29);
            Hud.TabIndex = 2;
            // 
            // OgreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 353);
            this.Controls.Add(Hud);
            this.Controls.Add(SelectBar);
            this.Controls.Add(webView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SkyLands";
            this.Text = "SkyLands";
            this.ResumeLayout(false);

        }

        #endregion

        public static Awesomium.Windows.Forms.WebControl Hud;
        public static Awesomium.Windows.Forms.WebControl SelectBar;
        public static Awesomium.Windows.Forms.WebControl webView;







    }
}
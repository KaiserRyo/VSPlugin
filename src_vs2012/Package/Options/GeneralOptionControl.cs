﻿using System;
using System.Windows.Forms;
using BlackBerry.NativeCore;
using BlackBerry.Package.Helpers;

namespace BlackBerry.Package.Options
{
    public partial class GeneralOptionControl : UserControl
    {
        public GeneralOptionControl()
        {
            InitializeComponent();
            OnReset();
        }

        #region Properties

        public string NdkPath
        {
            get { return txtNdkPath.Text; }
            set { txtNdkPath.Text = value; }
        }

        public string JavaHomePath
        {
            get { return txtJavaPath.Text; }
            set { txtJavaPath.Text = value; }
        }

        public string ToolsPath
        {
            get { return txtToolsPath.Text; }
            set { txtToolsPath.Text = value; }
        }

        public string ProfilePath
        {
            get { return txtProfilePath.Text; }
            set { txtProfilePath.Text = value; }
        }

        /// <summary>
        /// Checks if open URL links in internal or external browser.
        /// </summary>
        public bool IsOpeningExternal
        {
            get { return chkOpenInExternal.Checked; }
            set { chkOpenInExternal.Checked = value; }
        }

        #endregion

        private void bttNdkBrowse_Click(object sender, EventArgs e)
        {
            txtNdkPath.Text = DialogHelper.BrowseForFolder(txtNdkPath.Text, "Browse for NDK folder");
        }

        private void bttToolsBrowse_Click(object sender, EventArgs e)
        {
            txtToolsPath.Text = DialogHelper.BrowseForFolder(txtToolsPath.Text, "Browse for Tools folder");
        }

        private void bttJavaBrowse_Click(object sender, EventArgs e)
        {
            txtJavaPath.Text = DialogHelper.BrowseForFolder(txtJavaPath.Text, "Browse for Java Home folder");
        }

        private void bttOpenProfile_Click(object sender, EventArgs e)
        {
            DialogHelper.StartExplorer(ProfilePath);
        }

        public void OnReset()
        {
            txtNdkPath.Text = ConfigDefaults.NdkDirectory;
            txtToolsPath.Text = ConfigDefaults.ToolsDirectory;
            txtProfilePath.Text = ConfigDefaults.DataDirectory;
        }
    }
}

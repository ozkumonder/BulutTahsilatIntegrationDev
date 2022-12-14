
namespace BulutTahsilatIntegration.WinService
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BulutIntegrationServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BulutIntegrationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BulutIntegrationServiceProcessInstaller
            // 
            this.BulutIntegrationServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BulutIntegrationServiceProcessInstaller.Password = null;
            this.BulutIntegrationServiceProcessInstaller.Username = null;
            // 
            // BulutIntegrationServiceInstaller
            // 
            this.BulutIntegrationServiceInstaller.Description = "BTI Bulut Tahsilat Ödeme Harekerleri Entegrasyonu";
            this.BulutIntegrationServiceInstaller.DisplayName = "Bulut Tahsilat Ödeme Hareketleri Servisi";
            this.BulutIntegrationServiceInstaller.ServiceName = "BulutIntegrationService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BulutIntegrationServiceProcessInstaller,
            this.BulutIntegrationServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BulutIntegrationServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BulutIntegrationServiceInstaller;
    }
}
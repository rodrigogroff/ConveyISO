// Decompiled with JetBrains decompiler
// Type: ConveyISO.frmMain
// Assembly: ConveyISO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A1A29DB8-D4AD-4F47-B8FA-3FADEED7E861
// Assembly location: C:\Users\rodrigo.groff\Desktop\ciso\ConveyISO.exe

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConveyISO
{
    public class frmMain : Form
    {
        public string versao = "v(1.2)";

        private IContainer components = (IContainer)null;
        private bool appActive;
        private bool appTreadStart;
        public Thread workThread;
        private SocketConvey SocketWork;
        private string hoje;
        private ListBox logTela;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem MenuParametros;
        private ToolStripMenuItem MenuDebug;
        private ToolStripMenuItem MenuIniciar;
        private StatusStrip statBar;
        private System.Windows.Forms.Timer timer1;
        private ProgressBar progressBar1;
        private ToolStripMenuItem MenuPausar;
        private ToolStripMenuItem MenuEncerrar;
        private ToolStripMenuItem logsToolStripMenuItem;
        private ToolStripMenuItem MenuVisualizar;

        public frmMain()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("LIXOOOOO frmMain");
                
                throw (new System.Exception(ex.ToString()));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string str = "F1F1F1F1F2F2F2F2";
                byte[] numArray = new byte[8];
                for (int index = 0; index < str.Length / 2; ++index)
                    numArray[index] = (byte)Convert.ToInt32(str.Substring(index * 2, 2), 16);
                Util.DESdeCript(Util.DESCript("00345678", numArray), numArray);
                Util.DESdeCript(Util.DESCript("00345678", "12345678"), "12345678");
                string path = DateTime.Now.DayOfYear.ToString() + "Log.txt";
                GlobalVar.nomelog = path;
                GlobalVar.m_log_file = !File.Exists(path) ? new FileStream(path, FileMode.Create, FileAccess.Write) : new FileStream(path, FileMode.Append, FileAccess.Write);
                GlobalVar.m_Log = new StreamWriter((Stream)GlobalVar.m_log_file);
                GlobalVar.m_Log.AutoFlush = true;
                GlobalVar.m_Log.WriteLine("");
                GlobalVar.m_Log.WriteLine("============================================");
                GlobalVar.m_Log.WriteLine("# ");
                GlobalVar.m_Log.WriteLine("# ConveyISO Server Log " + versao);
                GlobalVar.m_Log.WriteLine("# " + DateTime.Now.ToString());
                GlobalVar.m_Log.WriteLine("# ");
                GlobalVar.m_Log.WriteLine("============================================");
                GlobalVar.SocketPort = Util.GetIni("Socket", "Port", "parametros.ini", "");
                if (Util.GetIni("GERAL", "DEBUG", "parametros.ini", "").ToUpper() == "SIM")
                {
                    Util.LOGCHECK("Iniciando em modo  DEBUG");
                    this.AtualizaTela("Iniciando em modo  DEBUG");
                    GlobalVar.debugApp = true;
                    this.MenuDebug.Checked = true;
                }
                if (Util.GetIni("GERAL", "INICIAR", "parametros.ini", "") == "AUTOMATICO")
                {
                    Util.LOGCHECK("Iniciando automaticamente o sistema");
                    this.AtualizaTela("Iniciando automaticamente o sistema");
                    this.MenuIniciar_Click(sender, e);
                }
                GlobalVar.tipoRoteamento = Util.GetIni("GERAL", "tipoCE", "parametros.ini", "");
                Util.LOGDADOS("Tipo Roteamento : " + GlobalVar.tipoRoteamento);
                this.AtualizaTela("Tipo Roteamento : " + GlobalVar.tipoRoteamento);
                GlobalVar.SocketIPCE = Util.GetIni("Socket", "ipCE", "parametros.ini", "");
                Util.LOGDADOS("IP Cartao Empresarial = " + GlobalVar.SocketIPCE);
                this.AtualizaTela("IP Cartao Empresarial = " + GlobalVar.SocketIPCE);
                GlobalVar.SocketPortCE = Util.GetIni("Socket", "PortCE", "parametros.ini", "");
                Util.LOGDADOS("Porta Cartao Empresarial = " + GlobalVar.SocketPortCE);
                this.AtualizaTela("Porta Cartao Empresarial = " + GlobalVar.SocketPortCE);
                GlobalVar.frmPrincipal = this;
                Util.LOGSAIDA();
            }
            catch (System.Exception ex)
            {
                ProcessException(ex);
                Application.Exit();
            }
        }

        public void ProcessException(System.Exception ex)
        {
            StreamWriter sw = new StreamWriter("Fail" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".txt", false, Encoding.Default);
            sw.WriteLine(ex.ToString());
            sw.Close();
        }

        public void AtualizaTela(string t)
        {
            try
            {
                if (this.logTela.InvokeRequired)
                    this.Invoke((Delegate)new frmMain.AtualizaTextoCallBack(this.AtualizaTela), (object)t);
                else
                    this.logTela.Items.Add((object)t);
            }
            catch (System.Exception ex)
            {
                ProcessException(ex);
                Application.Exit();
            }
        }

        private void MenuIniciar_Click(object sender, EventArgs e)
        {
            Util.LOGENTRADA();
            if (this.appActive)
                return;
            this.appActive = true;
            this.statBar.Text = "Aplicacao Ativa";
            Util.LOGCHECK("Aplicacao Ativada");
            if (this.appTreadStart)
            {
                Util.LOGSAIDA();
            }
            else
            {
                this.workThread = new Thread(new ThreadStart(this.ThreadAPP));
                this.workThread.Start();
                this.appTreadStart = true;
                Util.LOGSAIDA();
            }
        }

        private void ThreadAPP()
        {
            try
            {
                Util.LOGENTRADA();
                this.SocketWork = new SocketConvey();
                this.SocketWork.port = int.Parse(GlobalVar.SocketPort);
                this.AtualizaTela("Socket Iniciado - Porta :" + GlobalVar.SocketPort);
                this.SocketWork.start();
                Util.LOGSAIDA();
            }
            catch (System.Exception ex)
            {
                ProcessException(ex);
                Application.Exit();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!this.appActive)
                    return;
                ++this.progressBar1.Value;
                if (this.progressBar1.Value > 99)
                    this.progressBar1.Value = 0;
                string path = DateTime.Now.DayOfYear.ToString() + "Log.txt";
                if (!(GlobalVar.nomelog != path))
                    return;
                GlobalVar.m_log_file.Close();
                GlobalVar.nomelog = path;
                GlobalVar.m_log_file = !File.Exists(path) ? new FileStream(path, FileMode.Create, FileAccess.Write) : new FileStream(path, FileMode.Append, FileAccess.Write);
                GlobalVar.m_Log = new StreamWriter((Stream)GlobalVar.m_log_file);
                GlobalVar.m_Log.AutoFlush = true;
                GlobalVar.m_Log.WriteLine("");
                GlobalVar.m_Log.WriteLine("============================================");
                GlobalVar.m_Log.WriteLine("# ");
                GlobalVar.m_Log.WriteLine("# ConveyISO Server Log " + versao);
                GlobalVar.m_Log.WriteLine("# " + DateTime.Now.ToString());
                GlobalVar.m_Log.WriteLine("# ");
                GlobalVar.m_Log.WriteLine("============================================");
            }
            catch (System.Exception ex)
            {
                ProcessException(ex);
                Application.Exit();
            }
        }

        private void MenuDebug_Click(object sender, EventArgs e)
        {
            Util.LOGENTRADA();
            if (GlobalVar.debugApp)
            {
                Util.LOGCHECK("DESATIVANDO OPCAO DEBUG");
                GlobalVar.debugApp = false;
                this.MenuDebug.Checked = false;
            }
            else
            {
                Util.LOGCHECK("ATIVANDO OPCAO DEBUG");
                GlobalVar.debugApp = true;
                this.MenuDebug.Checked = true;
            }
            Util.LOGSAIDA();
        }

        private void MenuVisualizar_Click(object sender, EventArgs e)
        {
            Util.LOGENTRADA();
            this.hoje = DateTime.Now.DayOfYear.ToString();
            string fileName = this.hoje + "Log.txt";
            Util.LOGCHECK(" Visualizando Logs");
            Process.Start(fileName);
            Util.LOGSAIDA();
        }

        private void MenuParametros_Click(object sender, EventArgs e)
        {
        }

        private void MenuEncerrar_Click(object sender, EventArgs e)
        {
            Util.LOGENTRADA();
            Util.LOGCHECK("FECHANDO SISTEMA");
            try
            {
                if (this.appTreadStart)
                {
                    GlobalVar.finalizar = true;
                    this.SocketWork.abort();
                    this.workThread.Abort();
                    this.workThread.Join(100);
                }
                GlobalVar.m_Log.Close();
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", (object)ex);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GlobalVar.finalizar)
                return;
            e.Cancel = true;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.logTela = new ListBox();
            this.menuStrip1 = new MenuStrip();
            this.MenuParametros = new ToolStripMenuItem();
            this.MenuDebug = new ToolStripMenuItem();
            this.MenuIniciar = new ToolStripMenuItem();
            this.MenuPausar = new ToolStripMenuItem();
            this.MenuEncerrar = new ToolStripMenuItem();
            this.logsToolStripMenuItem = new ToolStripMenuItem();
            this.MenuVisualizar = new ToolStripMenuItem();
            this.statBar = new StatusStrip();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new ProgressBar();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            this.logTela.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.logTela.FormattingEnabled = true;
            this.logTela.Location = new Point(0, 28);
            this.logTela.Name = "logTela";
            this.logTela.Size = new Size(561, 186);
            this.logTela.TabIndex = 0;
            this.menuStrip1.Items.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.MenuParametros,
        (ToolStripItem) this.logsToolStripMenuItem
            });
            this.menuStrip1.Location = new Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new Size(561, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            this.MenuParametros.DropDownItems.AddRange(new ToolStripItem[4]
            {
        (ToolStripItem) this.MenuDebug,
        (ToolStripItem) this.MenuIniciar,
        (ToolStripItem) this.MenuPausar,
        (ToolStripItem) this.MenuEncerrar
            });
            this.MenuParametros.Name = "MenuParametros";
            this.MenuParametros.Size = new Size(64, 20);
            this.MenuParametros.Text = "Aplicação";
            this.MenuParametros.Click += new EventHandler(this.MenuParametros_Click);
            this.MenuDebug.Name = "MenuDebug";
            this.MenuDebug.Size = new Size(126, 22);
            this.MenuDebug.Text = "Depurar";
            this.MenuDebug.Click += new EventHandler(this.MenuDebug_Click);
            this.MenuIniciar.Name = "MenuIniciar";
            this.MenuIniciar.Size = new Size(126, 22);
            this.MenuIniciar.Text = "Iniciar";
            this.MenuIniciar.Click += new EventHandler(this.MenuIniciar_Click);
            this.MenuPausar.Name = "MenuPausar";
            this.MenuPausar.Size = new Size(126, 22);
            this.MenuPausar.Text = "Pausar";
            this.MenuEncerrar.Name = "MenuEncerrar";
            this.MenuEncerrar.Size = new Size(126, 22);
            this.MenuEncerrar.Text = "Encerrar";
            this.MenuEncerrar.Click += new EventHandler(this.MenuEncerrar_Click);
            this.logsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.MenuVisualizar
            });
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            this.logsToolStripMenuItem.Size = new Size(41, 20);
            this.logsToolStripMenuItem.Text = "Logs";
            this.MenuVisualizar.Name = "MenuVisualizar";
            this.MenuVisualizar.Size = new Size(129, 22);
            this.MenuVisualizar.Text = "Visualizar";
            this.MenuVisualizar.Click += new EventHandler(this.MenuVisualizar_Click);
            this.statBar.Location = new Point(0, 295);
            this.statBar.Name = "statBar";
            this.statBar.Size = new Size(561, 22);
            this.statBar.TabIndex = 2;
            this.statBar.Text = "ConveyISO ";
            this.timer1.Enabled = true;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            this.progressBar1.Dock = DockStyle.Bottom;
            this.progressBar1.Location = new Point(0, 280);
            this.progressBar1.MarqueeAnimationSpeed = 10;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(561, 15);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 3;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(561, 317);
            this.Controls.Add((Control)this.progressBar1);
            this.Controls.Add((Control)this.statBar);
            this.Controls.Add((Control)this.logTela);
            this.Controls.Add((Control)this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "ConveyISO - Roteamento de mensagens ISO8583";
            this.Load += new EventHandler(this.Form1_Load);
            this.FormClosing += new FormClosingEventHandler(this.frmMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private delegate void AtualizaTextoCallBack(string t);
    }
}

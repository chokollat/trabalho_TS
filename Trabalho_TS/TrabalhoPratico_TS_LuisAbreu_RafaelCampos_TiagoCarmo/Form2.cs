using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EI.SI;
using static System.Windows.Forms.DataFormats;

namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class Form2 : Form
    {
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private string username;
        private Thread listenerThread;

        public Form2(TcpClient client, NetworkStream networkStream, string username)
        {
            InitializeComponent();
            this.client = client;
            this.networkStream = networkStream;
            this.username = username;
            this.protocolSI = new ProtocolSI();

            listenerThread = new Thread(ListenForMessages);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        private void ListenForMessages()
        {
            try
            {
                while (true)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    ProtocolSICmdType cmd = protocolSI.GetCmdType();

                    if (cmd == ProtocolSICmdType.DATA)
                    {
                        string mensagem = protocolSI.GetStringFromData();

                        Invoke(new Action(() => txtChat.AppendText(mensagem + Environment.NewLine)));
                    }
                    else if (cmd == ProtocolSICmdType.EOT)
                    {
                        break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ligação terminada.");
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            byte[] packet = protocolSI.Make(ProtocolSICmdType.EOT);
            networkStream.Write(packet, 0, packet.Length);
            client.Close();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            string mensagem = txtMensagem.Text.Trim();
            if (!string.IsNullOrEmpty(mensagem))
            {
                string mensagemCifrada = AESCrypto.Encrypt(mensagem);

                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mensagem);
                networkStream.Write(packet, 0, packet.Length);
                txtMensagem.Clear();

                txtChat.AppendText("Tu: " + mensagem + Environment.NewLine);
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }


}

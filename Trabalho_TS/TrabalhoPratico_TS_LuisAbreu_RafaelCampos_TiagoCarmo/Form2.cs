using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EI.SI;

namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class Form2 : Form
    {
        // Variáveis de comunicação
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

            // Começa a escutar mensagens do servidor em background
            listenerThread = new Thread(ListenForMessages);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // Escuta mensagens do servidor
        private void ListenForMessages()
        {
            try
            {
                while (true)
                {
                    // Lê os dados da rede
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    ProtocolSICmdType cmd = protocolSI.GetCmdType();

                    // Se for uma mensagem
                    if (cmd == ProtocolSICmdType.DATA)
                    {
                        string mensagemCifrada = protocolSI.GetStringFromData();
                        string mensagem = AESCrypto.Decrypt(mensagemCifrada); // Desencripta

                        // Mostra a mensagem no chat
                        Invoke(new Action(() => txtChat.AppendText(mensagem + Environment.NewLine)));
                    }
                    // Se for comando para terminar
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

        // Quando o form é fechado
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
                // Encripta e envia
                string mensagemCifrada = AESCrypto.Encrypt(mensagem);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mensagemCifrada);
                networkStream.Write(packet, 0, packet.Length);
                txtMensagem.Clear();

                // Mostra no chat do próprio utilizador
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

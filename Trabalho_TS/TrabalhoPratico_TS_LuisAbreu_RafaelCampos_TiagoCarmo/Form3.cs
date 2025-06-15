using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Sockets;
using EI.SI;

namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class Form3 : Form
    {
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private Aes aes = Aes.Create();

        public Form3()
        {
            InitializeComponent();
        }
        public Form3(TcpClient client, NetworkStream stream, ProtocolSI proto, Aes aesObject)
        {
            InitializeComponent();
            this.client = client;
            this.networkStream = stream;
            this.protocolSI = proto;
            this.aes = aesObject;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtRegistoUsername.Text.Trim();
            string password = txtRegistoPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Preenche todos os campos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Concatenar dados
                string dados = username + "+" + password;

                // Cifrar
                string dadosCifrados = CifrarTexto(dados);

                
                if (client == null || !client.Connected)
                {
                    MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, dadosCifrados);
                networkStream.Write(packet, 0, packet.Length);

                MessageBox.Show("Registo enviado com sucesso!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar registo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CifrarTexto(string texto)
        {
            byte[] textoBytes = Encoding.UTF8.GetBytes(texto);
            byte[] textoCifrado;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(textoBytes, 0, textoBytes.Length);
                    cs.Close();
                }
                textoCifrado = ms.ToArray();
            }

            return Convert.ToBase64String(textoCifrado);
        }
    }
}
 
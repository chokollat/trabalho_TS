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
        private Aes aes;

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

            // AES com chave e vetor IV fixos para manter consistência com o servidor
            aes = aesObject;
            aes.Key = Encoding.UTF8.GetBytes("1234567890123456");
            aes.IV = Encoding.UTF8.GetBytes("6543210987654321");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ler inputs
            string username = txtRegistoUsername.Text.Trim();
            string password = txtRegistoPassword.Text;

            // Validação básica dos campos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Preenche todos os campos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Concatenar dados para envio
                string dados = username + "+" + password;

                // Cifrar os dados com AES
                string dadosCifrados = CifrarTexto(dados);

                // Validar conexão TCP
                if (client == null || !client.Connected)
                {
                    MessageBox.Show("Client is not connected.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Criar e enviar o pacote cifrado ao servidor com o comando USER_OPTION_1 (registo)
                byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, dadosCifrados);
                networkStream.Write(packet, 0, packet.Length);

                // Feedback ao utilizador
                MessageBox.Show("Registo enviado com sucesso!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Em caso de erro na operação
                MessageBox.Show("Erro ao enviar registo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Função para cifrar texto com AES
        private string CifrarTexto(string texto)
        {
            byte[] textoBytes = Encoding.UTF8.GetBytes(texto);
            byte[] textoCifrado;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(textoBytes, 0, textoBytes.Length);
                    cs.Close(); // fecha o stream e finaliza a cifragem
                }
                textoCifrado = ms.ToArray(); // obtém o array cifrado do MemoryStream
            }

            // Codificar para Base64 para envio seguro via protocolo
            return Convert.ToBase64String(textoCifrado);
        }
    }
}

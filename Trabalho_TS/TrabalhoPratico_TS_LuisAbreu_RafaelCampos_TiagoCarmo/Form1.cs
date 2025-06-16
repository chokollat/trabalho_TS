using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using EI.SI;



namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class TapSend : Form
    {
        private const int SALT_SIZE = 8;
        private const int NUMBER_OF_ITERATIONS = 1000;
        private const int PORT = 10000;
        private RSACryptoServiceProvider rsa;
        private AesCryptoServiceProvider aes;

        ProtocolSI protocolSI;
        NetworkStream networkStream;
        TcpClient client;



        public TapSend()
        {
            InitializeComponent();

            rsa = new RSACryptoServiceProvider();
            aes = new AesCryptoServiceProvider();

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            protocolSI = new ProtocolSI();
            client = new TcpClient();
            client.Connect(endpoint);
            networkStream = client.GetStream();

        }

        // Gera um salt aleatório
        private static byte[] GenerateSalt(int size)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] buff = new byte[size];
                rng.GetBytes(buff);
                return buff;
            }
        }

        // Gera hash usando Rfc2898 com salt e iterações
        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS))
            {
                return rfc2898.GetBytes(32); // tamanho do hash
            }
        }

        // Função para registrar usuário na base


        // Evento do botão Registrar


        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, username + '+' + password);
            networkStream.Write(packet, 0, packet.Length);

        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preenche todos os campos!");
                return;
            }

            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 10000);
                NetworkStream networkStream = client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();

                string dados = username + "+" + password;
                string dadosCifrados = AESCrypto.Encrypt(dados); // Usa a classe AESCrypto.cs

                byte[] loginPacket = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, dadosCifrados);
                networkStream.Write(loginPacket, 0, loginPacket.Length);

                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                ProtocolSICmdType resposta = protocolSI.GetCmdType();
                string respostaServidor = protocolSI.GetStringFromData();

                if (resposta == ProtocolSICmdType.DATA && respostaServidor == "logado")
                {
                    MessageBox.Show("Login bem-sucedido!");
                    Form2 chatForm = new Form2(client, networkStream, username);
                    chatForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Credenciais inválidas.");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao comunicar com o servidor: " + ex.Message);
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}

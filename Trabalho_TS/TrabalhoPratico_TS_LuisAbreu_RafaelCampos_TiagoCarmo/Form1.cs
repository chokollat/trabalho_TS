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
      

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, username +'+'+ password);
            networkStream.Write(packet, 0, packet.Length);

            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
            {
                string ResultadoLogin = (protocolSI.GetStringFromData());
                if (ResultadoLogin == "logado")
                {
                    MessageBox.Show("Logado Com Sucesso");
       
                }
                else if (ResultadoLogin == "deslogado")
                {
                    MessageBox.Show("Logado sem Sucesso");
                }
            }

        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, username+'+'+password);
            networkStream.Write(packet, 0, packet.Length);

        }

    }
}

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using EI.SI;
using System.Security.Cryptography;


namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class TapSend : Form
    {
        private Aes aes = Aes.Create();
        private const string AES_KEY = "1234567890123456";
        private const string AES_IV = "6543210987654321";

        public TapSend()
        {
            InitializeComponent();
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
            aes.IV = Encoding.UTF8.GetBytes(AES_IV);
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    TcpClient client = new TcpClient("127.0.0.1", 10000);
                    NetworkStream networkStream = client.GetStream();
                    ProtocolSI protocolSI = new ProtocolSI();

                    string dados = username + "+" + password;
                    string dadosCifrados = CifrarTexto(dados);

                    byte[] loginPacket = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, dadosCifrados);
                    networkStream.Write(loginPacket, 0, loginPacket.Length);

                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    ProtocolSICmdType resposta = protocolSI.GetCmdType();

                    if (resposta == ProtocolSICmdType.ACK && protocolSI.GetStringFromData() == "LOGIN_SUCESSO")
                    {
                        Form2 formPrincipal = new Form2();
                        formPrincipal.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Login falhou. Verifica o username ou password.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro na ligação com o servidor: " + ex.Message, "Erro de rede", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Preenche os campos necessários, User Name & Password.", "Erro de Login!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_registro_Click(object sender, EventArgs e)
        {
            try
            {
                // Cria e conecta o cliente TCP ao servidor
                TcpClient client = new TcpClient("127.0.0.1", 10000);
                NetworkStream stream = client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                Aes aes = Aes.Create();

                // Define chave e vetor IV iguais aos do servidor
                aes.Key = Encoding.UTF8.GetBytes("1234567890123456");
                aes.IV = Encoding.UTF8.GetBytes("6543210987654321");

                // Abre o formulário de registo (Form3), passando os objetos necessários
                Form3 formRegisto = new Form3(client, stream, protocolSI, aes);
                formRegisto.Show();

                // Opcional: esconder o formulário atual (Form1)
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

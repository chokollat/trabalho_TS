using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using EI.SI;    
using System.Security.Cryptography;
using System.IO;


namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class TapSend : Form
    {

        public TapSend()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    TcpClient client = new TcpClient("127.0.0.1", 10000); // muda IP/porta se for diferente
                    NetworkStream networkStream = client.GetStream();
                    ProtocolSI protocolSI = new ProtocolSI();

                    // Enviar pedido de login
                    string dados = username + "+" + password;
                    byte[] loginPacket = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, dados);
                    networkStream.Write(loginPacket, 0, loginPacket.Length);

                    // Esperar resposta
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
                // Cria e conecta o cliente TCP no IP e porta do servidor
                TcpClient client = new TcpClient("127.0.0.1", 10000);
                NetworkStream stream = client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                Aes aes = Aes.Create();

                // Cria o Form3 (registro) passando os objetos necessários
                Form3 formRegisto = new Form3(client, stream, protocolSI, aes);
                formRegisto.Show();

                // Opcional: esconder este form se quiser
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor: " + ex.Message);
            }
        }
    }
}

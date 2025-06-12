using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using EI.SI;
using Microsoft.Data.SqlClient;


namespace Consola_Server
{
    class Program
    {
        private const int PORT = 10000;
        private string publickey;
        private static int clientes_counter = 0;
        /*public static List<ClientHandler> clientes = new List<ClientHandler>();*/
        public static readonly object lockObj = new object();

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endPoint);

            listener.Start();
            Console.WriteLine("The server is READY!!");
            int clientes_counter = 0;

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientes_counter++;
                Console.WriteLine("Client {0} connected", clientes_counter);
                ClientHandler clientHandler = new ClientHandler(client, clientes_counter);

                lock (lockObj)
                {
                    /*clientes.Add(clientHandler);*/
                }

                clientHandler.Handle();
            }
        }

        class ClientHandler
        {
            private TcpClient client;
            private int clientID;
            private const int SALTSIZE = 8;
            private const int NUMBER_OF_ITERATIONS = 1000;
            private AesCryptoServiceProvider aes;
            private string pk;
            private string iv;

            public ClientHandler(TcpClient client, int clientID)
            {
                this.client = client;
                this.clientID = clientID;
                this.aes = new AesCryptoServiceProvider();
            }

            public void Handle()
            {
                Thread thread = new Thread(threadHandler);
                thread.Start();
            }

            private void threadHandler()
            {
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocoloSI = new ProtocolSI();

                while (protocoloSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);
                    byte[] ack;
                    switch (protocoloSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            // ESCREVER MENSAGEM DO CLIENTE
                            string mensagemRecebida = protocoloSI.GetStringFromData();
                            Console.WriteLine("Client " + clientID + ": " + mensagemRecebida);

                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);

                            lock (Program.lockObj)
                            {
                                /*foreach (var clientes in Program.clientes)
                                {
                                    if (clientes != this)
                                    {
                                        clientes.MandarMensagem("Cliente " + clientID + ": " + mensagemRecebida);
                                    }
                                }*/
                            }

                            break;

                        case ProtocolSICmdType.EOT:
                            // CASO O CLIENTE ENVIO EOT (FIM DE TRANSMISSAO)
                            Console.WriteLine("Ending Thread from Client {0}", clientID);
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;

                        case ProtocolSICmdType.USER_OPTION_1:
                            string dadosCifrados = protocoloSI.GetStringFromData();
                            // AES decrypt ainda não está feito, mas deverá ser aqui
                            string dadosDecifrados = DecifrarTexto(dadosCifrados);
                            Console.WriteLine("Registo recebido do cliente " + clientID + ": " + dadosDecifrados);

                            // Dividir username e password
                            string[] partes = dadosDecifrados.Split('+');
                            if (partes.Length == 2)
                            {
                                string username = partes[0];
                                string password = partes[1];
                                GuardarNaBaseDeDados(username, password);
                            }

                            break;
                    }
                }
            }

            private void MandarMensagem(string mensagemenviada)
            {
                try
                {
                    ProtocolSI protocolSI = new ProtocolSI();
                    NetworkStream ns = client.GetStream();
                    byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mensagemenviada);
                    ns.Write(packet, 0, packet.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao enviar para cliente " + clientID + ": " + ex.Message);
                }
            }

            private string DecifrarTexto(string textoCifrado)
            {
                byte[] textoBytes = Convert.FromBase64String(textoCifrado);
                string textoDecifrado = "";

                using (MemoryStream ms = new MemoryStream(textoBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
                        {
                            textoDecifrado = sr.ReadToEnd();
                        }
                    }
                }

                return textoDecifrado;
            }



            private void GuardarNaBaseDeDados(string username, string password)
            {

                // Gerar salt
                byte[] salt = new byte[SALTSIZE];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Gerar hash com salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordComSalt = passwordBytes.Concat(salt).ToArray();
                byte[] hash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    hash = sha256.ComputeHash(passwordComSalt);
                }

                string saltBase64 = Convert.ToBase64String(salt);
                string hashBase64 = Convert.ToBase64String(hash);

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\USER\DOCUMENTS\GIT\TRABALHO_TS\TRABALHO_TS\TRABALHOPRATICO_TS_LUISABREU_RAFAELCAMPOS_TIAGOCARMO\DB'S\Database1.mdf;Integrated Security=True";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Utilizadores (Username, PasswordHash, Salt) VALUES (@username, @hash, @salt)";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@hash", hashBase64);
                            cmd.Parameters.AddWithValue("@salt", saltBase64);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Registo inserido com sucesso na base de dados.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao guardar na base de dados: " + ex.Message);
                }
            }
        }
    }
}

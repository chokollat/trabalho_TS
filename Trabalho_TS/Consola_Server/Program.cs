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
        private static int clientes_counter = 0;
        public static List<ClientHandler> clientes = new List<ClientHandler>();
        public static readonly object lockObj = new object(); // Proteção para acesso

        static void Main(string[] args)
        {
            // Cria um servidor Tcp a escuta na porta 10000
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endPoint);

            listener.Start();
            Console.WriteLine("The server is READY!!");

            while (true)
            {
                // Espera por cliente
                TcpClient client = listener.AcceptTcpClient();
                clientes_counter++;
                Console.WriteLine("Client {0} connected", clientes_counter);

                // Handler para o cliente
                ClientHandler clientHandler = new ClientHandler(client, clientes_counter);

                // Adiciona o cliente à lista protegida
                lock (lockObj)
                {
                    clientes.Add(clientHandler);
                }

                // Inicia comunicação com o cliente
                clientHandler.Handle();
            }
        }

        public class ClientHandler
        {
            private TcpClient client;
            private int clientID;
            private AesCryptoServiceProvider aes;

            public ClientHandler(TcpClient client, int clientID)
            {
                this.client = client;
                this.clientID = clientID;

                // Chave e vetor de IV fixos
                this.aes = new AesCryptoServiceProvider();
                this.aes.Key = Encoding.UTF8.GetBytes("1234567890123456");
                this.aes.IV = Encoding.UTF8.GetBytes("6543210987654321");
            }

            // Cria uma nova thread para este cliente
            public void Handle()
            {
                Thread thread = new Thread(threadHandler);
                thread.Start();
            }

            private void threadHandler()
            {
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocoloSI = new ProtocolSI();
                bool clienteAutenticado = false;

                while (protocoloSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    try
                    {
                        int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine($"Cliente {clientID} desconectado.");
                            return;
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Erro na ligação com cliente {clientID}");
                        return;
                    }

                    byte[] ack;

                    switch (protocoloSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            if (!clienteAutenticado)
                            {
                                MandarMensagem("ERRO: Você precisa estar autenticado para mandar mensagens.");
                                break;
                            }

                            // Mensagem recebida do cliente
                            string mensagemRecebida = protocoloSI.GetStringFromData();
                            Console.WriteLine("Client " + clientID + ": " + mensagemRecebida);

                            // Responde com ACK
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);

                            // Envia a mensagem para os outros clientes
                            lock (Program.lockObj)
                            {
                                foreach (var clientes in Program.clientes)
                                {
                                    if (clientes != this)
                                    {
                                        clientes.MandarMensagem("Cliente " + clientID + ": " + mensagemRecebida);
                                    }
                                }
                            }
                            break;

                        case ProtocolSICmdType.USER_OPTION_1:  // REGISTO
                            string dadosCifrados = protocoloSI.GetStringFromData();
                            Console.WriteLine("Registo do cliente " + clientID + ": " + dadosCifrados);

                            string[] partes = dadosCifrados.Split('+');
                            string username = partes[0];
                            string password = partes[1];

                            byte[] salt = GenerateSalt(8);
                            byte[] hash = GenerateSaltedHash(password, salt);

                            Register(username, hash, salt);
                            break;

                        case ProtocolSICmdType.USER_OPTION_2:  // LOGIN
                            string dadosLoginCifrados = protocoloSI.GetStringFromData();
                            string dadosLoginDecifrados = ClientHandler.AESCrypto.Decrypt(dadosLoginCifrados);

                            string[] partesLogin = dadosLoginDecifrados.Split('+');
                            string usernameLogin = partesLogin[0];
                            string passwordLogin = partesLogin[1];

                            if (VerificarCredenciais(usernameLogin, passwordLogin))
                            {
                                clienteAutenticado = true;
                                MandarMensagem("logado");
                            }
                            else
                            {
                                MandarMensagem("deslogado");
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Cliente {0} desconectado.", clientID);
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            client.Close();
                            return;
                    }
                }
            }

            // Envia mensagem para o cliente
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

            // Função para registar novo utilizador na base de dados
            private void Register(string username, byte[] saltedPasswordHash, byte[] salt)
            {
                try
                {
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = "connection_string_aqui";
                    conn.Open();

                    string checkUserSql = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    SqlCommand checkCmd = new SqlCommand(checkUserSql, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);

                    int userExists = (int)checkCmd.ExecuteScalar();
                    if (userExists > 0)
                    {
                        MandarMensagem("erro: user já existe");
                        return;
                    }

                    // Inserir utilizador novo
                    string sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt) VALUES (@username,@saltedPasswordHash,@salt)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@saltedPasswordHash", saltedPasswordHash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MandarMensagem("user inserido com sucesso");
                }
                catch (Exception e)
                {
                    throw new Exception("Erro ao inserir utilizador: " + e.Message);
                }
            }

            // Verifica se o username e password
            private bool VerificarCredenciais(string username, string password)
            {
                try
                {
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = "connection_string_aqui";
                    conn.Open();

                    string sql = "SELECT * FROM Users WHERE Username = @username";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return false;

                    reader.Read();
                    byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];
                    byte[] saltStored = (byte[])reader["Salt"];
                    conn.Close();

                    byte[] hash = GenerateSaltedHash(password, saltStored);
                    return saltedPasswordHashStored.SequenceEqual(hash);
                }
                catch
                {
                    return false;
                }
            }

            // Gera salt com um valor aleatorio
            private static byte[] GenerateSalt(int size)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buff = new byte[size];
                rng.GetBytes(buff);
                return buff;
            }

            // Gera hash da password com o salt
            private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, 1000);
                return rfc2898.GetBytes(32);
            }

            // Classe para encriptar e desencriptar com AES
            public static class AESCrypto
            {
                private static readonly byte[] key = Encoding.UTF8.GetBytes("1234567890123456");
                private static readonly byte[] iv = Encoding.UTF8.GetBytes("6543210987654321");

                public static string Encrypt(string plainText)
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = key;
                        aesAlg.IV = iv;
                        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                            sw.Close();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }

                public static string Decrypt(string cipherText)
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = key;
                        aesAlg.IV = iv;
                        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                        using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

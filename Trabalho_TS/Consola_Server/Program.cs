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
        public static List<ClientHandler> clientes = new List<ClientHandler>();
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
                    clientes.Add(clientHandler);
                }

                clientHandler.Handle();
            }
        }

        public class ClientHandler
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
                this.aes.Key = Encoding.UTF8.GetBytes("1234567890123456"); 
                this.aes.IV = Encoding.UTF8.GetBytes("6543210987654321");  
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
                bool clienteAutenticado = false;  // <- Novo: só manda mensagens se logado

                while (protocoloSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    byte[] buffer = new byte[1024];
                    try
                    {
                        int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine($"Cliente {clientID} desconectado.");
                            return; // Fecha a thread deste cliente
                        }
                    }
                    catch (IOException ioEx)
                    {
                        Console.WriteLine($"[IO ERROR] Cliente {clientID} - {ioEx.Message}");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERRO GERAL] Cliente {clientID} - {ex.Message}");
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

                            // Mensagem do cliente
                            string mensagemRecebida = protocoloSI.GetStringFromData();
                            Console.WriteLine("Client " + clientID + ": " + mensagemRecebida);

                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);

                            // Reenviar a outros clientes
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

                        case ProtocolSICmdType.USER_OPTION_1:  // Registro
                            string dadosCifrados = protocoloSI.GetStringFromData();
                  
                            Console.WriteLine("Registo recebido do cliente " + clientID + ": " + dadosCifrados);

                            string[] partes = dadosCifrados.Split('+');
                            string username = partes[0];
                            string password = partes[1];
                            byte[] salt = GenerateSalt(SALTSIZE);
                            byte[] hash = GenerateSaltedHash(password, salt);
                            Register(username, hash, salt);
                            break;

                        case ProtocolSICmdType.USER_OPTION_2:  // Login
                            string dadosLoginCifrados = protocoloSI.GetStringFromData();
                            Console.WriteLine("Tentativa de login do cliente " + clientID + ": " + dadosLoginCifrados);

                            // Desencriptar os dados recebidos
                            string dadosLoginDecifrados = ClientHandler.AESCrypto.Decrypt(dadosLoginCifrados);

                            Console.WriteLine("Tentativa de login do cliente " + clientID + ": " + dadosLoginDecifrados);

                            string[] partesLogin = dadosLoginDecifrados.Split('+');
                            string usernameLogin = partesLogin[0];
                            string passwordLogin = partesLogin[1];

                            if (VerificarCredenciais(usernameLogin, passwordLogin))
                            {
                                clienteAutenticado = true;
                                Console.WriteLine("Login bem-sucedido do cliente " + clientID);
                                MandarMensagem("logado");
                            }
                            else
                            {
                                Console.WriteLine("Login FALHOU do cliente " + clientID);
                                MandarMensagem("deslogado");
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Ending Thread from Client {0}", clientID);
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            client.Close(); 
                            return;         
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



            private void Register(string username, byte[] saltedPasswordHash, byte[] salt)
            {
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection();
                    conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\User\Documents\GIT\trabalho_TS\Trabalho_TS\TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo\DB\Dbuser.mdf';Integrated Security=True");
                    conn.Open();


                    string checkUserSql = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    SqlCommand checkCmd = new SqlCommand(checkUserSql, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);

                    int userExists = (int)checkCmd.ExecuteScalar();
                    if (userExists > 0)
                    {
                        Console.WriteLine("Utilizador já existe.");

                        MandarMensagem("erro: user já existe");
                        return;
                    }

                    // Se não existe, insere
                    string sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt) VALUES (@username,@saltedPasswordHash,@salt)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@saltedPasswordHash", saltedPasswordHash);
                    cmd.Parameters.AddWithValue("@salt", salt);

                    int lines = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (lines == 0)
                    {
                        throw new Exception("Error while inserting user");
                    }

                    MandarMensagem("user inserido com sucesso");
                    Console.WriteLine("Inserido"); 
                }
                catch (Exception e)
                {

                    throw new Exception("Erro ao inserir utilizador: " + e.Message);
                }
            }

            private bool VerificarCredenciais(string username, string password)
            {
                SqlConnection conn = null;
                try
                {
                    // Configurar ligação à Base de Dados
                    conn = new SqlConnection();
                    conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\User\Documents\GIT\trabalho_TS\Trabalho_TS\TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo\DB\Dbuser.mdf';Integrated Security=True");

                    // Abrir ligação à Base de Dados
                    conn.Open();

                    // Declaração do comando SQL
                    String sql = "SELECT * FROM Users WHERE Username = @username";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;

                    // Declaração dos parâmetros do comando SQL
                    SqlParameter param = new SqlParameter("@username", username);

                    // Introduzir valor ao parâmentro registado no comando SQL
                    cmd.Parameters.Add(param);

                    // Associar ligação à Base de Dados ao comando a ser executado
                    cmd.Connection = conn;

                    // Executar comando SQL
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {

                        throw new Exception("Error while trying to access an user");
                    }

                    // Ler resultado da pesquisa
                    reader.Read();

                    // Obter Hash (password + salt)
                    byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];

                    // Obter salt
                    byte[] saltStored = (byte[])reader["Salt"];

                    conn.Close();

                    //TODO: verificar se a password na base de dados 
                    byte[] hash = GenerateSaltedHash(password, saltStored);

                    return saltedPasswordHashStored.SequenceEqual(hash);

                    throw new NotImplementedException();
                }
                catch (Exception e)
                {
                    //MessageBox.Show("An error occurred: " + e.Message);
                    return false;
                }

            }

            private static byte[] GenerateSalt(int size)
            {
                //Generate a cryptographic random number.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buff = new byte[size];
                rng.GetBytes(buff);
                return buff;
            }
            private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
                return rfc2898.GetBytes(32);
            }

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

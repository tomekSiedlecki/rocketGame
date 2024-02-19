using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class CredentialsLogic
    {
        static public void Register()
        {
            using (var context = new Context())
            {
                Console.WriteLine("Rejestracja");

                //login
                User tmp;
                string login;
                do
                {
                    Console.WriteLine("Podaj login: ");
                    login = Console.ReadLine();
                    tmp = context.Users.FirstOrDefault(x => x.Login == login);
                    Console.Clear();
                } while (tmp != null);

                //password
                string password;
                string passwordAgain;
                do
                {
                    Console.WriteLine("Podaj haslo: ");
                    password = GetConsolePassword();
                    Console.WriteLine("Powtorz haslo: ");
                    passwordAgain = GetConsolePassword();
                    Console.Clear();
                } while (!passwordsOk(password, passwordAgain));

                byte[] salt;
                string hash = HashPasword(password, out salt);

                User newUser = new User();
                newUser.Login = login;
                newUser.Password = hash;
                newUser.Salt = salt;
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        static private bool passwordsOk(string password1, string password2)
        {
            bool result = true;
            if (password1 != password2)
            {
                result = false;
            }
            if(password1 == null)
            {
                result = false;
            }
            return result;
        }
        public static string GetConsolePassword()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('*');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        static public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
            salt,
                iterations,
                HashAlgorithmName.SHA512,
                keySize);
            return Convert.ToHexString(hash);
        }

        static public User Login()
        {
            Console.WriteLine("Logowanie");
            string login;
            string password;
            User result = new User();
            do
            {
                Console.WriteLine("Podaj Login: ");
                login = Console.ReadLine();
                Console.WriteLine("Podaj haslo: ");
                password = GetConsolePassword();
                Console.Clear();
            }
            while (!isLogingCompleted(login, password, ref result));
            return result;
        }

        static private bool isLogingCompleted(string login, string password,ref User tmp)
        {
            using (var context = new Context())
            {
                tmp = context.Users.FirstOrDefault(x => x.Login == login);
                if(tmp == null)
                {
                    return false;
                }
                if(VerifyPassword(password,tmp.Password,tmp.Salt))
                {
                    return true;
                }
                return false;
            }
        }


        static public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

    }
}

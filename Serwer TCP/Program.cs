using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Net.NetworkInformation;
using System.Threading;

namespace Serwer_TCP
{
    class Program
    {

        Socket sck;
        Thread[] tabThread = new Thread[5];
        String port;

        static void Main(string[] args)
        {

            Program program = new Program();
            program.init(program);
            program.initThread(program);
            for (int i = 0; i < 5; i++)
            {
                program.tabThread[i].Start();
            }
            for (int i = 0; i < 5; i++)
            {
                program.tabThread[i].Join();
            }




        }

        public void init(Program program)
        {
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //socket   
            port = program.getPort();
            program.SetBind(Int32.Parse(port), sck);
        }

        public void initThread(Program program)
        {
            for (int i = 0; i < 5; i++)
            {

                tabThread[i] = new Thread(() => newSocket(program));
            }

        }


        private void newSocket(Program program)
        {
             //BIND - ustalamy adres i port

            sck.Listen(5);//maksymalna ilosc polaczen 

            Socket accepted = program.SetAccept(sck); // gniazdo do nasłuchu
            while (true)
            {
                try
                {
                    //tworzy tablicę o takim rozmiarze jak rozmiar buforu danych 
                    byte[] Buffer = new byte[accepted.SendBufferSize];
                    int bytesRead = accepted.Receive(Buffer);
                    if (bytesRead == 0) break;
                    Console.WriteLine("\n\nPolaczenie z " + accepted.RemoteEndPoint);
                    Console.Write("Dane odebrane: " + Encoding.ASCII.GetString(Buffer, 0, bytesRead));

                    byte[] formatted = new byte[bytesRead]; //nowa tablica na wyslane dane

                    for (int i = 0; i < bytesRead; i++)
                        formatted[i] = Buffer[i];

                    Console.Write("\nDane Wyslane: " + Encoding.ASCII.GetString(formatted) + "\r\n\n");
                    accepted.Send(formatted);
                }
                catch (SocketException)
                {
                    Console.Write("\nPolaczenie zostalo zerwane przez klienta. \nWcisnij ESC zeby zamknac program, lub dowolny klawisz aby kontynuowac dzialanie.\n\n");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        sck.Close();
                        break;
                    }

                    else
                    {
                        //Main(args);
                    }
                }


            }
            //zamknięcie gniazda
            sck.Close();
        }
        private String getPort()
        {
            Console.Write("Podaj numer portu: ");      //podawanie portu
            String port = Console.ReadLine();
            int validPort = 0;

            //Sprawdzanie czy port jest poprawny;

            bool portValue = isValidPort(port);

            while (!portValue)
            {
                Console.Write("\nPodales zly numer portu.\n");
                Console.Write("Podaj numer portu: ");
                port = Console.ReadLine();
                portValue = isValidPort(port);

            }
            return port;
        }


        private bool isValidPort(String port)
        {
            int validPort;
            bool isValidPort = int.TryParse(port, out validPort);
            if (validPort < 0 || validPort > 65535)
            {
                isValidPort = false;
            }
            return isValidPort;
        }

        public void SetBind(int port, Socket sck)
        {
            try
            {
                sck.Bind(new IPEndPoint(0, port));
                Console.Write("Serwer zostal uruchomiony!\n\n");

            }
            catch (SocketException e)
            {
                Console.Write("Exception Bind SocketException");
                ExceptionService();
            }
            catch (SecurityException e)
            {
                Console.Write("Exception Bind SecurityException");
                ExceptionService();
            }
            catch (Exception e)
            {
                Console.Write("Exception Bind ");
                throw e;
            }
        }

        public static void ExceptionService()
        {
            Console.ReadKey();
            Environment.Exit(0);
        }

        public Socket SetAccept(Socket sck)
        {
            try
            {
                Socket accepted = sck.Accept();
                return accepted;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Exception Accept InvalidOperationException");
                ExceptionService();
            }
            catch (Exception e)
            {
                Console.Write("Exception Accept ");
                throw e;
            }
            return null;
        }


    }

}

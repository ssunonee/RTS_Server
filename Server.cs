using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RTS_Server.Models;

namespace RTS_Server
{
    class Server
    {
        Thread send_thread;
        Thread receive_thread;

        public Server()
        {
            send_thread = new Thread(new ThreadStart(SendThread));
            send_thread.Start();
            receive_thread = new Thread(new ThreadStart(ReceiveThread));
            receive_thread.Start();
        } 

        public void SendThread()
        {
            Socket listener = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(localAddr, 7331));
                listener.Listen(10);

                while (true)
                {
                    Socket handler = listener.Accept();
                    var unit_infos = new List<UnitDTO>();
                    foreach (var unit in Playground.instance.units)
                    {
                        unit_infos.Add(UnitDTO.ToDTO(unit));
                    }

                    var dto = new PlaygroundDTO()
                    {
                        Size = Playground.instance.size.X,
                        UnitInfos = unit_infos
                    };
                    string response = JsonConvert.SerializeObject(dto);

                    byte[] data = Encoding.ASCII.GetBytes(response);
                    
                    handler.Send(data);
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Close();
                if (receive_thread != null)
                    receive_thread.Abort();
            }
        }

        public void ReceiveThread()
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 7332);
 
            Socket rec_listener = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.Tcp);

            rec_listener.Bind(ipEndPoint);
            rec_listener.Listen(10);

            while (true)
            {
                Socket handler = rec_listener.Accept();
                string data = null;
                while (true)
                {
                    byte[] bytes = new byte[1024];

                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    if (data.IndexOf("<theend>") > -1)
                    {
                        break;
                    }
                }

                string message = data.Substring(0, data.Length - 8);
                var command = JsonConvert.DeserializeObject<CommandDTO>(message);

                Playground.instance.SetTarget(command.Pos, command.Ids);

                Thread.Sleep(1000 / 30);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}

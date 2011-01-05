using System;
using System.Text;
using System.Threading;
using ZMQ;
using System.Collections;
using System.Collections.Generic;

namespace Kiss.Events
{
    public delegate string ResponseAction(string msg);

    public static class Broker
    {
        static readonly Context context = new Context(1);

        public static bool Push(string addr, string message)
        {
            return Push(addr, message, 1000);
        }

        public static bool Push(string addr, string message, int timeout)
        {
            using (Socket skt = context.Socket(SocketType.REQ))
            {
                skt.Connect(addr);

                PollItem[] items = new PollItem[1];
                items[0] = skt.CreatePollItem(IOMultiPlex.POLLIN);
                items[0].PollInHandler += delegate(Socket s, IOMultiPlex revents) { s.Recv(); };

                skt.Send(message, Encoding.UTF8);

                DateTime dt = DateTime.Now;

                int rt = context.Poll(items, 1000 * timeout);

                // fix timeout bug of zmq 2.0.10
                while (rt == 0 && (DateTime.Now - dt).TotalMilliseconds < timeout)
                {
                    dt = DateTime.Now;
                    rt = context.Poll(items, 1000 * timeout);
                }

                return rt > 0;
            }
        }

        public static void DoEvent(string addr, int max_concurrent, Action<string> action)
        {
            using (Context context = new Context(1))
            {
                using (Socket clients = context.Socket(SocketType.REP),
                        workers = context.Socket(SocketType.PUSH))
                {
                    //  Socket to talk to clients
                    clients.Bind(addr);
                    //  Socket to talk to workers
                    workers.Bind("inproc://worker");

                    for (int i = 0; i < max_concurrent; i++)
                    {
                        new Thread(delegate(object state)
                        {
                            //  Socket to talk to dispatcher
                            using (Socket receiver = context.Socket(SocketType.PULL))
                            {
                                receiver.Connect("inproc://worker");
                                while (true)
                                {
                                    string message = receiver.Recv(Encoding.UTF8);

                                    action.Invoke(message);
                                }
                            }
                        }).Start(context);
                    }

                    PollItem[] pollItems = new PollItem[1];

                    pollItems[0] = clients.CreatePollItem(IOMultiPlex.POLLIN);
                    pollItems[0].PollInHandler += delegate(Socket skt, IOMultiPlex revents)
                    {
                        string msg = clients.Recv(Encoding.UTF8);

                        clients.Send("OK", Encoding.UTF8);

                        workers.Send(msg, Encoding.UTF8);
                    };

                    while (true)
                    {
                        context.Poll(pollItems);

                        Thread.Sleep(1);
                    }
                }
            }
        }

        public static string Request(string addr, string msg)
        {
            return Request(addr, msg, 1000);
        }

        public static string Request(string addr, string msg, int timeout)
        {
            string result = null;

            using (Socket skt = context.Socket(SocketType.REQ))
            {
                skt.Connect(addr);

                PollItem[] items = new PollItem[1];
                items[0] = skt.CreatePollItem(IOMultiPlex.POLLIN);
                items[0].PollInHandler += delegate(Socket s, IOMultiPlex revents)
                {
                    result = s.Recv(Encoding.UTF8);
                };

                skt.Send(msg, Encoding.UTF8);

                DateTime dt = DateTime.Now;

                int rt = context.Poll(items, 1000 * timeout);

                // fix timeout bug of zmq 2.0.10
                while (rt == 0 && (DateTime.Now - dt).TotalMilliseconds < timeout)
                {
                    dt = DateTime.Now;
                    rt = context.Poll(items, 1000 * timeout);
                }

                return result;
            }
        }

        public static void DoResponse(string addr, int max_concurrent, ResponseAction action)
        {
            int available_workers = max_concurrent;

            Heartbeat ht = Heartbeat.Instance;
            ht.CurrentData.WorkAddr = addr;
            ht.CurrentData.FreeResourceCount = max_concurrent;
            ht.Start();

            using (Context context = new Context(1))
            {
                using (Socket clients = context.Socket(SocketType.XREP),
                        workers = context.Socket(SocketType.XREQ))
                {
                    //  Socket to talk to clients
                    clients.Bind(addr);
                    //  Socket to talk to workers
                    workers.Bind("inproc://responser");

                    for (int i = 0; i < max_concurrent; i++)
                    {
                        new Thread(delegate(object state)
                        {
                            using (Socket skt = ((Context)state).Socket(SocketType.REP))
                            {
                                skt.Connect("inproc://responser");

                                while (true)
                                {
                                    string msg = skt.Recv(Encoding.UTF8);

                                    DateTime dt = DateTime.Now;
                                    string response = action.Invoke(msg);
                                    ht.CurrentData.LastRunTicks = (DateTime.Now - dt).Ticks;

                                    skt.Send(response, Encoding.UTF8);
                                }
                            }
                        }).Start(context);
                    }

                    PollItem[] pollItems = new PollItem[2];

                    pollItems[0] = clients.CreatePollItem(IOMultiPlex.POLLIN);
                    pollItems[0].PollInHandler += delegate(Socket skt, IOMultiPlex revents)
                    {
                        HeartbeatData hbdata = null;

                        if (available_workers == 0)
                            hbdata = ht.GetBestNeighbor();

                        if (hbdata == null)
                            Interlocked.Decrement(ref available_workers);

                        ht.CurrentData.FreeResourceCount = available_workers;

                        while (true)
                        {
                            byte[] msg = skt.Recv(SendRecvOpt.SNDMORE);

                            if (hbdata == null)
                                workers.Send(msg, skt.RcvMore ? SendRecvOpt.SNDMORE : SendRecvOpt.NONE);
                            else
                            {
                                clients.Send(skt.RcvMore ? msg : Encoding.UTF8.GetBytes(Broker.Request(hbdata.WorkAddr, Encoding.UTF8.GetString(msg), 10000)),
                                   skt.RcvMore ? SendRecvOpt.SNDMORE : SendRecvOpt.NONE);
                            }

                            if (!skt.RcvMore) break;
                        }
                    };

                    pollItems[1] = workers.CreatePollItem(IOMultiPlex.POLLIN);
                    pollItems[1].PollInHandler += delegate(Socket skt, IOMultiPlex revents)
                    {
                        while (true)
                        {
                            byte[] msg = skt.Recv(SendRecvOpt.SNDMORE);

                            clients.Send(msg, skt.RcvMore ? SendRecvOpt.SNDMORE : SendRecvOpt.NONE);

                            if (!skt.RcvMore) break;
                        }

                        Interlocked.Increment(ref available_workers);

                        ht.CurrentData.FreeResourceCount = available_workers;
                    };

                    while (true)
                    {
                        context.Poll(pollItems);
                    }

                    //Socket.Device.Queue(clients, workers);
                }
            }
        }
    }
}

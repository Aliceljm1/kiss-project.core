using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Kiss.Config;
using Kiss.Utils;
using ZMQ;

namespace Kiss.Events
{
    public class Heartbeat
    {
        private readonly Dictionary<string, HeartbeatData> _datas = new Dictionary<string, HeartbeatData>();

        private Heartbeat()
        {
        }

        public static readonly Heartbeat Instance = new Heartbeat();
        public readonly HeartbeatData CurrentData = new HeartbeatData();

        public void Start()
        {
            HeartbeatConfig config = HeartbeatConfig.Instance;
            Start(config.Addr, config.Neighbor_addr, config.Interval);
        }

        /// <summary>
        /// start headt beat
        /// </summary>
        /// <param name="addr">address to bind</param>
        /// <param name="neighbor_addr">neighbor address to connect</param>
        /// <param name="heartbeat_interval">heartbeat interval time in second</param>
        public void Start(string addr, string neighbor_addr, int heartbeat_interval)
        {
            if (!string.IsNullOrEmpty(CurrentData.HeartbeatAddr)) throw new System.Exception("Heartbeat aleady started!");

            CurrentData.HeartbeatAddr = addr;

            if (!string.IsNullOrEmpty(neighbor_addr))
            {
                foreach (string neighbor in StringUtil.Split(neighbor_addr, StringUtil.Comma, true, true))
                {
                    _datas.Add(neighbor, new HeartbeatData() { HeartbeatAddr = neighbor });
                }
            }

            Thread server_thread = new Thread(delegate()
            {
                using (Context context = new Context(1))
                {
                    using (Socket skt = context.Socket(SocketType.REP))
                    {
                        skt.Bind(addr);

                        while (true)
                        {
                            HeartbeatData data = HeartbeatData.Create(skt.Recv(Encoding.UTF8));

                            if (data != null)
                            {
                                lock (_datas)
                                {
                                    _datas[data.HeartbeatAddr] = data;
                                }
                            }

                            skt.Send("OK", Encoding.UTF8);
                        }
                    }
                }
            });

            server_thread.Start();

            Thread client_thread = new Thread(delegate()
            {
                while (true)
                {
                    List<string> timeout_addrs = new List<string>();

                    using (Context context = new Context(1))
                    {
                        lock (_datas)
                        {
                            foreach (var neighbor in _datas.Keys)
                            {
                                using (Socket skt = context.Socket(SocketType.REQ))
                                {
                                    skt.Connect(neighbor);

                                    skt.Send(CurrentData.ToString(), Encoding.UTF8);

                                    Thread.Sleep(1);

                                    PollItem[] pollitems = new PollItem[1];

                                    pollitems[0] = skt.CreatePollItem(IOMultiPlex.POLLIN);
                                    pollitems[0].PollInHandler += delegate(Socket s, IOMultiPlex revents) { s.Recv(); };

                                    DateTime dt = DateTime.Now;

                                    int rt = context.Poll(pollitems, 1000 * 1000);

                                    // fix timeout bug of zmq 2.0.10
                                    while (rt == 0 && (DateTime.Now - dt).TotalMilliseconds < 1000)
                                    {
                                        dt = DateTime.Now;
                                        rt = context.Poll(pollitems, 1000 * 1000);
                                    }

                                    if (rt <= 0)
                                    {
                                        timeout_addrs.Add(neighbor);
                                    }
                                }
                            }
                        }

                        foreach (var item in timeout_addrs)
                        {
                            _datas.Remove(item);
                        }
                    }

                    Thread.Sleep(heartbeat_interval * 1000);
                }
            });
            client_thread.Start();
        }

        public HeartbeatData GetBestNeighbor()
        {
            List<HeartbeatData> list = new List<HeartbeatData>(_datas.Values);

            HeartbeatData d = null;

            foreach (var item in list)
            {
                if (item.FreeResourceCount > 0 && (d == null || item.LastRunTicks <= d.LastRunTicks))
                    d = item;
            }

            return d;
        }
    }

    public class HeartbeatData
    {
        public string WorkAddr { get; set; }
        public string HeartbeatAddr { get; set; }
        public long LastRunTicks { get; set; }
        public int FreeResourceCount { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", WorkAddr, HeartbeatAddr, LastRunTicks, FreeResourceCount);
        }

        public static HeartbeatData Create(string data)
        {
            HeartbeatData d = new HeartbeatData();

            string[] strs = StringUtil.Split(data, StringUtil.Comma, true, false);
            if (strs.Length != 4) return null;

            d.WorkAddr = strs[0];
            d.HeartbeatAddr = strs[1];
            d.LastRunTicks = Convert.ToInt64(strs[2]);
            d.FreeResourceCount = strs[3].ToInt();

            return d;
        }
    }

    [ConfigNode("heartbeat", UseCache = true)]
    public class HeartbeatConfig : ConfigBase
    {
        public static HeartbeatConfig Instance { get { return HeartbeatConfig.GetConfig<HeartbeatConfig>(); } }

        [ConfigProp("local")]
        public string Addr { get; private set; }

        [ConfigProp("remote")]
        public string Neighbor_addr { get; private set; }

        [ConfigProp("interval", ConfigPropAttribute.DataType.Int)]
        public int Interval { get; private set; }
    }
}

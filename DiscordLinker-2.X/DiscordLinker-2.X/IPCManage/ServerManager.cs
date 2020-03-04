using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using HttpServerLibrary;

namespace DiscordLinker_2.X.IPCManage
{
    class ServerManager
    {
        public delegate string GETRequest(string parameter);
        public event GETRequest OnGETRequest;

        public delegate void POSTRequest(JArray jArray);
        public event POSTRequest OnPOSTRequest;

        private object LockObject;

        private HttpServer HttpServer;
        private JArray JsonQueue;

        public ServerManager(int port)
        {
            LockObject = new object();
            JsonQueue = new JArray();

            HttpServer = new HttpServer(new IPEndPoint(IPAddress.Any, port));
            HttpServer.OnClientRequest += HttpServer_OnClientRequest;
        }

        public ServerManager(int port, CountdownEvent countdownEvent)
        {
            LockObject = new object();
            JsonQueue = new JArray();

            HttpServer = new HttpServer(new IPEndPoint(IPAddress.Any, port), countdownEvent);
            HttpServer.OnClientRequest += HttpServer_OnClientRequest;
        }

        ~ServerManager()
        {
            HttpServer.OnClientRequest -= HttpServer_OnClientRequest;
        }

        public void Start() => HttpServer.Start();

        public void JsonEnqueue(JToken jToken)
        {
            lock (LockObject)
            {
                JsonQueue.Add(jToken);
            }
        }

        private string HttpServer_OnClientRequest(RequestType requestType, string parameter, string content)
        {
            if (requestType == RequestType.GET)
            {
                string eventresult = OnGETRequest?.Invoke(parameter);
                if (eventresult != null)
                    return eventresult;

                if (parameter == "json")
                {
                    string returnvalue;
                    lock (LockObject)
                    {
                        returnvalue = JsonQueue.ToString();
                        JsonQueue.Clear();
                    }
                    return returnvalue;
                }
                else
                    return "server running";
            }
            else if (requestType == RequestType.POST)
            {
                OnPOSTRequest?.Invoke(JArray.Parse(content));
                return "OK";
            }
            else
                return "server running";
        }
    }
}

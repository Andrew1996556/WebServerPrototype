using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace ChatPrograms
{
    class WebServer
    {
        HttpListener _listener;
        string _baseFolder;

        private static readonly Dictionary<string, Type> currentTypes = Assembly.GetExecutingAssembly().GetTypes().ToDictionary(t => t.Name);

        public WebServer(string prefixUrl)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefixUrl);          
        }

        public async void Start()
        {
            _listener.Start();
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                await ProcessRequestAsync(context);
            }
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            Regex urlRegex = new Regex(@"^\/MyApp\/(\w*)\/(\w*)|\/$");
            Match match = urlRegex.Match(context.Request.RawUrl);
            if (match.Success)
            {
                string className = $"{match.Groups[1].Value}Controller";
                string methodName = match.Groups[2].Value;


                Type typeClass = GetController(className);

                if (typeClass != null)
                {
                    Object instance = Activator.CreateInstance(typeClass);
                    string result = (typeClass.GetMethod(methodName)?.Invoke(instance, null) as string) ?? "Found";
                    await Write(context, result);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await Write(context, "Not Found");
            }
        }

        private async Task Write(HttpListenerContext context, string msg)
        {
            context.Response.ContentLength64 = msg.Length;
            using (Stream s = context.Response.OutputStream)
            {
                await s.WriteAsync(Encoding.UTF8.GetBytes(msg), 0, msg.Length);
            }
        }

        private Type GetController(string typeName)
        {
            return currentTypes[typeName];
        }
    }
}
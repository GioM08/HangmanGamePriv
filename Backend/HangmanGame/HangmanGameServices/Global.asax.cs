using System;
using System.Web;
using HangmanGameServices.Services;

namespace HangmanGameServices
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ChatServer.Start();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            ChatServer.Stop();
        }
    }
}

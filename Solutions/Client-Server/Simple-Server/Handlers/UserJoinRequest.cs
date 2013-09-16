using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;


namespace SimpleServer.Handlers
{
    public class UserJoinRequest : RequestHandler
    {
        public override void OnHandle(Request request)
        {
            var uid = request.GetParamValue("uid");

            this.Response.Content = string.Format("Welcome! You are now connected as \"{0}\".", uid);
        }
    }
}

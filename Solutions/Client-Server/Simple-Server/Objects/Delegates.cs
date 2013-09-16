using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleServer
{
    public delegate void PlayerStatusEvent(object sender, object player);
    public delegate void UserStatusEvent(object sender, object user);
}

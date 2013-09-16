using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public class RequestHandlerFactory
    {
        #region Statics
        private static RequestHandlerFactory __instance = new RequestHandlerFactory();
        public static void Register<T>() where T : RequestHandler
        {
            __instance.Register2<T>();
        }
        public static RequestHandler GetHandler(string handlerName)
        {
            foreach (var n in __instance.handlerTypes)
            {
                if (n.Name.Equals(handlerName, StringComparison.InvariantCultureIgnoreCase))
                    return (RequestHandler)Activator.CreateInstance(n);
            }
            return null;
        } 
        #endregion

        private List<Type> handlerTypes = new List<Type>();

        protected void Register2<T>() where T : RequestHandler
        {
            var type = typeof(T);
            if (!handlerTypes.Contains(type))
            {
                handlerTypes.Add(type);
            }
        }
    }
}

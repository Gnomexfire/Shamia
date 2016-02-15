using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Shamai_R4.Class
{
    public class Configuration
    {
        #region declare
        public List<TemplateChannels> Channels = new List<TemplateChannels>();
        public StructConfiguration StConfiguration = new StructConfiguration();
        public struct StructConfiguration
        {
            /// <summary>
            /// server used
            /// </summary>
            public string Server { get; set; }
            /// <summary>
            /// login user
            /// </summary>
            public string Nick { get; set; }
            /// <summary>
            /// nick used in channel
            /// </summary>
            public string Owner { get; set; } 
            /// <summary>
            /// use auth moderate channel
            /// </summary>
            public string AuthSsl { get; set; }
            /// <summary>
            /// password used user
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// port used connect server
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// set channel to connect
            /// </summary>
            public string Channel { get; set; }
        }

        #endregion
    }

    #region TemplateClass

    public class TemplateChannels
    {
        public string Channels { get; set; }
    }

    #endregion
}

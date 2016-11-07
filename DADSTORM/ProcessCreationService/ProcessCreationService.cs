using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace ProcessCreationService
{
    public class ProcessCreationService
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(10000);

            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NodeManagerService),
                "NodeManagerService",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Press <enter> to terminate PCS...");
            System.Console.ReadLine();
        }
    }

    public class NodeManagerService : MarshalByRefObject, NodeManager.NodeManager
    {
        /*System.Xml.Serialization.XmlSerializer serializer =
new System.Xml.Serialization.XmlSerializer(typeof());  //HELP Is this to use + ??? Or NodeOperator should not be serialized???*/

        public void crash(string processname)
        {
            throw new NotImplementedException();
        }

        public void freeze(string processname)
        {
            throw new NotImplementedException();
        }

        public void interval(int operator_id, int x_ms)
        {
            throw new NotImplementedException();
        }

        public void start(int operator_id)
        {
            throw new NotImplementedException();
        }

        public string status()
        {
            throw new NotImplementedException();
        }

        public void unfreeze(string processname)
        {
            throw new NotImplementedException();
        }
    }
}

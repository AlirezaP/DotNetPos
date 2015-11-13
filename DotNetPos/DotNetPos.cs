using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPos
{
    public delegate void Recived(string[] iso8583);
    public delegate void RecivedXml(string xml);
    public delegate void RecivedExcel(MessageParser.NET.Tools.ExcelParser xml);

    public class DotNetPosMain
    {
        public event Recived RecivedIso;
        public event RecivedXml RecivedXml;
        public event RecivedExcel RecivedExcel;

        APSocket.Net.Server socket = new APSocket.Net.Server();
        APSocket.Net.Client Destination = new APSocket.Net.Client();

        System.Net.IPEndPoint destinationEndPoint;

        public enum MessageType { Iso8583, Xml, Excel };

        private MessageType _msgType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPointIP">Sender IPAddress</param>
        /// <param name="endPointPort">Sender Port</param>
        /// <param name="messageType">Message Type:
        /// ISO8583.
        /// XML.
        /// EXCEL.</param>
        public DotNetPosMain(string endPointIP, int endPointPort, MessageType messageType)
        {
            _msgType = messageType;

            destinationEndPoint =
               new System.Net.IPEndPoint(System.Net.IPAddress.Parse(endPointIP), endPointPort);
        }

        /// <summary>
        /// Start listening
        /// </summary>
        /// <param name="currentHostIP">Current Host IP (Listener Host)</param>
        /// <param name="currentHostPort">Current Host Port (Listener Host)</param>
        public async void StartListeninigAsyn(string currentHostIP, int currentHostPort)
        {
            socket.ReciveByteIntterupt += socket_ReciveByteIntterupt;
            await Task.Factory.StartNew(() => { socket.StartListeninig(currentHostIP, currentHostPort, APSocket.Net.Server.CommunicationMode.StreamFile); });
        }

        void socket_ReciveByteIntterupt(System.Net.Sockets.Socket socket, byte[] data)
        {
            Task.Factory.StartNew(() =>
            {
                switch (_msgType)
                {
                    case MessageType.Iso8583:
                        MessageParser.NET.Tools.ISO8583 iso = new MessageParser.NET.Tools.ISO8583();
                        string[] iso8583 = iso.Parse(Encoding.Unicode.GetString(data));

                        if (RecivedIso != null)
                            RecivedIso(iso8583);
                        break;

                    case MessageType.Xml:
                        string xmlData = Encoding.Unicode.GetString(data);
                        if (RecivedXml != null)
                            RecivedXml(xmlData);
                        break;

                    case MessageType.Excel:
                        MessageParser.NET.Tools.ExcelParser excel = new MessageParser.NET.Tools.ExcelParser(data);
                        if (RecivedExcel != null)
                            RecivedExcel(excel);
                        break;
                }
            });
        }

        public void Send(string[] iso8583, string mti)
        {
            MessageParser.NET.Tools.ISO8583 iso = new MessageParser.NET.Tools.ISO8583();
            Destination.Connect(destinationEndPoint.Address, destinationEndPoint.Port);
            Destination.Send(Encoding.Unicode.GetBytes(iso.Build(iso8583, mti)));
        }
    }

    /// <summary>
    /// Generate message and send it to specified host
    /// </summary>
    public class DotNetPosForTest
    {
        /// <summary>
        /// generate iso8583 message and send it to to specified host
        /// </summary>
        /// <param name="ipAddress">Host IPAddress</param>
        /// <param name="port">Host Port</param>
        public void SendIso8583(string ipAddress, int port)
        {
            MessageParser.NET.Tools.ISO8583 iso = new MessageParser.NET.Tools.ISO8583();
            Random rand = new Random();

            string[] DE = new string[130];

            DE[2] = (rand.Next(0, 9999).ToString() + rand.Next(0, 9999).ToString() + rand.Next(0, 9999).ToString() + rand.Next(0, 99999).ToString()).PadLeft(17, '0');

            DE[3] = (rand.Next(0, 999999).ToString()).PadLeft(6, '0');

            DE[4] = (rand.Next(0, 99999).ToString()).PadLeft(5, '0');

            DE[7] =
                (DateTime.Now.Month.ToString().PadLeft(2, '0')
                + DateTime.Now.Day.ToString().PadLeft(2, '0')
                + DateTime.Now.Hour.ToString().PadLeft(2, '0')
                + DateTime.Now.Minute.ToString().PadLeft(2, '0')
                + DateTime.Now.Second.ToString().PadLeft(2, '0'));

            DE[11] = (rand.Next(0, 999).ToString()).PadLeft(3, '0');

            DE[22] = (rand.Next(0, 99999999).ToString()).PadLeft(8, '0');

            DE[41] = (rand.Next(0, 99).ToString()).PadLeft(2, '0');

            string data = iso.Build(DE, "0200");

            APSocket.Net.Client client = new APSocket.Net.Client();
            client.Connect(System.Net.IPAddress.Parse(ipAddress), port);
            client.SendAsync(Encoding.Unicode.GetBytes(data));
        }

        /// <summary>
        /// generate xml message and send it to to specified host
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void SendXml(string ipAddress, int port)
        {
            string xmlData = @"<?xml version='1.0'?>
< doc >
    < assembly >
        < name > xmlsample </ name >
    </ assembly >
    < members >
        < member name = 'T:SomeClass' >
             < summary >
             Class level summary documentation goes here.</ summary >
             < remarks >
             Longer comments can be associated with a type or member
             through the remarks tag </ remarks >
         </ member >
         < member name = 'F:SomeClass.myName' >
              < summary >
              Store for the name property </ summary >
          </ member >
          < member name = 'M:SomeClass.#ctor' >
              < summary > The class constructor.</summary> 
        </member>
        <member name = 'M:SomeClass.SomeMethod(System.String)' >
            < summary >
            Description for SomeMethod.</summary>
            <param name = 's' > Parameter description for s goes here</param>
            <seealso cref = 'T:System.String' >
            You can use the cref attribute on any tag to reference a type or member
            and the compiler will check that the reference exists. </seealso>
        </member>
        <member name = 'M:SomeClass.SomeOtherMethod' >
            < summary >
            Some other method. </summary>
            <returns>
            Return results are described through the returns tag.</returns>
            <seealso cref = 'M:SomeClass.SomeMethod(System.String)' >
            Notice the use of the cref attribute to reference a specific method</seealso>
        </member>
        <member name = 'M:SomeClass.Main(System.String[])' >
            < summary >
            The entry point for the application.
            </summary>
            <param name = 'args' > A list of command line arguments</param>
        </member>
        <member name = 'P:SomeClass.Name' >
            < summary >
            Name property</summary>
            <value>
            A value tag is used to describe the property value</value>
        </member>
    </members>
</doc>";

            byte[] data = Encoding.Unicode.GetBytes(xmlData);
            APSocket.Net.Client client = new APSocket.Net.Client();
            client.Connect(System.Net.IPAddress.Parse(ipAddress), port);
            client.SendAsync(data);
        }

    }
}

# DotNetPos
DotNetPos is an open source tool for transaction Message.</br>
DotNetPos Support Iso8583,Xml,Excel Format.</br>

DotNetPos also contain tester calss, tester calss generate message (iso8583(transaction card originated messages standard ),xml) and send it to specified host.</br></br>

Nuget Url: http://www.nuget.org/packages/DotNetPos/ </br>

CommunicationMode:</br>
1- StreamFile: This mode is for Reciving a file streamly.

2- LengthFirst:This mode first Recive file size then Recive file.
(Send(byte[] data,bool firstLenght) do that automatically if firstlenght value be true)

Example:</br>

            DotNetPosMain pos = new DotNetPosMain("Sender IP Address", Sender Port, DotNetPosMain.MessageType.Iso8583);
            pos.RecivedIso += pos_RecivedIso;
            pos.StartListeninigAsyn("Listener Host IPAddress", Listener Host Port);

            //OR
            //FirstLengh Mode
            pos.StartListeninigAsyn("Listener Host IPAddress", ListenerHostPort,DotNetPosMain.CommunicationMode.LengthFirst);
            
            
            DotNetPosMain pos2 = new DotNetPosMain("Sender IP Address", Sender Port, DotNetPosMain.MessageType.Xml);
            pos2.RecivedXml += Pos2_RecivedXml; ;
            pos2.StartListeninigAsyn("Listener Host IPAddress", Listener Host Port);
            
            //OR
            //FirstLengh Mode
            pos2.StartListeninigAsyn("Listener Host IPAddress",ListenerHostPort,DotNetPosMain.CommunicationMode.LengthFirst);
            
          private void Pos2_RecivedXml(string xml)
          {
           //When Recive Xml Message
          }


          void pos_RecivedIso(string[] iso8583)
          {
          //When Recive ISO8583 Message
            var a = iso8583[(int)MessageParser.NET.Tools.ISO8583.FieldUsage.PANExtendedCountryCode];
            var b = iso8583[(int)MessageParser.NET.Tools.ISO8583.FieldUsage.PrimaryAccountNumber_PAN];
            var c = iso8583[(int)MessageParser.NET.Tools.ISO8583.FieldUsage.Application_PAN_Sequencenumber];
            
            //Here Edit Recived Message And Send It Back To Sender
            iso8583[(int)MessageParser.NET.Tools.ISO8583.FieldUsage.PrimaryAccountNumber_PAN] = "1234789412364587";
            pos.Send(iso8583, "0200");
          }

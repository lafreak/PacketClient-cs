# Packet Client
NuGet package installation:
```
Install-Package lafreak.PacketClient -Version 0.0.4 
```

C# implementation of TCP Client that works with [this server](https://github.com/lafreak/go-packet-server).

# Example
For this example we will request result of `X * Y` as soon as connection is estabilished.
## Client
``` cs
class Program
{
  static void Main(string[] args)
  {
    var client = new Client("localhost", 3000);
    
    // When connection is estabilished, ask server for result of 25 * 12 using packet type 30
    client.OnConnected(() => client.Send(30 /* Type */, 25, 12));
    
    // Subscribe to packet type 40 recv
    // When received, print result of multiplication
    client.On(40 /* Type */, (packet) =>
    {
      // Will print 300
      Console.WriteLine("Result: " + packet.ReadInt());
    });
    
    // Client listens for packets in another thread
    client.Connect();
    
    Console.ReadKey();
  }
}
```
## Server
``` go
package main

import "github.com/lafreak/go-packet-server"

func main() {
  serv := server.New("localhost:3000")
  
  // Subscribe to packet type 30 recv
  serv.On(30 /* Type */, func(s *server.Session, p *server.Packet) {
    // Read X & Y sent by client
    var x, y int
    p.Read(&x, &y)
    
    // Send result of X*Y
    s.Send(40 /* Type */, (int)(x*y))
  })
  
  serv.Start()
}
```

## More
Try to reconnect when server goes down:
``` cs
client.OnDisconnected(() => client.Connect());
```
Keep trying to connect if it's not possible to connect (server downtime):
``` cs
client.OnUnableToConnect((e) => client.Connect());
```
Keep track of packets you did not subscribe to:
``` cs
client.OnUnknownPacket((packet) => Console.WriteLine("Unknown packet: " + packet.Type));
```
Read more data at once:
``` cs
client.On(102, (packet) =>
{
  // One by one:
  byte b = packet.ReadByte();
  sbyte sb = packet.ReadSByte();
  string message = packet.ReadString();
  
  // All at once:
  var data = packet.Read(typeof(byte), typeof(sbyte), typeof(string));
  byte b_ = (byte)data[0];
  sbyte sb_ = (sbyte)data[1];
  string message_ = (string)data[2];
});
```

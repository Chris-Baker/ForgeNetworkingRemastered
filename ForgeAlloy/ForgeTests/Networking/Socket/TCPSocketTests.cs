﻿using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using Forge.Networking.Sockets;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Socket
{
	[TestFixture]
	public class TCPSocketTests
	{
		private const string LOCAL_IP = "127.0.0.1";
		private const ushort TEST_PORT = 15937;

		[Test]
		public void SocketListenAndClose_ShouldNotThrow()
		{
			ForgeTCPSocket socket = new ForgeTCPSocket();
			socket.Listen(TEST_PORT, 100);
			socket.Close();
		}

		[Test]
		public void SocketConnect_ShouldNotThrow()
		{
			ForgeTCPSocket server = new ForgeTCPSocket();
			server.Listen(TEST_PORT, 100);

			Task backgroundThread = Task.Run(() =>
			{
				server.AwaitAccept();
			});

			ForgeTCPSocket client = new ForgeTCPSocket();
			client.Connect(LOCAL_IP, TEST_PORT);
			client.Close();
			server.Close();
		}

		[Test]
		public void SendAndReceiveBuffer_ShouldBeEqual()
		{
			byte[] msg = new byte[]
			{
				3, 6, 9
			};
			BMSByte buffer = new BMSByte();
			buffer.SetArraySize(512);

			ForgeTCPSocket server = new ForgeTCPSocket();
			server.Listen(TEST_PORT, 100);

			Task backgroundThread = Task.Run(() =>
			{
				var connectedClient = server.AwaitAccept();
				connectedClient.Send(A.Fake<EndPoint>(), msg, msg.Length);
			});

			EndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15937);
			ForgeTCPSocket client = new ForgeTCPSocket();
			client.Connect(LOCAL_IP, TEST_PORT);
			int readLength = client.Receive(buffer, ref ep);
			client.Close();
			server.Close();

			Assert.AreEqual(msg.Length, readLength);
			Assert.AreEqual(msg.Length, buffer.Size);
			for (int i = 0; i < msg.Length; ++i)
			{
				Assert.AreEqual(msg[i], buffer[i]);
			}
		}
	}
}

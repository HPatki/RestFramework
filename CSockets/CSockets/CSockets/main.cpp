/*#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include "Socket.h"

#define DEFAULT_BUFLEN 32000
#define DEFAULT_PORT "15990"

int main (int c, char** argv)
{
	int iSendResult, iRecvResult;
    unsigned char recvbuf[DEFAULT_BUFLEN];
    int recvbuflen = DEFAULT_BUFLEN;

	TCPSocket sock (DEFAULT_PORT);
	if ( false == sock.InError())
		sock.StartListening();

	SOCKET ClientSocket;
	
	while ( sock.Accept() )
	{
		if (true == sock.InError() )
		{
			continue;
		}
	
		ClientSocket = sock.ReturnClientSocket();

		// Receive until the peer shuts down the connection
		do 
		{
			iRecvResult = TCPSocket::FetchData(ClientSocket, recvbuf, recvbuflen);
			if (iRecvResult == DEFAULT_BUFLEN) 
			{
				printf("Bytes received: %d\n", iRecvResult);

			}
			else if (iRecvResult < DEFAULT_BUFLEN)
				printf("Connection closing...\n");
			else  {
				printf("recv failed with error: %d\n", WSAGetLastError());
				closesocket(ClientSocket);
				WSACleanup();
				return 1;
			}

		 } while (iRecvResult >= DEFAULT_BUFLEN );

		//send back message
		unsigned char* reply = (unsigned char*)"HTTP/1.1 200 OK\r\n";
		TCPSocket::SendReply (ClientSocket,reply,17);
		TCPSocket::CloseClientSocket(ClientSocket);
	}
}*/
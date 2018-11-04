#include "Socket.h"

TCPSocket::TCPSocket (char* port)
{
	WSADATA wsaData;
    int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);

	struct addrinfo *result = nullptr, *ptr = nullptr, hints;

	ZeroMemory(&hints, sizeof (hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	// Resolve the local address and port to be used by the server
	iResult = getaddrinfo(nullptr, port, &hints, &result);
	if (iResult != 0) 
	{
		printf("getaddrinfo failed: %d\n", iResult);
		//WSACleanup();
		m_InError = true;
	}

	// Create a SOCKET for the server to listen for client connections
	m_ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);

	int rcvbuff = 65537, sendbuff = 65537, val = sizeof(bool), ret;

	bool nodelay = true, nodelay1 = 0;

	ret = setsockopt (m_ListenSocket,SOL_SOCKET,SO_RCVBUF,(char*)&rcvbuff,sizeof(int));
	if (0 != ret)
			ret = WSAGetLastError();

	ret = setsockopt (m_ListenSocket,SOL_SOCKET,SO_SNDBUF,(char*)&sendbuff,sizeof(int));
	if (0 != ret)
			ret = WSAGetLastError();
	val = sizeof(int);
	ret = getsockopt (m_ListenSocket,IPPROTO_TCP,TCP_NODELAY,(char*)&rcvbuff,&(val));

	ret = setsockopt (m_ListenSocket,IPPROTO_TCP,TCP_NODELAY,(char*)&nodelay,sizeof(nodelay));
	if (0 != ret)
			ret = WSAGetLastError();
	ret = getsockopt (m_ListenSocket,IPPROTO_TCP,TCP_NODELAY,(char*)&nodelay1,&(val));
	if (0 != ret)
			ret = WSAGetLastError();

	if (m_ListenSocket == INVALID_SOCKET) 
	{
		printf("Error at socket(): %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		//WSACleanup();
		m_InError = true;
	}

	iResult = bind( m_ListenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) 
	{
        printf("bind failed with error: %d\n", WSAGetLastError());
        freeaddrinfo(result);
        closesocket(m_ListenSocket);
        //WSACleanup();
        m_InError = true;
    }

	m_InError = false;
}

void TCPSocket::StartListening ()
{
	int iResult = listen( m_ListenSocket, SOMAXCONN );

	if (SOCKET_ERROR == iResult ) 
	{
		printf( "Listen failed with error: %ld\n", WSAGetLastError() );
		closesocket(m_ListenSocket);
		//WSACleanup();
		m_InError = true;
	}
}

bool TCPSocket::Accept()
{
	m_CurrentAccepted = accept(m_ListenSocket, nullptr, nullptr);

	if (m_CurrentAccepted == INVALID_SOCKET) 
	{
		m_InError = true;
	}
	else
	{
		int rcvbuff = 100000, sendbuff = 100000, val = sizeof(bool), ret;
		DWORD nodelay = 1, nodelay1 = 0;
		ret = setsockopt (m_CurrentAccepted,SOL_SOCKET,SO_RCVBUF,(char*)&rcvbuff,sizeof(rcvbuff));
		if (0 != ret)
			ret = WSAGetLastError();
		ret = setsockopt (m_CurrentAccepted,SOL_SOCKET,SO_SNDBUF,(char*)&sendbuff,sizeof(sendbuff));
		if (0 != ret)
			ret = WSAGetLastError();
		ret = setsockopt (m_CurrentAccepted,IPPROTO_TCP,TCP_NODELAY,(char*)&nodelay,sizeof(nodelay));
		if (0 != ret)
			ret = WSAGetLastError();
		ret = getsockopt (m_CurrentAccepted,IPPROTO_TCP,TCP_NODELAY,(char*)&nodelay1,&(val));
		if (0 != ret)
			ret = WSAGetLastError();
	}
	return true;
}

void TCPSocket::SendReply(SOCKET ClientSocket, unsigned char* buffer, int size)
{
	int sent = send(ClientSocket,(char*)buffer,size,0);
	while ( sent < size)
	{
		int newSize = size - sent;
		size = newSize;
		sent = send(ClientSocket,(char*)buffer+sent,size,0);
	}
}

SOCKET TCPSocket::ReturnClientSocket ()
{
	return m_CurrentAccepted;
}

int TCPSocket::FetchData (SOCKET ClientSocket, unsigned char* buffer, int size)
{
	int iResult = recv(ClientSocket, (char*)buffer, size, 0);
	return iResult;
}

void TCPSocket::CloseClientSocket (SOCKET ClientSocket)
{
	closesocket(ClientSocket);
	
}
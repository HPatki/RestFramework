#ifndef __socket_h__
#define __socket_h__

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>

public ref class TCPSocket
{
private:
	SOCKET	m_ListenSocket;
	SOCKET  m_CurrentAccepted;
	bool	m_InError;
public :

	TCPSocket (char* Port);
	void StartListening ();
	bool Accept();
	SOCKET ReturnClientSocket();
	static int FetchData (SOCKET ClientSocket, unsigned char* buffer, int size);
	static void SendReply (SOCKET ClientSocket, unsigned char* buffer, int size);
	static void CloseClientSocket(SOCKET ClientSocket);

	inline bool InError()
	{
		return m_InError;
	}
};


#endif
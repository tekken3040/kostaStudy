#include "IOCPEchoClnt_win.h"
#pragma comment(lib, "ws2_32.lib")
#pragma warning(disable:4996)

#define SERVER_ADDR "106.242.203.69"			//������ ���� �ּ�
#define SERVER_PORT 2738						//������ ���� ��Ʈ
//========================================================================================
//���� �Լ�
//========================================================================================
int main()
{
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2,2),&wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	SOCKET hSocket = WSASocket(PF_INET,SOCK_STREAM,0,NULL,0,WSA_FLAG_OVERLAPPED);
	if(hSocket == INVALID_SOCKET)
		ErrorHandling("socket() error");

	//������ ������ ���� ����
	SOCKADDR_IN recvAddr;
	memset(&recvAddr,0,sizeof(recvAddr));
	recvAddr.sin_family = AF_INET;
	recvAddr.sin_addr.s_addr = inet_addr(SERVER_ADDR);		//������ ���� �ּ� ����
	recvAddr.sin_port = htons(SERVER_PORT);					//������ ���� ��Ʈ ����

	//������ ���� �õ� connect����� SOCKET_ERROR �ϰ�� ���� ���
	if(connect(hSocket,(SOCKADDR*)&recvAddr,sizeof(recvAddr)) == SOCKET_ERROR)
		ErrorHandling("connect() error!");

	WSAEVENT event = WSACreateEvent();

	WSAOVERLAPPED overlapped;
	memset(&overlapped,0,sizeof(overlapped));

	overlapped.hEvent = event;

	WSABUF dataBuf;
	char message[1024] ={0,};								//�Էµ� ���� ���� ���ڿ�
	int sendBytes = 0;										//���� ���ڿ��� ũ��
	int recvBytes = 0;										//���� ���ڿ��� ũ��
	int flags = 0;

	//����ϸ鼭 ����ڿ��� ������ ���ڿ��� �Է¹޴´�
	while(true)
	{
		flags = 0;
		printf("�����ҵ�����(���Ḧ���ҽ�exit)\n");
		fgets(message, BUFSIZ, stdin);
		printf("�޼��� : %s", message);
		//exit �� �ԷµǸ� Ŭ���̾�Ʈ�� �����Ѵ�.
		if(!strncmp(message, "exit", 4)) break;
		dataBuf.len = strlen(message);
		printf("���� : %d\n", dataBuf.len);
		dataBuf.buf = message;
		printf("���� : %s\n", dataBuf.buf);

		//�Է¹��� ���ڿ��� ������ ����
		if(WSASend(hSocket,&dataBuf,1,(LPDWORD)&sendBytes,0,&overlapped,NULL) == SOCKET_ERROR)
		{
			if(WSAGetLastError() != WSA_IO_PENDING)
				ErrorHandling("WSASend() error");
		}

		//�߰��� ������ �����͸� ���
		WSAWaitForMultipleEvents(1,&event,TRUE,WSA_INFINITE,FALSE);
		//�������Ͽ� ���� ó���� �����
		WSAGetOverlappedResult(hSocket,&overlapped,(LPDWORD)&sendBytes,FALSE,NULL);

		printf("���۵ȹ���Ʈ��: %d \n",sendBytes);

		//�����κ��� ������� ���Ϲ���
		if(WSARecv(hSocket,&dataBuf,1,(LPDWORD)&recvBytes,(LPDWORD)&flags,&overlapped,NULL) == SOCKET_ERROR)
		{
			if(WSAGetLastError() != WSA_IO_PENDING)
				ErrorHandling("WSASend() error");
		}

		printf("Recv[%s]\n",dataBuf.buf);
	}
	//����� ���� ����
	closesocket(hSocket);

	WSACleanup();

	return 0;
}

//========================================================================================
//���� ��� �Լ�
//========================================================================================
void ErrorHandling(const char *message)
{
	fputs(message,stderr);
	fputc('\n',stderr);

	exit(1);
}

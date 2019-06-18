#include "IOCPEchoServ_win.h"
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "libmySQL.lib")
#pragma warning(disable:4996)

#define BUFSIZE 1024						//���ڿ� ��Ŷ�� ���� ������

//========================================================================================
//MySql Define
//========================================================================================
#define ADRRESS "106.242.203.69"			//������ �����ͺ��̽��� �ּ�
#define ACOUNT "LimeParfait"				//������ �����ͺ��̽��� ���̵�
#define PASSWORD "0000"						//������ �����ͺ��̽��� ��й�ȣ
#define DATABASE "employees"				//������ �����ͺ��̽��� �̸�
#define PORT 3315							//������ �����ͺ��̽��� ��Ʈ

//========================================================================================
//����� ���� ���� ����ü
//========================================================================================
typedef struct
{
	SOCKET hClntSock;						//Ŭ���̾�Ʈ�� ����
	SOCKADDR_IN clntAddr;					//Ŭ���̾�Ʈ�� �ּ�
} PER_HANDLE_DATA,*LPPER_HANDLE_DATA;		//����� ���� ���� ����ü�� ��ü�� ��ü ������

//========================================================================================
//���ڿ� ������ ���� ��Ŷ ����ü
//========================================================================================
typedef struct
{
	OVERLAPPED overlapped;					//IOCP ����� ���� ������ ����ü
	WSABUF wsaBuf;
	char buffer[BUFSIZE];					//������ ���ڿ��� ���� ����
} PER_IO_DATA,*LPPER_IO_DATA;				//���ڿ� ��Ŷ ����ü�� ��ü�� ��ü ������

//========================================================================================
//���� �Լ�
//========================================================================================
int main(int argc,char** argv)
{
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2,2),&wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	HANDLE hCompletionPort = CreateIoCompletionPort(INVALID_HANDLE_VALUE,NULL,0,0);

	SYSTEM_INFO SystemInfo;
	GetSystemInfo(&SystemInfo);

	//��Ŷ �޴� ó���� ���� ������ ����	
	//CPU�� �ھ� ���ڸ�ŭ ����
	for(int i=0; i<SystemInfo.dwNumberOfProcessors; ++i)
		_beginthreadex(NULL,0,CompletionThread,(LPVOID)hCompletionPort,0,NULL);

	SOCKET hServSock = WSASocket(AF_INET,SOCK_STREAM,0,NULL,0,WSA_FLAG_OVERLAPPED);

	SOCKADDR_IN servAddr;
	servAddr.sin_family = AF_INET;
	servAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	//servAddr.sin_port = htons(atoi("2738"));
	servAddr.sin_port = htons(2738);

	bind(hServSock,(SOCKADDR*)&servAddr,sizeof(servAddr));
	listen(hServSock,5);

	LPPER_IO_DATA PerIoData;
	LPPER_HANDLE_DATA PerHandleData;

	int RecvBytes;
	int i,Flags;

	//�����ͺ��̽� ����
	SqlConnect();

	//Ŭ���̾�Ʈ ���� ���
	while(TRUE)
	{
		SOCKADDR_IN clntAddr;
		int addrLen = sizeof(clntAddr);

		SOCKET hClntSock = accept(hServSock,(SOCKADDR*)&clntAddr,&addrLen);
		//Ŭ���̾�Ʈ ���� ��ü ���� �� �޸� �Ҵ�
		PerHandleData = (LPPER_HANDLE_DATA)malloc(sizeof(PER_HANDLE_DATA));
		PerHandleData->hClntSock = hClntSock;
		memcpy(&(PerHandleData->clntAddr),&clntAddr,addrLen);

		CreateIoCompletionPort((HANDLE)hClntSock,hCompletionPort,(DWORD)PerHandleData,0);

		//����� ���ڿ� ��Ŷ ��ü ���� �� �޸� �Ҵ�
		PerIoData = (LPPER_IO_DATA)malloc(sizeof(PER_IO_DATA));
		memset(&(PerIoData->overlapped),0,sizeof(OVERLAPPED));
		PerIoData->wsaBuf.len = BUFSIZE;
		PerIoData->wsaBuf.buf = PerIoData->buffer;
		Flags = 0;

		//Ŭ���̾�Ʈ�� ������ �޾� Ŭ���̾�Ʈ ���Ͽ� ����
		WSARecv(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,(LPDWORD)&RecvBytes,(LPDWORD)&Flags,&(PerIoData->overlapped),NULL);
	}

	//�����ͺ��̽��� ���� ����
	mysql_close(ConnPtr);
	printf("MySQL disconnected\n");
	return 0;
}

//========================================================================================
//��Ŷ�� �ޱ����� ���������� ���ư��� ������
//========================================================================================
unsigned int __stdcall CompletionThread(LPVOID pComPort)
{
	HANDLE hCompletionPort = (HANDLE)pComPort;
	DWORD BytesTransferred;
	LPPER_HANDLE_DATA PerHandleData;
	LPPER_IO_DATA PerIoData;
	DWORD flags;
	
	while(1) 
	{
		//ť�� ��� ��Ŷ�� ���´�
		GetQueuedCompletionStatus(hCompletionPort,&BytesTransferred,(PULONG_PTR)&PerHandleData,(LPOVERLAPPED*)&PerIoData,INFINITE);

		//ť�� ��� �����Ͱ� ������ ������ ����
		if(BytesTransferred == 0)
		{
			closesocket(PerHandleData->hClntSock);
			free(PerHandleData);
			free(PerIoData);	
			continue;
		}

		//ť�� ��� �����Ͱ� ������ �о�´�(���κп� �������� �� Ȯ���� ���� �ι��ڸ� �߰��Ѵ�)
		PerIoData->wsaBuf.buf[BytesTransferred] = '\0';
		printf("Recv[%s]\n",PerIoData->wsaBuf.buf);
		printf("%d\n",BytesTransferred);
		printf("%d\n",PerIoData->wsaBuf.len);

		//�����ͺ��̽��� ��������
		SQLQuerySend(PerIoData->wsaBuf.buf);
		//SQLQuerySend("insert into myemployees(firstName,lastName,email,phone) values('����Ʈ','������','RCF','010-0000-0010');");
		PerIoData->wsaBuf.len = BytesTransferred;

		//Ŭ���̾�Ʈ�� Ŭ���̾�Ʈ�� ���� �޼��� ����(Ȯ�ο�)
		WSASend(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,NULL,0,NULL,NULL);

		memset(&(PerIoData->overlapped),0,sizeof(OVERLAPPED));
		PerIoData->wsaBuf.len = BUFSIZE;
		PerIoData->wsaBuf.buf = PerIoData->buffer;

		flags = 0;
		//Ŭ���̾� ���� �޼����� �д´�
		WSARecv(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,NULL,&flags,&(PerIoData->overlapped),NULL);
	}
	
	return 0;
}

//========================================================================================
//���� �޽��� ��� �Լ�
//========================================================================================
void ErrorHandling(const char *message)
{
	fputs(message,stderr);
	fputc('\n',stderr);
	exit(1);
}

//========================================================================================
//MYSQL ���� �Լ�
//========================================================================================
int SqlConnect()
{
	mysql_init(&Conn);									//MySQL ���� �ʱ�ȭ

	//�����ͺ��̽��� ����
	//=================================�����ּ�=����====���====DB�̸�==��Ʈ=====����===�÷���
	ConnPtr = mysql_real_connect(&Conn,ADRRESS,ACOUNT,PASSWORD,DATABASE,PORT,(char*)NULL,0);

	//========================================================================================
	//�ѱ��� ���Ե� �������� �����ϱ����� ���ڿ� ��Ʈ ����
	//========================================================================================
	mysql_query(ConnPtr,"set session character_set_connection=euckr;");
	mysql_query(ConnPtr,"set session character_set_results=euckr;");
	mysql_query(ConnPtr,"set session character_set_client=euckr;");

	//���� ��� Ȯ��. NULL�� ��� �������
	if(ConnPtr == NULL)
	{
		fprintf(stderr,"MySQL connection error : %s",mysql_error(&Conn));
		return 1;
	}
	printf("MySQL connected\n");

	return 0;
}

//========================================================================================
//������ ���� �Լ�
//========================================================================================
int SQLQuerySend(const char *message)
{
	const char* Query = message;						//���޵� �������� ���� ���ڿ�
	stat = mysql_query(ConnPtr,Query);					//������û �� �������� �޾ƿ���
	if(stat != 0)										//������û ���� �� ����ó��
	{
		fprintf(stderr,"MySQL query error : %s\n",mysql_error(&Conn));
		return 1;
	}

	Result = mysql_store_result(ConnPtr);				//��� Ȯ���ϱ�


	//��� ����ϱ�
	//=========================================================================================
	//insert �������� ��� NULL�� �����ϱ� ������ ����ó���� ���ϸ� ������ �״´�
	//=========================================================================================
	if(Result != NULL)
	{
		while((Row = mysql_fetch_row(Result)) != NULL)		
		{
			//printf("------------------------------------------------------------------------------------------------------------------\n");
			//printf("| empNo : %s | FirstName : %s | LastName : %s  | Email : %s | Phone : %s |\n",Row[0],Row[1],Row[2],Row[3],Row[4]);
			//printf("------------------------------------------------------------------------------------------------------------------\n");
			printf("------------------------------------------------------------------------------------------------------------------\n");
			printf("| ID : %s | Name : %s | Password : %s  |\n",Row[0],Row[1],Row[2]);
			printf("------------------------------------------------------------------------------------------------------------------\n");
		}
	}
	else
	{
		printf("���������� ���� �Ϸ�. ������� NULL ����.(�� : Insert��)\n");
	}

	mysql_free_result(Result);							//��� ����

	return 0;
}
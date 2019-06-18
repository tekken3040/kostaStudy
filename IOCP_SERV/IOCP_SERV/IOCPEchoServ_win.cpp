#include "IOCPEchoServ_win.h"
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "libmySQL.lib")
#pragma warning(disable:4996)

#define BUFSIZE 1024						//문자열 패킷의 버퍼 사이즈

//========================================================================================
//MySql Define
//========================================================================================
#define ADRRESS "106.242.203.69"			//접속할 데이터베이스의 주소
#define ACOUNT "LimeParfait"				//접속할 데이터베이스의 아이디
#define PASSWORD "0000"						//접속할 데이터베이스의 비밀번호
#define DATABASE "employees"				//접속할 데이터베이스의 이름
#define PORT 3315							//접속할 데이터베이스의 포트

//========================================================================================
//통신을 위한 소켓 구조체
//========================================================================================
typedef struct
{
	SOCKET hClntSock;						//클라이언트의 소켓
	SOCKADDR_IN clntAddr;					//클라이언트의 주소
} PER_HANDLE_DATA,*LPPER_HANDLE_DATA;		//통신을 위한 소켓 구조체의 객체와 객체 포인터

//========================================================================================
//문자열 전송을 위한 패킷 구조체
//========================================================================================
typedef struct
{
	OVERLAPPED overlapped;					//IOCP 통신을 위한 오버랩 구조체
	WSABUF wsaBuf;
	char buffer[BUFSIZE];					//전달할 문자열을 담을 버퍼
} PER_IO_DATA,*LPPER_IO_DATA;				//문자열 패킷 구조체의 객체와 객체 포인터

//========================================================================================
//메인 함수
//========================================================================================
int main(int argc,char** argv)
{
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2,2),&wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	HANDLE hCompletionPort = CreateIoCompletionPort(INVALID_HANDLE_VALUE,NULL,0,0);

	SYSTEM_INFO SystemInfo;
	GetSystemInfo(&SystemInfo);

	//패킷 받는 처리를 위한 쓰레드 생성	
	//CPU의 코어 숫자만큼 생성
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

	//데이터베이스 연결
	SqlConnect();

	//클라이언트 접속 대기
	while(TRUE)
	{
		SOCKADDR_IN clntAddr;
		int addrLen = sizeof(clntAddr);

		SOCKET hClntSock = accept(hServSock,(SOCKADDR*)&clntAddr,&addrLen);
		//클라이언트 소켓 객체 생성 및 메모리 할당
		PerHandleData = (LPPER_HANDLE_DATA)malloc(sizeof(PER_HANDLE_DATA));
		PerHandleData->hClntSock = hClntSock;
		memcpy(&(PerHandleData->clntAddr),&clntAddr,addrLen);

		CreateIoCompletionPort((HANDLE)hClntSock,hCompletionPort,(DWORD)PerHandleData,0);

		//통신할 문자열 패킷 객체 생성 및 메모리 할당
		PerIoData = (LPPER_IO_DATA)malloc(sizeof(PER_IO_DATA));
		memset(&(PerIoData->overlapped),0,sizeof(OVERLAPPED));
		PerIoData->wsaBuf.len = BUFSIZE;
		PerIoData->wsaBuf.buf = PerIoData->buffer;
		Flags = 0;

		//클라이언트의 접속을 받아 클라이언트 소켓에 설정
		WSARecv(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,(LPDWORD)&RecvBytes,(LPDWORD)&Flags,&(PerIoData->overlapped),NULL);
	}

	//데이터베이스와 연결 해제
	mysql_close(ConnPtr);
	printf("MySQL disconnected\n");
	return 0;
}

//========================================================================================
//패킷을 받기위해 독립적으로 돌아가는 쓰레드
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
		//큐에 담긴 패킷을 얻어온다
		GetQueuedCompletionStatus(hCompletionPort,&BytesTransferred,(PULONG_PTR)&PerHandleData,(LPOVERLAPPED*)&PerIoData,INFINITE);

		//큐에 담긴 데이터가 없으면 루프를 돈다
		if(BytesTransferred == 0)
		{
			closesocket(PerHandleData->hClntSock);
			free(PerHandleData);
			free(PerIoData);	
			continue;
		}

		//큐에 담긴 데이터가 있으면 읽어온다(끝부분에 데이터의 끝 확인을 위해 널문자를 추가한다)
		PerIoData->wsaBuf.buf[BytesTransferred] = '\0';
		printf("Recv[%s]\n",PerIoData->wsaBuf.buf);
		printf("%d\n",BytesTransferred);
		printf("%d\n",PerIoData->wsaBuf.len);

		//데이터베이스로 쿼리전송
		SQLQuerySend(PerIoData->wsaBuf.buf);
		//SQLQuerySend("insert into myemployees(firstName,lastName,email,phone) values('세인트','버나드','RCF','010-0000-0010');");
		PerIoData->wsaBuf.len = BytesTransferred;

		//클라이언트로 클라이언트가 보낸 메세지 전송(확인용)
		WSASend(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,NULL,0,NULL,NULL);

		memset(&(PerIoData->overlapped),0,sizeof(OVERLAPPED));
		PerIoData->wsaBuf.len = BUFSIZE;
		PerIoData->wsaBuf.buf = PerIoData->buffer;

		flags = 0;
		//클라이언가 보낸 메세지를 읽는다
		WSARecv(PerHandleData->hClntSock,&(PerIoData->wsaBuf),1,NULL,&flags,&(PerIoData->overlapped),NULL);
	}
	
	return 0;
}

//========================================================================================
//에러 메시지 출력 함수
//========================================================================================
void ErrorHandling(const char *message)
{
	fputs(message,stderr);
	fputc('\n',stderr);
	exit(1);
}

//========================================================================================
//MYSQL 연결 함수
//========================================================================================
int SqlConnect()
{
	mysql_init(&Conn);									//MySQL 정보 초기화

	//데이터베이스와 연결
	//=================================서버주소=계정====비번====DB이름==포트=====소켓===플래그
	ConnPtr = mysql_real_connect(&Conn,ADRRESS,ACOUNT,PASSWORD,DATABASE,PORT,(char*)NULL,0);

	//========================================================================================
	//한글이 포함된 쿼리문을 전달하기위한 문자열 셋트 설정
	//========================================================================================
	mysql_query(ConnPtr,"set session character_set_connection=euckr;");
	mysql_query(ConnPtr,"set session character_set_results=euckr;");
	mysql_query(ConnPtr,"set session character_set_client=euckr;");

	//연결 결과 확인. NULL일 결우 연결실패
	if(ConnPtr == NULL)
	{
		fprintf(stderr,"MySQL connection error : %s",mysql_error(&Conn));
		return 1;
	}
	printf("MySQL connected\n");

	return 0;
}

//========================================================================================
//쿼리문 전송 함수
//========================================================================================
int SQLQuerySend(const char *message)
{
	const char* Query = message;						//전달된 쿼리문을 담을 문자열
	stat = mysql_query(ConnPtr,Query);					//쿼리요청 및 성공여부 받아오기
	if(stat != 0)										//쿼리요청 실패 시 예외처리
	{
		fprintf(stderr,"MySQL query error : %s\n",mysql_error(&Conn));
		return 1;
	}

	Result = mysql_store_result(ConnPtr);				//결과 확인하기


	//결과 출력하기
	//=========================================================================================
	//insert 쿼리문의 경우 NULL을 리턴하기 때문에 예외처리를 안하면 서버가 죽는다
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
		printf("정상적으로 전달 완료. 결과값에 NULL 있음.(예 : Insert문)\n");
	}

	mysql_free_result(Result);							//결과 비우기

	return 0;
}
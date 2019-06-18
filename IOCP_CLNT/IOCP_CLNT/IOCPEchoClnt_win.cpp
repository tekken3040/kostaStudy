#include "IOCPEchoClnt_win.h"
#pragma comment(lib, "ws2_32.lib")
#pragma warning(disable:4996)

#define SERVER_ADDR "106.242.203.69"			//접속할 서버 주소
#define SERVER_PORT 2738						//접속할 서버 포트
//========================================================================================
//메인 함수
//========================================================================================
int main()
{
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2,2),&wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	SOCKET hSocket = WSASocket(PF_INET,SOCK_STREAM,0,NULL,0,WSA_FLAG_OVERLAPPED);
	if(hSocket == INVALID_SOCKET)
		ErrorHandling("socket() error");

	//접속할 서버의 소켓 생성
	SOCKADDR_IN recvAddr;
	memset(&recvAddr,0,sizeof(recvAddr));
	recvAddr.sin_family = AF_INET;
	recvAddr.sin_addr.s_addr = inet_addr(SERVER_ADDR);		//접속할 서버 주소 셋팅
	recvAddr.sin_port = htons(SERVER_PORT);					//접속할 서버 포트 셋팅

	//서버에 접속 시도 connect결과가 SOCKET_ERROR 일경우 에러 출력
	if(connect(hSocket,(SOCKADDR*)&recvAddr,sizeof(recvAddr)) == SOCKET_ERROR)
		ErrorHandling("connect() error!");

	WSAEVENT event = WSACreateEvent();

	WSAOVERLAPPED overlapped;
	memset(&overlapped,0,sizeof(overlapped));

	overlapped.hEvent = event;

	WSABUF dataBuf;
	char message[1024] ={0,};								//입력된 값을 담을 문자열
	int sendBytes = 0;										//보낼 문자열의 크기
	int recvBytes = 0;										//받을 문자열의 크기
	int flags = 0;

	//대기하면서 사용자에게 전송할 문자열을 입력받는다
	while(true)
	{
		flags = 0;
		printf("전송할데이터(종료를원할시exit)\n");
		fgets(message, BUFSIZ, stdin);
		printf("메세지 : %s", message);
		//exit 가 입력되면 클라이언트를 종료한다.
		if(!strncmp(message, "exit", 4)) break;
		dataBuf.len = strlen(message);
		printf("길이 : %d\n", dataBuf.len);
		dataBuf.buf = message;
		printf("버퍼 : %s\n", dataBuf.buf);

		//입력받은 문자열을 서버로 전송
		if(WSASend(hSocket,&dataBuf,1,(LPDWORD)&sendBytes,0,&overlapped,NULL) == SOCKET_ERROR)
		{
			if(WSAGetLastError() != WSA_IO_PENDING)
				ErrorHandling("WSASend() error");
		}

		//추가로 전송할 데이터를 대기
		WSAWaitForMultipleEvents(1,&event,TRUE,WSA_INFINITE,FALSE);
		//오버랩하여 동시 처리된 결과값
		WSAGetOverlappedResult(hSocket,&overlapped,(LPDWORD)&sendBytes,FALSE,NULL);

		printf("전송된바이트수: %d \n",sendBytes);

		//서버로부터 결과값을 리턴받음
		if(WSARecv(hSocket,&dataBuf,1,(LPDWORD)&recvBytes,(LPDWORD)&flags,&overlapped,NULL) == SOCKET_ERROR)
		{
			if(WSAGetLastError() != WSA_IO_PENDING)
				ErrorHandling("WSASend() error");
		}

		printf("Recv[%s]\n",dataBuf.buf);
	}
	//연결된 소켓 종료
	closesocket(hSocket);

	WSACleanup();

	return 0;
}

//========================================================================================
//에러 출력 함수
//========================================================================================
void ErrorHandling(const char *message)
{
	fputs(message,stderr);
	fputc('\n',stderr);

	exit(1);
}

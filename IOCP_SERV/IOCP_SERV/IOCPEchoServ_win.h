#pragma once
#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <process.h>
#include <mysql.h>

MYSQL Conn;														//MySQL 정보를 담을 구조체
MYSQL* ConnPtr = NULL;											//MySQL 핸들
MYSQL_RES* Result;												//쿼리성공시 결과를 담는 구조체 포인터
MYSQL_ROW Row;													//쿼리성공시 결과로 나온 행의 정보를 담는 구조체
int stat;														//쿼리요청 후 결과(성공,실패)

unsigned int __stdcall CompletionThread(LPVOID pComPort);		//패킷을 받기위한 쓰레드 함수 선언
void ErrorHandling(const char *message);						//에러출력 함수 선언
int SqlConnect();												//데이터베이스 연결 함수 선언
int SQLQuerySend(const char *message);							//쿼리 전송 함수 선언
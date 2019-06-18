#pragma once
#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <process.h>
#include <mysql.h>

MYSQL Conn;														//MySQL ������ ���� ����ü
MYSQL* ConnPtr = NULL;											//MySQL �ڵ�
MYSQL_RES* Result;												//���������� ����� ��� ����ü ������
MYSQL_ROW Row;													//���������� ����� ���� ���� ������ ��� ����ü
int stat;														//������û �� ���(����,����)

unsigned int __stdcall CompletionThread(LPVOID pComPort);		//��Ŷ�� �ޱ����� ������ �Լ� ����
void ErrorHandling(const char *message);						//������� �Լ� ����
int SqlConnect();												//�����ͺ��̽� ���� �Լ� ����
int SQLQuerySend(const char *message);							//���� ���� �Լ� ����
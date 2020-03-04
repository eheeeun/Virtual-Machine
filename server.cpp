#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>
#include <process.h>

#pragma comment (lib , "ws2_32.lib")

#define BUFSIZE 512
#define PORT 4000

DWORD WINAPI ProcessClient(LPVOID arg) {
	SOCKET client_sock = (SOCKET)arg;
	char buf[BUFSIZE + 1];
	SOCKADDR_IN clientaddr;
	int addrlen;
	int retval;

	addrlen = sizeof(clientaddr);
	getpeername(client_sock, (SOCKADDR*)&clientaddr, &addrlen);

	// 클라이언트와 데이터 통신
	while (1) {
		retval = recv(client_sock, buf, BUFSIZE, 0);
		if (retval == SOCKET_ERROR){ break; }
		else if (retval == 0) break;

		// 받은 데이터 출력
		buf[retval] = '\0';
		printf("[TCP /%s:%d] %s\n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port), buf);

		// 데이터 보내기
		retval = send(client_sock, buf, retval, 0);

		if (retval == SOCKET_ERROR){
			printf("send() error\n");
			break;
		}
	}
	// closesocket()
	closesocket(client_sock);
	printf("TCP 서버, 클라이언트 종료 : IP 주소 = %s, 포트번호 = %d\n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));

	return 0;
}

int main(){
	// 윈속초기화
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return -1;

	// 리턴 값 저장시킬 변수
	int return_val;

	// socket() 
	SOCKET listen_sock = socket(AF_INET, SOCK_STREAM, 0);
	if (listen_sock == INVALID_SOCKET) printf("소켓() 에러염\n");

	// bind()
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_port = htons(PORT);

	/*
	XP 이전에는 INADDR_ANY의 값에 빈 스트링을 넘겨줄 경우 INADDR_ANY로 세팅되었지만,
	2003 이후로는 INADDR_NONE의 에러값을 넘겨준다, INADDR_NONE은 IP대역이 A.B.C.D 중 하나라도 255를 초과 할 경우 세팅
	INADDR_ANY는 어느 주소로 접속하던 접속을 받아들인다.
	*/

	serveraddr.sin_addr.s_addr = htons(INADDR_ANY);
	return_val = bind(listen_sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr)); // connect가 아니라 bind

	if (return_val == SOCKET_ERROR) printf("bind() error\n");

	// listen()
	return_val = listen(listen_sock, SOMAXCONN);
	if (return_val == SOCKET_ERROR) printf("listen() error\n");

	// 데이터 통신에 사용할 변수
	SOCKET client_sock;
	SOCKADDR_IN clientaddr;
	char buf[BUFSIZE + 1];
	int addrlen;

	HANDLE hThread;              // 스레드 핸들
	DWORD ThreadID;              // 스레드 아이디
	
	// 클라이언트와 데이터 통신 
	while (1){ 
		addrlen = sizeof(clientaddr);
		// accept()
		client_sock = accept(listen_sock, (SOCKADDR*)&clientaddr, &addrlen);
		if (client_sock == INVALID_SOCKET) {
			printf("accept() 에러\n");
			continue;
		}
		printf("TCP 서버, 클라이언트 접속 : IP 주소 = %s, 포트번호 = %d\n", 
			inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));

		hThread = CreateThread(NULL, 0, ProcessClient, (LPVOID)client_sock, 0, &ThreadID);
		if (hThread == NULL) { printf("failed to create thread...\n"); }
		else{ CloseHandle(hThread);}
	}
	closesocket(listen_sock);

	// 윈속 종료
	WSACleanup();
	return 0;

}
//출처: https://wonjayk.tistory.com/156 [배고파서 까먹고 만든 블로그]

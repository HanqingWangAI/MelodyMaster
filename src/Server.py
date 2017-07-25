import threading
import os
import socket
from collections import deque
from utils import *
class Server(object):

    def __init__(self, port=10010, max_clients=100):
        #self.s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.s = socket.socket()
        self.s.bind(('',port))
        self.s.listen(max_clients)
        self.listen_threads = []
        self.parse_threads = []

        self.watch_thread = None
        self.send_thread = None

        self.clients = []
        self.bufferQueues = {}
        self.packages = deque()
        self.sendQueue = deque()

    def start(self):

        self.watch_thread = threading.Thread(target=self.watch)
        self.watch_thread.setDaemon(True)
        self.watch_thread.start()

        self.send_thread = threading.Thread(target=self.sender)
        self.send_thread.setDaemon(True)
        self.send_thread.start()

    '''
        if package is not empty, return a request
    '''
    def fetch_package(self):
        if len(self.packages) != 0:
            return self.packages.popleft()
        return None

    def watch(self):
        print('Watch thread start!')
        while True:
            try:
                conn,address = self.s.accept()
                self.clients.append(conn)
                self.bufferQueues[conn] = deque()
                thread = threading.Thread(target=self.listen,args=[conn])
                thread.setDaemon(True)
                thread.start()
                self.listen_threads.append(thread)

                thread = threading.Thread(target=self.pares_package,args=[conn])
                thread.setDaemon(True)
                thread.start()
                self.parse_threads.append(thread)

            except Exception as ex:
                print ('Watch thread',ex)

    '''
        listen thread, each client has one
    '''
    def listen(self,client):
        print("Connected from",client)
        queue = self.bufferQueues[client]
        while True:
            try:
                buf = client.recv(1024)
                for byte in buf:
                    #print(byte.__class__)
                    queue.append(byte)
            except Exception as ex:
                self.clients.remove(client)
                print (ex)
                print ('Current online client:', len(self.clients))
                break


    '''
        parse package thread, each client has one
        put the data in buffer into the package queue
        element format: [package, client_socket]
    '''
    def pares_package(self,client):
        pac = Package()
        data = []
        length = 0
        cnt = 0
        queue = self.bufferQueues[client]
        while True:
            if len(queue) != 0:
                b = queue.popleft()
                if pac.length == -2:
                    length = b
                    pac.length = -1
                elif pac.length == -1:
                    length += b * 256
                    pac.length = length
                else:
                    data.append(b)
                    cnt += 1
                    if cnt == pac.length:
                        pac.data = bytearray(data).decode()
                        self.packages.append([pac,client])
                        print('Parse a package:',pac.data)
                        pac = Package()
                        data = []
                        cnt = 0

    def send_msg(self,msg,client):
        pac = Package(msg,len(msg))
        self.sendQueue.append([pac,client])


    def sender(self):
        import six
        print ('Sender thread starts!')
        while True:
            try:
                if len(self.sendQueue) == 0:
                    continue
                pac,client = self.sendQueue.popleft()
                client.send(pac.format())
            except Exception as ex:
                print ('sender error',ex)

    def send_all(self,msg):
        for client in self.clients:
            self.send_msg(msg,client)

if __name__ == '__main__':
    server = Server()
    server.start()
    while True:
        op = input()
        if op == 'send':
            content = input()
            server.send_all(content)
        if op == 'fetch':
            total = server.fetch_package()
            if total == None:
                print('Nothing left..')
            else:
                pac = total[0]
                client = total[1]
                print('receive message',pac.data,'from',client)
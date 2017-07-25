from .utils.Server import Server
from .utils.test import *
import threading
from .utils.NoteTranslator import NoteTranslator
from .utils.StdScore import StdScore

time_step = 8
preY = []

def Listen_and_Response(server):
    translator = NoteTranslator()
    while True:
        package = server.fetch_package() #fetch a request sent from the client
        if package == None:
            continue
        pac, client = package

        key_track = translator.fw_run(pac.data)
        score = StdScore([key_track])

        prediction, _ = predictId(score.getKeyFeature(),pre_Y=preY,time_step=time_step,first=False)

        server.send_msg(msg=repr(prediction),client=client)   # repr(object) can change the object into string, \
                                                                # using eval(string) to restore the object from string



if __name__ == '__main__':
    global preY # need to be initialized

    restore(time_step)
    server = Server(port=10010)
    server.start()

    handle_thread = threading.Thread(target=Listen_and_Response,args=[server,])
    handle_thread.setDaemon(True)
    handle_thread.start()

    while True:
        op = input()
        if op == 'shutdown':
            break
        '''
            Add other operations
        '''
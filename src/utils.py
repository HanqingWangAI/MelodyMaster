
class Package(object):

    def __init__(self, data=[],length=-2):
        if data != []:
            length = len(data)
        self.length = length
        self.data = data

    def format(self):
        try:
            ret = []
            ret.append(int(self.length%256))
            ret.append(int(self.length/256))
            temp = self.data.encode('utf-8')
            for _ in temp:
                ret.append(_)
            ret = bytearray(ret)
        except Exception as ex:
            print ('format error',ex)
        return ret

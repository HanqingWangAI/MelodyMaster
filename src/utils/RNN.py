import sys
sys.path.append(r"D:\liuchang\Projects\Hackathon\Source\MelodyMaster\src\utils")
# sys.path.append(r"D:\Programs\Python27\lib")
# sys.path.append(r"D:\Programs\Python27\Lib\site-packages\numpy\lib")
import NoteTranslator


# sys.argv is the list of command line arguments passed to the python script
# score = arg1
score = sys.argv[1]
print(score)


MelodyMaster = NoteTranslator.NoteTranslator()
PraxisSucks = MelodyMaster.fw_run(score)
#
#
#
# for bar in PraxisSucks:
#     for note in bar:
#         print(note)

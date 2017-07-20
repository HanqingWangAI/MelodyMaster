import sys
import NoteTranslator
# sys.argv is the list of command line arguments passed to the python script
score = sys.argv[1]
MelodyMaster = NoteTranslator.NoteTranslator()
PraxisSucks = MelodyMaster.run(score)

for bar in PraxisSucks:
    for note in bar:
        print(note.pitch,note.duration)

import sys
import NoteTranslator
import StdScore
import test
# sys.argv is the list of command line arguments passed to the python script
#score = arg1
MelodyMaster = NoteTranslator.NoteTranslator()
line = sys.argv[1]
PraxisSucks=MelodyMaster.fw_run(line)
# with open("test.icd",'r') as f:
# 	line = f.readline()
# 	bars = line.strip().split(',')
# 	score = []
# 	for i,bar in enumerate(bars):
# 		notes = bar.strip().split(' ')
# 		score.append(notes)
# 	#print(score)
# 	PraxisSucks=MelodyMaster.fw_run(line)

#print(PraxisSucks)
score = StdScore.StdScore([PraxisSucks])
keyFeature = score.getKeyFeature()
times = len(keyFeature)
preY = []
for i in range(times):
	preY.append([0 for _ in range(test.class_num)])
output = []
for i in range(times):
	output_i, feature_i = test.predictId([keyFeature],[preY],times,i==0)
	output.append(output_i[i])
	if i+1 < times:
		preY[i+1] = feature_i[i]
for i,key in enumerate(output):
	print(key,end=" ")
#PraxisSucks = MelodyMaster.fw_run(score)
#
#
#
# for bar in PraxisSucks:
#     for note in bar:
#         #print(note.pitch)
#         print(note.getReadableNote())

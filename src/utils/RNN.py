import sys
import NoteTranslator
import StdScore
import test_mul
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
preY = []
for i in range(test_mul.time_step):
	preY.append([0 for _ in range(test_mul.output_size)])
output = []
for i in range(test_mul.time_step):
	output_i, feature_i = test_mul.predictId([keyFeature[0:8]],[preY],i==0)
	output.append(output_i[i])
	if i+1 < test_mul.time_step:
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

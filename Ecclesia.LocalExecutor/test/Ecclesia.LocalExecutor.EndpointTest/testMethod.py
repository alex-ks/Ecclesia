import sys
import os

if __name__ == "__main__":
    result = [10]
    
    args  = []
    nameFiles = []

    nameFiles = sorted(os.listdir(sys.argv[1]))
    nameFiles.remove("source.py")

    for i in range(len(nameFiles)):
        inputPath = os.path.join(sys.argv[1], nameFiles[i])
        with open(inputPath, 'r') as input:
            arg = input.readline()
        args.append(int(arg))
        
    result = map(lambda x: x * 2, args)

    for i in range(len(result)):
        outputPath = os.path.join(sys.argv[1], str(i) + "output.txt")
        with open(outputPath, 'w') as output:
            output.write(str(result[i]))

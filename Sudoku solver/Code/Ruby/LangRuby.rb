def forStatement()
    sumInteger1To10 = 0

    for i in 1...11 #From inclusive and to exclusive
        sumInteger1To10 += i
    end #The end closes the block

    puts sumInteger1To10

    for i in 1..10 #From inclusive and to inclusive
        if (i == 1)  #Works without the parentheses as well
            sumInteger1To10 = 0
        end
        sumInteger1To10 += i
    end

    puts sumInteger1To10
end

def readWriteTextFromFile()
    fs = open("C:\\tmp\\tmp.txt", "rt")
    s = fs.read()
    fs.close()
    puts s

    # Works also with: fs = File.new("C:\\tmp\\tmp.txt", "wt")
    fs = open("C:\\tmp\\tmp.txt", "wt")
    fs.write(s + " Hello World!")
    fs.close()

    fs = open("C:\\tmp\\tmp.txt", "rt")
    s = fs.read()
    fs.close()
    puts s
end

def whileStatement()
    sumInteger1To10 = 0

    i = 1
    while (i <= 10) #Works without the parentheses as well
        sumInteger1To10 += i
        i += 1
    end

    puts sumInteger1To10

    i = 1
    while i <= 10
        if i == 1  #Works with the parentheses as well
            sumInteger1To10 = 0
        end
        sumInteger1To10 += i
        i += 1
    end

    puts sumInteger1To10
end

forStatement()
readWriteTextFromFile()
whileStatement()
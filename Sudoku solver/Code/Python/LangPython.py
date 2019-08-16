import sys
import os.path
import datetime
from enum import Enum
from random import randrange

def forStatement():
    sumInteger1To10 = 0

    for i in range(1, 11): #From inclusive and to exclusive
        sumInteger1To10 += i

    print(sumInteger1To10)

    for i in range(1, 11):
        if (i == 1):  #Works without the parentheses as well
            sumInteger1To10 = 0
        sumInteger1To10 += i  #Same indentation for the block, i.e. this line belongs to same block as the if-statement

    print(sumInteger1To10)

def readWriteTextFromFile():
    fs = open("C:\\tmp\\tmp.txt", "rt")
    s = fs.read()
    fs.close()
    print(s);

    fs = open("C:\\tmp\\tmp.txt", "wt")
    fs.write(s + " Hello World!")
    fs.close()

    fs = open("C:\\tmp\\tmp.txt", "rt")
    s = fs.read()
    fs.close()
    print(s);

def whileStatement():
    sumInteger1To10 = 0

    i = 1
    while (i <= 10): #Works without the parentheses as well
        sumInteger1To10 += i
        i += 1

    print(sumInteger1To10)

    i = 1
    while i <= 10:
        if i == 1:  #Works with the parentheses as well
            sumInteger1To10 = 0
        sumInteger1To10 += i  #Same indentation for the block, i.e. this line belongs to same block as the if-statement
        i += 1

    print(sumInteger1To10)

forStatement()
readWriteTextFromFile()
whileStatement()
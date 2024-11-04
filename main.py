import os
import cv2
from cvzone.HandTrackingModule import HandDetector
import mediapipe as mp
import time
from dollarpy import Recognizer, Template, Point
import Dollarpy as dp

#functions
def reverse_integer(f: int) -> int:
    if f == 0:
        return 1
    else:
        return 0


mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_holistic = mp.solutions.holistic

templates=[] #Template el training

vid = "D:\College\Semester 7\HCI\Project\Classify\VUP1.mp4"
points = dp.getPoints(vid,"Correct Volume UP")

tmpl_2 = Template('Correct Volume UP', points)
templates.append(tmpl_2)

vid = "D:\College\Semester 7\HCI\Project\Classify\VUP2.mp4"
points = dp.getPoints(vid,"Correct Volume UP")

tmpl_2 = Template('Correct Volume UP', points)
templates.append(tmpl_2)

vid = "D:\College\Semester 7\HCI\Project\Classify\VUP3.mp4"
points = dp.getPoints(vid,"Correct Volume UP")

tmpl_2 = Template('Correct Volume UP', points)
templates.append(tmpl_2)


#vol DOWNN

vid = "D:\College\Semester 7\HCI\Project\Classify\VDWN1.mp4"
points2 = dp.getPoints(vid,"Correct Volume Down")
tmpl_3 = Template('Correct Volume Down', points2)
templates.append(tmpl_3)

vid = "D:\College\Semester 7\HCI\Project\Classify\VDWN2.mp4"
points2 = dp.getPoints(vid,"Correct Volume Down")
tmpl_3 = Template('Correct Volume Down', points2)
templates.append(tmpl_3)

vid = "D:\College\Semester 7\HCI\Project\Classify\VDWN2.mp4"
points2 = dp.getPoints(vid,"Correct Volume Down")
tmpl_3 = Template('Correct Volume Down', points2)
templates.append(tmpl_3)

#print(points)









#Variables
width, height = 720 , 500
flag = 0

#Hand Detector
detector = HandDetector(detectionCon=0.8, maxHands=1)
#camera Setup
cap = cv2.VideoCapture(0)





cap.set(3,width )
cap.set(4,height )


while True:
    success, img = cap.read()
    #img = cv2.flip(img, 1)
    hands, img = detector.findHands(img) #FlipType = False
    #hands = detector.findHands(img, draw=False)  # no draw


    if hands:
        hand1 = hands[0]
        fingers1 = detector.fingersUp(hand1)
        lmList1 = hand1["lmList"] #Landmarks
        bbox1 = hand1["bbox"] # x,y,w,h
        Center1 = hand1["center"]  # cx , cy of the hand
        handtype1 = hand1["type"] # Left or Right
        #print(lmList1)
        #print(Center1)
        #cx , cy = hand['center']

        #Gesture1 - left
        if fingers1 == [1,0,0,0,0]: #Thumps up
            print("Thumbs up")



        #DollarPY
        mah = dp.getPoints(0,"Correct Volume UP")

        recognizer = Recognizer(templates)
        result = recognizer.recognize(mah)
        print(result[0])

        #Gestures

    cv2.imshow('Image',img)
    key = cv2.waitKey(1)
    if key == ord('q'):
        break
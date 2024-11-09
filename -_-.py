from encodings.utf_7 import encode
import cv2
from deepface import DeepFace
import json
import os
import numpy as np

#                  sssssssssss                        Kol da momken yetmeseh bas ashan bey add fe json file elly lesa ma3maltohosh

#kkkkkkkkkkkkkkkkkkkkkk                             mm Awel Hena


db_path = './Images/'
embedding_file = 'embeddings.json'
embeddings = []


for filename in os.listdir(db_path):
    #print(filename)
    if os.path.isdir(os.path.join(db_path, filename)):

        for img in os.listdir(os.path.join(db_path, filename)):
            print(img)
            if img.endswith('.jpg') or img.endswith('.png'):
                filepath = os.path.join(db_path, filename, img)
          
                try:
                    embedding = DeepFace.represent(img_path=filepath, model_name='VGG-Face', enforce_detection=False)
                    if embedding:  
                        embeddings.append({
                        'identity': filename,  
                        'embedding': embedding[0]['embedding']  
                        })
                except Exception as e:
                    print(f"Could not process {filename}: {e}")


with open(embedding_file, 'w') as f:
    json.dump(embeddings, f)

print(f"Embeddings successfully saved to {embedding_file}")

with open('embeddings.json', 'r') as f:
    saved_embeddings = json.load(f)
    
#sssssssssssssssssss                              Lehad Hena
attendance = []

cap = cv2.VideoCapture(0)
while True:
    ret, frame = cap.read()
    frame = cv2.resize(frame, (640, 480))


    res = DeepFace.find(frame,db_path='./Images/', enforce_detection=False, model_name='VGG-Face')

    if len(res[0]['identity']) > 0:
        name = res[0]['identity'][0].split('/')[-1].split('.')[0].split('\\')[0]
        xmin = int(res[0]['source_x'][0])
        ymin = int(res[0]['source_y'][0])

        w = res[0]['source_w'][0]
        h = res[0]['source_h'][0]
        xmax = int(xmin + w)
        ymax = int(ymin + h)
        if name not in attendance:
            attendance.append(name)
            with open('Names.txt', 'a') as file:
                file.write(name+"\n")

        cv2.rectangle(frame,(xmin,ymin),(xmax,ymax),(0,0,255),2)
        cv2.putText(frame,name ,(xmin + (xmax-xmin),ymin), cv2.FONT_HERSHEY_SIMPLEX, 2, (255,0,0), 2, cv2.LINE_AA )
    cv2.imshow("Login", frame)
    c = cv2.waitKey(1)

    if c == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()

#model = DeepFace.find(db_path='./Images', enforce_detection=False, model_name='VGG-Face')
#name = model[0]['identity'][0].split('/')[1].split('\\')[1]
#DeepFace.stream(r"D:\College\Semester 7\HCI\Project\Meow\Abo Nesma\.venv\Images")

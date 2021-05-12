# Cicada

Final Computer Science thesis for 5 points (Bagrut).

Trivia game, inspired by the legendary organization "Cicada 3301".

Project consists of a Firebase database, used to communicate with android devices.


2 players join the same session using a matchmaking process done by Firestore.
They have 10 seconds to answer any question, by pressing the submit button "שלח".

Whoever is closest to the answer shown in the middle, gets the point - first to 5 wins (there could be a tie).
Then, they have 5 seconds of "resting time" before the next question.

![image](https://user-images.githubusercontent.com/59180254/117958207-77cbbc00-b323-11eb-96fd-0e0e96f760b6.png)

After each game the data is saved on Firestore, and can be accessed on the "Me" page.

![image](https://user-images.githubusercontent.com/59180254/117959273-84044900-b324-11eb-9d8a-d374d0c82f55.png)

The goal for this project was to learn how to develop an android application, and learn how to use Firebase to communicate between devices and save data on the cloud.


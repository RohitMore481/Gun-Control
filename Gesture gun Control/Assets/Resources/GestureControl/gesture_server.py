from flask import Flask, jsonify
import cv2
import mediapipe as mp
import math
import threading

app = Flask(__name__)
cap = cv2.VideoCapture(0)
hands = mp.solutions.hands.Hands()
gesture = "none"
angle = 0

def get_angle(a, b):
    dx = b.x - a.x
    dy = b.y - a.y
    radians = math.atan2(dy, dx)
    degrees = math.degrees(radians)
    return degrees

def detect_gesture():
    global gesture, angle
    while True:
        success, frame = cap.read()
        if not success:
            continue
        frame = cv2.flip(frame, 1)
        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = hands.process(rgb)

        gesture = "none"
        angle = 0

        if results.multi_hand_landmarks:
            hand = results.multi_hand_landmarks[0]
            lm = hand.landmark

            # Convert to pixel coordinates
            h, w, _ = frame.shape
            wrist = (int(lm[0].x * w), int(lm[0].y * h))

            # New: "Thumb-up with closed fingers" pistol gesture
            thumb_up = lm[4].y < lm[3].y
            index_down = lm[8].y > lm[6].y
            middle_down = lm[12].y > lm[10].y
            ring_down = lm[16].y > lm[14].y

            if thumb_up and index_down and middle_down and ring_down:
                gesture = "pistol"

                # Angle from wrist to thumb
                dx = lm[4].x - lm[0].x
                dy = lm[4].y - lm[0].y
                radians = math.atan2(dy, dx)
                angle = math.degrees(radians)

                # Draw debug line and angle
                thumb_tip = (int(lm[4].x * w), int(lm[4].y * h))
                cv2.line(frame, wrist, thumb_tip, (0, 0, 255), 3)
                cv2.putText(frame, f"Angle: {int(angle)}", (wrist[0], wrist[1] - 20),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 0, 0), 2)

            # Draw hand landmarks always
            mp.solutions.drawing_utils.draw_landmarks(
                frame, hand, mp.solutions.hands.HAND_CONNECTIONS)

        # Display gesture text
        cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        cv2.imshow("Gesture Debug", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break


@app.route('/gesture')
def get_gesture():
    return jsonify({"gesture": gesture, "angle": angle})

if __name__ == '__main__':
    threading.Thread(target=detect_gesture, daemon=True).start()
    app.run(host='127.0.0.1', port=5000)

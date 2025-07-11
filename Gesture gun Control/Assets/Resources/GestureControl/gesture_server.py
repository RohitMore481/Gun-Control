from flask import Flask, jsonify
import cv2
import mediapipe as mp
import math
import threading

app = Flask(__name__)
cap = cv2.VideoCapture(0)
hands = mp.solutions.hands.Hands(min_detection_confidence=0.7, min_tracking_confidence=0.7)

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
            h, w, _ = frame.shape

            # Get landmark values
            index_tip_y = lm[8].y
            index_joint_y = lm[6].y
            thumb_tip_y = lm[4].y
            thumb_joint_y = lm[3].y

            # -- Logic flags --
            index_bent_enough = (index_tip_y - index_joint_y) > 0.015  # <- Reload threshold
            index_extended = index_tip_y < index_joint_y and abs(lm[8].x - lm[6].x) > 0.02
            index_up = index_tip_y < index_joint_y
            thumb_up = thumb_tip_y < thumb_joint_y
            thumb_hidden_or_down = thumb_tip_y > thumb_joint_y or abs(thumb_tip_y - thumb_joint_y) < 0.01

            # -- Priority order: Fire > Idle > Reload > None
            if index_extended and thumb_hidden_or_down:
                gesture = "fire"
            elif index_up and thumb_up:
                gesture = "idle"
            elif index_bent_enough:
                gesture = "reload"
            else:
                gesture = "none"

            # Angle calc
            dx = lm[8].x - lm[0].x
            dy = lm[8].y - lm[0].y
            angle = math.degrees(math.atan2(dy, dx))

            # Draw info
            wrist_px = (int(lm[0].x * w), int(lm[0].y * h))
            index_px = (int(lm[8].x * w), int(lm[8].y * h))
            cv2.line(frame, wrist_px, index_px, (0, 0, 255), 2)
            cv2.putText(frame, f"Angle: {int(angle)}", (wrist_px[0], wrist_px[1] - 20),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 0, 0), 2)

            mp.solutions.drawing_utils.draw_landmarks(
                frame, hand, mp.solutions.hands.HAND_CONNECTIONS)

        # Label on screen
        cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        cv2.imshow("Gesture Debug", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

@app.route('/gesture')
def get_gesture():
    return jsonify({
        "gesture": gesture,
        "angle": angle
    })

if __name__ == '__main__':
    threading.Thread(target=detect_gesture, daemon=True).start()
    app.run(host='127.0.0.1', port=5000)

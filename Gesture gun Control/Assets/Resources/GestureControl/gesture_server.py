from flask import Flask, jsonify
import cv2
import mediapipe as mp
import math
import threading

app = Flask(__name__)
cap = cv2.VideoCapture(0)
hands = mp.solutions.hands.Hands(min_detection_confidence=0.7,
                                  min_tracking_confidence=0.7,
                                  max_num_hands=2)

gesture = "none"
angle = 0

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

        if results.multi_hand_landmarks and results.multi_handedness:
            for i, hand_landmarks in enumerate(results.multi_hand_landmarks):
                hand_label = results.multi_handedness[i].classification[0].label  # 'Left' or 'Right'
                lm = hand_landmarks.landmark
                h, w, _ = frame.shape

                # Right hand – Gesture detection
                if hand_label == "Right":
                    index_tip_y = lm[8].y
                    index_joint_y = lm[6].y
                    thumb_tip_y = lm[4].y
                    thumb_joint_y = lm[3].y

                    index_bent = (index_tip_y - index_joint_y) > 0.015
                    index_extended = index_tip_y < index_joint_y and abs(lm[8].x - lm[6].x) > 0.02
                    thumb_up = thumb_tip_y < thumb_joint_y
                    thumb_hidden_or_down = thumb_tip_y > thumb_joint_y or abs(thumb_tip_y - thumb_joint_y) < 0.01

                    if index_extended and thumb_hidden_or_down:
                        gesture = "fire"
                    elif index_tip_y < index_joint_y and thumb_up:
                        gesture = "idle"
                    elif index_bent:
                        gesture = "reload"
                    else:
                        gesture = "none"

                # Left hand – Rotation control using thumb position
                elif hand_label == "Left":
                    fingers_extended = 0
                    for fid in [8, 12, 16, 20]:  # index to pinky tips
                        if lm[fid].y < lm[fid - 2].y:
                            fingers_extended += 1

                    if fingers_extended <= 1:  # fingers folded
                        thumb_diff = lm[4].x - lm[0].x  # thumb.x - wrist.x

                        if abs(thumb_diff) < 0.03:
                            angle = 0  # Idle
                        elif thumb_diff > 0:
                            angle = 90  # Right
                        else:
                            angle = -90  # Left

                        # Debug line
                        wrist_px = (int(lm[0].x * w), int(lm[0].y * h))
                        thumb_px = (int(lm[4].x * w), int(lm[4].y * h))
                        cv2.line(frame, wrist_px, thumb_px, (255, 255, 0), 2)

                        cv2.putText(frame, f"Thumb-X: {thumb_diff:.2f}", (10, 100),
                                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 255), 2)

                # Draw landmarks
                mp.solutions.drawing_utils.draw_landmarks(
                    frame, hand_landmarks, mp.solutions.hands.HAND_CONNECTIONS)

        # Debug HUD
        cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
        cv2.putText(frame, f"Angle: {int(angle)}", (10, 70),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.8, (255, 255, 0), 2)

        cv2.imshow("Gesture Debug", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            cap.release()
            cv2.destroyAllWindows()
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

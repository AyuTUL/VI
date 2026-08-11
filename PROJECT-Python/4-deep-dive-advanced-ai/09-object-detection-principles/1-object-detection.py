# Object detection :
#     computer vision where model answers 2 questions:
#         - what object is present? classification
#         - where object is? localization

# hugging face, pickle
# YOLO = You Only Look Once
#     used for real time object detection models

# modern YOLO versions are used for:
#     object detection
#     tracking
#     post estimation

# YOLO Basic Working:
#     Image -> YOLO -> feature extraction -> prediction -> boxes, classes, confidence -> NMS -> final result

# NMS = Non Maximum Suppression
#     keeps strongest detection and removes highly overlapping weaker detections

# IoU  = Intersection Over Union
#     measures how much 2 boxes overlap

# NMS uses IoU
# if overlap is very high, the detector may decide:
#     these probably represent the same objects

# YOLO : loads and runs YOLO object detection model
# CV2 : open cv is used for webcam, displaying frames, drawing boxes, mouse events
from ultralytics import YOLO
import cv2

# Load YOLO model
# yolov8n.pt : YOLO version 8 nano . PyTorch , pre-trained model
model = YOLO("yolov8n.pt")

# Open webcam, default / 1st webcam
cap = cv2.VideoCapture(0)

# Exit button settings
exit_button_pos = (500, 20)
exit_button_size = (120, 50)

window_name = "Bottle Detection"

running = True


# Mouse click function
def mouse_click(event, x, y, flags, param):
    global running

    if event == cv2.EVENT_LBUTTONDOWN:

        # Check if click is inside exit button
        if (
            exit_button_pos[0] <= x <= exit_button_pos[0] + exit_button_size[0]
            and exit_button_pos[1] <= y <= exit_button_pos[1] + exit_button_size[1]
        ):
            running = False


# Create window
cv2.namedWindow(window_name)
cv2.setMouseCallback(window_name, mouse_click)


while running:

    ret, frame = cap.read()

    if not ret:
        break

    # Run YOLO detection
    results = model(frame)

    bottle_count = 0

    # Loop through detections
    for result in results:

        for box in result.boxes:

            cls_id = int(box.cls[0])
            label = model.names[cls_id]

            # Detect only bottles
            if label == "bottle":

                bottle_count += 1

                # Bounding box coordinates
                x1, y1, x2, y2 = box.xyxy[0]  # top-left (x1,y1)
                # (x2,y2)= bottom-right

                # Confidence score
                conf = float(box.conf[0])

                # Draw rectangle
                cv2.rectangle(
                    frame, (int(x1), int(y1)), (int(x2), int(y2)), (255, 255, 0), 2
                )

                # Label text
                cv2.putText(
                    frame,
                    f"{label} {conf:.2f}",
                    (int(x1), int(y1) - 10),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.6,
                    (0, 255, 0),
                    2,
                )

    # Show bottle count
    cv2.putText(
        frame,
        f"Total Bottles: {bottle_count}",
        (20, 50),
        cv2.FONT_HERSHEY_SIMPLEX,
        1,
        (0, 0, 255),
        3,
    )

    # Draw EXIT button
    cv2.rectangle(
        frame,
        exit_button_pos,
        (
            exit_button_pos[0] + exit_button_size[0],
            exit_button_pos[1] + exit_button_size[1],
        ),
        (0, 0, 255),
        -1,
    )

    # EXIT text
    cv2.putText(
        frame,
        "EXIT (Q)",
        (exit_button_pos[0] + 10, exit_button_pos[1] + 32),
        cv2.FONT_HERSHEY_SIMPLEX,
        0.7,
        (255, 255, 255),
        2,
    )

    # Show frame
    cv2.imshow(window_name, frame)

    # Press Q to quit
    if cv2.waitKey(1) & 0xFF == ord("q"):
        break


# Cleanup
cap.release()
cv2.destroyAllWindows()

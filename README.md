# 🔫 Gesture-Controlled Gun Demo (Unity)

This is a Unity-based prototype where the player controls gun actions (shooting, switching weapons, and rotation) using **hand gestures**. It serves as a creative demonstration of gesture-driven input applied in a 2D shooting environment.

---

## 🎮 Features

- ✋ **Gesture-Based Controls**
  - Right hand: performs *pistol* gesture to shoot
  - Left hand: performs *V* gesture to switch guns
  - Second hand (horizontal movement): rotates the player/soldier
  
- 🔫 **Multiple Guns**
  - Each gun has its own:
    - Name
    - Idle, firing, and reload sprite animations
    - Bullet round capacity
    - Bullet speed
    - Custom firePoint (position and rotation)

- 💥 **Shooting System**
  - Bullets are spawned from the gun’s muzzle
  - Travel in the direction of the gun (green arrow / local `up`)
  - Automatically destroyed once off-screen

- 🔁 **Reload System**
  - UI displays bullets left as `10 / 100` format
  - When bullets run out, a “Reload” text appears
  - Reloading restores full magazine (unlimited reloads)

- 🧠 **Smart Gun Switching**
  - Switch between guns with the "V" gesture
  - Automatically updates animation and bullet logic

- 🧩 Built using:
  - Unity 2D
  - C#
  - TextMeshPro for UI
  - External Python server for gesture recognition

---

## 🛠️ How It Works

### 🔗 Gesture Input
The Unity app connects to a local Flask server (or similar) that interprets webcam hand gestures and sends them as strings like `"pistol"`, `"v"`, etc.

### 🔧 Gun Data (Scriptable Style)
Each gun is defined via a `GunData` class that includes animation frames, capacity, and a `firePoint` for bullet origin.

### 🧱 Bullet Mechanics
Bullets are spawned with velocity set to the direction of `firePoint.up`. Gravity is disabled to allow straight-line travel.

---

## 🧪 Demo Use Cases

- 🔬 Gesture research / interaction demo
- 🎯 Game prototype for hand-controlled shooting mechanics
- 🧠 AI/AR experimentation using real-time input systems

---

## 🚀 Getting Started

1. Clone the repo into Unity
2. Assign guns and firePoints in the Inspector
3. Set up your gesture detection server to return gestures
4. Press Play in Unity and start gesturing!

---

## 📂 Project Structure

Assets/
├── Scripts/
│ ├── GunController.cs
│ ├── Bullet.cs
│ └── GestureInput.cs
├── Prefabs/
│ ├── Guns (Pistol, Rifle, etc.)
│ └── Bullet.prefab
├── Resources/
│ └── Sprites & Animations
└── UI/
└── Bullet Text, Reload Text


---

## 👑 Credits

- Developed by: **You** 💪
- Powered by: Unity + C# + Python Flask
- Idea & Logic: Gesture-based shooting experiment

---

## 🧠 Future Ideas

- Add enemies with hit detection
- Add gun sound effects and fire rate control
- Show gesture icons on-screen for feedback
- Create a WebGL build for showcasing

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

You are free to:
- Use 🟢
- Modify 🛠️
- Share 📤
- Even commercially 💼

Just make sure to include the original license and give proper credit.


---

## 🎨 Sprites used in this project:


- **Animated Top Down Survivor Player** by [Sanderfrenken](https://opengameart.org/users/sanderfrenken)  
  Source: [OpenGameArt.org](https://opengameart.org/content/animated-top-down-survivor-player)  
  License: [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/)

These assets are used under the Creative Commons Attribution license. All rights belong to the original artist.

---



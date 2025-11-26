# 🪽 The Last Seraphim — Tactical Sci-Fi Strategy Game

**The Last Seraphim** is a personal game project developed independently using **Unity**, blending tactical turn‑based combat with a dark science‑fiction atmosphere.  
Its purpose is both artistic and technical: to demonstrate clean, scalable gameplay architecture and robust combat logic fully driven by code.

---

## 🛰️ Overview

The game focuses on **tactical decision‑making**, **modular abilities**, and **reactive visual feedback**.  
Over several months of development, this project has grown into a flexible and maintainable foundation for a full tactical combat experience, featuring:

- A modular turn‑based combat system  
- Custom animation playback controlled entirely through logic (no Animator transitions)  
- Dynamic camera systems and responsive combat feedback  
- Abilities, passives, hit/wound/save rolls, crit systems, and damage typing  
- UI, FX, and polished gameplay feedback loops  
- A reusable and expandable core architecture

---

## 🧱 Project Structure

```
TheLastSeraphim/
│
├── Scripts/
│   ├── Core/                → combat logic, turn flow, architecture
│   ├── Abilities/           → modular ScriptableObjects, effects, logic
│   ├── Characters/          → stats, rolls, damage system, leveling
│   ├── UI/                  → floating text, HUD, bars, feedback
│   ├── Interaction/         → selection, targeting, controls
│   └── Animation/           → custom logic-driven animation controller
│
├── Art/                     → sprites, VFX, shaders
├── Prefabs/                 → modular combat units and UI elements
├── Scenes/                  → combat scenarios and test environments
└── ...
```

---

## ✨ Features

- **Modular Ability System**  
  Each ability is a ScriptableObject supporting multiple actions, events, and timings.

- **Advanced Damage Resolution**  
  Includes hit roll → wound roll → save throw logic, bonuses, resistances, and crits.

- **Animation Without Animator**  
  Combat animations are played and synchronized purely through code, using timing windows.

- **Dynamic Floating Text Manager**  
  Buffered pop-ups, crit indicators, elemental damage colors, total damage summaries, etc.

- **Health Bar System**  
  Smooth delayed loss bar animations synchronized with impact windows.

- **Scalable Architecture**  
  Every system is reusable, isolated, and designed to scale with content.

- **Cinematic Camera Feedback**  
  Camera reactions and focus during attacks.

- **Clean UX and Combat Workflow**  
  Ability preview, hit chance display, targeting outlines, and more.

---

## 🚀 Getting Started

### 1️⃣ Clone the Repository  
```
git clone https://github.com/H0DEI/TheLastSeraphim.git
```

### 2️⃣ Open in Unity  
Use **Unity 2022 LTS** or newer (recommended).

### 3️⃣ Run the Project  
Open the main combat scene and press **Play**.

---

## 🛠️ Technical Highlights

### 🔹 Custom Animation Timing System  
Animations are driven by code with **impact windows**, syncing:
- damage application  
- floating text  
- total damage  
- health bar animation  

### 🔹 Combat Architecture  
Fully modular, with:
- actions (hit, wound, heal, effects…)
- targeting modes  
- timing buffers  
- reusable roll system  
- extensible damage types  

### 🔹 Floating Text & Feedback  
Every hit generates feedback according to:
- damage type  
- crit / non‑crit  
- healing  
- resist / avoid / invulnerable  

Buffered windows ensure perfect sync with animation events.

---

## 🧬 Why This Project Matters

This repository demonstrates experience in:

- Object‑oriented game architecture  
- Scalable system design  
- Complex gameplay loops  
- Unity tooling and optimization  
- Clean, maintainable C# patterns (SOLID‑inspired)  
- Real‑time feedback and UX design  
- Animation, camera, and FX coordination  

It represents a **portfolio‑grade gameplay framework**, ready to be expanded into a full title.

---

## 🎥 Trailer

👉 *(Insert link here to your gameplay trailer)*

---

## 📄 License  
MIT License — feel free to explore, learn, and adapt parts of the architecture.

---

## 💬 Contact  
If you’d like to discuss the project, collaborate, or review the architecture:  
**LinkedIn:** https://www.linkedin.com/in/your-profile/  
**Email:** your.email@example.com  

# Food-Ordering-with-Voice-Assistant
It provides menu management, cart operations, and item suggestions via voice commands, but **does not support real-time speech responses**. The bot only returns **JSON data** based on your menu.
---
## Features
- Fetches menu items from `/Home/GetMenu`.
- Supports **cart management** (add/remove items).
- Highlights and displays the "heard" item in the UI.
- Basic voice recognition to detect item names.
- Returns **textual responses only** via OpenAI API.
---
## Technologies Used
- ASP.NET MVC (C#)
- JavaScript (frontend UI)
- HTML/CSS
- OpenAI GPT-4o-mini-realtime-preview API (text only)
- Newtonsoft.Json for JSON handling
---
## Demo

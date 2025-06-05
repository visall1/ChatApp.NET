# 🗨️ Chat Client and Server Application

## Overview

This project is a **C# TCP-based chat application** that allows multiple clients to connect to a central server and communicate in real time. The system is designed with modular network logic, separating communication from the UI, making it easy to extend or adapt to different platforms.

## Features

### ✅ Server
- Accepts multiple client connections concurrently.
- Associates each client IP with a unique username.
- Maintains and broadcasts a real-time list of connected users.
- Logs connection and disconnection events.
- Broadcasts chat messages to all clients.
- Notifies clients when a user is typing.

### ✅ Client
- Connects to the server using IP address and custom username.
- Displays a real-time chat window.
- Shows a dynamic list of online users.
- Indicates when another user is typing.
- User-friendly graphical interface using **Windows Forms**.

## Technologies Used

- Language: **C#**
- Platform: **.NET Framework / .NET Core**
- UI: **Windows Forms**
- Networking: **System.Net.Sockets (TCP)**

## How It Works

### 1. Server
The server listens on a specified port and waits for incoming TCP client connections. Each connected client is managed in a separate thread, allowing real-time communication. When a user sends a message or starts typing, the server relays that information to all other connected clients.

### 2. Client
Each client connects to the server, sets a unique username, and starts listening for messages. Clients can send messages, receive messages, and see who is online or typing in real time.

## Setup and Usage

### 🛠 Requirements
- .NET 6.0 SDK or higher
- Visual Studio 2022 or higher

### 🚀 How to Run

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/ChatClientServerApp.git
   ```

2. **Build the Server:**
   - Open `ChatServer` project in Visual Studio.
   - Build and run.

3. **Build the Client:**
   - Open `ChatClient` project in Visual Studio.
   - Enter the server IP and your username.
   - Start chatting!

### 📌 Note
- Ensure the server is running before connecting the client.
- Port and IP must match in both client and server configurations.
- Typing indicator appears after a short pause while typing.

## Screenshots

> *(Insert client and server UI screenshots here)*

## Future Enhancements

- Encryption for secure message transmission.
- File sharing support.
- Private (one-to-one) messaging.
- Cross-platform UI using WPF or MAUI.

## License

This project is open-source and free to use under the [MIT License](LICENSE).

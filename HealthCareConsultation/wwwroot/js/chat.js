// Initialize SignalR Connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Global variables
let appointmentId;
let currentUser;
let typingTimer;

// DOM Elements
const chatMessages = document.getElementById("chatMessages");
const messageInput = document.getElementById("messageInput");
const sendButton = document.getElementById("sendButton");
const typingIndicator = document.getElementById("typingIndicator");

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", function () {
    // Get values from ViewBag (these should be set in the view)
    appointmentId = window.appointmentId || "1"; // fallback value
    currentUser = window.currentUser || "Anonymous"; // fallback value

    start();
    setupEventListeners();
});

// Start SignalR Connection
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");

        // Join room
        await connection.invoke("JoinRoom", appointmentId.toString());

        // Add system message
        addMessageToChat("System", "Connected to chat room", false);

    } catch (err) {
        console.error("SignalR Connection Error:", err);
        addMessageToChat("System", "Connection failed. Retrying...", false);
        setTimeout(start, 5000);
    }
}

// Handle connection close
connection.onclose(async () => {
    console.log("SignalR Connection Closed. Reconnecting...");
    await start();
});

// Setup event listeners
function setupEventListeners() {
    // Send button click
    sendButton.addEventListener("click", sendMessage);

    // Enter key press
    messageInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") {
            sendMessage();
        }
    });

    // Typing indicator
    messageInput.addEventListener("input", () => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.invoke("SendTyping", appointmentId.toString(), currentUser);
        }

        clearTimeout(typingTimer);
        typingTimer = setTimeout(() => {
            if (typingIndicator) {
                typingIndicator.textContent = "";
            }
        }, 2000);
    });
}

// Send Message Function
async function sendMessage() {
    const message = messageInput.value.trim();
    if (message === "" || connection.state !== signalR.HubConnectionState.Connected) {
        return;
    }

    try {
        await connection.invoke("SendMessage",
            appointmentId.toString(),
            currentUser,
            message);

        messageInput.value = "";

    } catch (err) {
        console.error("Send Message Error:", err);
        addMessageToChat("System", "Failed to send message", false);
    }
}

// SignalR Event Handlers
connection.on("ReceiveMessage", (sender, message) => {
    addMessageToChat(sender, message, sender === currentUser);

    // Clear typing indicator when message is received
    if (typingIndicator && sender !== currentUser) {
        typingIndicator.textContent = "";
    }
});

connection.on("ShowTyping", (sender) => {
    if (sender !== currentUser && typingIndicator) {
        typingIndicator.textContent = `${sender} is typing...`;
    }
});

connection.on("ReceiveSystemMessage", (message) => {
    addMessageToChat("System", message, false);
});

// Add Message to Chat UI
function addMessageToChat(sender, message, isCurrentUser) {
    if (!chatMessages) return;

    const messageElement = document.createElement("div");
    messageElement.className = `message ${isCurrentUser ? "sent" : "received"}`;

    const timestamp = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    messageElement.innerHTML = `
        <div class="message-sender">${escapeHtml(sender)}</div>
        <div class="message-text">${escapeHtml(message)}</div>
        <div class="message-time">${timestamp}</div>
    `;

    chatMessages.appendChild(messageElement);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Utility function to escape HTML
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, function (m) { return map[m]; });
}
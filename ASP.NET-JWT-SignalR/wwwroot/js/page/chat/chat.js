

const chatWindow = document.getElementById("chatWindow");

/**
 * Initialize SignalR
 */
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

function ensureConnectionStarted() {
    if (connection.state === "Disconnected") {
        console.log("SignalR connection start")
        return connection.start();
    }
    return Promise.resolve();
}

ensureConnectionStarted()
    .then(() => connection.invoke("JoinGroup", userGroup))
    .then(() => console.log("Joined"+ userGroup))
    .catch(err => console.error(err));


/**
 * SignalR receive response
 */
connection.on("ReceiveMessage", (user, message) => {
     //console.log("ReceiveMessage " + message);
     if (user === currentUser) { 
        // This is MY message → ignore it 
        return; }
    addIncoming(user, message);
});

connection.on("ReceiveSystemMessage", (user, message) => {
    //console.log("ReceiveSystemMessage "+ message);
    addSystem(message)
});

/**
 * Send message to SignalR
 */
function sendMessage() {
    const input = document.getElementById("messageInput");
    const message = input.value;
    if (message.trim() === "") return;
    // Add to UI immediately 
    addOutgoing(userName, message);
    connection.invoke("SendMessageToGroup",userGroup, userName, message)
            .then(() => {})
            .catch(err => console.error(err));
    document.getElementById("messageInput").value=null;
}

function addIncoming(user, message) {
    chatWindow.innerHTML += `
        <div class="d-flex flex-column mb-2">
            <div class="username">
                ${user}
            </div>
            <div class="chat-message incoming">    
                ${message}
            </div>
        </div>`;
}

function addOutgoing(user, message) {
    chatWindow.innerHTML += `
        <div class="d-flex flex-column mb-2">
            <div class="chat-message outgoing">
                ${message}
            </div>
        </div>`;
}

function addSystem(message) {
    chatWindow.innerHTML += `
        <div class="system-message">
            ${message}
        </div>`;
}

/**
 * When user click Enter, send user input
 */
document.getElementById('messageInput').addEventListener('keydown', function(event) {
if (event.key === 'Enter') {
    event.preventDefault(); // Prevent the default Enter behavior (newline)
    sendMessage();
}
});

/**
 * User click logout
 */

document.getElementById("logoutBtn").addEventListener("click", async () => {
try {
    await connection.stop();
} catch (e) {
    console.error("SignalR disconnect error:", e);
}

window.location.href = "/home/Logout";
});





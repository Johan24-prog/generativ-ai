const form = document.querySelector("#chat-form");
const input = document.querySelector("#message-input");
const messages = document.querySelector("#messages");
const sendButton = document.querySelector("#send-button");

function addMessage(text, type) {
  const message = document.createElement("article");
  message.className = `message ${type}-message`;
  message.innerHTML = `<div class="avatar">${type === "user" ? "DU" : "AI"}</div><div class="bubble"></div>`;
  message.querySelector(".bubble").textContent = text;
  messages.appendChild(message);
  messages.scrollTop = messages.scrollHeight;
  return message;
}

function resizeInput() {
  input.style.height = "auto";
  input.style.height = `${Math.min(input.scrollHeight, 150)}px`;
}

input.addEventListener("input", resizeInput);
input.addEventListener("keydown", (event) => {
  if (event.key === "Enter" && !event.shiftKey) {
    event.preventDefault();
    form.requestSubmit();
  }
});

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  const message = input.value.trim();
  if (!message || sendButton.disabled) return;

  addMessage(message, "user");
  input.value = "";
  resizeInput();
  sendButton.disabled = true;
  const typing = addMessage("Skriver...", "assistant");
  typing.querySelector(".bubble").classList.add("typing");

  try {
    const response = await fetch("/api/chat", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ message }),
    });
    const responseText = await response.text();
    let data;
    try {
      data = JSON.parse(responseText);
    } catch {
      throw new Error("Servern skickade ett ogiltigt svar.");
    }
    typing.remove();
    if (!response.ok) {
      throw new Error(data.detail || data.title || "Något gick fel.");
    }
    addMessage(data.message, "assistant");
  } catch (error) {
    typing.remove();
    addMessage(error.message || "Kunde inte kontakta AI:n.", "assistant")
      .querySelector(".bubble")
      .classList.add("error");
  } finally {
    sendButton.disabled = false;
    input.focus();
  }
});

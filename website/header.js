import Cookies from "./_cookies";
import { send } from "./_utils";

/**@type {HTMLDivElement} */
let loggedOutDiv = document.getElementById("loggedOutDiv");

/**@type {HTMLButtonElement} */
let logInButton = document.getElementById("logInButton");

/**@type {HTMLButtonElement} */
let signUpButton = document.getElementById("signUpButton");

/**@type {HTMLDivElement} */
let loggedInDiv = document.getElementById("loggedInDiv");

/**@type {HTMLDivElement} */
let greetingDiv = document.getElementById("greetingDiv");

/**@type {HTMLButtonElement} */
let logOutButton = document.getElementById("logOutButton");

logInButton.onclick = function () {
  top.location.href = "logIn.html";
}

signUpButton.onclick = function () {
  top.location.href = "signUp.html";
}

logOutButton.onclick = function () {
  Cookies.remove("id");
  top.location.href = "index.html";
}

async function autoLogIn() {
  let id = Cookies.get("id");

  if (id == undefined) {
    loggedInDiv.classList.add("hidden");
    Cookies.remove("id");
    return;
  }

  /**@type {{username: string} | null} */
  let user = await send("/autoLogIn", id);

  if (user == null) {
    loggedInDiv.classList.add("hidden");
    Cookies.remove("id");
    return;
  }

  loggedOutDiv.classList.add("hidden");
  greetingDiv.innerText = "Welcome, " + user.username + "!";
}

autoLogIn();
import { send } from "./_utils";

/**@type {HTMLInputElement} */
let titleInput = document.getElementById("titleInput");

/**@type {HTMLInputElement} */
let imageSourceInput = document.getElementById("imageSourceInput");

/**@type {HTMLTextAreaElement} */
let descriptionTextarea = document.getElementById("descriptionTextarea");

/**@type {HTMLButtonElement} */
let addButton = document.getElementById("addButton");

addButton.onclick = function () {

  let book = {
    title: titleInput.value,
    imageSource: imageSourceInput.value,
    description: descriptionTextarea.value
  };

  send("/addBook", book);

  top.location.href = "index.html";
}
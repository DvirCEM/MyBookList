import Cookies from "./_cookies";
import { send } from "./_utils";

/**
 * @typedef Preview
 * @property {number} id
 * @property {string} title
 * @property {string} imageSource
 * */

/**@type {HTMLHeadingElement} */
let favoritesH2 = document.getElementById("favoritesH2");

/**@type {HTMLDivElement} */
let favoritesContainer = document.getElementById("favoritesContainer");

/**@type {HTMLHeadingElement} */
let moreBooksH2 = document.getElementById("moreBooksH2");

/**@type {HTMLDivElement} */
let previewsContainer = document.getElementById("previewsContainer");

/**@type {string | undefined} */
let userId = Cookies.get("id");

/**@type {Preview[]} */
let previews = await send("/getPreviews");

/**
 * @param {Preview} preview
 * @returns {HTMLAnchorElement} 
 */
function createPreviewA(preview) {
  let a = document.createElement("a");
  a.classList.add("preview");
  a.href = "book.html?bookId=" + preview.id;

  let img = document.createElement("img");
  img.classList.add("bookImage");
  img.src = preview.imageSource;
  a.appendChild(img);

  let titleDiv = document.createElement("div");
  titleDiv.innerText = preview.title;
  a.appendChild(titleDiv);

  return a;
}

async function generatePreviewsForUser() {
  for (let i = 0; i < previews.length; i++) {
    let previewA = createPreviewA(previews[i]);

    /**@type {boolean} */
    let isFavorite = await send("/getIsFavorite", {
      userId: userId,
      bookId: previews[i].id
    });

    console.log(isFavorite);

    if (isFavorite) {
      console.log("favorite");
      favoritesContainer.appendChild(previewA);
    }
    else {
      previewsContainer.appendChild(previewA);
    }
  }
}

async function generatePreviews() {
  for (let i = 0; i < previews.length; i++) {
    let previewA = createPreviewA(previews[i]);
    previewsContainer.appendChild(previewA);
  }
}


if (userId != undefined) {
  generatePreviewsForUser();
}
else {
  favoritesH2.classList.add("hidden");
  favoritesContainer.classList.add("hidden");
  moreBooksH2.classList.add("hidden");
  generatePreviews();
}
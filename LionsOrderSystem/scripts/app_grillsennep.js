let body = document.querySelector('body')
let basketList = document.querySelector('basket-list')
let productElements = [];
let basketItems = [];

async function fetchProductsAll() {
    fetch('https://localhost:44373/api/Products/GetAll/GrillSennep')
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        if (data.active == true) {
            const elementWithOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="empty">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>Pris kr. ${data.normalPricePerPiece} pr. styk - ${data.offerMinPieces} styk for kr. ${data.offerMinPieces * data.offerPricePerPiece} - derudover ${data.offerPricePerPiece} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value="0"> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `
            const elementWithoutOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="empty">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>Pris kr. ${data.normalPricePerPiece} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value="0"> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `

            const elementSoldOut = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="empty">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>UDSOLGT !!!</p>
                </div>
            </a>
            `

            productElements = data.offerMinPieces > 0 && data.offerPricePerPiece > 0 ? elementWithOffer : elementWithoutOffer;
            if (data.soldOut == true) { productElements = elementSoldOut;}
            document.querySelector(".product-section").insertAdjacentHTML('beforeend', productElements);
        }
    }))
    .catch(error => console.log(error));
}

function showBasketTab() {
    body.classList.toggle('showBasket')
}

function hideBasketTab() {
    body.classList.toggle('showBasket')
}

function addToBasket(button) {
    var buttonId = button.id;
    var quantityInputId = "";
    var quantityInput = "";
    var quantity = "";

    quantityInputId = buttonId + "Qty";
    quantityInput = document.getElementById(quantityInputId);
    quantity = parseInt(quantityInput.value);

    console.log("Product added to basket"); // Implementation for adding product to basket
    console.log("Button Id: " + buttonId); // Implementation for adding product to basket
    console.log("Input Id: " + quantityInputId); // Implementation for adding product to basket
    console.log("Quantity: " + quantity); // Implementation for adding product to basket}
}

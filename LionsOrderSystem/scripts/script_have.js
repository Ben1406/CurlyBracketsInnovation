let body = document.querySelector('body')
let basketList = document.querySelector('basket-list')
let productElements = [];
let basketItems = [];

function pageLoaded() {
    fetchProductsAll();
    displayBasketItemCount();
}

async function fetchProductsAll() {
    await fetch('https://localhost:44373/api/Products/GetAll/Have')
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        if (data.active == true) {
            const elementWithOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>Pris kr. ${data.normalPricePerPiece} pr. styk - ${data.offerMinPieces} styk for kr. ${data.offerMinPieces * data.offerPricePerPiece} - derudover ${data.offerPricePerPiece} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value=""> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `
            const elementWithoutOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>Pris kr. ${data.normalPricePerPiece} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value=""> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `

            const elementSoldOut = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>UDSOLGT !!!</p>
                </div>
            </a>
            `

            const elementNotReleased = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.imageName} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.description}</p>
                    <p>${data.description2}</p>
                    <p>IKKE UDKOMMET ENDNU !!!</p>
                </div>
            </a>
            `

            productElements = data.offerMinPieces > 0 && data.offerPricePerPiece > 0 ? elementWithOffer : elementWithoutOffer;
            if (data.soldOut == true) { productElements = elementSoldOut;}
            if (data.notReleased == true) { productElements = elementNotReleased;}
            document.querySelector(".product-section").insertAdjacentHTML('beforeend', productElements);
        }
    }))
    .catch(error => console.log(error));
}

function showBasket() {
    window.location.href = "basket.html"
}

async function addToBasket(button) {
    var selectedProductId = button.id;
    var quantityInputId = "";
    var quantityInput = "";
    var quantity = "";

    quantityInputId = selectedProductId + "Qty";
    quantityInput = document.getElementById(quantityInputId);
    quantity = parseInt(quantityInput.value);

    if (isNaN(quantity) || quantity <= 0) {
        alert("Venligst indtast en gyldig mængde større end 0");
        return;
    }

    let basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    const existingBasketIndex = basket.findIndex(item => item.id === selectedProductId);

    if (existingBasketIndex !== -1 && basket[existingBasketIndex].quantity > 0) {
        basket[existingBasketIndex].quantity += quantity;
    } else {
        await fetch('https://localhost:44373/api/Products/GetById/' + selectedProductId)
        .then(res => { return res.json(); })
        .then(productItem => {
            basket.push({
                id: selectedProductId,
                description: productItem.description,
                description2: productItem.description2,
                imageName: productItem.imageName,
                normalPricePerPiece: productItem.normalPricePerPiece,
                offerPricePerPiece: productItem.offerPricePerPiece,
                offerMinPieces: productItem.offerMinPieces,
                quantity: quantity
            });
        });
    }

    sessionStorage.setItem("basket", JSON.stringify(basket));

    const productQty = document.getElementById(selectedProductId + "Qty");
    productQty.value = "";

    displayBasketItemCount();
}

function displayBasketItemCount() {
    const basketItemCount = document.getElementById("basket-item-count");
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    const basketTotal = basket.reduce((total, item) => total + item.quantity, 0);
    basketItemCount.textContent = basketTotal;
}

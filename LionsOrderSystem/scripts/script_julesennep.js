let body = document.querySelector('body')
let basketList = document.querySelector('basket-list')
let productElements = [];
let basketItems = [];

function pageLoaded() {
    fetchProductsAll();
    displayBasketItemCount();
}

async function fetchProductsAll() {
    await fetch('https://localhost:44373/api/Products/GetAllProducts/JuleSennep')
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        if (data.aktiv == true) {
            const elementWithOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.billedenavn} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.beskrivelse}</p>
                    <p>${data.beskrivelse2}</p>
                    <p>Pris kr. ${data.normalprisperstyk} pr. styk - ${data.tilbudminstyk} styk for kr. ${data.tilbudminstyk * data.tilbudsprisperstyk} - derudover ${data.normalprisperstyk} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value=""> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `
            const elementWithoutOffer = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.billedenavn} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.beskrivelse}</p>
                    <p>${data.beskrivelse2}</p>
                    <p>Pris kr. ${data.normalprisperstyk} pr. styk</p>
                    <p class="product-quantity">Antal: <input class="product-quantity-input" id=${data.id}Qty type="number" min="0" value=""> <button class="button-add" id=${data.id} onclick="addToBasket(this)">Læg i kurv</button></p>
                </div>
            </a>
            `

            const elementSoldOut = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.billedenavn} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.beskrivelse}</p>
                    <p>${data.beskrivelse2}</p>
                    <p>UDSOLGT !!!</p>
                </div>
            </a>
            `

            const elementNotReleased = `
            <a class="product-card">
                <img class="product-logo" src=images/${data.billedenavn} alt="venter på foto">
                <div class="product-text">
                    <p class="product-title">${data.beskrivelse}</p>
                    <p>${data.beskrivelse2}</p>
                    <p>IKKE UDKOMMET ENDNU !!!</p>
                </div>
            </a>
            `

            productElements = data.tilbudminstyk > 0 && data.normalprisperstyk > 0 ? elementWithOffer : elementWithoutOffer;
            if (data.udsolgt == true) { productElements = elementSoldOut;}
            if (data.ikkefrigivet == true) { productElements = elementNotReleased;}
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

    if (existingBasketIndex !== -1 && basket[existingBasketIndex].antal > 0) {
        basket[existingBasketIndex].antal += quantity;
    } else {
        await fetch('https://localhost:44373/api/Products/GetProductById/' + selectedProductId)
        .then(res => { return res.json(); })
        .then(productItem => {
            basket.push({
                id: selectedProductId,
                beskrivelse: productItem.beskrivelse,
                beskrivelse2: productItem.beskrivelse2,
                billedenavn: productItem.billedenavn,
                normalprisperstyk: productItem.normalprisperstyk,
                tilbudsprisperstyk: productItem.tilbudsprisperstyk,
                tilbudminstyk: productItem.tilbudminstyk,
                antal: quantity
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
    const basketTotal = basket.reduce((total, item) => total + item.antal, 0);
    basketItemCount.textContent = basketTotal;
}

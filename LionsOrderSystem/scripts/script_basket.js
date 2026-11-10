let body = document.querySelector('body')

function pageLoaded() {
    displayBasketItemCount();
    displayBasketItems();
}

function showBasket() {
    window.location.href = "basket.html"
}

function displayBasketItemCount() {
    const basketItemCount = document.getElementById("basket-item-count");
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    const basketTotal = basket.reduce((total, item) => total + item.quantity, 0);
    basketItemCount.textContent = basketTotal;
}

function displayBasketItems() {
    const grandtotalElement = document.getElementById("grandtotal");
    const basketItemsContainer = document.querySelector(".basket-items");
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];

    basketItemsContainer.innerHTML = "";

    var grandtotal = 0;

    if (basket.length === 0) {
        const grandtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(grandtotal);
        grandtotalElement.textContent = grandtotalFormat;
        return;
    }

    for (let index = 0; index < basket.length; index++) {
        const item = basket[index];
        var itemTotal = 0;
        
        const basketDescription = item.description;
        const basketDescription2 = item.description2;
        const basketImageName = item.imageName;
        const basketQuantity = parseInt(item.quantity);
        const normalPrice = parseInt(item.normalPricePerPiece);
        const offerPrice = parseInt(item.offerPricePerPiece);
        const offerMinimumPieces = parseInt(item.offerMinPieces);
        
        if (basketQuantity < offerMinimumPieces) {
            itemTotal = basketQuantity * normalPrice;
        } else {
            itemTotal = basketQuantity * offerPrice;
        }
        
        grandtotal += itemTotal;

        const itemtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(itemTotal);

        const basketItem = document.createElement("div");
        basketItem.classList.add("basket-item");
        basketItem.innerHTML = `
            <div class="basket-product">
                <img src="images/${basketImageName}">
                <div class="product-text">
                    <p>${basketDescription}</p>
                    <p>${basketDescription2}</p>
                    <p>Pris kr. ${normalPrice} pr. styk - ${offerMinimumPieces} styk for kr. ${offerMinimumPieces * offerPrice} - derudover ${offerPrice} pr. styk</p>
                </div>
            </div>
            <div class="basket-quantity"><input type="number" id="basket-quantity-${index}" value="${basketQuantity}" min="1" data-index="${index}"></div>
            <span class="basket-totalprice">${itemtotalFormat}</span>
            <button class="remove-button" id="remove-button-${index}" data-index="${index}"><i class="ri-close-line"></i></button>
        `;

        basketItemsContainer.appendChild(basketItem);
        removeBasketItem(index);
        updateBasketItem(index);
    };

    const grandtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(grandtotal);
    grandtotalElement.textContent = grandtotalFormat;
}

function removeBasketItem(index) {
    var button = document.getElementById("remove-button-" + index);
    button.addEventListener("click", function () {
        const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
        console.log("Removing item at index:", index);
        basket.splice(index, 1);
        sessionStorage.setItem("basket", JSON.stringify(basket));
        displayBasketItemCount();
        displayBasketItems();
    });
}

function updateBasketItem(index) {
    var input = document.getElementById("basket-quantity-" + index);
    input.addEventListener("change", function () {
        const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
        console.log("Updating item at index:", index);
        basket[index].quantity = parseInt(this.value);
        sessionStorage.setItem("basket", JSON.stringify(basket));
        displayBasketItemCount();
        displayBasketItems();
    });
}

function makeOrder() {
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    if (basket.length === 0) {
        alert("Der er ikke nogen produkter i din indkøbskurv");
        return;
    }

    window.location.href = "makeorder.html"
}

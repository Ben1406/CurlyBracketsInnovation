let body = document.querySelector('body')

function pageLoaded() {
    displayBasketItemCount();
    displayOrder();
    fillAddresses();
}

function showBasket() {
    window.location.href = "basket.html"
}

function displayBasketItemCount() {
    const basketItemCount = document.getElementById("basket-item-count");
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    const basketTotal = basket.reduce((total, item) => total + item.antal, 0);
    basketItemCount.textContent = basketTotal;
}

function displayOrder() {
    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];

    var grandtotal = 0;

    for (let index = 0; index < basket.length; index++) {
        const item = basket[index];
        var itemTotal = 0;
        
        const basketDescription = item.beskrivelse;
        const basketDescription2 = item.beskrivelse2;
        const basketImageName = item.billedenavn;
        const basketQuantity = parseInt(item.antal);
        const normalPrice = parseInt(item.normalprisperstyk);
        const offerPrice = parseInt(item.tilbudsprisperstyk);
        const offerMinimumPieces = parseInt(item.tilbudminstyk);
        
        if (basketQuantity < offerMinimumPieces) {
            itemTotal = basketQuantity * normalPrice;
        } else {
            itemTotal = basketQuantity * offerPrice;
        }
        
        grandtotal += itemTotal;
    }

    const grandtotalElement = document.getElementById("grandtotal");
    const grandtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(grandtotal);
    grandtotalElement.textContent = grandtotalFormat;
}

async function fillAddresses() {
    var addressElements = '';

    await fetch('https://localhost:44373/api/addresses/GetAll')
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        addressElements += `
        <div class="addr-option">${data.betegnelse}</div>
        `
    }))
    .catch(error => console.log(error));

    if (addressElements.length > 0)
    {
        console.log("Addresses found: " + addressElements.length);
        document.querySelector(".addr-options").insertAdjacentHTML('beforeend', addressElements);
        console.log("Addresses ready!");
    }
}

function confirmOrder() {
    //window.location.href = "makeorder.html"
}

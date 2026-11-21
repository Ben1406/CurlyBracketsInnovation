let body = document.querySelector('body');

function pageLoaded() {
    displayBasketItemCount();
    displayOrderItems();
}

function getFetchPrefix()
{
    var prefix = "";
    if (window.location.hostname == "127.0.0.1")
    {
        prefix = "https://localhost:44373";
    }
    return prefix;
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

async function displayOrderItems() {
    const grandtotalElement = document.getElementById("grandtotal");
    const orderItemsContainer = document.querySelector(".order-items");
    const yourOrderNumberElement = document.querySelector(".your-order-number");
    const paymentElement = document.querySelector(".payment");
    const delivery1Element = document.querySelector(".delivery1");
    const delivery2Element = document.querySelector(".delivery2");
    const orderStorage = JSON.parse(sessionStorage.getItem("order")) || [];

    orderItemsContainer.innerHTML = "";

    var grandtotal = 0;
    const orderId = orderStorage[0].ordreId;
    const paymentMethod = orderStorage[0].betalingsmetode;
    const deliveryMethod = orderStorage[0].leveringsmetode;

    if (orderId <= 0) {
        const grandtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(grandtotal);
        grandtotalElement.textContent = grandtotalFormat;
        return;
    }

    yourOrderNumberElement.textContent = "Dit ordrenummer: ' " + orderId + " '";

    await fetch(getFetchPrefix() + '/api/Orders/GetAllOrderLines/' + orderId)
    .then(res => { return res.json(); })
    .then(item => item.forEach(item => {
        const basketDescription = item.produktbeskrivelse;
        const basketDescription2 = item.produktbeskrivelse2;
        const basketImageName = item.billedenavn;
        const basketQuantity = parseInt(item.antal);
        const normalPrice = parseInt(item.normalprisperstyk);
        const offerPrice = parseInt(item.tilbudsprisperstyk);
        const offerMinimumPieces = parseInt(item.tilbudminstyk);
        const totalprice = parseInt(item.totalpris);
        
        if (offerPrice <= 0 || offerMinimumPieces <= 0)
        {
            itemTotal = basketQuantity * normalPrice;
        } else if (basketQuantity < offerMinimumPieces) {
            itemTotal = basketQuantity * normalPrice;
        } else {
            itemTotal = basketQuantity * offerPrice;
        }
        
        grandtotal += totalprice;

        const itemtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(totalprice);

        const orderItem = document.createElement("div");
        orderItem.classList.add("order-item");

        if (offerPrice > 0 && offerMinimumPieces > 0)
        {
            orderItem.innerHTML = `
                <div class="order-product">
                    <div class="product-text">
                        <p>${basketDescription}</p>
                        <p>${basketDescription2}</p>
                        <p>Pris kr. ${normalPrice} pr. styk - ${offerMinimumPieces} styk for kr. ${offerMinimumPieces * offerPrice} - derudover ${offerPrice} pr. styk</p>
                    </div>
                </div>
                <span class="basket-quantity">${basketQuantity}</span>
                <span class="basket-totalprice">${itemtotalFormat}</span>
            `;
        } else {
            orderItem.innerHTML = `
                <div class="order-product">
                    <div class="product-text">
                        <p>${basketDescription}</p>
                        <p>${basketDescription2}</p>
                        <p>Pris kr. ${normalPrice} pr. styk</p>
                    </div>
                </div>
                <span class="basket-quantity">${basketQuantity}</span>
                <span class="basket-totalprice">${itemtotalFormat}</span>
            `;
        }

        orderItemsContainer.appendChild(orderItem);
    }))
    .catch(error => console.log(error));

    const grandtotalFormat = new Intl.NumberFormat("da-DK", { style: "currency", currency: "DKK" }).format(grandtotal);
    grandtotalElement.textContent = grandtotalFormat;

    var paymentText = "";
    if (paymentMethod == "mobilepay") paymentText = "Indbetal " + grandtotalFormat + " med MobilePay på nr. 88345 og skriv ' " + orderId + " ' i besked-feltet.";
    if (paymentMethod == "bank") paymentText = "Overfør " + grandtotalFormat + " til reg.nr. 9070 kontonr. 2021032452 og skriv ' " + orderId + " ' i tekst-til-modtager-feltet." ;
    paymentElement.textContent = paymentText;

    var deliveryText1 = "";
    var deliveryText2 = "";
    if (deliveryMethod == "henter-selv") {
        deliveryText1 = "Du kan hente din bestilling hos Lions Club Vinderup.";
        deliveryText2 = "Vi kontakter dig og aftaler tid og sted.";
    }
    else {
        deliveryText1 = "Vi levere periodevis";
        deliveryText2 = "hvilket typisk betyder at du får din bestilling indenfor 14 dage.";
    }
    delivery1Element.textContent = deliveryText1;
    delivery2Element.textContent = deliveryText2;

    clearSessionStorage();
    sendMail();
}

function clearSessionStorage()
{
    sessionStorage.removeItem("basket");
    displayBasketItemCount();
}

async function sendMail()
{
    const orderStorage = JSON.parse(sessionStorage.getItem("order")) || [];
    const orderId = orderStorage[0].ordreId;

    const sendMail = {
        ordreid: orderId
    };

    const response = await fetch(getFetchPrefix() + '/api/Mail/SendOrderMail', {
        method: "POST",
        body: JSON.stringify(sendMail),
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok)
    {
        console.log("Error: " + response.statusText);
        return false;
    };
}

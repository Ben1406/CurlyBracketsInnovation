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
        
        const basketQuantity = parseInt(item.antal);
        const normalPrice = parseInt(item.normalprisperstyk);
        const offerPrice = parseInt(item.tilbudsprisperstyk);
        const offerMinimumPieces = parseInt(item.tilbudminstyk);
        
        if (offerPrice <= 0 || offerMinimumPieces <= 0)
        {
            itemTotal = basketQuantity * normalPrice;
        } else if (basketQuantity < offerMinimumPieces) {
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

    const addrOptions = document.querySelector(".addr-options");
    const addressInput = document.getElementById("addressInput");

    await fetch('https://localhost:44373/api/addresses/GetAllAddresses')
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        const addrOption = document.createElement("div");
        addrOption.textContent = data.betegnelse;
        addrOption.id = data.id;
        addrOption.classList.add("addr-option");
        addrOption.addEventListener("click", () => {
            addressInput.value = data.betegnelse;
            addrOptions.classList.remove("active");
        })
        addrOptions.appendChild(addrOption);
    }))
    .catch(error => console.log(error));

    if (addressElements.length > 0)
    {
        document.querySelector(".addr-options").insertAdjacentHTML('beforeend', addressElements);
    }
}

function onAddressInputChangedEvent()
{
    const addressOptions = document.querySelector(".addr-options");
    const addressList = document.querySelectorAll(".addr-option");
    const addressInput = document.getElementById("addressInput");
    if (addressInput.value.length < 3)
    {
        addressOptions.classList.remove("active");
    } else {
        addressOptions.classList.add("active");
       
        const searchText = addressInput.value.toLowerCase();
        const searchTextLength = searchText.length;
        addressList.forEach((addrOption) => {
            const addressText = addrOption.textContent.toLowerCase();
            if (addressText.substring(0, searchTextLength) == searchText.substring(0, searchTextLength))
            {
                addrOption.style.display = "flex";
            } else {
                addrOption.style.display = "none";
            }
        });
    }
}

async function confirmOrder() {
    const fullnameInput = document.getElementById("fullnameInput");
    const addressInput = document.getElementById("addressInput");
    const phoneInput = document.getElementById("phoneInput");
    const mailInput = document.getElementById("mailInput");
    const paymentmethodSelect = document.getElementById("paymentmethodSelect");
    const deliverymethodSelect = document.getElementById("deliverymethodSelect");
    const noteInput = document.getElementById("noteInput");

    const fullnameValue = fullnameInput.value ?? "";
    const addressValue = addressInput.value ?? "";
    const phoneValue = phoneInput.value ?? "";
    const mailValue = mailInput.value ?? "";
    const paymentmethodValue = paymentmethodSelect.value ?? "";
    const deliverymethodValue = deliverymethodSelect.value ?? "";
    const noteValue = noteInput.value ?? "";

    if (fullnameValue.length < 2)
    {
        alert("Ugyldigt navn");
        return;
    }

    var addressValid = false;
    const addressList = document.querySelectorAll(".addr-option");
    addressList.forEach((addrOption) => {
        const addressText = addrOption.textContent.toLowerCase();
        if (addressText == addressValue.toLowerCase())
        {
            addressValid = true;
            return;
        };
    });
    if (!addressValid)
    {
        alert("Ugyldigt adresse");
        addressInput.value = "";
        return;
    }

    if (phoneValue.replace(" ", "").length < 8)
    {
        alert("Ugyldigt telefonnummer");
        return;
    }

    if (mailValue.length > 0)
    {
        const validateEmail = mailValue.toLowerCase().match
        (
            /\S+@\S+\.\S+/
        )
        if (mailValue != validateEmail)
        {
            alert("Ugyldig mailadresse");
            return;
        }
    }

    if (paymentmethodValue.length <= 0)
    {
        alert("Mangler valf af betaling");
        return;
    }

    if (deliverymethodValue.length <= 0)
    {
        alert("Mangler valg af levering");
        return;
    }

    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    if (basket.length === 0) {
        alert("Der er ikke nogen produkter i din indkøbskurv");
        return;
    }

    await createOrderInDb(fullnameValue, addressValue, phoneValue, mailValue, paymentmethodValue, deliverymethodValue, noteValue);

    window.location.href = "confirmedOrder.html"
};

async function createOrderInDb(fullname, address, phone, mail, paymentmethod, deliverymethod, note)
{
    var orderid = 0;
    var orderlineid = 0;
    var grandtotal = 0;

    await fetch('https://localhost:44373/api/Orders/GetLastOrderId')
        .then(res => { return res.json(); })
        .then(lastorder => {
            orderid = lastorder.ordreid;
        });

    orderid += 1;

    const basket = JSON.parse(sessionStorage.getItem("basket")) || [];
    for (var index = 0; index < basket.length; index++) {
        const item = basket[index];
        var itemTotal = 0;
        
        const basketProductid = item.id;
        const basketDescription = item.beskrivelse;
        const basketDescription2 = item.beskrivelse2;
        const basketQuantity = parseInt(item.antal);
        const normalPrice = parseInt(item.normalprisperstyk);
        const offerPrice = parseInt(item.tilbudsprisperstyk);
        const offerMinimumPieces = parseInt(item.tilbudminstyk);
        
        if (offerPrice <= 0 || offerMinimumPieces <= 0)
        {
            itemTotal = basketQuantity * normalPrice;
        } else if (basketQuantity < offerMinimumPieces) {
            itemTotal = basketQuantity * normalPrice;
        } else {
            itemTotal = basketQuantity * offerPrice;
        }
        
        grandtotal += itemTotal;
        orderlineid += 1;

        const newOrderLine = {
            ordreid: orderid,
            ordrelinjeid: orderlineid,
            produktid: basketProductid,
            produktbeskrivelse: basketDescription,
            produktbeskrivelse2: basketDescription2,
            antal: basketQuantity,
            normalprisperstyk: normalPrice,
            tilbudsprisperstyk: offerPrice,
            tilbudminstyk: offerMinimumPieces,
            totalpris: itemTotal
        };

        const response = await fetch('https://localhost:44373/api/Orders/SetNewOrderLine', {
            method: "POST",
            body: JSON.stringify(newOrderLine),
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok)
        {
            console.log("Error: " + response.statusText);
            return false;
        }
    }

    const newOrder = {
        ordreid: orderid,
        navn: fullname,
        adresse: address,
        telefon: phone,
        mail: mail,
        leveringsmetode: deliverymethod,
        totalpris: grandtotal,
        betalingsmetode: paymentmethod,
        note: note
    };

    const response = await fetch('https://localhost:44373/api/Orders/SetNewOrder', {
        method: "POST",
        body: JSON.stringify(newOrder),
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok)
    {
        console.log("Error: " + response.statusText);
        return false;
    }

    console.log("Order completed");
}

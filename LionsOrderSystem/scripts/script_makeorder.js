let body = document.querySelector('body')

function pageLoaded() {
    displayBasketItemCount();
    displayPersonFields();
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

function toggleLiveInVinderup(chkbox) {
    displayPersonFields();
}

function displayPersonFields()
{
    const liveInVinderup = document.getElementById("live_in_vinderup");
    const checkedLiveInVinderup = liveInVinderup.checked;
    
    var fieldZipText = document.getElementById('zipText');
    var fieldZipInput = document.getElementById('zipInput');
    var fieldCityText = document.getElementById('cityText');
    var fieldCityInput = document.getElementById('cityInput');
    if (checkedLiveInVinderup) {
        fieldZipText.style.display = 'none';
        fieldZipInput.style.visibility = 'none';
        fieldCityText.style.display = 'none';
        fieldCityInput.style.visibility = 'none';
    } else {
        fieldZipText.style.display = 'flex';
        fieldZipInput.style.visibility = 'flex';
        fieldCityText.style.display = 'flex';
        fieldCityInput.style.visibility = 'flex';
    }
}

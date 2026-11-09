let body = document.querySelector('body')

function pageLoaded() {
    displayBasketItemCount();
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

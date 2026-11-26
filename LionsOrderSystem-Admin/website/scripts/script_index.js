let body = document.querySelector('body')

function pageLoaded() {
    checkIfUserIsLoggedIn();
    showUsername();
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

function openLogin()
{
    sessionStorage.removeItem("login");

    const modalLogin = document.getElementById("modal-login");
    modalLogin.classList.add("active");
    showUsername();
}

function closeLogin()
{
    const modalLogin = document.getElementById("modal-login");
    modalLogin.classList.remove("active");
    showUsername();
}

async function userLogin()
{
    sessionStorage.removeItem("login");
    showUsername();

    var name = "";

    const user = document.getElementById("user");
    const pass = document.getElementById("pass");

    const bruger = {
        initialer: user.value,
        navn: "admin",
        adgangskode: pass.value
    };

    await fetch(getFetchPrefix() + '/api/Users/ValidateUser', {
        method: "POST",
        body: JSON.stringify(bruger),
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(res => { return res.json(); })
    .then(data => data.forEach(data => {
        name = data.navn
    }))
    .catch(error => console.log(error));

    if (name.length == 0)
    {
        alert("Initialer eller adgangskode er forkert");
        showUsername();
        return;
    }

    const login = JSON.parse(sessionStorage.getItem("login")) || [];
    login.push({
        initialer: user.value,
        navn: name
    });
    sessionStorage.setItem("login", JSON.stringify(login));

    const modalLogin = document.getElementById("modal-login");
    modalLogin.classList.remove("active");
    showUsername();
}

function checkIfUserIsLoggedIn()
{
    const login = JSON.parse(sessionStorage.getItem("login")) || [];
    if (login.length > 0 && login[0].navn.length > 0)
    {
        return;
    }
 
    const modalLogin = document.getElementById("modal-login");
    modalLogin.classList.add("active");
}

function showUsername()
{
    const usernameElement = document.getElementById("user-name");
    const login = JSON.parse(sessionStorage.getItem("login")) || [];
    if (login.length > 0 && login[0].navn.length > 0)
    {
        usernameElement.textContent = login[0].navn;
    } else {
        usernameElement.textContent = "";
    }
}

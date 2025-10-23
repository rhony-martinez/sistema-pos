document.addEventListener("DOMContentLoaded", () => {
    const btnConsultar = document.querySelector(".btn-filter");

    if (btnConsultar) {
        btnConsultar.addEventListener("click", () => {
            btnConsultar.innerHTML = "<i class='fas fa-spinner fa-spin'></i> Cargando...";
            btnConsultar.disabled = true;
            setTimeout(() => window.location.href = "consultar_sede.html", 800);
            window.location.href = "consultar_sede.html";
        });
    }
});

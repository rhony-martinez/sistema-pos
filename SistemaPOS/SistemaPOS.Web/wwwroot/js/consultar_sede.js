document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("formBuscar");

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const id = document.getElementById("idSede").value.trim();
        const nombre = document.getElementById("nombreSede").value.trim();

        if (!id && !nombre) {
            alert("Por favor ingresa un ID o un nombre de sede.");
            return;
        }

        const params = new URLSearchParams();
        if (id) params.append("id", id);
        if (nombre) params.append("nombre", nombre);

        const url = `http://localhost:5289/api/Sede/buscar?${params.toString()}`;
        console.log("🌐 Solicitando:", url);

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error(await response.text());
            const sede = await response.json();

            localStorage.setItem("sedeEncontrada", JSON.stringify(sede));
            window.location.href = `consultar_sede_resultado.html?id=${sede.SedeId}`;
        } catch (error) {
            alert("❌ " + error.message);
            console.error(error);
        }
    });
});
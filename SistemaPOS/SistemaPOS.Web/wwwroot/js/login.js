document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector("form");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value.trim();

        if (!username || !password) {
            alert("Por favor, completa todos los campos.");
            return;
        }

        try {
            const response = await fetch("http://localhost:5289/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                alert("Credenciales incorrectas o error en el servidor.");
                return;
            }

            const data = await response.json();
            console.log("Token recibido:", data.accessToken);

            // Guardar token y expiración en sessionStorage
            sessionStorage.setItem("token", data.accessToken);
            sessionStorage.setItem("expiresAt", data.expiresAt);

            // Decodificar token para obtener el rol
            const payload = JSON.parse(atob(data.accessToken.split(".")[1]));
            const userRole = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            // Redirigir según el rol
            switch (userRole) {
                case "ADMIN_GENERAL":
                    window.location.href = "dashboard-adm-gral.html";
                    break;
                case "ADMIN_LOCAL":
                    window.location.href = "dashboard-adm-local.html";
                    break;
                case "CAJERO":
                    window.location.href = "gestion-caja.html";
                    break;
                default:
                    alert("Rol no reconocido.");
            }
        } catch (error) {
            console.error("Error de conexión:", error);
            alert("Error de conexión con el servidor.");
        }
    });
});
﻿document.addEventListener("DOMContentLoaded", () => {
    aplicarValidacionesGlobales(); //  Activa las validaciones universales
    const form = document.querySelector("form");
    const usernameInput = document.getElementById("username");
    const passwordInput = document.getElementById("password");
    const errorMsg = document.getElementById("error-message");
    
    

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const username = usernameInput.value.trim();
        const password = passwordInput.value.trim();


        if (!username || !password) {
            errorMsg.textContent = "Por favor, completa todos los campos.";
            errorMsg.style.display = "block";
            usernameInput.classList.add("input-error");
            passwordInput.classList.add("input-error");
            return;
        }

    
    

        try {
            const response = await fetch(`${API_URL}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                // Mostrar error visual
                errorMsg.textContent = "Usuario o contraseña incorrectos.";
                errorMsg.style.display = "block";
                usernameInput.classList.add("input-error");
                passwordInput.classList.add("input-error");
                return;
            }

            // Si inicia sesión correctamente, limpiamos errores
            usernameInput.classList.remove("input-error");
            passwordInput.classList.remove("input-error");
            errorMsg.style.display = "none";

            const data = await response.json();
            console.log("Token recibido:", data.accessToken);

            sessionStorage.setItem("token", data.accessToken);
            sessionStorage.setItem("expiresAt", data.expiresAt);

            const payload = JSON.parse(atob(data.accessToken.split(".")[1]));
            const userRole = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            // === EXTRAER SEDE DEL TOKEN ===
            // === GUARDAR USUARIO ID ===
            if (payload.uid) {
                sessionStorage.setItem("usuarioId", payload.uid);
                console.log("👤 usuarioId:", payload.uid);
            } else {
                console.warn("⚠️ No se encontró uid en el token.");
            }

            let sedeId = payload["sedeId"] || payload["sedeid"];
            if (sedeId) {
                sessionStorage.setItem("sedeId", sedeId);
                console.log("🏬 Sede del usuario:", sedeId);
            } else {
                console.warn("⚠️ No se encontró sedeId en el token.");
            }

            // 🔹 Obtener perfil completo y guardarlo en localStorage
            try {
                const perfilResponse = await fetch(`${API_URL}/Users/me`, {
                    headers: {
                        "Authorization": `Bearer ${data.accessToken}`,
                        "Content-Type": "application/json"
                    }
                });

                if (perfilResponse.ok) {
                    const perfil = await perfilResponse.json();
                    console.log("✅ Perfil recibido:", perfil);
                    localStorage.setItem("userProfile", JSON.stringify(perfil));
                } else {
                    console.warn("No se pudo obtener el perfil del usuario.");
                }
            } catch (error) {
                console.error("Error obteniendo perfil del usuario:", error);
            }

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
                    errorMsg.textContent = "Rol no reconocido.";
                    errorMsg.style.display = "block";
            }
        } catch (error) {
            console.error("Error de conexión:", error);
            errorMsg.textContent = "Error de conexión con el servidor.";
            errorMsg.style.display = "block";
        }
    });
});
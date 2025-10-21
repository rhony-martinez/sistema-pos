document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const usuario = document.getElementById("usuario").value;
    const contraseña = document.getElementById("contraseña").value;

    try {
        const response = await fetch("http://localhost:5289/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ username, password })
        }),
        });

        if (!response.ok) {
            throw new Error("Error en la autenticación");
        }

        const data = await response.json();
        console.log("✅ Token recibido:", data.token);

        // Guardar token y redirigir
        localStorage.setItem("token", data.token);
        window.location.href = "dashboard.html";
    } catch (error) {
        alert("Error de conexión con el servidor.");
        console.error("❌ Error de conexión:", error);
    }
});

// Verifica si el usuario tiene token y si no ha expirado
function checkAuth() {
    const token = sessionStorage.getItem("token");
    const expiresAt = sessionStorage.getItem("expiresAt");

    if (!token || !expiresAt || new Date(expiresAt) < new Date()) {
        alert("Sesión expirada. Por favor inicia sesión nuevamente.");
        sessionStorage.clear();
        window.location.href = "index.html";
        return false;
    }
    return true;
}

// Agregar encabezado Authorization automáticamente
async function authorizedFetch(url, options = {}) {
    const token = sessionStorage.getItem("token");
    const headers = {
        ...options.headers,
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
    };

    return fetch(url, { ...options, headers });
}

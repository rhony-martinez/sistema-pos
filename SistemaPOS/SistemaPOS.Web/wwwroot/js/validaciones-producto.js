﻿function aplicarValidacionesProducto() {
  const inputsProducto = [
    "proNombre",
    "proDescripcion",
    "proUnidad",
    "proPrecioVenta"
  ];

  const RESERVED = ["SELECT", "INSERT", "UPDATE", "DELETE", "FROM", "WHERE", "GROUP BY", "HAVING", "ORDER BY", "INTO"];
  const MAX_CHARS = 15;
  const MAX_DESC = 70; // nuevo límite para descripción

  inputsProducto.forEach(id => {
    const input = document.getElementById(id);
    if (!input) return;

    if (input.dataset.validacionActiva === "true") return;
    input.dataset.validacionActiva = "true";

    let errorSpan = document.createElement("small");
    errorSpan.classList.add("error-msg");
    errorSpan.style.color = "red";
    errorSpan.style.display = "none";
    errorSpan.style.fontSize = "0.8rem";
    errorSpan.style.marginTop = "2px";

    if (input.tagName === "TEXTAREA") {
      input.parentNode.appendChild(errorSpan);
    } else {
      input.insertAdjacentElement("afterend", errorSpan);
    }

    input.addEventListener("input", (e) => {
      let value = e.target.value;
      let error = "";

      // (1) No espacios al inicio
      if (/^\s/.test(value)) {
        e.target.value = value.trimStart();
        error = "No se permiten espacios al inicio.";
      }

      // (2) Máximo de caracteres
      const limiteActual = id === "proDescripcion" ? MAX_DESC : MAX_CHARS;
      if (value.length > limiteActual) {
        e.target.value = value.slice(0, limiteActual);
        error = `Máximo ${limiteActual} caracteres permitidos.`;
      }

      // (3) No dos espacios seguidos
      if (/\s{2,}/.test(value)) {
        e.target.value = value.replace(/\s{2,}/g, " ");
        error = "No se permiten más de un espacio seguido.";
      }

      // (4) No caracteres especiales
      if (/[^a-zA-Z0-9\s]/.test(value) && id !== "proPrecioVenta") {
        e.target.value = value.replace(/[^a-zA-Z0-9\s]/g, "");
        error = "No se permiten caracteres especiales.";
      }

        // (5) Precio de venta solo numérico con máximo un punto decimal
      // (5) Precio de venta solo numérico con máximo un punto decimal y límite de caracteres
    if (id === "proPrecioVenta") {
    let nuevoValor = value.replace(/[^0-9.]/g, ""); // eliminar letras
    const partes = nuevoValor.split(".");
    if (partes.length > 2) {
        nuevoValor = partes[0] + "." + partes.slice(1).join("");
    }

    // límite de caracteres 
    const MAX_PRECIO = 10;
    if (nuevoValor.length > MAX_PRECIO) {
        nuevoValor = nuevoValor.slice(0, MAX_PRECIO);
        error = `Máximo ${MAX_PRECIO} caracteres permitidos.`;
    }

    if (nuevoValor !== value && !error) {
        error = "Solo se permiten números y un punto decimal.";
    }

    value = nuevoValor;
    e.target.value = value;
}


      // (6) Palabras reservadas SQL
      for (let word of RESERVED) {
        const regex = new RegExp(`\\b${word}\\b`, "i");
        if (regex.test(value)) {
          e.target.value = value.replace(regex, "");
          error = `No se permite la palabra reservada: ${word}`;
          break;
        }
      }

      // Mostrar u ocultar mensaje de error
      if (error) {
        errorSpan.textContent = error;
        errorSpan.style.display = "block";
        e.target.classList.add("input-error");
      } else {
        errorSpan.textContent = "";
        errorSpan.style.display = "none";
        e.target.classList.remove("input-error");
      }
    });
  });
}

--------------------------------------------------------
-- SISTEMA POS - DATOS INICIALES (MS SQL SERVER)
--------------------------------------------------------

-- =====================
-- 1️⃣ SEDE
-- =====================
INSERT INTO SEDE (
    SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD, SEDE_DEPARTAMENTO,
    SEDE_UBICACION, SEDE_TELEFONO, SEDE_CORREO, SEDE_ESTADO
)
VALUES (
    'Sede Central', 'Av. Principal #123', 'Bogotá', 'Cundinamarca',
    'Centro Comercial El Sol, Local 12', '3201234567', 'central@pos.com', 'ACTIVA'
);
INSERT INTO SEDE (
    SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD, SEDE_DEPARTAMENTO,
    SEDE_UBICACION, SEDE_TELEFONO, SEDE_CORREO, SEDE_ESTADO
)
VALUES (
    'Sede Bogotá', 'Av. Principal #123', 'Bogotá', 'Cundinamarca',
    'Centro Comercial El Sol, Local 12', '3201234567', 'central@pos.com', 'ACTIVA'
);

-- =====================
-- 2️⃣ CATEGORÍAS DE PRODUCTO
-- =====================
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Herramientas de Construcción');
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Juguetes');
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Papelería');

-- =====================
-- 3️⃣ PRODUCTOS
-- CAT_ID = 1, 2, 3 según orden anterior
-- =====================
INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Martillo de acero', 'Martillo de carpintero de 16 oz con mango de goma', 25000, 'unidad', 1);

INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Carro de juguete', 'Carro metálico a escala 1:32 con tracción manual', 18000, 'unidad', 2);

INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Cuaderno universitario', 'Cuaderno de 100 hojas cuadriculadas tamaño carta', 8000, 'unidad', 3);

-- =====================
-- 4️⃣ CATALOGO (Sede central ofrece los 3 productos)
-- =====================
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 1);
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 2);
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 3);

-- =====================
-- 5️⃣ USUARIOS (USU_ID MANUAL)
-- =====================
-- ADMIN_GENERAL (puede crear sedes y usuarios de tipo ADMIN_LOCAL)
INSERT INTO USUARIO (
    USU_ID, USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL, USU_ESTADO
) VALUES (
    1001, 'Laura', 'Gómez', 'laura.gomez@pos.com', 'admin', '$2b$12$xr.knbw8qLxpK5wn4jUrdOa6SzlmRL7hvxK6gFxT1N.X59rBNwT3.', 'ADMIN_GENERAL', 'ACTIVO'
);


-- ADMIN_LOCAL (asociado a la sede central)
INSERT INTO USUARIO (
    USU_ID, USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL, SEDE_ID, USU_ESTADO
) VALUES (
    1002, 'Carlos', 'Martínez', 'carlos.martinez@pos.com', 'admin_local', 'clave123', 'ADMIN_LOCAL', 1, 'ACTIVO'
);

-- CAJERO (también en la sede central)
INSERT INTO USUARIO (
    USU_ID, USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL, SEDE_ID, USU_ESTADO
) VALUES (
    1003, 'Ana', 'Rojas', 'ana.rojas@pos.com', 'cajero_1', 'clave123', 'CAJERO', 1, 'ACTIVO'
);

-- =====================
-- 6️⃣ CAJA (asociada a la sede central)
-- =====================
INSERT INTO CAJA (CAJA_FECHA_APERTURA, CAJA_MONTO_INICIAL, SEDE_ID, CAJA_ESTADO)
VALUES (GETDATE(), 200000, 1, 'ABIERTA');

-- =====================
-- 7️⃣ VERIFICACIÓN RÁPIDA
-- =====================
SELECT 'SEDE' AS TABLA, COUNT(*) AS TOTAL FROM SEDE
UNION ALL SELECT 'CATEGORIA_PRODUCTO', COUNT(*) FROM CATEGORIA_PRODUCTO
UNION ALL SELECT 'PRODUCTO', COUNT(*) FROM PRODUCTO
UNION ALL SELECT 'CATALOGO', COUNT(*) FROM CATALOGO
UNION ALL SELECT 'USUARIO', COUNT(*) FROM USUARIO
UNION ALL SELECT 'CAJA', COUNT(*) FROM CAJA;

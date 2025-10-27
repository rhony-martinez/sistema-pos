CREATE OR ALTER PROCEDURE sp_inactivar_sede
    @sede_id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SEDE
    SET SEDE_ESTADO = 'INACTIVA'
    WHERE SEDE_ID = @sede_id AND ISNULL(SEDE_ESTADO, 'ACTIVA') <> 'INACTIVA';

    IF @@ROWCOUNT > 0
        SELECT 'Sede inactivada correctamente.' AS Mensaje;
    ELSE
        SELECT 'La sede ya estaba inactiva o no existe.' AS Mensaje;
END;
GO
